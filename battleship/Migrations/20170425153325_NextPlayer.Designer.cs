using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using battleship.Models;

namespace battleship.Migrations
{
    [DbContext(typeof(GameContext))]
    [Migration("20170425153325_NextPlayer")]
    partial class NextPlayer
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.1");

            modelBuilder.Entity("battleship.Models.Game", b =>
                {
                    b.Property<int>("GameId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("NextPlayerId");

                    b.Property<int?>("WinnerId");

                    b.HasKey("GameId");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("battleship.Models.Player", b =>
                {
                    b.Property<int>("PlayerId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("GameId");

                    b.Property<bool>("IsReady");

                    b.Property<int>("LastShot");

                    b.Property<string>("Name");

                    b.Property<string>("_opponentBoard");

                    b.Property<string>("_ownBoard");

                    b.HasKey("PlayerId");

                    b.HasIndex("GameId");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("battleship.Models.Player", b =>
                {
                    b.HasOne("battleship.Models.Game", "Game")
                        .WithMany("Players")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
