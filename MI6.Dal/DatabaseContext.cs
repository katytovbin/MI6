using MI6.Entities;
using Microsoft.EntityFrameworkCore;

namespace MI6.Dal
{
    public class DatabaseContext : DbContext
    {
        public DbSet<MissionEntity> Missions { get; set; }
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }


        public new int SaveChanges()
        {
            return base.SaveChanges();
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<MissionEntity>().ToTable("Missions")
                                .HasKey(m => m.Id)
                                .HasName("PK_Mission")                                ;
        }


    }
}
