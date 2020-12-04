using System.Collections.Generic;

namespace Flashcard.Models.DTO
{
    public class LoginUserDTO
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public List<string> Roles { get; set; }
        public string Token { get; set; }
    }
}
