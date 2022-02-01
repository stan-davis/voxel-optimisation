using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreedyChunk : MonoBehaviour
{
    public const int CHUNK_SIZE = 5;

    public Voxel[,,] data = new Voxel[CHUNK_SIZE, CHUNK_SIZE, CHUNK_SIZE];

    private ChunkHelper chunkHelper = new ChunkHelper();
    
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();

    private void Start()
    {
        //For testing only!!
        for (int y = 0; y < CHUNK_SIZE; y++)
            for (int z = 0; z < CHUNK_SIZE; z++)
                for (int x = 0; x < CHUNK_SIZE; x++)
                {
                    data[x, y, z].type = 1;
                }

        GenerateMesh();
    }

    public void GenerateMesh()
    {
        DateTime startTime = DateTime.Now;

        Mesh mesh = new Mesh();
        mesh.Clear();

        for (int y = 0; y < CHUNK_SIZE; y++)
            for (int z = 0; z < CHUNK_SIZE; z++)
                for (int x = 0; x < CHUNK_SIZE; x++)
                {
                    ref Voxel v = ref data[x, y, z];

                    if (v.type != 0) CreateVoxel(ref v, new Vector3Int(x, y, z));
                }

        CreateTriangles();
        
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);

        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;

        //Debug Output
        DateTime currentTime = DateTime.Now;
        TimeSpan duration = currentTime.Subtract(startTime);

        Debug.Log("Vertices: " + vertices.Count.ToString());
        Debug.Log("Triangles: " + triangles.Count.ToString());
        Debug.Log("Chunk built in " + duration.Milliseconds.ToString() + "ms");
    }

    private void CreateVoxel(ref Voxel voxel, Vector3Int position)
    {
        int x = position.x;
        int y = position.y;
        int z = position.z;

        int length = 0;

        //LEFT (X-)
        if (!chunkHelper.visitedXN[x, y, z] && VisibleFaceXN(x - 1, y, z))
        {
            for (int i = y; i < CHUNK_SIZE; i++)
            {
                //If we reach an empty block, end the run
                if (DifferentVoxel(new Vector3Int(x, i, z), voxel)) 
                   break;

                chunkHelper.visitedXN[x, i, z] = true;

                length++;
            }

            if(length > 0)
            {
                AppendQuad(
                    new Vector3(x, y + length, z),
                    new Vector3(x, y + length, z),
                    new Vector3(x, y, z),
                    new Vector3(x, y, z),
                    Direction.LEFT);
            }
        }

        length = 0;

        //RIGHT (X+)
        if (!chunkHelper.visitedXP[x, y, z] && VisibleFaceXP(x + 1, y, z))
        {
            for (int i = y; i < CHUNK_SIZE; i++)
            {
                //If we reach an empty block, end the run
                if (DifferentVoxel(new Vector3Int(x, i, z), voxel))
                    break;

                chunkHelper.visitedXP[x, i, z] = true;

                length++;
            }

            if (length > 0)
            {
                AppendQuad(
                    new Vector3(x, y + length, z),
                    new Vector3(x, y + length, z),
                    new Vector3(x, y, z),
                    new Vector3(x, y, z),
                    Direction.RIGHT);
            }
        }

        length = 0;

        //BACK (Z-)
        if (!chunkHelper.visitedZN[x, y, z] && VisibleFaceZN(x, y, z - 1))
        {
            for (int i = y; i < CHUNK_SIZE; i++)
            {
                //If we reach an empty block, end the run
                if (DifferentVoxel(new Vector3Int(x, i, z), voxel))
                    break;

                chunkHelper.visitedZN[x, i, z] = true;

                length++;
            }

            if (length > 0)
            {
                AppendQuad(
                    new Vector3(x, y + length, z),
                    new Vector3(x, y + length, z),
                    new Vector3(x, y, z),
                    new Vector3(x, y, z),
                    Direction.BACK);
            }
        }

        length = 0;

        //FRONT (Z+)
        if (!chunkHelper.visitedZP[x, y, z] && VisibleFaceZP(x, y, z + 1))
        {
            for (int i = y; i < CHUNK_SIZE; i++)
            {
                //If we reach an empty block, end the run
                if (DifferentVoxel(new Vector3Int(x, i, z), voxel))
                    break;

                chunkHelper.visitedZP[x, i, z] = true;

                length++;
            }

            if (length > 0)
            {
                AppendQuad(
                    new Vector3(x, y + length, z),
                    new Vector3(x, y + length, z),
                    new Vector3(x, y, z),
                    new Vector3(x, y, z),
                    Direction.FRONT);
            }
        }

        length = 0;

        //BACK (Y-)
        if (!chunkHelper.visitedYN[x, y, z] && VisibleFaceYN(x, y - 1, z))
        {
            for (int i = x; i < CHUNK_SIZE; i++)
            {
                //If we reach an empty block, end the run
                if (DifferentVoxel(new Vector3Int(i, y, z), voxel))
                    break;

                chunkHelper.visitedYN[i, y, z] = true;

                length++;
            }

            if (length > 0)
            {
                AppendQuad(
                    new Vector3(x + length, y, z),
                    new Vector3(x + length, y, z),
                    new Vector3(x, y, z),
                    new Vector3(x, y, z),
                    Direction.BOTTOM);
            }
        }

        length = 0;

        //TOP (Y+)
        if (!chunkHelper.visitedYP[x, y, z] && VisibleFaceYP(x, y + 1, z))
        {
            for (int i = x; i < CHUNK_SIZE; i++)
            {
                //If we reach an empty block, end the run
                if (DifferentVoxel(new Vector3Int(i, y, z), voxel))
                    break;

                chunkHelper.visitedYP[i, y, z] = true;

                length++;
            }

            if (length > 0)
            {
                AppendQuad(
                    new Vector3(x + length, y, z),
                    new Vector3(x + length, y, z),
                    new Vector3(x, y, z),
                    new Vector3(x, y, z),
                    Direction.TOP);
            }
        }
    }

    bool VisibleFaceXN(int x, int y, int z)
    {
        if (x < 0) return true;

        return data[x, y, z].type == 0;
    }

    bool VisibleFaceXP(int x, int y, int z)
    {
        if (x >= CHUNK_SIZE) return true;

        return data[0, y, z].type == 0;
    }

    bool VisibleFaceZN(int x, int y, int z)
    {
        if (z < 0) return true;

        return data[x, y, z].type == 0;
    }

    bool VisibleFaceZP(int x, int y, int z)
    {
        if (z >= CHUNK_SIZE) return true;

        return data[x, y, 0].type == 0;
    }

    bool VisibleFaceYN(int x, int y, int z)
    {
        if (y < 0) return true;

        return data[x, y, z].type == 0;
    }

    bool VisibleFaceYP(int x, int y, int z)
    {
        if (y >= CHUNK_SIZE) return true;

        return data[x, 0, z].type == 0;
    }

    bool DifferentVoxel(Vector3Int access, Voxel current)
    {
        ref Voxel v = ref data[access.x, access.y, access.z];

        return v.type != current.type;
    }

    private void AppendQuad(Vector3 tl, Vector3 tr, Vector3 br, Vector3 bl, Direction direction)
    {
        vertices.Add(tl + VoxelMeshData.vertices[(int)direction][0]);
        vertices.Add(tr + VoxelMeshData.vertices[(int)direction][1]);
        vertices.Add(br + VoxelMeshData.vertices[(int)direction][2]);
        vertices.Add(bl + VoxelMeshData.vertices[(int)direction][3]);
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

public class ChunkHelper
{
    public bool[,,] visitedXN = new bool[Chunk.CHUNK_SIZE, Chunk.CHUNK_SIZE, Chunk.CHUNK_SIZE];
    public bool[,,] visitedXP = new bool[Chunk.CHUNK_SIZE, Chunk.CHUNK_SIZE, Chunk.CHUNK_SIZE];
    public bool[,,] visitedYN = new bool[Chunk.CHUNK_SIZE, Chunk.CHUNK_SIZE, Chunk.CHUNK_SIZE];
    public bool[,,] visitedYP = new bool[Chunk.CHUNK_SIZE, Chunk.CHUNK_SIZE, Chunk.CHUNK_SIZE];
    public bool[,,] visitedZN = new bool[Chunk.CHUNK_SIZE, Chunk.CHUNK_SIZE, Chunk.CHUNK_SIZE];
    public bool[,,] visitedZP = new bool[Chunk.CHUNK_SIZE, Chunk.CHUNK_SIZE, Chunk.CHUNK_SIZE];
}