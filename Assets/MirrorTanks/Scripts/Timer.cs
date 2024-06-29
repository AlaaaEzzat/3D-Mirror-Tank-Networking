using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MirrorTanks
{
    public class Timer : MonoBehaviour
    {
        [SerializeField] private Image uiFill;
        [SerializeField] private TextMeshProUGUI text;
        public bool canRevivePlayer;

        public int duration;

        private int remaningDuration;
        // Start is called before the first frame update

        private void OnEnable()
        {
            canRevivePlayer = true;
            Begin(duration);
        }

        public void Begin(int seconds)
        {
            remaningDuration = seconds;
            StartCoroutine(UpdateTimer());
        }

        IEnumerator UpdateTimer()
        {
            while(remaningDuration >= 0)
            {
                text.text = $"{remaningDuration / 60:00} : {remaningDuration & 60:00}";
                uiFill.fillAmount = Mathf.InverseLerp(0,duration, remaningDuration);
                remaningDuration--;
                yield return new WaitForSeconds(1f);
            }
            onEnd();
        }

        public void onEnd()
        {
            print("Timer Finished");
            canRevivePlayer = false;
        }
    }
}
