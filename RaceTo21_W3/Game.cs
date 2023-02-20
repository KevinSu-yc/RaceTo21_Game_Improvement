using System;
using System.Collections.Generic;

namespace RaceTo21
{
    /// <summary>
    /// Represents the whole RaceTo21 game
    /// </summary>
    public class Game
    {
        // The CardTable and the cheating mode don't have to be reachable in other classes so I set it as totally private
        private CardTable cardTable; // object in charge of displaying game information
        private bool cheating = false; // lets you cheat for testing purposes if true

        // These fields should be read-only outside of Game object so other classes can't affect the game process.
        public List<Player> Players { get; private set; } = new List<Player>(); // list of objects containing player data
        public Deck Deck { get; private set; } = new Deck(); // deck of cards
        public int NumberOfPlayers { get; private set; } // number of players in current game
        public int CurrentPlayer { get; private set; } = 0; // current player on list
        public int CurrentPot { get; private set; } = 0; // amount of bet put in current pot
        public int TimesToWin { get; private set; } = 3;
        public Task NextTask { get; private set; } // keeps track of game state

        /// <summary>
        /// Sets up the card table and suffle the deck while creaing the game. Initiated the task to starts the game process.
        /// Called by Program.
        /// </summary>
        /// <param name="c">The card table set up for the game</param>
        public Game(CardTable c)
        {
            cardTable = c;
            Deck.Shuffle();
            // deck.ShowAllCards();
            NextTask = Task.GetNumberOfPlayers;
        }

        /// <summary>
        /// Creates a player to add to the current game. Called by DoNextTask() method after getting the player names.
        /// </summary>
        /// <param name="n">Names for creating a Player object</param>
        public void AddPlayer(string n)
        {
            Players.Add(new Player(n));
        }

        /// <summary>
        /// Figures out what task to do next in game as represented by field nextTask.
        /// Calls methods required to complete task then sets nextTask.
        /// </summary>
        public void DoNextTask()
        {
            CardTable.WriteCardTableMessage("================================"); // Devider that seperates each phase of the game
            if (NextTask == Task.GetNumberOfPlayers) // Gets the number of players that are joining the game
            {
                NumberOfPlayers = cardTable.GetNumberOfPlayers(); 
                NextTask = Task.AskTimesToWin;
            }
            else if (NextTask == Task.AskTimesToWin) // Gets the amount of times that players have to win to end the game
            {
                TimesToWin = cardTable.GetTimesToWin();
                NextTask = Task.GetNames;
            }
            else if (NextTask == Task.GetNames) // Gets the names for every player
            {
                for (int count = 1; count <= NumberOfPlayers; count++)
                {
                    string name = cardTable.GetPlayerName(count);

                    if (Players.Find(p => p.Name == name) != null) // If there's a player in the player list has the same name as the current input
                    {
                        CardTable.WriteCardTableMessage("The name is used.");
                        count--; // count minus 1 to repeat this iteration, ask for input until the name is unique
                    }
                    else // the input name is unique
                    {
                        AddPlayer(name); // NOTE: player list will start from 0 index even though we use 1 for our count here to make the player numbering more human-friendly
                    }
                }
                NextTask = Task.IntroducePlayers;
            }
            else if (NextTask == Task.IntroducePlayers) // Prints out the information of the player that is still playing
            {
                cardTable.ShowPlayers(Players.FindAll(player => player.status != PlayerStatus.quit)); // Uses players.FindAll() to get the players who haven't quit
                NextTask = Task.AskBet;
            }
            else if (NextTask == Task.AskBet) // Asks players to bet an amount of cash
            {
                CurrentPot = cardTable.CollectBet(Players.FindAll(player => player.status != PlayerStatus.quit)); // Asks and collect cash from the players who haven't quit
                NextTask = Task.OfferFirstCard;
            }
            else if (NextTask == Task.OfferFirstCard) // Forces to give every player a card at the beginning
            {
                CardTable.WriteCardTableMessage("Giving the players their first card..."); // This is same as Console.WriteLine() but I want it to be more like the card table is showing the message
                
                foreach (Player p in Players.FindAll(player => player.status != PlayerStatus.quit)) // Loop through the players who haven't quit 
                {
                    // offer a card to each player and calculate the score
                    Card card = Deck.DealTopCard();
                    p.cards.Add(card);
                    p.score = ScoreHand(p);
                }
                NextTask = Task.PlayerTurn;
            }
            else if (NextTask == Task.PlayerTurn) // Check players actions in their turns
            {
                // Get the current player by the position from the players who haven't quit
                Player player = Players.FindAll(player => player.status != PlayerStatus.quit)[CurrentPlayer];

                if (player.status == PlayerStatus.active) // If the player can still get a card, ask if they want to
                {
                    // Show every player's cards to help the current player decides if they want to get a card
                    cardTable.ShowHands(Players.FindAll(player => player.status != PlayerStatus.quit));

                    if (cardTable.OfferACard(player)) // If the player wants to get a card
                    {
                        // Offer a card from the Deck to the player and calculate the scores
                        Card card = Deck.DealTopCard();
                        player.cards.Add(card);
                        player.score = ScoreHand(player);

                        if (player.score > 21) // If the total score is over 21, the player is busted
                        {
                            player.status = PlayerStatus.bust;
                        }
                        else if (player.score == 21) // If the total score is over 21, the player wins
                        {
                            player.status = PlayerStatus.win;
                        }
                        cardTable.ShowHand(player); // Show the current player's card to see what card does they get
                    }
                    else // If the player doesn't want a card, set their status as stay
                    {
                        player.status = PlayerStatus.stay;
                        CardTable.WriteCardTableMessage($"{player.Name} decides to stay...");
                    }
                }
                else // If the player is busted or chose to stay in their previous turn
                {
                    // Don't print out every player and just show a message about skipping the player's turn
                    CardTable.WriteCardTableMessage($"Skipping {player.Name}'s turn({player.status.ToString().ToUpper()})...");
                }

                CurrentPlayer++; // add 1 to currentPlayer to get to the next player's position
                if (CurrentPlayer > Players.FindAll(player => player.status != PlayerStatus.quit).Count - 1)
                {
                    CurrentPlayer = 0; // back to the first player who hasn't quit the game
                }

                NextTask = Task.CheckForEnd;
            }
            else if (NextTask == Task.CheckForEnd) // After every player's turn, check if there's a winner occurs
            {
                if (CheckWinner()) // If there is a winner
                {
                    // Determine whose the winner from the players who hasn't quit
                    Player winner = DoFinalScoring(Players.FindAll(player => player.status != PlayerStatus.quit));

                    // The winner gets the cash from pot and gets a win
                    winner.wins++;
                    Pay(winner, CurrentPot);

                    // The card table announce the winner for the current round and how much they win
                    cardTable.AnnounceWinner(winner, CurrentPot);
                    CurrentPot = 0; // Reset the pot

                    NextTask = Task.CheckForNewGame;
                }
                else // If there's no winner yet
                {
                    CardTable.WriteCardTableMessage("Next player...");
                    NextTask = Task.PlayerTurn; // return to player turn phase
                }
            }
            else if (NextTask == Task.CheckForNewGame) // After a winner for a round appears, ask players if they want to keep playing
            {
                // If there is only 1 player who havn't left the game still have cash
                if (Players.FindAll(player => player.status != PlayerStatus.quit).FindAll(player => player.cash > 0).Count == 1) 
                {
                    // Card table announce the player who wins the most cash as the final winner
                    // (case 2) Only 1 player who doesn't leave the game still has cash
                    cardTable.AnnounceFinalWinner(Players, 2, Players.FindAll(player => player.status != PlayerStatus.quit).Find(player => player.cash > 0));
                    NextTask = Task.GameOver;
                }
                else // There are still more than 1 players in current round and they all still have cash
                {
                    Player endGamePlayer = Players.Find(player => player.wins == TimesToWin); // Gets the player who wins enough times to end the game
                    if (endGamePlayer == null) // If there isn't a player who wins enough times
                    {
                        // Ask each one of the players in the current round if they want to keep playing
                        foreach (Player p in Players.FindAll(player => player.status != PlayerStatus.quit))
                        {
                            if (!cardTable.AskNewGame(p)) // if the player don't want to keep playing, set their status as quit
                            {
                                p.status = PlayerStatus.quit;
                            }
                        }

                        ResetPlayers(); // reset player's cards, card points, and status
                        if (Players.FindAll(player => player.status != PlayerStatus.quit).Count <= 1) // if no more than 1 player wants to keep playing, end the game
                        {
                            // Card table announce the player who wins the most cash as the final winner
                            // (case 3) // No enough players
                            cardTable.AnnounceFinalWinner(Players, 3, null); 
                            NextTask = Task.GameOver;
                        }
                        else // more than 1 players wants to keep playing
                        {
                            // Reset the variable and the deck for next game
                            CurrentPlayer = 0;
                            RearrangePlayers();
                            Deck = new Deck();
                            Deck.Shuffle();

                            NextTask = Task.IntroducePlayers; // Return to introduce phase
                        }
                    }
                    else
                    {
                        // Card table announce the player who wins the most cash as the final winner
                        // (case 1) Someone wins the amount of time set at the beginning
                        cardTable.AnnounceFinalWinner(Players, 1, endGamePlayer);
                        NextTask = Task.GameOver;
                    }
                }
            }
            else // we shouldn't get here...
            {
                CardTable.WriteCardTableMessage("Game crashed because of unexpected issues :(");
                NextTask = Task.GameOver;
            }
        }

        /// <summary>
        /// Calculates the scores according to the cards in the player's hand.
        /// </summary>
        /// <param name="player">The player whose cards are ready to be calculated</param>
        /// <returns>Returns the total score in the player's hand</returns>
        public int ScoreHand(Player player)
        {
            int score = 0;
            if (cheating == true && player.status == PlayerStatus.active) // Cheat mode, assign the score to players directly
            {
                string response = null;
                while (int.TryParse(response, out score) == false)
                {
                    Console.Write("OK, what should player " + player.Name + "'s score be?");
                    response = Console.ReadLine();
                }
                return score;
            }
            else // Not in cheat mode
            {
                // loop through all the cards and sum up their values
                foreach (Card card in player.cards)
                {
                    string faceValue = card.Id.Remove(card.Id.Length - 1); // Gets the card value from the card's short name
                    switch (faceValue)
                    {
                        // K, Q, J, add 10 to score
                        case "K":
                        case "Q":
                        case "J":
                            score = score + 10;
                            break;
                        // A, add 1 to score
                        case "A":
                            score = score + 1;
                            break;
                        // If the card value is number, convert it to integer and add it to score
                        default:
                            score = score + int.Parse(faceValue);
                            break;
                    }
                }
            }
            return score;
        }

        /// <summary>
        /// Checks if there's a winner at the end of each turn.
        /// </summary>
        /// <returns>Return treu if there's a winner, otherwise, returns false</returns>
        public bool CheckWinner()
        {
            int bustedPlayers = 0; // Count the number of busted players for every round
            int activePlayers = 0; // Count the number of active players for every round

            // Loops throuh all the players who haven't quit
            foreach (var player in Players.FindAll(player => player.status != PlayerStatus.quit))
            {
                if (player.status == PlayerStatus.win) // If theres a player's status is win, there must be a winner
                {
                    return true;
                }
                else if (player.status == PlayerStatus.active) // If the player's status is active
                {
                    activePlayers++; // Add a count and keep looping
                }
                else if (player.status == PlayerStatus.bust) // If the player's status is bust
                {
                    bustedPlayers++; // Add a count and keep looping
                }
            }

            // If there's only 1 player isn't busted, there must be a winner
            if (bustedPlayers == (Players.FindAll(player => player.status != PlayerStatus.quit).Count - 1))
            {
                return true;
            }

            // If there's at least 1 player still active, there's no winner
            if (activePlayers > 0)
            {
                return false;
            }

            // At this point, every player should be stayed, compare the score and announce the winner
            return true; 
        }

        /// <summary>
        /// Compare the scores and find out whose the winner
        /// </summary>
        /// <param name="currentPlayers">The players who haven't quit</param>
        /// <returns>Returns the winner</returns>
        public Player DoFinalScoring(List<Player> currentPlayers)
        {
            int highScore = 0; // Keep track of the highest score
            int bustedPlayers = 0; // Count the number of busted players for every round
            foreach (var player in currentPlayers)
            {
                cardTable.ShowHand(player);
                if (player.status == PlayerStatus.win) // return the player whose status is win as the winner immediately
                {
                    return player;
                }
                if (player.status == PlayerStatus.stay) // if the player's status is stay
                {
                    // Replace the highest score if the player's score is higher
                    if (player.score > highScore)
                    {
                        highScore = player.score;
                    }
                }
                if (player.status == PlayerStatus.bust) // keep track of the number of busted players
                {
                    bustedPlayers++; 
                }
            }
            if (highScore > 0) // someone scored, anyway!
            {
                // find the FIRST player in list who meets win condition as the winner
                return currentPlayers.Find(player => player.score == highScore);
            }

            if (bustedPlayers == (Players.Count - 1)) // If only 1 player isn't busted
            {
                // the only player left should have a score that is less than 21 and they should be the winner
                return currentPlayers.Find(player => player.score <= 21);
            }

            // Shouldn't get to this point since the only player who is not busted will be the winner
            return null; // everyone must have busted because nobody won!
        }

        /// <summary>
        /// Pays the player from the pot.
        /// </summary>
        /// <param name="player">The player who is getting the cash</param>
        /// <param name="cashAmount">The cash ready to be paid</param>
        public void Pay(Player player, int cashAmount)
        {
            player.cash += cashAmount;
        }

        /// <summary>
        /// Resets the cards, status, and scores of the players who haven't quit
        /// </summary>
        public void ResetPlayers()
        {
            foreach (Player player in Players.FindAll(player => player.status != PlayerStatus.quit))
            {
                player.cards = new List<Card>();
                player.status = PlayerStatus.active;
                player.score = 0;
            }
        }

        /// <summary>
        /// Rearranges the order of the players
        /// </summary>
        public void RearrangePlayers()
        {
            CardTable.WriteCardTableMessage("Rearranging players order...");
            Random rng = new Random();
            for (int i = 0; i < Players.Count; i++)
            {
                Player tmp = Players[i];
                int swapindex = rng.Next(Players.Count);
                Players[i] = Players[swapindex];
                Players[swapindex] = tmp;
            }
        }
    }
}
