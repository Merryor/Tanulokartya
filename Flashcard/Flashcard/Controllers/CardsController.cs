using Flashcard.Data;
using Flashcard.Models;
using Flashcard.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flashcard.Controllers
{
    /// <summary>
    /// This class manages Cards.
    /// Contains all methods for handling cards.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CardsController : ControllerBase
    {

        private readonly ILogger<CardsController> _logger;
        private readonly IFlashcardDbContext _context;

        public CardsController(IFlashcardDbContext context, ILogger<CardsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// This function returns list of cards.
        /// </summary>
        // GET: api/Cards
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CardDTO>>> GetCards()
        {
            _logger.LogInformation("Listing all cards in the GetCards() method");
            return await _context.Cards
                .Select(x => ItemToDTO(x))
                .ToListAsync();
        }

        /// <summary>
        /// This function returns list of cards in the specified deck.
        /// </summary>
        // GET: api/Cards/CardsByDeck/2
        [AllowAnonymous]
        [ActionName("CardsByDeck")]
        [HttpGet("[action]/{deckId}")]
        public async Task<ActionResult<IEnumerable<CardMobileDTO>>> GetCardsById([FromRoute] int deckId)
        {
            _logger.LogInformation("Listing all cards by deckId in the GetCardsById() method");
            var cards = await _context.Cards.Include(c => c.Deck)
                .Select(x => ItemToMobileDTO(x))
                .ToListAsync();

            return cards.OrderBy(c => c.Card_number).Where(c => c.DeckId == deckId).ToList();
        }

        /// <summary>
        /// This function returns the card by ID.
        /// </summary>
        // GET: api/Cards/1
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<CardMobileDTO>> GetCard([FromRoute] int id)
        {
            var card = await _context.Cards.Include(c => c.Deck).SingleOrDefaultAsync(c => c.Id == id);

            if (card == null)
            {
                _logger.LogWarning("Card not found in the GetCard({0}) method", id);
                return NotFound();
            }

            _logger.LogInformation("Getting {0}. card", id);
            return ItemToMobileDTO(card);
        }

        /// <summary>
        /// This function returns the number of cards in the specified deck.
        /// </summary>
        // GET: api/Cards/CountGetCard/2
        [ActionName("CountGetCard")]
        [HttpGet("[action]/{deckId}")]
        public int GetCountCardsById([FromRoute] int deckId)
        {
            _logger.LogInformation("Count cards by deckId in the GetCountCardsById() method");
            var cards = _context.Cards.Include(c => c.Deck).Select(x => ItemToDTO(x)).ToListAsync();
            var selectedCards = cards.Result.Where(c => c.Deck.Id == deckId);

            var countCards = selectedCards.Count();
            return countCards;
        }

        /// <summary>
        /// This function creates the new card.
        /// </summary>
        // POST: api/Cards
        [HttpPost]
        public async Task<ActionResult<CardDTO>> CreateCard([FromBody] CardDTO cardDTO)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("ModelState is invalid in the CreateCard() method");
                return BadRequest(ModelState);
            }
            var card = new Card
            {
                Type = cardDTO.Type,
                Card_number = cardDTO.Card_number,
                Question_text = cardDTO.Question_text,
                Question_picture = cardDTO.Question_picture,
                Answer_text = cardDTO.Answer_text,
                Answer_picture = cardDTO.Answer_picture,
                DeckId = cardDTO.Deck.Id
            };

            _context.Cards.Add(card);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Creating card in the CreateCard() method");

            return CreatedAtAction(nameof(GetCard), new { id = card.Id }, card);
        }

        /// <summary>
        /// This function updates an existing card.
        /// </summary>
        // PUT: api/Cards/2
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCard([FromRoute] int id, [FromBody] CardDTO cardDTO)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("ModelState is invalid in the UpdateCard() method");
                return BadRequest(ModelState);
            }

            var card = await _context.Cards.FindAsync(id);
            if (card == null)
            {
                _logger.LogWarning("Card not found in the UpdateCard({0}) method", id);
                return NotFound();
            }

            card.Type = cardDTO.Type;
            card.Answer_text = cardDTO.Answer_text;
            card.Answer_picture = cardDTO.Answer_picture;
            card.Question_text = cardDTO.Question_text;
            card.Question_picture = cardDTO.Question_picture;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!CardExists(id))
            {
                return NotFound();
            }

            _logger.LogInformation("Updating card in the UpdateCard({0}) method", id);
            return NoContent();
        }

        /// <summary>
        /// This function deletes an existing card.
        /// </summary>
        // DELETE: api/Cards/2
        [Authorize(Roles = "Administrator, Card creator")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCard([FromRoute] int id)
        {
            var card = await _context.Cards.FindAsync(id);

            if (card == null)
            {
                _logger.LogWarning("Card not found in the DeleteCard({0}) method", id);
                return NotFound();
            }

            _context.Cards.Remove(card);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleting card in the DeleteCard({0}) method", id);
            return NoContent();
        }

        /// <summary>
        /// This function deletes an existing card's specified picture.
        /// </summary>
        // PUT: api/Cards/DeleteCardPicture/2/type
        [Authorize(Roles = "Card creator,Lector,Main Lector,Graphic,Main Graphic,Professional reviewer,Main Professional reviewer")]
        [ActionName("DeleteCardPicture")]
        [HttpPut("[action]/{id}/{type}")]
        public async Task<ActionResult> DeleteCardPicture([FromRoute] int id, [FromRoute] string type)
        {
            var card = await _context.Cards.FindAsync(id);

            if (card == null)
            {
                _logger.LogWarning("Card not found in the DeleteCardPicture({0}) method", id);
                return NotFound();
            }

            if (type == "question")
            {
                card.Question_picture = "";
            }
            else if (type == "answer")
            {
                card.Answer_picture = "";
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!CardExists(id))
            {
                return NotFound();
            }

            _logger.LogInformation("Deleting card picture in the DeleteCardPicture({0}) method", id);
            return NoContent();
        }

        #region helperMethods

        /// <summary>
        /// This helper function checks for the existence of the given card based on the ID.
        /// </summary>
        private bool CardExists(int id) =>
         _context.Cards.Any(e => e.Id == id);

        /// <summary>
        /// This helper function converts the card to a DTO.
        /// </summary>
        private static CardDTO ItemToDTO(Card card) =>
        new CardDTO
        {
            Id = card.Id,
            Type = card.Type,
            Card_number = card.Card_number,
            Question_text = card.Question_text,
            Question_picture = card.Question_picture,
            Answer_text = card.Answer_text,
            Answer_picture = card.Answer_picture,
            Deck = new DeckInfoDTO()
            {
                Id = card.Deck.Id,
                Name = card.Deck.Name,
                Module = card.Deck.Module,
                Status = card.Deck.Status,
                Deck_number = card.Deck.Deck_number
            }
        };

        /// <summary>
        /// This helper function converts the card to a DTO.
        /// </summary>
        private static CardMobileDTO ItemToMobileDTO(Card card) =>
        new CardMobileDTO
        {
            Id = card.Id,
            Type = card.Type,
            Card_number = card.Card_number,
            Question_text = card.Question_text,
            Question_picture = card.Question_picture,
            Answer_text = card.Answer_text,
            Answer_picture = card.Answer_picture,
            DeckId = card.Deck.Id
        };

        #endregion
    }
}
