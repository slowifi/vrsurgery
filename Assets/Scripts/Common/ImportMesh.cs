using UnityEngine;
using UnityEngine.UI;
using Dummiesman;
using System.IO;
using UnityEngine.WSA;
#if UNITY_ANDROID
using SimpleFileBrowser;
#endif

#if UNITY_STANDALONE_WIN
using Crosstales.FB;
#endif

public class ImportMesh : MonoBehaviour
{
    public GameObject PlayerObject;
    public GameObject MainObject;
    public GameObject HumanHeart;
    public GameObject PivotObject;
    public GameObject ButtonPress;
    public GameObject PartialModel;

    public int Length;

    private bool Active;

    private string[] ObjsPath;

    private string FolderPath;
    

#if UNITY_STANDALONE_WIN
    public void FilesBrowsing()
    {
        int tempCount = 0;
        Active = PlayerObject.activeSelf;
        
        // 이부분 필요한지 체크하기//////
        PlayerObject.SetActive(false);
        ///////////////////////////////

        FolderPath = FileBrowser.OpenSingleFolder();
        if (FolderPath == "")
            return;

        var info = new DirectoryInfo(FolderPath);
        var fileInfo = info.GetFiles();
        ObjsPath = new string[fileInfo.Length];

        foreach (var item in fileInfo)
            ObjsPath[tempCount++] = item.FullName;

        Length = ObjsPath.Length;
        MultiMeshManager.Instance.InitObjSize();
        EventManager.Instance.Events.InvokeUIChanged();

        if(ObjsPath[0] == "")
        {
            //playerObject.transform.localScale = Vector3.one setting해줘야함
            //두가지 고려해야함
            // 1. 이미 메쉬가 불려저 있는 경우
            // 2. 메쉬가 불려서 있지 않은 경우
            // 두번째 Import 할때부터 발생하는 문제
            // 첫번째 메쉬 Import후 Heart 확대한 다음
            // 두번째 메쉬 Import 이후부터 PivotTransform Scale이 확대된 상태에서 Scale이 기준이 되어버림
            Debug.Log("아무것도 안나옴");
            ChatManager.Instance.GenerateMessage("아무것도 안나옴");
            PlayerObject.SetActive(Active);
            return;
        }
        else
        {
            SetMeshes(ObjsPath);
        }

        PlayerObject.SetActive(true);
        MainObject.GetComponent<CHD>().AllButtonInteractable();
    }

    public void SetMeshes(string[] Paths)
    {
        string[] FilesName = new string[Length];
        GameObject[] HeartPart = new GameObject[Length];

        for (int i = 0; i < Length; i++)
            FilesName[i] = Path.GetFileNameWithoutExtension(Paths[i]);

        ///////// 이부분도 확인 ////////////////////
        //UIManager.Instance.SetFileName(FilesName);
        ///////////////////////////////////////////
        ///
        if (MainObject.activeSelf)
        {
            DestroyImmediate(PartialModel);
            PartialModel = new GameObject("PartialModel");
            PartialModel.transform.SetParent(HumanHeart.transform);

            PivotObject.transform.localPosition = Vector3.zero;
            PivotObject.transform.localScale = Vector3.one;
            PivotObject.transform.localEulerAngles = Vector3.zero;

            for (int i = 0; i < Length; i++)
            {
                HeartPart[i] = new OBJLoader().Load(Paths[i]);
                HeartPart[i].name = FilesName[i];
                HeartPart[i].transform.SetParent(PartialModel.transform);
                HeartPart[i].transform.localScale = Vector3.one;
            }
            MainObject.SendMessage("ResetMain");
            ButtonPress.SendMessage("ResetButton");
            SetMeshManagerMember(HeartPart);
            return;
        }

        PartialModel = new GameObject("PartialModel");
        PartialModel.transform.SetParent(HumanHeart.transform);

        for (int i=0;i<Length;i++)
        {
            HeartPart[i] = new OBJLoader().Load(Paths[i]);
            HeartPart[i].name = FilesName[i];
            HeartPart[i].transform.SetParent(PartialModel.transform);
            HeartPart[i].transform.localScale = Vector3.one;
        }

        SetMeshManagerMember(HeartPart);
        MainObject.SetActive(true);
    }
    public void SetMeshManagerMember(GameObject[] HeartPart)
    {
        for (int i = 0; i < Length; i++)
        {
            MultiMeshManager.Instance.HeartParts[i] = HeartPart[i].transform.GetChild(0).gameObject;
            MultiMeshManager.Instance.HeartParts[i].transform.localPosition = Vector3.zero;
            MultiMeshManager.Instance.objsTransform[i] = MultiMeshManager.Instance.HeartParts[i].transform;
        } 
    }
#endif
#if UNITY_ANDROID

    IEnumerator ShowLoadDialogCoroutine()
    {
        yield return FileBrowser.WaitForLoadDialog(false, "/storage/emulated/0/hearts", "Load File", "Load");

        if (FileBrowser.Success)
        {
            SetMesh(FileBrowser.Result);
            EventManager.Instance.Events.InvokeUIChanged();
            playerObject.SetActive(true);
        }
        
        EventManager.Instance.Events.InvokeUIFixed();
    }

    public void FileBrowsing()
    {
        FileBrowser.SetFilters(false, new FileBrowser.Filter("obj files", ".obj"));
        EventManager.Instance.Events.InvokeUIFixed();
        playerObject.SetActive(false);
        StartCoroutine(ShowLoadDialogCoroutine());
    }

    public void SetMesh(string path)
    {
        string fileName = Path.GetFileName(path);
        fileName = fileName.Substring(0, fileName.Length - 4);
        UIManager.Instance.SetFileName(fileName);
        
        
        Debug.Log(path);
        
        //var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);
        // 여기에 추가 해야될 것은 새로 읽어들였을 때, 리셋 기능.

        //ObjImporter asdf = new ObjImporter();

        if (mainObject.activeSelf)
        {
            pivotObject.transform.localPosition = Vector3.zero;
            pivotObject.transform.localScale = Vector3.one;
            pivotObject.transform.localEulerAngles = Vector3.zero;

            Destroy(GameObject.Find("PartialModel"));
            GameObject newLocalHeart = new OBJLoader().Load(path);
            newLocalHeart.name = "PartialModel";
            newLocalHeart.transform.SetParent(GameObject.Find("HumanHeart").transform);
            MeshManager.Instance.Heart = newLocalHeart.transform.GetChild(0).gameObject;
            MeshManager.Instance.Heart.transform.localPosition = Vector3.zero;
            MeshManager.Instance.objTransform = MeshManager.Instance.Heart.transform;
            mainObject.SendMessage("ResetMain");
            buttonPressScript.SendMessage("ResetButton");
            return;
        }

        GameObject newHeart = new OBJLoader().Load(path);

        newHeart.name = "PartialModel";
        ChatManager.Instance.GenerateMessage(newHeart.name);
        newHeart.transform.SetParent(GameObject.Find("HumanHeart").transform);
        newHeart.transform.localScale = Vector3.one;
        MeshManager.Instance.Heart = newHeart.transform.GetChild(0).gameObject;
        MeshManager.Instance.Heart.transform.localPosition = Vector3.zero;
        MeshManager.Instance.objTransform = MeshManager.Instance.Heart.transform;

        mainObject.SetActive(true);
    }
#endif


}
