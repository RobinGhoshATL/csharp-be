using Microsoft.Extensions.Configuration;
using SqlAlias;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProSuite.Support.WebAPI.DB
{
    public class Database
    {
        public string _connectionString;
        public Database(IConfiguration configuration)
        {
            _connectionString = Aliases.Map(configuration.GetConnectionString("defaultConnection"));
        }
    }
}
