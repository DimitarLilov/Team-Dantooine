﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using StarWars.Characters;
using StarWars.Characters.Players;
using StarWars.Contracts;
using StarWars.GameObject;
using StarWars.Items;
using StarWars.UI;
using StarWars.Characters.Enemies;
using StarWars.Exceptions;
using StarWars.Items.ArmorSet;
using StarWars.Items.HealthRestore;
using StarWars.Items.WeaponType;

namespace StarWars.Engine
{
    public class StarWarsEngine
    {
        public const int mapHeight = 15;
        public const int mapWidth = 45;
        private const int Exp = 25;

        private const int enemiesNumber = 25;
        private static Random random = new Random();
        private char[,] map = new char[mapHeight, mapWidth];

        private readonly CondoleRenderer renderer;
        private readonly ConsoleReader reader;

        private readonly string[] enemyNames =
        {
            "Gocho",
             "Rambo",
             "Stamat",
             "Pepi",
             "Radan",
             "Canko",
             "DartBoyko",
             "Semo"
        };

        private readonly List<Character> enemies = new List<Character>();
        private Player player;


        public StarWarsEngine(ConsoleReader reader, CondoleRenderer renderer)
        {
            this.renderer = renderer;
            this.reader = reader;
            this.enemies = MakeEnemy();
        }

        public bool IsRun { get; private set; }

        public void Run()
        {
            this.IsRun = true;
            PrintLogo();
            this.renderer.WriteLine("");
            var playerName = GetPlayerName();
            Player heroe = GetPlayer(playerName);

            renderer.Clear();

            map = PopulateMap();

            PrintMap(PopulateMap(), heroe);
            DrowStaticInfo(heroe);

            while (this.IsRun)
            {
                if (Console.KeyAvailable)
                {

                    renderer.Clear();
                    ConsoleKeyInfo pressedKey = Console.ReadKey(true);

                    try
                    {
                        heroe.Move(pressedKey);

                    }
                    catch (ObjectOutOfMap ex)
                    {

                    }

                    catch (Exception ex)
                    {

                    }
                    PrintMap(map, heroe);
                    DrowStaticInfo(heroe);
                    bool isEnemy = enemies.Any(x => x.Position.X == heroe.Position.X && x.Position.Y == heroe.Position.Y);
                    bool isHospital = (map[heroe.Position.X, heroe.Position.Y] == 'H');
                    bool isArmory = (map[heroe.Position.X, heroe.Position.Y] == 'W');
                    bool isForgery = (map[heroe.Position.X, heroe.Position.Y] == 'W');
                    if (isHospital)
                    {
                        heroe.UseItem(new BactaTank(heroe.Position));
                    }
                    else if (isArmory)
                    {
                        heroe.UseItem(new LaserSword(heroe.Position));
                    }
                    else if (isForgery)
                    {
                        heroe.UseItem(new Chest(heroe.Position));
                    }
                   else if (isEnemy)
                    {
                        renderer.Clear();
                        Console.WriteLine("Battle mode");
                        var enemy = enemies.Find(x => x.Position.X == heroe.Position.X && x.Position.Y == heroe.Position.Y);
                        Fight(heroe, enemy);
                    }
                    if ((heroe).Symbol == ' ')
                    {
                        renderer.Clear();
                        renderer.WriteLine("Game over");
                        break;
                    }
                    else if (enemies.Count == 0)
                    {
                        renderer.Clear();
                        renderer.WriteLine("You won!");
                        break;
                    }

                }
            }
        }
        private void Fight(Player heroe, Character enemy)
        {
            while (true)
            {
                heroe.Attack(enemy);
                if (enemy.Health == 0)
                {
                    enemy.Symbol = '.';
                    enemies.Remove(enemy);
                    heroe.Experience += Exp;
                    heroe.LevelUP();
                    renderer.WriteLine("{0} is dead", enemy.Name);
                    break;
                }
                enemy.Attack(heroe);
                if (heroe.Health == 0)
                {
                    heroe.Symbol = ' ';
                    break;
                }
            }
        }
        private void DrowStaticInfo(Player player)
        {
            DrowInfo(46, 1, "Player Name: " + player.Name);
            DrowInfo(46, 2, "Level: " + player.Level);
            DrowInfo(46, 3, "Experience: " + player.Experience);
            DrowInfo(46, 4, "Health: " + player.Health);
            DrowInfo(46, 5, "Damage: " + player.Damage);
            DrowInfo(46, 6, "Armor: " + player.Armor);
            DrowInfo(46, 8, "H- Hospital (+50 Health)");
            DrowInfo(46, 9, "W- Blacksmith (+30 Damage)");
            DrowInfo(46, 11, "T- Storm Trooper (Damage: 100 Health: 100)");
            DrowInfo(46, 12, "B- Bounty Hunter (Damage: 250 Health: 200)");
            DrowInfo(46, 13, "A- Sith Apprentice (Damage: 350 Health: 300)");
            DrowInfo(46, 14, "S- Sith (Damage: 600 Health: 500)");


        }
        private void DrowInfo(int x, int y, string str)
        {
            Console.SetCursorPosition(x, y);
            this.renderer.WriteLine(str);

        }
        private Player GetPlayer(string name)
        {
            this.renderer.WriteLine("Choose your hero");
            this.renderer.WriteLine("1. Battle Droid (damage : 100, health : 100, armor : 30)");
            this.renderer.WriteLine("2. Rebel (damage : 200, health : 200, armor : 30)");
            this.renderer.WriteLine("3. Gungan Warrior (damage : 300, health : 300, armor : 30)");
            this.renderer.WriteLine("4. Jedi (damage : 400, health : 400, armor : 30)");
            string choiceHeroe = this.reader.ReadLine();
            string[] numberHeroes = { "1", "2", "3", "4" };
            while (!numberHeroes.Contains(choiceHeroe))
            {
                this.renderer.WriteLine("Invalid choice of Heroe");
                choiceHeroe = this.reader.ReadLine();
            }
            Player heroe = null;
            switch (choiceHeroe)
            {
                case "1":
                    heroe = new BattleDroid(new Position(0, 0), 'P', name);
                    break;

                case "2":
                    heroe = new Rebel(new Position(0, 0), 'P', name);
                    break;
                case "3":
                    heroe = new GunganWarrior(new Position(0, 0), 'P', name);
                    break;
                case "4":
                    heroe = new Jedi(new Position(0, 0), 'P', name);
                    break;
            }
            return heroe;
        }
        private string GetPlayerName()
        {
            this.renderer.WriteLine("Please enter your name: ");
            string playerName = this.reader.ReadLine();
            while (string.IsNullOrWhiteSpace(playerName))
            {
                this.renderer.WriteLine("Please name cannot be empty.");
                playerName = this.reader.ReadLine();
            }
            return playerName;
        }
        private void PrintLogo()
        {
            string logo = File.ReadAllText("logo.txt");

            this.renderer.WriteLine(logo);
        }

        private void PrintMap(char[,] map, Player player)
        {
            map = PopulateMap();
            map[player.Position.X, player.Position.Y] = player.Symbol;
            map[1,1] = 'H';
            map[1, 0] = 'W';
            StringBuilder builder = new StringBuilder();
            for (int row = 0; row < map.GetLength(0); row++)
            {
                for (int col = 0; col < map.GetLength(1); col++)
                {
                    builder.Append(map[row, col]);
                }
                builder.AppendLine();
            }
            renderer.WriteLine(builder.ToString());
        }

        private char[,] PopulateMap()
        {
            map = CreateMap();

            for (int i = 0; i < enemies.Count; i++)
            {
                map[enemies[i].Position.X, enemies[i].Position.Y] = enemies[i].Symbol;
            }
            return map;
        }

        private char[,] CreateMap()
        {
            for (int row = 0; row < mapHeight; row++)
            {
                for (int col = 0; col < mapWidth; col++)
                {
                    map[row, col] = '.';
                }
            }
            return map;
        }

        private List<Character> MakeEnemy()
        {
            for (int i = 0; i < enemiesNumber; i++)
            {
                string enemyName = enemyNames[random.Next(0, enemyNames.Length)];
                Position enemyPosition = new Position(random.Next(3, mapHeight), random.Next(2, mapWidth));
                bool isEmpty = CheckPosition(enemyPosition);
                while (isEmpty == false)
                {
                    enemyPosition = new Position(random.Next(1, mapHeight), random.Next(1, mapWidth));
                    isEmpty = CheckPosition(enemyPosition);
                }

                int typePick = random.Next(1, 5);
                switch (typePick)
                {
                    case 1:
                        enemies.Add(new BountyHunter(enemyPosition, 'B', enemyName));
                        break;
                    case 2:
                        enemies.Add(new Sith(enemyPosition, 'S', enemyName));
                        break;
                    case 3:
                        enemies.Add(new SIthAPprentice(enemyPosition, 'A', enemyName));
                        break;
                    case 4:
                        enemies.Add(new StormTrooper(enemyPosition, 'T', enemyName));
                        break;
                }
            }
            return enemies;
        }

        private bool CheckPosition(Position pos)
        {
            bool isEmpty = true;

            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].Position.X == pos.X && enemies[i].Position.Y == pos.Y)
                {
                    isEmpty = false;
                }
            }
            return isEmpty;
        }
    }
}
