using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BFS : Singleton<BFS>
{
    public void BFS_Boundary(int vertex, List<int> boundaryList)
    {
        // start point부터 end point까지 겹치는 point들 전부가 boundary list에 들어가야됨.
        // 일단 incision에서 사용한 기능부터 만들어 보고
        // 여기는 그냥 BFS만 구현 하면됨.
        Queue<int> temp = new Queue<int>();
        HashSet<int> duplicateCheck = new HashSet<int>();
        List<Vector3> worldVertices = AdjacencyList.Instance.worldPositionVertices;
        temp.Enqueue(vertex);
        duplicateCheck.Add(vertex);
        Dictionary<int, HashSet<int>> connectedVertices = AdjacencyList.Instance.connectedVertices;
        Dictionary<int, HashSet<int>> connectedTriangles = AdjacencyList.Instance.connectedTriangles;
        HashSet<int> removeTrianglesSet = new HashSet<int>();
        foreach (int item in boundaryList)
        {
            duplicateCheck.Add(item);

            //GameObject v_test = new GameObject();
            //v_test = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //v_test.transform.position = worldVertices[item];
        }
        foreach (int item2 in connectedTriangles[vertex])
            removeTrianglesSet.Add(item2);
        int asdf = 0;
        while (temp.Count != 0)
        {
            asdf++;
            if (asdf == 2000)
            {
                ChatManager.Instance.GenerateMessage(" 자를 수 있는 영역이 아닙니다.");
                MeshManager.Instance.LoadOldMesh();
                return;
            }
            foreach (int item in connectedVertices[temp.Dequeue()])
            {
                //Debug.Log(item);
                bool temp_check = false;
                foreach (int check in duplicateCheck)
                {
                    if (check == item)
                    {
                        temp_check = true;
                        break;
                    }
                }
                if (temp_check)
                    continue;
                foreach (int item2 in connectedTriangles[item])
                    removeTrianglesSet.Add(item2);
                duplicateCheck.Add(item);
                temp.Enqueue(item);
            }
        }

        List<int> removeTrianglesList = removeTrianglesSet.ToList();
        removeTrianglesList.Sort();
        // triangles를 지울 수 있는 걸 만들어야됨.
        int[] triangles = MeshManager.Instance.mesh.triangles;
        int[] newTriangles = new int[triangles.Length - removeTrianglesList.Count * 3];
        int removeCount = 0, newTriangleCount = 0;
        for (int i = 0; i < triangles.Length; i+=3)
        {
            if(removeTrianglesList[removeCount] == i)
            {
                removeCount++;
                if (removeCount == removeTrianglesList.Count)
                    removeCount = 0;
            }
            else
            {
                newTriangles[newTriangleCount++] = triangles[i];
                newTriangles[newTriangleCount++] = triangles[i+1];
                newTriangles[newTriangleCount++] = triangles[i+2];
            }
        }

        MeshManager.Instance.mesh.triangles = newTriangles;

        return;
        
    }
    

    public void BFS_Circle(int vertex_num, Vector3 startPoint, Vector3 endPoint, bool isLeft)
    {
        Vector3 center = Vector3.Lerp(startPoint, endPoint, 0.5f);
        float dst = Vector2.Distance(startPoint, endPoint) / 2;
        List<Vector3> worldVertices = AdjacencyList.Instance.worldPositionVertices;
        int currentIndex = IncisionManager.Instance.currentIndex;
        Debug.Log(currentIndex);
        // vertex_num
        Queue<int> temp = new Queue<int>();
        HashSet<int> duplicateCheck = new HashSet<int>();
        duplicateCheck.Add(vertex_num);
        //duplicateCheck.Add(IncisionManager.Instance.firstInnerVertexIndex);
        duplicateCheck.Add(IncisionManager.Instance.firstOuterVertexIndex);
        //duplicateCheck.Add(IncisionManager.Instance.lastInnerVertexIndex);
        duplicateCheck.Add(IncisionManager.Instance.lastOuterVertexIndex);
        //시작점과 끝점을 하나씩 넣어주는게 좋음.
        //duplicateCheck.Add()

        //if (_left)
        //{
        //    IncisionManager.Instance.leftSide.Add(vertex_num);
        //    foreach (int item in IncisionManager.Instance.leftSide)
        //        duplicateCheck.Add(item);
        //}
        //else
        //{
        //    IncisionManager.Instance.rightSide.Add(vertex_num);
        //    foreach (int item in IncisionManager.Instance.rightSide)
        //        duplicateCheck.Add(item);
        //}

        temp.Enqueue(vertex_num);
        // outerVertices.Add(vertex_num);
        //GameObject v_t = new GameObject();
        //v_t = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        //v_t.transform.position = worldVertices[vertex_num];
        int exceptionalErrorCheck = 0;
        while (temp.Count != 0)
        {
            exceptionalErrorCheck++;
            if(exceptionalErrorCheck==4000)
            {
                //여기에 다른 초기화 함수들 다시 넣고 다 해야됨.
            }
            foreach (int item in AdjacencyList.Instance.connectedVertices[temp.Dequeue()])
            {
                bool temp_check = false;
                // vtx에서 다른거로 어떻게 넘어갈까 흠..
                foreach (int check in duplicateCheck)
                {
                    if (check == item)
                    {
                        temp_check = true;
                        break;
                    }
                }
                if (temp_check)
                    continue;
                // 원에 포함된다면
                // perpendicular vector를 추가로 더해줘서 안쪽에 포함 안되도록

                if (Vector2.Distance(center, worldVertices[item]) < dst)
                {
                    duplicateCheck.Add(item);
                    temp.Enqueue(item);
                    if(isLeft)
                        IncisionManager.Instance.leftSide[currentIndex].Add(item);
                    else
                        IncisionManager.Instance.rightSide[currentIndex].Add(item);
                    //if (AlgorithmsManager.Instance.isLeft(startPoint, endPoint, worldVertices[item]))
                    //{
                    //    if (_left)
                    //    {
                    //        duplicateCheck.Add(item);
                    //        temp.Enqueue(item);
                    //        IncisionManager.Instance.leftSide.Add(item);

                    //        GameObject v_test = new GameObject();
                    //        v_test = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    //        v_test.transform.position = worldVertices[item];
                    //    }
                    //    else
                    //        continue;
                    //}
                    //else
                    //{
                    //    if (!_left)
                    //    {
                    //        duplicateCheck.Add(item);
                    //        temp.Enqueue(item);
                    //        IncisionManager.Instance.rightSide.Add(item);
                    //        GameObject v_test = new GameObject();
                    //        v_test = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    //        v_test.transform.position = worldVertices[item];
                    //    }
                    //    else
                    //        continue;
                    //}
                }
            }
        }
    }
    

}
