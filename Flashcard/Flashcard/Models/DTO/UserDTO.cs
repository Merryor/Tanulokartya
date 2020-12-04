using System.Collections.Generic;

namespace Flashcard.Models.DTO
{
    public class UserDTO
    {
        public string Id { get; set; }

        public string Name { get; set; }
        
        public string Email { get; set; }

        public List<string> Roles { get; set; }
    }
}
