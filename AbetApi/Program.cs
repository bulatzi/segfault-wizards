using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AbetApi.Data;

namespace AbetApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var ctx = new ABETDBContext())
            {
                // This creates a database (described by the ABETDBContext class), if it doesn't already exist
                // Changes to the ABETDBContext class will not apply changes to the existing database.
                // If you don't have real data in the database, pick up changes via dropping your database, and run the program again. It will auto generate.
                ctx.Database.EnsureCreated();
                //AbetApi.Data.Database.WipeTables();
            }
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
