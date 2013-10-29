using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using MUDInterface.Entities;
using MUDInterface.Entities.Characters;
using MUDInterface.Entities.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace MUDInterface
{
    public class CommandParser
    {
        private CommandParser() 
        {
            gameClients = GlobalHost.ConnectionManager.GetHubContext<GameHub>().Clients;
            gameGroups = GlobalHost.ConnectionManager.GetHubContext<GameHub>().Groups;

            _commands = new Dictionary<string, Func<KeyValuePair<string, String[]>, bool>>();
            _help = new List<string>();

            initCommandDict();
        }        
        private static CommandParser _instance = new CommandParser();
        public static CommandParser Instance { get { return _instance; } }

        private IHubConnectionContext gameClients;
        private IGroupManager gameGroups;

        private static Dictionary<string, Func<KeyValuePair<string, String[]>, bool>> _commands;
        private static List<string> _help;

        public bool TryCommand(KeyValuePair<string, String[]> cmd)
        {
            string connID = cmd.Key;

            try
            {
                return _commands[cmd.Value[0].ToUpper()].Invoke(cmd);
            }
            catch
            {
                GameOutput.Client.ClientMessage("Unknown command: " + cmd.Value[0], connID);
                return false;
            }
        }

        private void initCommandDict()
        {
            _commands.Add("HELP", Help);

            _commands.Add("SAY", Say);
            _help.Add("say [text]:  Used to speak to others in the same room.  e.g. say Nice to meet you");

            _commands.Add("MOVE", Move);
            _help.Add("move [room #]:  Used to teleport directly to a location based off of the room ID.  e.g. move 9");

            _commands.Add("N", MoveDirection);
            _commands.Add("S", MoveDirection);
            _commands.Add("E", MoveDirection);
            _commands.Add("W", MoveDirection);
            _help.Add("n, s, e, w:  Move in the specified direction.");

            _commands.Add("L", Look);
            _commands.Add("LOOK", Look);
            _help.Add("look, l:  Used to display a room description.  Can use l and look interchangeably.");
            _help.Add("look, l [entity name]:  Used to display the description of an entity.  Can use l and look interchangeably.  e.g. look Thug or l Dwarf");

            _commands.Add("TIME", Time);
            _help.Add("time:  Used to display the running time of the simulation.");

            _commands.Add("SUMMON", Summon);
            _help.Add("summon [entity name]:  Summons a level 1 entity of the specified name to your location.  e.g. summon Thug");
            _help.Add("summon [entity name] [level] [health] [strength]:  Summons an entity with the given attributes to your location.  e.g. summon Dwarf 10 400 100");            
        }

        private bool Help(KeyValuePair<string, String[]> cmd)
        {
            try
            {
                GameOutput.Client.ClientMessage("<br/>", cmd.Key);
                GameOutput.Client.ClientMessage("--------HELP--------", cmd.Key);
                GameOutput.Client.ClientMessage("<br/>", cmd.Key);
                foreach (string s in _help)
                {                    
                    GameOutput.Client.ClientMessage(s, cmd.Key);
                    GameOutput.Client.ClientMessage("<br/>", cmd.Key); 
                }
                GameOutput.Client.ClientMessage("--------------------", cmd.Key);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool Say(KeyValuePair<string, String[]> cmd)
        {
            Player player = null;
            StringBuilder txt = new StringBuilder();

            try
            {
                player = EntityManager.Instance.GetPlayerByConnectionID(cmd.Key);

                //ignore first string in array because that is the command string
                for (int i = 1; i < cmd.Value.Length; i++)
                {
                    txt.Append(cmd.Value[i] + " ");
                }

                GameOutput.Client.GroupMessage(player.Name + ": " + txt.ToString(), player.Location.ToString());
                return true;
            }
            catch
            {
                return false;
            }            
        }

        private bool Move(KeyValuePair<string, String[]> cmd)
        {
            Player player = null;

            try
            {
                if (cmd.Value.Length == 2)
                {
                    player = EntityManager.Instance.GetPlayerByConnectionID(cmd.Key);
                    player.ChangeLocation(Convert.ToInt32(cmd.Value[1]));
                }
                else
                    GameOutput.Client.ClientMessage("Unrecognized command: " + string.Join(" ", cmd.Value), cmd.Key);                

                return true;
            }
            catch
            {
                GameOutput.Client.ClientMessage("Unrecognized command: " + string.Join(" ", cmd.Value), cmd.Key);
                return false;
            }
        }

        private bool Look(KeyValuePair<string, String[]> cmd)
        {
            Player player = null;
            Room currentRoom = null;
            GameEntity targetEntity = null;

            try
            {
                player = EntityManager.Instance.GetPlayerByConnectionID(cmd.Key);
                currentRoom = EntityManager.Instance.GetRoomByLocation(player.Location);

                if (cmd.Value.Length == 1)
                    currentRoom.Display(player.ConnectionID);
                else if (cmd.Value.Length == 2)
                {
                    targetEntity = EntityManager.Instance.GetAllEntities().Where(e => e.Name.ToUpper().Contains(cmd.Value[1].ToUpper())).FirstOrDefault();
                    if (targetEntity != null)
                    {
                        if (targetEntity.Location == player.Location)
                            targetEntity.Display(cmd.Key);
                        else
                            GameOutput.Client.ClientMessage("Unrecognized target: " + cmd.Value[1], cmd.Key);
                    }
                    else
                        GameOutput.Client.ClientMessage("Unrecognized target: " + cmd.Value[1], cmd.Key);
                }
                else
                    GameOutput.Client.ClientMessage("Unrecognized target: " + cmd.Value[1], cmd.Key);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool Time(KeyValuePair<string, String[]> cmd)
        {
            try
            {
                GameOutput.Client.ClientMessage(GameTime.Instance.ToString(), cmd.Key);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool Summon(KeyValuePair<string, String[]> cmd)
        {
            Player player = null;

            try
            {
                player = EntityManager.Instance.GetPlayerByConnectionID(cmd.Key);

                if (cmd.Value.Length == 1)
                    GameOutput.Client.ClientMessage("Must have a target to summon.", cmd.Key);
                else if (cmd.Value.Length == 2)
                {
                    NPC npc = new NPC(10, 1, player.Location, cmd.Value[1]);
                    EntityManager.Instance.RegisterNPC(npc);
                }
                else if (cmd.Value.Length == 4)
                {
                    int health = Convert.ToInt32(cmd.Value[2]);
                    int strength = Convert.ToInt32(cmd.Value[3]);
                    NPC npc = new NPC(health, strength, player.Location, cmd.Value[1]);
                    EntityManager.Instance.RegisterNPC(npc);
                }
                else if (cmd.Value.Length == 5)
                {
                    int health = Convert.ToInt32(cmd.Value[3]);
                    int strength = Convert.ToInt32(cmd.Value[4]);
                    NPC npc = new NPC(health, strength, player.Location, cmd.Value[1]);
                    npc.Level = Convert.ToInt32(cmd.Value[2]);
                    EntityManager.Instance.RegisterNPC(npc);
                }
                else
                    GameOutput.Client.ClientMessage("Unrecognized target: " + cmd.Value[1], cmd.Key);

                return true;
            }
            catch
            {
                GameOutput.Client.ClientMessage("Unrecognized target: " + string.Join(" ", cmd.Value), cmd.Key);
                return false;
            }
        }

        private bool MoveDirection(KeyValuePair<string, String[]> cmd)
        {
            Player player = null;
            Room currentLocation = null;

            try
            {
                if (cmd.Value.Length == 1)
                {
                    player = EntityManager.Instance.GetPlayerByConnectionID(cmd.Key);
                    currentLocation = EntityManager.Instance.GetRoomByLocation(player.Location);

                    if (currentLocation.Exits.Keys.Contains(cmd.Value[0].ToUpper()))
                    {
                        player.ChangeLocation(currentLocation.Exits[cmd.Value[0].ToUpper()]);
                    }
                    else
                    {
                        string direction = string.Empty;
                        string ordinal = cmd.Value[0].ToUpper();
                        switch (ordinal)
                        {
                            case "N":
                                direction = "north";
                                break;
                            case "S":
                                direction = "south";
                                break;
                            case "E":
                                direction = "east";
                                break;
                            case "W":
                                direction = "west";
                                break;
                        }

                        GameOutput.Client.ClientMessage("There is no exit to the " + direction + ".", cmd.Key);
                    }                    
                }
                else
                    GameOutput.Client.ClientMessage("Unrecognized command: " + string.Join(" ", cmd.Value), cmd.Key);

                return true;
            }
            catch
            {
                GameOutput.Client.ClientMessage("Unrecognized command: " + string.Join(" ", cmd.Value), cmd.Key);
                return false;
            }
        }
    }
}