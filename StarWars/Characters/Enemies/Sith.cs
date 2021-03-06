﻿using StarWars.GameObject;

namespace StarWars.Characters.Enemies
{
    public class Sith : Character
    {
        private const int SithDamage = 600;
        private const int SithHealth = 500;
        private const int SithArmor = 20;
        public Sith(Position position, char symbol, string name) : base(position, symbol, SithDamage, SithHealth, name, SithArmor)
        {
        }
    }
}
