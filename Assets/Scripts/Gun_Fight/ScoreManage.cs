using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class ScoreManage : MonoBehaviourPunCallbacks
{
    public int score=0;
    public GameObject myPlayer;

    public GameObject scoreText;

    // Start is called before the first frame update
    


    [PunRPC]
    void addScoreRPC(){
        score++;
        scoreText.GetComponent<Text>().text = score.ToString();
    }

}
