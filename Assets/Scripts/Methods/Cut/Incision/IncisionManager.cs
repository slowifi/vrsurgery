using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncisionManager : Singleton<IncisionManager>
{
    public List<int> leftSide { get; set; }
    public List<int> rightSide { get; set; }

    public void InitializeIncision()
    {
        leftSide = new List<int>();
        rightSide = new List<int>();
    }
}
