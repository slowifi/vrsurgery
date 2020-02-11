using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BFS : MonoBehaviour
{
    private void BFS_Boundary()
    {
        // start point부터 end point까지 겹치는 point들 전부가 boundary list에 들어가야됨.
        // 일단 incision에서 사용한 기능부터 만들어 보고
        // 여기는 그냥 BFS만 구현 하면됨.
        Queue<int> temp = new Queue<int>();
        HashSet<int> duplicateCheck = new HashSet<int>();
        // temp.Enqueue(vertex_num);
        /*
        foreach (int item in collection)
        {
            duplicateCheck.Add(item);
        }
        
        while (temp.Count != 0)
        {
            foreach (int item in connectedVertices[temp.Dequeue()])
            {
                Debug.Log(item);
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

                duplicateCheck.Add(item);
                temp.Enqueue(item);
            }
        }
        return;
        */
    }
    

    
    private void BFS_Circle(int vertex_num, Vector3 startPoint, Vector3 endPoint, bool _left)
    {
        Vector3 center = Vector3.Lerp(startPoint, endPoint, 0.5f);
        float dst = Vector2.Distance(startPoint, endPoint) / 2;
        Vector3[] worldVertices = Initialize.Instance.worldPositionVertices;
        // vertex_num
        Queue<int> temp = new Queue<int>();
        HashSet<int> duplicateCheck = new HashSet<int>();
        duplicateCheck.Add(vertex_num);

        if (_left)
        {
            IncisionManager.Instance.leftSide.Add(vertex_num);
            foreach (int item in IncisionManager.Instance.leftSide)
                duplicateCheck.Add(item);
        }
        else
        {
            IncisionManager.Instance.rightSide.Add(vertex_num);
            foreach (int item in IncisionManager.Instance.rightSide)
                duplicateCheck.Add(item);
        }
        Debug.Log(vertex_num);
        temp.Enqueue(vertex_num);
        // outerVertices.Add(vertex_num);
        GameObject v_t = new GameObject();
        v_t = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        v_t.transform.position = worldVertices[vertex_num];

        while (temp.Count != 0)
        {
            foreach (int item in Initialize.Instance.connectedVertices[temp.Dequeue()])
            {
                Debug.Log(item);
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
                    if (AlgorithmsManager.Instance.isLeft(startPoint, endPoint, worldVertices[item]))
                    {
                        if (_left)
                        {
                            duplicateCheck.Add(item);
                            temp.Enqueue(item);
                            IncisionManager.Instance.leftSide.Add(item);

                            GameObject v_test = new GameObject();
                            v_test = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                            v_test.transform.position = worldVertices[item];
                        }
                        else
                            continue;
                    }
                    else
                    {
                        if (!_left)
                        {
                            duplicateCheck.Add(item);
                            temp.Enqueue(item);
                            IncisionManager.Instance.rightSide.Add(item);
                            GameObject v_test = new GameObject();
                            v_test = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            v_test.transform.position = worldVertices[item];
                        }
                        else
                            continue;
                    }
                }
            }
        }
    }
    

}
