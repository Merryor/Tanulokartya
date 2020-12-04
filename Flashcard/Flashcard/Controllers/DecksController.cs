using Flashcard.Data;
using Flashcard.Models;
using Flashcard.Models.DTO;
using Flashcard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flashcard.Controllers
{
    /// <summary>
    /// This class manages Decks.
    /// Contains all methods for handling decks.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class DecksController : ControllerBase
    {
        private readonly ILogger<DecksController> _logger;
        private readonly IFlashcardDbContext _context;
        private IOrderService orderService;

        public DecksController(IFlashcardDbContext context, IOrderService orderService, ILogger<DecksController> logger)
        {
            _context = context;
            this.orderService = orderService;
            _logger = logger;
        }

        /// <summary>
        /// This function returns list of activated decks.
        /// </summary>
        // GET: api/Decks
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeckMobileDTO>>> GetDecks()
        {
            _logger.LogInformation("Listing all activated decks in the GetDecks() method");
            var decks = await _context.Decks.Include(d => d.ApplicationUser)
                .Select(x => ItemToMobileDTO(x))
                .ToListAsync();

            return decks.Where(d => d.Activated).ToList();
        }

        /// <summary>
        /// This function returns list of all decks.
        /// </summary>
        // GET: api/Decks/GetAllDecks
        [ActionName("GetAllDecks")]
        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<DeckDTO>>> GetAllDecks()
        {
            _logger.LogInformation("Listing all decks in the GetAllDecks() method");
            return await _context.Decks.Include(d => d.ApplicationUser)
                .Select(x => ItemToDTO(x))
                .ToListAsync();
        }

        /// <summary>
        /// This function returns list of decks for the specified user.
        /// </summary>
        // GET: api/Decks/GetMyDecks/userId
        [ActionName("GetMyDecks")]
        [HttpGet("[action]/{userId}")]
        public async Task<ActionResult<IEnumerable<DeckDTO>>> GetMyDecks([FromRoute] string userId)
        {
            _logger.LogInformation("Listing all decks from user in the GetMyDecks() method");
            var decks = await _context.Decks.Include(d => d.ApplicationUser)
                .Select(x => ItemToDTO(x))
                .ToListAsync();

            return decks.Where(x => x.ApplicationUser.Id == userId).ToList();
        }

        /// <summary>
        /// This function returns all packets for the specified module.
        /// </summary>
        // GET: api/Decks/GetDecksByModule/module
        [AllowAnonymous]
        [ActionName("GetDecksByModule")]
        [HttpGet("[action]/{module}")]
        public async Task<ActionResult<IEnumerable<DeckMobileDTO>>> GetDecksByModule([FromRoute] int module, int? deckId = 0)
        {
            _logger.LogInformation("Listing all decks by module in the GetDecksByModule() method");
            var decks = await _context.Decks.Include(d => d.ApplicationUser)
                .Select(x => ItemToMobileDTO(x))
                .ToListAsync();
            
            return decks.Where(d => d.Activated && d.Id > deckId && (int)d.Module == module).ToList();
        }

        /// <summary>
        /// This function returns the number of decks of the specified module increased by one.
        /// </summary>
        // GET: api/Decks/CountGetDeck/2
        [ActionName("CountGetDeck")]
        [HttpGet("[action]/{module}")]
        public int GetCountDecksById([FromRoute] int module)
        {
            _logger.LogInformation("Count decks by module in the GetCountDecksById() method");
            var decks = _context.Decks.Include(d => d.ApplicationUser).Select(x => ItemToDTO(x)).ToListAsync();
            var selectedDecks = decks.Result.Where(c => (int)c.Module == module);

            var countDecks = selectedDecks.Count();
            if (countDecks == 0)
            {
                return 1;
            }
            return countDecks + 1;
        }

        /// <summary>
        /// This function returns list of modules.
        /// </summary>
        // GET: api/Decks/GetModules
        [AllowAnonymous]
        [ActionName("GetModules")]
        [HttpGet("[action]")]
        public ActionResult<List<Module>> GetModules()
        {
            _logger.LogInformation("Listing all modules in the GetModules() method");
            List<Module> modulesList = new List<Module>();
            modulesList.Add(Module.E);
            modulesList.Add(Module.A);
            modulesList.Add(Module.B);
            modulesList.Add(Module.C);
            modulesList.Add(Module.D);
            modulesList.Add(Module.F);
            modulesList.Add(Module.G);
            modulesList.Add(Module.J);
            return modulesList;
        }

        /// <summary>
        /// This function returns the deck by ID.
        /// </summary>
        // GET: api/Decks/1
        [HttpGet("{id}")]
        public async Task<ActionResult<DeckDTO>> GetDeck([FromRoute] int id)
        {
            var deck = await _context.Decks.Include(d => d.ApplicationUser).SingleOrDefaultAsync(c => c.Id == id);

            if (deck == null)
            {
                _logger.LogWarning("Deck not found in the GetDeck({0}) method", id);
                return NotFound();
            }

            _logger.LogInformation("Getting {0}. deck", id);
            return ItemToDTO(deck);
        }

        /// <summary>
        /// This function creates the new deck.
        /// </summary>
        // POST: api/Decks
        [HttpPost]
        public async Task<ActionResult<DeckDTO>> CreateDeck([FromBody] DeckDTO deckDTO)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("ModelState is invalid in the CreateDeck() method");
                return BadRequest(ModelState);
            }
            var deck = new Deck
            {
                Name = deckDTO.Name,
                Content = deckDTO.Content,
                Module = deckDTO.Module,
                Deck_number = deckDTO.Deck_number,
                Activated = false,
                Status = DeckStatus.Initial,
                ApplicationUserId = deckDTO.ApplicationUser.Id
            };

            _context.Decks.Add(deck);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Creating deck in the CreateDeck() method");

            return CreatedAtAction(nameof(GetDeck), new { id = deck.Id }, deck);
        }

        /// <summary>
        /// This function updates an existing deck.
        /// </summary>
        // PUT: api/Decks/2
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDeck([FromRoute] int id, [FromBody] DeckDTO deckDTO)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("ModelState is invalid in the UpdateDeck() method");
                return BadRequest(ModelState);
            }

            var deck = await _context.Decks.FindAsync(id);
            if (deck == null)
            {
                _logger.LogWarning("Deck not found in the UpdateDeck({0}) method", id);
                return NotFound();
            }

            deck.Name = deckDTO.Name;
            deck.Content = deckDTO.Content;
            deck.Module = deckDTO.Module;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!DeckExists(id))
            {
                return NotFound();
            }

            _logger.LogInformation("Updating deck in the UpdateDeck({0}) method", id);
            return NoContent();
        }

        /// <summary>
        /// This function updates the activated attribute an existing deck.
        /// </summary>
        // PUT: api/Decks/UpdateActivate/2
        [Authorize(Roles = "Administrator")]
        [ActionName("UpdateActivate")]
        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> UpdateActivateDeck([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("ModelState is invalid in the UpdateActivateDeck() method");
                return BadRequest(ModelState);
            }

            var deck = await _context.Decks.FindAsync(id);
            if (deck == null)
            {
                _logger.LogWarning("Deck not found in the UpdateActivateDeck({0}) method", id);
                return NotFound();
            }

            deck.Activated = !deck.Activated;
            if (deck.Activated)
            {
                deck.Activation_time = DateTime.Now;
            }            

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!DeckExists(id))
            {
                return NotFound();
            }

            _logger.LogInformation("Updating deck in the UpdateActivateDeck({0}) method", id);
            return NoContent();
        }

        /// <summary>
        /// This function updates an existing deck state.
        /// </summary>
        // PUT: api/Decks/UpdateState/2/Initial
        [Authorize]
        [ActionName("UpdateState")]
        [HttpPut("[action]/{id}/{state}")]
        public async Task<IActionResult> UpdateStateDeck([FromRoute] int id, [FromRoute] int state)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("ModelState is invalid in the UpdateStateDeck() method");
                return BadRequest(ModelState);
            }

            var deck = await _context.Decks.FindAsync(id);
            if (deck == null)
            {
                _logger.LogWarning("Deck not found in the UpdateStateDeck({0}) method", id);
                return NotFound();
            }

            deck.Status = (DeckStatus)state;
            if (state == 4)
            {
                deck.Activated = true;
                deck.Activation_time = DateTime.Now;
                var result = await orderService.OrderCardNumbers(id);
                if (result == 0)
                {
                    return NotFound();
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!DeckExists(id))
            {
                return NotFound();
            }

            _logger.LogInformation("Updating deck in the UpdateStateDeck({0}) method", id);
            return NoContent();
        }

        #region helperMethods

        /// <summary>
        /// This helper function checks for the existence of the given deck based on the ID.
        /// </summary>
        private bool DeckExists(int id) =>
         _context.Decks.Any(e => e.Id == id);

        /// <summary>
        /// This helper function converts the deck to a DTO.
        /// </summary>
        private static DeckDTO ItemToDTO(Deck deck) =>
        new DeckDTO
        {
            Id = deck.Id,
            Name = deck.Name,
            Content = deck.Content,
            Module = deck.Module,
            Deck_number = deck.Deck_number,
            Activated = deck.Activated,
            Activation_time = deck.Activation_time,
            Status = deck.Status,
            ApplicationUser = new ApplicationUserDTO()
            {
                Id = deck.ApplicationUser.Id,
                Name = deck.ApplicationUser.Name
            }
        };

        /// <summary>
        /// This helper function converts the deck to a DTO.
        /// </summary>
        private static DeckMobileDTO ItemToMobileDTO(Deck deck) =>
        new DeckMobileDTO
        {
            Id = deck.Id,
            Name = deck.Name,
            Content = deck.Content,
            Module = deck.Module,
            Deck_number = deck.Deck_number,
            Activated = deck.Activated,
            Activation_time = deck.Activation_time,
            Status = deck.Status,
            DeckAuthor = deck.ApplicationUser.Name
        };

        #endregion
    }
}
