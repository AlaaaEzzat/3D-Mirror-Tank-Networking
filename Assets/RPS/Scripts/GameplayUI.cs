using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPS
{
    public enum RPSMove
    {
        None = 0,
        Rock = 1,
        Paper = 2,
        Scezers = 3
    }

    public enum endResult
    {
        Draw,
        Win,
        Lose
    }

    public class GameplayUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI text_p1Name;
        [SerializeField] TextMeshProUGUI text_p1Score;
        [SerializeField] TextMeshProUGUI text_p2Name;
        [SerializeField] TextMeshProUGUI text_p2Score;
        [SerializeField] CanvasGroup Conditions;
        [SerializeField] GameObject lose;
        [SerializeField] GameObject win;
        [SerializeField] GameObject draw;

        public GameObject Lose => lose;
        public GameObject Win => win;
        public GameObject Draw => draw;



        public void UpdateName(bool isLocalPlayer , string pName)
        {
            if (isLocalPlayer)
            {
                text_p1Name.text = pName;  
            }
            else
            {
                text_p2Name.text = pName;            }
        }

        public void OnActionTaken(int Move)
        {
            RPSMove move = (RPSMove) Move;
            NetworkingManager.Instance.LocalPlayer.cmdUpdatePlayerMove(move);
            DiSableInputs();
        }

        public void UpdateScore(bool isLocalPlayer, int score)
        {
            if(isLocalPlayer)
            {
                text_p1Score.text = score.ToString();
            }
            else
            {
                text_p2Score.text = score.ToString();
            }

        }

        public void DiSableInputs()
        {
            Conditions.interactable = false;
        }

        public void ResetResults()
        {
            StartCoroutine(RestartGame());
        }

        IEnumerator RestartGame()
        {
            yield return new WaitForSeconds(3);
            NetworkingManager.Instance.networkingPlayers[0].playerMove = RPSMove.None;
            NetworkingManager.Instance.networkingPlayers[1].playerMove = RPSMove.None;
            Conditions.interactable = true;
            lose.SetActive(false);
            win.SetActive(false);
            draw.SetActive(false);
        }
    }
}
