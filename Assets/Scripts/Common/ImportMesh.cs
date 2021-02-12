using UnityEngine;
using Dummiesman;
using System.IO;
using System;
using System.Collections.Generic;
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
        MultiMeshManager.Instance.InitSize();
        MultiMeshAdjacencyList.Instance.InitSize();
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
            VZMManager.Instance.SetMember();
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
        VZMManager.Instance.SetMember();
    }
    public void SetMeshManagerMember(GameObject[] HeartPart)
    {
        for (int i = 0; i < Length; i++)
        {
            MultiMeshManager.Instance.Parts[i] = HeartPart[i].transform.GetChild(0).gameObject;
            MultiMeshManager.Instance.Parts[i].transform.localPosition = Vector3.zero;
            MultiMeshManager.Instance.Transforms[i] = MultiMeshManager.Instance.Parts[i].transform;
        }
        MultiMeshManager.Instance.MeshUpdate();

        long start = DateTime.Now.Ticks;

        float xMin, yMin, zMin, xMax, yMax, zMax;
        BinaryTree binaryTree = new BinaryTree();
        binaryTree.copyMeshFromOriginal(MultiMeshManager.Instance.Meshes[0]);

        xMin = float.MaxValue; xMax = float.MinValue; yMin = float.MaxValue; yMax = float.MinValue; zMin = float.MaxValue; zMax = float.MinValue;
        Vector3[] savedVertices;
        int length = MultiMeshManager.Instance.Meshes[0].vertices.Length;
        int[] triangles = MultiMeshManager.Instance.Meshes[0].triangles;
        binaryTree.root.vertices = new int[length];
        binaryTree.root.length = length;
        binaryTree.root.faces = new int[triangles.Length / 3];
        for (int i = 0; i < length; i++)
        {
            binaryTree.root.vertices[i] = i;
        }
        //BinaryTree.Initialize();
        savedVertices = MultiMeshManager.Instance.Meshes[0].vertices;
        for (int i = 0; i < binaryTree.root.vertices.Length; i++)
        {
            if (xMin > savedVertices[binaryTree.root.vertices[i]].x) xMin = savedVertices[binaryTree.root.vertices[i]].x;
            if (xMax < savedVertices[binaryTree.root.vertices[i]].x) xMax = savedVertices[binaryTree.root.vertices[i]].x;
            if (yMin > savedVertices[binaryTree.root.vertices[i]].y) yMin = savedVertices[binaryTree.root.vertices[i]].y;
            if (yMax < savedVertices[binaryTree.root.vertices[i]].y) yMax = savedVertices[binaryTree.root.vertices[i]].y; 
            if (zMin > savedVertices[binaryTree.root.vertices[i]].z) zMin = savedVertices[binaryTree.root.vertices[i]].z;
            if (zMax < savedVertices[binaryTree.root.vertices[i]].z) zMax = savedVertices[binaryTree.root.vertices[i]].z;
        }
        binaryTree.root.maxPos.Set(xMax, yMax, zMax);
        binaryTree.root.minPos.Set(xMin, yMin, zMin);
        for (int i = 0; i < triangles.Length/3; i++)
        {
            binaryTree.root.faces[i] = i;
        }

        /*GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = (binaryTree.root.minPos + binaryTree.root.maxPos) / 2;
        cube.GetComponent<Renderer>().material.color = new Color(0.5f, 0, 0.5f, 0.1f);

        cube.transform.localScale = new Vector3(xMax - xMin, yMax - yMin, zMax - zMin);
        cube.GetComponent<Renderer>().material.shader = Shader.Find("Transparent/Diffuse");*/


        binaryTree.root.curDepth = 1;
        binaryTree.root.index = 0;
        BinaryTree.savedNode = binaryTree.root;
        BinaryTree.MakeInitialChild(BinaryTree.savedNode);

        /*Mesh mesh = MultiMeshManager.Instance.Meshes[0];
        for (int i = 0; i < binaryTree.root.vertices.Length/3; i++)
        {
            //Debug.DrawLine(transform.TransformPoint(mesh.vertices[i]), transform.TransformPoint(mesh.vertices[i]) + transform.TransformVector(mesh.normals[i]));
                        
            Debug.Log(i.ToString() + "번째 normal : " + mesh.normals[i]);
            GameObject a = GameObject.CreatePrimitive(PrimitiveType.Sphere);            
            a.transform.position = transform.TransformPoint(mesh.vertices[i]) + mesh.normals[i];
            a.transform.localScale = Vector3.one * 0.05f;
            a.name = "normal" + i.ToString();
        }*/

        long end = DateTime.Now.Ticks;
        Debug.Log("ImportMesh tick timer: " + (double)(end - start) / 10000000.0f);
        HapticGrabber.meshChecker();
    }
}
