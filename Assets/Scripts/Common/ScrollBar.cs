using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ScrollBar : MonoBehaviour
{
    public GameObject playerObject;

    public void Scrolling()
    {
        if(playerObject.activeSelf)
            playerObject.SendMessage("OneSecondOff");
        //Debug.Log("On Scroll");
    }
}
