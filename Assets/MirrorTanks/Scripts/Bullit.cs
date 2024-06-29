using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MirrorTanks
{
    public class Bullit : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private float damage;
        Rigidbody rb;
        uint netId;
        int teamId;
        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.velocity = transform.forward * speed;
        }

        public void init(uint netid , int teamid)
        {
            netId = netid;
            this.teamId = teamid;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (NetworkingManager.Instance.IsServer)
            {
                if (other.CompareTag("Player"))
                {
                    if(other.TryGetComponent<NetworkingPlayer>(out NetworkingPlayer netPlayer))
                    {
                        if(netPlayer.TeamID != teamId)
                        {
                            netPlayer.ApplyDamage(damage, netId);
                        }
                        else
                        {
                            Debug.Log("SameTeamYala");
                        }
                    }
                }
            }

            Destroy(gameObject);
        }
    }
}
