using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public const int CHUNK_SIZE = 6;

    public Voxel[,,] data = new Voxel[CHUNK_SIZE, CHUNK_SIZE, CHUNK_SIZE];

    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private int faceCount = 0;

    private void Start()
    {
        //For testing only!!
        for (int x = 0; x < CHUNK_SIZE; x++)
            for (int z = 0; z < CHUNK_SIZE; z++)
                for (int y = 0; y < CHUNK_SIZE; y++)
                {
                    data[x, y, z].type = 1;
                }
        
       GenerateMesh();
    }

    public void GenerateMesh()
    {
        DateTime startTime = DateTime.Now;

        Mesh mesh = new Mesh();

        for(int x = 0; x < CHUNK_SIZE; x++)
            for (int z = 0; z < CHUNK_SIZE; z++)
                for (int y = 0; y < CHUNK_SIZE; y++)
                {
                    if (data[x,y,z].type != 0)
                    {
                        Vector3Int position = new Vector3Int(x, y, z);
                        CreateVoxel(position);
                    }
                }

        SetTrisFromVerts();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;

        //Debug timing
        DateTime currentTime = DateTime.Now;
        TimeSpan duration = currentTime.Subtract(startTime);

        Debug.Log("Face count: " + faceCount.ToString());
        Debug.Log("Vertices: " + vertices.Count.ToString());
        Debug.Log("Triangles: " + triangles.Count.ToString());
        Debug.Log("Chunk built in " + duration.Milliseconds.ToString() + "ms");
    }

    private void CreateVoxel(Vector3Int position)
    {
        //top
        if (position.y == CHUNK_SIZE - 1 || data[position.x, position.y + 1, position.z].type == 0)
        {
            AppendQuad(position, Direction.TOP);
        }

        //bottom
        if (position.y == 0 || data[position.x, position.y - 1, position.z].type == 0)
        {
            AppendQuad(position, Direction.BOTTOM);
        }

        //front
        if (position.z == CHUNK_SIZE - 1 || data[position.x, position.y, position.z + 1].type == 0)
        {
            AppendQuad(position, Direction.FRONT);
        }

        //back
        if (position.z == 0|| data[position.x, position.y, position.z - 1].type == 0)
        {
            AppendQuad(position, Direction.BACK);
        }

        //right
        if (position.x == CHUNK_SIZE - 1 || data[position.x + 1, position.y, position.z].type == 0)
        {
            AppendQuad(position, Direction.RIGHT);
        }

        //left
        if (position.x == 0 || data[position.x - 1, position.y, position.z].type == 0)
        {
            AppendQuad(position, Direction.LEFT);
        }
    }

    private void AppendQuad(Vector3 position, Direction direction)
    {
        for (int i = 0; i < 4; i++)
            vertices.Add(position + VoxelMeshData.vertices[(int)direction][i]);

        faceCount++;
    }

    private void SetTrisFromVerts()
    {
        int tl = vertices.Count - 4 * faceCount;

        for (int i = 0; i < faceCount; i++)
            triangles.AddRange(new int[]
            {
                tl + i * 4,
                tl + i * 4 + 1,
                tl + i * 4 + 2,
                tl + i * 4,
                tl + i * 4 + 2,
                tl + i * 4 + 3
            });
    }
}