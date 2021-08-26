using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Patience : MonoBehaviour
{
    Vector2 transform;
    Vector3 moveDir;

    Transform cameraTransform;

    public GameObject player;
    
    GameObject[] isYourPlayer;
    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = GetComponent<Transform>();
        
        
        isYourPlayer = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log(isYourPlayer[0].name);
         foreach(GameObject yourPlayer in isYourPlayer){
            if(yourPlayer.GetComponent<Move_Patience>().photonView.IsMine)
                player = yourPlayer;
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {   
        if(player != null){
            moveDir = Vector3.zero;
            moveDir.z = -10f;
            transform = player.transform.position;
            if(Mathf.Abs(transform.x)> 0){
                moveDir.x = transform.x /12;
                moveDir.y = transform.y;
            }
            if(Mathf.Abs(transform.y)>0){
                moveDir.x = transform.x;
                moveDir.y = transform.y;
            }
            
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, moveDir, 5*Time.deltaTime);
        }
    }

}
