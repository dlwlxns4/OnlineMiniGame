using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;

public class Churu : MonoBehaviourPunCallbacks, IPointerDownHandler
{
    [SerializeField] GameObject gameManager;
    StartChuru startChuru;
    public bool isPenalty;
    public int churuLocation;

    public void OnPointerDown(PointerEventData eventData)
    {
        if(startChuru.state == StartChuru.State.Turn){
            if(PhotonNetwork.PlayerList[startChuru.turn].NickName.Equals(PhotonNetwork.LocalPlayer.NickName)){

                startChuru.myManager.GetComponent<PhotonView>().RPC("NextTurn",RpcTarget.AllBuffered);

                if(isPenalty){
                    //벌칙 코드


                    Instantiate(startChuru.myManager.GetComponent<GameManager_Churu>().hate,new Vector3(1.5f, 1.5f, 0), Quaternion.identity);
                }else{
                    Instantiate(startChuru.myManager.GetComponent<GameManager_Churu>().love,new Vector3(1.5f, 1.5f, 0), Quaternion.identity);
                    startChuru.SpreadChuruLocation(churuLocation);
                    startChuru.myManager.GetComponent<PhotonView>().RPC("DeleteChuruRPC", RpcTarget.AllBuffered, churuLocation);
                    startChuru.myManager.GetComponent<PhotonView>().RPC("SetTurnTextRPC",RpcTarget.AllBuffered);
                }

            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        startChuru = gameManager.GetComponent<StartChuru>();
    }

}
