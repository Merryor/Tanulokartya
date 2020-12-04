using Flashcard.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Flashcard.Services
{
    /// <summary>
    /// This interface belongs to OrderService.
    /// </summary>
    public interface IOrderService
    {
        Task<int> OrderCardNumbers(int deckId);
    }

    /// <summary>
    /// This service class helps to sort the cards within the specified deck.
    /// </summary>
    public class OrderService : IOrderService
    {
        private readonly FlashcardDbContext dbContext;
        public OrderService(FlashcardDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <summary>
        /// This function sorts the specified cards.
        /// </summary>
        public async Task<int> OrderCardNumbers(int deckId)
        {
            var deck = await dbContext.Decks.FindAsync(deckId);
            if (deck == null)
            {
                return 0;
            }

            var cards = await dbContext.Cards.Where(c => c.DeckId == deckId).ToListAsync();
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i].Card_number != i)
                {
                    cards[i].Card_number = i;
                }
            }
            await dbContext.SaveChangesAsync();

            return 1;
        }
    }
}
