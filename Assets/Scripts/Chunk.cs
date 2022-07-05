using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;

[RequireComponent(typeof(MeshFilter))] [RequireComponent(typeof(MeshRenderer))]
public class Chunk : MonoBehaviour
{
    public const int CHUNK_SIZE = 16;

    public Voxel[] data = new Voxel[CHUNK_SIZE * CHUNK_SIZE * CHUNK_SIZE];

    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();

    public void GenerateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.Clear();

        for(int x = 0; x < CHUNK_SIZE; x++)
            for (int z = 0; z < CHUNK_SIZE; z++)
                for (int y = 0; y < CHUNK_SIZE; y++)
                {
                    if (data[PositionToIndex(x,y,z)].type == 0)
                        continue;

                    CreateVoxel(x,y,z);
                }

        CreateTriangles();

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);

        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void CreateVoxel(int x, int y, int z)
    {
        Vector3 position = new Vector3(x, y, z);

        //top
        if (y == CHUNK_SIZE - 1 || data[PositionToIndex(x, y + 1, z)].type == 0)
        {
            AppendQuad(position, Direction.TOP);
        }

        //bottom
        if (position.y == 0 || data[PositionToIndex(x, y - 1, z)].type == 0)
        {
            AppendQuad(position, Direction.BOTTOM);
        }

        //front
        if (position.z == CHUNK_SIZE - 1 || data[PositionToIndex(x, y, z + 1)].type == 0)
        {
            AppendQuad(position, Direction.FRONT);
        }

        //back
        if (position.z == 0|| data[PositionToIndex(x, y, z - 1)].type == 0)
        {
            AppendQuad(position, Direction.BACK);
        }

        //right
        if (position.x == CHUNK_SIZE - 1 || data[PositionToIndex(x + 1, y, z)].type == 0)
        {
            AppendQuad(position, Direction.RIGHT);
        }

        //left
        if (position.x == 0 || data[PositionToIndex(x - 1, y, z)].type == 0)
        {
            AppendQuad(position, Direction.LEFT);
        }
    }

    private int PositionToIndex(int x, int y, int z)
    {
        return x + CHUNK_SIZE * (y + CHUNK_SIZE * z);
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