using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiMeshSliceMode : MonoBehaviour
{
    private Ray oldRay;
    private bool saveonce = true;

    private LineRendererManipulate lineRenderer;
    private string mode;
    private MultiMeshSliceMethod sliceMethods;

    private GameObject[] MultiMeshLeftHeart = new GameObject[MultiMeshManager.Instance.Size];
    private GameObject[] MultiMeshRightHeart = new GameObject[MultiMeshManager.Instance.Size];
    private GameObject HitGameObject;
    private int SliceResultIndex = 0;
    private string SelectHeart;

    private void Awake()
    {
        mode = "slice";
        sliceMethods = new MultiMeshSliceMethod();
    }

    private void Update()
    {
        switch (mode)
        {
            case "slice":
                MultiMeshHandleSlice();
                break;
            case "select":
                MultiMeshHeartPartSelect();
                break;
        }
    }
    private void MultiMeshHandleSlice()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lineRenderer = new LineRendererManipulate(transform);
            Ray ray = MultiMeshManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
            oldRay = ray;
            sliceMethods.SetIntersectedValues("first", ray);
            EventManager.Instance.Events.InvokeModeManipulate("StopAll");
        }
        else if (Input.GetMouseButtonUp(0))
        {
            EventManager.Instance.Events.InvokeModeManipulate("EndAll");
            Ray ray = MultiMeshManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
            sliceMethods.SetIntersectedValues("second", ray);

            GameObject[] SliceResult = sliceMethods.MultiMeshSlicing();

            for (int i = 0; i < MultiMeshManager.Instance.Size; i++)
            {
                MultiMeshLeftHeart[i] = SliceResult[SliceResultIndex++];
                MultiMeshRightHeart[i] = SliceResult[SliceResultIndex++];
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
    private void MultiMeshHeartPartSelect()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray SelectRay = MultiMeshManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit Hitobj;
            if (Physics.Raycast(SelectRay, out Hitobj, 1000f)) // Clicked Object
                HitGameObject = Hitobj.collider.gameObject;
            else // Clicked Empthy place
            {
                for (int i = 0; i < MultiMeshManager.Instance.Size; i++)
                {
                    DestroyImmediate(MultiMeshLeftHeart[i]);
                    DestroyImmediate(MultiMeshRightHeart[i]);
                    GameObject.Find("PartialModel").transform.GetChild(i).transform.GetChild(0).gameObject.SetActive(true);
                }
                Debug.Log("빈 공간 입니다.");
                return;
            }
            Debug.Log(HitGameObject.name);
            for (int i = 0; i < MultiMeshManager.Instance.Size; i++)
            {
                if (HitGameObject.name == MultiMeshLeftHeart[i].name)
                    SelectHeart = "Left";

                if (HitGameObject.name == MultiMeshRightHeart[i].name)
                    SelectHeart = "Right";
            }
            MultiMeshDestroySelectedHeart(SelectHeart);
        }
        else if(Input.GetMouseButtonUp(0))
        {
            //MultiMeshMakeDoubleFace.Instance.Reinitialize();
            Destroy(this);
        }
    }
    private void MultiMeshDestroySelectedHeart(string SelectHeart)
    {
        Debug.Log(SelectHeart);
        if(SelectHeart == "Left")
        {
            for (int i = 0; i < MultiMeshManager.Instance.Size; i++)
            {
                DestroyImmediate(MultiMeshLeftHeart[i]);
                MultiMeshManager.Instance.SetNewObjects(MultiMeshRightHeart[i], i);
            }
        }
        else if (SelectHeart == "Right")
        {
            for (int i = 0; i < MultiMeshManager.Instance.Size; i++)
            {
                DestroyImmediate(MultiMeshRightHeart[i]);
                MultiMeshManager.Instance.SetNewObjects(MultiMeshLeftHeart[i], i);
            }
        }
    }
}
