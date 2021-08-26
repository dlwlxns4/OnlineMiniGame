using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class GameManager_NumberNumber : MonoBehaviourPunCallbacks
{
    
    struct PlayerRank{
        public string nickName;
        public int playerScore;
    };



    public Image[] image;
    public Text[] image_Text;
    int score;
    public GameObject player;
    [SerializeField] Text scoreText;

    public Text startText;

    public float startTime, endTime; // 제한시간관리 변수
    [SerializeField] Text timeText;


    public enum State {Start, Done, Finish};
    public State state;


    [Header("랭크관리")]
    [SerializeField] GameObject rankpanel;
    [SerializeField] Text[] rankText;
    
    PlayerRank[] playerRanks;
    GameObject RoomPanel;


    // Start is called before the first frame update
    void Start()
    {
        RoomPanel = GameObject.Find("Canvas").transform.Find("RoomPanel").gameObject;
        player = PhotonNetwork.Instantiate("Player_NumberNumber", Vector3.zero, Quaternion.identity);
        CreateNumber();
        StartCoroutine("SetState");
        score = player.GetComponent<Player_NumberNumber>().score;
        scoreText.text = score.ToString();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        PlayGame();
    }

    void PlayGame(){

        //제한시간 관리 파트
        if(state ==State.Done){
            timeText.text = (endTime - Time.realtimeSinceStartup).ToString("N1");
            if(endTime-Time.realtimeSinceStartup <=0){
                timeText.text = 0.ToString();
                state = State.Finish;
                setRank();
            }
        }
    }

    public void CheckNumber(int panelNumber, Image image, Text textNumber){
        if(panelNumber == score+1 && state == State.Done){
            score++;
            scoreText.text = score.ToString();
            image.GetComponentInChildren<Text>().text=null;
            StartCoroutine("DisappearNumber", image);
            textNumber.text=null;
            player.GetComponent<Player_NumberNumber>().score = score;
            if(score == x*25){
                StopCoroutine("DisappearNumber");
                CreateNumber();
            }
        }
    }

    
    IEnumerator DisappearNumber(Image numberImage){
        float fadeCount = 1; // 처음 알파 값
        while(fadeCount > 0){
            
            fadeCount -= 0.1f;
            numberImage.color = new Color(255, 255, 255, fadeCount);
            yield return new WaitForSeconds(0.01f);
        }
        // numberImage.gameObject.SetActive(false);
    }

    int x=0;
    void CreateNumber(){
        int[] ar = new int[25];
        ar = CreateDuplicateRandom(x*25, (x+1)*25);
        for(int i=0; i<25; i++){
            image[i].color = new Color(213 / 255f ,253 / 255f ,255 / 255f ,1);
            image_Text[i].text = ar[i].ToString();
        }
        x++;
    }

    int[] CreateDuplicateRandom(int min, int max){
        int[] ar = new int[25];
        int length=0;
        
        //배열 숫자 초기화
        for(int i=min+1; i<max+1; i++){
            ar[length++]=i;
        }

        int random1,random2, tmp;

        //섞기
        for(int i=0; i<25; i++){
            random1 = Random.Range(0, ar.Length);
            random2 = Random.Range(0, ar.Length);

            tmp = ar[random1];
            ar[random1] = ar[random2];
            ar[random2] = tmp;
        }

        return ar;

    }
    IEnumerator SetState(){
        
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
            
            
            state = State.Done;
            startTime = Time.realtimeSinceStartup;
            
            endTime = startTime + 30;
            
        }
    
    }   


    void setRank(){
        GameObject[] allPlayer;
        allPlayer = GameObject.FindGameObjectsWithTag("GameManager_NumberNumber");
        Debug.Log(allPlayer.Length);
        playerRanks = new PlayerRank[PhotonNetwork.PlayerList.Length];
        Debug.Log(playerRanks.Length);
        //구조체에 정보담기
        for(int i=0; i<PhotonNetwork.PlayerList.Length; i++){
            playerRanks[i].nickName = allPlayer[i].GetComponent<PhotonView>().Owner.NickName;
            playerRanks[i].playerScore = allPlayer[i].GetComponent<Player_NumberNumber>().score;
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


        rankpanel.SetActive(true);
    }

    public void ReturnRoom(){
        SceneManager.LoadScene("InitRoom");
        Time.timeScale = 1;
        RoomPanel.SetActive(true);
        if(PhotonNetwork.IsMasterClient)
            PhotonNetwork.CurrentRoom.IsVisible=true;
    }

}
