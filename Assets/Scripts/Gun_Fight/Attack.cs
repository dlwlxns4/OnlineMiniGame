using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.EventSystems;



public class Attack : MonoBehaviourPunCallbacks, IPointerDownHandler 


{
    public GameObject myPlayer;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] player;
        player = GameObject.FindGameObjectsWithTag("Player");
        for(int i=0; i<player.Length; i++){
            if(player[i].GetComponent<PhotonView>().IsMine){
                myPlayer = player[i];
                break;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        myPlayer.GetComponent<Manage_GunFight>().CreateBullet();
    }





}
