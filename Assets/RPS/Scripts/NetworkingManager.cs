using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

namespace RPS
{
    public class NetworkingManager : NetworkManager
    {
        static NetworkingManager instance;
        string localPlayerName;

        public static NetworkingManager Instance => instance;

        public bool IsServer { get; private set; }

        public bool IsClient { get; private set; }

        public bool IsHost => IsServer && !IsClient;

        public string LocalPlayerName => localPlayerName;

        public List<NetworkingPlayer> networkingPlayers;

        public NetworkingPlayer LocalPlayer => networkingPlayers.Find(x => x.isLocalPlayer);

        public NetworkingPlayer otherPlayer => networkingPlayers.Find(x => !x.isLocalPlayer);

        public override void Awake()
        {
            base.Awake();
            if(!instance)
            {
                instance = this;
            }
            DontDestroyOnLoad(gameObject);
        }

        public override void Start()
        {
           // StartHost();
           base.Start();
            networkingPlayers = new List<NetworkingPlayer>();  
        }

        public void AddPlayer(NetworkingPlayer player)
        {
            if (!networkingPlayers.Contains(player))
            {
                networkingPlayers.Add(player);
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

        public void UpdatePlayerName(string name)
        {
            localPlayerName = name;
        }

        public bool DidAllPlayersMakeMoves()
        {
            return networkingPlayers.TrueForAll(x => x.PlayerMove != RPSMove.None);
        }

        public void CheckToCalculateResults()
        {
            if(!DidAllPlayersMakeMoves())
            {
                return;
            }

            RPSMove p1Move = networkingPlayers[0].PlayerMove;
            RPSMove p2Move = networkingPlayers[1].PlayerMove;
            endResult p1EndResult = endResult.Lose;
            endResult p2EndResult = endResult.Lose;

            if(p1Move == p2Move)
            {
                p1EndResult = p2EndResult = endResult.Draw;
            }
            else
            {
                p1EndResult = p1Move switch
                {
                    RPSMove.Rock => p2Move == RPSMove.Paper ? endResult.Lose : endResult.Win,
                    RPSMove.Paper => p2Move == RPSMove.Scezers ? endResult.Lose : endResult.Win,
                    RPSMove.Scezers => p2Move == RPSMove.Rock ? endResult.Lose : endResult.Win,
                    _=> endResult.Lose

                };

                p2EndResult = p1EndResult == endResult.Win ? endResult.Lose : endResult.Win;

                int p1Score = networkingPlayers[0].Scoreing;
                int p2Score = networkingPlayers[1].Scoreing;

                if (p1EndResult == endResult.Win)
                {
                    p1Score++;
                   // p2Score--;
                }
                else
                {
                  //  p1Score--;
                    p2Score++;
                }
                p1Score = Mathf.Max(p1Score, 0);
                p2Score = Mathf.Max(p2Score, 0);
                networkingPlayers[0].UpdateScore(p1Score);
                networkingPlayers[1].UpdateScore(p2Score);
            }

            LocalPlayer.TargetSetEndResult(p1EndResult);
            otherPlayer.TargetSetEndResult(p2EndResult);


        }
    }

}
