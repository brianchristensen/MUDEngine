using System.Web;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR;
using MUDInterface.Entities;

namespace MUDInterface
{
    [HubName("gameHub")]
    public class GameHub : Hub
    {
        // New client joins the MUD
        public void Join()
        {
            Clients.Caller.stdout("****|| Ascendant Reverie ||****");
            System.Threading.Thread.Sleep(1000);
            Clients.Caller.stdout("__________________________________________________________________________________________________");
            Clients.Caller.stdout("New character or Load character?");
        }

        // Existing client exits the MUD
        public override Task OnDisconnected()
        {
            Player player = EntityManager.Instance.GetPlayerByConnectionID(Context.ConnectionId);
            EntityManager.Instance.RemovePlayer(player);
            return Clients.All.stdout(player.Name + " has left WebMUD.");
        }

        // Client sends command which is queued 
        public void Send(string mestext)
        {
            Player player = EntityManager.Instance.GetPlayerByConnectionID(Context.ConnectionId);
            string[] command = mestext.Split(' ');

            if (player != null)
            {                
                GameManager.Instance.EnqueueCommand(Context.ConnectionId, command);
            }
            else
            {
                if (command[0].ToUpper() == "LOAD")
                {
                    if (command.Length == 2)
                        player = GameManager.Instance.LoadPlayer(command[1]);
                    else
                        Clients.Caller.stdout("Enter a name after 'load' to load a persona.");
                }
                else if (command[0].ToUpper() == "NEW")
                {
                    if (command.Length == 2)
                        player = GameManager.Instance.CreateNewPlayer(Context.ConnectionId, command[1]);
                    else
                        Clients.Caller.stdout("Enter a name after 'new' to create a new persona.");
                }
                else
                    Clients.Caller.stdout("Unknown command: " + command[0]);                    
            }
        }        
    }
}