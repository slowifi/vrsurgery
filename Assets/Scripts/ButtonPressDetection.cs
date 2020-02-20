using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class ButtonPressDetection : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
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

    public void OnPointerDown(PointerEventData eventData)
    {
        thisData.pressedTime = Time.time;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // find 쓰지말고 매니저 하나 둬서 그곳에서 관리하도록 해야겠다.
        if (this.name == "Control button")
        {
            ColorBlock cb = GameObject.Find("Control button_").GetComponent<Button>().colors;
            cb.normalColor = new Color32(176, 48, 48, 255);
            GameObject.Find("Control button_").GetComponent<Button>().colors = cb;

            cb.normalColor = new Color32(137, 96, 96, 255);
            GameObject.Find("Patching button_").GetComponent<Button>().colors = cb;
            GameObject.Find("Measure Distance button_").GetComponent<Button>().colors = cb;
            GameObject.Find("Cutting button_").GetComponent<Button>().colors = cb;
            GameObject.Find("Incision button_").GetComponent<Button>().colors = cb;

            GameObject.Find("HumanHeart").GetComponent<TouchInput>().enabled = true;
            GameObject.Find("Main Camera").gameObject.SendMessage("RendererOverlapping");
            GameObject.Find("Main Camera").gameObject.SendMessage("Initializing");
            // script on/off 
            // GameObject.Find()
        }
        else if (this.name == "Cutting button")
        {
            
            ColorBlock cb = GameObject.Find("Cutting button_").GetComponent<Button>().colors;
            cb.normalColor = new Color32(176, 48, 48, 255);
            GameObject.Find("Cutting button_").GetComponent<Button>().colors = cb;

            cb.normalColor = new Color32(137, 96, 96, 255);
            GameObject.Find("Control button_").GetComponent<Button>().colors = cb;
            GameObject.Find("Measure Distance button_").GetComponent<Button>().colors = cb;
            GameObject.Find("Patching button_").GetComponent<Button>().colors = cb;
            GameObject.Find("Incision button_").GetComponent<Button>().colors = cb;

            GameObject.Find("HumanHeart").GetComponent<TouchInput>().enabled = false;
            // GameObject.Find("Main Camera").GetComponent<RayIntersection>().enabled = true;
            //GameObject.Find("Main Camera").gameObject.SendMessage("Initializing");
            GameObject.Find("Main Camera").gameObject.SendMessage("CuttingOn");
            //GameObject.Find("Main Camera").gameObject.SendMessage("RendererOverlapping");
        }
        else if (this.name == "Patching button")
        {
            ColorBlock cb = GameObject.Find("Patching button_").GetComponent<Button>().colors;
            if(cb.normalColor == new Color32(176,48,48, 255))
            {
                GameObject.Find("Main Camera").gameObject.SendMessage("RendererOverlapping");
                GameObject.Find("Main Camera").gameObject.SendMessage("Initializing");
                cb.normalColor = new Color32(137, 96, 96, 255);
                GameObject.Find("Patching button_").GetComponent<Button>().colors = cb;
                GameObject.Find("HumanHeart").GetComponent<TouchInput>().enabled = true;
                return;
            }
            cb.normalColor = new Color32(176, 48, 48, 255);
            GameObject.Find("Patching button_").GetComponent<Button>().colors = cb;

            cb.normalColor = new Color32(137, 96, 96, 255);
            GameObject.Find("Control button_").GetComponent<Button>().colors = cb;
            GameObject.Find("Measure Distance button_").GetComponent<Button>().colors = cb;
            GameObject.Find("Cutting button_").GetComponent<Button>().colors = cb;
            GameObject.Find("Incision button_").GetComponent<Button>().colors = cb;

            GameObject.Find("HumanHeart").GetComponent<TouchInput>().enabled = false;
            // GameObject.Find("Main Camera").GetComponent<RayIntersection>().enabled = true;
            GameObject.Find("Main Camera").gameObject.SendMessage("Initializing");
            GameObject.Find("Main Camera").gameObject.SendMessage("PatchingOn");
        }
        else if (this.name == "Measure Distance button")
        {
            ColorBlock cb = GameObject.Find("Measure Distance button_").GetComponent<Button>().colors;
            cb.normalColor = new Color32(176, 48, 48, 255);
            GameObject.Find("Measure Distance button_").GetComponent<Button>().colors = cb;

            cb.normalColor = new Color32(137, 96, 96, 255);
            GameObject.Find("Control button_").GetComponent<Button>().colors = cb;
            GameObject.Find("Patching button_").GetComponent<Button>().colors = cb;
            GameObject.Find("Cutting button_").GetComponent<Button>().colors = cb;
            GameObject.Find("Incision button_").GetComponent<Button>().colors = cb;

            GameObject.Find("HumanHeart").GetComponent<TouchInput>().enabled = false;
            // GameObject.Find("Main Camera").GetComponent<RayIntersection>().enabled = true;
            GameObject.Find("Main Camera").gameObject.SendMessage("Initializing");
            GameObject.Find("Main Camera").gameObject.SendMessage("RendererOverlapping");
            GameObject.Find("Main Camera").gameObject.SendMessage("MeasuringOn");
        }
        else if (this.name == "Incision button")
        {
            ColorBlock cb = GameObject.Find("Incision button_").GetComponent<Button>().colors;
            if (cb.normalColor == new Color32(176, 48, 48, 255))
            {
                // GameObject.Find("Main Camera").gameObject.SendMessage("RendererOverlapping");
                GameObject.Find("Main Camera").gameObject.SendMessage("Initializing");
                cb.normalColor = new Color32(137, 96, 96, 255);
                GameObject.Find("Incision button_").GetComponent<Button>().colors = cb;
                GameObject.Find("HumanHeart").GetComponent<TouchInput>().enabled = true;
                return;
            }
            cb.normalColor = new Color32(176, 48, 48, 255);
            GameObject.Find("Incision button_").GetComponent<Button>().colors = cb;

            cb.normalColor = new Color32(137, 96, 96, 255);
            GameObject.Find("Main Camera").gameObject.SendMessage("RendererOverlapping");
            GameObject.Find("Control button_").GetComponent<Button>().colors = cb;
            GameObject.Find("Patching button_").GetComponent<Button>().colors = cb;
            GameObject.Find("Cutting button_").GetComponent<Button>().colors = cb;
            GameObject.Find("Measure Distance button_").GetComponent<Button>().colors = cb;

            GameObject.Find("HumanHeart").GetComponent<TouchInput>().enabled = false;
            // GameObject.Find("Main Camera").GetComponent<RayIntersection>().enabled = true;
            GameObject.Find("Main Camera").gameObject.SendMessage("Incisioning");
        }
        else
        {
            thisData.pressedTime = Time.time - thisData.pressedTime;
            GameObject.Find("ButtonManager").gameObject.SendMessage("ButtonPressDetectionManager", thisData);
        }

    }
}
