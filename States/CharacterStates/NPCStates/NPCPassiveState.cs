using MUDInterface.Entities;
using MUDInterface.Entities.Characters;
using MUDInterface.Entities.Locations;
using MUDInterface.States.CharacterStates.NPCStates.Battle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MUDInterface.States.CharacterStates.NPCStates
{
    public class NPCPassiveState : State<NPC>
    {
        private NPCPassiveState() { }
        private static NPCPassiveState _instance = new NPCPassiveState();
        public static NPCPassiveState Instance { get { return _instance; } }

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
            //regenerate health
            if (StateClock[entity.ID].ActionTime())
            {
                entity.Health = entity.Health + entity.Level;
                if (entity.Health > entity.MAX_HEALTH())
                    entity.Health = entity.MAX_HEALTH();

                StateClock[entity.ID].SetActionDelaySecs(15 - entity.Level);
            }

            #region Aggression Decision
            //If not already in a battle and not dead
            if (entity.StateMachine.CurrentState != BattleState.Instance && entity.StateMachine.CurrentState != DeadState.Instance)
            {
                //If there is another (non-player) entity in the room, advance to attack
                Room room = Entities.EntityManager.Instance.GetRoomByLocation(entity.Location);
                if (room.Entities.Count > 1)
                {
                    //Get list of potential targets
                    List<NPC> opponents = Entities.EntityManager.Instance.GetAllNPC().Where(c => c.Location == entity.Location && 
                                                                                                 c.GetType() != typeof(Player) && 
                                                                                                 !c.StateMachine.IsInState(DeadState.Instance)).ToList();
                    NPC opponent = null;
                    int itr = 0;

                    do
                    {
                        if (itr < opponents.Count)
                        {
                            opponent = opponents[itr];
                            ++itr;
                        }
                        else
                            break;
                    } while (opponent.ID == entity.ID);

                    if (opponent.ID != entity.ID)
                    {                        
                        entity.Opponent = opponent;                                         
                        Messaging.MessageDispatcher.Instance.DispatchMessage(entity.ID, opponent.ID, (int)Messaging.Battle.BattleMessages.MSG_OPPONENT_ADVANCING, 0);
                        entity.StateMachine.ChangeState(BattleState.Instance);
                        GameOutput.Client.GroupMessage(entity.Name + " is advancing towards " + opponent.Name + "...", entity.Location.ToString());
                        StateClock[entity.ID].SetActionDelaySecs(2);
                        while (!StateClock[entity.ID].ActionTime()) { }
                    }
                }
            }
            #endregion
        }

        public override bool OnMessage(NPC entity, ref Messaging.Telegram msg)
        {
            switch (msg.Msg)
            {
                case (int)Messaging.Battle.BattleMessages.MSG_OPPONENT_ADVANCING:
                    Guid attackerID = msg.Sender;
                    NPC opponent = EntityManager.Instance.GetAllNPC().Where(npc => npc.ID == attackerID).FirstOrDefault();
                    entity.Opponent = opponent;
                    entity.StateMachine.ChangeState(BattleState.Instance);
                    return true;
                default:
                    return false;
            }
        }
    }
}