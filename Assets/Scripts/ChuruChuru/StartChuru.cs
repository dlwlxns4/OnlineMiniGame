using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class StartChuru : MonoBehaviour
{
    [SerializeField] GameObject[] churu;
    [SerializeField] Text stateText;
    [SerializeField] Text turnText;

    public int turn =0;
    public GameObject myManager;

    
    GameObject[] gameManagers;
    

    public enum State { None, Turn, WaiteTurn, Finish}; // 시작준비, 턴, 턴대기, 완료
    public State state;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.Instantiate("GameManager_Churu", Vector3.zero,Quaternion.identity).GetComponent<GameManager_Churu>().SetObject(churu, stateText, turnText);
        myManager = FindMyManager();
        myManager.GetComponent<GameManager_Churu>().StartGameRPC();
        gameManagers = GameObject.FindGameObjectsWithTag("GameManager_Churu");
    }


    GameObject FindMyManager(){
        GameObject[] manager;
        manager = GameObject.FindGameObjectsWithTag("GameManager_Churu");
        for(int i=0; i<manager.Length; i++){
            if(manager[i].GetComponent<PhotonView>().IsMine){
                return manager[i];
            }
        }
        return null;
    }

    public void SpreadChuruLocation(int churuLocation_){
        
        for(int i=0; i<gameManagers.Length; i++){
            gameManagers[i].GetComponent<GameManager_Churu>().churuLocation = churuLocation_;
        }
    }

    public void SpreadTurn(int turn_){
        turn = turn_;
        for(int i=0; i<gameManagers.Length; i++){
            gameManagers[i].GetComponent<GameManager_Churu>().turn=turn;
        }
    }
}
