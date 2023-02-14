using System;
using System.Collections.Generic;

namespace RaceTo21
{
	public class Player
	{
		public string name;
		public List<Card> cards = new List<Card>();
		public PlayerStatus status = PlayerStatus.active;
		public int score;
		public int cash = 100; // Set a default value for player to have at the beginning
		public int wins = 0;

		public Player(string n)
		{
			name = n;
        }

		/* Introduces player by name
		 * Called by CardTable object
		 */
		public void Introduce(int playerNum)
		{
			Console.WriteLine("Hello, my name is " + name + " and I am player #" + playerNum + $"(Cash: {cash}$, Wins: {wins})");
		}

		/* Bets an amount from player's cash
		 * Called by CardTable object
		 */
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

