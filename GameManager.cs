using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MUDInterface.Entities;
using MUDInterface.Entities.Characters;
using MUDInterface.Entities.Locations;
using System.Collections.Concurrent;
using MUDInterface.States.CharacterStates.NPCStates.Battle;

namespace MUDInterface
{
    //In charge of creating and destroying all game entities
    //Keeps track of global game statistics
    //All user input is queued and routed through this class
    //Creation and loading of player objects
    public class GameManager
    {
        private ConcurrentQueue<KeyValuePair<string, String[]>> _inputQueue;

        private GameManager() 
        {
            _inputQueue = new ConcurrentQueue<KeyValuePair<string, String[]>>();

            createLocations();
            createEntities();
        }

        private static GameManager _instance = new GameManager();
        public static GameManager Instance { get { return _instance; } }

        public void Update()
        {
            //Get lists of game entities that are not dead
            List<GameEntity> allEntities = EntityManager.Instance.GetAllEntities().ToList();
            List<NPC> allNPC = EntityManager.Instance.GetAllNPC().Where(e => !e.StateMachine.IsInState(DeadState.Instance)).ToList();

            //If less than 2 npc, create 2 new ones in random locations
            if (allNPC.Count < 2)
            {
                NPC npc = new NPC(Chance.Instance.RandInt(10, 100), Chance.Instance.RandInt(10, 100), EntityManager.Instance.GetRandomRoom().Location, "Thug" + Chance.Instance.RandInt(1, 999).ToString());
                npc.Level = Chance.Instance.RandInt(1, 10);
                EntityManager.Instance.RegisterNPC(npc);
                NPC npc2 = new NPC(Chance.Instance.RandInt(10, 100), Chance.Instance.RandInt(10, 100), EntityManager.Instance.GetRandomRoom().Location, "Thug" + Chance.Instance.RandInt(1, 999).ToString());
                npc.Level = Chance.Instance.RandInt(1, 10);
                EntityManager.Instance.RegisterNPC(npc2);
            }

            //Update each game entity
            foreach (GameEntity entity in allEntities)
            {
                entity.Update();
            }
        }

        public void HandleInput()
        {
            KeyValuePair<string, String[]> cmd = new KeyValuePair<string,string[]>();

            while (_inputQueue.Count > 0)
            {
                if (_inputQueue.TryDequeue(out cmd))
                {
                    if (cmd.Value.Length > 0)
                        CommandParser.Instance.TryCommand(cmd);
                    else
                        return;
                }
            }
        }

        public void EnqueueCommand(string connID, string [] cmd)
        {
            _inputQueue.Enqueue(new KeyValuePair<string, String[]>(connID, cmd));
        }

        public Player CreateNewPlayer(string connID, string name)
        {
            Player player = new Player(connID, name);
            EntityManager.Instance.RegisterPlayer(player);
            GameOutput.Client.ClientMessage("**********************************", player.ConnectionID);
            GameOutput.Client.ClientMessage("Welcome to WebMUD " + player.Name + "!", player.ConnectionID);

            Room startingRoom = EntityManager.Instance.GetRoomByLocation(1);
            startingRoom.Display(player.ConnectionID);
            
            GameOutput.Client.Groups.Add(player.ConnectionID, player.Location.ToString());
            
            return player;
        }

        public Player LoadPlayer(string playerName)
        {
            Player player = EntityManager.Instance.GetPlayerByName(playerName);
            return player;
        }

        private void createLocations()
        {
            MapLoader.Instance.LoadMap("LocationMap.xml");
        }

        private void createEntities()
        {
            NPC npc1 = new NPC(10, 1, 1, "Limping man");
            EntityManager.Instance.RegisterNPC(npc1);

            NPC npc2 = new NPC(10, 1, 1, "Mysterious stranger");
            EntityManager.Instance.RegisterNPC(npc2);
        }        
    }
}
