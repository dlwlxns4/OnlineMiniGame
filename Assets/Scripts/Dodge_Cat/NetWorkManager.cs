using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class NetWorkManager : MonoBehaviourPunCallbacks
{   
    public GameObject canvas;

    // Start is called before the first frame update
    void Awake()
    {
        


        CreatePlayer();
    }
    
    void CreatePlayer(){
        PhotonNetwork.Instantiate("Player", Vector3.zero, transform.rotation);
    }

}
