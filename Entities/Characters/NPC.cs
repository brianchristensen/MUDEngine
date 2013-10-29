using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MUDInterface.Messaging;
using MUDInterface.States;
using MUDInterface.States.CharacterStates.NPCStates;

namespace MUDInterface.Entities.Characters
{
    public class NPC : GameEntity
    {
        public StateMachine<NPC> StateMachine { get; set; }
        private int BASE_HEALTH { get; set; }
        public int Health { get; set; }
        public int Strength { get; set; }
        public int Level { get; set; }
        public NPC Opponent { get; set; }

        public NPC(int _health, int _strength, int _location = 1, string _name = "Unnamed") : base(_location, _name)
        {
            StateMachine = new StateMachine<NPC>(this);

            StateMachine.SetGlobalState(NPCPassiveState.Instance);
            StateMachine.SetCurrentState(ExploreState.Instance);

            BASE_HEALTH = _health;
            this.Health = BASE_HEALTH;
            this.Strength = _strength;
            Level = 1;
        }

        public void LevelUp()
        {
            ++Level;
            ++Strength;
        }

        public int MAX_HEALTH()
        {
            return BASE_HEALTH + Level;
        }

        public override bool HandleMessage(ref Telegram msg)
        {
            return StateMachine.HandleMessage(ref msg);
        }

        public override void Update()
        {
            StateMachine.Update();
        }

        public override void Display(string connID)
        {
            GameOutput.Client.ClientMessage("------------------", connID);
            GameOutput.Client.ClientMessage("Name    : " + this.Name, connID);
            GameOutput.Client.ClientMessage("Level   : " + this.Level.ToString(), connID);
            GameOutput.Client.ClientMessage("Health  : " + this.Health.ToString() + "/" + this.MAX_HEALTH().ToString(), connID);
            GameOutput.Client.ClientMessage("Strength: " + this.Strength, connID);
            GameOutput.Client.ClientMessage("------------------", connID);
        }
    }
}