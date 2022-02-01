using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AbetApi.Data;

namespace AbetApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomController : ControllerBase
    {
        //This function is here to run arbitrary code from an endpoint
        //If you make a get request to this endpoint, it will run the code below. Swap this out for whatever you want
        //In postman, I use the URL "https://localhost:44344/Custom" to hit this endpoint.
        [HttpGet]
        public string DoStuff()
        {
            Database database = new Database();
            database.DoStuff();
            return "Done";
        }

        // This function empties database tables defined in ABETDBContext
        [HttpGet("WipeDBTables")]
        public string WipeDatabaseTables()
        {
            Database database = new Database();
            database.WipeTables();
            return "Database tables are empty";
        }

        // This function drops the whole database, as described.
        // This should be used when you make a schema change, and you want the database to be rebuilt with the new schema
        // Drop the table, then run the program again. A database will be built during program startup
        [HttpGet("DropDatabase")]
        public string DropDatabase()
        {
            Database database = new Database();
            database.DropDatabase();
            return "The database is dropped";
        }

    }
}
