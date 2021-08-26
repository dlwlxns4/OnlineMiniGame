using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectMap : MonoBehaviour, IPointerClickHandler
{
    public GameObject netWorkManager;
    public int setMapNum;
    public void OnPointerClick(PointerEventData eventData){
        netWorkManager.GetComponent<NetworkManager>().setMapNum(setMapNum);
    }
}
