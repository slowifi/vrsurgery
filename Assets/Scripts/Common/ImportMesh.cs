using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Dummiesman;
using System.IO;
#if UNITY_ANDROID
using SimpleFileBrowser;
#endif

#if UNITY_STANDALONE_WIN
using Crosstales.FB;
#endif

public class ImportMesh : MonoBehaviour
{
    public GameObject mainObject;
    public GameObject playerObject;
    public GameObject pivotObject;
    public GameObject buttonPressScript;

#if UNITY_STANDALONE_WIN
    
    public void FileBrowsing()
    {
        bool active = playerObject.activeSelf;
        playerObject.SetActive(false);
        string objpath = FileBrowser.OpenSingleFile("obj");
        EventManager.Instance.Events.InvokeUIChanged();
        if (objpath == "")
        {
            Debug.Log("아무것도 안나옴");
            playerObject.SetActive(active);
            return;
        }
        else
        {
            SetMesh(objpath);
        }
        
        playerObject.SetActive(true);

        GameObject.Find("Undo Button").GetComponent<Undo_Redo>().Undo_Redo_init();
        GameObject.Find("Incision Button").GetComponent<Button>().interactable = true;
        GameObject.Find("Slicing Button").GetComponent<Button>().interactable = true;
        GameObject.Find("Patching Button").GetComponent<Button>().interactable = true;
        GameObject.Find("Cutting Button").GetComponent<Button>().interactable = true;
        GameObject.Find("Undo Button").GetComponent<Button>().interactable = true;
        GameObject.Find("Redo Button").GetComponent<Button>().interactable = true;
        GameObject.Find("Extended Measure Distance Button").GetComponent<Button>().interactable = true;
        //GameObject.Find("Measure Diameter Button").GetComponent<Button>().interactable = true;
    }

    public void SetMesh(string path)
    {

        Debug.Log(path);
        //var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);
        string fileName = Path.GetFileName(path);
        fileName = fileName.Substring(0, fileName.Length - 4);
        UIManager.Instance.SetFileName(fileName);
        // 여기에 추가 해야될 것은 새로 읽어드렸을 때, 리셋 기능.

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
            newLocalHeart.transform.localScale = Vector3.one;
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
