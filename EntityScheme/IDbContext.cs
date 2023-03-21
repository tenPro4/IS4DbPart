using EntityScheme.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace EntityScheme
{
    public interface IDbContext
    {
        DbSet<Log> Logs { get; set; }
        DbSet<TestTable> TestTable { get; set; }
    }
}