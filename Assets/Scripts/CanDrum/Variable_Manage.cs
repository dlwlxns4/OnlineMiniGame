using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;



public class Variable_Manage : MonoBehaviourPunCallbacks, IPunObservable
{
struct PlayerRank{
    public string nickName;
    public int playerScore;
};


    public int score;
    public float time=15f;

    public Text timeText;
    public GameManager_CatDrum gameManager;
    // public PhotonView photonView;
    
    public GameObject[] player;
    PlayerRank[] playerRanks;

    bool PaenlCheck = true;


    [Header("Manage")]
    public State state = State.Start;
    public enum State { Start, Done }; 
    public Text startText;
    public GameObject arrow;

    void Awake(){

        timeText = GameObject.Find("TimeText").GetComponent<Text>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager_CatDrum>();
        startText = GameObject.Find("StartText").GetComponent<Text>();
        arrow = GameObject.Find("Canvas_CanDrum").transform.Find("Arrow").gameObject;


        gameManager.startTime = Time.realtimeSinceStartup;
        gameManager.endTime = gameManager.startTime + 33;

        StartCoroutine("SetState");
    }

    void FixedUpdate(){
        if(photonView.IsMine && state == State.Done ){
            score = gameManager.getScore();
            
            
            photonView.RPC("setRank", RpcTarget.All);
        }
    }

    public void setScore(int scr){
        score = scr;
    }

    [PunRPC]
    void setRank(){
        if((gameManager.endTime -Time.realtimeSinceStartup) < 0 ){
            timeText.text = "0";
            photonView.RPC("RankRPC", RpcTarget.AllBuffered);
            
        }else{
            timeText.text = (gameManager.endTime -Time.realtimeSinceStartup).ToString("N1");
            
        }
    }

    IEnumerator SetState(){
        if(photonView.IsMine){
            if( state == State.Done){
                StopCoroutine("SetState");
            }else if( state == State.Start){
                startText.text = 3.ToString();
                yield return new WaitForSeconds(1f);
                startText.text = 2.ToString();
                yield return new WaitForSeconds(1f);
                startText.text = 1.ToString();
                yield return new WaitForSeconds(1f);
                startText.gameObject.SetActive(false);
                
                
                arrow.SetActive(true);
                state = State.Done;
                gameManager.state = GameManager_CatDrum.State.Start;
                Debug.Log(gameManager.startTime+ "  " + gameManager.endTime);
                
            }
        }
    }


    [PunRPC]
    void RankRPC(){
        StartCoroutine("delayTime");


        if(PaenlCheck && photonView.IsMine){
            PaenlCheck = false;
            Time.timeScale=0;
            photonView.RPC("setRankRPC", RpcTarget.AllBuffered);
        }
    }

    IEnumerator delayTime(){
        yield return new WaitForSeconds(1.5f);
    }


    [PunRPC]
    void setRankRPC(){
        player = GameObject.FindGameObjectsWithTag("Player");

        playerRanks = new PlayerRank[PhotonNetwork.PlayerList.Length];
        for(int i=0; i<PhotonNetwork.PlayerList.Length; i++){
            playerRanks[i].nickName = player[i].GetComponent<PhotonView>().Owner.NickName;
            playerRanks[i].playerScore = player[i].GetComponent<Variable_Manage>().score;
            Debug.Log(playerRanks[i].nickName +" : " + playerRanks[i].playerScore);
        }


        PlayerRank tmp;
        //순위 sort
        for(int i=PhotonNetwork.PlayerList.Length-1; i>0; i--){
            for(int j=0; j<i; j++){
                 if(playerRanks[j].playerScore <= playerRanks[j+1].playerScore){
                     tmp = playerRanks[j];
                     playerRanks[j] = playerRanks[j+1];
                     playerRanks[j+1] = tmp;
                 }

            }
        }
        for(int i=0; i<PhotonNetwork.PlayerList.Length; i++){
            gameManager.rankText[i].text = playerRanks[i].nickName + "            " + playerRanks[i].playerScore;
        }

        arrow.SetActive(false);
        if(state == State.Done)
            gameManager.rankPanel.SetActive(true);

    }


    //점수변수 동기화
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting) stream.SendNext(score);
        else score = (int)stream.ReceiveNext();
    }
}
