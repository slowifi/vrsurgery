using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChangeByDropDown : MonoBehaviour
{
    public void ChangeScene()
    {
        if(GetComponent<Dropdown>().value==0)
        {
            SceneManager.LoadScene("scene_2019");
        }
        else if (GetComponent<Dropdown>().value == 1)
        {
            SceneManager.LoadScene("scene_2018");
        }
        else if (GetComponent<Dropdown>().value == 2)
        {
            SceneManager.LoadScene("scene_2017");
        }
    }
}
