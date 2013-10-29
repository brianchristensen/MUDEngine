using MUDInterface.Entities.Characters;

namespace MUDInterface.States.CharacterStates.NPCStates
{
    public class RestState : State<NPC>
    {
        private RestState() { State_Change_Accumulator = 0; Accumulator_Threshold = 10; }
        private static RestState _instance = new RestState();
        public static RestState Instance { get { return _instance; } }

        public int State_Change_Accumulator { get; set; }
        public int Accumulator_Threshold { get; set; }

        public override void Enter(NPC entity)
        {
            GameOutput.Client.GroupMessage(entity.Name + " has sat down for a rest.", entity.Location.ToString());
        }

        public override void Exit(NPC entity)
        {
            GameOutput.Client.GroupMessage(entity.Name + " has gotten up from his rest.", entity.Location.ToString());
        }

        public override bool OnMessage(NPC entity, ref Messaging.Telegram msg)
        {
            return false;
        }

        public override void Execute(NPC entity)
        {
            if (Chance.Instance.Percent(10))
            {
                if (State_Change_Accumulator >= Accumulator_Threshold)
                {
                    //chance to start exploring
                    if (Chance.Instance.Percent(5))
                    {
                        entity.StateMachine.ChangeState(ExploreState.Instance);
                        State_Change_Accumulator = 0;
                    }                                        
                }

                ++State_Change_Accumulator;
            }            
        }
    }
}