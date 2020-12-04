using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Flashcard.Models
{
    /// <summary>
    /// This class represents the user entity.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "Name field is required.")]
        public string Name { get; set; }

        [Phone(ErrorMessage = "User phone is not a valid phone number.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Workplace field is required.")]
        public string Workplace { get; set; }

        [Required(ErrorMessage = "Create Module field is required.")]
        public Module Create_module { get; set; }

        [Required(ErrorMessage = "Will Create Module field is required.")]
        public Module Will_create_module { get; set; }

        public bool Activated { get; set; }
        public string Token { get; set; }

        public ICollection<Deck> Decks { get; set; }
    }
}
