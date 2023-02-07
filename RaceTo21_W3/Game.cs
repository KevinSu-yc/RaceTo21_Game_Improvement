using System;
using System.Collections.Generic;

namespace RaceTo21
{
    public class Game
    {
        int numberOfPlayers; // number of players in current game
        List<Player> players = new List<Player>(); // list of objects containing player data
        CardTable cardTable; // object in charge of displaying game information
        Deck deck = new Deck(); // deck of cards
        int currentPlayer = 0; // current player on list
        int currentPot = 0; // amount of bet put in current pot
        public Task nextTask; // keeps track of game state
        private bool cheating = false; // lets you cheat for testing purposes if true

        public Game(CardTable c)
        {
            cardTable = c;
            deck.Shuffle();
            deck.ShowAllCards();
            nextTask = Task.GetNumberOfPlayers;
        }

        /* Adds a player to the current game
         * Called by DoNextTask() method
         */
        public void AddPlayer(string n)
        {
            players.Add(new Player(n));
        }

        /* Figures out what task to do next in game
         * as represented by field nextTask
         * Calls methods required to complete task
         * then sets nextTask.
         */
        public void DoNextTask()
        {
            Console.WriteLine("================================"); // this line should be elsewhere right?
            if (nextTask == Task.GetNumberOfPlayers)
            {
                numberOfPlayers = cardTable.GetNumberOfPlayers();
                nextTask = Task.GetNames;
            }
            else if (nextTask == Task.GetNames)
            {
                for (var count = 1; count <= numberOfPlayers; count++)
                {
                    var name = cardTable.GetPlayerName(count);
                    AddPlayer(name); // NOTE: player list will start from 0 index even though we use 1 for our count here to make the player numbering more human-friendly
                }
                nextTask = Task.IntroducePlayers;
            }
            else if (nextTask == Task.IntroducePlayers)
            {
                cardTable.ShowPlayers(players);
                nextTask = Task.AskBet;
            }
            else if (nextTask == Task.AskBet) // Bet Feature: Ask players to bet an amount of cash
            {
                foreach(Player p in players)
                {
                    currentPot += cardTable.CollectBet(p);
                }
                Console.WriteLine("The total amount in pot is:" + currentPot);
                nextTask = Task.OfferFirstCard;
            }
            else if (nextTask == Task.OfferFirstCard) // Force to give every player a card at the beginning
            {
                Console.WriteLine("Giving the players their first card...");
                foreach (Player p in players)
                {
                    Card card = deck.DealTopCard();
                    p.cards.Add(card);
                    p.score = ScoreHand(p);
                }
                nextTask = Task.PlayerTurn;
            }
            else if (nextTask == Task.PlayerTurn)
            {
                cardTable.ShowHands(players);
                Player player = players[currentPlayer];
                if (player.status == PlayerStatus.active)
                {
                    if (cardTable.OfferACard(player))
                    {
                        Card card = deck.DealTopCard();
                        player.cards.Add(card);
                        player.score = ScoreHand(player);
                        if (player.score > 21)
                        {
                            player.status = PlayerStatus.bust;
                        }
                        else if (player.score == 21)
                        {
                            player.status = PlayerStatus.win;
                        }
                    }
                    else
                    {
                        player.status = PlayerStatus.stay;
                    }
                }
                cardTable.ShowHand(player);
                nextTask = Task.CheckForEnd;
            }
            else if (nextTask == Task.CheckForEnd)
            {
                if (!CheckActivePlayers())
                {
                    Player winner = DoFinalScoring();

                    // The winner gets the cash from pot
                    cardTable.Pay(winner, currentPot);
                    cardTable.AnnounceWinner(winner, currentPot);
                    currentPot = 0; // Reset the pot
                    nextTask = Task.CheckForNewGame;
                }
                else
                {
                    currentPlayer++;
                    if (currentPlayer > players.Count - 1)
                    {
                        currentPlayer = 0; // back to the first player...
                    }
                    nextTask = Task.PlayerTurn;
                }
            }
            else if (nextTask == Task.CheckForNewGame) // Ask the players if they want to keep playing
            {
                List<Player> newPlayers = new List<Player>(players); // copy a temporary list of players to record who wants to keep playing
                foreach (Player p in players)
                {
                    if (!cardTable.AskNewGame(p)) // if the player don't want to keep playing, remove them from the game
                    {
                        newPlayers.Remove(p);
                    }
                }
                players = newPlayers;
                cardTable.ResetPlayers(players); // reset player's cards, scores, and status
                if (players.Count < 1) // if no one wants to keep playing, end the game
                {
                    cardTable.AnnounceWinner(null, -1);
                    nextTask = Task.GameOver;
                }
                else if (players.Count == 1) // if only one player wants to keep playing, the player become winner right away
                {
                    cardTable.AnnounceWinner(players[0], -1);
                    nextTask = Task.GameOver;
                }
                else
                {
                    // Reset the variable and the deck for next game
                    currentPlayer = 0;
                    deck = new Deck();
                    deck.Shuffle();

                    // Rearrange the order of the players in the player list
                    Random rng = new Random();
                    for (int i = 0; i < players.Count; i++)
                    {
                        Player tmp = players[i];
                        int swapindex = rng.Next(players.Count);
                        players[i] = players[swapindex];
                        players[swapindex] = tmp;
                    }
                    nextTask = Task.IntroducePlayers;
                }
            }
            else // we shouldn't get here...
            {
                Console.WriteLine("I'm sorry, I don't know what to do now!");
                nextTask = Task.GameOver;
            }
        }

        public int ScoreHand(Player player)
        {
            int score = 0;
            if (cheating == true && player.status == PlayerStatus.active)
            {
                string response = null;
                while (int.TryParse(response, out score) == false)
                {
                    Console.Write("OK, what should player " + player.name + "'s score be?");
                    response = Console.ReadLine();
                }
                return score;
            }
            else
            {
                foreach (Card card in player.cards)
                {
                    string faceValue = card.id.Remove(card.id.Length - 1);
                    switch (faceValue)
                    {
                        case "K":
                        case "Q":
                        case "J":
                            score = score + 10;
                            break;
                        case "A":
                            score = score + 1;
                            break;
                        default:
                            score = score + int.Parse(faceValue);
                            break;
                    }
                }
            }
            return score;
        }

        public bool CheckActivePlayers()
        {
            int bustedPlayers = 0; // Count the number of busted players for every round
            int activePlayers = 0; // Count the number of active players for every round
            foreach (var player in players)
            {
                if (player.status == PlayerStatus.win) // if a player wins, immediately announce the winner
                {
                    return false;
                }
                else if (player.status == PlayerStatus.active)
                {
                    activePlayers++;
                }
                else if (player.status == PlayerStatus.bust)
                {
                    bustedPlayers++;
                }
            }

            // If there's only 1 player isn't busted, immediately announce the winner
            if (bustedPlayers == (players.Count - 1))
            {
                return false;
            }

            // If there's at least 1 player still active, keep playing next round
            if (activePlayers > 0)
            {
                return true;
            }

            // At this point, every player should be stayed, compare the score and announce the winner
            return false; 
        }

        public Player DoFinalScoring()
        {
            int highScore = 0;
            int bustedPlayers = 0; // Count the number of busted players for every round
            foreach (var player in players)
            {
                cardTable.ShowHand(player);
                if (player.status == PlayerStatus.win) // someone hit 21
                {
                    return player;
                }
                if (player.status == PlayerStatus.stay) // still could win...
                {
                    if (player.score > highScore)
                    {
                        highScore = player.score;
                    }
                }
                if (player.status == PlayerStatus.bust)
                {
                    bustedPlayers++;
                }
            }
            if (highScore > 0) // someone scored, anyway!
            {
                // find the FIRST player in list who meets win condition
                return players.Find(player => player.score == highScore);
            }

            if (bustedPlayers == (players.Count - 1)) // If only 1 player isn't busted
            {
                // the only player left should have a score that is less than 21
                return players.Find(player => player.score <= 21);
            }

            // Shouldn't get to this point since the only player who is not busted will be the winner
            return null; // everyone must have busted because nobody won!
        }
    }
}
