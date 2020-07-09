using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Undo_Redo : MonoBehaviour
{
    [SerializeField]
    public List<Mesh> InnerHeartArray = new List<Mesh>();
    public List<Mesh> newHeartArray = new List<Mesh>();
    public List<int> PatchIndexArray = new List<int>();

    public int currentIndex = 0, maxIndex = 0;
    public int PatchIndexCount = 0;
    public int PatchIndex = 0;
    public int First_Button_Called_Value = 1;
    public int Second_Button_Called_Value = 3;
    public int Except_first = 1;
    public int StartPoint;
    public int HeartDeleteRange;
    public int PatchDeleteRange;

    public bool Zero_Index_Set = true;

    public void Undo_Redo_init()
    {
        // Clear InnerHeartArray, newHeartArray List
        // Save the init model mesh file at index 0
        InnerHeartArray.Clear();
        newHeartArray.Clear();
        if (PatchIndexArray.Count != 0)
        {
            for (int i = 0; i < PatchIndexArray.Count - 1; i++)
            {
                Destroy(GameObject.Find("OuterPatch" + i.ToString()));
                Destroy(GameObject.Find("InnerPatch" + i.ToString()));
            }
        }
        PatchIndexArray.Clear();
        currentIndex = 0;
        maxIndex = 0;
    }

    public void SaveMesh()
    {
        if (currentIndex < maxIndex)
        {
            Debug.Log("JKM SaveMesh0");
            StartPoint = currentIndex + 1;
            HeartDeleteRange = maxIndex - currentIndex;
            PatchDeleteRange = (PatchIndexArray.Count - 1) - (PatchIndex - 1);

            InnerHeartArray.RemoveRange(StartPoint, HeartDeleteRange);
            newHeartArray.RemoveRange(StartPoint, HeartDeleteRange);
            Debug.Log("JKM PatchIndex : " + PatchIndex + " PatchIndexArray.Count : " + PatchIndexArray.Count);
            for (int i = PatchIndex; i < PatchIndexArray.Count; i++)
            {
                Debug.Log("JKM i: " + i);
                Destroy(GameObject.Find("OuterPatch" + i.ToString()));
                Destroy(GameObject.Find("InnerPatch" + i.ToString()));
            }
            PatchIndexArray.RemoveRange(PatchIndex, PatchDeleteRange);
            Debug.Log("JKM SaveMesh1");
            GameObject.Find("Main").GetComponent<ModeEvents>().PatchNum -= PatchIndexArray.Count - PatchIndex;
            maxIndex = currentIndex;
        }

        // when click button first time save current model mesh...1
        //if (GameObject.Find("Main").GetComponent<CHD>().Detect_Second_ButtonCall() == First_Button_Called_Value)
        //{
        //    InnerHeartArray.Add(Instantiate(GameObject.Find("Heart_Inner").GetComponent<MeshFilter>().mesh));
        //    newHeartArray.Add(Instantiate(GameObject.Find("COLOR____").GetComponent<MeshFilter>().mesh));
        //}
        //else // when click button second time, Save model mesh that did some action...2
        //{
        //    InnerHeartArray.Add(Instantiate(GameObject.Find("Heart_Inner").GetComponent<MeshFilter>().mesh));
        //    newHeartArray.Add(Instantiate(GameObject.Find("COLOR____").GetComponent<MeshFilter>().mesh));
        //    if (currentIndex > Except_first) // After perfoming the above operation1,2 once, the mesh saved twice, So delete overlapped mesh
        //    {
        //        InnerHeartArray.RemoveAt(currentIndex - Except_first);
        //        newHeartArray.RemoveAt(currentIndex - Except_first);
        //        currentIndex--;
        //    }
        //}
        //GameObject.Find("Main").GetComponent<CHD>().Detect_Second_NumReset();
        currentIndex++;
        maxIndex = currentIndex;

        // when first time, index addded one more time, so set index as zero
        if (Zero_Index_Set == true)
        {
            currentIndex = 0;
            maxIndex = currentIndex;
            Zero_Index_Set = false;
        }
    }
    public void SaveMeshAtPatch()
    {
        InnerHeartArray.RemoveAt(InnerHeartArray.Count - 1);
        newHeartArray.RemoveAt(newHeartArray.Count - 1);
        PatchIndexArray.RemoveAt(PatchIndexArray.Count - 1);
        PatchIndex--;
        currentIndex--;
        maxIndex = currentIndex;
        //GameObject.Find("Main").GetComponent<CHD>().Detect_Second_NumReset();
    }

    public void UndoMesh()
    {
        if (currentIndex <= 0) return;
        if (PatchIndex != 0 && currentIndex == PatchIndexArray.ElementAt(PatchIndex - 1))
        {
            GameObject.Find("OuterPatch" + (PatchIndex - 1).ToString()).transform.position = new Vector3(1500, 0, 0);
            GameObject.Find("InnerPatch" + (PatchIndex - 1).ToString()).transform.position = new Vector3(1500, 0, 0);
            PatchIndex--;
        }
        else
        {
            currentIndex--;
            GameObject.Find("Heart_Inner").GetComponent<MeshFilter>().mesh = InnerHeartArray.ElementAt(currentIndex);
            GameObject.Find("COLOR____").GetComponent<MeshFilter>().mesh = newHeartArray.ElementAt(currentIndex);
        }
    }

    public void RedoMesh()
    {
        if (currentIndex == maxIndex && PatchIndexArray.Count == PatchIndex) return;
        if (PatchIndexArray.Count != 0 && PatchIndex < PatchIndexArray.Count && currentIndex == PatchIndexArray.ElementAt(PatchIndex))
        {
            GameObject.Find("OuterPatch" + PatchIndex.ToString()).transform.position = new Vector3(0, 0, 0);
            GameObject.Find("InnerPatch" + PatchIndex.ToString()).transform.position = new Vector3(0, 0, 0);
            PatchIndex++;
        }
        else
        {
            currentIndex++;
            GameObject.Find("Heart_Inner").GetComponent<MeshFilter>().mesh = InnerHeartArray.ElementAt(currentIndex);
            GameObject.Find("COLOR____").GetComponent<MeshFilter>().mesh = newHeartArray.ElementAt(currentIndex);
        }
    }

    public void SavePatchIndex()
    {
        PatchIndexArray.Add(currentIndex);
        PatchIndex++;
        PatchIndexCount++;
    }

}
