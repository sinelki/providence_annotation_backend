using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Childes.Configuration
{
    public static class Environment
    {
        public static readonly string Prefix = "CHILDES_";
        public static readonly string SQLConnectionString = "SQL_DB_CONNECTION_STRING";
        public static readonly string NoSQLConnectionString = "NOSQL_DB_CONNECTION_STRING";
        public static readonly string TestArg = "test";
    }
}
