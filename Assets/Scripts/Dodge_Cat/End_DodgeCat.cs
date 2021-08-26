using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GoogleMobileAds.Api;

public class End_DodgeCat : MonoBehaviourPunCallbacks
{
    public GameObject adManager;
    public int survived;
    public GameObject rankPanel;

    public string[] player;
    int playerNum=0;
    public Text[] rank;
    // Start is called before the first frame update

    public GameObject disconnectPanel;
    public GameObject RoomPanel;

    void Start()
    {
        adManager = GameObject.Find("AdManager");
        survived = PhotonNetwork.PlayerList.Length;
        RoomPanel = GameObject.Find("Canvas").transform.Find("RoomPanel").gameObject;
    }

    public void SetPlayer(string nickname){
        player[playerNum++] = nickname;

    }

    public void UpdateRankPanel(){
        int x=0;
        for(int i = playerNum-1; i>=0; i--){
            
            rank[i].text = i+1 + " : " + player[x];
            x++;
        }
    }


    public void ReturnRoom(){
        SceneManager.LoadScene("InitRoom");


        adManager.GetComponent<AdmobManager>().ShowBannderAd();

        Time.timeScale = 1;
        
        RoomPanel.SetActive(true);
        if(PhotonNetwork.IsMasterClient)
            PhotonNetwork.CurrentRoom.IsVisible=true;
    }
}
