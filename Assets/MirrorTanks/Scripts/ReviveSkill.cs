using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MirrorTanks
{
    public class ReviveSkill : MonoBehaviour
    {
        NetworkingPlayer mainPlayer;
        NetworkingPlayer otherPlayer;
        [SerializeField] private GameObject obj;
        [SerializeField] private Timer timer;
        private bool isRevivingNow;
        private void Start()
        {
            isRevivingNow = false;
            mainPlayer = GetComponentInParent<NetworkingPlayer>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                otherPlayer = other.gameObject.GetComponentInParent<NetworkingPlayer>();
                if(otherPlayer.TeamID == mainPlayer.TeamID && (mainPlayer.IsDead == true && otherPlayer.IsDead != true) && timer.canRevivePlayer == true && isRevivingNow == false)
                {
                    isRevivingNow = true;
                    obj.SetActive(true);
                    StartCoroutine(TimeBeforeRevive());
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if(other.gameObject.name == otherPlayer.gameObject.name)
            {
                isRevivingNow = false;
                obj.SetActive(false);
                StopAllCoroutines();
            }
        }

        IEnumerator TimeBeforeRevive()
        {
            yield return new WaitForSeconds(5);
            mainPlayer.ResetPlayer();
        }
    }
}
