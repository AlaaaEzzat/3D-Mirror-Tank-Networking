using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MirrorTanks
{
    public class MainMenueUi : MonoBehaviour
    {
        [SerializeField] TMP_InputField if_PlayerName;
        [SerializeField] TMP_Dropdown teamSelection;
        [SerializeField] TMP_Dropdown playerType;


        public void OnStartServerClicked()
        {
            NetworkingManager.Instance.StartServer();
        }

        public void OnStartHostrClicked()
        {
            if (!string.IsNullOrEmpty(if_PlayerName.text))
            {
                NetworkingManager.Instance.UpdatePlayerName(if_PlayerName.text);
                NetworkingManager.Instance.UpdatePlayerTeamID(teamSelection.value + 1);
                NetworkingManager.Instance.UpdatePlayerType(playerType.value + 1);
                NetworkingManager.Instance.StartHost();
            }
        }

        public void OnStartClientClicked()
        {
            if (!string.IsNullOrEmpty(if_PlayerName.text))
            {
                NetworkingManager.Instance.UpdatePlayerName(if_PlayerName.text);
                NetworkingManager.Instance.UpdatePlayerTeamID(teamSelection.value + 1);
                NetworkingManager.Instance.UpdatePlayerType(playerType.value + 1);
                NetworkingManager.Instance.StartClient();

            }
        }
    }
}
