using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityConfiguration.Configuration
{
    public class DatabaseMigrationsConfiguration
    {
        public bool ApplyDatabaseMigrations { get; set; } = false;

        //public string ConfigurationDbMigrationsAssembly { get; set; }

        //public string PersistedGrantDbMigrationsAssembly { get; set; }

        public string DbConnectionMigrationsAssembly { get; set; }

        //public string IdentityDbMigrationsAssembly { get; set; }

        //public string AdminAuditLogDbMigrationsAssembly { get; set; }

        public string DataProtectionDbMigrationsAssembly { get; set; }

        public void SetMigrationsAssemblies(string commonMigrationsAssembly)
        {
            //AdminAuditLogDbMigrationsAssembly = commonMigrationsAssembly;
            DbConnectionMigrationsAssembly = commonMigrationsAssembly;
            //ConfigurationDbMigrationsAssembly = commonMigrationsAssembly;
            DataProtectionDbMigrationsAssembly = commonMigrationsAssembly;
            //IdentityDbMigrationsAssembly = commonMigrationsAssembly;
            //PersistedGrantDbMigrationsAssembly = commonMigrationsAssembly;
        }
    }
}
