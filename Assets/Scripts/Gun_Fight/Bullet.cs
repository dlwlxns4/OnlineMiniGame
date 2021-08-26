using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class Bullet : MonoBehaviourPunCallbacks
{
    [SerializeField] float speed=7f;
    [SerializeField] int dir;


    // Update is called once per frame
    
    void Update()
    {
        transform.Translate(Vector3.up *speed * Time.deltaTime *dir);
    }

    public void SetDir(int dir){
        this.dir = dir;
    }

    [PunRPC]
    void DestoryBullet(){
        Destroy(gameObject);
    }


    //충돌 시 파괴    
    
    void OnTriggerEnter2D(Collider2D col){
        if(col.tag=="Ground") {
            photonView.RPC("DestoryBullet", RpcTarget.All);
        }
        //느린쪽에 맞춰 Hit판정
        if(!photonView.IsMine && col.tag=="Player" && col.GetComponent<PhotonView>().IsMine){
            col.GetComponent<Manage_GunFight>().Hit(photonView.Owner.NickName);
            photonView.RPC("DestoryBullet", RpcTarget.All);
            
        }
        
            

    }

    [PunRPC]
    void DirRPC(int dir){
        this.dir = dir;
    }
}
