using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MUDInterface
{
    public class Chance
    {
        private Chance() { Rand = new Random(); }
        private static Chance _instance = new Chance();
        public static Chance Instance { get { return _instance; } }

        public Random Rand { get; set; }

        public int RandInt(int min, int max)
        {
            return Rand.Next(min, max);
        }

        public bool Percent(int percentChance)
        {
            int roll = Rand.Next(1, 100);

            if (roll >= (100 - percentChance))
                return true;
            else
                return false;
        }
    }
}