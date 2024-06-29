using Mirror;
using RPS;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace MirrorTanks
{
    public class NetworkingPlayer : NetworkBehaviour
    {
        Rigidbody rb;
        Vector2 movePos;

        [SerializeField] Transform cannonPivot;
        [SerializeField] float moveSpeed;
        [SerializeField] float rotationSpeed;

        [Header("Tank")]
        [SerializeField] float tankmoveSpeed;
        [SerializeField] float tankRotationSpeed;
        [SerializeField] float tankMaxHealth;
         
        [Header("DPS")]
        [SerializeField] float dpsMoveSpeed;
        [SerializeField] float dpsRotationSpeed;
        [SerializeField] float dpsMaxHealth;

        [Header("UI")]
        [SerializeField] Transform uiRoot;
        [SerializeField] GameplayUi gameplayUi;

        Transform cameraMain;
        [SerializeField] TextMeshProUGUI text_Pname;
        [SerializeField] Image Image;

        [Header("Health")]
        [SerializeField] float maxHealth;
        [SerializeField] GameObject reviveSkill;

        [Header("Shoot")]
        [SerializeField] Bullit bulletPrefab;
        [SerializeField] Transform bulletSpawnPosition;
        [SerializeField] bool isDead;

        [Header("TeamColors")]
        [SerializeField] private Material teamRed;
        [SerializeField] private Material teamGreen;

        //Networking Variables
        [SyncVar(hook = nameof(OnNameUpdated))] string pName;
        [SyncVar(hook = nameof(OnHealthUpdate))] float Hp;
        [SyncVar(hook = nameof(OnTeamIDUpdate))] int teamID;
        [SyncVar(hook = nameof(OnPlayerTypeUpdate))] int playerType;


        //properties

        public int TeamID => teamID;
        public bool IsDead => isDead;
        public int PlayerType => playerType;

        private void Awake()
        {
            gameplayUi = FindFirstObjectByType<GameplayUi>();
        }

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            cameraMain = Camera.main.transform;
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            isDead = false;
        }

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            CmdUpdatePlayerName(NetworkingManager.Instance.LocalPlayerName);
            CmdUpdatePlayerTeamID(NetworkingManager.Instance.LocalPlayerTeamID);
            CmdUpdatePlayerTypeID(NetworkingManager.Instance.LocalPlayerTypeID);
        }

        private void Update()
        {
            if (isLocalPlayer && !isDead)
            {
                //movement
                movePos.x = Input.GetAxis("Horizontal");
                movePos.y = Input.GetAxis("Vertical");
                bool right = Input.GetKey(KeyCode.RightArrow);
                bool left = Input.GetKey(KeyCode.LeftArrow);
                bool shoot = Input.GetKeyDown(KeyCode.Space);
                Vector3 moveVec = new Vector3(movePos.x, 0, movePos.y) * moveSpeed * Time.deltaTime;
                rb.Move(rb.position + moveVec, rb.rotation);

                //Cannon Rotation
                if (right || left)
                {
                    float roationAngle = rotationSpeed * Time.deltaTime;
                    if (left)
                    {
                        roationAngle -= 1;
                    }

                    if (right)
                    {
                        roationAngle += 1;
                    }
                    cannonPivot.Rotate(transform.up, roationAngle);
                }
                if (shoot)
                {
                    CmdPlayerShoot();
                }
            }

            uiRoot.transform.LookAt(cameraMain.transform.position);
        }

        [Command]
        void CmdUpdatePlayerName(string PlayerName)
        {
            pName = PlayerName;
        }

        [Command]
        void CmdUpdatePlayerTeamID(int teamid)
        {
            this.teamID = teamid;
        }

        [Command]
        void CmdUpdatePlayerTypeID(int playerTypeID)
        {
            this.playerType = playerTypeID;
        }

        [Server]
        public void ApplyDamage(float  damage , uint ownerId)
        {
            if(playerType == 1)
            {
                Hp -= (damage / 2);
            }
            else
            {
                Hp -= damage;
            }
            Hp = Mathf.Max(Hp, 0);

            if(Hp == 0)
            {
                RpcDie(ownerId);
            }
        }

        [Command]
        void CmdPlayerShoot()
        {
           Bullit bullet =  Instantiate(bulletPrefab, bulletSpawnPosition.position, cannonPivot.rotation);
            bullet.init(netId , teamID);
            RpcShoot(bullet.transform.position, bullet.transform.rotation);
        }

        [ClientRpc]
        void RpcShoot(Vector3 position , Quaternion roation)
        {
            if (!NetworkingManager.Instance.IsHost)
            {
                Instantiate(bulletPrefab, position, roation);
            }
        }

        [ClientRpc]
        void RpcDie(uint KillerNetID)
        {
            isDead = true;
            gameplayUi.OnUpdateKillStats($"{NetworkingManager.Instance.FindPlayerByNetId(KillerNetID).pName} Killed {pName}");
            CheckAllPlayersDead();
            reviveSkill.SetActive(true);
        }

        #region
        void OnNameUpdated(string oldVal , string newVal)
        {
            pName = newVal;
            text_Pname.text = pName;
        }

        void OnHealthUpdate(float oldVal , float newVal)
        {
            Hp = newVal;
            Debug.Log(Hp);
            Image.transform.localScale = new Vector3(Hp / maxHealth, 1 , 1);
        }

        void OnTeamIDUpdate(int oldVal, int newVal)
        {
            teamID = newVal;
            Debug.Log("ColorUpdate");
            Debug.Log(teamID);

            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = teamID == 2 ? teamGreen : teamRed;
            }
            NetworkingManager.Instance.AddPlayer(this);
        }
        void OnPlayerTypeUpdate(int oldVal, int newVal)
        {
            playerType = newVal;
            if (playerType == 1)
            {
                moveSpeed = tankmoveSpeed;
                maxHealth = tankMaxHealth;
                rotationSpeed = tankRotationSpeed;
            }
            else
            {
                moveSpeed = dpsMoveSpeed;
                maxHealth = dpsMaxHealth;
                rotationSpeed = dpsRotationSpeed;
            }
            Hp = maxHealth;
            Image.transform.localScale = new Vector3(Hp / maxHealth, 1, 1);
        }

        public void CheckAllPlayersDead()
        {
            bool Team1Dead = NetworkingManager.Instance.Team1.Count == 0 || NetworkingManager.Instance.Team1.All(player => player.IsDead);

            bool Team2Dead = NetworkingManager.Instance.Team2.Count == 0 || NetworkingManager.Instance.Team2.All(player => player.IsDead);

            if (Team1Dead && !Team2Dead)
            {
                gameplayUi.OnUpdateWinWindow("Team 2 Wins");
            }
            else if (Team2Dead && !Team1Dead)
            {
                gameplayUi.OnUpdateWinWindow("Team 1 Wins");
            }
        }

        public void ResetPlayer()
        {
            isDead = false;
            Hp = maxHealth;
            reviveSkill.SetActive(false);
            Image.transform.localScale = new Vector3(Hp / maxHealth, 1, 1);
        }
        #endregion
    }
}
