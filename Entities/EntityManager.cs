using MUDInterface.Entities.Characters;
using MUDInterface.Entities.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MUDInterface.Entities
{
    public class EntityManager
    {
        private Dictionary<Guid, GameEntity> _entityDict;
        private List<GameEntity> _entityList;
        private List<Room> _locationList;
        private List<Player> _playerList;
        private List<NPC> _npcList;

        private static EntityManager _instance = new EntityManager();
        public static EntityManager Instance { get { return _instance; } }

        private EntityManager()
        {
            _entityDict = new Dictionary<Guid, GameEntity>();
            _entityList = new List<GameEntity>();
            _locationList = new List<Room>();
            _playerList = new List<Player>();
            _npcList = new List<NPC>();
        }

        public void RegisterEntity(GameEntity newEntity)
        {
            _entityDict.Add(newEntity.ID, newEntity);
            _entityList.Add(newEntity);

            Room startingRoom = GetRoomByLocation(newEntity.Location);
            startingRoom.Entities.Add(newEntity);
            GameOutput.Client.GroupMessage(newEntity.Name + " has entered the room.", newEntity.Location.ToString());
        }

        public void RegisterNPC(NPC npc)
        {
            _npcList.Add(npc);
            RegisterEntity(npc);
        }

        public void RegisterPlayer(Player player)
        {
            _playerList.Add(player);
            RegisterEntity(player);
        }

        public void RegisterLocation(Room room)
        {
            _locationList.Add(room);
            _entityDict.Add(room.ID, room);
            _entityList.Add(room);
        }        

        public GameEntity GetEntityByID(Guid id)
        {
            return _entityDict[id];
        }

        public GameEntity GetEntityByName(string name)
        {
            return _entityList.Where(e => e.Name == name).FirstOrDefault();
        }

        public IEnumerable<GameEntity> GetEntitiesByLocation(int location)
        {
            return _entityList.Where(e => e.Location == location);
        }

        public IEnumerable<GameEntity> GetEntitiesByType(Type type)
        {
            return _entityList.Where(e => e.GetType() == type);
        }

        public IEnumerable<NPC> GetAllNPC()
        {
            return _npcList.ToList();
        }

        public IEnumerable<GameEntity> GetAllEntities()
        {
            return _entityList;
        }

        public List<Room> GetRooms()
        {
            return _locationList.ToList();
        }

        public Room GetRandomRoom()
        {
            return _locationList[Chance.Instance.RandInt(0, _locationList.Count - 1)];
        }

        public Room GetRoomByLocation(int location)
        {
            return _locationList.Where(r => r.Location == location).FirstOrDefault();
        }

        public List<Player> GetPlayers()
        {
            return _playerList.ToList();
        }

        public Player GetPlayerByConnectionID(string connID)
        {
            return _playerList.Where(e => e.ConnectionID == connID).FirstOrDefault();
        }

        public Player GetPlayerByName(string name)
        {
            return _playerList.Where(p => p.Name == name).FirstOrDefault();
        }

        public void RemoveEntity(GameEntity entity)
        {
            _entityDict.Remove(entity.ID);
            _entityList.Remove(entity);

            Room currentLocation = GetRoomByLocation(entity.Location);
            currentLocation.Entities.Remove(entity);
            entity.Dispose();
        }

        public void RemoveNPC(NPC npc)
        {
            _npcList.Remove(npc);
            RemoveEntity(npc);
        }

        public void RemovePlayer(Player player)
        {
            _playerList.Remove(player);
            RemoveEntity(player);
        }

        public void RemoveLocation(Room room)
        {
            _locationList.Remove(room);
            _entityDict.Remove(room.ID);
            _entityList.Remove(room);

            room.Dispose();
        }
    }
}
