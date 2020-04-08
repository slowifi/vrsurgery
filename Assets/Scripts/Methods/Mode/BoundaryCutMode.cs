using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BoundaryCutMode : Singleton<BoundaryCutMode>
{
  private bool isLastBoundaryCut = false;
  private int boundaryCount = 0;
  private bool isFirst = true;
  private GameObject lineRenderer;
  public Vector3 firstPosition;
  public Vector3 oldPosition;
  public void SetIsLastBoundaryCut(bool flag)
  {
    isLastBoundaryCut = flag;
  }
  public bool OnUpdate()
  {
    // 조건을 잘 짜야됨.
    if (isFirst)
    {
      Debug.Log("Boundary cut 실행");
      //playerObject.SetActive(false);
      MeshManager.Instance.SaveCurrentMesh();
      AdjacencyList.Instance.ListUpdate();
      isFirst = false;
      boundaryCount = 0;
      return false;
    }

    if (isLastBoundaryCut)
    {
      bool checkError = true;
      // 이걸 뒤에 넣어서 한프레임 늦게 실행 되도록 하기.
      checkError = BoundaryCutManager.Instance.PostProcess();
      if (!checkError)
      {
        Destroy(lineRenderer);
        // ButtonOff();
        return true;
      }
      MeshManager.Instance.mesh.RecalculateNormals();

      Destroy(lineRenderer);
      AdjacencyList.Instance.ListUpdate();
      if (!BoundaryCutManager.Instance.AutomaticallyRemoveTriangles())
      {
        ChatManager.Instance.GenerateMessage(" 영역이 잘못 지정되었습니다.");
        MeshManager.Instance.LoadOldMesh();
      }
      else
        MeshManager.Instance.SaveCurrentMesh();
      AdjacencyList.Instance.ListUpdate();
      MakeDoubleFaceMesh.Instance.MeshUpdateInnerFaceVertices();
      BoundaryCutManager.Instance.BoundaryCutUpdate();
      // ButtonOff();
      return true;
    }

    if (Input.GetMouseButtonDown(0))
    {
      Debug.Log("실행");
      //test();
      //AdjacencyList.Instance.ListUpdate();
      boundaryCount = 0;
      oldPosition = Vector3.zero;
      Ray ray = ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition);

      Vector3 startVertexPosition = Vector3.zero;
      int startTriangleIndex = -1;
      if (IntersectionManager.Instance.RayObjectIntersection(ray, ref startVertexPosition, ref startTriangleIndex))
      {
        BoundaryCutManager.Instance.rays.Add(ray);
        BoundaryCutManager.Instance.intersectedPosition.Add(startVertexPosition);
        BoundaryCutManager.Instance.startTriangleIndex = startTriangleIndex;

        oldPosition = startVertexPosition;
        firstPosition = oldPosition;
        boundaryCount++;
      }
      else
      {
        ChatManager.Instance.GenerateMessage("intersect 되지 않음.");
      }
    }
    else if (Input.GetMouseButton(0))
    {
      Vector3 currentPosition = Vector3.zero;
      if (IntersectionManager.Instance.RayObjectIntersection(ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition), ref currentPosition))
      {
        Debug.Log(boundaryCount);
        if (boundaryCount > 3 && Vector3.Distance(currentPosition, firstPosition) < 2f * ObjManager.Instance.pivotTransform.lossyScale.z)
        {

          var line = lineRenderer.GetComponent<LineRenderer>();
          line.positionCount++;
          //line.positionCount++;

          line.SetPosition(boundaryCount - 1, oldPosition);
          line.SetPosition(boundaryCount, firstPosition);
          line.GetComponent<LineRenderer>().material.color = Color.blue;
          //EditorApplication.isPaused = true;
          ChatManager.Instance.GenerateMessage(" 작업이 진행중입니다아122.");
          isLastBoundaryCut = true;
        }
        else if (Vector3.Distance(currentPosition, oldPosition) < 1.5f * ObjManager.Instance.pivotTransform.lossyScale.z)
        {
          if (oldPosition == Vector3.zero)
            return false;
          if (lineRenderer)
          {
            var line = lineRenderer.GetComponent<LineRenderer>();
            line.SetPosition(boundaryCount - 1, currentPosition);
          }

          return false;
        }
        else if (boundaryCount == 1)
        {
          BoundaryCutManager.Instance.rays.Add(ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition));
          BoundaryCutManager.Instance.intersectedPosition.Add(currentPosition);
          lineRenderer = new GameObject("Boundary Line", typeof(LineRenderer));
          lineRenderer.layer = 8;
          var line = lineRenderer.GetComponent<LineRenderer>();
          line.numCornerVertices = 45;
          line.material.color = Color.black;
          //var line = lineRenderer.GetComponent<LineRenderer>();

          line.SetPosition(0, oldPosition);
          line.SetPosition(boundaryCount++, currentPosition);

          oldPosition = currentPosition;
        }
        else
        {
          if (boundaryCount == 0)
            return false;
          var line = lineRenderer.GetComponent<LineRenderer>();
          line.positionCount++;
          line.SetPosition(boundaryCount++, currentPosition);

          BoundaryCutManager.Instance.rays.Add(ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition));
          BoundaryCutManager.Instance.intersectedPosition.Add(currentPosition);

          oldPosition = currentPosition;
          //boundaryCount++;
        }
      }
      else
      {
        if (boundaryCount == 0)
          return false;
        Destroy(lineRenderer);
        BoundaryCutManager.Instance.BoundaryCutUpdate();
        ChatManager.Instance.GenerateMessage(" 심장이 아닙니다.");
        // ButtonOff();
        return true;
      }
    }
    else if (Input.GetMouseButtonUp(0))
    {
      if (boundaryCount == 0)
        return false;
      if (IntersectionManager.Instance.RayObjectIntersection(ObjManager.Instance.cam.ScreenPointToRay(Input.mousePosition)))
      {
        var line = lineRenderer.GetComponent<LineRenderer>();
        line.positionCount++;
        line.material.color = Color.blue;
        line.SetPosition(boundaryCount - 1, oldPosition);
        line.SetPosition(boundaryCount, firstPosition);
        //EditorApplication.isPaused = true;
        ChatManager.Instance.GenerateMessage(" 작업이 진행중입니다.");

        isLastBoundaryCut = true;
      }
    }
    return false;
  }
}