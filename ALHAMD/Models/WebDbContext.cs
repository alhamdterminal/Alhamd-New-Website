using Microsoft.EntityFrameworkCore;

namespace ALHAMD.Models
{
    public class WebDbContext : DbContext
    { // Constructor accepting DbContextOptions to configure the context
        public WebDbContext(DbContextOptions<WebDbContext> options)
            : base(options)
        {
        }

        // DbSet property for the ContactUsTable
        public DbSet<ContactUsTable> ContactUsTables { get; set; }
        public DbSet<ContainerInquiry> ContainerInquiries { get; set; }

        public DbSet<ContainerIndex> ContainerIndex { get; set; }
        public DbSet<CYContainer> CYContainer { get; set; }
        public DbSet<ContainerInfo> ContainerInfos { get; set; }



        // Configuring the model (optional)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
           
            modelBuilder.Entity<ContainerIndex>().HasNoKey(); 
            modelBuilder.Entity<ContainerInfo>().HasNoKey(); 
            modelBuilder.Entity<CYContainer>().HasNoKey();
            modelBuilder.Entity<ContainerInquiry>().HasNoKey();
            modelBuilder.Entity<ContactUsTable>().HasNoKey();
            // Configure the ContactUsTable entity
            modelBuilder.Entity<ContactUsTable>(entity =>
            {
                entity.ToTable("ContactUsTable"); // Table name in the database

                // Optionally configure properties
                entity.Property(e => e.Topic).HasMaxLength(100);
                entity.Property(e => e.First_Name).HasMaxLength(50);
                entity.Property(e => e.Last_Name).HasMaxLength(50);
                entity.Property(e => e.Email_Address).HasMaxLength(100);
                entity.Property(e => e.Phone_Number).HasMaxLength(20);
                entity.Property(e => e.Message).HasMaxLength(500);

                // Additional configuration can be done here if needed
            });
        }
    }
}

