using System;

namespace Flashcard.Models.DTO
{
    public class DeckDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Content { get; set; }

        public Module Module { get; set; }

        public int Deck_number { get; set; }
        public DeckStatus Status { get; set; }

        public bool Activated { get; set; }

        public DateTime Activation_time { get; set; }

        public ApplicationUserDTO ApplicationUser { get; set; }
    }
}
