using System;

namespace RaceTo21
{
    class Program
    {
        static void Main(string[] args)
        {
            CardTable cardTable = new CardTable();
            Game game = new Game(cardTable);
            while (game.NextTask != Task.GameOver)
            {
                game.DoNextTask();
            }
        }
    }
}

