using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ButtonListOnOff : MonoBehaviour
{
    public GameObject _List;

    public void ClickEvent()
    {
        if(_List.activeSelf==true)
        {
            var asdf = this.GetComponent<Button>().colors;
            asdf.selectedColor = new Color32(255, 255, 255, 255);
            this.GetComponent<Button>().colors = asdf;
            _List.SetActive(false);
        }
        else
        {
            var asdf = this.GetComponent<Button>().colors;
            asdf.selectedColor = new Color32(110, 110, 110, 255);
            this.GetComponent<Button>().colors = asdf;
            _List.SetActive(true);
        }
    }

    
}