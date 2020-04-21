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

    public void ResetButton()
    {
        ColorBlock colorTemp = CutButton.colors;
        colorTemp.normalColor = new Color32(137, 96, 96, 255);
        SliceButton.colors = colorTemp;
        CutButton.colors = colorTemp;
        PatchButton.colors = colorTemp;
        IncisionButton.colors = colorTemp;
        MeasureButton.colors = colorTemp;
    }

    public void Slicing()
    {
        if (SliceButton.colors.normalColor == new Color32(176, 48, 48, 255))
        {
            ColorBlock colorTemp = SliceButton.colors;
            colorTemp.normalColor = new Color32(137, 96, 96, 255);
            SliceButton.colors = colorTemp;
            MainManager.SendMessage("Exit");
        }
        else
        {
            ColorBlock colorTemp = SliceButton.colors;
            PatchButton.colors = colorTemp;
            MeasureButton.colors = colorTemp;
            IncisionButton.colors = colorTemp;
            CutButton.colors = colorTemp;
            colorTemp.normalColor = new Color32(176, 48, 48, 255);
            SliceButton.colors = colorTemp;
            MainManager.SendMessage("Exit");
            MainManager.SendMessage("SliceMode");
        }
    }


    public void Cutting()
    {
        // selected Color32(176, 48, 48, 255);
        // unselected Color32(137, 96, 96, 255);
        if (CutButton.colors.normalColor == new Color32(176, 48, 48, 255))
        {
            ColorBlock colorTemp = CutButton.colors;
            colorTemp.normalColor = new Color32(137, 96, 96, 255);
            CutButton.colors = colorTemp;
            MainManager.SendMessage("Exit");
        }
        else
        {
            ColorBlock colorTemp = CutButton.colors;
            PatchButton.colors = colorTemp;
            MeasureButton.colors = colorTemp;
            IncisionButton.colors = colorTemp;
            SliceButton.colors = colorTemp;
            colorTemp.normalColor = new Color32(176, 48, 48, 255);
            CutButton.colors = colorTemp;
            MainManager.SendMessage("Exit");
            MainManager.SendMessage("CutMode");
        }
    }

    public void Patching()
    {
        if (PatchButton.colors.normalColor == new Color32(176, 48, 48, 255))
        {
            ColorBlock colorTemp = PatchButton.colors;
            colorTemp.normalColor = new Color32(137, 96, 96, 255);
            PatchButton.colors = colorTemp;
            MainManager.SendMessage("Exit");
        }
        else
        {
            ColorBlock colorTemp = PatchButton.colors;
            CutButton.colors = colorTemp;
            MeasureButton.colors = colorTemp;
            IncisionButton.colors = colorTemp;
            SliceButton.colors = colorTemp;
            colorTemp.normalColor = new Color32(176, 48, 48, 255);
            PatchButton.colors = colorTemp;
            MainManager.SendMessage("Exit");
            MainManager.SendMessage("StartPatchMode");
        }
    }

    public void Measuring()
    {
        if (MeasureButton.colors.normalColor == new Color32(176, 48, 48, 255))
        {
            ColorBlock colorTemp = MeasureButton.colors;
            colorTemp.normalColor = new Color32(137, 96, 96, 255);
            MeasureButton.colors = colorTemp;
            MainManager.SendMessage("Exit");
        }
        else
        {
            ColorBlock colorTemp = MeasureButton.colors;
            PatchButton.colors = colorTemp;
            CutButton.colors = colorTemp;
            IncisionButton.colors = colorTemp;
            SliceButton.colors = colorTemp;
            colorTemp.normalColor = new Color32(176, 48, 48, 255);
            MeasureButton.colors = colorTemp;
            MainManager.SendMessage("Exit");
            MainManager.SendMessage("StartMeasureMode");
        }
    }

    public void Incisioning()
    {
        if (IncisionButton.colors.normalColor == new Color32(176, 48, 48, 255))
        {
            ColorBlock colorTemp = IncisionButton.colors;
            colorTemp.normalColor = new Color32(137, 96, 96, 255);
            IncisionButton.colors = colorTemp;
            MainManager.SendMessage("Exit");
        }
        else
        {
            ColorBlock colorTemp = IncisionButton.colors;
            PatchButton.colors = colorTemp;
            MeasureButton.colors = colorTemp;
            CutButton.colors = colorTemp;
            SliceButton.colors = colorTemp;
            colorTemp.normalColor = new Color32(176, 48, 48, 255);
            IncisionButton.colors = colorTemp;
            MainManager.SendMessage("Exit");
            MainManager.SendMessage("StartIncisionMode");
        }
    }

}
