using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingOnoff : MonoBehaviour
{
    public GameObject Settings;
    private bool fixSettings = false;
    private void Awake()
    {
        EventManager.Instance.Events.OnUIChanged += Events_OnUIChanged;
        EventManager.Instance.Events.OnUIFixed += Events_OnUIFixed;
        Settings.SetActive(false);
    }

    public void OnSettingButtons()
    {
        Settings.SetActive(true);
        fixSettings = true;
    }

    public void Events_OnUIChanged()
    {
        Settings.SetActive(false);
        fixSettings = false;
    }

    public void Events_OnUIFixed()
    {
        Debug.Log(fixSettings);
        fixSettings = fixSettings ? false : true;
        Debug.Log(fixSettings);
    }

    public void Update()
    {
        if(fixSettings && Input.GetMouseButtonDown(0))
        {
            Ray cameraRay = MeshManager.Instance.UICamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(cameraRay, out hit, Mathf.Infinity))
            {
                if (hit.collider.gameObject.layer == 5)
                {
                    return;
                }
            }
            Settings.SetActive(false);
            return;
        }
    }
}
