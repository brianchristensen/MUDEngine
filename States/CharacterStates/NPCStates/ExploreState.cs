using MUDInterface.Entities;
using MUDInterface.Entities.Characters;
using MUDInterface.Entities.Locations;
using System.Collections.Generic;
using System.Linq;

namespace MUDInterface.States.CharacterStates.NPCStates
{
    public class ExploreState : State<NPC>
    {
        private ExploreState() { State_Change_Accumulator = 0; Accumulator_Threshold = 10; }
        private static ExploreState _instance = new ExploreState();
        public static ExploreState Instance { get { return _instance; } }

        public int State_Change_Accumulator { get; set; }
        public int Accumulator_Threshold { get; set; }

        public override void Enter(NPC entity)
        {
            //GameOutput.Client.GroupMessage(entity.Name + " has decided to go exploring.", entity.Location.ToString());
        }

        public override void Exit(NPC entity)
        {
            //GameOutput.Client.GroupMessage(entity.Name + " is tired of exploring.", entity.Location.ToString());
        }

        public override bool OnMessage(NPC entity, ref Messaging.Telegram msg)
        {
            return false;
        }

        public override void Execute(NPC entity)
        {
            Room newLocation = null;
            
            List<Room> rooms = EntityManager.Instance.GetRooms();

            Room currentLocation = rooms.Where(r => r.Location == entity.Location).FirstOrDefault();
            List<Room> exits = rooms.Where(r => currentLocation.Exits.Values.Contains(r.Location)).ToList();

            do
            {
                newLocation = exits[Chance.Instance.RandInt(0, exits.Count - 1)];
            }while (entity.Location == newLocation.Location);
            
            if (Chance.Instance.Percent(10))
            {
                if (State_Change_Accumulator >= Accumulator_Threshold)
                {
                    //chance to move to new location
                    if (Chance.Instance.Percent(50))
                    {
                        //Leave current room
                        Messaging.MessageDispatcher.Instance.DispatchMessage(entity.ID, currentLocation.ID, (int)Messaging.Movement.RoomMessages.MSG_LEAVE_ROOM, 0, newLocation.Location);
                        //Enter new room
                        Messaging.MessageDispatcher.Instance.DispatchMessage(entity.ID, newLocation.ID, (int)Messaging.Movement.RoomMessages.MSG_ENTER_ROOM, 0, currentLocation.Location);
                    }
                    //chance to enter rest state
                    else if (Chance.Instance.Percent(10))
                        entity.StateMachine.ChangeState(RestState.Instance);

                    State_Change_Accumulator = 0;
                }
            }

            ++State_Change_Accumulator;
        }
    }
}