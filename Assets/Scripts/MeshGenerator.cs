using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    private Mesh mesh;
    private MeshCollider meshCollider;

    private Vector3[] vertices;
    private Vector2[] uv;
    private int[] triangles;

    public int xSize = 30;
    public int zSize = 30;
    public float yMultiplier = 0f;

    void Awake()
    {
        mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh = mesh;

        meshCollider = transform.GetComponent<MeshCollider>();
    }

    void Update() 
    {
        CreateVerticesForGrid();
        UpdateMesh();

        yMultiplier = 1.7f * Mathf.Sin(Time.time) + 1.7f;
    }

    // generate mesh using specified vertices, triangles and uvs
    private void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();
        
        mesh.RecalculateBounds();
        meshCollider.sharedMesh = mesh;
    }

    private void CreateVerticesForGrid()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        triangles = new int[xSize * zSize * 6];
        uv = new Vector2[(xSize + 1) * (zSize + 1)];

        // store the vertexes and UVs for a rectangular grid of the specified length and width
        for (int index = 0, z = 0; z <= zSize; z++) 
        {
            for (int x = 0; x <= xSize; x++) 
            {
                float y = Mathf.PerlinNoise(x * .2f, z * .2f) * yMultiplier;
                vertices[index] = new Vector3(x, y, z);
                uv[index] = new Vector2(z / (float)zSize, x / (float)xSize);

                index++;
            }
        }

        // store the coordinates of all triangular faces to be generated for a mesh spanning the grid of vertices
        int vertex = 0;
        int count = 0;
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[count + 0] = vertex + 0;
                triangles[count + 1] = vertex + xSize + 1;
                triangles[count + 2] = vertex + 1;
                triangles[count + 3] = vertex + 1;
                triangles[count + 4] = vertex + xSize + 1;
                triangles[count + 5] = vertex + xSize + 2;

                vertex++;
                count += 6;
            }
            vertex++;
        }
    }

    private void OnDrawGizmos() 
    {
        if (vertices == null) 
            return;
        
        for (int i = 0; i < vertices.Length; i++) 
            Gizmos.DrawSphere(vertices[i] + transform.position, 0.1f);
    }

}
