using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class ButtonPress : Singleton<ButtonPress>
{
    /// <summary>
    /// 여기도 대규모 수정 필요함.
    /// </summary>
    /// 

    public GameObject MainManager;
    public Button CutButton;
    public Button SliceButton;
    public Button PatchButton;
    public Button MeasureButton;
    public Button IncisionButton;

    private Sprite[] cutImage;
    private Sprite[] sliceImage;
    private Sprite[] patchImage;
    private Sprite[] measureImage;
    private Sprite[] incisionImage;

    //private ColorBlock selectedColorBlock;
    //private ColorBlock unselectedColorBlock;

    private void Awake()
    {
        EventManager.Instance.Events.OnModeChanged += Events_OnModeChanged;
        cutImage = new Sprite[2];
        sliceImage = new Sprite[2];
        patchImage = new Sprite[2];
        measureImage = new Sprite[2];
        incisionImage = new Sprite[2];

        cutImage[0] = Resources.Load("UI/Icon/Icon_D_0", typeof(Sprite)) as Sprite;
        cutImage[1] = Resources.Load("UI/Icon/Icon_D_1", typeof(Sprite)) as Sprite;

        sliceImage[0] = Resources.Load("UI/Icon/Icon_B_0", typeof(Sprite)) as Sprite;
        sliceImage[1] = Resources.Load("UI/Icon/Icon_B_1", typeof(Sprite)) as Sprite;

        patchImage[0] = Resources.Load("UI/Icon/Icon_C_0", typeof(Sprite)) as Sprite;
        patchImage[1] = Resources.Load("UI/Icon/Icon_C_1", typeof(Sprite)) as Sprite;

        //measureImage[0].sprite = Resources.Load("UI/Icon/Icon_D_0", typeof(Sprite)) as Sprite;
        //measureImage[1].sprite = Resources.Load("UI/Icon/Icon_D_1", typeof(Sprite)) as Sprite;

        incisionImage[0] = Resources.Load("UI/Icon/Icon_A_0", typeof(Sprite)) as Sprite;
        incisionImage[1] = Resources.Load("UI/Icon/Icon_A_1", typeof(Sprite)) as Sprite;

        //selectedColorBlock = SliceButton.colors;
        //unselectedColorBlock = SliceButton.colors;
        //selectedColorBlock.normalColor = new Color32(176, 48, 48, 255);
    }
    
    public void Events_OnModeChanged(string mode)
    {
        switch(mode)
        {
            case "Incision":
                EventManager.Instance.Events.InvokeModeChanged("IncisionMode");
                Debug.Log("incision 실행");
                break;
            case "Cut":
                EventManager.Instance.Events.InvokeModeChanged("CutMode");
                Debug.Log("cut 실행");
                break;
            case "Patch":
                EventManager.Instance.Events.InvokeModeChanged("PatchMode");
                Debug.Log("patch 실행");
                break;
            case "Slice":
                EventManager.Instance.Events.InvokeModeChanged("SliceMode");
                Debug.Log("slice 실행");
                break;
            case "Measure":
                EventManager.Instance.Events.InvokeModeChanged("MeasureMode");
                Debug.Log("measure 실행");
                break;
            case "ResetButton":
                ResetButton();
                break;
        }
    }


    public void ResetButton()
    {
        SliceButton.image.sprite = sliceImage[0];
        CutButton.image.sprite = cutImage[0];
        PatchButton.image.sprite = patchImage[0];
        IncisionButton.image.sprite = incisionImage[0];
        //MeasureButton.colors = colorTemp;
    }

    public void Slicing()
    {
        // 지금 이 지저분한 코드도 수정할 수 있으면 하도록하기.
        if (SliceButton.colors.normalColor == new Color32(176, 48, 48, 255))
        {
            SliceButton.image.sprite = sliceImage[0];
            EventManager.Instance.Events.InvokeModeChanged("Exit");
        }
        else
        {
            ResetButton();
            SliceButton.image.sprite = sliceImage[1];
            EventManager.Instance.Events.InvokeModeChanged("Exit");
            Events_OnModeChanged("Slice");
        }
    }


    public void Cutting()
    {
        if (CutButton.colors.normalColor == new Color32(176, 48, 48, 255))
        {
            CutButton.image.sprite = cutImage[0];
            EventManager.Instance.Events.InvokeModeChanged("Exit");
        }
        else
        {
            ResetButton();
            CutButton.image.sprite = cutImage[1];
            EventManager.Instance.Events.InvokeModeChanged("Exit");
            Events_OnModeChanged("Cut");
        }
    }

    public void Patching()
    {
        
        if (PatchButton.colors.normalColor == new Color32(176, 48, 48, 255))
        {
            PatchButton.image.sprite = patchImage[0];
            EventManager.Instance.Events.InvokeModeChanged("Exit");
        }
        else
        {
            ResetButton();
            PatchButton.image.sprite = patchImage[1];
            EventManager.Instance.Events.InvokeModeChanged("Exit");
            Events_OnModeChanged("Patch");
        }
    }

    public void Measuring()
    {
        if (MeasureButton.colors.normalColor == new Color32(176, 48, 48, 255))
        {
            //MeasureButton.colors = unselectedColorBlock;
            EventManager.Instance.Events.InvokeModeChanged("Exit");
        }
        else
        {
            ResetButton();
            //MeasureButton.colors = selectedColorBlock;
            EventManager.Instance.Events.InvokeModeChanged("Exit");
            Events_OnModeChanged("Measure");
        }
    }

    public void Incisioning()
    {
        if (IncisionButton.colors.normalColor == new Color32(176, 48, 48, 255))
        {
            IncisionButton.image.sprite = incisionImage[0];
            EventManager.Instance.Events.InvokeModeChanged("Exit");
        }
        else
        {
            ResetButton();
            IncisionButton.image.sprite = incisionImage[1];
            EventManager.Instance.Events.InvokeModeChanged("Exit");
            Events_OnModeChanged("Incision");
        }
    }

}
