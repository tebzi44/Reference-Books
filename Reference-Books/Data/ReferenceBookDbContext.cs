using Microsoft.EntityFrameworkCore;
using Reference_Books.Models.Domein;

namespace Reference_Books.Data
{
    public class ReferenceBookDbContext: DbContext
    {
        public ReferenceBookDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>()
                .HasMany(r => r.Relations)
                .WithOne(p => p.Person)
                .HasForeignKey(r => r.PersonId);


            modelBuilder.Entity<Person>()
                .HasOne(r => r.PhoneNumbers)
                .WithOne(p => p.Person)
                .HasForeignKey<PhoneNumber>(e => e.PersonId);

            base.OnModelCreating(modelBuilder);
        }



        public DbSet<Person> Person { get; set; }
        public DbSet<PhoneNumber> PhoneNumber { get; set; }
        public DbSet<Relation> Relations { get; set; }
    }
}
