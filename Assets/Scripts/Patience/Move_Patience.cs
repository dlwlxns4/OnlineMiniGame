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
        if(photonView.IsMine){
            if( state == State.Done){
                StopCoroutine("SetState");
            }else if( state == State.Start){
                startText.text = 3.ToString();
                yield return new WaitForSeconds(1f);
                startText.text = 2.ToString();
                yield return new WaitForSeconds(1f);
                startText.text = 1.ToString();
                yield return new WaitForSeconds(1f);
                startText.gameObject.SetActive(false);
                
                
                state = State.Done;
                
            }
        }
    }
}
