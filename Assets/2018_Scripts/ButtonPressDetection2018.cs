using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using UnityEngine;

public class ButtonPressDetection2018 : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
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
            thisData.buttonIndex = 11;
            thisData.name = "Reset";
        }
        else if (this.name.Equals("TricuspidValve button"))
        {
            thisData.buttonIndex = 10;
            thisData.name = "TricuspidValve";
        }
        else if (this.name.Equals("Coronary button"))
        {
            thisData.buttonIndex = 9;
            thisData.name = "Coronary";
        }
        else if (this.name.Equals("PAValve button"))
        {
            thisData.buttonIndex = 8;
            thisData.name = "PAValve";
        }
        else if (this.name.Equals("LV button"))
        {
            thisData.buttonIndex = 7;
            thisData.name = "LV";
        }
        else if (this.name.Equals("LA button"))
        {
            thisData.buttonIndex = 6;
            thisData.name = "LA";
        }
        else if (this.name.Equals("RV button"))
        {
            thisData.buttonIndex = 5;
            thisData.name = "RV";
        }
        else if (this.name.Equals("RA button"))
        {
            thisData.buttonIndex = 4;
            thisData.name = "RA";
        }
        else if (this.name.Equals("MitralValve button"))
        {
            thisData.buttonIndex = 3;
            thisData.name = "MitralValve";
        }
        else if (this.name.Equals("Aorta button"))
        {
            thisData.buttonIndex = 2;
            thisData.name = "Aorta";
        }
        else if (this.name.Equals("PA button"))
        {
            thisData.buttonIndex = 1;
            thisData.name = "PA";
        }
        else if (this.name.Equals("AortaValve button"))
        {
            thisData.buttonIndex = 0;
            thisData.name = "AortaValve";
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
