using Flashcard.Models.DTO;
using System.Collections.Generic;
using System.Text;

namespace Flashcard.Utility
{
    /// <summary>
    /// This class creates the html template to generate a pdf.
    /// </summary>
    public class TemplateGenerator
    {
        public TemplateGenerator() { }

        /// <summary>
        /// This function creates the html template to generate a pdf with the data of the specified cards.
        /// </summary>
        public static string GetHTMLString(List<CardDTO> cardList)
        {
            var sb = new StringBuilder();
            sb.Append(@"
            <html>
                <head>
                </head>
                    <body>
                        <div class='container'>
            ");

            foreach (var card in cardList)
            {
                if (card.Card_number == 0)
                {
                    if (card.Question_picture != "")
                    {
                        sb.AppendFormat(@"
                        <div class='card card_question'>
                        <div class='card-content'>
                        <div class='card-header'></div>
                          <div class='card-body initial-card'>
                                {0}<br/>{1}    
                              ", card.Question_text, "<img src='http://localhost:4200/" + card.Question_picture + "'/>" +
                       "</div></div>" +
                        "</div>");
                    }
                    else
                    {
                        sb.AppendFormat(@"
                        <div class='card card_question'>
                        <div class='card-content'>
                        <div class='card-header'></div>
                          <div class='card-body initial-card'>
                                {0}  
                              ", card.Question_text +
                          "</div></div>" +
                       "</div>");
                    }                    
                }
                else if(card.Question_picture != "")
                {
                    sb.AppendFormat(@"
                    <div class='card card_question'>
                    <div class='card-content'>
                    <div class='card-header'>{0}{1}/K{2}</div>
                      <div class='card-body'>
                            {3}<br/>{4}    
                          ", card.Deck.Deck_number, card.Deck.Module, card.Card_number, card.Question_text, "<img src='http://localhost:4200/" + card.Question_picture + "'/>" +
                   "</div></div>" +
                    "</div>");
                } else
                {
                    sb.AppendFormat(@"
                <div class='card card_question'>
                    <div class='card-content'>
                    <div class='card-header'>{0}{1}/K{2}</div>
                      <div class='card-body'>
                            <br/><br/>{3}
                          ", card.Deck.Deck_number, card.Deck.Module, card.Card_number, card.Question_text +
                    "</div></div>" +
                "</div>");
                }                
            }

            sb.Append(@"</div>
            <div class='tabbed-content'>
                <div class='break-after'>Kérdések vége</div>
            </div>
            <div class='container'>");

            foreach (var card in cardList)
            {
                if (card.Card_number == 0)
                {
                    if (card.Answer_picture != "")
                    {
                        sb.AppendFormat(@"                
                        <div class='card card_answer'>
                            <div class='card-content'>
                            <div class='card-header'></div>
                              <div class='card-body initial-card'>
                                    {0}<br/>{1}
                              ", card.Answer_text, "<img src = 'http://localhost:4200/" + card.Answer_picture + "'/>" +
                            "</div></div>" +
                        "</div>");
                    }
                    else
                    {
                        sb.AppendFormat(@"
                        <div class='card card_answer'>
                        <div class='card-content'>
                        <div class='card-header'></div>
                          <div class='card-body initial-card'>
                                {0}  
                              ", card.Answer_text +
                         "</div></div>" +
                      "</div>");
                    }                   
                }
                else if (card.Answer_picture != "")
                {
                    sb.AppendFormat(@"                
                    <div class='card card_answer'>
                        <div class='card-content'>
                        <div class='card-header'>{0}{1}/V{2}</div>
                          <div class='card-body'>
                                {3}<br/>{4}
                          ", card.Deck.Deck_number, card.Deck.Module, card.Card_number, card.Answer_text, "<img src = 'http://localhost:4200/" + card.Answer_picture + "'/>" +
                        "</div></div>" +
                    "</div>");
                } else
                {
                    sb.AppendFormat(@"                
                    <div class='card card_answer'>
                        <div class='card-content'>
                        <div class='card-header'>{0}{1}/V{2}</div>
                          <div class='card-body'>
                                <br/><br/>{3}
                          ", card.Deck.Deck_number, card.Deck.Module, card.Card_number, card.Answer_text +
                       "</div></div>" +
                   "</div>");
                }
            }

            sb.Append(@"
                    </div>
                </body>
            </html>
           ");

            return sb.ToString();
        }
    }
}
