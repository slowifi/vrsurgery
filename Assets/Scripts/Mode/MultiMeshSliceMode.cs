using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiMeshSliceMode : MonoBehaviour
{
    private Ray oldRay;

    private LineRendererManipulate lineRenderer;

    private MultiMeshSliceMethod SliceMethods;

    private GameObject[] LeftHeart;
    private GameObject[] RightHeart;

    private GameObject HitGameObject;

    private string mode;
    private string SelectHeart;

    private int SliceResultIndex = 0;
    private int Size = MultiMeshManager.Instance.Size;

    void Awake()
    {
        Initialize();
    }
    private void Update()
    {
        switch (mode)
        {
            case "slice":
                HandleSlice();
                break;
            case "select":
                HeartSelect();
                break;
        }
    }
    public void Initialize()
    {
        mode = "slice";
        SliceMethods = new MultiMeshSliceMethod();
        LeftHeart = new GameObject[Size];
        RightHeart = new GameObject[Size];
        SliceMethods.Initialize();
    }
    private void HandleSlice()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lineRenderer = new LineRendererManipulate(transform);
            Ray ray = MultiMeshManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
            oldRay = ray;
            SliceMethods.SetIntersectedValue("first", ray);
            EventManager.Instance.Events.InvokeModeManipulate("StopAll");
        }
        else if (Input.GetMouseButtonUp(0))
        {
            EventManager.Instance.Events.InvokeModeManipulate("EndAll");
            Ray ray = MultiMeshManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
            SliceMethods.SetIntersectedValue("second", ray);

            GameObject[] SliceResult = SliceMethods.Slicing();

            for (int i = 0; i < Size; i++)
            {
                LeftHeart[i] = SliceResult[SliceResultIndex++];
                RightHeart[i] = SliceResult[SliceResultIndex++];
            }

            mode = "select";
            Destroy(lineRenderer.lineObject);
        }
        else if (Input.GetMouseButton(0))
        {
            Ray ray = MultiMeshManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
            lineRenderer.SetFixedLineRenderer(oldRay.origin + oldRay.direction * 100f, ray.origin + ray.direction * 100f);
        }
    }
    private void HeartSelect()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray SelectRay = MultiMeshManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit Hitobj;

            if (Physics.Raycast(SelectRay, out Hitobj, 1000f)) // Clicked Object
                HitGameObject = Hitobj.collider.gameObject;
            else // Clicked Empthy place
            {
                for (int i = 0; i < Size; i++)
                {
                    DestroyImmediate(LeftHeart[i]);
                    DestroyImmediate(RightHeart[i]);
                    GameObject.Find("PartialModel").transform.GetChild(i).transform.GetChild(0).gameObject.SetActive(true);
                }
                Debug.Log("빈 공간 입니다.");
                return;
            }

            for (int i = 0; i < Size; i++)
            {
                if (HitGameObject.name == LeftHeart[i].name)
                    SelectHeart = "Left";

                if (HitGameObject.name == RightHeart[i].name)
                    SelectHeart = "Right";
            }
            DestroySelectedHeart(SelectHeart);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            EventManager.Instance.Events.InvokeModeChanged("ResetButton");
            GameObject.Find("Main").GetComponent<CHD>().AllButtonInteractable();
            Destroy(this);
            GameObject.Find("Undo Button").GetComponent<MultiMeshUndoRedo>().SliceSave();
        }
    }
    private void DestroySelectedHeart(string SelectHeart)
    {
        if (SelectHeart == "Left")
        {
            for (int i = 0; i < Size; i++)
            {
                DestroyImmediate(LeftHeart[i]);
                MultiMeshManager.Instance.SetNewObject(RightHeart[i], i);
            }
        }
        else if (SelectHeart == "Right")
        {
            for (int i = 0; i < Size; i++)
            {
                DestroyImmediate(RightHeart[i]);
                MultiMeshManager.Instance.SetNewObject(LeftHeart[i], i);
            }
        }
    }
}
