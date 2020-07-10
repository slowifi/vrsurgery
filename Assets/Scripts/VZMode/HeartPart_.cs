using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HeartPart_ : MonoBehaviour
{
    private Color unselectedColor = new Color(71/255f, 71 / 255f, 71 / 255f);
    private Color selectedColor = new Color(1, 50 / 255f, 50 / 255f);

    public void OnClick()
    {
        string objectName = this.GetComponentInChildren<Text>().text;
        for (int i = 0; i < MultiMeshManager.Instance.Size; i++)
        {
            if (objectName == MultiMeshManager.Instance.HeartParts[i].name)
            {
                if(MultiMeshManager.Instance.HeartParts[i].activeSelf)
                {
                    ColorBlock tempColorBlock = this.GetComponent<Button>().colors;
                    tempColorBlock.normalColor = selectedColor;
                    tempColorBlock.selectedColor = selectedColor;
                    this.GetComponent<Button>().colors = tempColorBlock;
                }
                else
                {
                    ColorBlock tempColorBlock = this.GetComponent<Button>().colors;
                    tempColorBlock.normalColor = unselectedColor;
                    tempColorBlock.selectedColor = unselectedColor;
                    this.GetComponent<Button>().colors = tempColorBlock;
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
            }
        }
        else if (objectName == "DeActive All")
        {
            for (int i = 0; i < MultiMeshManager.Instance.Size; i++)
                MultiMeshManager.Instance.HeartParts[i].SetActive(false);
        }
    }
}
