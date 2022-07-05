
// using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting; // for UseStartup
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.Logging;


namespace WebRansack
{


    public class Program
    {


        // https://www.heroku.com/free
        // https://medium.com/@AndreyAzimov/how-free-heroku-really-works-and-how-to-get-maximum-from-it-daa53f2b3c57

        public static void GetProcesses()
        {
            // https://cloud.google.com/appengine/docs/flexible/dotnet/quickstart
            // https://codelabs.developers.google.com/codelabs/cloud-app-engine-aspnetcore/#0
            // https://www.hanselman.com/blog/TryingASPNETCoreOnTheGoogleCloudPlatformAppEngineFlexibleEnvironment.aspx
            // https://cloud.google.com/free/

            System.Diagnostics.Process[] pls = System.Diagnostics.Process.GetProcesses();
        }


        // https://blog.discountasp.net/reducing-net-core-memory-usage/
        // <PropertyGroup>
        //    <TargetFramework>netcoreapp2.0</TargetFramework>
        //    <TypeScriptToolsVersion>2.5</TypeScriptToolsVersion>
        //    <ServerGarbageCollection>false</ServerGarbageCollection>
        // </PropertyGroup>
        public static async System.Threading.Tasks.Task GetProcess(System.Diagnostics.Process proc)
        {
            // https://stackoverflow.com/questions/47656988/viewing-memory-usage-stats-of-a-dotnetcore-2-self-contained-application-on-linux
            long currentMemoryUsage = proc.WorkingSet64;
            long peakPhysicalMemoryUsage = proc.PeakWorkingSet64;

            currentMemoryUsage /= 1048576; // 1048576 = 1024^2 = MB
            peakPhysicalMemoryUsage /= 1048576; // 1048576 = 1024^2 = MB

            System.Console.WriteLine("Current: " +
                                     currentMemoryUsage.ToString(System.Globalization.CultureInfo.InvariantCulture) +
                                     "MB");
            System.Console.WriteLine(
                "Peak: " + peakPhysicalMemoryUsage.ToString(System.Globalization.CultureInfo.InvariantCulture) + "MB");

            await System.Threading.Tasks.Task.CompletedTask;
        }


        public static async System.Threading.Tasks.Task PeriodicTask(System.TimeSpan interval,
            System.Threading.CancellationToken cancellationToken)
        {
            using (System.Diagnostics.Process proc = System.Diagnostics.Process.GetCurrentProcess())
            {

                while (true)
                {
                    await GetProcess(proc);
                    await System.Threading.Tasks.Task.Delay(interval, cancellationToken);
                }

            }
        }


        public static async System.Threading.Tasks.Task PeriodicTask(int interval,
            System.Threading.CancellationToken cancellationToken)
        {
            System.TimeSpan ts = new System.TimeSpan(0, 0, interval);
            await PeriodicTask(ts, cancellationToken);
        }


        public static void Main(string[] args)
        {
            // _ = PeriodicTask(10, System.Threading.CancellationToken.None);
            BuildWebHost(args).Run();
        }
        
        
        public static Microsoft.AspNetCore.Hosting.IWebHost BuildWebHost(string[] args)
        {
            return Microsoft.AspNetCore.WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
        }
        
        
    } // End Class Program 
    
    
} // End Namespace WebRansack 
