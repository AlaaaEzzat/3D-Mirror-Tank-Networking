using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace RPS
{
    public class NetworkingPlayer : NetworkBehaviour
    {
        [SyncVar(hook = nameof(OnUpdateName))] string pName;
        [SyncVar(hook = nameof(OnActionUpdated))] public RPSMove playerMove;
        [SyncVar(hook = nameof(OnUpdateScore))] int Score;

        public RPSMove PlayerMove => playerMove;
        public int Scoreing => Score;

        GameplayUI gameplayUi;

        private void Awake()
        {
            gameplayUi = FindFirstObjectByType<GameplayUI>();

        }

        private void Start()
        {
            NetworkingManager.Instance.AddPlayer(this);
        }

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            CmdUpdatePlayerName(NetworkingManager.Instance.LocalPlayerName);
        }

        //[Command] - Calls an RPC from the client to othe server
        //[ClientRpc] - calls on RPC from the server to all clients
        //[TargetRpc] - calls an RPC from the server to 1 specific clint
        //[client] - Restricts Calling a local method on a client
        //[server] - Restricts Calling a local method on a server

        [Command]
        void CmdUpdatePlayerName(string playerName)
        {
            pName = playerName;
        }

        [Command]
        public void cmdUpdatePlayerMove(RPSMove move)
        {
            playerMove = move;
            NetworkingManager.Instance.CheckToCalculateResults();

        }

        [TargetRpc]
        public void TargetSetEndResult(endResult result)
        {
            if(result == endResult.Win)
            {
                gameplayUi.Win.SetActive(true);
            }
            else if (result == endResult.Lose)
            {
                gameplayUi.Lose.SetActive(true);
            }
            else
            {
                gameplayUi.Draw.SetActive(true);
            }
            Debug.Log($"Result is {result}");
            gameplayUi.ResetResults();
        }

        [Server]
        public void UpdateScore(int val)
        {
            Score = val;
        }

        public void OnUpdateName(string oldVal , string newVal)
        {
            pName = newVal;
            gameplayUi.UpdateName(isLocalPlayer, pName);
        }

        public void OnActionUpdated(RPSMove oldVal, RPSMove newVal)
        {
            playerMove = newVal;
            Debug.Log($"{pName}' player move was {playerMove}");
        }

        public void OnUpdateScore(int oldVal , int newVal)
        {
            Score = newVal;
            gameplayUi.UpdateScore(isLocalPlayer, Score);
        }
    }
}
