using System;
using System.Collections.Generic;
using System.Linq; // currently only needed if we use alternate shuffle method

namespace RaceTo21
{
    public class Deck
    {
        List<Card> cards = new List<Card>();
        public Dictionary<string, string> cardImageName = new Dictionary<string, string>();

        public Deck()
        {
            Console.WriteLine("Building Deck...");
            string[] suits = { "Spades", "Hearts", "Clubs", "Diamonds" };

            for (int cardVal = 1; cardVal <= 13; cardVal++)
            {
                foreach (string cardSuit in suits)
                {
                    string cardName;
                    string cardImgNumber;
                    switch (cardVal)
                    {
                        case 1:
                            cardName = "Ace";
                            cardImgNumber = cardName.Substring(0,1);
                            break;
                        case 11:
                            cardName = "Jack";
                            cardImgNumber = cardName.Substring(0, 1);
                            break;
                        case 12:
                            cardName = "Queen";
                            cardImgNumber = cardName.Substring(0, 1);
                            break;
                        case 13:
                            cardName = "King";
                            cardImgNumber = cardName.Substring(0, 1);
                            break;
                        default:
                            cardName = cardVal.ToString();
                            cardImgNumber = cardVal.ToString().PadLeft(2, '0');
                            break;
                    }
                    string shortCardName = (cardName == "10") ? "10" : cardName.Substring(0,1);
                    cards.Add(new Card($"{shortCardName}{cardSuit.Substring(0, 1)}", $"{cardName} of {cardSuit}"));

                    // dictionary for card image file names
                    cardImageName[$"{shortCardName}{cardSuit.Substring(0, 1)}"] = $"card_{cardSuit.ToLower()}_{cardImgNumber}.png";
                }
            }
        }

        public void Shuffle()
        {
            Console.WriteLine("Shuffling Cards...");

            Random rng = new Random();

            // one-line method that uses Linq:
            // cards = cards.OrderBy(a => rng.Next()).ToList();

            // multi-line method that uses Array notation on a list!
            // (this should be easier to understand)
            for (int i=0; i<cards.Count; i++)
            {
                Card tmp = cards[i];
                int swapindex = rng.Next(cards.Count);
                cards[i] = cards[swapindex];
                cards[swapindex] = tmp;
            }
        }

        /* Maybe we can make a variation on this that's more useful,
         * but at the moment it's just really to confirm that our 
         * shuffling method(s) worked! And normally we want our card 
         * table to do all of the displaying, don't we?!
         */
        public void ShowAllCards()
        {
            for (int i = 0; i < cards.Count; i++)
            {
                // print the index, short name, and image file name of the cards
                Console.Write(i + ":" + cards[i].id + $"({cardImageName[cards[i].id]})"); // a list property can look like an Array!
                if (i < cards.Count -1)
                {
                    Console.Write(" ");
                } else
                {
                    Console.WriteLine("");
                }
            }

            /*
            foreach(string img in cardImageName.Values) // print card image file names with dictionary
            {
                Console.WriteLine(img);
            }
            */

        }

        public Card DealTopCard()
        {
            Card card = cards[cards.Count - 1];
            cards.RemoveAt(cards.Count - 1);
            // Console.WriteLine("I'm giving you " + card);
            return card;
        }
    }
}

