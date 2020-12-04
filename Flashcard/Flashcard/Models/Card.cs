using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flashcard.Models
{
    /// <summary>
    /// This class represents the card entity.
    /// </summary>
    public class Card
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(200)]
        [Required(ErrorMessage = "Type field is required.")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Card_number field is required.")]
        public int Card_number { get; set; }

        [Required(ErrorMessage = "Question_text field is required.")]
        public string Question_text { get; set; }

        public string Question_picture { get; set; }

        [Required(ErrorMessage = "Answer_text field is required.")]
        public string Answer_text { get; set; }

        public string Answer_picture { get; set; }

        public int DeckId { get; set; }
        public Deck Deck { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}
