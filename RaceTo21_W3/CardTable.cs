using System;
using System.Collections.Generic;
using System.Linq; // used to order the players for ranking

namespace RaceTo21
{
    /// <summary>
    /// Represents the card table of the game. Asks inputs from players and output information about the game.
    /// </summary>
    public class CardTable
    {
        public CardTable()
        {
            Console.WriteLine("Setting Up Table...");
        }

        /// <summary>
        /// Shows the name of each player and introduces them by their orders that are set at the begging of each round of the game.
        /// Called by Game object.
        /// </summary>
        /// <param name="players">A list of players who are still playing, provided by Game object</param>
        public void ShowPlayers(List<Player> players)
        {
            for (int i = 0; i < players.Count; i++)
            {
                players[i].Introduce(i+1); // List is 0-indexed but user-friendly player positions would start with 1...
            }
        }

        /// <summary>
        /// Gets the user input for number of players. Called by Game object.
        /// </summary>
        /// <returns>Returns a number of players</returns>
        public int GetNumberOfPlayers()
        {
            Console.Write("How many players? ");
            string response = Console.ReadLine();
            int numberOfPlayers;

            // Keep asking until the input is a number between 2~8
            while (int.TryParse(response, out numberOfPlayers) == false
                || numberOfPlayers < 2 || numberOfPlayers > 8)
            {
                Console.WriteLine("Invalid number of players.");
                Console.Write("How many players? ");
                response = Console.ReadLine();
            }
            return numberOfPlayers;
        }

        /// <summary>
        /// Gets the name of the player at the provided position. Called by Game object.
        /// </summary>
        /// <param name="playerNum">The order number represents the position of the player in the game</param>
        /// <returns>Returns the name of the player</returns>
        public string GetPlayerName(int playerNum)
        {
            Console.Write("What is the name of player# " + playerNum + "? ");
            string response = Console.ReadLine();

            // Keep asking until the name is at least 1 character long
            while (response.Length < 1)
            {
                Console.WriteLine("Invalid name.");
                Console.Write("What is the name of player# " + playerNum + "? ");
                response = Console.ReadLine();
            }
            return response;
        }

        /// <summary>
        /// Gets how many times the players need to win to end the game. Called by Game object.
        /// </summary>
        /// <returns>Returns the time to win</returns>
        public int GetTimesToWin()
        {
            Console.Write("How many times do a player have to win to end the game?(3 ~ 6 times) ");
            string response = Console.ReadLine();
            int times;

            // Keep asking until the input is a number between 3~6
            while (int.TryParse(response, out times) == false
                || times < 3 || times > 6)
            {
                Console.WriteLine("Invalid number of times.");
                Console.Write("How many times do a player have to win to end the game?(3 ~ 6 times) ");
                response = Console.ReadLine();
            }
            Console.WriteLine("================================");

            // Print the ending and winning conditions of the game.
            Console.WriteLine($"The game ends when a player 'Wins {times} Times' or 'No More Than 1 Player' wants to keep playing.");
            Console.WriteLine("When the game ends, the player with the most CASH is the final winner!");
            return times;
        }

        /// <summary>
        /// Gets the bets from players accoring to their input. Called by Game object.
        /// </summary>
        /// <param name="currentPlayers">A list of players that are still playing the game</param>
        /// <returns>Returns the amount of bet in the pot if it's valid</returns>
        public int CollectBet(List<Player> currentPlayers)
        {
            Console.WriteLine("The winner of this round wins the total of twice amount of cash every player bets.");
            int pot = 0;
            foreach (Player p in currentPlayers)
            {
                // Keep asking until the input is valid
                while (true)
                {
                    Console.Write(p.Name + ", how much do you want to bet? $");
                    string response = Console.ReadLine();
                    if (int.TryParse(response, out int betAmount))
                    {
                        int collectedBet = p.Bet(betAmount); // Calls Player.bet to get the the bet amount if it's affordable
                        if (collectedBet > 0) // If collected bet is greater than zero, it's valid
                        {
                            pot += collectedBet * 2; // I decided to set the pot to twice amount of the players' bet
                            break; // Break the while loop to stop asking
                        }
                        else if (collectedBet == 0) // Force player to bet an amount of cash
                        {
                            Console.WriteLine("You have to bet an amount.");
                        }
                        else // If the collected bet is less than zero, it means the player doesn't have enough cash.
                        {
                            Console.WriteLine("Your cash: " + p.cash + " is not enough.");
                        }
                    }
                    else // The input is not an integer
                    {
                        Console.WriteLine("Please enter a valid number.");
                    }
                }
            }
            Console.WriteLine($"The total amount in pot is: ${pot}.");
            return pot;
        }

        /// <summary>
        /// Asks the player if they want to get a card. Called by Game object.
        /// </summary>
        /// <param name="player">The player to be asked</param>
        /// <returns>Returns true if the player answer yes correctly(Y/y), false if the player answer no correctly(N/n)</returns>
        public bool OfferACard(Player player)
        {
            // Keep asking until the player enter a valid input
            while (true)
            {
                Console.Write(player.Name + ", do you want a card? (Y/N)");
                string response = Console.ReadLine();
                if (response.ToUpper().StartsWith("Y")) // If gets Y/y
                {
                    return true;
                }
                else if (response.ToUpper().StartsWith("N")) // If gets N/n
                {
                    return false;
                }
                else
                {
                    Console.WriteLine("Please answer Y(es) or N(o)!");
                }
            }
        }

        /// <summary>
        /// Asks each player if they want to keep playing. Called by Game object.
        /// </summary>
        /// <param name="player">The player to be asked</param>
        /// <returns>Returns true if the player answer yes correctly(Y/y), false if the player answer no correctly(N/n)</returns>
        public bool AskNewGame(Player player)
        {
            if (player.cash <= 0) // Don't have to ask if the palyer don't have enough cash for the next game
            {
                Console.WriteLine(player.Name + ", you don't have enough cash for the next game.");
                return false;
            }

            // Keep asking until the player enter a valid input
            while (true)
            {
                Console.Write($"{player.Name} , do you want to keep playing (cash: ${player.cash}, wins: {player.wins})? (Y/N)");
                string response = Console.ReadLine();
                if (response.ToUpper().StartsWith("Y")) // If gets Y/y
                {
                    return true;
                }
                else if (response.ToUpper().StartsWith("N")) // If gets N/n
                {
                    return false;
                }
                else
                {
                    Console.WriteLine("Please answer Y(es) or N(o)!");
                }
            }
        }

        /// <summary>
        /// Prints the cards and the scores in a player's hands. Called by Game and CardTable objects
        /// </summary>
        /// <param name="player">The player who is showing their cards</param>
        public void ShowHand(Player player)
        {
            if (player.cards.Count > 0) // If the player have more than 1 cards
            {
                Console.Write(player.Name + " has: ");

                // Show full names of cards as a comma-separated list
                for (int i = 0; i < player.cards.Count; i++)
                {
                    Console.Write(player.cards[i].DisplayName);
                    Console.Write((i == player.cards.Count - 1) ? " " : ", "); // if it's the last card, don't add a comma after it
                }
                Console.Write("= " + player.score + "/21 ");
                if (player.status != PlayerStatus.active) // Print the player's statis if it's not active
                {
                    Console.Write("(" + player.status.ToString().ToUpper() + ")");
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Prints the cards and the scores in a list of players' hands. Called by Game object.
        /// </summary>
        /// <param name="players">A list of players ready to be printed</param>
        public void ShowHands(List<Player> players)
        {
            Console.WriteLine("Current Card Table: ");
            foreach (Player player in players)
            {
                ShowHand(player);
            }
            Console.WriteLine("================================");
        }

        /// <summary>
        /// Announces a player as the winner of a round and show how much cash they are winning. Called by Game object.
        /// </summary>
        /// <param name="player">The player who wins a round</param>
        /// <param name="winAmount">The amount of cash the winner </param>
        public void AnnounceWinner(Player player, int winAmount)
        {
            Console.WriteLine($"{player.Name} wins ${winAmount}!");
            Console.Write("Press <Enter> to continue... ");
            while (Console.ReadKey().Key != ConsoleKey.Enter) { }
        }

        /// <summary>
        /// Announces the reason why the game ends and the final winner of the game and show how much cash they are winning. Called by Game object
        /// Show the rank of every player. Called by Game object.
        /// </summary>
        /// <param name="players">The list of every player who joins the game at the beginning</param>
        /// <param name="winReasonCode">Code for different reasons of why the game ends</param>
        /// <param name="endGamePlayer">The player who ends the game or doesn't leave the game at the end</param>
        public void AnnounceFinalWinner(List<Player> players, int endReasonCode, Player endGamePlayer)
        { 
            switch (endReasonCode)
            {
                case 1: // Someone wins the amount of time set at the beginning
                    Console.WriteLine($"{endGamePlayer.Name} ends the game by winning {endGamePlayer.wins} times...");
                    break;
                case 2: // Only 1 player who doesn't leave the game still has cash
                    Console.WriteLine($"Game's over because {endGamePlayer.Name} is the only player who hasn't quit and still has cash...");
                    break;
                default: // No enough players
                    Console.WriteLine("================================");
                    Console.WriteLine($"Game's over because there is no more than 1 player wants to keep playing...");
                    break;
            }

            /*
             * Uses System.Linq
             * https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.orderby?view=net-7.0 - OrderByDescending()
             * https://learn.microsoft.com/en-us/dotnet/api/system.linq.queryable.thenby?view=net-7.0 - ThenBy()
             * Uses List.OrderByDescending(player => player.cash) to order the players first according to their cash from most to least, 
             * if the cash is same, uses ThenByDescending(player => player.wins) to keep ordering the players according to their times of win from most to least,
             * if both above are the same, uses ThenBy(player => player.name) to keep ordering the players according to their names alphebatically.
             */
            List<Player> rankedPlayers = players.OrderByDescending(player => player.cash).ThenByDescending(player => player.wins).ThenBy(player => player.Name).ToList();

            // The first player in the ranked list must be the final winner because of winning the most cash
            Console.WriteLine($"\n*** {rankedPlayers[0].Name} is the final winner by winning ${rankedPlayers[0].cash}!! ***");

            Console.WriteLine("\n---------- Rank ------------");          
            for (int i = 0; i < rankedPlayers.Count; i++) // Print all the players according the rank order: cash -> wins -> player name
            {
                Player p = rankedPlayers[i];
                Console.WriteLine($"{i + 1} - {p.Name} (cash: ${p.cash}, wins: {p.wins})");
                
            }
            Console.WriteLine("\n(* Rank order: cash -> wins -> player name *)");

            Console.WriteLine();
            Console.Write("Press <Enter> to continue... ");
            while (Console.ReadKey().Key != ConsoleKey.Enter) { }
        }

        /// <summary>
        /// Prints out message as the Card Table. Called by Game object.
        /// This method is static since I want it to be able to be called anytime without creating any instance.
        /// </summary>
        /// <param name="message">The message ready to be printed</param>
        public static void WriteCardTableMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}