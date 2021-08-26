using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;

public class GetScore : MonoBehaviour, IPointerDownHandler
{
    public GameObject myManager;


    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log(123);
        myManager.GetComponent<GameManager_ClickClick>().score++;
    }

    GameObject FindMyManager(){
        GameObject[] manager;
        manager = GameObject.FindGameObjectsWithTag("GameManager_Click");
        for(int i=0; i<manager.Length; i++){
            if(manager[i].GetComponent<PhotonView>().IsMine){
                return manager[i];
            }
        }
        return null;
    }


    // Start is called before the first frame update
    void Start()
    {
        myManager = FindMyManager();
    }


}
