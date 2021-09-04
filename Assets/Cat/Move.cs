using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Move : MonoBehaviourPunCallbacks
{
    public float speed = 2f;

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

    
    [Header("UI")]
    [SerializeField] Text nickName;
    
    public State state = State.Start;
    void Start()
    {

        nickName.text = photonView.Owner.NickName;
        nickName.color = photonView.IsMine ?  Color.green :Color.red;


        fixedJoystick = GameObject.Find("Dodge").transform.Find("Canvas_GunFight").gameObject.transform.Find("Fixed_Joystick").gameObject.GetComponent<FixedJoystick>();

        PhotonNetwork.SerializationRate=30;
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
            if( fixedJoystick.Horizontal>0 ) {
                transform.Translate(Vector3.up *speed * Time.deltaTime, Space.Self);
            }
            if( fixedJoystick.Horizontal<0 ){
                transform.Translate(Vector3.down * speed *Time.deltaTime, Space.Self);
            }
            if( fixedJoystick.Vertical<0 ){
                transform.Translate(Vector3.right *speed * Time.deltaTime, Space.Self);
            }
            if( fixedJoystick.Vertical>0){
                transform.Translate(Vector3.left *speed * Time.deltaTime, Space.Self);
            }
        }
    }

    void setRankRPC(){
            playerRank = PhotonNetwork.PlayerList[photonView.Controller.ActorNumber-1].NickName;
              
    }

    [PunRPC]
    void SetStateRPC(){
        StartCoroutine("SetState");
    }
    
    IEnumerator SetState(){
        if(photonView.IsMine){
            if( state == State.None){
                StopCoroutine("SetState");
            }else if( state == State.Start){
                startText.text = 3.ToString("N0");
                yield return new WaitForSeconds(1f);
                startText.text = 2.ToString("N0");
                yield return new WaitForSeconds(1f);
                startText.text = 1.ToString("N0");
                yield return new WaitForSeconds(1f);
                startText.gameObject.SetActive(false);
                
                state = State.None;
                
            }
        }
    }



    //피격판정
    [PunRPC]
    void DamagedPlayerRPC(){
        isSurvived = 0 ;
        gameManager.GetComponent<GameManager_DodgeCat>().survived++;
        this.GetComponentInChildren<SpriteRenderer>().sprite = sprite;
        
        //순위 저장
        setRankRPC();
        
        Debug.Log(PhotonNetwork.PlayerList[photonView.Controller.ActorNumber-1].NickName);

        gameManager.GetComponent<GameManager_DodgeCat>().SetPlayer(playerRank);

        if( gameManager.GetComponent<GameManager_DodgeCat>().survived == gameManager.GetComponent<GameManager_DodgeCat>().totalPlayerNum){
            Time.timeScale = 0 ;
            rankPanel.SetActive(true); //랭크판 표시

            //순위 업데이트후 표시
            gameManager.GetComponent<GameManager_DodgeCat>().UpdateRankPanel();

        }       
    }

    //총알 충도감지 
    void OnTriggerEnter2D(Collider2D collision){
        if(collision.tag == "Bullet"){
            if(isSurvived == 1 && this.photonView.IsMine){ // 내 화면의 총알에 딜레이 맞추기
                photonView.RPC("DamagedPlayerRPC", RpcTarget.AllBuffered);
            }
        }
    }

    [PunRPC]
    void UpdatePlayerNumRPC(){
        photonView.RPC("UpdatePlayerNum", RpcTarget.AllBuffered);
    }
    IEnumerator UpdatePlayerNum(){
        while(true){
            gameManager.GetComponent<GameManager_DodgeCat>().totalPlayerNum = PhotonNetwork.PlayerList.Length;
            yield return new WaitForSeconds(1f);
        }
    }
}
