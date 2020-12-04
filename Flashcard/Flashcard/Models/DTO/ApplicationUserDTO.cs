using System.Collections.Generic;

namespace Flashcard.Models.DTO
{
    public class ApplicationUserDTO
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }

        public string Workplace { get; set; }

        public Module Create_module { get; set; }

        public Module Will_create_module { get; set; }
        public List<string> Roles { get; set; }

        public bool Activated { get; set; }
    }
}
