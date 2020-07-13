using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class VZMManager : Singleton<VZMManager>
{
    public GameObject[] HeartParts;
    public Button[] HeartButtons;

    public GameObject VZMList;
    public GameObject ButtonPrefab;
    public GameObject Content;

    private void Awake()
    {
        VZMList.SetActive(false);
    }

    public Button AddHeartPartsButton(GameObject prefab, string name)
    {
        var Instance = Instantiate(prefab, Content.transform);
        Instance.transform.localPosition = Vector3.zero;
        Instance.transform.localScale = Vector3.one;
        Instance.name = name + " Button";
        Instance.GetComponent<Button>().GetComponentInChildren<Text>().text = name;
        return Instance.GetComponent<Button>();
    }

    public void AddOnOffAllPartsButton(GameObject prefab)
    {
        var activeInstance = Instantiate(prefab, Content.transform);
        var deactiveInstance = Instantiate(prefab, Content.transform);
        activeInstance.name = "Active Button";
        deactiveInstance.name = "Deactive Button";
        activeInstance.transform.localPosition = Vector3.zero;
        activeInstance.transform.localScale = Vector3.one;
        deactiveInstance.transform.localPosition = Vector3.zero;
        deactiveInstance.transform.localScale = Vector3.one;

        activeInstance.GetComponent<Button>().GetComponentInChildren<Text>().text = "Active All";
        deactiveInstance.GetComponent<Button>().GetComponentInChildren<Text>().text = "Deactive All";
    }

    public void SetMember()
    {
        HeartParts = MultiMeshManager.Instance.HeartParts;
        int size = HeartParts.Length;
        HeartButtons = new Button[size];
        for (int i = 0; i < size; i++)
        {
            HeartButtons[i] = AddHeartPartsButton(ButtonPrefab, HeartParts[i].transform.parent.name);
        }

        AddOnOffAllPartsButton(ButtonPrefab);
    }

}