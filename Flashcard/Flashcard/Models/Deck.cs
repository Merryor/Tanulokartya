using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flashcard.Models
{
    /// <summary>
    /// This class represents the deck entity.
    /// </summary>
    public class Deck
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name field is required.")]
        public string Name { get; set; }

        [StringLength(200)]
        [Required(ErrorMessage = "Content field is required.")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Module field is required.")]
        public Module Module { get; set; }

        [Required(ErrorMessage = "Deck_number field is required.")]
        public int Deck_number { get; set; }

        public bool Activated { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Activation_time { get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public DeckStatus Status { get; set; }

        public ICollection<Card> Cards { get; set; }
    }
}
