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

    private ColorBlock selectedColorBlock;
    private ColorBlock unselectedColorBlock;

    private void Awake()
    {
        EventManager.Instance.Events.OnModeChanged += Events_OnModeChanged;
        selectedColorBlock = SliceButton.colors;
        unselectedColorBlock = SliceButton.colors;
        selectedColorBlock.normalColor = new Color32(176, 48, 48, 255);
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
        ColorBlock colorTemp = unselectedColorBlock;
        SliceButton.colors = colorTemp;
        CutButton.colors = colorTemp;
        PatchButton.colors = colorTemp;
        IncisionButton.colors = colorTemp;
        MeasureButton.colors = colorTemp;
    }

    public void Slicing()
    {
        // 지금 이 지저분한 코드도 수정할 수 있으면 하도록하기.
        if (SliceButton.colors.normalColor == new Color32(176, 48, 48, 255))
        {
            SliceButton.colors = unselectedColorBlock;
            EventManager.Instance.Events.InvokeModeChanged("Exit");
        }
        else
        {
            ResetButton();
            SliceButton.colors = selectedColorBlock;
            EventManager.Instance.Events.InvokeModeChanged("Exit");
            Events_OnModeChanged("Slice");
        }
    }


    public void Cutting()
    {
        if (CutButton.colors.normalColor == new Color32(176, 48, 48, 255))
        {
            CutButton.colors = unselectedColorBlock;
            EventManager.Instance.Events.InvokeModeChanged("Exit");
        }
        else
        {
            ResetButton();
            CutButton.colors = selectedColorBlock;
            EventManager.Instance.Events.InvokeModeChanged("Exit");
            Events_OnModeChanged("Cut");
        }
    }

    public void Patching()
    {
        
        if (PatchButton.colors.normalColor == new Color32(176, 48, 48, 255))
        {
            PatchButton.colors = unselectedColorBlock;
            EventManager.Instance.Events.InvokeModeChanged("Exit");
        }
        else
        {
            ResetButton();
            PatchButton.colors = selectedColorBlock;
            EventManager.Instance.Events.InvokeModeChanged("Exit");
            Events_OnModeChanged("Patch");
        }
    }

    public void Measuring()
    {
        if (MeasureButton.colors.normalColor == new Color32(176, 48, 48, 255))
        {
            MeasureButton.colors = unselectedColorBlock;
            EventManager.Instance.Events.InvokeModeChanged("Exit");
        }
        else
        {
            ResetButton();
            MeasureButton.colors = selectedColorBlock;
            EventManager.Instance.Events.InvokeModeChanged("Exit");
            Events_OnModeChanged("Measure");
        }
    }

    public void Incisioning()
    {
        if (IncisionButton.colors.normalColor == new Color32(176, 48, 48, 255))
        {
            IncisionButton.colors = unselectedColorBlock;
            EventManager.Instance.Events.InvokeModeChanged("Exit");
        }
        else
        {
            ResetButton();
            IncisionButton.colors = selectedColorBlock;
            EventManager.Instance.Events.InvokeModeChanged("Exit");
            Events_OnModeChanged("Incision");
        }
    }

}
