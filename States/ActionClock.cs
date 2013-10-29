using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MUDInterface.States
{
    public class ActionClock
    {
        private DateTime delay;

        public void SetActionDelaySecs(double delaySeconds)
        {
            delay = DateTime.Now.AddSeconds(delaySeconds);
        }

        public void SetActionDelayMins(double delayMinutes)
        {
            delay = DateTime.Now.AddMinutes(delayMinutes);
        }

        public bool ActionTime()
        {
            DateTime currentTime = DateTime.Now;

            if (currentTime >= delay)
                return true;
            else
                return false;
        }
    }
}