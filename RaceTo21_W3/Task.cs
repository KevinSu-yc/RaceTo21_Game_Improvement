using System;
namespace RaceTo21
{
	/// <summary>
	/// Represents different phase of the game
	/// </summary>
	public enum Task
	{
		GetNumberOfPlayers,
		GetNames,
		IntroducePlayers,
		AskTimesToWin,
		AskBet,
		OfferFirstCard,
		PlayerTurn,
		CheckForEnd,
		GameOver,
		CheckForNewGame
	}
}