using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CheckNumber : MonoBehaviour, IPointerDownHandler
{
    Text textNumber;
    Image parentImage;
    public GameObject gameManager;

    public void OnPointerDown(PointerEventData eventData)
    {   
        gameManager.GetComponent<GameManager_NumberNumber>().CheckNumber( int.Parse(textNumber.text), parentImage, textNumber);

    }

    // Start is called before the first frame update
    void Start()
    {
        parentImage = GetComponentInParent<Image>();
        textNumber = GetComponentInChildren<Text>();
    }


}
