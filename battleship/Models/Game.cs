using battleship.GameLogic;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace battleship.Models
{
    public class Game
    {
        public int GameId { get; set; }

        public int? WinnerId { get; set; }

        public int NextPlayerId { get; set; }

        public List<Player> Players { get; set; }

        public Player GetOpponentPlayer(Player player)
        {
            return Players.First(p => p.PlayerId != player.PlayerId);
        }
    }
}
