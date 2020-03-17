using UnityEngine;
using System.Collections;
using System;
using System.IO;
using SFB;
using UnityEditor;

public class ImportMesh : MonoBehaviour
{
    public GameObject mainObject;

    public void SetMesh()
    {
        var extensions = new[] {
            new ExtensionFilter("3D Model Files", "stl", "obj", "fbx" ),
            //new ExtensionFilter("All Files", "*" ),
        };

        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);

        //Debug.Log(Application.dataPath + "/obj/");
        // 아니면 아예 asset으로서 import 시키고 그걸 불러오는식으로 해야될 수 있음.
        //var path = StandaloneFileBrowser.SaveFilePanel(paths[0], Application.dataPath + "/obj/", "test.obj", extensions);
        //AssetDatabase.ImportAsset(paths[0]);

        ObjImporter asdf = new ObjImporter();

        if (paths.Length > 0)
        {
            Debug.Log(paths[0]);
            // 여기에 읽은것을 가져다가 mesh 정보 넣기.

            MeshManager.Instance.heart = new GameObject("OuterSurface", typeof(MeshFilter), typeof(MeshRenderer));

            //이거 다른걸로 대체 해야됨.
            //MakeDoubleFaceMesh.Instance.testObject.GetComponent<MeshRenderer>().material = MeshManager.Instance.heart.GetComponent<MeshRenderer>().material;

            MeshManager.Instance.heart.transform.SetParent(GameObject.Find("PartialModel").transform);
            MeshManager.Instance.heart.transform.localPosition = Vector3.zero;
            Mesh importNewMesh = MeshManager.Instance.heart.GetComponent<MeshFilter>().mesh;

            Mesh importMesh = asdf.ImportFile(paths[0]);

            //Debug.Log(importMesh.triangles[20000]);
         
            //뭔가 구조가 잘못되었다.
            int[] newTriangles = (int[])importMesh.triangles.Clone();
            Vector3[] newVertices = (Vector3[])importMesh.vertices.Clone();

            importNewMesh.vertices = newVertices;
            importNewMesh.triangles = newTriangles;

            importNewMesh.RecalculateNormals();

            //MeshManager.Instance.heart = MakeDoubleFaceMesh.Instance.testObject;
            mainObject.SetActive(true);
        }
    }




}
