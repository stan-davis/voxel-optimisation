using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))] [RequireComponent(typeof(MeshRenderer))]
public class Chunk : MonoBehaviour
{
    public const int CHUNK_SIZE = 16;

    public Voxel[,,] data = new Voxel[CHUNK_SIZE, CHUNK_SIZE, CHUNK_SIZE];

    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();

    public void GenerateMesh()
    {
        //DateTime startTime = DateTime.Now;

        Mesh mesh = new Mesh();
        mesh.Clear();

        for(int x = 0; x < CHUNK_SIZE; x++)
            for (int z = 0; z < CHUNK_SIZE; z++)
                for (int y = 0; y < CHUNK_SIZE; y++)
                {
                    if (data[x, y, z].type == 0)
                        continue;

                    CreateVoxel(new Vector3Int(x, y, z));
                }

        CreateTriangles();

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);

        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;

        /*
        //Debug timing
        DateTime currentTime = DateTime.Now;
        TimeSpan duration = currentTime.Subtract(startTime);
        
        
        Debug.Log("Vertices: " + vertices.Count.ToString());
        Debug.Log("Triangles: " + (triangles.Count / 3).ToString());
        Debug.Log("Chunk built in " + duration.Milliseconds.ToString() + "ms");
        */
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
            vertices.Add(position + VoxelMeshDataRegular.vertices[(int)direction][i]);
    }

    private void CreateTriangles()
    {
        for (int i = 0; i < vertices.Count; i += 4)
        {
            //Create a quad from triangles
            triangles.AddRange(new int[]
            {
                i, //Triangle 1
                i + 1,
                i + 2,
                i, //Triangle 2
                i + 2,
                i + 3,
            });
        }
    }
}