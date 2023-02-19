
# Race to 21 (Final)

### Overview

* The game ends when:
	* A player wins an agreed number of times
	* Among all the players who are still in the game, only 1 player still has cash
	* No more than 1 player wants to keep playing
* The final winner is the player who **Wins the most CASH**, all the players are ranked in the order of:
	1. Amount of cash
	2. Win times
	3. Player Name

### Condition fixed

- (UPDATED) When the game ends, the player who wins the most cash become the final winner even if they quit in the middle of the game.
- (UPDATED) If there's only 1 player left who still has cash, the game ends right away and won't ask if the player wants to keep playing.
- The player who gets 21 immediately wins the round.
- The only player who doesn't bust immediately wins the round.
- Force every player to get one card at the beginning so the problem caused by no one takes a card won't happen.


### Added features

1. **(NEW) (Level 2) At the beginning of the game, ask players to decide a number of times to win to end the game.** I set a range from 3 to 6 times. When a player wins the agreed amount of times, the game ends. However, I set up a rule that the final winner will be the player who wins the most cash. If players have the same amount of cash, the one who wins more times is the winner. If cash and wins still can't determine a winner, then rank the players according to their names alphabetically. The rank of every player will be shown at the end of the game after the final winner is announced.
2. **(UPDATED) (Level 2) At the end of a game, each player is asked if they want to keep playing.** If a player says no, they will be removed from the game. If there's only one player left, they will be the winner. At the beginning of each game, the deck will be rebuilt and shuffled, so does the order of the players.
    - UPDATES: Now, if there's only one player left, the player won't win. Instead, the whole game ends immediately and the player who wins the most cash becomes the final winner even if they quit the game.
3. **(UPDATED) (Level 1) Bet feature.** The players will be asked to bet an amount of money into a pot. The winner takes all the money from the pot. Players who have no more cash will be removed from the game automatically.
	- UPDATES: Now, the winner wins twice the amount of cash every player bets. If there is only 1 player who still has cash among all the players who are still in the game, the whole game also ends immediately. The only player left won't be asked if they want to keep playing.
