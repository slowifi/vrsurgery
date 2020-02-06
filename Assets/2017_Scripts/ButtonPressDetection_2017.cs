using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using UnityEngine;

public class ButtonPressDetection_2017 : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    buttonData thisData;
    public struct buttonData
    {
        public string name;
        public int buttonIndex;
        public float pressedTime;
    }
    // Start is called before the first frame update
    void Start()
    {
        if (this.name.Equals("Reset button"))
        {
            thisData.buttonIndex = 7;
            thisData.name = "Reset button";
        }
        else if (this.name.Equals("Coronary button"))
        {
            thisData.buttonIndex = 6;
            thisData.name = "Coronary";
        }
        else if (this.name.Equals("LV button"))
        {
            thisData.buttonIndex = 5;
            thisData.name = "LV";
        }
        else if (this.name.Equals("LA button"))
        {
            thisData.buttonIndex = 4;
            thisData.name = "LA";
        }
        else if (this.name.Equals("RV button"))
        {
            thisData.buttonIndex = 3;
            thisData.name = "RV";
        }
        else if (this.name.Equals("RA button"))
        {
            thisData.buttonIndex = 2;
            thisData.name = "RA";
        }
        else if (this.name.Equals("Aorta button"))
        {
            thisData.buttonIndex = 1;
            thisData.name = "Aorta";
        }            
        else if (this.name.Equals("PA button"))
        {
            thisData.buttonIndex = 0;
            thisData.name = "PA";
        }        
        thisData.pressedTime = 0;     
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {        
        thisData.pressedTime = Time.time;       
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        thisData.pressedTime = Time.time - thisData.pressedTime;
        GameObject.Find("ButtonManager").gameObject.SendMessage("ButtonPressDetectionManager", thisData);        
    }
}
