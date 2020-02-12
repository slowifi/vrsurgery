using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjManager : Singleton<ObjManager>
{
    public Mesh mesh { get; set; }
    public Transform objTransform;
    private new void Awake()
    {
        mesh = GameObject.Find("heart_2").GetComponent<MeshFilter>().mesh;
        objTransform = GameObject.Find("heart_2").transform;
    }
}
