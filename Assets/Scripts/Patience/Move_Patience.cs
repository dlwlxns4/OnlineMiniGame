using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;


public class Move_Patience : MonoBehaviourPunCallbacks
{
    Rigidbody2D rigidbody2D;
    public float speed = 100f;
    public float jumpPower = 2f;
    public PhotonView photonView;
    [SerializeField] FixedJoystick fixedJoystick;

    [Header("Manage")]
    public State state = State.Start;
    public enum State { Start, Done }; 
    public Text startText;
    public GameObject arrow;

    bool isGround ;


    float startTime;
    float startPlayTime;


    // Start is called before the first frame update
    void Start()
    {
        fixedJoystick = GameObject.Find("All").transform.Find("Canvas_Patience").gameObject.transform.Find("Fixed_Joystick").gameObject.GetComponent<FixedJoystick>();


        rigidbody2D = GetComponent<Rigidbody2D>();    
        
        rigidbody2D.velocity= Vector2.zero;
        startText = GameObject.Find("StartText").GetComponent<Text>();

        StartCoroutine("SetState");       
        
    }

    // Update is called once per frame
    void Update()
    {
        if(photonView.IsMine && state == State.Done){
            if( fixedJoystick.Vertical< 0)
            {
                    rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x,speed * -1);
            }
            else if
            ( fixedJoystick.Vertical> 0){

                    rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x,speed * 1);
            }else
            {
                rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0);
            }

            if (Input.GetKey(KeyCode.Space)){
                Jump();
            } 
        }else{
            rigidbody2D.velocity=  Vector2.zero;
        }
        //땅인지 체크
        isGround = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(-0.4f, 0), 0.07f, 1<< LayerMask.NameToLayer("Ground"));
            
    }

     public void Jump(){

        if(isGround && state == State.Done && photonView.IsMine){
            rigidbody2D.AddForce(Vector2.right*jumpPower);
        }
    }


    IEnumerator SetState(){
        startTime = Time.realtimeSinceStartup;
        startPlayTime = startTime+3.1f;

        if(photonView.IsMine){
            if( state == State.Done){
                StopCoroutine("SetState");
            }else if( state == State.Start){

                while((startPlayTime-Time.realtimeSinceStartup) >0){
                    startText.text = (startPlayTime-Time.realtimeSinceStartup).ToString("N0");

                    yield return new WaitForSeconds(0.1f);
                }
                startText.gameObject.SetActive(false);
                
                state = State.Done;
                
            }
        }
    }
}
