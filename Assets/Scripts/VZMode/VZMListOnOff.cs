using UnityEngine;
using UnityEngine.UI;

public class VZMListOnOff : MonoBehaviour
{
    private Color unselectedColor = new Color(71 / 255f, 71 / 255f, 71 / 255f);
    private Color selectedColor = new Color(1, 50 / 255f, 50 / 255f);

    public void OnButtonClicked_ListOnOff()
    {
        if (VZMManager.Instance.VZMList.activeSelf)
        {
            VZMManager.Instance.VZMList.SetActive(false);
        }
        else
        {
            VZMManager.Instance.VZMList.SetActive(true);
        }
    }

    public void OnButtonClicked_Parts()
    {
        string objectName = this.GetComponentInChildren<Text>().text;
        for (int i = 0; i < MultiMeshManager.Instance.Size; i++)
        {
            if (objectName == MultiMeshManager.Instance.HeartParts[i].name)
            {
                if (MultiMeshManager.Instance.HeartParts[i].activeSelf)
                {
                    ColorBlock tempColorBlock = this.GetComponent<Button>().colors;
                    this.GetComponent<Button>().colors = SetSelectedColor(tempColorBlock);
                }
                else
                {
                    ColorBlock tempColorBlock = this.GetComponent<Button>().colors;
                    this.GetComponent<Button>().colors = SetUnselectedColor(tempColorBlock);
                }
                MultiMeshManager.Instance.HeartParts[i].SetActive(!MultiMeshManager.Instance.HeartParts[i].activeSelf);
            }
        }
        //active all, deactive all 일때 버튼 색 조정 아직
        if (objectName == "Active All")
        {
            for (int i = 0; i < MultiMeshManager.Instance.Size; i++)
            {
                MultiMeshManager.Instance.HeartParts[i].SetActive(true);
                ColorBlock tempColorBlock = VZMManager.Instance.HeartButtons[i].GetComponent<Button>().colors;
                VZMManager.Instance.HeartButtons[i].GetComponent<Button>().colors = SetUnselectedColor(tempColorBlock);
            }
        }
        else if (objectName == "Deactive All")
        {
            for (int i = 0; i < MultiMeshManager.Instance.Size; i++)
            {
                MultiMeshManager.Instance.HeartParts[i].SetActive(false);
                ColorBlock tempColorBlock = VZMManager.Instance.HeartButtons[i].GetComponent<Button>().colors;
                VZMManager.Instance.HeartButtons[i].GetComponent<Button>().colors = SetSelectedColor(tempColorBlock);
            }
            //this.GetComponent<Button>().colors = SetSelectedColor(this.GetComponent<Button>().colors);
        }
    }

    private ColorBlock SetSelectedColor(ColorBlock colors)
    {
        ColorBlock tempColorBlock = colors;
        tempColorBlock.normalColor = selectedColor;
        tempColorBlock.selectedColor = selectedColor;
        return tempColorBlock;
    }

    private ColorBlock SetUnselectedColor(ColorBlock colors)
    {
        ColorBlock tempColorBlock = colors;
        tempColorBlock.normalColor = unselectedColor;
        tempColorBlock.selectedColor = unselectedColor;
        return tempColorBlock;
    }
    
}