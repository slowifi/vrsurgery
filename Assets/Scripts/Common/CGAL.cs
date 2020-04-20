using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Runtime.InteropServices;

public class CGAL
{
    [DllImport("CGALtest_dll.dll", EntryPoint = "CreateMeshObject", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr CreateMeshObject();

    [DllImport("CGALtest_dll.dll", EntryPoint = "BuildPolyhedron", CallingConvention = CallingConvention.Cdecl)]
    public static extern int BuildPolyhedron(IntPtr value, float[] _vertices, int verticesLength, int[] _indices, int indicesLength);

    [DllImport("CGALtest_dll.dll", EntryPoint = "SavePolyhedron", CallingConvention = CallingConvention.Cdecl)]
    public static extern int SavePolyhedron(IntPtr value, string path);

    [DllImport("CGALtest_dll.dll", EntryPoint = "ClipPolyhedronByMesh", CallingConvention = CallingConvention.Cdecl)]
    public static extern int ClipPolyhedronByMesh(IntPtr clippee, IntPtr clipper);

    [DllImport("CGALtest_dll.dll", EntryPoint = "ClipPolyhedron", CallingConvention = CallingConvention.Cdecl)]
    public static extern int ClipPolyhedron(IntPtr value, float[] _vertices, int verticesLength, int[] _indices, int indicesLength);

    [DllImport("CGALtest_dll.dll", EntryPoint = "GetVertices", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr GetVertices(IntPtr value);

    [DllImport("CGALtest_dll.dll", EntryPoint = "GetFaces", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr GetFaces(IntPtr value);

    [DllImport("CGALtest_dll.dll", EntryPoint = "GetNumberOfVertices", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetNumberOfVertices(IntPtr value);

    [DllImport("CGALtest_dll.dll", EntryPoint = "GetNumberOfFaces", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GetNumberOfFaces(IntPtr value);

    [DllImport("CGALtest_dll.dll", EntryPoint = "BuildPolyhedronByPath", CallingConvention = CallingConvention.Cdecl)]
    public static extern int BuildPolyhedronByPath(IntPtr value, string path);

    [DllImport("CGALtest_dll.dll", EntryPoint = "ClipPolyhedronByHull", CallingConvention = CallingConvention.Cdecl)]
    public static extern int ClipPolyhedronByHull(IntPtr value, float[] _vertices, int _verticesLength);

    [DllImport("CGALtest_dll.dll", EntryPoint = "ClipPolyhedronByPlane", CallingConvention = CallingConvention.Cdecl)]
    public static extern int ClipPolyhedronByPlane(IntPtr value, float[] _vertices);

    [DllImport("CGALtest_dll.dll", EntryPoint = "PreprocessDeformMeshArray", CallingConvention = CallingConvention.Cdecl)]
    public static extern int PreprocessDeformMesh(IntPtr value, int[] _roi, int roiLength, int[] _cvertices, int _cverticesLength);

    [DllImport("CGALtest_dll.dll", EntryPoint = "PreprocessDeformMeshRadius", CallingConvention = CallingConvention.Cdecl)]
    public static extern int PreprocessDeformMesh(IntPtr value, int centerIndex, float roiRadius, float controlRadius);

    [DllImport("CGALtest_dll.dll", EntryPoint = "DeformMesh", CallingConvention = CallingConvention.Cdecl)]
    public static extern int DeformMesh(IntPtr value, float[] direction);

    [DllImport("CGALtest_dll.dll", EntryPoint = "FillHole", CallingConvention = CallingConvention.Cdecl)]
    public static extern int FillHole(IntPtr value);

    [DllImport("CGALtest_dll.dll", EntryPoint = "Extrude", CallingConvention = CallingConvention.Cdecl)]
    public static extern int Extrude(IntPtr value, float[] direction);

    [DllImport("CGALtest_dll.dll", EntryPoint = "TriangulateVertices", CallingConvention = CallingConvention.Cdecl)]
    public static extern int TriangulateVertices(IntPtr value, float[] _vertices, int _verticesLength);

    [DllImport("CGALtest_dll.dll", EntryPoint = "GeodesicDistance", CallingConvention = CallingConvention.Cdecl)]
    public static extern float GeodesicDistance(IntPtr value, int v1, int v2);

    //사용시에 첫번째 포인트로 카메라 origin을 넣어줘야함.
    [DllImport("CGALtest_dll.dll", EntryPoint = "Intersection", CallingConvention = CallingConvention.Cdecl)]
    public static extern void Intersection(IntPtr value, float[] plane);

    [DllImport("CGALtest_dll.dll", EntryPoint = "CorefineByMesh", CallingConvention = CallingConvention.Cdecl)]
    public static extern int CorefineByMesh(IntPtr clippee, IntPtr clipper);

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

    public void NewversionOfBoundaryCut(List<Vector3> clipperVertices, List<Vector3> rayDirections, Vector3 rayDirection)
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
        Debug.Log(triangles.Length);
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
                Debug.Log(newVertices[i]);
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
                newVertices[i] = verticesPos[newCount] + (Vector3.Normalize(rayList[newCount++].direction) * ObjManager.Instance.objTransform.lossyScale.z * 10);
                Debug.Log(newVertices[i]);
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
        GameObject newObject = new GameObject("newHeart", typeof(MeshFilter), typeof(MeshRenderer));
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
