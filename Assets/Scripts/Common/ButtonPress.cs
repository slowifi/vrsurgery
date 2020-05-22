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
    public Button IncisionButton;
    public Button MeasureDistanceButton;
    public Button MeasureDiameterButton;

    public Sprite[] cutImage;
    public Sprite[] sliceImage;
    public Sprite[] patchImage;
    public Sprite[] incisionImage;
    public Sprite[] measureDistanceImage;
    public Sprite[] measureDiameterImage;
    public Sprite[] measurePerimeterImage;

    private void Awake()
    {
        EventManager.Instance.Events.OnModeChanged += Events_OnModeChanged;
        cutImage = new Sprite[2];
        sliceImage = new Sprite[2];
        patchImage = new Sprite[2];
        incisionImage = new Sprite[2];
        measureDistanceImage = new Sprite[3];
        measureDiameterImage = new Sprite[3];
        measurePerimeterImage = new Sprite[3];

        cutImage[0] = Resources.Load("UI/Icon/Icon_Lasso_0", typeof(Sprite)) as Sprite;
        cutImage[1] = Resources.Load("UI/Icon/Icon_Lasso_1", typeof(Sprite)) as Sprite;

        sliceImage[0] = Resources.Load("UI/Icon/Icon_Slice_0", typeof(Sprite)) as Sprite;
        sliceImage[1] = Resources.Load("UI/Icon/Icon_Slice_1", typeof(Sprite)) as Sprite;

        patchImage[0] = Resources.Load("UI/Icon/Icon_Patch_0", typeof(Sprite)) as Sprite;
        patchImage[1] = Resources.Load("UI/Icon/Icon_Patch_1", typeof(Sprite)) as Sprite;

        measureDistanceImage[0] = Resources.Load("UI/Icon/Icon_Measure_A_0", typeof(Sprite)) as Sprite;
        measureDistanceImage[1] = Resources.Load("UI/Icon/Icon_Measure_A_1", typeof(Sprite)) as Sprite;
        measureDistanceImage[2] = Resources.Load("UI/Icon/Icon_Measure_A_2", typeof(Sprite)) as Sprite;

        measureDiameterImage[0] = Resources.Load("UI/Icon/Icon_Measure_B_0", typeof(Sprite)) as Sprite;
        measureDiameterImage[1] = Resources.Load("UI/Icon/Icon_Measure_B_1", typeof(Sprite)) as Sprite;
        measureDiameterImage[1] = Resources.Load("UI/Icon/Icon_Measure_B_2", typeof(Sprite)) as Sprite;

        measurePerimeterImage[0] = Resources.Load("UI/Icon/Icon_Measure_C_0", typeof(Sprite)) as Sprite;
        measurePerimeterImage[1] = Resources.Load("UI/Icon/Icon_Measure_C_1", typeof(Sprite)) as Sprite;
        measurePerimeterImage[1] = Resources.Load("UI/Icon/Icon_Measure_C_2", typeof(Sprite)) as Sprite;

        incisionImage[0] = Resources.Load("UI/Icon/Icon_Incision_0", typeof(Sprite)) as Sprite;
        incisionImage[1] = Resources.Load("UI/Icon/Icon_Incision_1", typeof(Sprite)) as Sprite;

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
            case "MeasureDiameter":
                EventManager.Instance.Events.InvokeModeChanged("MeasureDiameterMode");
                Debug.Log("Measure diameter 실행");
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
        MeasureDistanceButton.image.sprite = measureDistanceImage[0];
        MeasureDiameterButton.image.sprite = measureDiameterImage[0];
    }

    public void Slicing()
    {
        // 지금 이 지저분한 코드도 수정할 수 있으면 하도록하기.
        if (SliceButton.image.sprite == sliceImage[1])
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
        if (CutButton.image.sprite == cutImage[1])
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
        
        if (PatchButton.image.sprite == patchImage[1])
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
        if (MeasureDistanceButton.image.sprite == measureDistanceImage[1])
        {
            MeasureDistanceButton.image.sprite = measureDistanceImage[0];
            EventManager.Instance.Events.InvokeModeChanged("Exit");
        }
        else
        {
            ResetButton();
            MeasureDistanceButton.image.sprite = measureDistanceImage[1];
            EventManager.Instance.Events.InvokeModeChanged("Exit");
            Events_OnModeChanged("Measure");
        }
    }

    public void MeasureDiameter()
    {
        if (MeasureDiameterButton.image.sprite == measureDiameterImage[1])
        {
            MeasureDiameterButton.image.sprite = measureDiameterImage[0];
            EventManager.Instance.Events.InvokeModeChanged("Exit");
        }
        else
        {
            ResetButton();
            MeasureDiameterButton.image.sprite = measureDiameterImage[1];
            EventManager.Instance.Events.InvokeModeChanged("Exit");
            Events_OnModeChanged("MeasureDiameter");
        }
    }

    public void Incisioning()
    {
        if (IncisionButton.image.sprite == incisionImage[1])
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
