using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;


public class GameManager_ClickClick : MonoBehaviourPunCallbacks, IPunObservable
{

    struct PlayerRank{
        public string nickName;
        public int playerScore;
    };
    PlayerRank[] playerRanks;
    public GameObject[] player;



    public int score=0;

    public Button foodBtn;
    public GameObject love;

    
    public ClickObjects gameManagerObject;

    // Start is called before the first frame update
    void Start()
    {
        FindObject();
        if(photonView.IsMine)
            StartCoroutine("SetStart");
    }



    //플레이어 오브젝트 생성 후 오브젝트 set
    void FindObject(){

        gameManagerObject = GameObject.Find("GameManager").GetComponent<ClickObjects>();
        foodBtn = GameObject.Find("Canvas_Click").transform.Find("Food").gameObject.GetComponent<Button>();
        if(photonView.IsMine)
            foodBtn.onClick.AddListener(IncreaseScore);
        foodBtn.interactable= false;
    }
    
    void IncreaseScore(){
        score++;
        gameManagerObject.scoreText.text = score.ToString();
    }



    float startTime;
    float endTimeState;
    float endTimeGame;

    //게임 시작 타이머
    IEnumerator SetStart(){
        startTime = Time.realtimeSinceStartup;
        endTimeState = startTime +3;
        endTimeGame = endTimeState + 15;


        while((endTimeState-Time.realtimeSinceStartup) >=0){
            gameManagerObject.stateText.text=(endTimeState-Time.realtimeSinceStartup).ToString("N0");
            yield return new WaitForSeconds(0.05f);
        }
        if((endTimeState-Time.realtimeSinceStartup) <=0){
            gameManagerObject.stateText.text = null;
            foodBtn.interactable= true;
            StartCoroutine("SetGameTime");
        }

    }

    //게임 타이머
    IEnumerator SetGameTime(){


        while((endTimeGame-Time.realtimeSinceStartup) >=0){
            gameManagerObject.timeText.text=(endTimeGame-Time.realtimeSinceStartup).ToString("N1");
            yield return new WaitForSeconds(0.1f);
        }
        if((endTimeGame-Time.realtimeSinceStartup) <=0){
            gameManagerObject.timeText.text=0.ToString();
            Instantiate(love, new Vector3(17.5f,23.5f,0f), Quaternion.identity);
            setRankRPC();
            gameManagerObject.rankPanel.SetActive(true);
            Time.timeScale = 0;
        }
    }




    [PunRPC]
    void setRankRPC(){
        StartCoroutine("delayTime");
        player = GameObject.FindGameObjectsWithTag("GameManager_Click");

        playerRanks = new PlayerRank[PhotonNetwork.PlayerList.Length];
        for(int i=0; i<PhotonNetwork.PlayerList.Length; i++){
            playerRanks[i].nickName = player[i].GetComponent<PhotonView>().Owner.NickName;
            playerRanks[i].playerScore = player[i].GetComponent<GameManager_ClickClick>().score;
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
            gameManagerObject.rankText[i].text = playerRanks[i].nickName + "            " + playerRanks[i].playerScore;
        }
        foodBtn.interactable=false;
        gameManagerObject.cat.SetActive(false);
        gameManagerObject.rankPanel.SetActive(true);

    }
    IEnumerator delayTime(){
        yield return new WaitForSeconds(1f);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting) stream.SendNext(score);
        else score = (int)stream.ReceiveNext();
    }
}
