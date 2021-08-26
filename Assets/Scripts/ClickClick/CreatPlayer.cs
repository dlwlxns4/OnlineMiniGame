using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CreatPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        PhotonNetwork.Instantiate("GameManager_ClickClick", Vector3.zero, Quaternion.identity);
        
    }



}
