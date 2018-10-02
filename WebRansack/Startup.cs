
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace WebRansack
{


    public class Startup
    {

        public Microsoft.Extensions.Configuration.IConfiguration Configuration { get; }


        public Startup(
            Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            Configuration = configuration;
        }


        // This method gets called by the runtime. 
        // Use this method to add services to the container.
        public void ConfigureServices(
            Microsoft.Extensions.DependencyInjection.IServiceCollection services)
        {
            services.AddMvc();
        }


        // This method gets called by the runtime. 
        // Use this method to configure the HTTP request pipeline.
        public void Configure(
            Microsoft.AspNetCore.Builder.IApplicationBuilder app,
            Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }


            app.UseDefaultFiles(
                new Microsoft.AspNetCore.Builder.DefaultFilesOptions()
                {
                    DefaultFileNames = new System.Collections.Generic.List<string>()
                    {
                        "index.htm", "index.html", "slick.htm"
                    }
                }
            );


            app.UseStaticFiles();

            Microsoft.AspNetCore.Builder.WebSocketOptions webSocketOptions = new Microsoft.AspNetCore.Builder.WebSocketOptions()
            {
                KeepAliveInterval = System.TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4 * 1024
            };


            app.UseWebSockets(webSocketOptions);
            app.UseOpenFolderOrFileExtensions("/openfolder");
            app.UseRansack("/ransack");
            

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

        } // End Sub Configure 


    } // End Class 


} // End Namespace 
