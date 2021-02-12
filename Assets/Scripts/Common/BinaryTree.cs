using System;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

public class Node
{
	public int[] vertices;
	public int[] faces;
	public int length = 0;
	public int index = 0;
	public int curDepth;
	public Vector3 minPos, maxPos;
	public Node left, right;
	public Node parent;

	public void newNode(int i)
	{
		index = i;
	}
}

public class BinaryTree : MonoBehaviour
{
	public static int depth = 0;
	public Node thisNode = new Node();
	public Node root = new Node();
	public static Node savedNode = new Node();
	public static bool ChildGene = false;
	public Mesh copiedMesh;
	private static int limit = 8;
	private static int count = 1;
	private static Node left, right, mainNode;

	void Start()
	{
		Debug.Log("Start enter");
	}

	public static void Initialize()
	{
		for (int i = 0; i < MultiMeshManager.Instance.Meshes[0].vertices.Length; i++)
		{
			//root.vertices.Add(MultiMeshManager.Instance.Meshes[0].vertices[i]);
		}
	}

	private static void calculateMinMax(Node thisNode)
    {
		float xMin, yMin, zMin, xMax, yMax, zMax;
		xMin = float.MaxValue; xMax = float.MinValue; yMin = float.MaxValue; yMax = float.MinValue; zMin = float.MaxValue; zMax = float.MinValue;




		Vector3[] ThisVertices = MultiMeshManager.Instance.Meshes[0].vertices;

		for (int i = 0; i < thisNode.length; i++)
		{
			if (xMin > ThisVertices[thisNode.vertices[i]].x) xMin = ThisVertices[thisNode.vertices[i]].x;
			if (xMax < ThisVertices[thisNode.vertices[i]].x) xMax = ThisVertices[thisNode.vertices[i]].x;
			if (yMin > ThisVertices[thisNode.vertices[i]].y) yMin = ThisVertices[thisNode.vertices[i]].y;
			if (yMax < ThisVertices[thisNode.vertices[i]].y) yMax = ThisVertices[thisNode.vertices[i]].y;
			if (zMin > ThisVertices[thisNode.vertices[i]].z) zMin = ThisVertices[thisNode.vertices[i]].z;
			if (zMax < ThisVertices[thisNode.vertices[i]].z) zMax = ThisVertices[thisNode.vertices[i]].z;
		}
		if (xMin == float.MaxValue || xMax == float.MinValue || yMin == float.MaxValue || yMax == float.MinValue || zMin == float.MaxValue || zMax == float.MinValue) Debug.Log("Calc MM Error");
		thisNode.maxPos.Set(xMax, yMax, zMax);
		thisNode.minPos.Set(xMin, yMin, zMin);
		//Debug.Log("maharaga: " + thisNode.vertices.Length + ", " + thisNode.maxPos + ", " + thisNode.minPos);
	}


	private static void generateCube(Node thisNode, int leftRight)
    {		
		GameObject childCubes = GameObject.CreatePrimitive(PrimitiveType.Cube);
		//childCubes.transform.parent = GameObject.Find("Cube").transform;
		childCubes.transform.position = (thisNode.minPos + thisNode.maxPos) / 2;
		if (leftRight == 0)
			childCubes.GetComponent<Renderer>().material.color = new Color(1, 0, 0, 0.1f);
		else childCubes.GetComponent<Renderer>().material.color = new Color(0, 0, 1, 0.1f);
		childCubes.transform.localScale = new Vector3(thisNode.maxPos.x - thisNode.minPos.x, thisNode.maxPos.y - thisNode.minPos.y, thisNode.maxPos.z - thisNode.minPos.z);
		childCubes.GetComponent<Renderer>().material.shader = Shader.Find("Transparent/Diffuse");
	}

	public void copyMeshFromOriginal(Mesh inMesh)
    {
		copiedMesh = new Mesh();
		copiedMesh.vertices = inMesh.vertices;
		copiedMesh.triangles = inMesh.triangles;
		copiedMesh.uv = inMesh.uv;
		copiedMesh.normals = inMesh.normals;
		copiedMesh.colors = inMesh.colors;
		copiedMesh.tangents = inMesh.tangents;
		//AssetDatabase.CreateAsset(copiedMesh, AssetDatabase.GetAssetPath(inMesh) + " copy.asset"); h

		 //copiedMesh = (Mesh) inMesh.;
    }
	public static void GetnewNode(Node thisNode)
	{
		Node temp = new Node();
		temp = thisNode;

		if (temp.left != null) temp = temp.left;
		else if (temp.right != null) temp = temp.right;
		else
		{
			temp.left = new Node();
			temp.right = new Node();
			if (depth < 8)
			{
				depth++;
				GetnewNode(temp.left);
				GetnewNode(temp.right);
				depth -= 1;
			}
		}
	}

	public static void MakeInitialChild(Node thisNode)
	{
		float xDist, yDist, zDist;

		int temp1 = 0;
		int temp2 = 0;

		xDist = thisNode.maxPos.x - thisNode.minPos.x;
		yDist = thisNode.maxPos.y - thisNode.minPos.y;
		zDist = thisNode.maxPos.z - thisNode.minPos.z;

		thisNode.left = new Node();
		thisNode.right = new Node();		

		thisNode.left.vertices = new int[thisNode.length];
		thisNode.right.vertices = new int[thisNode.length];

		thisNode.left.faces = new int[thisNode.faces.Length];
		thisNode.right.faces = new int[thisNode.faces.Length];

		thisNode.left.length = thisNode.length;
		thisNode.right.length = thisNode.length;
		//long start = DateTime.Now.Ticks;

		Vector3[] ThisVertices = MultiMeshManager.Instance.Meshes[0].vertices;
		int[] Triangle = MultiMeshManager.Instance.Meshes[0].triangles;
		//전역변수로 빼면 좋을듯?

		// X-axis division
		if (xDist > yDist && xDist > zDist)
		{
			xDist = thisNode.minPos.x + xDist / 2.0f;
			for (int i = 0; i < thisNode.length; i++)
			{
				if (ThisVertices[thisNode.vertices[i]].x < xDist)
					thisNode.left.vertices[temp1++] = thisNode.vertices[i];
				else
					thisNode.right.vertices[temp2++] = thisNode.vertices[i];

			}
			thisNode.left.length = temp1;
			thisNode.right.length = temp2;
			temp1 = 0; temp2 = 0;
			for (int i = 0; i < thisNode.faces.Length; i++)
			{
				if (ThisVertices[Triangle[thisNode.faces[i]*3]].x < xDist)
					thisNode.left.faces[temp1++] = thisNode.faces[i];
				else
					thisNode.right.faces[temp2++] = thisNode.faces[i];

			}
			Array.Resize(ref thisNode.left.faces, temp1);
			Array.Resize(ref thisNode.right.faces, temp2);
			calculateMinMax(thisNode.left);
			calculateMinMax(thisNode.right);
		}
		else if (yDist > xDist && yDist > zDist)
		{
			yDist = thisNode.minPos.y + yDist / 2.0f;
			for (int i = 0; i < thisNode.length; i++)
			{
				if (ThisVertices[thisNode.vertices[i]].y < yDist)
					thisNode.left.vertices[temp1++] = thisNode.vertices[i];
				else
					thisNode.right.vertices[temp2++] = thisNode.vertices[i];
			}
			thisNode.left.length = temp1;
			thisNode.right.length = temp2;
			temp1 = 0; temp2 = 0;
			for (int i = 0; i < thisNode.faces.Length; i++)
			{
				if (ThisVertices[Triangle[thisNode.faces[i] * 3]].y < yDist)
					thisNode.left.faces[temp1++] = thisNode.faces[i];
				else
					thisNode.right.faces[temp2++] = thisNode.faces[i];

			}
			Array.Resize(ref thisNode.left.faces, temp1);
			Array.Resize(ref thisNode.right.faces, temp2);
			calculateMinMax(thisNode.left);
			calculateMinMax(thisNode.right);
		}
		else if (zDist > xDist && zDist > yDist)
		{
			zDist = thisNode.minPos.z + zDist / 2.0f;
			for (int i = 0; i < thisNode.length; i++)
			{
				if (ThisVertices[thisNode.vertices[i]].z < zDist)
					thisNode.left.vertices[temp1++] = thisNode.vertices[i];
				else
					thisNode.right.vertices[temp2++] = thisNode.vertices[i];
			}
			thisNode.left.length = temp1;
			thisNode.right.length = temp2;
			temp1 = 0; temp2 = 0;
			for (int i = 0; i < thisNode.faces.Length; i++)
			{
				if (ThisVertices[Triangle[thisNode.faces[i] * 3]].z < zDist)
					thisNode.left.faces[temp1++] = thisNode.faces[i];
				else
					thisNode.right.faces[temp2++] = thisNode.faces[i];

			}
			Array.Resize(ref thisNode.left.faces, temp1);
			Array.Resize(ref thisNode.right.faces, temp2);
			calculateMinMax(thisNode.left);
			calculateMinMax(thisNode.right);

		}
		
		//long end = DateTime.Now.Ticks;
		//Debug.Log("ImportMesh tick timer: " + (double)(end - start) / 10000000.0f + ", count: " + thisNode.vertices.Length);


		//generateCube(thisNode.left, 0);
		//generateCube(thisNode.right, 1);

		if (thisNode.curDepth < limit)
		{
			thisNode.left.curDepth = thisNode.curDepth + 1;
			thisNode.left.parent = thisNode;
			thisNode.right.curDepth = thisNode.curDepth + 1;
			thisNode.right.parent = thisNode;
			
			//if (thisNode.left.length > 2) 
			MakeInitialChild(thisNode.left);
			//if (thisNode.right.length > 2) 
			MakeInitialChild(thisNode.right);
		}
		thisNode.index = count++;
	}

	public static Node Finding(Vector3 Position)
	{
		long start = DateTime.Now.Ticks;
		mainNode = savedNode;

		// Start from root
		if (PositionCheckRoot(Position))
		{
			while (mainNode.curDepth < limit)
            {
				mainNode = LeftRightCheck(Position, mainNode);                
				if (mainNode == null)
					break;
				//Debug.Log("maharaga: curdepth " + Node.curDepth);
			}			
				
		}
		// else Node = null;
		if (mainNode != null) 
			if (mainNode.curDepth == 1) mainNode = null;
		long end = DateTime.Now.Ticks;
		Debug.Log("Finding tick timer: " + (double)(end - start) / 10000000.0f);
		return mainNode;
	}

	private static bool PositionCheckRoot(Vector3 position)
    {		
		if (position.x <= mainNode.maxPos.x && position.y <= mainNode.maxPos.y && position.z <= mainNode.maxPos.z && position.x >= mainNode.minPos.x && position.y >= mainNode.minPos.y && position.z >= mainNode.minPos.z)
			return true;
		else return false;			
	}
	private static Node LeftRightCheck(Vector3 position, Node node)
    {
		left = node.left;
		right = node.right;
		if (position.x <= left.maxPos.x && position.y <= left.maxPos.y && position.z <= left.maxPos.z && position.x >= left.minPos.x && position.y >= left.minPos.y && position.z >= left.minPos.z)
		{
			Debug.Log("left check: " + node.curDepth);
			return left;
		}
		else if (position.x <= right.maxPos.x && position.y <= right.maxPos.y && position.z <= right.maxPos.z && position.x >= right.minPos.x && position.y >= right.minPos.y && position.z >= right.minPos.z)
		{
			Debug.Log("right check");
			return right;
		}
		Debug.Log("Error! Vectorcomparison: " + node.curDepth);
		return null;
	}

	/* private Node NodesearchByIndex(Node curNode, int i)
    {
		if (curNode.left != null) {
			if (curNode.index != i)
			{
				NodesearchByIndex(curNode.left, i);
				NodesearchByIndex(curNode.right, i);
				return;
			}
            else { return curNode; }
		}
        else
        {
			if (curNode.index != i) return;
            else { return curNode; }
        }
    }
	private Node NodesearchByIndex(int i)
	{
		Node curNode = BinaryTree.savedNode;
		if (curNode.left != null)
		{
			if (curNode.index != i)
			{
				NodesearchByIndex(curNode.left, i);
				NodesearchByIndex(curNode.right, i);
				return;
			}
			else { return curNode; }
		}
		else
		{
			if (curNode.index != i) return;
			else { return curNode; }
		}
	} */
}