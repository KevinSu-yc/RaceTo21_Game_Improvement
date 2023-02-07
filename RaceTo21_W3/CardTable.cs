using System;
using System.Collections.Generic;

namespace RaceTo21
{
    public class CardTable
    {
        public CardTable()
        {
            Console.WriteLine("Setting Up Table...");
        }

        /* Shows the name of each player and introduces them by table position.
         * Is called by Game object.
         * Game object provides list of players.
         * Calls Introduce method on each player object.
         */
        public void ShowPlayers(List<Player> players)
        {
            for (int i = 0; i < players.Count; i++)
            {
                players[i].Introduce(i+1); // List is 0-indexed but user-friendly player positions would start with 1...
            }
        }

        /* Gets the user input for number of players.
         * Is called by Game object.
         * Returns number of players to Game object.
         */
        public int GetNumberOfPlayers()
        {
            Console.Write("How many players? ");
            string response = Console.ReadLine();
            int numberOfPlayers;
            while (int.TryParse(response, out numberOfPlayers) == false
                || numberOfPlayers < 2 || numberOfPlayers > 8)
            {
                Console.WriteLine("Invalid number of players.");
                Console.Write("How many players?");
                response = Console.ReadLine();
            }
            return numberOfPlayers;
        }

        /* Gets the name of a player
         * Is called by Game object
         * Game object provides player number
         * Returns name of a player to Game object
         */
        public string GetPlayerName(int playerNum)
        {
            Console.Write("What is the name of player# " + playerNum + "? ");
            string response = Console.ReadLine();
            while (response.Length < 1)
            {
                Console.WriteLine("Invalid name.");
                Console.Write("What is the name of player# " + playerNum + "? ");
                response = Console.ReadLine();
            }
            return response;
        }

        /* get bet from player accoring to their input
         * called by Game object
         * returns the amount of bet if it's valid
         */
        public int CollectBet(Player player)
        {
            while(true)
            {
                Console.Write(player.name + ", how much do you want to bet? ");
                string response = Console.ReadLine();
                if (int.TryParse(response, out int betAmount))
                {
                    int collectedBet = player.bet(betAmount);
                    if (collectedBet > 0)
                    {
                        return collectedBet;
                    }
                    else if (collectedBet == 0)
                    {
                        Console.WriteLine("You have to bet an amount.");
                    }
                    else
                    {
                        Console.WriteLine("Your cash: " + player.cash + " is not enough.");
                    }
                } 
                else
                {
                    Console.WriteLine("Please enter a valid number.");
                }
            }
        }

        /* Pay the player from the pot
         * Called by Game object
         */
        public void Pay(Player player, int cashAmount)
        {
            player.cash += cashAmount;
        }

        public bool OfferACard(Player player)
        {
            while (true)
            {
                Console.Write(player.name + ", do you want a card? (Y/N)");
                string response = Console.ReadLine();
                if (response.ToUpper().StartsWith("Y"))
                {
                    return true;
                }
                else if (response.ToUpper().StartsWith("N"))
                {
                    return false;
                }
                else
                {
                    Console.WriteLine("Please answer Y(es) or N(o)!");
                }
            }
        }

        /* Ask each player if they want to keep playing
         * Called by Game object
         * Return a bool represent whether the player wants to keep playing
         */
        public bool AskNewGame(Player player)
        {
            if (player.cash <= 0)
            {
                Console.WriteLine(player.name + ", you don't have enough cash for the next game.");
                return false;
            }

            while (true)
            {
                Console.Write($"{player.name} , do you want to keep playing (cash: {player.cash}$)? (Y/N)");
                string response = Console.ReadLine();
                if (response.ToUpper().StartsWith("Y"))
                {
                    return true;
                }
                else if (response.ToUpper().StartsWith("N"))
                {
                    return false;
                }
                else
                {
                    Console.WriteLine("Please answer Y(es) or N(o)!");
                }
            }
        }

        public void ShowHand(Player player)
        {
            if (player.cards.Count > 0)
            {
                Console.Write(player.name + " has: ");
                // foreach (Card card in player.cards)
                // {
                //     Console.Write(card.displayName + " ");
                // }

                // Show full names of cards as a comma-separated list
                for (int i = 0; i < player.cards.Count; i++)
                {
                    Console.Write(player.cards[i].displayName);
                    Console.Write((i == player.cards.Count - 1) ? " " : ", "); // if it's the last card, don't add a comma after it
                }

                Console.Write("= " + player.score + "/21 ");
                if (player.status != PlayerStatus.active)
                {
                    Console.Write("(" + player.status.ToString().ToUpper() + ")");
                }
                Console.WriteLine();
            }
        }

        public void ShowHands(List<Player> players)
        {
            foreach (Player player in players)
            {
                ShowHand(player);
            }
        }

        public void AnnounceWinner(Player player, int winAmount)
        {
            if (player != null)
            {
                Console.WriteLine(winAmount > 0 ? $"{player.name} wins {winAmount}$!" : $"{player.name} wins!");
            }
            else
            {
                Console.WriteLine("No more players!");
            }
            Console.Write("Press <Enter> to continue... ");
            while (Console.ReadKey().Key != ConsoleKey.Enter) { }
        }

        public void ResetPlayers(List<Player> players)
        {
            foreach (Player player in players)
            {
                player.cards = new List<Card>();
                player.status = PlayerStatus.active;
                player.score = 0;
            }
        }
    }
}