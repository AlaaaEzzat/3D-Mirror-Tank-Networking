using RPS;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace MirrorTanks
{
    public class GameplayUi : MonoBehaviour
    {
        [SerializeField] private GameObject killStats;
        [SerializeField] private TextMeshProUGUI killStats_Text;
        [SerializeField] private GameObject WinWindow;
        [SerializeField] private TextMeshProUGUI winStats_Text;

        private void Start()
        {
            if(killStats != null)
            {
                killStats.SetActive(false);
            }
        }

        public void OnUpdateKillStats(string mgs)
        {
            if(killStats.activeInHierarchy == false)
            {
                killStats.SetActive(true);
                killStats_Text.text = mgs;
                StartCoroutine(TurnKillStatsFalse());
            }
            else if (killStats.activeInHierarchy == true)
            {
                StopCoroutine(TurnKillStatsFalse());
                killStats_Text.text = mgs;
                StartCoroutine(TurnKillStatsFalse());
            }
        }


        public void OnUpdateWinWindow(string mgs)
        {
            WinWindow.SetActive(true);
            winStats_Text.text = mgs;
        }

        IEnumerator TurnKillStatsFalse()
        {
            yield return new WaitForSeconds(4);
            killStats.SetActive(false);
        }
    }
}
