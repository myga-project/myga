using UnityEngine;
using MygaServer;

namespace MygaClient
{
    public class Connection : MonoBehaviour
    {
        [SerializeField] private string ip = "127.0.0.1";
        [SerializeField] private int port = 7777;
        [SerializeField] private int maxPlayers = 10;

        private void OnGUI()
        {
            if(GUI.Button(new Rect(10, 50, 125, 30), "Server & Client"))
            {
                RunServer();
                RunClient();
            }

            if(GUI.Button(new Rect(10, 85, 125, 30), "Server"))
            {
                RunServer(); 
            }

            if(GUI.Button(new Rect(10, 120, 125, 30), "Client"))
            {
                RunClient();
            }
        }

        private void RunServer()
        {
            Server.Start(ip, port, maxPlayers);
        }

        private void RunClient()
        {
            MygaNetwork.Connect(ip, port);
        }
    }
}
