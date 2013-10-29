using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR;
using System;

namespace MUDInterface
{
    public class GameOutput
    {
        private GameOutput() 
        { 
            gameClients = GlobalHost.ConnectionManager.GetHubContext<GameHub>().Clients;
            Groups = GlobalHost.ConnectionManager.GetHubContext<GameHub>().Groups;
        }

        private static GameOutput _instance = new GameOutput();
        public static GameOutput Client { get { return _instance; } }

        private IHubConnectionContext gameClients;
        public IGroupManager Groups;

        public void GlobalMessage(string mestext)
        {
            string theMessage = formatMessage(mestext);
            gameClients.All.output(theMessage);
        }

        public void GroupMessage(string mestext, string groupName)
        {
            string theMessage = formatMessage(mestext);
            gameClients.Group(groupName).output(theMessage);
        }

        public void ClientMessage(string mestext, string connID)
        {
            string theMessage = formatMessage(mestext);
            gameClients.Client(connID).output(theMessage);
        }

        private string formatMessage(string msg)
        {
            // Initialize message object
            MessageObject mesObj = new MessageObject { text = msg };

            // Initialize JSON serializer
            System.Web.Script.Serialization.JavaScriptSerializer jSerial = new System.Web.Script.Serialization.JavaScriptSerializer();

            // JSON serialize message object
            string jMessage = jSerial.Serialize(mesObj);

            return jMessage;
        }
    }

    public class MessageObject
    {
        public string text { get; set; }
    }    
}