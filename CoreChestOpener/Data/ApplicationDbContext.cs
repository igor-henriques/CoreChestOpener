using CoreChestOpener.License;
using Microsoft.EntityFrameworkCore;
using System;

namespace CoreChestOpener.Data
{
    public class ApplicationDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            try
            {
                optionsBuilder.UseMySql(Environment.GetEnvironmentVariable("Connection"));
                base.OnConfiguring(optionsBuilder);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public DbSet<CoreLicense> CoreLicense { get; set; }
    }
}
