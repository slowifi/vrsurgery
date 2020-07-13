using UnityEngine;

public class InitVZMList : MonoBehaviour
{
    [SerializeField]
    public GameObject _HeartPart;
    public Transform _Content;

    public GameObject AddHeartParts(int Num)
    {
        var Instance = Instantiate(_HeartPart);
        Instance.name = "HeartPart" + Num;
        Instance.transform.SetParent(_Content);
        Instance.transform.localPosition = Vector3.zero;
        Instance.transform.localScale = Vector3.one;
        return Instance;
    }
    public void AddOnAllParts()
    {
        var Instance = Instantiate(_HeartPart);
        Instance.name = "Active Button";
        Instance.transform.SetParent(_Content);
        Instance.transform.localPosition = Vector3.zero;
        Instance.transform.localScale = Vector3.one;
    }
    public void AddOffAllParts()
    {
        var Instance = Instantiate(_HeartPart);
        Instance.name = "DeActive Button";
        Instance.transform.SetParent(_Content);
        Instance.transform.localPosition = Vector3.zero;
        Instance.transform.localScale = Vector3.one;
    }
}
