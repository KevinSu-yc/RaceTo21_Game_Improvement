using System;
using System.Collections.Generic;

namespace RaceTo21
{
	public class Player
	{
		public string name { get; private set; } // I don't want players' name to be changed during the game
		public List<Card> cards = new List<Card>(); // The cards in the player's hands
		public PlayerStatus status = PlayerStatus.active; // Whether the player wants to keep playing, or if the player is able to keep playing
		public int score; // The score calculated by the cards in the player's hands
		public int cash = 100; // Set a default value for player to have at the beginning
		public int wins = 0; // Amount of times that the player wins a round

		/// <summary>
		/// Represents a player in the game
		/// </summary>
		/// <param name="n">The player's name</param>
		public Player(string n)
		{
			name = n;
        }

		/// <summary>
		/// Introduces player by their name, called by CardTable object.
		/// </summary>
		/// <param name="playerNum">The order number of the player</param>
		public void Introduce(int playerNum)
		{
			Console.WriteLine("Hello, my name is " + name + " and I am player #" + playerNum + $"(Cash: ${cash}, Wins: {wins})");
		}

		/// <summary>
		/// Bets an amount from player's cash if they can afford it. Called by CardTable object
		/// </summary>
		/// <param name="betAmount">The amount of cash to bet</param>
		/// <returns>Returns the bet amount if the player can afford it, otherwise, return -1</returns>
		public int bet(int betAmount)
        {
            if (betAmount > cash) // if a player can't afford the betAmount, don't subtract the amount from player's cash
            {
				return -1;
            }

			cash -= betAmount;
			return betAmount;
        }
	}
}

