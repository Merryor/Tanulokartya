using Flashcard.Controllers;
using Flashcard.Data;
using Flashcard.Models;
using Flashcard.Models.DTO;
using Flashcard.Services;
using Microsoft.AspNetCore.Identity;
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
    /// This class implements the unit tests associated with the DeckAssignmentsController.
    /// </summary>
    public class DeckAssignmentsControllerTests
    {
        private readonly Mock<IFlashcardDbContext> flashcardDbContextMock;
        private readonly ILogger<DeckAssignmentsController> logger;
        private readonly IEmailService emailService;

        public DeckAssignmentsControllerTests()
        {
            var mockLogger = new Mock<ILogger<DeckAssignmentsController>>();
            logger = mockLogger.Object;

            var mockEmailService = new Mock<IEmailService>();
            emailService = mockEmailService.Object;

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
                    Activation_time = DateTime.Parse("2020-10-31"),
                    Status = DeckStatus.Approved,
                    ApplicationUser = new ApplicationUser {
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
                    Id = 2,
                    Name = "Teszt csomag 2",
                    Content = "Teszt csomag tartalma 2",
                    Module = Module.A,
                    Deck_number = 2,
                    Activated = false,
                    Activation_time = DateTime.Parse("2020-10-31"),
                    Status = DeckStatus.Approved,
                    ApplicationUser = new ApplicationUser {
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
            var deckAssignments = new List<DeckAssignment>
            {
                new DeckAssignment {
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
                    ApplicationUserId = "5049ed9e-7929-4fae-a693-6449097059a8",
                    Deck = new Deck
                    {
                        Id = 1,
                        Name = "Teszt csomag",
                        Content = "Teszt csomag tartalma",
                        Module = Module.A,
                        Deck_number = 1,
                        Activated = false,
                        Activation_time = DateTime.Parse("2020-10-31"),
                        Status = DeckStatus.Approved
                    },
                    DeckId = 1
                },
                new DeckAssignment {
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
                    ApplicationUserId = "5049ed9e-7929-4fae-a693-6449097059a8",
                    Deck = new Deck
                    {
                        Id = 2,
                        Name = "Teszt csomag 2",
                        Content = "Teszt csomag tartalma 2",
                        Module = Module.A,
                        Deck_number = 2,
                        Activated = false,
                        Activation_time = DateTime.Parse("2020-10-31"),
                        Status = DeckStatus.Approved
                    },
                    DeckId = 2
                },
            };

            var deckDbSet = decks.AsQueryable().BuildMockDbSet();
            flashcardDbContextMock.Setup(x => x.Decks).Returns(deckDbSet.Object);

            var userDbSet = users.AsQueryable().BuildMockDbSet();
            flashcardDbContextMock.Setup(x => x.ApplicationUsers).Returns(userDbSet.Object);

            var deckAssignmentDbSet = deckAssignments.AsQueryable().BuildMockDbSet();
            flashcardDbContextMock.Setup(x => x.DeckAssignment).Returns(deckAssignmentDbSet.Object);
        }

        private Mock<UserManager<ApplicationUser>> GetMockUserManager()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            return new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);
        }

        [Fact]
        public async Task GetDeckAssignments_ReturnsAllAssignments()
        {
            // Arrange
            var controller = new DeckAssignmentsController(flashcardDbContextMock.Object, GetMockUserManager().Object, emailService, logger);

            // Act
            var result = await controller.GetDeckAssignments();

            // Asssert
            var assignments = Assert.IsType<List<DeckAssignmentDTO>>(result.Value);
            Assert.Equal(2, assignments.Count);
        }
        [Fact]
        public async Task GetDeckAssignmentsByDeck_ReturnsAllAssignments()
        {
            // Arrange
            var controller = new DeckAssignmentsController(flashcardDbContextMock.Object, GetMockUserManager().Object, emailService, logger);
            int deckId = 1;

            // Act
            var result = await controller.GetDeckAssignmentsByDeck(deckId);

            // Asssert
            var assignments = Assert.IsType<List<DeckAssignmentDTO>>(result.Value);
            Assert.Equal(1, assignments.Count);
        }

        [Fact]
        public async Task GetDeckAssignmentsByDeck_UnknownIdPassed_ReturnsNull()
        {
            // Arrange
            var controller = new DeckAssignmentsController(flashcardDbContextMock.Object, GetMockUserManager().Object, emailService, logger);
            int unknownDeckId = 22;

            // Act
            var result = await controller.GetDeckAssignmentsByDeck(unknownDeckId);

            // Asssert
            Assert.Null(result.Result);
        }

        [Fact]
        public async Task CreateDeckAssignment_InvalidObjectPassed_ReturnsBadRequest()
        {
            // Arrange
            var controller = new DeckAssignmentsController(flashcardDbContextMock.Object, GetMockUserManager().Object, emailService, logger);
            var deckIdMissingAssignment = new DeckAssignmentDTO
            {
                Deck = new DeckInfoDTO()
                {
                    Id = 2,
                    Name = "Teszt csomag 2",
                    Module = Module.A,
                    Deck_number = 2,
                    Status = DeckStatus.Approved
                },
                User = new UserDTO()
                {
                    Id = "268e543e-2020-ue63-a111-98321798c21",
                    Email = "tesztemail2@gmail.com",
                    Name = "Teszt User 2",
                }
            };
            controller.ModelState.AddModelError("DeckId", "Required");

            // Act
            var result = await controller.CreateDeckAssignment(deckIdMissingAssignment);

            // Asssert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateAssignment_RightObjectPassed_ReturnsCreatedAtAction()
        {
            // Arrange
            var controller = new DeckAssignmentsController(flashcardDbContextMock.Object, GetMockUserManager().Object, emailService, logger);
            var rightAssignment = new DeckAssignmentDTO
            {
                Deck = new DeckInfoDTO()
                {
                    Id = 2,
                    Name = "Teszt csomag 2",
                    Module = Module.A,
                    Deck_number = 2,
                    Status = DeckStatus.Approved
                },
                User = new UserDTO()
                {
                    Id = "268e543e-2020-ue63-a111-98321798c21",
                    Email = "tesztemail2@gmail.com",
                    Name = "Teszt User 2",
                }
            };

            // Act
            var result = await controller.CreateDeckAssignment(rightAssignment);

            // Asssert
            Assert.IsType<CreatedAtActionResult>(result.Result);
        }

        [Fact]
        public async Task DeleteDeckAssignment_UnknownIdPassed_ReturnsNotFound()
        {
            // Arrange
            var controller = new DeckAssignmentsController(flashcardDbContextMock.Object, GetMockUserManager().Object, emailService, logger);
            int unknownDeckId = 16;
            string unknownUserId = "12316521";

            // Act
            var result = await controller.DeleteDeckAssignment(unknownDeckId, unknownUserId);

            // Asssert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
