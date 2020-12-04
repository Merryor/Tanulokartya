namespace Flashcard.Models.DTO
{
    public class DeckInfoDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public Module Module { get; set; }

        public DeckStatus Status { get; set; }
        public int Deck_number { get; set; }
    }
}
