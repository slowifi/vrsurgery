using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class ButtonPress : MonoBehaviour
{
    public GameObject mainManager;

    public Button cutButton;
    public Button patchButton;
    public Button measureButton;
    public Button incisionButton;

    // event 처리로 바꾸는게 맞음... 기존의 방법대로 가는게 나으려나
    public void Cutting()
    {
        // selected Color32(176, 48, 48, 255);
        // unselected Color32(137, 96, 96, 255);
        if (cutButton.colors.normalColor == new Color32(176, 48, 48, 255))
        {
            ColorBlock colorTemp = cutButton.colors;
            colorTemp.normalColor = new Color32(137, 96, 96, 255);
            cutButton.colors = colorTemp;
            mainManager.SendMessage("Exit");
        }
        else
        {
            ColorBlock colorTemp = cutButton.colors;
            patchButton.colors = colorTemp;
            measureButton.colors = colorTemp;
            incisionButton.colors = colorTemp;
            colorTemp.normalColor = new Color32(176, 48, 48, 255);
            cutButton.colors = colorTemp;
            mainManager.SendMessage("CutMode");
        }
    }

    public void Patching()
    {
        if (patchButton.colors.normalColor == new Color32(176, 48, 48, 255))
        {
            ColorBlock colorTemp = patchButton.colors;
            colorTemp.normalColor = new Color32(137, 96, 96, 255);
            patchButton.colors = colorTemp;
            mainManager.SendMessage("Exit");
        }
        else
        {
            ColorBlock colorTemp = patchButton.colors;
            cutButton.colors = colorTemp;
            measureButton.colors = colorTemp;
            incisionButton.colors = colorTemp;
            colorTemp.normalColor = new Color32(176, 48, 48, 255);
            patchButton.colors = colorTemp;
            mainManager.SendMessage("PatchMode");
        }
    }

    public void Measuring()
    {
        if (measureButton.colors.normalColor == new Color32(176, 48, 48, 255))
        {
            ColorBlock colorTemp = measureButton.colors;
            colorTemp.normalColor = new Color32(137, 96, 96, 255);
            measureButton.colors = colorTemp;
            mainManager.SendMessage("Exit");
        }
        else
        {
            ColorBlock colorTemp = measureButton.colors;
            patchButton.colors = colorTemp;
            cutButton.colors = colorTemp;
            incisionButton.colors = colorTemp;
            colorTemp.normalColor = new Color32(176, 48, 48, 255);
            measureButton.colors = colorTemp;
            mainManager.SendMessage("MeasureMode");
        }
    }

    public void Incisioning()
    {
        if (incisionButton.colors.normalColor == new Color32(176, 48, 48, 255))
        {
            ColorBlock colorTemp = incisionButton.colors;
            colorTemp.normalColor = new Color32(137, 96, 96, 255);
            incisionButton.colors = colorTemp;
            mainManager.SendMessage("Exit");
        }
        else
        {
            ColorBlock colorTemp = incisionButton.colors;
            patchButton.colors = colorTemp;
            measureButton.colors = colorTemp;
            cutButton.colors = colorTemp;
            colorTemp.normalColor = new Color32(176, 48, 48, 255);
            incisionButton.colors = colorTemp;
            mainManager.SendMessage("IncisionMode");
        }
    }

}
