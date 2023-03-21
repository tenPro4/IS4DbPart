using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityConfiguration.Configuration
{
    public class ConnectionStringsConfiguration
    {
        //public string ConfigurationDbConnection { get; set; }

        //public string PersistedGrantDbConnection { get; set; }

        public string DbConnection { get; set; }

        //public string IdentityDbConnection { get; set; }

        //public string AdminAuditLogDbConnection { get; set; }

        public string DataProtectionDbConnection { get; set; }

        public void SetConnections(string commonConnectionString)
        {
            //AdminAuditLogDbConnection = commonConnectionString;
            DbConnection = commonConnectionString;
            //ConfigurationDbConnection = commonConnectionString;
            DataProtectionDbConnection = commonConnectionString;
            //IdentityDbConnection = commonConnectionString;
            //PersistedGrantDbConnection = commonConnectionString;
        }
    }
}
