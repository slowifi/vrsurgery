using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class IncisionMethods
{

    public void OnMouseDown()
    {
        IncisionManager.Instance.IncisionUpdate();
        AdjacencyList.Instance.ListUpdate();
        IncisionManager.Instance.SetStartVerticesDF();
    }
}