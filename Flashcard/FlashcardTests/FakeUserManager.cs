using Flashcard.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;

namespace FlashcardTests
{
    /// <summary>
    /// This class defines a fake class that the mocking framework will use by using Moq.
    /// </summary>
    public class FakeUserManager : UserManager<ApplicationUser>
    {
        public FakeUserManager() : base(
         new Mock<IUserStore<ApplicationUser>>().Object,
         new Mock<IOptions<IdentityOptions>>().Object,
         new Mock<IPasswordHasher<ApplicationUser>>().Object,
         new IUserValidator<ApplicationUser>[0],
         new IPasswordValidator<ApplicationUser>[0],
         new Mock<ILookupNormalizer>().Object,
         new Mock<IdentityErrorDescriber>().Object,
         new Mock<IServiceProvider>().Object,
         new Mock<ILogger<UserManager<ApplicationUser>>>().Object)
        { }
    }
}
