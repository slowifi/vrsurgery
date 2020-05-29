using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Runtime.InteropServices;

public class CGAL
{
    [DllImport("CGALtest_dll", EntryPoint = "CreateMeshObject", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr CreateMeshObject();

    [DllImport("CGALtest_dll", EntryPoint = "BuildPolyhedron", CallingConvention = CallingConvention.Cdecl)]
    public static extern int BuildPolyhedron(IntPtr value, float[] _vertices, int verticesLength, int[] _indices, int indicesLength);

    [DllImport("CGALtest_dll", EntryPoint = "SavePolyhedron", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SavePolyhedron(IntPtr value, string path);

    [DllImport("CGALtest_dll", EntryPoint = "ClipPolyhedronByMesh", CallingConvention = CallingConvention.Cdecl)]
    public static extern int ClipPolyhedronByMesh(IntPtr clippee, IntPtr clipper);

    [DllImport("CGALtest_dll", EntryPoint = "ClipPolyhedron", CallingConvention = CallingConvention.Cdecl)]
    public static extern int ClipPolyhedron(IntPtr value, float[] _vertices, int verticesLength, int[] _indices, int indicesLength);

    [DllImport("CGALtest_dll", EntryPoint = "GetVertices", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr GetVertices(IntPtr value);

    [DllImport("CGALtest_dll", EntryPoint = "GetFaces", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr GetFaces(IntPtr value);

    [DllImport("CGALtest_dll", EntryPoint = "GetNumberOfVertices", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetNumberOfVertices(IntPtr value);

    [DllImport("CGALtest_dll", EntryPoint = "GetNumberOfFaces", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetNumberOfFaces(IntPtr value);

    [DllImport("CGALtest_dll", EntryPoint = "BuildPolyhedronByPath", CallingConvention = CallingConvention.Cdecl)]
    public static extern int BuildPolyhedronByPath(IntPtr value, string path);

    [DllImport("CGALtest_dll", EntryPoint = "ClipPolyhedronByHull", CallingConvention = CallingConvention.Cdecl)]
    public static extern int ClipPolyhedronByHull(IntPtr value, float[] _vertices, int _verticesLength);

    [DllImport("CGALtest_dll", EntryPoint = "ClipPolyhedronByPlane", CallingConvention = CallingConvention.Cdecl)]
    public static extern int ClipPolyhedronByPlane(IntPtr value, float[] _vertices);

    [DllImport("CGALtest_dll", EntryPoint = "PreprocessDeformMeshArray", CallingConvention = CallingConvention.Cdecl)]
    public static extern int PreprocessDeformMesh(IntPtr value, int[] _roi, int roiLength, int[] _cvertices, int _cverticesLength);

    [DllImport("CGALtest_dll", EntryPoint = "PreprocessDeformMeshRadius", CallingConvention = CallingConvention.Cdecl)]
    public static extern int PreprocessDeformMesh(IntPtr value, int centerIndex, float roiRadius, float controlRadius);

    [DllImport("CGALtest_dll", EntryPoint = "DeformMesh", CallingConvention = CallingConvention.Cdecl)]
    public static extern int DeformMesh(IntPtr value, float[] direction);

    [DllImport("CGALtest_dll", EntryPoint = "FillHole", CallingConvention = CallingConvention.Cdecl)]
    public static extern int FillHole(IntPtr value, char option = 'd');

    [DllImport("CGALtest_dll", EntryPoint = "Extrude", CallingConvention = CallingConvention.Cdecl)]
    public static extern int Extrude(IntPtr value, float[] direction);

    [DllImport("CGALtest_dll", EntryPoint = "TriangulateVertices", CallingConvention = CallingConvention.Cdecl)]
    public static extern int TriangulateVertices(IntPtr value, float[] _vertices, int _verticesLength);

    [DllImport("CGALtest_dll", EntryPoint = "GeodesicDistance", CallingConvention = CallingConvention.Cdecl)]
    public static extern float GeodesicDistance(IntPtr value, int v1, int v2);

    //사용시에 첫번째 포인트로 카메라 origin을 넣어줘야함.
    [DllImport("CGALtest_dll", EntryPoint = "Intersection", CallingConvention = CallingConvention.Cdecl)]
    public static extern void Intersection(IntPtr value, float[] plane);

    // clipper가 mesh면 됨.
    [DllImport("CGALtest_dll", EntryPoint = "CorefineByMesh", CallingConvention = CallingConvention.Cdecl)]
    public static extern int CorefineByMesh(IntPtr clippee, IntPtr clipper);

    [DllImport("CGALtest_dll", EntryPoint = "ExtractCircle", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr ExtractCircle(IntPtr value, int v1, int v2, int v3);

    [DllImport("CGALtest_dll", EntryPoint = "ExtractCircleLength", CallingConvention = CallingConvention.Cdecl)]
    public static extern int ExtractCircleLength(IntPtr value);

    [DllImport("CGALtest_dll", EntryPoint = "ExtractCircleFixed", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr ExtractCircleFixed(IntPtr value, int v1, int v2, int v3);

    [DllImport("CGALtest_dll", EntryPoint = "ExtractCircleFixedLength", CallingConvention = CallingConvention.Cdecl)]
    public static extern int ExtractCircleFixedLength(IntPtr value);

    [DllImport("CGALtest_dll", EntryPoint = "VertexNormal", CallingConvention = CallingConvention.Cdecl)]
    public static extern float vertexNormal(IntPtr value, int v);

    [DllImport("CGALtest_dll", EntryPoint = "GetRoiVertices", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr GetRoiVertices(IntPtr value);

    [DllImport("CGALtest_dll", EntryPoint = "GetControlVertices", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr GetControlVertices(IntPtr value);


    public static void GenerateBigTriangle(Vector2 startMousePos, Vector2 endMousePos)
    {
        Vector2 newVec = startMousePos - endMousePos;
        float height = Screen.height;
        float width = Screen.width;
        float newHeight = (height - startMousePos.y) / newVec.y;
        float newWidth = (width - startMousePos.x) / newVec.x;
        float weight = Mathf.Abs(newHeight) > Mathf.Abs(newWidth) ? newWidth : newHeight;

        Vector2 maximumPos = startMousePos + (newVec * weight);

        newHeight = (0 - startMousePos.y) / newVec.y;
        newWidth = (0 - startMousePos.x) / newVec.x;
        weight = Mathf.Abs(newHeight) > Mathf.Abs(newWidth) ? newWidth : newHeight;

        Vector2 minimumPos = startMousePos + (newVec * weight);

        // 각 끝점에서 ray direction 값 가져오고 둘의 중간지점의 origin 값 가져오기.

        Vector3 maxPos = MeshManager.Instance.cam.ScreenPointToRay(maximumPos).direction.normalized * 1000;
        Vector3 minPos = MeshManager.Instance.cam.ScreenPointToRay(minimumPos).direction.normalized * 1000;
        Vector3 middlePos = MeshManager.Instance.cam.ScreenPointToRay(Vector2.Lerp(minimumPos, maximumPos, 0.5f)).origin;


        IntPtr heart = CreateMeshObject();
        float[] verticesCoordinate = ConvertToFloatArray(AdjacencyList.Instance.worldPositionVertices.ToArray());

        if (BuildPolyhedron(heart,
            verticesCoordinate,
            verticesCoordinate.Length / 3,
            MeshManager.Instance.mesh.triangles,
            MeshManager.Instance.mesh.triangles.Length / 3) == -1)
        {
            Debug.Log("polyhedron 형성이 안됨.");
            return;
        }

        IntPtr clipper = CreateMeshObject();
        float[] verticesClipper = {
            maxPos.x, maxPos.y, maxPos.z,
            minPos.x, minPos.y, minPos.z,
            middlePos.x, middlePos.y, middlePos.z
        };
        int[] trianglesClipper = {
            0, 1, 2
        };

        if (BuildPolyhedron(clipper,
            verticesClipper,
            verticesClipper.Length / 3,
            trianglesClipper,
            1) == -1)
        {
            Debug.Log("polyhedron 형성이 안됨.");
            return;
        }
        CorefineByMesh(heart, clipper);
        Debug.Log("done corefine");

        // 추가되는 vtx 활용
        Vector3[] newVertices = ConvertToVector(GetVertices(heart), GetNumberOfVertices(heart), GameObject.Find("PartialModel").transform);
        int[] newTriangles = ConvertToTriangle(GetFaces(heart), GetNumberOfFaces(heart));

        //AdjacencyList.Instance.worldPositionVertices.cou
















        Material heartMaterial = Resources.Load("Materials/Heart", typeof(Material)) as Material;
        MeshManager.Instance.SetNewObject(CGAL.GenerateNewObject(heart, heartMaterial));
        MakeDoubleFaceMesh.Instance.Reinitialize();

    }
    
    // 평면위의 점 a,b,c
    public static void ThreeDimensionToTwoDimension(Vector3 a, Vector3 b, Vector3 c, Vector3 ans)
    {
        //평면의 노말을 (0, 0, 1)로 변환
        Vector3 AB = b - a;
        Vector3 AC = c - a;
        Vector3 N = Vector3.Cross(AB, AC).normalized;
        Vector3 U = Vector3.Normalize(AB);

        Vector3 V = Vector3.Cross(U, Vector3.Normalize(N));

        U = new Vector3(Mathf.Abs(U.x), Mathf.Abs(U.y), Mathf.Abs(U.z));
        V = new Vector3(Mathf.Abs(V.x), Mathf.Abs(V.y), Mathf.Abs(V.z));
        N = new Vector3(Mathf.Abs(N.x), Mathf.Abs(N.y), Mathf.Abs(N.z));
        Vector3 u = a + U;
        Vector3 v = a + V;
        Vector3 n = a + N;

        Matrix4x4 S = new Matrix4x4(
            new Vector4(a.x, a.y, a.z, 1),
            new Vector4(u.x, u.y, u.z, 1),
            new Vector4(v.x, v.y, v.z, 1),
            new Vector4(n.x, n.y, n.z, 1)
            );

        Debug.Log(S);

        Matrix4x4 D = new Matrix4x4(
            new Vector4(0, 0, 0, 1),
            new Vector4(1, 0, 0, 1),
            new Vector4(0, 1, 0, 1),
            new Vector4(0, 0, 1, 1)
            );

        Debug.Log(Matrix4x4.Inverse(S));

        Matrix4x4 M = D * S.inverse;

        Vector3 temp = M * ans;
        Debug.Log(temp);
    }



    public static void GetDiameter(Vector3 firstPoint, Vector3 lastPoint, Vector3 rayOrigin)
    {
        AdjacencyList.Instance.ListUpdate();

        GameObject asdf = new GameObject("Triangle", typeof(MeshFilter), typeof(MeshRenderer));
        Mesh asdf_mesh = asdf.GetComponent<MeshFilter>().mesh;
        Vector3[] newV = new Vector3[3];
        newV[0] = asdf.transform.InverseTransformPoint(firstPoint);
        newV[1] = asdf.transform.InverseTransformPoint(lastPoint);
        newV[2] = asdf.transform.InverseTransformPoint(rayOrigin);

        int[] newT = new int[3];
        newT[0] = 0;
        newT[1] = 1;
        newT[2] = 2;

        asdf_mesh.vertices = newV;
        asdf_mesh.triangles = newT;
        asdf_mesh.RecalculateNormals();


        //IntPtr heart = CreateMeshObject();




        //float[] verticesCoordinate = ConvertToFloatArray(AdjacencyList.Instance.worldPositionVertices.ToArray());

        //if (BuildPolyhedron(heart,
        //    verticesCoordinate,
        //    verticesCoordinate.Length / 3,
        //    MeshManager.Instance.mesh.triangles,
        //    MeshManager.Instance.mesh.triangles.Length / 3) == -1)
        //{
        //    Debug.Log("polyhedron 형성이 안됨.");
        //    return;
        //}
        //Debug.Log(GetNumberOfVertices(heart));
        //Intersection(heart, GeneratePlane(rayOrigin, firstPoint, lastPoint));
        ////Debug.Log(GetNumberOfVertices(heart));




    }



    public void RabbitIncision(int centerIndex, float radius)
    {
        IntPtr heart = CreateMeshObject();
        float[] verticesCoordinate = ConvertToFloatArray(AdjacencyList.Instance.worldPositionVertices.ToArray());
        BuildPolyhedron(heart,
            verticesCoordinate,
            verticesCoordinate.Length / 3,
            MeshManager.Instance.mesh.triangles,
            MeshManager.Instance.mesh.triangles.Length / 3);

        PreprocessDeformMesh(heart, centerIndex, radius, 0.1f);
        
        Vector3[] newVertices = ConvertToVector(GetVertices(heart), GetNumberOfVertices(heart), GameObject.Find("PartialModel").transform);
        MeshManager.Instance.mesh.vertices = newVertices;

    }

    public void Slicer(Vector3 firstPoint, Vector3 lastPoint, Vector3 rayOrigin)
    {
        IntPtr heart = CreateMeshObject();
        float[] verticesCoordinate = ConvertToFloatArray(AdjacencyList.Instance.worldPositionVertices.ToArray());
        float[] slicerVerticesCoordinate = new float[9];

        slicerVerticesCoordinate[0] = firstPoint.x;
        slicerVerticesCoordinate[1] = firstPoint.y;
        slicerVerticesCoordinate[2] = firstPoint.z;

        slicerVerticesCoordinate[3] = lastPoint.x;
        slicerVerticesCoordinate[4] = lastPoint.y;
        slicerVerticesCoordinate[5] = lastPoint.z;

        slicerVerticesCoordinate[6] = rayOrigin.x;
        slicerVerticesCoordinate[7] = rayOrigin.y;
        slicerVerticesCoordinate[8] = rayOrigin.z;

        if (BuildPolyhedron(heart,
            verticesCoordinate,
            verticesCoordinate.Length / 3,
            MeshManager.Instance.mesh.triangles,
            MeshManager.Instance.mesh.triangles.Length / 3) == -1)
        {
            Debug.Log("polyhedron 형성이 안됨.");
            return;
        }

        ClipPolyhedronByPlane(heart, slicerVerticesCoordinate);
        Vector3[] newVertices = ConvertToVector(GetVertices(heart), GetNumberOfVertices(heart), GameObject.Find("PartialModel").transform);
        int[] newTriangles = ConvertToTriangle(GetFaces(heart), GetNumberOfFaces(heart));

        // 새롭게 mesh instance 만들어내는 과정이 필요함. 
        // 그리고 기존것 지우고 뭔가 또 해야됨.
        GameObject newObject = new GameObject("Heart_", typeof(MeshFilter), typeof(MeshRenderer));
        newObject.GetComponent<MeshRenderer>().material = MeshManager.Instance.Heart.GetComponent<MeshRenderer>().material;

        newObject.transform.SetParent(GameObject.Find("PartialModel").transform);
        newObject.transform.localPosition = Vector3.zero;
        newObject.transform.localScale = Vector3.one;
        Mesh newMesh = newObject.GetComponent<MeshFilter>().mesh;

        newMesh.vertices = newVertices;
        newMesh.triangles = newTriangles;
        newMesh.RecalculateNormals();

        //MeshManager.Instance.mesh.vertices = newVertices;
        //MeshManager.Instance.mesh.triangles = newTriangles;
    }

    public static int[] ExtractCircleByBFS_Test(int startVertexIndex, int endVertexIndex, int startBFSIndex)
    {
        IntPtr heart = CreateMeshObject();
        float[] verticesCoordinate = ConvertToFloatArray(AdjacencyList.Instance.worldPositionVertices.ToArray());
        BuildPolyhedron(heart,
            verticesCoordinate,
            verticesCoordinate.Length / 3,
            MeshManager.Instance.mesh.triangles,
            MeshManager.Instance.mesh.triangles.Length / 3);

        IntPtr circle = ExtractCircleFixed(heart, startVertexIndex, endVertexIndex, startBFSIndex);
        int length = ExtractCircleFixedLength(heart);
        int[] indexList = new int[length];
        Debug.Log(length);
        Marshal.Copy(circle, indexList, 0, length);

        return indexList;
    }

    public static int[] ExtractCircleByBFS(int startVertexIndex, int endVertexIndex, int startBFSIndex)
    {
        IntPtr heart = CreateMeshObject();
        float[] verticesCoordinate = ConvertToFloatArray(AdjacencyList.Instance.worldPositionVertices.ToArray());
        BuildPolyhedron(heart,
            verticesCoordinate,
            verticesCoordinate.Length / 3,
            MeshManager.Instance.mesh.triangles,
            MeshManager.Instance.mesh.triangles.Length / 3);

        IntPtr circle = ExtractCircle(heart, startVertexIndex, endVertexIndex, startBFSIndex);
        int length = ExtractCircleLength(heart);
        int[] indexList = new int[length];
        Debug.Log(length);
        Marshal.Copy(circle, indexList, 0, length);

        return indexList;
    }

    public void NewBoundaryCut(List<Vector3> clipperVertices, List<Vector3> rayDirections, Vector3 rayDirection)
    {
        //Debug.Log(clipperVertices.Count);
        IntPtr heart = CreateMeshObject();
        float[] verticesCoordinate = ConvertToFloatArray(AdjacencyList.Instance.worldPositionVertices.ToArray());
        BuildPolyhedron(heart,
            verticesCoordinate,
            verticesCoordinate.Length / 3,
            MeshManager.Instance.mesh.triangles,
            MeshManager.Instance.mesh.triangles.Length / 3);

        IntPtr clipper = CreateMeshObject();
        float[] clipperVerticesCoordinate = ConvertToFloatArray(clipperVertices.ToArray());
        float[] clipperDirection = new float[3];
        clipperDirection[0] = rayDirection.x;
        clipperDirection[1] = rayDirection.y;
        clipperDirection[2] = rayDirection.z;

        TriangulateVertices(clipper, clipperVerticesCoordinate, clipperVerticesCoordinate.Length / 3);

        //Extrude(clipper, clipperDirection);

        Vector3[] newClipperVertices = ConvertToVector(GetVertices(clipper), GetNumberOfVertices(clipper));
        int[] newClipperTriangles = ConvertToTriangle(GetFaces(clipper), GetNumberOfFaces(clipper));
        float[] newnewClipperVertices = ConvertToFloatArray(newClipperVertices);

        Vector3[] newVertices = new Vector3[newClipperVertices.Length * 2];
        int[] newTriangles = new int[(newClipperVertices.Length * 6) + (newClipperTriangles.Length)];

        // stamp 수정해야됨.
        //GenerateStamp(newClipperVertices, rayDirections, newClipperTriangles, ref newVertices, ref newTriangles);

        ClipPolyhedron(heart, newnewClipperVertices, newnewClipperVertices.Length / 3, newClipperTriangles, newClipperTriangles.Length / 3);
        //float[] newClipperVertices = Commonthings.GetFloatArray(GetVertices(clipper), clipperVerticesCoordinate.Length);
        //int[] newClipperTriangles = Commonthings.GetIntArray(GetFaces(clipper), GetNumberOfFaces(clipper)*3);

        //Vector3[] newClipperPositions = Commonthings.ConvertToVector(newClipperVertices);
        //float[] newnewVertices = Commonthings.ConvertToFloatArray(newClipperPositions);
        ////for (int i = 0; i < newClipperPositions.Length; i++)
        //{
        //    GameObject v_test = new GameObject();
        //    v_test = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //    v_test.transform.position = newClipperPositions[i];
        //}

        //ClipPolyhedron(heart, newnewVertices, newnewVertices.Length / 3, newClipperTriangles, newClipperTriangles.Length / 3);

        //Vector3[] newVertices = Commonthings.ConvertToVector(GetVertices(heart), GetNumberOfVertices(heart), GameObject.Find("PartialModel").transform);
        //int[] newTriangles = Commonthings.ConvertToTriangle(GetFaces(heart), GetNumberOfFaces(heart));

        GameObject newObject = new GameObject("Heart_", typeof(MeshFilter), typeof(MeshRenderer));
        newObject.GetComponent<MeshRenderer>().material = MeshManager.Instance.Heart.GetComponent<MeshRenderer>().material;

        newObject.transform.SetParent(GameObject.Find("PartialModel").transform);
        newObject.transform.localPosition = Vector3.zero;
        newObject.transform.localScale = Vector3.one;
        Mesh newMesh = newObject.GetComponent<MeshFilter>().mesh;

        newMesh.vertices = newClipperVertices;
        newMesh.triangles = newClipperTriangles;
        newMesh.RecalculateNormals();

    }

    public static Vector3[] ConvertToVector(IntPtr verticesPtr, int vertexCount, Transform parentTransform)
    {
        float[] verticesCoordinate = new float[vertexCount * 3];
        Marshal.Copy(verticesPtr, verticesCoordinate, 0, vertexCount * 3);
        Vector3[] vertices = new Vector3[vertexCount];
        int newCount = 0;
        for (int i = 0; i < vertexCount; i++)
        {
            vertices[i].x = verticesCoordinate[newCount++];
            vertices[i].y = verticesCoordinate[newCount++];
            vertices[i].z = verticesCoordinate[newCount++];
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = parentTransform.InverseTransformPoint(vertices[i]);
        }

        return vertices;
    }

    public static Vector3[] ConvertToVector(float[] verticesFloatArray)
    {
        Vector3[] vertices = new Vector3[verticesFloatArray.Length / 3];
        int newCount = 0;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].x = verticesFloatArray[newCount++];
            vertices[i].y = verticesFloatArray[newCount++];
            vertices[i].z = verticesFloatArray[newCount++];
        }

        return vertices;
    }

    public static Vector3[] ConvertToVector(IntPtr verticesPtr, int vertexCount)
    {
        float[] verticesCoordinate = new float[vertexCount * 3];
        Marshal.Copy(verticesPtr, verticesCoordinate, 0, vertexCount * 3);
        Vector3[] vertices = new Vector3[vertexCount];
        int newCount = 0;
        for (int i = 0; i < vertexCount; i++)
        {
            vertices[i].x = verticesCoordinate[newCount++];
            vertices[i].y = verticesCoordinate[newCount++];
            vertices[i].z = verticesCoordinate[newCount++] + 100f;
        }
        return vertices;
    }

    public static int[] ConvertToTriangle(IntPtr trianglesPtr, int triangleCount)
    {
        int[] triangles = new int[triangleCount * 3];
        Marshal.Copy(trianglesPtr, triangles, 0, triangleCount * 3);
        return triangles;
    }

    public static float[] ConvertToFloatArray(Vector3[] vertices)
    {
        float[] verticesCoordinate = new float[vertices.Length * 3];
        int newCount = 0;
        for (int i = 0; i < vertices.Length; i++)
        {
            verticesCoordinate[newCount++] = vertices[i].x;
            verticesCoordinate[newCount++] = vertices[i].y;
            verticesCoordinate[newCount++] = vertices[i].z;
        }
        return verticesCoordinate;
    }

    public static float[] GetFloatArray(IntPtr heart, int length)
    {
        //VERTICES 변환
        float[] newHeart = new float[length * 3];
        Marshal.Copy(heart, newHeart, 0, length);
        return newHeart;
    }

    public static int[] GetIntArray(IntPtr heart, int length)
    {
        //FACES 변환
        int[] newHeart = new int[length * 3];
        Marshal.Copy(heart, newHeart, 0, length);
        return newHeart;
    }

    public static void GenerateStamp(List<Ray> rayList, ref Vector3[] newVertices, ref int[] newTriangles)
    {
        int vertexCount = rayList.Count;

        int newCount = 0;

        for (int i = 0; i < rayList.Count * 2; i++)
        {
            if (i >= vertexCount)
            {
                newVertices[i] = rayList[newCount].origin + rayList[newCount++].direction * 5000f;
            }
            else
            {
                newVertices[i] = rayList[i].origin + rayList[i].direction * 1f;
            }
        }

        newCount = 0;
        int newVertexCount = vertexCount;
        for (int i = 0; i < vertexCount; i++)
        {
            if (i == vertexCount - 1)
            {
                newTriangles[newCount++] = i;
                newTriangles[newCount++] = newVertexCount;
                newTriangles[newCount++] = 0;

                newTriangles[newCount++] = 0;
                newTriangles[newCount++] = newVertexCount;
                newTriangles[newCount++] = vertexCount;
                break;
            }
            newTriangles[newCount++] = i;
            newTriangles[newCount++] = newVertexCount;
            newTriangles[newCount++] = i + 1;

            newTriangles[newCount++] = i + 1;
            newTriangles[newCount++] = newVertexCount;
            newTriangles[newCount++] = ++newVertexCount;
        }
        // 그냥 여기 뒤에 holefilling 넣어야 맞겠다. 

        //GameObject newObject = new GameObject("Heart_", typeof(MeshFilter), typeof(MeshRenderer));
        //newObject.GetComponent<MeshRenderer>().material = MeshManager.Instance.heart.GetComponent<MeshRenderer>().material;

        //newObject.transform.SetParent(GameObject.Find("PartialModel").transform);
        //newObject.transform.localPosition = Vector3.zero;
        //newObject.transform.localScale = Vector3.one;
        //Mesh newMesh = newObject.GetComponent<MeshFilter>().mesh;

        //newMesh.vertices = newVertices;
        //newMesh.triangles = newTriangles;
        //newMesh.RecalculateNormals();
    }

    public static void GenerateStampWithHeart(List<Vector3> verticesPos, List<Ray> rayList, ref Vector3[] newVertices, ref int[] newTriangles)
    {
        int vertexCount = rayList.Count;

        int newCount = 0;

        for (int i = 0; i < rayList.Count * 2; i++)
        {
            if (i >= vertexCount)
            {
                // vertex들 넣어줘야됨.
                newVertices[i] = verticesPos[newCount] + (Vector3.Normalize(rayList[newCount++].direction) * MeshManager.Instance.objTransform.lossyScale.z * 8);
            }
            else
            {
                newVertices[i] = rayList[i].origin + rayList[i].direction * 1f;
            }
        }

        newCount = 0;
        int newVertexCount = vertexCount;
        for (int i = 0; i < vertexCount; i++)
        {
            if (i == vertexCount - 1)
            {
                newTriangles[newCount++] = i;
                newTriangles[newCount++] = newVertexCount;
                newTriangles[newCount++] = 0;

                newTriangles[newCount++] = 0;
                newTriangles[newCount++] = newVertexCount;
                newTriangles[newCount++] = vertexCount;
                break;
            }
            newTriangles[newCount++] = i;
            newTriangles[newCount++] = newVertexCount;
            newTriangles[newCount++] = i + 1;

            newTriangles[newCount++] = i + 1;
            newTriangles[newCount++] = newVertexCount;
            newTriangles[newCount++] = ++newVertexCount;
        }
    }

    public static float[] GeneratePlane(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        float[] plane = new float[9];
        int count = 0;
        plane[count++] = v1.x;
        plane[count++] = v1.y;
        plane[count++] = v1.z;
        plane[count++] = v2.x;
        plane[count++] = v2.y;
        plane[count++] = v2.z;
        plane[count++] = v3.x;
        plane[count++] = v3.y;
        plane[count++] = v3.z;
        return plane;
    }

    public static GameObject GenerateNewObject(IntPtr heart, Material material)
    {
        Vector3[] newVertices = ConvertToVector(GetVertices(heart), GetNumberOfVertices(heart), GameObject.Find("PartialModel").transform);
        int[] newTriangles = ConvertToTriangle(GetFaces(heart), GetNumberOfFaces(heart));

        // 새롭게 mesh instance 만들어내는 과정이 필요함. 
        // 그리고 기존것 지우고 뭔가 또 해야됨.
        GameObject newObject = new GameObject("COLOR____", typeof(MeshFilter), typeof(MeshRenderer));
        newObject.GetComponent<MeshRenderer>().material = material;

        newObject.transform.SetParent(GameObject.Find("PartialModel").transform);
        newObject.transform.localPosition = Vector3.zero;
        newObject.transform.localRotation = Quaternion.identity;
        newObject.transform.localScale = Vector3.one;
        Mesh newMesh = newObject.GetComponent<MeshFilter>().mesh;

        newMesh.vertices = newVertices;
        newMesh.triangles = newTriangles;
        newMesh.RecalculateNormals();
        return newObject;
    }

}
