using battleship.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace battleship.GameLogic
{
    public class BattleshipGame
    {
        private const int SHIP_SIZE = 3;

        private Game _game;

        public BattleshipGame(Game game)
        {
            _game = game;
        }

        public bool Shoot(Player currentPlayer, int x, int y)
        {
            var opponentPlayer = _game.GetOpponentPlayer(currentPlayer);

            var thisShot = (opponentPlayer.OwnBoard[x, y] == OwnField.Ship)
                ? OpponentField.Hit
                : OpponentField.Miss;
            currentPlayer.SetOpponentField(x, y, thisShot);
            currentPlayer.LastShot = thisShot;

            _game.NextPlayerId = opponentPlayer.PlayerId;

            return CheckForWin(currentPlayer, opponentPlayer);
        }

        private bool CheckForWin(Player currentPlayer, Player opponentPlayer)
        {
            for (int x = 0; x < currentPlayer.OwnBoard.GetLength(0); x++)
            {
                for (int y = 0; y < currentPlayer.OwnBoard.GetLength(1); y++)
                {
                    // Hvis modstanderens bræt indeholder et skib og nuværende spiller ikke har ramt, er spillet ikke vundet
                    if (opponentPlayer.OwnBoard[x, y] == OwnField.Ship && currentPlayer.OpponentBoard[x, y] != OpponentField.Hit)
                    {
                        return false;
                    }
                }
            }

            // Alle modstanderens skibe er ramt
            return true;
        }

        public void PlaceShip(Player player, bool isShipHorizontal, int shipX, int shipY)
        {
            if (isShipHorizontal)
            {
                for (int x = shipX; x < shipX + SHIP_SIZE; x++)
                {
                    player.SetOwnField(x, shipY, OwnField.Ship);
                }
            }
            else
            {
                for (int y = shipX; y < shipX + SHIP_SIZE; y++)
                {
                    player.SetOwnField(shipX, y, OwnField.Ship);
                }
            }
        }
    }
}
