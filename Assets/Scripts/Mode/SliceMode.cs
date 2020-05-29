﻿using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class SliceMode : MonoBehaviour
{
    private Ray oldRay;
    private GameObject leftHeart;
    private GameObject rightHeart;

    private LineRendererManipulate lineRenderer;
    private string mode;
    private SliceMethods sliceMethods;

    private void Awake()
    {
        mode = "slice";
        sliceMethods = new SliceMethods();
    }

    private void Update()
    {
        switch (mode)
        {
            case "slice":
                handleSlice();
                break;
            case "select":
                handleSelect();
                break;
        }
    }

    private void handleSlice()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lineRenderer = new LineRendererManipulate(transform);
            Ray ray = MeshManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
            oldRay = ray;
            sliceMethods.SetIntersectedValues("first", ray);
            EventManager.Instance.Events.InvokeModeManipulate("StopAll");
        }
        else if (Input.GetMouseButtonUp(0))
        {
            EventManager.Instance.Events.InvokeModeManipulate("EndAll");
            Ray ray = MeshManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
            sliceMethods.SetIntersectedValues("second", ray);

            GameObject[] SliceResult = sliceMethods.Slicing();
            leftHeart = SliceResult[0];
            rightHeart = SliceResult[1];
            mode = "select";
            Destroy(lineRenderer.lineObject);
        }
        else if(Input.GetMouseButton(0))
        {
            Ray ray = MeshManager.Instance.cam.ScreenPointToRay(Input.mousePosition);
            lineRenderer.SetFixedLineRenderer(oldRay.origin + oldRay.direction * 100f, ray.origin + ray.direction * 100f);
        }
    }

    private void handleSelect()
    {
        if (Input.GetMouseButtonDown(0))
        {
            string selected = sliceMethods.CheckSelected(leftHeart, rightHeart);
            SelectHeart(selected);
            mode = "slice";
            //EventManager.Instance.Events.InvokeModeChanged("ResetButton");
            Destroy(this);
        }
    }

    private void SelectHeart(string select)
    {
        if (select == "none")
        {
            Destroy(rightHeart);
            Destroy(leftHeart);
            MeshManager.Instance.Heart.SetActive(true);
            return;
        }

        GameObject selectedHeart = null;
        GameObject destoryHeart = null;

        if (select == "left")
        {
            selectedHeart = rightHeart;
            destoryHeart = leftHeart;
        }
        else if (select == "right")
        {
            selectedHeart = leftHeart;
            destoryHeart = rightHeart;
        }
        
        Destroy(destoryHeart);
        MeshManager.Instance.SetNewObject(selectedHeart);
        //MeshManager.Instance.mesh = selectedHeart.GetComponent<MeshFilter>().mesh;
        MakeDoubleFaceMesh.Instance.Reinitialize();
    }

}