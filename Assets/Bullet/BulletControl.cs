using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BulletControl : MonoBehaviourPunCallbacks
{
    public GameObject target;
    Vector2 targetTransform;
    Vector2 targetVector;
    Transform transform;
    public float speed= 0.5f;
    public int totalBullet;
    public GameObject gameManager;
    GameObject[] players;
   
    public PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        SearchPlayer();
        PhotonNetwork.SerializationRate=30;

    }

    void SearchPlayer(){
        

        players = GameObject.FindGameObjectsWithTag("Player");
        int n = Random.Range(0,players.Length);
        //랜덤 플레이어 쫓기
        targetTransform = players[n].transform.position;

        transform = GetComponent<Transform>();
        
        targetTransform.x += Random.Range(0, 1.5f);
        targetTransform.y += Random.Range(0, 1.5f);
        targetTransform.x -= transform.position.x;
        targetTransform.y -= transform.position.y;
        // totalBullet = gameManager.GetComponent<BulletCreate>().totalBullet;
    }


    void Update(){
        if(transform !=null)
            transform.Translate(targetTransform * Time.deltaTime* speed, Space.Self);
    }
    void OnTriggerEnter2D(Collider2D collision){
        
        if(collision.tag == "wall"){   
            DestoryBullet();
        }
    }

    void DestoryBullet(){
        Destroy(this.gameObject);
        
        GameObject.Find("GameManager").GetComponent<BulletCreate>().totalBullet--;
               
    }


}
