using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;

public class PlayerInforInRoom : MonoBehaviourPunCallbacks, IPointerDownHandler
{
    [SerializeField] int PlayerNum;
    [SerializeField] GameObject netWorkManager;

    public void OnPointerDown(PointerEventData eventData)
    {
        //마스터 클라이언트 일때만 추방 가능
        if(PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            netWorkManager.GetComponent<NetworkManager>().kickPlayerNumber= PlayerNum;
            netWorkManager.GetComponent<NetworkManager>().playerKickPanel();
        }
    
    }

    int getPlayerNum(){ return PlayerNum; }

}
