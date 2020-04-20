using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class SliceMode : Mode
{
    private GameObject leftHeart;
    private GameObject rightHeart;


    private string mode;
    private SliceMethods SliceMethods;

    private void Awake()
    {
        mode = "slice";
        SliceMethods = new SliceMethods();
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
            IntersectedValues values = Intersections.GetIntersectedValues();
            if (values.Intersected)
            {
                SliceMethods.SetIntersectedValues("first", values);
            }

        }
        else if (Input.GetMouseButtonUp(0))
        {
            IntersectedValues values = Intersections.GetIntersectedValues();
            if (values.Intersected)
            {
                SliceMethods.SetIntersectedValues("second", values);

                GameObject[] SliceResult = SliceMethods.Slicing();
                leftHeart = SliceResult[0];
                rightHeart = SliceResult[1];
                mode = "select";
            }
        }
    }

    private void handleSelect()
    {
        if (Input.GetMouseButtonDown(0))
        {
            string selected = SliceMethods.CheckSelected(leftHeart, rightHeart);
            SelectHeart(selected);
            // Destroy(gameObject);
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
            selectedHeart = leftHeart;
            destoryHeart = rightHeart;
        }
        else if (select == "right")
        {
            selectedHeart = rightHeart;
            destoryHeart = leftHeart;
        }

        Destroy(destoryHeart);
        MeshManager.Instance.Heart = selectedHeart;
        MeshManager.Instance.mesh = selectedHeart.GetComponent<MeshFilter>().mesh;
        MakeDoubleFaceMesh.Instance.Reinitialize();
    }

}