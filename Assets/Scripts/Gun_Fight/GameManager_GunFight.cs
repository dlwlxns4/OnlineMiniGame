using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using GoogleMobileAds.Api;


public class GameManager_GunFight : MonoBehaviourPunCallbacks
{

    struct PlayerRank{
        public string nickName;
        public int playerScore;
    };


    GameObject adManager;

    public GameObject[] respawn;
    public Sprite deadSprite;
    public Sprite aliveSprite;

    [SerializeField] Text stateText;
    [SerializeField] Text timeText;
    [SerializeField] float time =5f;
    public GameObject rankPanel;

    public int score=0;

    GameObject myPlayer;

    PlayerRank[] playerRanks;

    public Text[] rankText;
    // Start is called before the first frame update

    public GameObject disconnectPanel;
    public GameObject RoomPanel;

    float endTime;

    
    // Start is called before the first frame update
    void Awake()
    {
        adManager = GameObject.Find("AdManager");
        CreatePlayer();
        StartCoroutine("StartGame");
        
        RoomPanel = GameObject.Find("Canvas").transform.Find("RoomPanel").gameObject;
        PhotonNetwork.Instantiate("Score_Manage", Vector3.zero, Quaternion.identity);
    }

    void CreatePlayer(){
        PhotonNetwork.Instantiate("Player_GunFight", respawn[Random.Range(0,6)].transform.position, transform.rotation).GetComponent<Manage_GunFight>().state=Manage_GunFight.State.None;
    }

    public void Spawn()
    {
        PhotonNetwork.Instantiate("Player_GunFight",  respawn[Random.Range(0,6)].transform.position, transform.rotation).GetComponent<Manage_GunFight>().Spawn();    
    }

    IEnumerator StartGame(){
        stateText.text = 3.ToString();
        yield return new WaitForSeconds(1f);
        stateText.text = 2.ToString();
        yield return new WaitForSeconds(1f);
        stateText.text = 1.ToString();
        yield return new WaitForSeconds(1f);
        
        stateText.gameObject.SetActive(false);

        GameObject[] myPlayer = GameObject.FindGameObjectsWithTag("Player");
        for(int i=0; i<myPlayer.Length; i++){
            if(myPlayer[i].GetComponent<PhotonView>().IsMine){
                myPlayer[i].GetComponent<Manage_GunFight>().state = Manage_GunFight.State.Start;
            }
        }

        endTime = Time.realtimeSinceStartup + 30;
        StartCoroutine("SetTimer");

    }

    IEnumerator Respawn(){
        stateText.gameObject.SetActive(true);   

        stateText.text = 3.ToString();
        yield return new WaitForSeconds(1f);
        stateText.text = 2.ToString();
        yield return new WaitForSeconds(1f);
        stateText.text = 1.ToString();
        yield return new WaitForSeconds(1f);
        
        stateText.gameObject.SetActive(false);
        Spawn();
    }
    
    IEnumerator SetTimer(){
        while(endTime - Time.realtimeSinceStartup >=0){
            timeText.text = (endTime - Time.realtimeSinceStartup).ToString("N1");
            time -= 0.1f;
            if(endTime - Time.realtimeSinceStartup <= 0.1){
                time = 0;
                timeText.text = time.ToString("N1");
            }
            if(time == 0){

                rankPanel.SetActive(true);
                Time.timeScale=0;

                setRank(); //순위 sort후 array
                StopCoroutine("SetTimer");
                
            }
        yield return new WaitForSeconds(0.1f);

        }
    }


    void setRank(){
        GameObject[] allPlayer;
        allPlayer = GameObject.FindGameObjectsWithTag("GameManager_GunFight");
        
        playerRanks = new PlayerRank[PhotonNetwork.PlayerList.Length];

        //구조체에 정보담기
        for(int i=0; i<PhotonNetwork.PlayerList.Length; i++){
            playerRanks[i].nickName = allPlayer[i].GetComponent<PhotonView>().Owner.NickName;
            playerRanks[i].playerScore = allPlayer[i].GetComponent<ScoreManage>().score;
        }

        PlayerRank tmp;

        //점수 sort
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
            rankText[i].text = playerRanks[i].nickName + "            " + playerRanks[i].playerScore;
        }

    }

    public void ReturnRoom(){
        Time.timeScale = 1;
        SceneManager.LoadScene("InitRoom");
        
        adManager.GetComponent<AdmobManager>().ShowBannderAd();
        

        RoomPanel.SetActive(true);
        if(PhotonNetwork.IsMasterClient)
            PhotonNetwork.CurrentRoom.IsVisible=true;
    }
}
