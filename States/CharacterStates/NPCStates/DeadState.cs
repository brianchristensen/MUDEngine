using MUDInterface.Entities;
using MUDInterface.Entities.Characters;
using MUDInterface.Entities.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MUDInterface.States.CharacterStates.NPCStates.Battle
{
    public class DeadState : State<NPC>
    {
        private DeadState() { }
        private static DeadState _instance = new DeadState();
        public static DeadState Instance { get { return _instance; } }

        public override void Enter(NPC entity)
        {
            GameOutput.Client.GroupMessage(entity.Name + " is dead!", entity.Location.ToString());
            entity.Name = entity.Name + " corpse";
            entity.Health = 0;
            StateClock.Add(entity.ID, new ActionClock());
            StateClock[entity.ID].SetActionDelaySecs(60);
        }

        public override void Exit(NPC entity)
        {
            return;
        }

        public override void Execute(NPC entity)
        {
            if (StateClock[entity.ID].ActionTime())
            {
                StateClock.Remove(entity.ID);
                EntityManager.Instance.RemoveNPC(entity);
            }
        }

        public override bool OnMessage(NPC entity, ref Messaging.Telegram msg)
        {
            return false;
        }
    }
}