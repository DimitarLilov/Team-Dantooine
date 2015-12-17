﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StarWars.Engine;
using StarWars.UI;

namespace Game
{
    class Game
    {
        static void Main(string[] args)
        {
            ConsoleReader reader = new ConsoleReader();
            CondoleRenderer renderer = new CondoleRenderer();
            StarWarsEngine engine = new StarWarsEngine(reader, renderer);
            engine.Run();
        }
    }
}
