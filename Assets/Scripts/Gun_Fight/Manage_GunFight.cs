using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;

public class Manage_GunFight : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header("Move")]
    FixedJoystick fixedJoystick;
    [SerializeField] float speed=5f;
    [SerializeField] float jumpPower = 8f;
    Rigidbody2D rigidbody2D;
    SpriteRenderer spriteRenderer;
    
    Vector3 curPos;
    bool isGround;

    [Header("UI")]
    [SerializeField] Text nickName;
    [SerializeField] public Image healthBar;
    [SerializeField] Text stateText;
    [SerializeField] Text scoreText;
    public int score;


    [Header("Attack")]
    [SerializeField] GameObject gun;
    int bulletdir=1;

    [Header("Respawn")]
    GameManager_GunFight gameManager;

    public GameObject score_Manager;

    [Header("State")]
    public State state;
    public enum State { None, Start, Dead, Done};
    


    // Start is called before the first frame update
    void Start()
    {
        
        //조이스틱 찾기
        fixedJoystick = GameObject.Find("All").transform.Find("Canvas_GunFight").gameObject.transform.Find("Fixed_Joystick").gameObject.GetComponent<FixedJoystick>();

        //점수 동기화
        score_Manager = FindMyManager();      
        score = score_Manager.GetComponent<ScoreManage>().score;
        score_Manager.GetComponent<ScoreManage>().scoreText = transform.Find("Canvas").transform.Find("score").gameObject;
        scoreText.text = score.ToString();

        stateText =GameObject.Find("Canvas_GunFight").transform.Find("state").gameObject.GetComponent<Text>();



        gameManager = GameObject.Find("GameManager").GetComponent<GameManager_GunFight>();
    

        nickName.text = photonView.Owner.NickName;
        nickName.color = photonView.IsMine ?  Color.green :Color.red;



        if(photonView.IsMine){
            rigidbody2D = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            //카메라에 따라갈 플레이어 찾기
            var CM = GameObject.Find("CM camera").GetComponent<CinemachineVirtualCamera>();
            CM.Follow = transform;
            CM.LookAt = transform;
        }
    }
    [PunRPC]
    void setScoreRPC(){
        
        score = gameManager.score;
        scoreText.text = score.ToString();
    }

    GameObject FindMyManager(){
        GameObject[] score_Manager;
        score_Manager = GameObject.FindGameObjectsWithTag("GameManager_GunFight");
        for(int i=0; i<score_Manager.Length; i++){
            if(score_Manager[i].GetComponent<PhotonView>().Owner.NickName.Equals(this.GetComponent<PhotonView>().Owner.NickName)){
                Debug.Log("찾았다!");

                score_Manager[i].GetComponent<ScoreManage>().myPlayer=this.gameObject;
                return score_Manager[i];
                
            }
        }
        return null;
    }

    

    public bool isRightDir = true;
    // Update is called once per frame
    void Update()
    {
        float axis ;
        if(photonView.IsMine){
            
            //PlayerMove
            if(state != State.None){
                if( fixedJoystick.Vertical< 0){
                    axis = -1f;
                    rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x,speed * axis);
                }else if( fixedJoystick.Vertical> 0){
                    axis =1f;
                    rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x,speed * axis);
                }else{
                    rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0);
                    axis=0;
                }
                if(axis!=0)
                    FlipX(axis);
                
            }

            
            //땅인지 체크
            isGround = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(-0.4f, 0), 0.07f, 1<< LayerMask.NameToLayer("Ground"));
            //PlayerJump
            if(Input.GetKeyDown(KeyCode.Space))
                Jump();

            //PlayerAttack
            if(Input.GetKeyDown(KeyCode.LeftControl) && state != State.None)
                // CreateBullet();
                PhotonNetwork.Instantiate("Bullet_GunFight", gun.transform.position, Quaternion.identity).GetComponent<Bullet>().GetComponent<PhotonView>().RPC("DirRPC", RpcTarget.All, isRightDir ? 1 : -1);
            

        }else if ((transform.position - curPos).sqrMagnitude >= 100) transform.position = curPos;
        else transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 10);
        
        
        
    }

    //좌우 Flip
    void FlipX(float axis){
        if(axis<0){
            gun.transform.localPosition = new Vector3(0.7f,0,0);
            isRightDir = false;
            bulletdir=-1;
        }
        else if(axis>0){
            gun.transform.localPosition = new Vector3(-0.7f,0,0);
            isRightDir = true;
            bulletdir=1;
        }


    }
    public void CreateBullet(){
        if(state != State.None)
            PhotonNetwork.Instantiate("Bullet_GunFight", gun.transform.position, Quaternion.identity).GetComponent<PhotonView>().RPC("DirRPC", RpcTarget.All, bulletdir);
    }

    
    public void Jump(){


        if(isGround && state != State.None && photonView.IsMine){
            Debug.Log(123);
            rigidbody2D.AddForce(Vector2.right*jumpPower);
        }
    }

    public void Hit(string owner){
        healthBar.fillAmount -=0.1f;
        if(healthBar.fillAmount<=0){

            gameManager.StartCoroutine("Respawn");
            photonView.RPC("DestroyPlayerRpc", RpcTarget.All);
        

            GameObject[] ownerManager = GameObject.FindGameObjectsWithTag("GameManager_GunFight");
            for(int i =0; i<ownerManager.Length; i++){
                Debug.Log(ownerManager[i].GetComponent<PhotonView>().Owner.NickName + " " + owner);
                if(ownerManager[i].GetComponent<PhotonView>().Owner.NickName.Equals(owner)){
                    Debug.Log("잡은놈 점수 올랐다  ");

                    ownerManager[i].GetComponent<ScoreManage>().photonView.RPC("addScoreRPC", RpcTarget.All);
                    
                    // score_Manager.GetComponent<ScoreManage>().myPlayer.GetComponent<PhotonView>().RPC("setScore", RpcTarget.AllBuffered);
                    
                    break;
                    // ownerManager[i].GetComponent<PhotonView>().RPC("addScore",RpcTarget.AllBuffered, ownerManager[i]);
                }
            }

            // photonView.RPC("setScore", RpcTarget.AllBuffered);
        }
    }


    [PunRPC]
    void addScore(GameObject owner){
        owner.GetComponent<ScoreManage>().score++;
    }

    IEnumerator RespawnPlayer(){
        stateText.gameObject.SetActive(true);

        stateText.text = 3.ToString();
        yield return new WaitForSeconds(1f);
        stateText.text = 2.ToString();
        yield return new WaitForSeconds(1f);
        stateText.text = 1.ToString();
        yield return new WaitForSeconds(1f);


        stateText.gameObject.SetActive(false);

    }



    [PunRPC]
    void DestroyPlayerRpc(){
        Destroy(gameObject);
    }



    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(gun.transform.position);
            stream.SendNext(healthBar.fillAmount);
        }
        else
        {
            curPos = (Vector3)stream.ReceiveNext();
            gun.transform.position = (Vector3)stream.ReceiveNext();
            healthBar.fillAmount = (float)stream.ReceiveNext();
        }
    }

    // void OnTriggerEnter2D(Collider2D col){
    //     if(col.tag == "Bullet" && healthBar.fillAmount<=0){
    //         Debug.Log(col.GetComponent<PhotonView>().Owner.NickName);
    //     }
    
    // }


    public void Spawn(){
        this.state = State.Start;
        fixedJoystick = GameObject.Find("All").transform.Find("Canvas_GunFight").gameObject.transform.Find("Fixed_Joystick").gameObject.GetComponent<FixedJoystick>();

        //내 오브젝트 죽었을 때만 재연결
        if(photonView.IsMine)
        {
     
        GameObject Jumpbtn =  GameObject.Find("All").transform.Find("Canvas_GunFight").gameObject.transform.Find("JumpBtn").gameObject;
        GameObject Attackbtn =  GameObject.Find("All").transform.Find("Canvas_GunFight").gameObject.transform.Find("AttackBtn").gameObject;

        Jumpbtn.GetComponent<Jump>().myPlayer = this.gameObject;
        Attackbtn.GetComponent<Attack>().myPlayer = this.gameObject;
        }
        
    }
}

