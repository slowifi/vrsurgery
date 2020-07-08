using Boo.Lang;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public class VZMListOnOff : MonoBehaviour
{
    private GameObject test;
    public GameObject Content;
    public GameObject List;
    public GameObject ContentParent;
    public GameObject[] HeartPart;
    public Button[] PartButton;
    public int State = 0;
    public bool SetOnce = true;
    public bool ClickState = true;
    public int index;
    public bool Once = false;
    public bool end = false;
    public void SetList()
    {
        State++;
    }
    
    public void SetMember()
    {
        int Size = MultiMeshManager.Instance.Size;

        PartButton = new Button[Size];
        HeartPart = new GameObject[Size];

        for (int i = 0; i < Size; i++)
            HeartPart[i] = GameObject.Find("PartialModel").transform.GetChild(i).gameObject;

        for (int i = 0; i < Size; i++)
            GameObject.Find("VZMScroll").GetComponent<InitVZMList>().AddHeartParts(i);

        for (int i = 0; i < Size; i++)
            GameObject.Find("HeartPart" + i.ToString()).transform.GetChild(0).GetComponent<Text>().text = GameObject.Find("PartialModel").transform.GetChild(i).name;

        for (int i = 0; i < Size; i++)
            GameObject.Find("HeartPart" + i.ToString()).name = GameObject.Find("PartialModel").transform.GetChild(i).name + " Button";

        GameObject.Find("VZMScroll").GetComponent<InitVZMList>().AddOnAllParts();
        GameObject.Find("VZMScroll").GetComponent<InitVZMList>().AddOffAllParts();

        GameObject.Find("Active Button").transform.GetChild(0).GetComponent<Text>().text = "Active All";
        GameObject.Find("DeActive Button").transform.GetChild(0).GetComponent<Text>().text = "DeActive All";
    }
    public void SetObject(string name)
    {
        for(int i=0;i<MultiMeshManager.Instance.Size;i++)
        {
            if (name == HeartPart[i].name)
                index = i;
        }

        if (HeartPart[index].activeSelf == true)
            HeartPart[index].SetActive(false);
        else if (HeartPart[index].activeSelf == false)
            HeartPart[index].SetActive(true);
    }

    void Update()
    {
        if (State == 0)
            List.SetActive(false);
        else if (State == 1)
        {
            List.SetActive(true);
            
            if (SetOnce == true)
            {
                if (Content.transform.childCount != 0)
                {
                    for (int i = 0; i < MultiMeshManager.Instance.Size; i++)
                        Destroy(Content.transform.GetChild(i).transform.gameObject);
                }
                SetMember();
                SetOnce = false;
            }

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = GameObject.Find("UICamera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1000f))
                {
                    test = hit.collider.gameObject;
                    Once = true;
                    Debug.Log(test.name);
                }
                else
                    Debug.Log("none");

                if (Once == true)
                {
                    if (test != null)
                    {
                        if (test.name == "Active Button")
                        {
                            for (int i = 0; i < MultiMeshManager.Instance.Size; i++)
                                HeartPart[i].SetActive(true);
                        }
                        else if (test.name == "DeActive Button")
                        {
                            for (int i = 0; i < MultiMeshManager.Instance.Size; i++)
                                HeartPart[i].SetActive(false);
                        }
                        else
                        {
                            SetObject(test.transform.GetChild(0).GetComponent<Text>().text);
                            Once = false;
                        }
                    }
                }
            }
        }
        else
        {
            State = 0;
            if (State == 0)
                end = true;
            if(end == true)
            {
                for (int i = 0; i < MultiMeshManager.Instance.Size; i++)
                    HeartPart[i].SetActive(true);
                end = false;
            }
            SetOnce = true;
        }
    }
}