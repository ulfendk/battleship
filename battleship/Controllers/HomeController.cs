using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using battleship.Models;
using battleship.GameLogic;
using Microsoft.EntityFrameworkCore;

namespace battleship.Controllers
{
    public class HomeController : Controller
    {
        private GameContext _ctx;

        public HomeController(GameContext ctx)
        {
            _ctx = ctx;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("new")]
        public IActionResult NewGame()
        {
            var game = new Game
            {
                Players = new List<Player>
                {
                    new Player
                    {
                        Name = "Spiller 1",
                        OwnBoard = new OwnField[9,9],
                        OpponentBoard = new OpponentField[9,9]
                    },
                    new Player
                    {
                        Name = "Spiller 2",
                        OwnBoard = new OwnField[9,9],
                        OpponentBoard = new OpponentField[9,9]
                    }
                }
            };

            _ctx.Games.Add(game);
            _ctx.SaveChanges();

            game = _ctx.Games.Find(game.GameId);
            game.NextPlayerId = game.Players[new Random().Next(game.Players.Count)].PlayerId;
            _ctx.SaveChanges();

            return View(game);
        }

        [Route("player/{playerId:int}/setup", Name = nameof(SetupPlayer))]
        public IActionResult SetupPlayer([FromRoute]int playerId)
        {
            var player = _ctx.Players
                .Include(p => p.Game)
                .SingleOrDefault(p => p.PlayerId == playerId);
            if (player == null)
            {
                return NotFound();
            }

            if (player.Game.WinnerId == player.PlayerId)
            {
                return RedirectToAction(nameof(Winner), new { playerId = player.PlayerId });
            }

            var setup = new PlayerSetup
            {
                PlayerId = player.PlayerId,
				Name = player.Name
            };

            return View(setup);
        }

        [HttpPost]
        [Route("player/configure", Name = nameof(ConfigurePlayer))]

        public async Task<IActionResult> ConfigurePlayer()//[FromBody]PlayerSetup setup)
        {
            var setup = new PlayerSetup();
            await TryUpdateModelAsync(setup);

            var player = _ctx.Players
                .Include(p => p.Game)
                .SingleOrDefault(p => p.PlayerId == setup.PlayerId);
            if (player == null)
            {
                return NotFound();
            }

            player.Name = setup.Name;

            var game = new BattleshipGame(player.Game);
            game.PlaceShip(player, setup.IsShipHorizontal, setup.ShipX, setup.ShipY);

            _ctx.SaveChanges();

            return RedirectToAction(nameof(Player), new { playerId = player.PlayerId });
        }


        [Route("player/{playerId:int}/wait", Name = nameof(Wait))]
        public IActionResult Wait([FromRoute]int playerId)
        {
            var player = _ctx.Players
                .Include(p => p.Game)
                .SingleOrDefault(p => p.PlayerId == playerId);
            if (player == null)
            {
                return NotFound();
            }

            if (player.Game.NextPlayerId == player.PlayerId)
            {
                return RedirectToAction(nameof(Player), new { playerId = player.PlayerId });
            }

            return View();
        }


        [Route("player/{playerId:int}", Name = nameof(Player))]
        public IActionResult Player([FromRoute]int playerId)
        {
            var player = _ctx.Players
                .Include(p => p.Game)
                .SingleOrDefault(p => p.PlayerId == playerId);
            if (player == null)
            {
                return NotFound();
            }

            if (player.Game.WinnerId.HasValue)
            {
                if (player.Game.WinnerId == player.PlayerId)
                {
                    return RedirectToAction(nameof(Winner), new { playerId = player.PlayerId });
                }
                else
                {
                    return RedirectToAction(nameof(Looser), new { playerId = player.PlayerId });
                }
            }

            if (player.Game.NextPlayerId != player.PlayerId)
            {
                return RedirectToAction(nameof(Wait), new { playerId = player.PlayerId });
            }

            return View(player);
        }

        [Route("player/{playerId:int}/shoot", Name = nameof(Shoot))]
        public IActionResult Shoot([FromRoute]int playerId, [FromQuery]int? x, [FromQuery]int? y)
        {
            var player = _ctx.Players
                .Include(p => p.Game)
                    .ThenInclude(g => g.Players)
                .SingleOrDefault(p => p.PlayerId == playerId);

            if (player == null)
            {
                return NotFound("Spilleren findes ikke");
            }
            if (!x.HasValue || !y.HasValue)
            {
                return BadRequest("x og y skal angives");
            }

            var game = new BattleshipGame(player.Game);
            bool isWinner = game.Shoot(player, x.Value, y.Value);
            if (isWinner)
            {
                player.Game.WinnerId = player.PlayerId;
            }

            _ctx.SaveChanges();

            if (isWinner)
            {
                return RedirectToAction(nameof(Winner), new { playerId = player.PlayerId });
            }

            return RedirectToAction(nameof(Wait), new { playerId = player.PlayerId });
        }

        [Route("player/{playerId:int}/winner", Name = nameof(Winner))]
        public IActionResult Winner([FromRoute]int playerId)
        {
            var player = _ctx.Players
                .Include(p => p.Game)
                .SingleOrDefault(p => p.PlayerId == playerId);
            if (player == null)
            {
                return NotFound();
            }

            if (player.Game.WinnerId == player.PlayerId)
            {
                return View(player);
            }

            return RedirectToAction(nameof(Player), new { playerId = player.PlayerId });
        }

        [Route("player/{playerId:int}/looser", Name = nameof(Looser))]
        public IActionResult Looser([FromRoute]int playerId)
        {
            var player = _ctx.Players
                .Include(p => p.Game)
                .SingleOrDefault(p => p.PlayerId == playerId);
            if (player == null)
            {
                return NotFound();
            }

            if (player.Game.WinnerId.HasValue && player.Game.WinnerId != player.PlayerId)
            {
                return View(player);
            }

            return RedirectToAction(nameof(Player), new { playerId = player.PlayerId });
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
