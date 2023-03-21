using EntityScheme;
using EntityScheme.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityShare.DbContexts
{
    public class MainDbContext: DbContext,IDbContext
    {
        public DbSet<Log> Logs { get; set; }
        public DbSet<TestTable> TestTable { get; set; }

        public MainDbContext(DbContextOptions<MainDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            ConfigureLogContext(builder);
        }

        private void ConfigureLogContext(ModelBuilder builder)
        {
            builder.Entity<Log>(log =>
            {
                log.ToTable("Logs");
                log.HasKey(x => x.Id);
                log.Property(x => x.Level).HasMaxLength(128);
            });

            builder.Entity<TestTable>(log =>
            {
                log.ToTable("TestTable");
                log.HasKey(x => x.Id);
                log.Property(x => x.Name).HasMaxLength(128);
            });
        }
    }
}
