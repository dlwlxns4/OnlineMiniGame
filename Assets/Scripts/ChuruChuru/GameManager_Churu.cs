using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class GameManager_Churu : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject[] Churu;
    public GameObject hate, love;
    public int turn; //턴

    public Text turnText;
    [SerializeField] Text stateText;
    public int churuLocation;

    string[] gamePlayer; //모든 유저 모음

    GameObject gameManager;


    // Start is called before the first frame update
    void Awake()
    {
        gameManager = GameObject.Find("GameManager");
        turn = gameManager.GetComponent<StartChuru>().turn;
        SetPenaltyChuru();
        // FindChuruAndText();
    }
    void FindChuruAndText(){
        Churu = GameObject.FindGameObjectsWithTag("Churu");
        stateText = GameObject.Find("Canvas_Churu").transform.Find("StateText").gameObject.GetComponent<Text>();
        turnText = GameObject.Find("Canvas_Churu").transform.Find("TurnText").gameObject.GetComponent<Text>();
    }



    [PunRPC]
    void SetPenaltyChuru(){
        
        if(PhotonNetwork.IsMasterClient){
            int ranNum = Random.Range(0,18); // 벌칙 걸리는 츄르 순서
            Debug.Log(ranNum);
            for(int i=0; i<18; i++){
                if(ranNum==i){
                    Churu[i].GetComponent<Churu>().isPenalty = true;
                }else{
                    Churu[i].GetComponent<Churu>().isPenalty = false;
                
                }
            }
        }
    }

    [PunRPC]
    public void NextTurn(){
        turn++;
        
        if(turn == PhotonNetwork.PlayerList.Length)
            turn=0;

        gameManager.GetComponent<StartChuru>().SpreadTurn(turn);
    }


    public void StartGameRPC(){
            StartCoroutine("StartGame");    
    }
    IEnumerator StartGame(){
        
            stateText.text = 3.ToString();
            yield return new WaitForSeconds(1f);
            stateText.text = 2.ToString();
            yield return new WaitForSeconds(1f);
            stateText.text = 1.ToString();
            yield return new WaitForSeconds(1f);
            
            stateText.gameObject.SetActive(false);

            // gamePlayer = GameObject.FindGameObjectsWithTag("Player");
            
            gameManager.GetComponent<StartChuru>().state = StartChuru.State.Turn;
            turnText.text = PhotonNetwork.PlayerList[0].NickName+"의 차례입니다.";
        
    }

    // void SetPlayerName(){
    //     for(int i=0; i<PhotonNetwork.PlayerList.Length; i++){
    //         gamePlayer[i] = PhotonNetwork.PlayerList[i].NickName;
    //     }
    // }


    [PunRPC]
    public void DeleteChuruRPC(int churuLocation){
        Debug.Log(churuLocation);
        Churu[churuLocation].SetActive(false);
    }

    public void SetObject(GameObject[] setChuru, Text turnText_, Text stateText_){
        Churu = setChuru;
        turnText=turnText_;
        stateText=stateText_;
    }

    [PunRPC]
    public void SetTurnTextRPC(){
        
        turnText.text=PhotonNetwork.PlayerList[turn].NickName;
    }

}
