using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MUDInterface.Entities;

namespace MUDInterface.Messaging
{
    public class MessageDispatcher
    {        
        private static MessageDispatcher _instance = new MessageDispatcher();
        public static MessageDispatcher Instance { get { return _instance; } }

        private Queue<Telegram> priorityQ;

        private MessageDispatcher()
        {
            priorityQ = new Queue<Telegram>();
        }

        private void discharge(GameEntity entity, ref Telegram msg)
        {
            entity.HandleMessage(ref msg);
        }

        public void DispatchMessage(Guid sender, Guid receiver, int msg, double delaySecs, int extraInfo = 0)
        {
            GameEntity _receiver = EntityManager.Instance.GetEntityByID(receiver);

            Telegram telegram = new Telegram(sender, receiver, msg, extraInfo);

            if (delaySecs <= 0.0)
                discharge(_receiver, ref telegram);
            else
            {
                telegram.DispatchTime = DateTime.Now.AddSeconds(delaySecs);
                priorityQ.Enqueue(telegram);
            }
        }

        public void DispatchDelayedMessages()
        {
            DateTime currTime = DateTime.Now;

            while (priorityQ.FirstOrDefault().DispatchTime < currTime)
            {
                Telegram telegram = priorityQ.Dequeue();

                GameEntity _receiver = EntityManager.Instance.GetEntityByID(telegram.Receiver);

                discharge(_receiver, ref telegram);
            }
        }
    }
}
