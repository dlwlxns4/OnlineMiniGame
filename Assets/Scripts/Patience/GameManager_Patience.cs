using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using GoogleMobileAds.Api;

public class GameManager_Patience : MonoBehaviourPunCallbacks
{
    public Text[] ranks;
    public int playerNum=0;
    GameObject RoomPanel;
    
    public enum State { noGoal, Goal}
    public State state;
    GameObject adManager;

    // Start is called before the first frame update
    void Awake()
    {
        
        adManager = GameObject.Find("AdManager");
        RoomPanel = GameObject.Find("Canvas").transform.Find("RoomPanel").gameObject;
        CreatePlayer();
    }

    public void setRank(string nickName, bool goal){
        if(goal)
            ranks[playerNum++].text = nickName;
        else
            ranks[playerNum++].text = "Retire" + "       " + nickName;
    }


    //버튼클릭 시 방 돌아가기
    public void ReturnRoom(){
        RoomPanel.SetActive(true);

    
        adManager.GetComponent<AdmobManager>().ShowBannderAd();

        Time.timeScale = 1;
        SceneManager.LoadScene("InitRoom");
        PhotonNetwork.CurrentRoom.IsVisible=true;
    }

    
    //캐릭터 생성
    void CreatePlayer(){
        PhotonNetwork.Instantiate("Player_Patience", new Vector3(0, -10f, 0), transform.rotation);
    }

}
