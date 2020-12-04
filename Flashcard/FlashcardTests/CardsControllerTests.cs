using Flashcard.Controllers;
using Flashcard.Data;
using Flashcard.Models;
using Flashcard.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FlashcardTests
{
    /// <summary>
    /// This class implements the unit tests associated with the CardsController.
    /// </summary>
    public class CardsControllerTests
    {
        private readonly Mock<IFlashcardDbContext> flashcardDbContextMock;
        private readonly ILogger<CardsController> logger;

        public CardsControllerTests()
        {
            var mockLogger = new Mock<ILogger<CardsController>>();
            logger = mockLogger.Object;
            flashcardDbContextMock = new Mock<IFlashcardDbContext>();

            var decks = new List<Deck>
            {
                new Deck
                {
                    Id = 1,
                    Name = "Teszt csomag",
                    Content = "Teszt csomag tartalma",
                    Module = Module.A,
                    Deck_number = 1,
                    Activated = false,
                    Activation_time = DateTime.Parse("2020-10-29"),
                    Status = DeckStatus.Approved
                },

            };
            var cards = new List<Card>
            {
                new Card {
                    Id = 1,
                    Type = "Teszt",
                    Card_number = 1,
                    Question_text = "Tesztkerdes",
                    Question_picture = "kep1",
                    Answer_text = "Teszt valasz",
                    Answer_picture = "kep 1",
                    Deck = new Deck
                    {
                        Id = 1,
                        Name = "Teszt csomag",
                        Content = "Teszt csomag tartalma",
                        Module = Module.A,
                        Deck_number = 1,
                        Activated = false,
                        Activation_time = DateTime.Parse("2020-10-29"),
                        Status = DeckStatus.Approved
                    }
                },
                new Card {
                    Id = 2,
                    Type = "Teszt2",
                    Card_number = 2,
                    Question_text = "Tesztkerdes 2",
                    Question_picture = "kep2",
                    Answer_text = "Teszt valasz 2",
                    Answer_picture = "kep 2",
                    Deck = new Deck
                    {
                        Id = 1,
                        Name = "Teszt csomag",
                        Content = "Teszt csomag tartalma",
                        Module = Module.A,
                        Deck_number = 1,
                        Activated = false,
                        Activation_time = DateTime.Parse("2020-10-29"),
                        Status = DeckStatus.Approved
                    }
                }
            };

            var deckDbSet = decks.AsQueryable().BuildMockDbSet();
            flashcardDbContextMock.Setup(x => x.Decks).Returns(deckDbSet.Object);

            var cardDbSet = cards.AsQueryable().BuildMockDbSet();
            flashcardDbContextMock.Setup(x => x.Cards).Returns(cardDbSet.Object);
        }

        [Fact]
        public async Task GetCards_ReturnsAllCards()
        {
            // Arrange
            var controller = new CardsController(flashcardDbContextMock.Object, logger);

            // Act
            var result = await controller.GetCards();

            // Asssert
            var cards = Assert.IsType<List<CardDTO>>(result.Value);
            Assert.Equal(2, cards.Count);
        }

        [Fact]
        public async Task GetCardsById_ReturnsAllCards()
        {
            // Arrange
            var controller = new CardsController(flashcardDbContextMock.Object, logger);
            int deckId = 1;

            // Act
            var result = await controller.GetCardsById(deckId);

            // Asssert
            var cards = Assert.IsType<List<CardMobileDTO>>(result.Value);
            Assert.Equal(2, cards.Count);
        }

        [Fact]
        public async Task GetCard_ExistingIdPassed_ReturnsRightCard()
        {
            // Arrange
            var controller = new CardsController(flashcardDbContextMock.Object, logger);
            int existingCardId = 2;

            // Act
            var result = await controller.GetCard(existingCardId);

            // Asssert
            Assert.Equal(2, result.Value.Id);
        }

        [Fact]
        public async Task GetCard_UnknownIdPassed_ReturnsNotFoundResult()
        {
            // Arrange
            var controller = new CardsController(flashcardDbContextMock.Object, logger);
            int unknownCardId = 5;

            // Act
            var result = await controller.GetCard(unknownCardId);

            // Asssert
            Assert.IsType<NotFoundResult>(result.Result);
        }

     
        [Fact]
        public async Task CreateCard_InvalidObjectPassed_ReturnsBadRequest()
        {
            // Arrange
            var controller = new CardsController(flashcardDbContextMock.Object, logger);
            var typeMissingCard = new CardDTO
            {
                Card_number = 3,
                Question_text = "Teszt kartya kerdes",
                Question_picture = "kep.jpg",
                Answer_text = "Teszt kartya valasz",
                Answer_picture = "kep2.jpg",
            };
            controller.ModelState.AddModelError("Type", "Required");

            // Act
            var result = await controller.CreateCard(typeMissingCard);

            // Asssert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateCard_RightObjectPassed_ReturnsCreatedAtAction()
        {
            // Arrange
            var controller = new CardsController(flashcardDbContextMock.Object, logger);
            var rightCard = new CardDTO
            {
                Type = "Teszt tipus",
                Card_number = 3,
                Question_text = "Teszt kartya kerdes",
                Question_picture = "kep.jpg",
                Answer_text = "Teszt kartya valasz",
                Answer_picture = "kep2.jpg",
                Deck = new DeckInfoDTO
                {
                    Id = 1,
                    Name = "Teszt csomag",
                    Module = Module.A,
                    Deck_number = 1,
                    Status = DeckStatus.Approved
                }
            };

            // Act
            var result = await controller.CreateCard(rightCard);

            // Asssert
            Assert.IsType<CreatedAtActionResult>(result.Result);
        }

        [Fact]
        public async Task UpdateCard_UnknownIdPassed_ReturnsNotFound()
        {
            // Arrange
            var controller = new CardsController(flashcardDbContextMock.Object, logger);
            int unknownId = 15;
            var rightCard = new CardDTO
            {
                Id = 15,
                Type = "Teszt tipus update",
                Card_number = 3,
                Question_text = "Teszt kartya kerdes",
                Question_picture = "kep.jpg",
                Answer_text = "Teszt kartya valasz",
                Answer_picture = "kep2.jpg",
                Deck = new DeckInfoDTO
                {
                    Id = 1,
                    Name = "Teszt csomag",
                    Module = Module.A,
                    Deck_number = 1,
                    Status = DeckStatus.Approved
                }
            };

            // Act
            var result = await controller.UpdateCard(unknownId, rightCard);

            // Asssert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteCard_UnknownIdPassed_ReturnsNotFound()
        {
            // Arrange
            var controller = new CardsController(flashcardDbContextMock.Object, logger);
            int unknownId = 16;

            // Act
            var result = await controller.DeleteCard(unknownId);

            // Asssert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteCardPicture_UnknownIdPassed_ReturnsNotFound()
        {
            // Arrange
            var controller = new CardsController(flashcardDbContextMock.Object, logger);
            int unknownId = 22;

            // Act
            var result = await controller.DeleteCardPicture(unknownId, "question");

            // Asssert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
