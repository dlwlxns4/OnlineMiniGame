using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class ClickObjects : MonoBehaviourPunCallbacks
{
    public Text scoreText;
    public Text stateText;
    public Text timeText;
    public GameObject rankPanel;
    GameObject RoomPanel;

    public Text[] rankText;

    
    public GameObject cat;
    // Start is called before the first frame update
    void Start()
    {
        
        RoomPanel = GameObject.Find("Canvas").transform.Find("RoomPanel").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReturnRoom(){
        Time.timeScale = 1;
        
        
        PhotonNetwork.OpRemoveCompleteCache();
        SceneManager.LoadScene("InitRoom");

        RoomPanel.SetActive(true);
        if(PhotonNetwork.IsMasterClient)
            PhotonNetwork.CurrentRoom.IsVisible=true;
    }
}
