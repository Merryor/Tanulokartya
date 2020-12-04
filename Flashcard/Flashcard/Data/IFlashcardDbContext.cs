using Flashcard.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Flashcard.Data
{
    /// <summary>
    /// This interface belongs to the FlashcardDbContext.
    /// </summary>
    public interface IFlashcardDbContext
    {
        DbSet<ApplicationUser> ApplicationUsers { get; set; }
        DbSet<Card> Cards { get; set; }
        DbSet<Comment> Comments { get; set; }
        DbSet<DeckAssignment> DeckAssignment { get; set; }
        DbSet<Deck> Decks { get; set; }

        Task<int> SaveChangesAsync();
    }
}
