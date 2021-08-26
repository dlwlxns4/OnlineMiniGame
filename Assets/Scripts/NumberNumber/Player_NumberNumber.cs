using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Player_NumberNumber : MonoBehaviourPunCallbacks, IPunObservable
{
    public int score = 0;


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting) stream.SendNext(score);
        else score = (int)stream.ReceiveNext();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
    
}
