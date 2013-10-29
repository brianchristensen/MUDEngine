using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MUDInterface.Messaging
{
    public class Telegram
    {
        public Telegram(Guid sender, Guid receiver, int msg, int extraInfo = 0)
        {
            Sender = sender;
            Receiver = receiver;
            Msg = msg;
            ExtraInfo = extraInfo;
        }

        public Guid Sender { get; set; }
        public Guid Receiver { get; set; }
        public int Msg { get; set; }
        public DateTime DispatchTime { get; set; }
        public int ExtraInfo { get; set; }
    }
}
