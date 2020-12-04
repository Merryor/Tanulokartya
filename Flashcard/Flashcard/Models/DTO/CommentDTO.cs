using System;

namespace Flashcard.Models.DTO
{
    public class CommentDTO
    {
        public int Id { get; set; }

        public int CardId { get; set; }

        public ApplicationUserDTO User { get; set; }

        public string Comment_text { get; set; }

        public DateTime Comment_time { get; set; }

    }
}
