namespace MUDInterface
{
    public class GameLoop
    {
        private const int LOOP_PER_MINUTE = 60;
        private const int GAME_SYNC = 1000 / LOOP_PER_MINUTE;
        private const int SECONDS_DEF = LOOP_PER_MINUTE / 2;

        private GameLoop() { }
        private static GameLoop _instance = new GameLoop();
        public static GameLoop Instance 
        {
            get { return _instance; }
        }

        public void Start()
        {
            System.Threading.Thread.Sleep(5000);
            int itr = 0;
            bool gameLive = true;
            int lastSecond = 0;

            while (gameLive)
            {            
                //update game time
                #region GameTime
                System.Threading.Thread.Sleep(GAME_SYNC);

                ++itr;

                if (itr == SECONDS_DEF)
                {
                    ++GameTime.Instance.Game_Seconds;
                    itr = 1;
                }

                if (GameTime.Instance.Game_Seconds == 60)
                {
                    ++GameTime.Instance.Game_Minutes;
                    GameTime.Instance.Game_Seconds = 1;
                }

                if (GameTime.Instance.Game_Minutes == 60)
                {
                    ++GameTime.Instance.Game_Hours;
                    GameTime.Instance.Game_Minutes = 1;
                }

                if (GameTime.Instance.Game_Hours == 24)
                {
                    ++GameTime.Instance.Game_Days;
                    GameTime.Instance.Game_Hours = 1;
                }
                #endregion

                //handle user input
                GameManager.Instance.HandleInput();

                //loop once per game second
                if (GameTime.Instance.Game_Seconds > 0 && (lastSecond + 1) == GameTime.Instance.Game_Seconds)
                {                  
                    //update game
                    GameManager.Instance.Update();

                    if (GameTime.Instance.Game_Seconds == 59)
                        lastSecond = 0;
                    else
                        lastSecond++;
                }
            }
        }        
    }
}
