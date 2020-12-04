using DinkToPdf;
using DinkToPdf.Contracts;
using Flashcard.Data;
using Flashcard.Models;
using Flashcard.Models.DTO;
using Flashcard.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Flashcard.Controllers
{
    /// <summary>
    /// This function handles pdf generation.
    /// </summary>
    [Route("api/pdfcreator")]
    [ApiController]
    public class PdfCreatorController : ControllerBase
    {
        private IConverter _converter;
        private readonly FlashcardDbContext _context;
        private readonly ILogger<PdfCreatorController> _logger;

        public PdfCreatorController(IConverter converter, FlashcardDbContext context, ILogger<PdfCreatorController> logger)
        {
            _converter = converter;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// This function returns list of cards in the specified deck.
        /// </summary>
        public async Task<List<CardDTO>> getCards(int deckId)
        {
            _logger.LogInformation("Listing all cards by deckId in the GetCards() method");
            var cards = await _context.Cards.Include(c => c.Deck)
                .Select(x => ItemToDTO(x))
                .ToListAsync();

            return cards.OrderBy(c => c.Card_number).Where(c => c.Deck.Id == deckId).ToList();
        }

        /// <summary>
        /// This function generates PDF from HTML.
        /// </summary>
        // GET: api/pdfcreator/1
        [HttpGet("{deckId}")]
        public IActionResult CreatePDF([FromRoute] int deckId)
        {
            var cardList = getCards(deckId).Result;

            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings( 5, 5, 5, 5 ),
                DocumentTitle = "PDF Report"
            };

            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = TemplateGenerator.GetHTMLString(cardList),
                WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "assets", "styles.css") },
            };

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };

            var file = _converter.Convert(pdf);

            _logger.LogInformation("Generating PDF in the CreatePDF() method");
            return File(file, "application/pdf");
        }

        #region helperMethods

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

        #endregion
    }
}