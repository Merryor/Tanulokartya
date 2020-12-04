using Flashcard.Controllers;
using Flashcard.Data;
using Flashcard.Models;
using Flashcard.Models.DTO;
using Flashcard.Services;
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
    /// This class implements the unit tests associated with the DecksController.
    /// </summary>
    public class DecksControllerTests
    {
        private readonly Mock<IFlashcardDbContext> flashcardDbContextMock;
        private readonly ILogger<DecksController> logger;
        private readonly IOrderService orderService;

        public DecksControllerTests()
        {
            var mockLogger = new Mock<ILogger<DecksController>>();
            logger = mockLogger.Object;
            
            var mockOrderService = new Mock<IOrderService>();
            orderService = mockOrderService.Object;

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
                    Activated = true,
                    Activation_time = DateTime.Parse("2020-10-29"),
                    Status = DeckStatus.Approved,
                    ApplicationUser = new ApplicationUser {
                        Id = "5049ed9e-7929-4fae-a693-6449097059a8",
                        Email = "tesztemail@gmail.com",
                        Name = "Teszt User",
                        PasswordHash = "teszt password",
                        Phone = "06301234567",
                        Workplace = "Teszt kornyezet",
                        Create_module = Module.E,
                        Will_create_module = Module.F,
                        Activated = true
                    },
                },
                new Deck
                {
                    Id = 2,
                    Name = "Teszt csomag 2",
                    Content = "Teszt csomag 2 tartalma",
                    Module = Module.A,
                    Deck_number = 2,
                    Activated = true,
                    Activation_time = DateTime.Parse("2020-10-30"),
                    Status = DeckStatus.Approved,
                    ApplicationUser = new ApplicationUser {
                        Id = "5049ed9e-7929-4fae-a693-6449097059a8",
                        Email = "tesztemail@gmail.com",
                        Name = "Teszt User",
                        PasswordHash = "teszt password",
                        Phone = "06301234567",
                        Workplace = "Teszt kornyezet",
                        Create_module = Module.E,
                        Will_create_module = Module.F,
                        Activated = true
                    },
                },
                new Deck
                {
                    Id = 3,
                    Name = "Teszt csomag 3",
                    Content = "Teszt csomag 3 tartalma",
                    Module = Module.B,
                    Deck_number = 1,
                    Activated = true,
                    Activation_time = DateTime.Parse("2020-10-31"),
                    Status = DeckStatus.Approved,
                    ApplicationUser =  new ApplicationUser {
                        Id = "268e543e-2020-ue63-a111-98321798c21",
                        Email = "tesztemail2@gmail.com",
                        Name = "Teszt User 2",
                        PasswordHash = "teszt password2",
                        Phone = "063012345672",
                        Workplace = "Teszt kornyezet2",
                        Create_module = Module.E,
                        Will_create_module = Module.F,
                        Activated = true
                    }
                },
                new Deck
                {
                    Id = 4,
                    Name = "Teszt csomag 4",
                    Content = "Teszt csomag 4 tartalma",
                    Module = Module.B,
                    Deck_number = 2,
                    Activated = false,
                    Activation_time = DateTime.Parse("2020-10-31"),
                    Status = DeckStatus.Approved,
                    ApplicationUser =  new ApplicationUser {
                        Id = "268e543e-2020-ue63-a111-98321798c21",
                        Email = "tesztemail2@gmail.com",
                        Name = "Teszt User 2",
                        PasswordHash = "teszt password2",
                        Phone = "063012345672",
                        Workplace = "Teszt kornyezet2",
                        Create_module = Module.E,
                        Will_create_module = Module.F,
                        Activated = true
                    }
                }
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
            var users = new List<ApplicationUser>
            {
                new ApplicationUser {
                    Id = "5049ed9e-7929-4fae-a693-6449097059a8",
                    Email = "tesztemail@gmail.com",
                    Name = "Teszt User",
                    PasswordHash = "teszt password",
                    Phone = "06301234567",
                    Workplace = "Teszt kornyezet",
                    Create_module = Module.E,
                    Will_create_module = Module.F,
                    Activated = true
                },
                new ApplicationUser {
                    Id = "268e543e-2020-ue63-a111-98321798c21",
                    Email = "tesztemail2@gmail.com",
                    Name = "Teszt User 2",
                    PasswordHash = "teszt password2",
                    Phone = "063012345672",
                    Workplace = "Teszt kornyezet2",
                    Create_module = Module.E,
                    Will_create_module = Module.F,
                    Activated = true
                }
            };

            var userDbSet = users.AsQueryable().BuildMockDbSet();
            flashcardDbContextMock.Setup(x => x.ApplicationUsers).Returns(userDbSet.Object);

            var deckDbSet = decks.AsQueryable().BuildMockDbSet();
            flashcardDbContextMock.Setup(x => x.Decks).Returns(deckDbSet.Object);

            var cardDbSet = cards.AsQueryable().BuildMockDbSet();
            flashcardDbContextMock.Setup(x => x.Cards).Returns(cardDbSet.Object);
        }

        [Fact]
        public async Task GetDecks_ReturnsAllActivatedDecks()
        {
            // Arrange
            var controller = new DecksController(flashcardDbContextMock.Object, orderService, logger);

            // Act
            var result = await controller.GetDecks();

            // Asssert
            var cards = Assert.IsType<List<DeckMobileDTO>>(result.Value);
            Assert.Equal(3, cards.Count);
        }

        [Fact]
        public async Task GetAllDecks_ReturnsAllDecks()
        {
            // Arrange
            var controller = new DecksController(flashcardDbContextMock.Object, orderService, logger);

            // Act
            var result = await controller.GetAllDecks();

            // Asssert
            var cards = Assert.IsType<List<DeckDTO>>(result.Value);
            Assert.Equal(4, cards.Count);
        }

        [Fact]
        public async Task GetMyDecks_ReturnsAllDecksByUserId()
        {
            // Arrange
            var controller = new DecksController(flashcardDbContextMock.Object, orderService, logger);
            string existingUserId = "5049ed9e-7929-4fae-a693-6449097059a8";

            // Act
            var result = await controller.GetMyDecks(existingUserId);

            // Asssert
            var cards = Assert.IsType<List<DeckDTO>>(result.Value);
            Assert.Equal(2, cards.Count);
        }

        [Fact]
        public async Task GetDecksByModule_ReturnsAllActivatedDecksByModule()
        {
            // Arrange
            var controller = new DecksController(flashcardDbContextMock.Object, orderService, logger);
            int module = 1;

            // Act
            var result = await controller.GetDecksByModule(module);

            // Asssert
            var cards = Assert.IsType<List<DeckMobileDTO>>(result.Value);
            Assert.Equal(2, cards.Count);
        }

        [Fact]
        public async Task GetDeck_ExistingIdPassed_ReturnsRightDeck()
        {
            // Arrange
            var controller = new DecksController(flashcardDbContextMock.Object, orderService, logger);
            int existingDeckId = 2;

            // Act
            var result = await controller.GetDeck(existingDeckId);

            // Asssert
            Assert.Equal("Teszt csomag 2", result.Value.Name);
        }

        [Fact]
        public async Task GetDeck_UnknownIdPassed_ReturnsNotFoundResult()
        {
            // Arrange
            var controller = new DecksController(flashcardDbContextMock.Object, orderService, logger);
            int unknownDeckId = 5;

            // Act
            var result = await controller.GetDeck(unknownDeckId);

            // Asssert
            Assert.IsType<NotFoundResult>(result.Result);
        }

     
        [Fact]
        public async Task CreateDeck_InvalidObjectPassed_ReturnsBadRequest()
        {
            // Arrange
            var controller = new DecksController(flashcardDbContextMock.Object, orderService, logger);
            var nameMissingDeck = new DeckDTO
            {
                Content = "Uj Teszt csomag tartalma",
                Module = Module.B,
                Deck_number = 3,
                ApplicationUser = new ApplicationUserDTO
                {
                    Id = "268e543e-2020-ue63-a111-98321798c21",
                    Email = "tesztemail2@gmail.com",
                    Name = "Teszt User 2",
                    Phone = "063012345672",
                    Workplace = "Teszt kornyezet2",
                    Create_module = Module.E,
                    Will_create_module = Module.F,
                    Activated = true
                }
            };
            controller.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await controller.CreateDeck(nameMissingDeck);

            // Asssert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateDeck_RightObjectPassed_ReturnsCreatedAtAction()
        {
            // Arrange
            var controller = new DecksController(flashcardDbContextMock.Object, orderService, logger);
            var rightCard = new DeckDTO
            {
                Name = "Uj csomag neve",
                Content = "Uj Teszt csomag tartalma",
                Module = Module.B,
                Deck_number = 3,
                ApplicationUser = new ApplicationUserDTO
                {
                    Id = "268e543e-2020-ue63-a111-98321798c21",
                    Email = "tesztemail2@gmail.com",
                    Name = "Teszt User 2",
                    Phone = "063012345672",
                    Workplace = "Teszt kornyezet2",
                    Create_module = Module.E,
                    Will_create_module = Module.F,
                    Activated = true
                }
            };

            // Act
            var result = await controller.CreateDeck(rightCard);

            // Asssert
            Assert.IsType<CreatedAtActionResult>(result.Result);
        }

        [Fact]
        public async Task UpdateDeck_UnknownIdPassed_ReturnsNotFound()
        {
            // Arrange
            var controller = new DecksController(flashcardDbContextMock.Object, orderService, logger);
            int unknownId = 15;
            var rightDeck = new DeckDTO
            {
                Name = "Uj csomag neve",
                Content = "Uj Teszt csomag tartalma",
                Module = Module.B,
                Deck_number = 3,
                ApplicationUser = new ApplicationUserDTO
                {
                    Id = "268e543e-2020-ue63-a111-98321798c21",
                    Email = "tesztemail2@gmail.com",
                    Name = "Teszt User 2",
                    Phone = "063012345672",
                    Workplace = "Teszt kornyezet2",
                    Create_module = Module.E,
                    Will_create_module = Module.F,
                    Activated = true
                }
            };

            // Act
            var result = await controller.UpdateDeck(unknownId, rightDeck);

            // Asssert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateActivateDeck_UnknownIdPassed_ReturnsNotFound()
        {
            // Arrange
            var controller = new DecksController(flashcardDbContextMock.Object, orderService, logger);
            int unknownId = 15;

            // Act
            var result = await controller.UpdateActivateDeck(unknownId);

            // Asssert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateStateDeck_UnknownIdPassed_ReturnsNotFound()
        {
            // Arrange
            var controller = new DecksController(flashcardDbContextMock.Object, orderService, logger);
            int unknownId = 20;
            int state = 1;

            // Act
            var result = await controller.UpdateStateDeck(unknownId, state);

            // Asssert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
