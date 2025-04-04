

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using PawaPayGateway.Domain.Entities;

namespace PawaPayGateway.Infrastructure.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        #region Required

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //User account
            modelBuilder.Entity<Gateway>()
                .HasKey(e => e.Id);
            modelBuilder.Entity<Gateway>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd()
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);


        }

        #endregion Required

        //Set up the Entities
        public DbSet<Gateway> Gateways { get; set; }
    }
}
