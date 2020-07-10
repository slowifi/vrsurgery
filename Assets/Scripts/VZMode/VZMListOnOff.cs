using UnityEngine;
using UnityEngine.UI;

public class VZMListOnOff : MonoBehaviour
{
    public GameObject[] HeartPart;

    public GameObject List;
    public GameObject Content;
    public GameObject HitButton;

    public int ButtonState = 0;
    public int index;

    public bool Operate = false;
    public bool ButtonClicked = false;

    void Awake()
    {
        List.SetActive(false);
    }

    void Update()
    {
        if (Operate == true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = GameObject.Find("UICamera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 1000f))
                {
                    HitButton = hit.collider.gameObject;
                    Debug.Log(HitButton);
                    ButtonClicked = true;
                }
            }
        }

        //if (ButtonClicked == true)
        //{
        //    if (HitButton.name == "Active Button")
        //    {
        //        for (int i = 0; i < MultiMeshManager.Instance.Size; i++)
        //            HeartPart[i].SetActive(true);

        //        ButtonClicked = false;
        //    }
        //    else if (HitButton.name == "DeActive Button")
        //    {
        //        for (int i = 0; i < MultiMeshManager.Instance.Size; i++)
        //            HeartPart[i].SetActive(false);

        //        ButtonClicked = false;
        //    }
        //    else
        //    {
        //        if(HitButton.name != "Viewport")
        //            SetObject(HitButton.transform.GetChild(0).GetComponent<Text>().text);

        //        ButtonClicked = false;
        //    }
        //}
    }

    public void SetMember()
    {
        int Size = MultiMeshManager.Instance.Size;

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
    
    

    public void ListOnOff()
    {
        ButtonState++; // Change Button State When Clicked

        if (ButtonState == 0)
        {
            List.SetActive(false);
        }
        else if (ButtonState == 1)
        {
            List.SetActive(true);
            
            if(Content.transform.childCount == 0) // 제일 처음에만 실행함
                SetMember();

            Operate = true;
        }
        else
        {
            Operate = false;
            ButtonState = 0;
            List.SetActive(false);
        }
    }
    
}