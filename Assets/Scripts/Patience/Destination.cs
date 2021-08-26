using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Destination : MonoBehaviourPunCallbacks
{

    public Text timer;
    float time = 15f;
    bool isGoal = false;
    public GameObject gameManager;
    public GameObject rankPanel;

    public float endTime;

    // Start is called before the first frame update
    void Start()
    {
        
    }


    void OnTriggerEnter2D(Collider2D collision){
        if(collision.tag == "Player" ){

            if(!isGoal)  {
                endTime = Time.realtimeSinceStartup +10;
                StartCoroutine("SetTimer");
                gameManager.GetComponent<GameManager_Patience>().setRank(collision.GetComponent<PhotonView>().Owner.NickName, true);
            }
            isGoal = true;
            
        }
    }

    [PunRPC]
    public void SetTimerRPC(){
        StartCoroutine("SetTimer");
    }

    IEnumerator SetTimer(){
        while(endTime - Time.realtimeSinceStartup >=0){
            if(isGoal){
                timer.text = (endTime - Time.realtimeSinceStartup).ToString("N1");
                time -= 0.1f;
            }
            if(endTime - Time.realtimeSinceStartup < 0.1){
                time = 0;
                timer.text = time.ToString("N1");
            }
            if(time == 0){
                retiredPlayer();

                rankPanel.SetActive(true);
                StopCoroutine("SetTimer");
                
                GameEnd();
            }
        yield return new WaitForSeconds(0.1f);

        }
    }
    
    void GameEnd(){
        Time.timeScale=0;
    }

    void retiredPlayer(){
        GameObject[] retirePlayer;
        retirePlayer = GameObject.FindGameObjectsWithTag("Player");
        for(int i=0; i<retirePlayer.Length; i++){
            gameManager.GetComponent<GameManager_Patience>().setRank(retirePlayer[i].GetComponent<PhotonView>().Owner.NickName, false);
        }
    }
}
