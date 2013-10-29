using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MUDInterface
{
    public class GameTime
    {
        private GameTime() { }
        private static GameTime _instance = new GameTime();
        public static GameTime Instance { get { return _instance; } }

        public int Game_Seconds { get; set; }
        public int Game_Minutes { get; set; }
        public int Game_Hours { get; set; }
        public int Game_Days { get; set; }        

        public override string ToString()
        {
            string time = "Days: " + Game_Days.ToString() + " Hours: " + Game_Hours.ToString() + " Minutes: " + Game_Minutes.ToString() + " Seconds: " + Game_Seconds.ToString();
            return time;
        }        
    }
}