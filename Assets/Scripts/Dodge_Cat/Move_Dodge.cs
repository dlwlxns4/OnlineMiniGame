using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Move_Dodge : MonoBehaviourPunCallbacks
{
    public float speed = 2f;
    public PhotonView photonView;

    // Start is called before the first frame update
    Transform transform;

    [Header("JoyStick")]
    public FixedJoystick fixedJoystick;


    [Header("GameManage")]
    public GameObject rankPanel;
    public int isSurvived=1;
    public Sprite sprite;
    public GameObject gameManager;

    string playerRank;

    [Header("State")]
    public Text startText;
    public enum State { None, Start, StartGame}
    
    public State state = State.Start;
    void Start()
    {
        fixedJoystick = GameObject.Find("GameObject").transform.Find("Canvas_GunFight").transform.Find("Fixed JoyStick").gameObject.GetComponent<FixedJoystick>();

        Time.timeScale=0;
        transform = GetComponent<Transform>();
        gameManager = GameObject.Find("GameManager");
        rankPanel = gameManager.GetComponent<GameManager_DodgeCat>().rankPanel;
        startText = GameObject.Find("StartText").GetComponent<Text>();
        
        if(PhotonNetwork.IsMasterClient)
            photonView.RPC("SetStateRPC", RpcTarget.All);
    }

    // Update is called once per frame
    void Update()
    {
        if(photonView.IsMine && state == State.None){
            if( fixedJoystick.Horizontal<0 ) {
                transform.Translate(Vector3.left *speed * Time.deltaTime, Space.Self);
            }
            if( fixedJoystick.Horizontal>0 ){
                transform.Translate(Vector3.right * speed *Time.deltaTime, Space.Self);
            }
            if( fixedJoystick.Vertical>0 ){
                transform.Translate(Vector3.up *speed * Time.deltaTime, Space.Self);
            }
            if( fixedJoystick.Vertical<0){
                transform.Translate(Vector3.down *speed * Time.deltaTime, Space.Self);
            }
        }
    }

    void setRankRPC(){
            playerRank = PhotonNetwork.PlayerList[photonView.Controller.ActorNumber-1].NickName;
              
    }

    [PunRPC]
    void SetStateRPC(){
        StartCoroutine("SetStart");
    }

    
    float startTime;
    float endTimeState;
    float endTimeGame;

    //?????? ?????? ?????????
    IEnumerator SetStart(){
        startTime = Time.realtimeSinceStartup;
        endTimeState = startTime +3;
        endTimeGame = endTimeState + 15;


        while((endTimeState-Time.realtimeSinceStartup) >=0){
            startText.text=(endTimeState-Time.realtimeSinceStartup).ToString("N0");
            yield return new WaitForSeconds(0.05f);
        }
        if((endTimeState-Time.realtimeSinceStartup) <=0){
            startText.text = null;
            state = State.None;
        }

    }



    //????????????
    [PunRPC]
    void DamagedPlayerRPC(){
        isSurvived = 0 ;
        gameManager.GetComponent<GameManager_DodgeCat>().survived--;
        this.GetComponentInChildren<SpriteRenderer>().sprite = sprite;
        
        //?????? ??????
        setRankRPC();
        

        gameManager.GetComponent<GameManager_DodgeCat>().SetPlayer(playerRank);

        if( gameManager.GetComponent<GameManager_DodgeCat>().survived == 0){
            Time.timeScale = 0 ;
            rankPanel.SetActive(true); //????????? ??????

            //?????? ??????????????? ??????
            gameManager.GetComponent<GameManager_DodgeCat>().UpdateRankPanel();

        }       
    }

    //?????? ???????????? 
    void OnTriggerEnter2D(Collider2D collision){
        if(collision.tag == "Bullet"){
            if(isSurvived == 1 && this.photonView.IsMine){ // ??? ????????? ????????? ????????? ????????? 
                photonView.RPC("DamagedPlayerRPC", RpcTarget.AllBuffered);
            }
        }
    }
}
