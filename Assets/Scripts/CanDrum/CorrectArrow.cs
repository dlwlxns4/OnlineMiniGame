using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CorrectArrow : MonoBehaviour, IPointerClickHandler
{
    public int ArrowDirection;
    public GameObject gameManager;

    void Start(){
    }

    public void OnPointerClick(PointerEventData eventData){
        gameManager.GetComponent<GameManager_CatDrum>().CheckArrow(ArrowDirection);
    }


}
