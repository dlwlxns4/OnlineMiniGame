using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class BulletCreate : MonoBehaviourPun
{
    public GameObject Bullet;
    public float speed = 1f;
    public int totalBullet = 0;
    public int maxBullet = 15;

    private Vector2[] limitMin = new Vector2[4];
    private Vector2[] limitMax = new Vector2[4];

    public Text bulletText;

    


    bool IsStart(){
        GameObject player = GameObject.Find("Player(Clone)");
        if( player.GetComponent<Move>().state == Move.State.None){
            return true;
        }else{
            return false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Initialized Bullet Create Position
        limitMin[0] = new Vector2(-5,-4);
        limitMax[0] = new Vector2(-5,4);

        limitMin[1] = new Vector2(-5,4);
        limitMax[1] = new Vector2(5,4);

        limitMin[2] = new Vector2(5,-4);
        limitMax[2] = new Vector2(5,4);
        
        limitMin[3] = new Vector2(-5,-4);
        limitMax[3] = new Vector2(5,-4);

        // for(int i=0; i<4; i++){
        //     Vector2 startBullet = new Vector2(-5, Random.Range(-4f, 4f));
        //     PhotonNetwork.Instantiate("Bullet", startBullet, Quaternion.identity);
        // }
        // for(int i=0; i<4; i++){
        //     Vector2 startBullet = new Vector2(5, Random.Range(-4f, 4f));
        //     PhotonNetwork.Instantiate("Bullet", startBullet, Quaternion.identity);
        // }
        // for(int i=0; i<4; i++){
        //     Vector2 startBullet = new Vector2(Random.Range(-5f, 5f), 4f);
        //     PhotonNetwork.Instantiate("Bullet", startBullet, Quaternion.identity);
        // }
        // for(int i=0; i<4; i++){
        //     Vector2 startBullet = new Vector2(Random.Range(-5f, 5f), -4f);
        //     PhotonNetwork.Instantiate("Bullet", startBullet, Quaternion.identity);
        // }

        StartCoroutine(CreateBullet());
        
        StartCoroutine(IncreaseMaxBullet());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        bulletText.text = "총알 갯수 : " + totalBullet ; 
        // Debug.Log(totalBullet);
        // Debug.Log(maxBullet);
        
    }

    IEnumerator CreateBullet(){
        // Debug.Log("총알 생성!");
        while(true && PhotonNetwork.LocalPlayer.IsMasterClient ){
            if(totalBullet < maxBullet+3 && IsStart()){
                int createType = Random.Range(1,5);
                if(createType==1){
                    Vector2 startBullet = new Vector2(-5, Random.Range(-4f, 4f));
                    PhotonNetwork.Instantiate("Bullet", startBullet, Quaternion.identity);
                }else if(createType == 2){
                    Vector2 startBullet = new Vector2(5, Random.Range(-4f, 4f));
                    PhotonNetwork.Instantiate("Bullet", startBullet, Quaternion.identity);
                }else if(createType == 3){
                    Vector2 startBullet = new Vector2(Random.Range(-5f, 5f), 4f);
                    PhotonNetwork.Instantiate("Bullet", startBullet, Quaternion.identity);
                }else if(createType == 4){
                    Vector2 startBullet = new Vector2(Random.Range(-5f, 5f), -4f);
                    PhotonNetwork.Instantiate("Bullet", startBullet, Quaternion.identity);
                }

                totalBullet ++;
            }
            
              yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator IncreaseMaxBullet(){
        while(maxBullet<40){
            maxBullet ++;
            yield return new WaitForSeconds(3f);
        }
    }

    private void OnDrawGizmos(){
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(limitMin[0], limitMax[0]);
        Gizmos.DrawLine(limitMin[1], limitMax[1]);
        Gizmos.DrawLine(limitMin[2], limitMax[2]);
        Gizmos.DrawLine(limitMin[3], limitMax[3]);
    }
}
