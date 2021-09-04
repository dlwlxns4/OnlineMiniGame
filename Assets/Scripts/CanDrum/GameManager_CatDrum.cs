using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using Photon.Pun;

using HashTable = ExitGames.Client.Photon.Hashtable;

public class GameManager_CatDrum : MonoBehaviourPunCallbacks
{
    public GameObject[] arrow_Object;

    public Sprite[] Arrow_Sprite;
    
    //화살표 정보/////////////////
    public Image[] Arrow_Image;
    public int[] randomArrowNum;


    public int currentArrowNum=0;
    //////////////////////////////

    [Header("Score")]
    public int score=0;
    public Text scoreText;
    public int playerNum=0;

    [Header("Animate")]
    public Image catDrum;
    public Sprite[] catDrum_Sprite;
    bool Change_sprite=true;
    public ParticleSystem particle;

    [Header("TimeManage")]
    public Text timeText;
    public float time=15;

    public GameObject rankPanel;
    public GameObject RoomPanel;
    public Text[] rankText;

    public enum State { Ready, Start };
    public State state;


    public float startTime;
    public float endTime;
    

    // Start is called before the first frame update
    void Awake()
    {
        state =State.Ready;
        CreateArrow();
        
        RoomPanel = GameObject.Find("Canvas").transform.Find("RoomPanel").gameObject;
        PhotonNetwork.Instantiate("Manage_CanDrum", Vector3.zero, Quaternion.identity) ;
    }



    void CreateArrow(){
        StopCoroutine("DisappearArrow");
        for(int i=0; i<11; i++){
            int randomArrow = Random.Range(0,4);
            randomArrowNum[i] = randomArrow;
            Arrow_Image[i].color = new Color(255, 255, 255, 1);
            
            Arrow_Image[i].sprite = Arrow_Sprite[randomArrow];
        }
    }


    //정답 체크
    public void CheckArrow(int yourArrowDirection){
        if(time != 0 &&state !=State.Ready){//시간초 다되지 않았을때만 작동
            if(randomArrowNum[currentArrowNum] == yourArrowDirection){
                
                if(currentArrowNum !=10)
                    StartCoroutine("DisappearArrow", Arrow_Image[currentArrowNum]);
                score++;
                scoreText.text = score.ToString();


                currentArrowNum++; 

                if(Change_sprite){
                    catDrum.sprite = catDrum_Sprite[1];
                    Change_sprite=false;
                }else{
                    catDrum.sprite = catDrum_Sprite[0];
                    Change_sprite=true;
                }

                if(currentArrowNum == 11){
                    CreateArrow();
                    currentArrowNum=0;
                }
                Instantiate(particle, new Vector3(0, 0, 0), Quaternion.identity);
            }else{
                score--;
                scoreText.text = score.ToString();
            }
        }
    }

    IEnumerator DisappearArrow(Image Arrow){
        float fadeCount = 1; // 처음 알파 값
        while(fadeCount > 0){
            
            fadeCount -= 0.1f;
            Arrow.color = new Color(255, 255, 255, fadeCount);
            yield return new WaitForSeconds(0.01f);
        }
    }

    [PunRPC]
    void UpdateTimeRPC(){
        StartCoroutine("UpdateTime");
    }
    IEnumerator UpdateTime(){

        while(time>=0){
            timeText.text = time.ToString("N1");
            time -= 0.1f;
            if(time < 0.1){
                time = 0;
                timeText.text = time.ToString("N1");
            }
            if(time == 0){
                Time.timeScale=0;
                

            }
        yield return new WaitForSeconds(0.1f);
        }
    }

    public void ReturnRoom(){
        Time.timeScale = 1;


        SceneManager.LoadScene("InitRoom");

        
        RoomPanel.SetActive(true);
        if(PhotonNetwork.IsMasterClient)
            PhotonNetwork.CurrentRoom.IsVisible=true;
    }

    public int getScore(){
        return score;
    }

}
