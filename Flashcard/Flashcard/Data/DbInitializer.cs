using Flashcard.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;

namespace Flashcard.Data
{
    /// <summary>
    /// This class handles filling the empty database with initial values.
    /// </summary>
    public class DbInitializer
    {
        /// <summary>
        /// This function initializes the initial values.
        /// </summary>
        public static void Initialize(FlashcardDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            context.Database.EnsureCreated();

            if (context.Decks.Any())
            {
                return;
            }

            //Roles
            var roles = new IdentityRole[]
            {
               new IdentityRole {
                    Name = "Administrator",
                },
                new IdentityRole
                {
                     Name = "Card creator",
                },
                new IdentityRole
                {
                     Name = "Coordinator",
                },
                new IdentityRole
                {
                     Name = "Main Lector",
                },
                new IdentityRole
                {
                     Name = "Lector",
                },
                new IdentityRole
                {
                     Name = "Main Professional reviewer",
                },
                new IdentityRole
                {
                     Name = "Professional reviewer",
                },
                new IdentityRole
                {
                     Name = "Main Graphic",
                },
                new IdentityRole
                {
                     Name = "Graphic",
                },                
            };
            foreach (IdentityRole r in roles)
            {
                var result = roleManager.CreateAsync(r);
                result.Wait();
            }
            //context.SaveChanges();

            //Users
            var users = new ApplicationUser[]
            {
               new ApplicationUser
                {
                    Name = "Admin User",
                    Phone = "06301111111",
                    Email = "admin@gmail.com",
                    UserName = "admin@gmail.com",
                    PasswordHash = "admin",
                    Workplace = "Admin Általános Iskola",
                    Create_module = Module.A,
                    Will_create_module = Module.A,
                    Activated = true
                },
               new ApplicationUser
                {
                    Name = "Kártyakészítő User",
                    Phone = "06202222222",
                    Email = "kartyakeszito@gmail.com",
                    UserName = "kartyakeszito@gmail.com",
                    PasswordHash = "kartyakeszito",
                    Workplace = "Kártyakészítő Általános Iskola",
                    Create_module = Module.B,
                    Will_create_module = Module.B,
                    Activated = true
                },
               new ApplicationUser
                {
                    Name = "Koordinátor User",
                    Phone = "06303333333",
                    Email = "koordinator@gmail.com",
                    UserName = "koordinator@gmail.com",
                    PasswordHash = "koordinator",
                    Workplace = "Koordinátor Általános Iskola",
                    Create_module = Module.C,
                    Will_create_module = Module.D,
                    Activated = true
                },
               new ApplicationUser
                {
                    Name = "Főlektor User",
                    Phone = "06304444444",
                    Email = "folektor@gmail.com",
                    UserName = "folektor@gmail.com",
                    PasswordHash = "folektor",
                    Workplace = "Főlektor Általános Iskola",
                    Create_module = Module.C,
                    Will_create_module = Module.D,
                    Activated = true
                },
            };

            int i = 0;
            foreach (ApplicationUser a in users)
            {
                context.ApplicationUsers.Add(a);
                var result = userManager.CreateAsync(a, a.PasswordHash);
                result.Wait();
                var roleResult = userManager.AddToRoleAsync(a, roles[i].Name);
                i++;
                roleResult.Wait();
            }
            context.SaveChanges();

            //Decks
            var decks = new Deck[]
            {
                new Deck
                {
                    Name = "Évszakok (próbacsomag)",
                    Content = "Az évszakok, iskolába megyek, válogatások",
                    Module = Module.A,
                    Deck_number = 1,
                    Activated = true,
                    Activation_time = DateTime.Parse("2020-07-06"),
                    ApplicationUserId = context.ApplicationUsers.FirstOrDefault().Id,
                    Status = DeckStatus.Approved
                },
            };

            foreach (Deck d in decks)
            {
                context.Decks.Add(d);
            }
            context.SaveChanges();

            //Cards
            var cards = new Card[]
            {
               //1. csomag
                new Card
                {
                    Type = "Kérdés-felelet",
                    Card_number = 1,
                    Question_text = "Mit látsz az avarban?",
                    Question_picture = "Resources\\Images\\sün.jpg",
                    Answer_text = "Sün",
                    Answer_picture = "Resources\\Images\\sün.jpg",
                    DeckId = 1
                },
               new Card
                {
                    Type = "Kérdés-felelet",
                    Card_number = 2,
                    Question_text = "Melyik gyümölcs a kakukktojás? Miért?",
                    Question_picture = "Resources\\Images\\gyümölcs.png",
                    Answer_text = "Cseresznye, mert nyári gyümölcs.",
                    Answer_picture = "Resources\\Images\\gy_5.png",
                    DeckId = 1
                },
               new Card
                {
                    Type = "Kérdés-felelet",
                    Card_number = 3,
                    Question_text = "Mi nem való az íróasztalra?",
                    Question_picture = "Resources\\Images\\a.png",
                    Answer_text = "Szendvics",
                    Answer_picture = "Resources\\Images\\a_2.png",
                    DeckId = 1
                },
               new Card
                {
                    Type = "Kérdés-felelet",
                    Card_number = 4,
                    Question_text = "Melyik falevél ugyanolyan, mint az első?",
                    Question_picture = "Resources\\Images\\levél.png",
                    Answer_text = "Az utolsó",
                    Answer_picture = "Resources\\Images\\levél_1.png",
                    DeckId = 1
                },
               new Card
                {
                    Type = "Kérdés-felelet",
                    Card_number = 5,
                    Question_text = "Hányadik mókus különbözik a többitől?",
                    Question_picture = "Resources\\Images\\mókus.png",
                    Answer_text = "A második. (2.)",
                    Answer_picture = "Resources\\Images\\mokus_2.png",
                    DeckId = 1
                },
               new Card
                {
                    Type = "Kérdés-felelet",
                    Card_number = 6,
                    Question_text = "Melyik állat részleteit látod?",
                    Question_picture = "Resources\\Images\\őz.png",
                    Answer_text = "Az őz",
                    Answer_picture = "Resources\\Images\\őz_válasz.png",
                    DeckId = 1
                },
               new Card
                {
                    Type = "Tevékenykedtető",
                    Card_number = 7,
                    Question_text = "Húzd végig a mutatóujjad a csigavonalon! A zöldnél kezdj!",
                    Question_picture = "Resources\\Images\\csigavonal.png",
                    Answer_text = "-",
                    Answer_picture = "Resources\\Images\\csigavonal.png",
                    DeckId = 1
                },
               new Card
                {
                    Type = "Kérdés-felelet",
                    Card_number = 8,
                    Question_text = "Melyik formához hasonlítható a dió?",
                    Question_picture = "Resources\\Images\\forma.png",
                    Answer_text = "A gömb alakhoz.",
                    Answer_picture = "Resources\\Images\\forma_2.png",
                    DeckId = 1
                },
               new Card
                {
                    Type = "Kérdés-felelet",
                    Card_number = 9,
                    Question_text = "Se ablaka, se ajtaja, mégis négyen laknak benne. Mi az?",
                    Question_picture = "",
                    Answer_text = "A dió",
                    Answer_picture = "Resources\\Images\\dio.png",
                    DeckId = 1
                },
               new Card
                {
                    Type = "Tevékenykedtető",
                    Card_number = 10,
                    Question_text = "Ez a Te kártyád! Mondd a mondókát, és kísérd mozdulatokkal! Ereszkedik le a felhő, hull a fára őszi eső. Hull a fának a levele, mégis szól a fülemüle.",
                    Question_picture = "",
                    Answer_text = "-",
                    Answer_picture = "Resources\\Images\\mondoka.png",
                    DeckId = 1
                },
            };

            foreach (Card c in cards)
            {
                context.Cards.Add(c);
            }
            context.SaveChanges();
        }
    }
}
