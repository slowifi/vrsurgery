﻿using UnityEngine;
using System.Collections;
using System;
using System.IO;
using UnityEditor;
using Dummiesman;
using SimpleFileBrowser;

public class ImportMesh : MonoBehaviour
{
    public GameObject mainObject;
    public GameObject playerObject;
    public GameObject pivotObject;
    public GameObject buttonPressScript;

    IEnumerator ShowLoadDialogCoroutine()
    {
        // Show a load file dialog and wait for a response from user
        // Load file/folder: file, Initial path: default (Documents), Title: "Load File", submit button text: "Load"
        yield return FileBrowser.WaitForLoadDialog(false, "/storage/emulated/0/hearts", "Load File", "Load");
        
        
        if (FileBrowser.Success)
        {
            SetMesh(FileBrowser.Result);
            playerObject.SetActive(true);
        }
    }

    public void FileBrowsing()
    {
        FileBrowser.SetFilters(false, new FileBrowser.Filter("obj files", ".obj"));
        playerObject.SetActive(false);
        StartCoroutine(ShowLoadDialogCoroutine());
    }

    public void SetMesh(string path)
    {
        //var extensions = new[] {
        //    new ExtensionFilter("3D Model Files", "stl", "obj", "fbx" ),
        //    //new ExtensionFilter("All Files", "*" ),
        //};

        Debug.Log("불러오는중");
        //var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);
        ChatManager.Instance.GenerateMessage(path);
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
            MeshManager.Instance.heart = newLocalHeart.transform.GetChild(0).gameObject;
            MeshManager.Instance.heart.transform.localPosition = Vector3.zero;
            ObjManager.Instance.objTransform = MeshManager.Instance.heart.transform;
            mainObject.SendMessage("ResetMain");
            buttonPressScript.SendMessage("ResetButton");
            return;
        }
        
        GameObject newHeart = new OBJLoader().Load(path);
        
        newHeart.name = "PartialModel";
        ChatManager.Instance.GenerateMessage(newHeart.name);
        newHeart.transform.SetParent(GameObject.Find("HumanHeart").transform);
        
        MeshManager.Instance.heart = newHeart.transform.GetChild(0).gameObject;
        MeshManager.Instance.heart.transform.localPosition = Vector3.zero;
        ObjManager.Instance.objTransform = MeshManager.Instance.heart.transform;
        
        mainObject.SetActive(true);
    }




}
