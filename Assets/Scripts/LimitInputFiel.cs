using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LimitInputFiel : MonoBehaviour
{
    public int limit=8;

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.GetComponent<InputField>().characterLimit=limit;
    }


}
