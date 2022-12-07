using Microsoft.EntityFrameworkCore;
using PLC_Connect_Test.Model.EntityFramework;
using System;
using System.Configuration;

namespace PLC_Connect_Test.Framework.Database
{
    public partial class PlcDbContext : DbContext
    {
        private string _connectionString = string.Empty;
        public PlcDbContext(DbContextOptions<PlcDbContext> options) : base(options)
        {
        }

        public PlcDbContext()
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = ConfigurationManager.AppSettings["connectionString"];
                optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
                                        mySqlOptions => mySqlOptions.EnableRetryOnFailure(
                                                        maxRetryCount: 10,
                                                        maxRetryDelay: TimeSpan.FromSeconds(2),
                                                        errorNumbersToAdd: null))
                                .EnableDetailedErrors();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        public virtual DbSet<TbPlcInfo> PlcInfos { get; set; }

        public virtual DbSet<TbPlcInfoDtl> PlcInfoDtls { get; set; }
    }
}
