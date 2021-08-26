using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using GoogleMobileAds.Api;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    

    [Header("DisconnectPanel")]
    public GameObject disconnectPanel;
    public InputField NickNameInput;

    [Header("LobbyPanel")]
    public GameObject LobbyPanel;
    public InputField RoomInput;
    public Text WelcomeText;
    public Text LobbyInfoText;
    public Button[] CellBtn;
    public Button PreviousBtn;
    public Button NextBtn;
    public Toggle hideRoomToggle;

    [Header("RoomPanel")]
    public GameObject RoomPanel;
    public Text ListText;
    public Text RoomInfoText;
    public Text[] ChatText;
    public InputField ChatInput;

    public Text[] playerName;


    [Header("ETC")]
    public Text StatusText;
    public PhotonView PV;


    [Header("Ready")]
    public Button readyBtn;
    public int ready_num=0;
    
    [Header("SelectMap")]

    public GameObject mapSelectPanel;
    public int mapNum=1;

    public Sprite[] mapImageArray;
    public Image mapImage;


    List<RoomInfo> myList = new List<RoomInfo>();
    int currentPage = 1, maxPage, multiple;

    public GameObject deconnectRoom;

    string currentRoomName;

    [Header("AD")]
    public GameObject adManager;



    #region 방리스트 갱신
    // ◀버튼 -2 , ▶버튼 -1 , 셀 숫자
    public void MyListClick(int num)
    {
        if (num == -2) --currentPage;
        else if (num == -1) ++currentPage;
        else {
        
            currentRoomName = myList[multiple + num].Name;
            Debug.Log(currentRoomName);
            // PV.RPC("SetInteractable", RpcTarget.AllBuffered);
            
            PhotonNetwork.JoinRoom(myList[multiple + num].Name);
            
            Debug.Log(myList[multiple + num].Name);
        }
        MyListRenewal();
    }

    [PunRPC]
    void SetInteractable(GameObject currentButton){
        currentButton.GetComponent<Button>().interactable=false;
    }

    void MyListRenewal()
    {
        // 최대페이지
        maxPage = (myList.Count % CellBtn.Length == 0) ? myList.Count / CellBtn.Length : myList.Count / CellBtn.Length + 1;

        // 이전, 다음버튼
        PreviousBtn.interactable = (currentPage <= 1) ? false : true;
        NextBtn.interactable = (currentPage >= maxPage) ? false : true;

        // 페이지에 맞는 리스트 대입
        multiple = (currentPage - 1) * CellBtn.Length;
        for (int i = 0; i < CellBtn.Length; i++)
        {
            
            CellBtn[i].interactable = (multiple + i < myList.Count) ? true : false;

            // if(currentRoomName.Equals(myList[multiple + i].Name)){
            //     CellBtn[i].interactable = false;
            // }
            CellBtn[i].transform.GetChild(0).GetComponent<Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].Name : "";
            CellBtn[i].transform.GetChild(1).GetComponent<Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].PlayerCount + "/" + myList[multiple + i].MaxPlayers : "";
        }
    }

    // [PunRPC]
    // void SetInteractable(){
    //     MyListRenewal();
    // }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        int roomCount = roomList.Count;
        for (int i = 0; i < roomCount; i++)
        {
            if (!roomList[i].RemovedFromList)
            {
                if (!myList.Contains(roomList[i])) myList.Add(roomList[i]);
                else myList[myList.IndexOf(roomList[i])] = roomList[i];
            }
            else if (myList.IndexOf(roomList[i]) != -1) myList.RemoveAt(myList.IndexOf(roomList[i]));
        }
        MyListRenewal();
    }
    #endregion

    #region 서버연결
    void Awake() 
    {
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
        PhotonNetwork.AutomaticallySyncScene = true;
        DontDestroyOnLoad(gameObject);
    }
    void Update()
    {
        StatusText.text = PhotonNetwork.NetworkClientState.ToString();
        LobbyInfoText.text = (PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms) + "로비 / " + PhotonNetwork.CountOfPlayers + "접속";
    }

    public void Connect() {
        
        if(NickNameInput.text == "")   
            StatusText.text = "닉네임을 입력해주세요";
        else{

            PhotonNetwork.ConnectUsingSettings();
        }

    } 
    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();

    public override void OnJoinedLobby()
    {
        LobbyPanel.SetActive(true);
        RoomPanel.SetActive(false);
        PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
        WelcomeText.text = PhotonNetwork.LocalPlayer.NickName + "님 환영합니다";
        myList.Clear();
    }
    

    public void Disconnect() {
      PhotonNetwork.Disconnect();
        disconnectPanel.SetActive(true);
    } 
    public override void OnDisconnected(DisconnectCause cause)
    {
        LobbyPanel.SetActive(false);
        RoomPanel.SetActive(false);
    }
    #endregion


    #region 방
    public void CreateRoom(){
        RoomOptions roomOption = new RoomOptions();
        
        roomOption.MaxPlayers = 8;
        if(hideRoomToggle.isOn)
            roomOption.IsVisible=false;
        PhotonNetwork.CreateRoom(RoomInput.text == "" ? "Room" + Random.Range(0, 100) : RoomInput.text, roomOption);
        
    }

    public void JoinRandomRoom() => PhotonNetwork.JoinRandomRoom();

    public void LeaveRoom() {
        if(readyBtn.GetComponentInChildren<Text>().text == "준비 완료")
        {
            readyBtn.GetComponentInChildren<Text>().text =  "준비";
            PV.RPC("decrease_numRPC", RpcTarget.All);
        }

        PhotonNetwork.LeaveRoom();

        
        //방장이 방 나갈 때 
        if(readyBtn.GetComponentInChildren<Text>().text == "게임 시작"){
            readyBtn.GetComponentInChildren<Text>().text = "준비";
        }
    }
    
    public override void OnJoinedRoom()
    {   
        
       
        RoomPanel.SetActive(true);
        
        IsRoomMaster();
        RoomRenewal();
        ChatInput.text = "";
        for (int i = 0; i < ChatText.Length; i++) ChatText[i].text = "";

        RefreshPlayerInRoom();
        

    }

    //플레이리스트 리프레쉬 메소드
    void RefreshPlayerInRoom(){
        

        //플레이리스트 초기화
        for(int i=0; i<playerName.Length; i++){
            playerName[i].text = "";
        }

        for(int i=0; i<PhotonNetwork.PlayerList.Length; i++){
            if(PhotonNetwork.PlayerList[i].IsMasterClient){
                 playerName[i].text = "  " + PhotonNetwork.PlayerList[i].NickName + "  " + "방장";

            }else{

                  playerName[i].text = "  " + PhotonNetwork.PlayerList[i].NickName;
            }
        }

    }

    public override void OnCreateRoomFailed(short returnCode, string message) { RoomInput.text = ""; CreateRoom(); } 

    public override void OnJoinRandomFailed(short returnCode, string message) { RoomInput.text = ""; CreateRoom(); }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RoomRenewal();
        RefreshPlayerInRoom();
        ChatRPC("<color=yellow>" + newPlayer.NickName + "님이 참가하셨습니다</color>");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RoomRenewal();
        RefreshPlayerInRoom();
        ChatRPC("<color=yellow>" + otherPlayer.NickName + "님이 퇴장하셨습니다</color>");
        
    }

    void RoomRenewal()
    {
        RoomInfoText.text = PhotonNetwork.CurrentRoom.Name + " / " + PhotonNetwork.CurrentRoom.PlayerCount + "명 / " + PhotonNetwork.CurrentRoom.MaxPlayers + "최대";
    }
    #endregion


    #region 채팅
    public void Send()
    {
        PV.RPC("ChatRPC", RpcTarget.All, PhotonNetwork.NickName + " : " + ChatInput.text);
        ChatInput.text = "";
    }

    void IsRoomMaster(){
        if(PhotonNetwork.LocalPlayer.IsMasterClient){
            readyBtn.GetComponentInChildren<Text>().text = "게임 시작";
        }

    }


    

    [PunRPC]
    void AbleReadyText(string nickName){
        for(int i=0; i<playerName.Length; i++){
            if(playerName[i].text == "  " + nickName){
                playerName[i].text = "  " + nickName + "  " + "Ready";
            }
        }
    }

    [PunRPC]
    void DisableReadyText(string nickName){
        for(int i=0; i<playerName.Length; i++){
            if(playerName[i].text == "  " + nickName + "  " + "Ready"){
                playerName[i].text = "  " + nickName;;
            }
        }
    }
    
    public void GameReadyRPC(){
        
        
        if(readyBtn.GetComponentInChildren<Text>().text == "준비")
        {
            
            PV.RPC("AbleReadyText", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName);
            PV.RPC("increase_numRPC", RpcTarget.All);
            readyBtn.GetComponentInChildren<Text>().text =  "준비 완료";
        }
        else if(readyBtn.GetComponentInChildren<Text>().text == "준비 완료")
        {
            
            PV.RPC("DisableReadyText", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName);
            readyBtn.GetComponentInChildren<Text>().text =  "준비";
            PV.RPC("decrease_numRPC", RpcTarget.All);
        }
        else if(readyBtn.GetComponentInChildren<Text>().text == "게임 시작")
        {
            if(PhotonNetwork.PlayerList.Length== ready_num+1)
            {
                if(PhotonNetwork.LocalPlayer.IsMasterClient)
                {
                    
                    PhotonNetwork.CurrentRoom.IsVisible=false;
                    PV.RPC("SetActivePanel", RpcTarget.All);
                    
                    PV.RPC("HideBannerViewRPC", RpcTarget.AllBuffered);
                    //맵 선택
                    if(mapNum == 0){
                        int randomMap = Random.Range(0,5);

                        if(randomMap == 0)
                            PhotonNetwork.LoadLevel("Dodge_Cat");
                        else if(randomMap == 1)
                            PhotonNetwork.LoadLevel("Patience");
                        else if(randomMap == 2)
                            PhotonNetwork.LoadLevel("CanDrum");
                        else if(randomMap == 3)
                            PhotonNetwork.LoadLevel("GunFight");
                        else if(randomMap == 4)
                            PhotonNetwork.LoadLevel("NumberNumber");
                        else if(randomMap == 5)
                            PhotonNetwork.LoadLevel("ClickClick");


                    }else if(mapNum == 1)
                        PhotonNetwork.LoadLevel("Dodge_Cat");
                    else if(mapNum == 2)
                        PhotonNetwork.LoadLevel("Patience");
                    else if(mapNum == 3)
                        PhotonNetwork.LoadLevel("CanDrum");
                    else if(mapNum == 4)
                        PhotonNetwork.LoadLevel("GunFight");
                    else if(mapNum == 5)
                        PhotonNetwork.LoadLevel("NumberNumber");
                    else if(mapNum == 6)
                        PhotonNetwork.LoadLevel("ClickClick");
                }
            }
        }
    }

#region Private Methods


[PunRPC]
void HideBannerViewRPC(){
    
    adManager.GetComponent<AdmobManager>().HideBannderAd();
}

void LoadArena()
{

    if (!PhotonNetwork.IsMasterClient)
    {
        Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
    }
    Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
    PhotonNetwork.LoadLevel("Room for " + PhotonNetwork.CurrentRoom.PlayerCount);
    
}


#endregion
    [PunRPC]
    void SetActivePanel(){
        deconnectRoom.SetActive(false);
        LobbyPanel.SetActive(false);
        RoomPanel.SetActive(false);

    }

    //readyButton
    
    [PunRPC] // RPC는 플레이어가 속해있는 방 모든 인원에게 전달한다
    void increase_numRPC()
    {
        ready_num++;
    }

    [PunRPC] // RPC는 플레이어가 속해있는 방 모든 인원에게 전달한다
    void decrease_numRPC(){
            ready_num--;
        }



    [PunRPC] // RPC는 플레이어가 속해있는 방 모든 인원에게 전달한다
    void ChatRPC(string msg)
    {
        bool isInput = false;
        for (int i = 0; i < ChatText.Length; i++)
            if (ChatText[i].text == "")
            {
                isInput = true;
                ChatText[i].text = "  " + msg;
                break;
            }
        if (!isInput) // 꽉차면 한칸씩 위로 올림
        {
            for (int i = 1; i < ChatText.Length; i++) ChatText[i - 1].text = ChatText[i].text;
            ChatText[ChatText.Length - 1].text = "  " + msg;
        }
    }
    #endregion
   

   // MapSelecBtn
   public void OnMapSelect(){
        if(PhotonNetwork.IsMasterClient)
            mapSelectPanel.SetActive(true);
   }

   public void OffMapSelect(){
       mapSelectPanel.SetActive(false);
   }

   public void setMapNum(int num){
       mapNum = num;
       mapSelectPanel.SetActive(false);
       PV.RPC("setMapImage", RpcTarget.AllBuffered, num);
   }  

   [PunRPC]
   void setMapImage(int num){
            mapImage.sprite = mapImageArray[num];    
   }




    public GameObject PlayerListPanel;
    public GameObject ChatPanel;

    #region 플레이어 추방
    [SerializeField] GameObject kickPanel;
    public int kickPlayerNumber;
    public void playerKickPanel(){
        kickPanel.SetActive(true);
        kickPanel.GetComponentInChildren<Text>().text = PhotonNetwork.PlayerList[kickPlayerNumber].NickName +"님을 추방시겠습니까?";
    }

    public void KickNo(){
        kickPanel.SetActive(false);
    }

    public void playerKick(){
        if(!PhotonNetwork.PlayerList[kickPlayerNumber].IsMasterClient)
            PhotonNetwork.CloseConnection(PhotonNetwork.PlayerList[kickPlayerNumber]);
        RefreshPlayerInRoom();
        kickPanel.SetActive(false);
        Debug.Log("추방");

    }

    #endregion
 

    #region 방찾기

    [SerializeField] Text searchRoomText;
    [SerializeField] GameObject searchRoomPanel;

    public void SetSearchRoomPanel(){
        searchRoomPanel.SetActive(true);
    }
    public void OffSearchRoomPanel(){
        searchRoomPanel.SetActive(false);
    }

    public void SearchRoom(){
        bool isEnter;
        isEnter = PhotonNetwork.JoinRoom(searchRoomText.text);
        if(isEnter)
            searchRoomPanel.SetActive(false);
        RefreshPlayerInRoom();
    }


    #endregion

    #region 초기화
    public void init(){
        ready_num=0;
    }
    #endregion
}
