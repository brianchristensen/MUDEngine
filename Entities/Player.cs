using MUDInterface.Entities.Characters;
using MUDInterface.Entities.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MUDInterface.Entities
{
    public class Player : GameEntity
    {
        public Player(string connID, string name) : base(1, name)
        {
            ConnectionID = connID;
        }

        public string ConnectionID { get; set; }

        public override bool HandleMessage(ref Messaging.Telegram msg)
        {
            return false;
        }

        public override void Update()
        {
            return;
        }

        public override void Display(string connID)
        {
            GameOutput.Client.ClientMessage("------------------", connID);
            GameOutput.Client.ClientMessage("Name: " + this.Name, connID);
            GameOutput.Client.ClientMessage("------------------", connID);
        }

        public void ChangeLocation(int newLocation)
        {
            try
            {
                Room currentRoom = EntityManager.Instance.GetRoomByLocation(this.Location);
                Room newRoom = EntityManager.Instance.GetRoomByLocation(newLocation);

                string exitDirection = string.Empty;
                string ordinal = currentRoom.Exits.Where(e => e.Value == newLocation).FirstOrDefault().Key;
                switch (ordinal)
                {
                    case "N":
                        exitDirection = "north";
                        break;
                    case "S":
                        exitDirection = "south";
                        break;
                    case "E":
                        exitDirection = "east";
                        break;
                    case "W":
                        exitDirection = "west";
                        break;
                }

                GameOutput.Client.Groups.Remove(this.ConnectionID, currentRoom.Location.ToString());
                currentRoom.Entities.Remove(this);
                GameOutput.Client.GroupMessage(this.Name + " has exited to the " + exitDirection + ".", currentRoom.Location.ToString());


                string enterDirection = string.Empty;
                string ordinal2 = newRoom.Exits.Where(e => e.Value == currentRoom.Location).FirstOrDefault().Key;
                switch (ordinal2)
                {
                    case "N":
                        enterDirection = "north";
                        break;
                    case "S":
                        enterDirection = "south";
                        break;
                    case "E":
                        enterDirection = "east";
                        break;
                    case "W":
                        enterDirection = "west";
                        break;
                }

                this.Location = newRoom.Location;
                GameOutput.Client.GroupMessage(this.Name + " has entered from the " + enterDirection + ".", newRoom.Location.ToString());
                GameOutput.Client.Groups.Add(this.ConnectionID, newRoom.Location.ToString());
                newRoom.Entities.Add(this);
                newRoom.Display(this.ConnectionID);
            }
            catch
            {
                GameOutput.Client.ClientMessage("Unrecognized location: " + newLocation, this.ConnectionID);
            }
        }
    }
}