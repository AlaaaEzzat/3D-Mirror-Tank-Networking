using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RPS
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] TMP_InputField if_PlayerName;

        public void OnStartServerClicked()
        {
            NetworkingManager.Instance.StartServer();
        }

        public void OnStartHostrClicked()
        {
            if (!string.IsNullOrEmpty(if_PlayerName.text))
            {
                NetworkingManager.Instance.UpdatePlayerName(if_PlayerName.text);
                NetworkingManager.Instance.StartHost();
            }
        }

        public void OnStartClientClicked()
        {
            if (!string.IsNullOrEmpty(if_PlayerName.text))
            {
                NetworkingManager.Instance.UpdatePlayerName(if_PlayerName.text);
                NetworkingManager.Instance.StartClient();
            }
        }
    }
}
