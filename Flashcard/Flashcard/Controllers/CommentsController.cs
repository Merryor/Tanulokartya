using Flashcard.Data;
using Flashcard.Models;
using Flashcard.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flashcard.Controllers
{
    /// <summary>
    /// This class manages Comments.
    /// Contains all methods for handling comments.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CommentsController : ControllerBase
    {

        private readonly ILogger<CommentsController> _logger;
        private readonly IFlashcardDbContext _context;

        public CommentsController(IFlashcardDbContext context, ILogger<CommentsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// This function returns list of comments on the specified card.
        /// </summary>
        // GET: api/Comments/GetCommentsByCardId/1
        [ActionName("GetCommentsByCardId")]
        [HttpGet("[action]/{cardId}")]
        public async Task<ActionResult<IEnumerable<CommentDTO>>> GetComments([FromRoute] int cardId)
        {
            _logger.LogInformation("Listing all comments about a card in the GetComments() method");
            var comments = await _context.Comments.Include(c => c.ApplicationUser)
                .Select(x => ItemToDTO(x))
                .ToListAsync();
            return comments.Where(c => c.CardId == cardId).ToList();
        }

        /// <summary>
        /// This function returns the comment by ID.
        /// </summary>
        // GET: api/Comments/1
        [HttpGet("{id}")]
        public async Task<ActionResult<CommentDTO>> GetComment([FromRoute] int id)
        {
            var comment = await _context.Comments.Include(c => c.Card).SingleOrDefaultAsync(c => c.Id == id);

            if (comment == null)
            {
                _logger.LogWarning("Comment not found in the GetComment({0}) method", id);
                return NotFound();
            }

            _logger.LogInformation("Getting {0}. comment", id);
            return ItemToDTO(comment);
        }

        /// <summary>
        /// This function returns the number of comments on the specified card.
        /// </summary>
        // GET: api/Comments/CountGetComment/2
        [ActionName("CountGetComment")]
        [HttpGet("[action]/{cardId}")]
        public int GetCountCommentsById([FromRoute] int cardId)
        {
            _logger.LogInformation("Count comments by cardId in the GetCountCommentsById() method");
            var comments = _context.Comments.Select(x => ItemToDTO(x)).ToListAsync();
            var selectedComments = comments.Result.Where(c => c.CardId == cardId);

            var countComments = selectedComments.Count();
            if (countComments == 0)
            {
                return 0;
            }
            return countComments;
        }

        /// <summary>
        /// This function creates the new comment.
        /// </summary>
        // POST: api/Comments
        [HttpPost]
        public async Task<ActionResult<CommentDTO>> CreateComment([FromBody] CommentDTO commentDTO)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("ModelState is invalid in the CreateComment() method");
                return BadRequest(ModelState);
            }
            var comment = new Comment
            {
                CardId = commentDTO.CardId,
                ApplicationUserId = commentDTO.User.Id,
                Comment_text = commentDTO.Comment_text,
                Comment_time = DateTime.Now
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Creating comment in the CreateComment() method");

            return CreatedAtAction(nameof(GetComment), new { id = comment.Id }, comment);
        }

        /// <summary>
        /// This function deletes an existing comment.
        /// </summary>
        // DELETE: api/Comments/2
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteComment([FromRoute] int id)
        {
            var comment = await _context.Comments.FindAsync(id);

            if (comment == null)
            {
                _logger.LogWarning("Comment not found in the DeleteComment({0}) method", id);
                return NotFound();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleting comment in the DeleteComment({0}) method", id);
            return NoContent();
        }

        #region helperMethods

        /// <summary>
        /// This helper function converts the comment to a DTO.
        /// </summary>
        private static CommentDTO ItemToDTO(Comment comment) =>
        new CommentDTO
        {
            Id = comment.Id,
            CardId = comment.CardId,
            User = new ApplicationUserDTO()
            {
                Id = comment.ApplicationUserId,
                Name = comment.ApplicationUser.Name
            },
            Comment_text = comment.Comment_text,
            Comment_time = comment.Comment_time
        };

        #endregion
    }
}
