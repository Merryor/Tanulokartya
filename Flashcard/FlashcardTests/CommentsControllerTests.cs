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
    /// This class implements the unit tests associated with the CommentsController.
    /// </summary>
    public class CommentsControllerTests
    {
        private readonly Mock<IFlashcardDbContext> flashcardDbContextMock;
        private readonly ILogger<CommentsController> logger;

        public CommentsControllerTests()
        {
            var mockLogger = new Mock<ILogger<CommentsController>>();
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
                }
            };
            var comments = new List<Comment>
            {
                new Comment {
                    Id = 1,
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
                    Card =  new Card {
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
                    CardId = 1,
                    Comment_text = "Proba komment",
                    Comment_time =  DateTime.Parse("2020-10-31")
                },
                new Comment {
                    Id = 2,
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
                    Card =  new Card {
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
                    CardId = 1,
                    Comment_text = "Proba komment 2",
                    Comment_time =  DateTime.Parse("2020-10-31")
                }
            };

            var userDbSet = users.AsQueryable().BuildMockDbSet();
            flashcardDbContextMock.Setup(x => x.ApplicationUsers).Returns(userDbSet.Object);

            var deckDbSet = decks.AsQueryable().BuildMockDbSet();
            flashcardDbContextMock.Setup(x => x.Decks).Returns(deckDbSet.Object);

            var cardDbSet = cards.AsQueryable().BuildMockDbSet();
            flashcardDbContextMock.Setup(x => x.Cards).Returns(cardDbSet.Object);

            var commentDbSet = comments.AsQueryable().BuildMockDbSet();
            flashcardDbContextMock.Setup(x => x.Comments).Returns(commentDbSet.Object);
        }

        [Fact]
        public async Task GetComments_ReturnsAllCommentsByCardId()
        {
            // Arrange
            var controller = new CommentsController(flashcardDbContextMock.Object, logger);
            int cardId = 1;

            // Act
            var result = await controller.GetComments(cardId);

            // Asssert
            var comments = Assert.IsType<List<CommentDTO>>(result.Value);
            Assert.Equal(2, comments.Count);
        }
        
        [Fact]
        public async Task GetComment_ExistingIdPassed_ReturnsRightComment()
        {
            // Arrange
            var controller = new CommentsController(flashcardDbContextMock.Object, logger);
            int existingCommentId = 2;

            // Act
            var result = await controller.GetComment(existingCommentId);

            // Asssert
            Assert.Equal(2, result.Value.Id);
        }

        [Fact]
        public async Task GetComment_UnknownIdPassed_ReturnsNotFoundResult()
        {
            // Arrange
            var controller = new CommentsController(flashcardDbContextMock.Object, logger);
            int unknownCommentId = 5;

            // Act
            var result = await controller.GetComment(unknownCommentId);

            // Asssert
            Assert.IsType<NotFoundResult>(result.Result);
        }
        
     
        [Fact]
        public async Task CreateComment_InvalidObjectPassed_ReturnsBadRequest()
        {
            // Arrange
            var controller = new CommentsController(flashcardDbContextMock.Object, logger);
            var typeMissingComment = new CommentDTO
            {
                Id = 2,
                User = new ApplicationUserDTO
                {
                    Id = "5049ed9e-7929-4fae-a693-6449097059a8",
                    Email = "tesztemail@gmail.com",
                    Name = "Teszt User",
                    Phone = "06301234567",
                    Workplace = "Teszt kornyezet",
                    Create_module = Module.E,
                    Will_create_module = Module.F,
                    Activated = true
                },
                CardId = 1,
                Comment_text = "Uj Proba komment"
            };
            controller.ModelState.AddModelError("Comment_time", "Required");

            // Act
            var result = await controller.CreateComment(typeMissingComment);

            // Asssert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        
        [Fact]
        public async Task CreateComment_RightObjectPassed_ReturnsCreatedAtAction()
        {
            // Arrange
            var controller = new CommentsController(flashcardDbContextMock.Object, logger);
            var rightComment = new CommentDTO
            {
                Id = 2,
                User = new ApplicationUserDTO
                {
                    Id = "5049ed9e-7929-4fae-a693-6449097059a8",
                    Email = "tesztemail@gmail.com",
                    Name = "Teszt User",
                    Phone = "06301234567",
                    Workplace = "Teszt kornyezet",
                    Create_module = Module.E,
                    Will_create_module = Module.F,
                    Activated = true
                },
                CardId = 1,
                Comment_text = "Uj Proba komment",
                Comment_time = DateTime.Parse("2020-10-31")
            };

            // Act
            var result = await controller.CreateComment(rightComment);

            // Asssert
            Assert.IsType<CreatedAtActionResult>(result.Result);
        }
       
        [Fact]
        public async Task DeleteComment_UnknownIdPassed_ReturnsNotFound()
        {
            // Arrange
            var controller = new CommentsController(flashcardDbContextMock.Object, logger);
            int unknownId = 20;

            // Act
            var result = await controller.DeleteComment(unknownId);

            // Asssert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
