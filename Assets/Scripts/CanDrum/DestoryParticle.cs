using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoryParticle : MonoBehaviour
{
    public float time=1f;
    // Start is called before the first frame update
    void Start()
    {
        DestroyParticle();
    }

    void DestroyParticle(){
        Destroy(this.gameObject, time);
    }
}
