using MUDInterface.Entities.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MUDInterface.States.CharacterStates.NPCStates.Battle
{
    public class BattleState : State<NPC>
    {
        private BattleState() { }
        private static BattleState _instance = new BattleState();
        public static BattleState Instance { get { return _instance; } }

        public override void Enter(NPC entity)
        {
            StateClock.Add(entity.ID, new ActionClock());
            return;
        }

        public override void Exit(NPC entity)
        {
            StateClock.Remove(entity.ID);
            return;
        }

        public override void Execute(NPC entity)
        {
            //every timespan
            if (StateClock[entity.ID].ActionTime())
            {                
                //try to attack opponent
                if (Chance.Instance.Percent(40 + entity.Level))
                {
                    //if successful calculate damage and send MSG_TAKEN_DAMAGE to opponent with amount of damange taken
                    int damage = entity.Strength + Chance.Instance.RandInt(0 + entity.Level, 5 + entity.Level);
                    GameOutput.Client.GroupMessage(entity.Name + " has hit " + entity.Opponent.Name + " for " + damage.ToString() + " damage!", entity.Location.ToString());
                    Messaging.MessageDispatcher.Instance.DispatchMessage(entity.ID, entity.Opponent.ID, (int)Messaging.Battle.BattleMessages.MSG_TAKEN_DAMAGE, 0, damage);
                }
                else
                {
                    GameOutput.Client.GroupMessage(entity.Name + " barely misses " + entity.Opponent.Name + ".", entity.Location.ToString());
                }

                //check to see if opponent is in state DeadState
                if (entity.Opponent.StateMachine.IsInState(DeadState.Instance))
                {
                    entity.LevelUp();
                    entity.Opponent = null;
                    entity.StateMachine.RevertToPreviousState();
                }

                if (StateClock.ContainsKey(entity.ID))
                    StateClock[entity.ID].SetActionDelaySecs(5 / entity.Level);
            }
        }

        public override bool OnMessage(NPC entity, ref Messaging.Telegram msg)
        {
            switch (msg.Msg)
            {
                case (int)Messaging.Battle.BattleMessages.MSG_TAKEN_DAMAGE:
                    entity.Health = entity.Health - msg.ExtraInfo;
                    if (entity.Health <= 0)
                    {
                        entity.StateMachine.SetGlobalState(null);
                        entity.StateMachine.ChangeState(DeadState.Instance);
                    }
                    return true;
                default:
                    return false;
            }
        }
    }
}