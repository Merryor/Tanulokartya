using Flashcard.Data;
using Flashcard.Helpers;
using Flashcard.Models;
using Flashcard.Models.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Flashcard.Services
{
    /// <summary>
    /// This interface belongs to LoginService.
    /// </summary>
    public interface ILoginService
    {
        LoginUserDTO Authenticate(string email, string password);
    }

    /// <summary>
    /// This service class handles user logins.
    /// </summary>
    public class LoginService : ILoginService
    {
        private readonly FlashcardDbContext dbContext;
        private readonly AppSettings _appSettings;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public LoginService(FlashcardDbContext dbContext, IOptions<AppSettings> appSettings, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.dbContext = dbContext;
            _appSettings = appSettings.Value;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        /// <summary>
        /// This function manage authentication and creates token for the user.
        /// </summary>
        public LoginUserDTO Authenticate(string email, string password)
        {
            var user = dbContext.ApplicationUsers.SingleOrDefault(x => x.Email == email);

            if (user == null || user.Activated == false)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                        
            var roles = userManager.GetRolesAsync(user);
            var role = roleManager.FindByNameAsync(roles.Result[0]);

            var tokenDescriptor = new SecurityTokenDescriptor();
            if (roles.Result.Count > 1)
            {
                tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, roles.Result[0]),
                    new Claim(ClaimTypes.Role, roles.Result[1]),

                }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
            }
            else
            {
                tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, role.Result.Name)

                }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
            }            
            
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            return new LoginUserDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Roles = roles.Result.ToList(),
                Token = user.Token
            };
        }
    }
}
