using Flashcard.Controllers;
using Flashcard.Data;
using Flashcard.Models;
using Flashcard.Models.DTO;
using Flashcard.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
    /// This class implements the unit tests associated with the ApplicationUsersController.
    /// </summary>
    public class ApplicationUsersControllerTests
    {
        private readonly Mock<IFlashcardDbContext> flashcardDbContextMock;
        private readonly ILogger<ApplicationUsersController> logger;
        private readonly ILoginService loginService;
        private readonly Mock<FakeUserManager> fakeUserManager;

        public ApplicationUsersControllerTests()
        {
            var mockLogger = new Mock<ILogger<ApplicationUsersController>>();
            logger = mockLogger.Object;
            
            var mockLoginService = new Mock<ILoginService>();
            loginService = mockLoginService.Object;

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

            fakeUserManager = new Mock<FakeUserManager>();
            fakeUserManager.Setup(x => x.Users).Returns(flashcardDbContextMock.Object.ApplicationUsers);
            fakeUserManager.Setup(x => x.CheckPasswordAsync(users[0], users[0].PasswordHash)).ReturnsAsync(true);
            fakeUserManager.Setup(x => x.CheckPasswordAsync(users[1], users[1].PasswordHash)).ReturnsAsync(true);
            fakeUserManager.Setup(x => x.GetRolesAsync(users[0])).ReturnsAsync(new List<string> { "Administrator" });
            fakeUserManager.Setup(x => x.GetRolesAsync(users[1])).ReturnsAsync(new List<string> { "Card creator" });

        }
        private Mock<SignInManager<ApplicationUser>> GetMockSignInManager()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var mockUsrMgr = new UserManager<ApplicationUser>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            var ctxAccessor = new HttpContextAccessor();
            var mockClaimsPrinFact = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var mockOpts = new Mock<IOptions<IdentityOptions>>();
            var mockLogger = new Mock<ILogger<SignInManager<ApplicationUser>>>();

            return new Mock<SignInManager<ApplicationUser>>(mockUsrMgr, ctxAccessor, mockClaimsPrinFact.Object, mockOpts.Object, mockLogger.Object, null, null);
        }

        [Fact]
        public async Task GetUsers_ReturnsAllUsers()
        {
            // Arrange
            var controller = new ApplicationUsersController(flashcardDbContextMock.Object, logger, fakeUserManager.Object, GetMockSignInManager().Object, loginService);

            // Act
            var result = await controller.GetUsers();

            // Asssert
            var cards = Assert.IsType<List<ApplicationUserDTO>>(result.Value);
            Assert.Equal(2, cards.Count);
        }        
        
        [Fact]
        public async Task GetUser_ExistingIdPassed_ReturnsRightUser()
        {
            // Arrange            
            var controller = new ApplicationUsersController(flashcardDbContextMock.Object, logger, fakeUserManager.Object, GetMockSignInManager().Object, loginService);
            string existingUserId = "5049ed9e-7929-4fae-a693-6449097059a8";

            // Act
            var result = await controller.GetUser(existingUserId);

            // Asssert
            Assert.Equal("Teszt User", result.Value.Name);
        }
        
        [Fact]
        public async Task GetUser_UnknownIdPassed_ReturnsNotFoundResult()
        {
            // Arrange
            var controller = new ApplicationUsersController(flashcardDbContextMock.Object, logger, fakeUserManager.Object, GetMockSignInManager().Object, loginService);
            string unknownUserId = "2225049ed9e-7929-4fae-a693-6449097059a8";

            // Act
            var result = await controller.GetUser(unknownUserId);

            // Asssert
            Assert.IsType<NotFoundResult>(result.Result);
        }
        
     
        [Fact]
        public async Task CreateUser_InvalidObjectPassed_ReturnsBadRequest()
        {
            // Arrange
            var controller = new ApplicationUsersController(flashcardDbContextMock.Object, logger, fakeUserManager.Object, GetMockSignInManager().Object, loginService);
            var nameMissingUser = new ApplicationUserDTO
            {
                Email = "teszt.email@gmail.com",
                Phone = "06201234567",
                Password = "tesztjelszo",
                Workplace = "Teszt iskola",
                Create_module = Module.E,
                Will_create_module = Module.F,
                Activated = true
            };
            controller.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await controller.CreateUser(nameMissingUser);

            // Asssert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task AddUserRole_UnknownIdPassed_ReturnsNotFound()
        {
            // Arrange
            var controller = new ApplicationUsersController(flashcardDbContextMock.Object, logger, fakeUserManager.Object, GetMockSignInManager().Object, loginService);
            var user = new ApplicationUserDTO
            {
                Id = "111333322223333444",
                Email = "tesztemail@gmail.com",
                Name = "Teszt User",
                Password = "teszt password",
                Phone = "06301234567",
                Workplace = "Teszt kornyezet",
                Create_module = Module.E,
                Will_create_module = Module.F,
                Activated = true,
            };

            // Act
            var result = await controller.AddUserRole(user);

            // Asssert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateUser_UnknownIdPassed_ReturnsNotFound()
        {
            // Arrange
            var controller = new ApplicationUsersController(flashcardDbContextMock.Object, logger, fakeUserManager.Object, GetMockSignInManager().Object, loginService);
            string unknownId = "198732167921";
            var user = new ApplicationUserDTO
            {
                Id= "198732167921",
                Name = "Teszt user",
                Email = "teszt.email@gmail.com",
                Phone = "06201234567",
                Password = "tesztjelszo",
                Workplace = "Teszt iskola",
                Create_module = Module.E,
                Will_create_module = Module.F,
                Activated = true
            };

            // Act
            var result = await controller.UpdateUser(unknownId, user);

            // Asssert
            Assert.IsType<NotFoundResult>(result);
        }

        
        [Fact]
        public async Task DeleteUserRole_UnknownIdPassed_ReturnsNotFound()
        {
            // Arrange
            var controller = new ApplicationUsersController(flashcardDbContextMock.Object, logger, fakeUserManager.Object, GetMockSignInManager().Object, loginService);
            string unknownId = "123456789";
            string role = "Card creator";

            // Act
            var result = await controller.DeleteUserRole(unknownId, role);

            // Asssert
            Assert.IsType<NotFoundResult>(result);
        }
        

        [Fact]
        public async Task DeactivateUser_UnknownIdPassed_ReturnsNotFound()
        {
            // Arrange
            var controller = new ApplicationUsersController(flashcardDbContextMock.Object, logger, fakeUserManager.Object, GetMockSignInManager().Object, loginService);
            string unknownId = "12a345-678-re9";

            // Act
            var result = await controller.DeactivateUser(unknownId);

            // Asssert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
