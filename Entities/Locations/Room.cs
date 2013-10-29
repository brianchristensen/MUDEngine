using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MUDInterface.States;
using MUDInterface.States.LocationStates.RoomStates;
using System.Text;

namespace MUDInterface.Entities.Locations
{
    public class Room : GameEntity
    {
        public StateMachine<Room> StateMachine { get; set; }

        public string Description { get; set; }
        public List<GameEntity> Entities { get; set; }
        public Dictionary<string, int> Exits { get; set; }

        public Room(string _name = "Room 1", string _description = "Just another bland room." , int _location = 9999) : base(_location, _name)
        {
            StateMachine = new StateMachine<Room>(this);
            Description = _description;
            Entities = new List<GameEntity>();
            Exits = new Dictionary<string, int>();

            StateMachine.SetCurrentState(ObserveState.Instance);
        }

        public override void Display(string connID)
        {
            StringBuilder entities = new StringBuilder();
            StringBuilder exits = new StringBuilder();
            Player player = EntityManager.Instance.GetPlayerByConnectionID(connID);
            int itr = 2;
            
            foreach (GameEntity e in Entities)
            {
                if (e.ID != player.ID)
                {
                    if (itr < Entities.Count)
                        entities.Append(e.Name + ", ");
                    else
                        entities.Append(e.Name);

                    ++itr;
                }
                
            }

            GameOutput.Client.ClientMessage("__________________________________________________________________________________________________", connID);
            GameOutput.Client.ClientMessage(this.Name, connID);
            GameOutput.Client.ClientMessage(this.Description, connID);
            foreach (string ex in Exits.Keys)
            {
                exits.Append(ex.ToUpper() + " ");
            }
            GameOutput.Client.ClientMessage("Exits: " + exits.ToString(), connID);
            GameOutput.Client.ClientMessage("In room: " + entities.ToString(), connID);
        }

        public override bool HandleMessage(ref Messaging.Telegram msg)
        {
            return StateMachine.HandleMessage(ref msg);
        }

        public override void Update()
        {
            StateMachine.Update();
        }
    }
}