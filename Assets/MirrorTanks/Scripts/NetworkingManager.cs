using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using RPS;

namespace MirrorTanks
{
    public class NetworkingManager : NetworkManager
    {
        static NetworkingManager instance;
        string localPlayerName;
        int localPlayerTeamID;
        int localPlayerTypeID;

        public bool hostStarted = false;
        public List<NetworkingPlayer> networkingPlayers;
        public List<NetworkingPlayer> Team1;
        public List<NetworkingPlayer> Team2;

        //Properties
        public static NetworkingManager Instance => instance;

        public bool IsServer { get; private set; }

        public bool IsClient { get; private set; }

        public bool IsHost => IsServer && IsClient;

        public string LocalPlayerName => localPlayerName;

        public int LocalPlayerTeamID => localPlayerTeamID;

        public int LocalPlayerTypeID => localPlayerTypeID;



        public NetworkingPlayer LocalPlayer => networkingPlayers.Find(x => x.isLocalPlayer);

        public override void Awake()
        {
            base.Awake();

            if (!instance)
            {
                instance = this;
            }

            DontDestroyOnLoad(gameObject);
        }

        public override void Start()
        {
            base.Start();
            hostStarted = false;
            networkingPlayers = new List<NetworkingPlayer>();
            Team1 = new List<NetworkingPlayer>();
            Team2 = new List<NetworkingPlayer>();
        }

        public void AddPlayer(NetworkingPlayer player)
        {
            if (!networkingPlayers.Contains(player))
            {
                if(player.TeamID == 1) 
                {
                    Team1.Add(player);
                    Debug.Log("Team1 Added Player");
                }
                else
                {
                    Team2.Add(player);
                    Debug.Log(player.TeamID);
                    Debug.Log("Team2 Added Player");

                }
                // networkingPlayers.Add(player);
            }
        }

        public void RemovePlayer(NetworkingPlayer player)
        {
            if (networkingPlayers.Contains(player))
            {
                networkingPlayers.Remove(player);
            }
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            IsServer = true;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            IsClient = true;
        }

        public NetworkingPlayer FindPlayerByNetId(uint netid)
        {
            if(Team1.Find(x => x.netId == netid) != null)
            {
                return Team1.Find(x => x.netId == netid);
            }
            else
            {
                return Team2.Find(x => x.netId == netid);
            }
            //return networkingPlayers.Find(x => x.netId == netid);
        }

        public void UpdatePlayerName(string name)
        {
            localPlayerName = name;
        }

        public void UpdatePlayerTeamID(int TeamId)
        {
            localPlayerTeamID = TeamId;
        }

        public void UpdatePlayerType(int typeid)
        {
            localPlayerTypeID = typeid;
        }
    }
}
