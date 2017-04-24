using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace battleship.Models
{
    public class Player
    {
        private string _ownBoard;
        private string _opponentBoard;

        public int PlayerId { get; set; }

        public string Name { get; set; }

        public bool IsReady { get; set; }

        public OpponentField LastShot { get; set; }

        [NotMapped]
        public OwnField[,] OwnBoard
        {
            get => JsonConvert.DeserializeObject<OwnField[,]>(_ownBoard);
            set => _ownBoard = JsonConvert.SerializeObject(value);
        }

        public void SetOwnField(int x, int y, OwnField value)
        {
            var ownBoard = OwnBoard;
            ownBoard[x, y] = value;

            _ownBoard = JsonConvert.SerializeObject(ownBoard);
        }

        [NotMapped]
        public OpponentField[,] OpponentBoard
        {
            get => JsonConvert.DeserializeObject<OpponentField[,]>(_opponentBoard);
            set => _opponentBoard = JsonConvert.SerializeObject(value);
        }

        public void SetOpponentField(int x, int y, OpponentField value)
        {
            var opponentBoard = OpponentBoard;
            opponentBoard[x, y] = value;

            _opponentBoard = JsonConvert.SerializeObject(opponentBoard);
        }

        public int GameId { get; set; }

        public Game Game { get; set; }
    }
}
