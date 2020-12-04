namespace Flashcard.Models
{
    /// <summary>
    /// This class represents the deck assignment entity.
    /// </summary>
    public class DeckAssignment
    {
        public int DeckId { get; set; }
        public Deck Deck { get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

    }
}
