using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace battleship.Models
{
    public class PlayerSetup
    {
        public string Name { get; set; }

        public bool IsShipHorizontal { get; set; }

        public int ShipX { get; set; }

        public int ShipY { get; set; }

        public int PlayerId { get; set; }
    }
}
