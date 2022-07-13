using UnityEngine;
using Unity.Collections;
using Unity.Jobs;

public struct GreedyChunkJob : IJob
{
    public NativeArray<Voxel> data;

    public NativeList<Vector3> vertices;
    public NativeList<int> triangles;

    public NativeArray<bool> visitedXN;
    public NativeArray<bool> visitedXP;
    public NativeArray<bool> visitedYN;
    public NativeArray<bool> visitedYP;
    public NativeArray<bool> visitedZN;
    public NativeArray<bool> visitedZP;

    public void Execute()
    {
        for (int x = 0; x < Chunk.CHUNK_SIZE; x++)
            for (int z = 0; z < Chunk.CHUNK_SIZE; z++)
                for (int y = 0; y < Chunk.CHUNK_SIZE; y++)
                {
                    if (data[PositionToIndex(x, y, z)].type == 0)
                        continue;

                    CreateVoxel(x, y, z);
                }

        CreateTriangles();
    }

    private void CreateVoxel(int x, int y, int z)
    {
        int length = 0;
        Voxel voxel = data[PositionToIndex(x,y,z)];

        //LEFT (X-)
        if (!visitedXN[PositionToIndex(x, y, z)] && VisibleFaceXN(x - 1, y, z))
        {
            for (int i = y; i < Chunk.CHUNK_SIZE; i++)
            {
                //If we reach an empty block, end the run
                if (DifferentVoxel(new Vector3Int(x, i, z), voxel))
                    break;

                visitedXN[PositionToIndex(x, i, z)] = true;

                length++;
            }

            if (length > 0)
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
        if (!visitedXP[PositionToIndex(x, y, z)] && VisibleFaceXP(x + 1, y, z))
        {
            for (int i = y; i < Chunk.CHUNK_SIZE; i++)
            {
                //If we reach an empty block, end the run
                if (DifferentVoxel(new Vector3Int(x, i, z), voxel))
                    break;

                visitedXP[PositionToIndex(x, i, z)] = true;

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

        //FRONT (Z-)
        if (!visitedZN[PositionToIndex(x, y, z)] && VisibleFaceZN(x, y, z - 1))
        {
            for (int i = y; i < Chunk.CHUNK_SIZE; i++)
            {
                //If we reach an empty block, end the run
                if (DifferentVoxel(new Vector3Int(x, i, z), voxel))
                    break;

                visitedZN[PositionToIndex(x, i, z)] = true;

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

        //BACK (Z+)
        if (!visitedZP[PositionToIndex(x, y, z)] && VisibleFaceZP(x, y, z + 1))
        {
            for (int i = y; i < Chunk.CHUNK_SIZE; i++)
            {
                //If we reach an empty block, end the run
                if (DifferentVoxel(new Vector3Int(x, i, z), voxel))
                    break;

                visitedZP[PositionToIndex(x, i, z)] = true;

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

        //BOTTOM (Y-)
        if (!visitedYN[PositionToIndex(x, y, z)] && VisibleFaceYN(x, y - 1, z))
        {
            for (int i = z; i < Chunk.CHUNK_SIZE; i++)
            {
                //If we reach an empty block, end the run
                if (DifferentVoxel(new Vector3Int(x, y, i), voxel))
                    break;

                visitedYN[PositionToIndex(x, y, i)] = true;

                length++;
            }

            if (length > 0)
            {
                AppendQuad(
                    new Vector3(x, y, z),
                    new Vector3(x, y, z),
                    new Vector3(x, y, z + length),
                    new Vector3(x, y, z + length),
                    Direction.BOTTOM);
            }
        }

        length = 0;

        //TOP (Y+)
        if (!visitedYP[PositionToIndex(x, y, z)] && VisibleFaceYP(x, y + 1, z))
        {
            for (int i = z; i < Chunk.CHUNK_SIZE; i++)
            {
                //If we reach an empty block, end the run
                if (DifferentVoxel(new Vector3Int(x, y, i), voxel))
                    break;

                visitedYP[PositionToIndex(x, y, i)] = true;

                length++;
            }

            if (length > 0)
            {
                AppendQuad(
                    new Vector3(x, y, z + length),
                    new Vector3(x, y, z + length),
                    new Vector3(x, y, z),
                    new Vector3(x, y, z),
                    Direction.TOP);
            }
        }
        
    }

    private int PositionToIndex(int x, int y, int z)
    {
        return x + Chunk.CHUNK_SIZE * (y + Chunk.CHUNK_SIZE * z);
    }

    private void AppendQuad(Vector3 tl, Vector3 tr, Vector3 bl, Vector3 br, Direction direction)
    {
        vertices.Add(tl + VoxelMeshData.vertices[(int)direction][0]);
        vertices.Add(tr + VoxelMeshData.vertices[(int)direction][1]);
        vertices.Add(bl + VoxelMeshData.vertices[(int)direction][2]);
        vertices.Add(br + VoxelMeshData.vertices[(int)direction][3]);
    }

    bool DifferentVoxel(Vector3Int access, Voxel current)
    {
        Voxel v = data[PositionToIndex(access.x, access.y, access.z)];

        return v.type != current.type;
    }

    private void CreateTriangles()
    {
        for (int i = 0; i < vertices.Length; i += 4)
        {
            triangles.Add(i);
            triangles.Add(i + 1);
            triangles.Add(i + 2);
            triangles.Add(i);
            triangles.Add(i + 2);
            triangles.Add(i + 3);
        }
    }

    bool VisibleFaceXN(int x, int y, int z)
    {
        if (x < 0) return true;

        return data[PositionToIndex(x, y, z)].type == 0;
    }

    bool VisibleFaceXP(int x, int y, int z)
    {
        if (x >= Chunk.CHUNK_SIZE) return true;

        return data[PositionToIndex(0, y, z)].type == 0;
    }

    bool VisibleFaceZN(int x, int y, int z)
    {
        if (z < 0) return true;

        return data[PositionToIndex(x, y, z)].type == 0;
    }

    bool VisibleFaceZP(int x, int y, int z)
    {
        if (z >= Chunk.CHUNK_SIZE) return true;

        return data[PositionToIndex(x, y, 0)].type == 0;
    }

    bool VisibleFaceYN(int x, int y, int z)
    {
        if (y < 0) return true;

        return data[PositionToIndex(x, y, z)].type == 0;
    }

    bool VisibleFaceYP(int x, int y, int z)
    {
        if (y >= Chunk.CHUNK_SIZE) return true;

        return data[PositionToIndex(x, 0, z)].type == 0;
    }
}