using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BFS
{

    /// <summary>
    /// 여기도 수정을 해야됨.
    /// 
    /// </summary>
    /// <param name="vertex"></param>
    /// <param name="boundaryList"></param>
    /// <returns></returns>


    public static bool Boundary(int vertex, List<int> boundaryList)
    {
        // start point부터 end point까지 겹치는 point들 전부가 boundary list에 들어가야됨.
        // 일단 incision에서 사용한 기능부터 만들어 보고
        // 여기는 그냥 BFS만 구현 하면됨.
        Queue<int> temp = new Queue<int>();
        HashSet<int> duplicateCheck = new HashSet<int>();

        //AdjacencyList.Instance.ListUpdate();
        //List<Vector3> worldVertices = AdjacencyList.Instance.worldPositionVertices;
        temp.Enqueue(vertex);
        duplicateCheck.Add(vertex);
        Dictionary<int, HashSet<int>> connectedVertices = AdjacencyList.Instance.connectedVertices;
        Dictionary<int, HashSet<int>> connectedTriangles = AdjacencyList.Instance.connectedTriangles;
        HashSet<int> removeTrianglesSet = new HashSet<int>();
        foreach (int item in boundaryList)
        {
            duplicateCheck.Add(item);

        }
        foreach (int item2 in connectedTriangles[vertex])
            removeTrianglesSet.Add(item2);
        int asdf = 0;
        while (temp.Count != 0)
        {
            asdf++;
            if (asdf == 3000)
            {
                //ChatManager.Instance.GenerateMessage(" 자를 수 있는 영역이 아닙니다.");
                //Debug.Break();
                //MeshManager.Instance.LoadOldMesh();
                return false;
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

        int[] triangles = MeshManager.Instance.mesh.triangles;

        int[] newTriangles = new int[triangles.Length - removeTrianglesList.Count * 3];

        int removeCount = 0, newTriangleCount = 0;
        bool checkRemove = true;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            if (checkRemove && removeTrianglesList[removeCount] == i)
            {
                removeCount++;
                if (removeCount == removeTrianglesList.Count)
                    checkRemove = false;
            }
            else
            {
                newTriangles[newTriangleCount++] = triangles[i];
                newTriangles[newTriangleCount++] = triangles[i + 1];
                newTriangles[newTriangleCount++] = triangles[i + 2];
            }
        }

        MeshManager.Instance.mesh.triangles = newTriangles;

        return true;

    }


    /// <summary>
    /// Incision 전용
    /// </summary>
    /// <param name="vertex_num"></param>
    /// <param name="startPoint"></param>
    /// <param name="endPoint"></param>
    /// <param name="isLeft"></param>
    public static List<int> Circle(int vertex_num, Vector3 startPoint, Vector3 endPoint, float zMin, float zMax, int currentIndex, int firstOuterVertexIndex, int lastOuterVertexIndex)
    {
        List<int> result = new List<int>();
        Vector3 center = Vector3.Lerp(startPoint, endPoint, 0.5f);
        float dst = Vector2.Distance(startPoint, endPoint) / 2;
        List<Vector3> worldVertices = AdjacencyList.Instance.worldPositionVertices;

        Debug.Log(currentIndex);
        // vertex_num
        Queue<int> temp = new Queue<int>();
        HashSet<int> duplicateCheck = new HashSet<int>();
        duplicateCheck.Add(vertex_num);
        duplicateCheck.Add(firstOuterVertexIndex);
        duplicateCheck.Add(lastOuterVertexIndex);

        temp.Enqueue(vertex_num);
        int exceptionalErrorCheck = 0;
        while (temp.Count != 0)
        {
            exceptionalErrorCheck++;
            if (exceptionalErrorCheck == 4000)
            {
                ChatManager.Instance.GenerateMessage(" BFS 에러");
                return result;
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
                    //여기서 depth관련된 조건을 하나 넣어주는식으로 해야됨.
                    if (worldVertices[item].z < zMax && worldVertices[item].z > zMin)
                    {
                        temp.Enqueue(item);
                        result.Add(item);

                    }
                }
            }
        }
        return result;
    }


}
