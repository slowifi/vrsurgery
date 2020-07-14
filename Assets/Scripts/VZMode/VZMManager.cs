using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class VZMManager : Singleton<VZMManager>
{
    public GameObject[] HeartButtons;

    public GameObject VZMList;
    public GameObject ButtonPrefab;
    public GameObject Content;

    private void Awake()
    {
        VZMList.SetActive(false);
    }

    private GameObject AddHeartPartsButton(GameObject prefab, string name)
    {
        var Instance = Instantiate(prefab, Content.transform);
        Instance.transform.localPosition = Vector3.zero;
        Instance.transform.localScale = Vector3.one;
        Instance.name = name + " Button";
        Instance.GetComponent<Button>().GetComponentInChildren<Text>().text = name;
        return Instance;
    }

    private GameObject AddOnOffAllPartsButton(GameObject prefab, bool isActive)
    {
        var Instance = Instantiate(prefab, Content.transform);
        if (isActive)
        {
            Instance.name = "Active Button";
            Instance.GetComponent<Button>().GetComponentInChildren<Text>().text = "Active All";
            
        }
        else
        {
            Instance.name = "Deactive Button";
            Instance.GetComponent<Button>().GetComponentInChildren<Text>().text = "Deactive All";
        }
        
        Instance.transform.localPosition = Vector3.zero;
        Instance.transform.localScale = Vector3.one;
        return Instance;
        
    }

    public void SetMember()
    {
        RemoveMembers();
        GameObject[] HeartParts = MultiMeshManager.Instance.Parts;
        int size = HeartParts.Length;
        HeartButtons = new GameObject[size+2];
        for (int i = 0; i < size; i++)
        {
            HeartButtons[i] = AddHeartPartsButton(ButtonPrefab, HeartParts[i].transform.parent.name);
        }

        HeartButtons[size] = AddOnOffAllPartsButton(ButtonPrefab, true);
        HeartButtons[size+1] = AddOnOffAllPartsButton(ButtonPrefab, false);
    }

    private void RemoveMembers()
    {
        for (int i = 0; i < HeartButtons.Length; i++)
        {
            Destroy(HeartButtons[i]);
        }
        
    }


}