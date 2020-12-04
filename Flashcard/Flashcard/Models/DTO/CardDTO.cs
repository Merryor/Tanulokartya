namespace Flashcard.Models.DTO
{
    public class CardDTO
    {
        public int Id { get; set; }

        public string Type { get; set; }

        public int Card_number { get; set; }

        public string Question_text { get; set; }

        public string Question_picture { get; set; }

        public string Answer_text { get; set; }

        public string Answer_picture { get; set; }

        public DeckInfoDTO Deck { get; set; }
    }
}
