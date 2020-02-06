using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonClickEventHandler_2017 : MonoBehaviour
{
    public Button[] mButtonObjects = new Button[4];
    private ColorBlock theRed;
    private ColorBlock theWhite;
    private ColorBlock theBlue;
    private ColorBlock theYellow;
    float[] mButtonPressedTime = new float[7];
    private bool initialized = false;
    public struct buttonData
    {
        public string name;
        public float pressedTime;
    }
    // Start is called before the first frame update
    void Start()
    {        
    }

    void initializeButtonPress()
    {
        for (int i = 0; i < 4; i++)
            mButtonPressedTime[i] = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialized)
        {
            initialized = true;
            Resolution[] resolutions = Screen.resolutions;
            float XResolution = resolutions[0].width;
            float YResolution = resolutions[0].height;
            float buttonWidth = XResolution * 0.2f;
            float buttonHeight = YResolution * 0.2f;
            float buttonXGap = XResolution * 0.05f;
            float buttonYGap = YResolution * 0.05f;            
            Debug.Log("Maharaga: X and Y Resolutions: " + XResolution + ", " + YResolution + new Vector2(buttonWidth, buttonHeight));

            // Verifying this component
            // CurrentMode - 1: Partial(SMRA) 2: Partial SMRV 3: Partial SMLA, 4: Partial SMLV, 5: SMPA, 6: SMAORTA
            /*
            mButtonObjects[0] = GameObject.Find("Others button").GetComponent<Button>();
            mButtonObjects[1] = GameObject.Find("SMPA button").GetComponent<Button>();
            mButtonObjects[2] = GameObject.Find("SMAORTA button").GetComponent<Button>();
            mButtonObjects[3] = GameObject.Find("Reset button").GetComponent<Button>();
            */
            
            /*
            theWhite = mButtonObjects[0].colors;
            theRed = theWhite;
            
            theBlue = theWhite;
            theYellow = theWhite;
            theYellow.normalColor = Color.yellow;
            theYellow.selectedColor = Color.yellow;
            theYellow.highlightedColor = Color.yellow;
            theBlue.normalColor = Color.blue;
            theBlue.selectedColor = Color.blue;
            theBlue.highlightedColor = Color.blue;
            
            theRed.normalColor = Color.red;
            theRed.selectedColor = Color.red;
            theRed.highlightedColor = Color.red;
            */
        }

        else if (EventSystem.current.currentSelectedGameObject != null)
        {

        }
    }

    public void ButtonPressDetectionManager(ButtonPressDetection.buttonData mItem)
    {
        if (mItem.buttonIndex != 7)
        {
            if (mItem.pressedTime <= 0.5f)
            {
                GameObject.Find("HumanHeart").gameObject.SendMessage("ToggleParts", mItem);
                // mButtonObjects[mItem.buttonIndex].enabled = false;
                /*
                if (mButtonObjects[mItem.buttonIndex].colors.normalColor == Color.white)
                    mButtonObjects[mItem.buttonIndex].colors = theRed;
                
                else if(mButtonObjects[mItem.buttonIndex].colors.normalColor == Color.blue)
                    mButtonObjects[mItem.buttonIndex].colors = theYellow;
                else if (mButtonObjects[mItem.buttonIndex].colors.normalColor == Color.yellow)
                    mButtonObjects[mItem.buttonIndex].colors = theBlue;
                    
                else 
                    mButtonObjects[mItem.buttonIndex].colors = theWhite;
                */
                // mButtonObjects[mItem.buttonIndex].enabled = true;
            }
            else
            {
                /*
                foreach(Button checkObj in mButtonObjects)
                {
                    if(checkObj!=mButtonObjects[mItem.buttonIndex])
                    { 
                        if (checkObj.colors == theBlue)
                            checkObj.colors = theWhite;
                        else if(checkObj.colors == theYellow)
                            checkObj.colors = theRed;
                    }
                }
                
                if(mButtonObjects[mItem.buttonIndex].colors == theWhite)
                    mButtonObjects[mItem.buttonIndex].colors = theBlue;
                else if(mButtonObjects[mItem.buttonIndex].colors == theBlue)
                    mButtonObjects[mItem.buttonIndex].colors = theWhite;
                else
                    mButtonObjects[mItem.buttonIndex].colors = theYellow;
                */
                GameObject.Find("HumanHeart").gameObject.SendMessage("ProjectParts", mItem);
            }
            Debug.Log("Maharaga: pointer down check : " + mItem.buttonIndex);
        }
        else
        {
            GameObject.Find("HumanHeart").gameObject.SendMessage("ResetPivot", mItem);
        }
    }

    public void OnClickButton()
    {
        
    }
}

