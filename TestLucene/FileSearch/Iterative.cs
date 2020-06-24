
namespace TestLucene.FileSearch
{


    class Iterative
    {

        //Iterative File and Folder Listing in VB.NET
        public static bool IterativeSearch2(string strPath)
        {
            System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(strPath);
            System.IO.FileSystemInfo[] arrfsiEntities = null;
            arrfsiEntities = dirInfo.GetFileSystemInfos();


            // Creates and initializes a new Stack.
            System.Collections.Stack strLastPathStack = new System.Collections.Stack();
            System.Collections.Stack iIndexStack = new System.Collections.Stack();

            int iIndex = 0;
            int iMaxEntities = arrfsiEntities.Length;
            do
            {
                while (iIndex < iMaxEntities)
                {

                    if (arrfsiEntities[iIndex].Attributes == System.IO.FileAttributes.Directory)
                    {
                        //Console.WriteLine("Searching directory " + arrfsiEntities[iIndex].FullName);

                        strLastPathStack.Push(System.IO.Directory.GetParent(arrfsiEntities[iIndex].FullName).FullName);
                        strLastPathStack.Push(arrfsiEntities[iIndex].FullName);
                        iIndexStack.Push(iIndex);

                        dirInfo = null;
                        System.Array.Clear(arrfsiEntities, 0, arrfsiEntities.Length);
                        dirInfo = new System.IO.DirectoryInfo(strLastPathStack.Pop().ToString());
                        arrfsiEntities = dirInfo.GetFileSystemInfos();

                        iIndex = 0;
                        iMaxEntities = arrfsiEntities.Length;
                        continue;
                    }
                    else
                    {
                        //Console.WriteLine(arrfsiEntities[iIndex].FullName);
                    }

                    iIndex += 1;
                } // Whend


                dirInfo = null;
                System.Array.Clear(arrfsiEntities, 0, arrfsiEntities.Length);
                // Dont try to do move the next line in the loop while/until statement, null reference when pop on an empty stack...
                if (strLastPathStack.Count == 0)
                    break;

                dirInfo = new System.IO.DirectoryInfo(strLastPathStack.Pop().ToString());
                arrfsiEntities = dirInfo.GetFileSystemInfos();

                iIndex = (int)iIndexStack.Pop() + 1;
                iMaxEntities = arrfsiEntities.Length;
            } // End do
            while (true);

            return true;
        } // End Function IterativeSearch2


        public static bool IterativeSearch1(string path)
        {

            System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(path);
            System.IO.FileSystemInfo[] arrfsiEntities = null;
            arrfsiEntities = dirInfo.GetFileSystemInfos();


            // Creates and initializes a new Stack.
            System.Collections.Stack myStack = new System.Collections.Stack();
            //Console.WriteLine("Stack is empty when \t stack.count={0}", myStack.Count);


            int iIndex = 0;
            int iMaxEntities = arrfsiEntities.Length - 1;

            do
            {
                for (iIndex = 0; iIndex <= iMaxEntities; iIndex += 1)
                {
                    if (arrfsiEntities[iIndex].Attributes == System.IO.FileAttributes.Directory)
                    {
                        //Console.WriteLine("Searching directory " + arrfsiEntities[iIndex].FullName);
                        myStack.Push(arrfsiEntities[iIndex].FullName);
                    }
                    else
                    {
                        //Console.WriteLine("{0}", arrfsiEntities[iIndex].FullName);
                    }
                } // Next iIndex

                dirInfo = null;
                System.Array.Clear(arrfsiEntities, 0, arrfsiEntities.Length);
                // Dont try to do move the next line in the loop while/until statement, null reference when pop on an empty stack...
                if (myStack.Count == 0)
                    break;

                dirInfo = new System.IO.DirectoryInfo(myStack.Pop().ToString());
                arrfsiEntities = dirInfo.GetFileSystemInfos();

                iIndex = 0;
                iMaxEntities = arrfsiEntities.Length - 1;
            }
            while (true);

            return true;
        } // End Function IterativeSearch1


    }


}
