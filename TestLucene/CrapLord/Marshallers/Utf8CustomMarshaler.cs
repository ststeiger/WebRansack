
namespace TestLucene.CrapLord
{
    
    
    /// <summary>
    /// Marshaller for UTF8 strings.
    /// </summary>
    public class Utf8CustomMarshaler
        : System.Runtime.InteropServices.ICustomMarshaler
    {
        private static readonly Utf8CustomMarshaler s_staticInstance;
        private static readonly System.Action<int> s_setLastWin32Error;
        
        
        private static System.Action<int> CreateSetter(System.Reflection.MethodInfo mtd)
        {
            // Object's type:
            System.Linq.Expressions.ParameterExpression valueExp = System.Linq.Expressions.Expression.Parameter(typeof(int), "error");
            // System.Linq.Expressions.Expression[] prams = new System.Linq.Expressions.Expression[] {valueExp};
            System.Linq.Expressions.ParameterExpression[] prams2 = new System.Linq.Expressions.ParameterExpression[] {valueExp};
            System.Linq.Expressions.Expression methodCall = System.Linq.Expressions.Expression.Call(null, mtd, prams2);
            
            System.Action<int> invoker = System.Linq.Expressions.Expression.Lambda<System.Action<int>>(methodCall, false, prams2).Compile();
            return invoker;
        }
        
        
        static Utf8CustomMarshaler()
        {
            System.Type t = typeof(System.Runtime.InteropServices.Marshal);
            System.Reflection.MethodInfo mi = t.GetMethod("SetLastWin32Error", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            // mi.Invoke(null, new object[] { (object)lastError });
            s_setLastWin32Error = CreateSetter(mi);
            
            s_staticInstance = new Utf8CustomMarshaler();
        }



        System.IntPtr System.Runtime.InteropServices.ICustomMarshaler.MarshalManagedToNative(object objManagedObj)
        {
            string managedObj = objManagedObj as string;
            
            if (managedObj == null)
                return System.IntPtr.Zero;
            
            // not null terminated
            byte[] strbuf = System.Text.Encoding.UTF8.GetBytes(managedObj);
            
            System.IntPtr buffer = System.Runtime.InteropServices.Marshal.AllocHGlobal(strbuf.Length + 1);
            System.Runtime.InteropServices.Marshal.Copy(strbuf, 0, buffer, strbuf.Length);

            // write the terminating null
            //Marshal.WriteByte(buffer + strbuf.Length, 0);

            long lngPosEnd = buffer.ToInt64() + strbuf.Length;
            System.IntPtr ptrPosEnd = new System.IntPtr(lngPosEnd);
            System.Runtime.InteropServices.Marshal.WriteByte(ptrPosEnd, 0);

            return buffer;
        } // End Function MarshalManagedToNative 
        
        
        /*
        unsafe object System.Runtime.InteropServices.ICustomMarshaler.MarshalNativeToManaged(System.IntPtr pNativeData)
        {
            byte* walk = (byte*)pNativeData;

            // find the end of the string
            while (*walk != 0)
            {
                walk++;
            }
            int length = (int)(walk - (byte*)pNativeData);

            // should not be null terminated
            //byte[] strbuf = new byte[length - 1];
            byte[] strbuf = new byte[length];
            
            // skip the trailing null
            //Marshal.Copy(pNativeData, strbuf, 0, length - 1);
            System.Runtime.InteropServices.Marshal.Copy(pNativeData, strbuf, 0, length);
            
            string data = System.Text.Encoding.UTF8.GetString(strbuf);
            return data;
        } // End Function MarshalNativeToManaged 
        */
        
        
        object System.Runtime.InteropServices.ICustomMarshaler.MarshalNativeToManaged(System.IntPtr pNativeData)
        {
            int i = 0;
            while (System.Runtime.InteropServices.Marshal.ReadByte(pNativeData, i) != 0)
            {
                ++i;
            } // Whend 
            
            byte[] ba = new byte[i];
            System.Runtime.InteropServices.Marshal.Copy(pNativeData, ba, 0, i);
            string str = System.Text.Encoding.UTF8.GetString(ba);
            
            // System.Console.WriteLine(str);
            return str;
            // return Instance.MarshalNativeToManaged(pNativeData);
        }
        
        
        public static T2[] MapArray<T1, T2>(T1[] array, System.Func<T1, T2> map)
        {
            T2[] newArray = new T2[array.Length];
            
            for (int i = 0; i < array.Length; ++i)
            {
                newArray[i] = map(array[i]);
            } // Next i 
            
            return newArray;
        } // End Function MapArray 
        
        
        public static System.Collections.Generic.List<T2> MapList<T1, T2>(System.Collections.Generic.IEnumerable<T1> array, System.Func<T1, T2> map)
        {
            System.Collections.Generic.List<T2> newList = new System.Collections.Generic.List<T2>();
            
            foreach (T1 thisElement in array)
            {
                newList.Add(map(thisElement));
            } // Next 
            
            return newList;
        } // End Function MapList 


        public static System.Delegate MakeCompiledMethod(System.Reflection.MethodInfo mtd)
        {
            if (mtd == null)
                throw new System.ArgumentNullException("mtd mustn't be null");

            System.Reflection.ParameterInfo[] pis = mtd.GetParameters();
            // System.Linq.Expressions.ParameterExpression[] prams = MapArray(pis,pi=> System.Linq.Expressions.Expression.Parameter(pi.ParameterType, pi.Name));
            System.Collections.Generic.List<System.Linq.Expressions.ParameterExpression> prams = MapList(pis,
                pi => System.Linq.Expressions.Expression.Parameter(pi.ParameterType, pi.Name));
            
            //var prams = mtd.GetParameters().Select(p => System.Linq.Expressions.Expression.Parameter(p.ParameterType, p.Name)).ToList();
            System.Linq.Expressions.Expression methodCall;
            if (mtd.IsStatic) 
                methodCall = System.Linq.Expressions.Expression.Call(null, mtd, prams);
            else
            {
                // on instance-Methods the ownerInstance must be included
                System.Linq.Expressions.ParameterExpression ownerInstance = System.Linq.Expressions.Expression.Variable(mtd.DeclaringType, "ownerInstance");
                methodCall = System.Linq.Expressions.Expression.Call(ownerInstance, mtd, prams);
                prams.Insert(0, ownerInstance);
            }
            
            return System.Linq.Expressions.Expression.Lambda(methodCall, false, prams).Compile();
        } // End Function MakeCompiledMethod 
        
        
        void System.Runtime.InteropServices.ICustomMarshaler.CleanUpNativeData(System.IntPtr pNativeData)
        {
            int lastError = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
            System.Runtime.InteropServices.Marshal.FreeHGlobal(pNativeData);
            s_setLastWin32Error(lastError);
        } // End Function CleanUpNativeData 
        
        
        void System.Runtime.InteropServices.ICustomMarshaler.CleanUpManagedData(object pNativeData)
        { }
        
        
        int System.Runtime.InteropServices.ICustomMarshaler.GetNativeDataSize()
        {
            return System.IntPtr.Size;
        } // End Function GetNativeDataSize 
        
        
        // This is required for CustomMarshal apart from implementing the interface ! 
        public static System.Runtime.InteropServices.ICustomMarshaler GetInstance(string cookie)
        {
            return s_staticInstance;
        } // End Function GetInstance 
        
        
    } // End Class Utf8CustomMarshaler 
    
    
} // End Namespace 
