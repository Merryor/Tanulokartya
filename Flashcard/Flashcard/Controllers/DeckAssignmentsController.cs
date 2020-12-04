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
    /// This class manages DeckAssignments.
    /// Contains all methods for handling assignments.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class DeckAssignmentsController : ControllerBase
    {
        private readonly ILogger<DeckAssignmentsController> _logger;
        private readonly IFlashcardDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;
        private IEmailService emailService;

        public DeckAssignmentsController(IFlashcardDbContext context, UserManager<ApplicationUser> userManager, IEmailService emailService, ILogger<DeckAssignmentsController> logger)
        {
            _context = context;
            _logger = logger;
            this.userManager = userManager;
            this.emailService = emailService;
        }

        /// <summary>
        /// This function returns list of assignments.
        /// </summary>
        // GET: api/DeckAssignments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeckAssignmentDTO>>> GetDeckAssignments()
        {
            _logger.LogInformation("Listing all assignments in the GetDeckAssignments() method");
            var list = new List<DeckAssignmentDTO>();
            foreach (var deckAssignment in _context.DeckAssignment.Include(d => d.Deck).Include(d => d.ApplicationUser).ToList())
            {
                list.Add(new DeckAssignmentDTO()
                {
                    Deck = new DeckInfoDTO()
                    {
                        Id = deckAssignment.Deck.Id,
                        Name = deckAssignment.Deck.Name,
                        Module = deckAssignment.Deck.Module,
                        Status = deckAssignment.Deck.Status,
                        Deck_number = deckAssignment.Deck.Deck_number
                    },
                    User = new UserDTO()
                    {
                        Id = deckAssignment.ApplicationUser.Id,
                        Name = deckAssignment.ApplicationUser.Name,
                        Email = deckAssignment.ApplicationUser.Email,
                        Roles = GetUserRole(deckAssignment.ApplicationUser.Id).Result
                    }
                });
            }
            return list;
        }

        /// <summary>
        /// This function returns list of assignments for the specified deck.
        /// </summary>
        // GET: api/DeckAssignments/ByDeck/1
        [ActionName("ByDeck")]
        [HttpGet("[action]/{deckId}")]
        public async Task<ActionResult<IEnumerable<DeckAssignmentDTO>>> GetDeckAssignmentsByDeck([FromRoute] int deckId)
        {
            _logger.LogInformation("Listing all assignments by deckId in the GetDeckAssignmentsByDeck() method");
            var list = new List<DeckAssignmentDTO>();
            foreach (var deckAssignment in _context.DeckAssignment.Include(d => d.Deck).Include(d => d.ApplicationUser).Where(d => d.DeckId == deckId).ToList())
            {
                list.Add(new DeckAssignmentDTO()
                {
                    Deck = new DeckInfoDTO()
                    {
                        Id = deckAssignment.Deck.Id,
                        Name = deckAssignment.Deck.Name,
                        Module = deckAssignment.Deck.Module,
                        Status = deckAssignment.Deck.Status,
                        Deck_number = deckAssignment.Deck.Deck_number
                    },
                    User = new UserDTO()
                    {
                        Id = deckAssignment.ApplicationUser.Id,
                        Name = deckAssignment.ApplicationUser.Name,
                        Email = deckAssignment.ApplicationUser.Email,
                        Roles = GetUserRole(deckAssignment.ApplicationUser.Id).Result
                    }
                });
            }
            return list;
        }

        /// <summary>
        /// This function returns list of assignments for the specified user.
        /// </summary>
        // GET: api/DeckAssignments/ByUser/1
        [ActionName("ByUser")]
        [HttpGet("[action]/{userId}")]
        public async Task<ActionResult<IEnumerable<DeckAssignmentListDTO>>> GetDeckAssignmentsByUser([FromRoute] string userId)
        {
            _logger.LogInformation("Listing all assignments by userId in the GetDeckAssignmentsByUser() method");
            var list = new List<DeckAssignmentListDTO>();
            foreach (var deckAssignment in _context.DeckAssignment.Include(d => d.Deck).Include(d => d.Deck.ApplicationUser).Include(d => d.ApplicationUser).Where(d => d.ApplicationUserId == userId).ToList())
            {
                list.Add(new DeckAssignmentListDTO()
                {
                    Deck = new DeckDTO()
                    {
                        Id = deckAssignment.Deck.Id,
                        Name = deckAssignment.Deck.Name,
                        Status = deckAssignment.Deck.Status,
                        Content = deckAssignment.Deck.Content,
                        Module = deckAssignment.Deck.Module,
                        Deck_number = deckAssignment.Deck.Deck_number,
                        Activated = deckAssignment.Deck.Activated,
                        Activation_time = deckAssignment.Deck.Activation_time,
                        ApplicationUser = new ApplicationUserDTO()
                        {
                            Id = deckAssignment.Deck.ApplicationUser.Id,
                            Name = deckAssignment.Deck.ApplicationUser.Name
                        }
                    },
                    User = new UserDTO()
                    {
                        Id = deckAssignment.ApplicationUser.Id,
                        Name = deckAssignment.ApplicationUser.Name,
                        Email = deckAssignment.ApplicationUser.Email,
                        Roles = GetUserRole(deckAssignment.ApplicationUser.Id).Result
                    }
                });
            }
            return list;
        }

        /// <summary>
        /// This function returns the assignment by userID.
        /// </summary>
        // GET: api/DeckAssignments/1
        [HttpGet("{userId}")]
        public async Task<ActionResult<DeckAssignmentDTO>> GetDeckAssignmentByUserId([FromRoute] string userId)
        {
            _logger.LogInformation("Getting assignment by userId in the GetDeckAssignmentByUserId() method");
            var deckAssignment = await _context.DeckAssignment.Include(d => d.Deck).Include(d => d.ApplicationUser).SingleOrDefaultAsync(c => c.ApplicationUserId == userId);

            if (deckAssignment == null)
            {
                _logger.LogWarning("Assignment not found in the GetDeckAssignmentByUserId() method");
                return NotFound();
            }

            return ItemToDTO(deckAssignment, GetUserRole(deckAssignment.ApplicationUser.Id).Result);
        }

        /// <summary>
        /// This function creates the new assignment.
        /// </summary>
        // POST: api/DeckAssignments
        [Authorize(Roles = "Coordinator,Main Lector,Main Graphic,Main Professional reviewer")]
        [HttpPost]
        public async Task<ActionResult<DeckAssignmentDTO>> CreateDeckAssignment([FromBody] DeckAssignmentDTO deckAssignmentDTO)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("ModelState is invalid in the CreateDeckAssignment() method");
                return BadRequest(ModelState);
            }
            var deckAssignment = new DeckAssignment
            {
                DeckId = deckAssignmentDTO.Deck.Id,
                ApplicationUserId = deckAssignmentDTO.User.Id
            };

            _context.DeckAssignment.Add(deckAssignment);
            await _context.SaveChangesAsync();

            try
            {
                var from = "Tanulókártya";
                var subject = "Tanulókártya hozzárendelés";
                var name = deckAssignmentDTO.User.Name;
                var email = deckAssignmentDTO.User.Email;

                var body = "Kedves <b>" + name + "! </b><br/>" + "A tanulókártya projektben hozzárendeltek egy kártyacsomaghoz!";
                await emailService.SendEmail(from, email, subject, body);
            }
            catch (System.Exception)
            {
                _logger.LogError("BadRequest the CreateDeckAssignment() method");
                return BadRequest(new { message = "SendEmail was not successful. " });
            }

            _logger.LogInformation("Creating assignment in the CreateDeckAssignment() method");
            return CreatedAtAction(nameof(GetDeckAssignmentByUserId), deckAssignment);
        }

        /// <summary>
        /// This function deletes an existing assignment.
        /// </summary>
        // DELETE: api/DeckAssignments/2/userId
        [Authorize(Roles = "Coordinator")]
        [HttpDelete("{deckId}/{userId}")]
        public async Task<ActionResult> DeleteDeckAssignment([FromRoute] int deckId, [FromRoute] string userId)
        {
            var deckAssignment = await _context.DeckAssignment.SingleOrDefaultAsync(d => d.DeckId == deckId && d.ApplicationUserId == userId);

            if (deckAssignment == null)
            {
                _logger.LogWarning("DeckAssignment not found in the DeleteDeckAssignment({0}) method", deckId);
                return NotFound();
            }

            _context.DeckAssignment.Remove(deckAssignment);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleting deck assignment in the DeleteDeckAssignment({0}) method", deckId);
            return NoContent();
        }

        /// <summary>
        /// This function returns the role(s) of a user.
        /// </summary>
        // GET: api/DeckAssignments/GetUserRole/1
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
        /// This function checks to see if a user with the specified role is assigned to the specified package.
        /// </summary>
        // GET: api/DeckAssignments/IsRoleOk/1/Card creator
        [Authorize(Roles = "Coordinator,Main Lector,Main Graphic,Main Professional reviewer")]
        [ActionName("IsRoleOk")]
        [HttpGet("[action]/{deckId}/{roleName}")]
        public async Task<bool> IsRoleOk([FromRoute] int deckId, [FromRoute] string roleName)
        {
            var assignments = await GetDeckAssignmentsByDeck(deckId);
            foreach (var a in assignments.Value)
            {
                var user = await _context.ApplicationUsers.SingleOrDefaultAsync(u => u.Name == a.User.Name);
                var roles = await userManager.GetRolesAsync(user);

                if (a.User.Id == user.Id && a.User.Roles.Contains(roleName))
                {
                    _logger.LogInformation("Return true in the IsRoleOk() method");
                    return true;
                }
            }
            _logger.LogInformation("Return false in the IsRoleOk() method");
            return false;
        }

        #region helperMethods

        /// <summary>
        /// This helper function converts the assignment to a DTO.
        /// </summary>
        private static DeckAssignmentDTO ItemToDTO(DeckAssignment deckAssignment, List<string> roles) =>
        new DeckAssignmentDTO
        {
            Deck = new DeckInfoDTO()
            {
                Id = deckAssignment.Deck.Id,
                Name = deckAssignment.Deck.Name,
                Module = deckAssignment.Deck.Module,
                Status = deckAssignment.Deck.Status,
                Deck_number = deckAssignment.Deck.Deck_number
            },
            User = new UserDTO()
            {
                Id = deckAssignment.ApplicationUser.Id,
                Name = deckAssignment.ApplicationUser.Name,
                Email = deckAssignment.ApplicationUser.Email,
                Roles = roles
            }
        };

        #endregion
    }
}
