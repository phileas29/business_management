using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FM.Domain.Abstractions.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FM.Repository.Context
{
    public class PgSqlHelper : IPgSqlHelper
    {
        public readonly IConfiguration _configuration;
        public string ConnectionString { get; set; }
        public string ProviderName { get; }

        public PgSqlHelper(IConfiguration configuration, DbContextOptionsBuilder optionsBuilder)
        {
            _configuration = configuration;
            ConnectionString = _configuration.GetConnectionString("DefaultConnection");
            ProviderName = "System.Data.SqlClient";

            optionsBuilder.UseNpgsql(ConnectionString);
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        public IDbConnection GetPgSqlContextHelper()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}
