using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sc : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Maharaga: This is located at " + this.transform.position);
        Debug.Log("Maharaga: This is located local at " + this.transform.localPosition);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

