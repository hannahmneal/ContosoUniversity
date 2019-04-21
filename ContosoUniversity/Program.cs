using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
//NOTE: Add the following two using statements in order to recognize both SchoolContext and DbInitializer:
using Microsoft.Extensions.DependencyInjection;
using ContosoUniversity.Data;

namespace ContosoUniversity
{
    public class Program
    {
        /*
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
    */
    //NOTE: The code above was the boilerplate code on project set up. Below is the modified Main method that allows the seed method in DbInitializer to run; The seed method checks the database for a Student and if none exists, it creates one.

            //NOTE: IMPORTANT:  Older tutorials use similar code in Configure method in Startup.cs, this code here is Application startup code. Application startup code belongs in the Main method, while the Configure method is only for setting up the request pipeline.

        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<SchoolContext>();
                    DbInitializer.Initialize(context);
                    //NOTE: Microsoft.Extensions.DependencyInjection and ContosoUniversity.Data are used to recognize 
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}

//NOTE, Now, with the Configure method in Startup.cs, the seed method in DbInitializer.cs, and the Main method here, when the application is run for the first time the database will be created and seeded with test data. Whenever you change your data model, you can delete the database, update your seed method, and start afresh with a new database the same way.
