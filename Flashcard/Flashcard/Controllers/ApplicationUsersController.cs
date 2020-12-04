using Flashcard.Data;
using Flashcard.Models;
using Flashcard.Models.DTO;
using Flashcard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flashcard.Controllers
{
    /// <summary>
    /// This class manages ApplicationUsers.
    /// Contains all methods for handling users.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ApplicationUsersController : ControllerBase
    {
        private readonly ILogger<ApplicationUsersController> _logger;
        private readonly IFlashcardDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private ILoginService loginService;

        public ApplicationUsersController(IFlashcardDbContext context, ILogger<ApplicationUsersController> logger, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILoginService loginService)
        {
            _context = context;
            _logger = logger;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.loginService = loginService;
        }

        /// <summary>
        /// This function handles users login.
        /// </summary>
        // POST api/ApplicationUsers/Login
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody]LoginDTO userParam)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(userParam.Email, userParam.Password, false, false);

                if (result.Succeeded)
                {
                    var user = loginService.Authenticate(userParam.Email, userParam.Password);
                    _logger.LogInformation("Successful login in the Login() method");
                    return Ok(user);
                }
                _logger.LogWarning("Unsuccessful login in the Login() method");
                return NotFound(new { message = "Bejelentkezés sikertelen!" });
            }
            _logger.LogError("ModelState is invalid in the Login() method");
            return BadRequest(ModelState);
        }

        /// <summary>
        /// This function handles users logout.
        /// </summary>
        // POST api/ApplicationUsers/Logout
        [Authorize]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            _logger.LogInformation("Successful logout in the Logout() method");
            return Ok(new { message = "Logout was correct" });
        }

        /// <summary>
        /// This function returns list of users.
        /// </summary>
        // GET: api/ApplicationUsers
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApplicationUserDTO>>> GetUsers()
        {
            _logger.LogInformation("Listing all users in the GetUsers() method");
            var list = new List<ApplicationUserDTO>();
            foreach (var user in userManager.Users.ToList())
            {
                list.Add(new ApplicationUserDTO()
                {
                    Id = user.Id,
                    Name = user.Name,
                    Phone = user.Phone,
                    Email = user.Email,
                    Workplace = user.Workplace,
                    Create_module = user.Create_module,
                    Will_create_module = user.Will_create_module,
                    Activated = user.Activated,
                    Roles = GetUserRole(user.Id).Result
                });
            }
            return list;
        }

        /// <summary>
        /// This function returns the role(s) of a user.
        /// </summary>
        // GET: api/ApplicationUsers/GetUserRole/1
        [Authorize]
        [ActionName("GetUserRole")]
        [HttpGet("[action]/{id}")]
        public async Task<List<string>> GetUserRole([FromRoute] string id)
        {
            var user = await _context.ApplicationUsers.FindAsync(id);
            var roles = await userManager.GetRolesAsync(user);
            if (user == null || roles == null)
            {
                _logger.LogWarning("User or roles not found in the GetUserRole({0}) method", id);
                return null;
            }
            _logger.LogInformation("Getting UserRoles in the GetUserRole({0}) method", id);
            return roles.ToList();
        }

        /// <summary>
        /// This function returns the users for a specific role.
        /// </summary>
        // GET: api/ApplicationUsers/UsersByRole
        [Authorize(Roles = "Coordinator,Main Lector,Main Graphic,Main Professional reviewer")]
        [ActionName("UsersByRole")]
        [HttpGet("[action]/{role}")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsersByRole([FromRoute] string role)
        {
            _logger.LogInformation("Listing Users by role in the GetUsersByRole() method");
            var list = new List<UserDTO>();
            foreach (var user in userManager.Users.ToList())
            {
                if(GetUserRole(user.Id).Result.Contains(role) && user.Activated)
                list.Add(new UserDTO()
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Roles = GetUserRole(user.Id).Result
                });
            }
            return list;
        }

        /// <summary>
        /// This function returns the user by ID.
        /// </summary>
        // GET: api/ApplicationUsers/1
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicationUserDTO>> GetUser([FromRoute] string id)
        {
            var user = await _context.ApplicationUsers.SingleOrDefaultAsync(u => u.Id == id);
            var roles = await userManager.GetRolesAsync(user);
            if (user == null || roles == null)
            {
                _logger.LogWarning("User or roles not found in the GetUser({0}) method", id);
                return NotFound();
            }

            _logger.LogInformation("Getting {0}. user with roles", id);
            return ItemToDTO(user, roles.ToList());
        }

        /// <summary>
        /// This function creates the new user.
        /// </summary>
        // POST: api/ApplicationUsers
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<ActionResult<ApplicationUserDTO>> CreateUser([FromBody] ApplicationUserDTO userDTO)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("ModelState is invalid in the CreateUser() method");
                return BadRequest(ModelState);
            }

            var foundUser = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Email == userDTO.Email);
            if (foundUser != null)
            {
                _logger.LogError("BadRequest in the CreateUser() method");
                return BadRequest(new { message = "Ezzel az emailcímmel felhasználó már létezik!" });
            }

            var user = new ApplicationUser();

            user = new ApplicationUser
            {
                Name = userDTO.Name,
                Phone = userDTO.Phone,
                Email = userDTO.Email,
                UserName = userDTO.Email,
                Workplace = userDTO.Workplace,
                Create_module = userDTO.Create_module,
                Will_create_module = userDTO.Will_create_module,
                Activated = userDTO.Activated
            };

            var result = await userManager.CreateAsync(user, userDTO.Password);
            var roleResult = await userManager.AddToRoleAsync(user, userDTO.Roles.FirstOrDefault());

            if (result.Succeeded && roleResult.Succeeded)
            {
                await signInManager.SignInAsync(user, isPersistent: false);
            }

            _logger.LogInformation("Creating user in the CreateUser() method");
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, ItemToDTO(user, userDTO.Roles));
        }

        /// <summary>
        /// This function creates the role to the user.
        /// </summary>
        // POST: api/ApplicationUsers/AddUserRole
        [Authorize(Roles = "Administrator")]
        [ActionName("AddUserRole")]
        [HttpPost("[action]")]
        public async Task<IActionResult> AddUserRole([FromBody] ApplicationUserDTO userDTO)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("ModelState is invalid in the AddUserRole() method");
                return BadRequest(ModelState);
            }

            var foundUser = await userManager.FindByIdAsync(userDTO.Id);
            if (foundUser == null)
            {
                _logger.LogWarning("User not found in the AddUserRole() method");
                return NotFound();
            }
            var roleResult = await userManager.AddToRoleAsync(foundUser, userDTO.Roles.LastOrDefault());

            if (roleResult.Succeeded)
            {
                await signInManager.SignInAsync(foundUser, isPersistent: false);
            }

            _logger.LogInformation("Creating role to user in the AddUserRole() method");
            return NoContent();
        }

        /// <summary>
        /// This function updates an existing user.
        /// </summary>
        // PUT: api/ApplicationUsers/2
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser([FromRoute] string id, [FromBody] ApplicationUserDTO userDTO)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("ModelState is invalid in the UpdateUser() method");
                return BadRequest(ModelState);
            }
            if (id != userDTO.Id)
            {
                _logger.LogError("BadRequest in the UpdateUser() method");
                return BadRequest();
            }

            var user = await _context.ApplicationUsers.FindAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User not found in the UpdateUser({0}) method", id);
                return NotFound();
            }

            user.Name = userDTO.Name;
            user.Phone = userDTO.Phone;
            user.Email = userDTO.Email;
            user.UserName = userDTO.Email;
            user.NormalizedUserName = userDTO.Email.ToUpper();
            user.NormalizedEmail = userDTO.Email.ToUpper();
            user.PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(user, userDTO.Password);
            user.Workplace = userDTO.Workplace;
            user.Create_module = userDTO.Create_module;
            user.Will_create_module = userDTO.Will_create_module;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!UserExists(id))
            {
                return NotFound();
            }

            _logger.LogInformation("Updating user in the UpdateUser({0}) method", id);
            return NoContent();
        }

        /// <summary>
        /// This function deletes the role of a user.
        /// </summary>
        // DELETE: api/ApplicationUsers/DeleteUserRole/userRole/role
        [Authorize(Roles = "Administrator")]
        [ActionName("DeleteUserRole")]
        [HttpDelete("[action]/{id}/{role}")]
        public async Task<ActionResult> DeleteUserRole([FromRoute] string id, [FromRoute] string role)
        {
            var user = await userManager.FindByIdAsync(id);

            if (user == null)
            {
                _logger.LogWarning("User not found in the DeleteUserRole({0}) method", id);
                return NotFound();
            }
            
            try
            {
                var roleResult = await userManager.RemoveFromRoleAsync(user, role);
                if (!roleResult.Succeeded)
                {
                    _logger.LogError("BadRequest in the DeleteUserRole() method");
                    return BadRequest(new { message = "Nem sikerült törölni a szerepkört!" });
                }
            }
            catch (DbUpdateConcurrencyException) when (!UserExists(id))
            {
                return NotFound();
            }

            _logger.LogInformation("Deleting user's role in the DeleteUserRole({0}) method", id);
            return NoContent();
        }

        /// <summary>
        /// This function deactivates a user.
        /// </summary>
        // DELETE: api/ApplicationUsers/Deactivate/2
        [Authorize(Roles = "Administrator")]
        [ActionName("Deactivate")]
        [HttpDelete("[action]/{id}")]
        public async Task<ActionResult> DeactivateUser([FromRoute] string id)
        {
            var user = await _context.ApplicationUsers.FindAsync(id);

            if (user == null)
            {
                _logger.LogWarning("User not found in the DeactivateUser({0}) method", id);
                return NotFound();
            }

            user.Activated = false;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!UserExists(id))
            {
                return NotFound();
            }

            _logger.LogInformation("Deactivating user in the DeactivateUser({0}) method", id);
            return NoContent();
        }

        #region helperMethods

        /// <summary>
        /// This helper function checks for the existence of the given user based on the ID.
        /// </summary>
        private bool UserExists(string id) =>
         _context.ApplicationUsers.Any(e => e.Id == id);

        /// <summary>
        /// This helper function converts the user to a DTO.
        /// </summary>
        private static ApplicationUserDTO ItemToDTO(ApplicationUser user, List<string> roles) =>
        new ApplicationUserDTO
        {
            Id = user.Id,
            Name = user.Name,
            Phone = user.Phone,
            Email = user.Email,
            Password = user.PasswordHash,
            Workplace = user.Workplace,
            Create_module = user.Create_module,
            Will_create_module = user.Will_create_module,
            Activated = user.Activated,
            Roles = roles
        };

        #endregion
    }
}
