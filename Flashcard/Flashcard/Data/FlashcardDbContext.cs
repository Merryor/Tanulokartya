using System.Threading.Tasks;
using Flashcard.Models;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Flashcard.Data
{
    /// <summary>
    /// This class instance represents a session with the database and can be used to query and save instances of your entities.
    /// </summary>
    public class FlashcardDbContext : ApiAuthorizationDbContext<ApplicationUser>, IFlashcardDbContext
    {
        public FlashcardDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        { }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<DeckAssignment> DeckAssignment { get; set; }
        public DbSet<Deck> Decks { get; set; }

        public async Task<int> SaveChangesAsync() => await base.SaveChangesAsync();

        /// <summary>
        /// .This override method to further configure the model that was discovered by convention from the entity types exposed in DbSet<TEntity> properties on your derived context.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>().ToTable("ApplicationUser");
            modelBuilder.Entity<Card>().ToTable("Card");
            modelBuilder.Entity<Comment>().ToTable("Comment");
            modelBuilder.Entity<DeckAssignment>().ToTable("DeckAssignment");
            modelBuilder.Entity<Deck>().ToTable("Deck");

            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(a => a.Email)
                .IsUnique();
            
            modelBuilder.Entity<DeckAssignment>()
                .HasKey(c => new { c.DeckId, c.ApplicationUserId });
        }
    }
}
