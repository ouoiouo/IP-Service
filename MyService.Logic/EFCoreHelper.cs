using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MyService.Frp;
using System;

namespace MyService.Logic
{
    public class EFCoreHelper : DbContext
    {
        private readonly IConfigurationRoot _configuration;
        private readonly IHostEnvironment _env;

        public EFCoreHelper(IHostEnvironment env)
        {
            _env = env;
            _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        }

        public EFCoreHelper(DbContextOptions<EFCoreHelper> options) : base(options)
        {
        }

        public virtual DbSet<ServiceIpInfo> ServiceIpInfos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer(_configuration.GetConnectionString($"{_env.EnvironmentName}Connection"));
            

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ServiceIpInfo>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Service___3214EC07B88C48FB");

                entity.ToTable("Service_IpInfo");

                entity.Property(e => e.CreateTime).HasColumnType("datetime");
                entity.Property(e => e.IsDelete).HasDefaultValueSql("((0))");
            });
        }
    }
}
