using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoint : MonoBehaviour
{
    [SerializeField] Transform[] wayPoint;
    [SerializeField] float speed = 1f;
    int wayPointNum = 0;

    // Update is called once per frame
    void FixedUpdate()
    {
        MovePath();
    }

    public void MovePath(){
        transform.position = Vector2.MoveTowards(transform.position, wayPoint[wayPointNum].transform.position, speed*Time.deltaTime);
   
        if(transform.position == wayPoint[wayPointNum].transform.position){
            wayPointNum++;
        }

        if(wayPointNum == wayPoint.Length)
            wayPointNum=0;
    }

}
