using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreedyChunkNew : MonoBehaviour
{
    public const int CHUNK_SIZE = 5;

    public Voxel[,,] data = new Voxel[CHUNK_SIZE, CHUNK_SIZE, CHUNK_SIZE];

    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();

    private int vertexCount = 0;

    private void Start()
    {
        //AppendQuad(new Vector3Int(0, 0, 0), new Vector3Int(1, 0, 0), new Vector3Int(1, -1, 0), new Vector3Int(0, -1, 0)); //Quad for testing

        for(int x = 0; x < CHUNK_SIZE; x++)
            for(int y = 0; y < CHUNK_SIZE; y++)
                for(int z = 0; z < CHUNK_SIZE; z++)
                {
                    data[x, y, z].type = (byte)UnityEngine.Random.Range(0, 2);
                }

        GenerateMesh();
    }

    private void CreateGreedyMesh()
    {
        //Loop through each direction (x, y, z)

        //-Z facing (Front)
        for(int x = 0; x < CHUNK_SIZE; x++)
            for(int z = 0; z < CHUNK_SIZE ; z++)
            {
                int runStart = 0;
                int length = 0;

                for (int y = 0; y < CHUNK_SIZE; y++)
                {
                    //Have we reached an empty voxel?
                    if (!IsVoxel(x, y, z))
                    {
                        //Empty voxel found so the run has finished; did it have any length?
                        if (length > 0)
                        {
                            //It has length, so we need to finalise the quad based on the length, and the position this run started at
                            AppendQuad(new Vector3Int(x, runStart + length, z - 1), new Vector3Int(x + 1, runStart + length, z - 1), new Vector3Int(x + 1, runStart, z - 1), new Vector3Int(x, runStart, z - 1));
                        }

                        length = 0;
                        runStart = y + 1;

                        continue;
                    }

                    /*
                    //Is the a voxel directly in front, if so, cull the face
                    if(IsVoxel(x, y, z - 1))
                    {
                        length = 0;
                        runStart = y + 1;

                        continue;
                    }
                    */

                    length++;
                }
            }

        //+Z facing (Back)
        for (int x = 0; x < CHUNK_SIZE; x++)
            for (int z = CHUNK_SIZE; z > 0; z--)
            {
                int runStart = 0;
                int length = 0;

                for (int y = 0; y < CHUNK_SIZE; y++)
                {
                    //Have we reached an empty voxel?
                    if (!IsVoxel(x, y, z))
                    {
                        //Empty voxel found so the run has finished; did it have any length?
                        if (length > 0)
                        {
                            //It has length, so we need to finalise the quad based on the length, and the position this run started at
                            AppendQuad(new Vector3Int(x + 1, runStart + length, z), new Vector3Int(x, runStart + length, z), new Vector3Int(x, runStart, z), new Vector3Int(x + 1, runStart, z));
                        }

                        length = 0;
                        runStart = y + 1;

                        continue;
                    }

                    length++;
                }
            }

        //-X facing (Left)
        for (int x = 0; x < CHUNK_SIZE; x++)
            for (int z = 0; z < CHUNK_SIZE; z++)
            {
                int runStart = 0;
                int length = 0;

                for (int y = 0; y < CHUNK_SIZE; y++)
                {
                    //Have we reached an empty voxel?
                    if (!IsVoxel(x, y, z))
                    {
                        //Empty voxel found so the run has finished; did it have any length?
                        if (length > 0)
                        {
                            //It has length, so we need to finalise the quad based on the length, and the position this run started at
                            AppendQuad(new Vector3Int(x - 1, runStart + length, z + 1), new Vector3Int(x - 1, runStart + length, z), new Vector3Int(x - 1, runStart, z), new Vector3Int(x - 1, runStart, z + 1));
                        }

                        length = 0;
                        runStart = y + 1;

                        continue;
                    }

                    length++;
                }
            }

        //+X facing (Right)
        for (int x = CHUNK_SIZE; x > 0; x--)
            for (int z = 0; z < CHUNK_SIZE; z++)
            {
                int runStart = 0;
                int length = 0;

                for (int y = 0; y < CHUNK_SIZE; y++)
                {
                    //Have we reached an empty voxel?
                    if (!IsVoxel(x, y, z))
                    {
                        //Empty voxel found so the run has finished; did it have any length?
                        if (length > 0)
                        {
                            //It has length, so we need to finalise the quad based on the length, and the position this run started at
                            AppendQuad(new Vector3Int(x, runStart + length, z), new Vector3Int(x, runStart + length, z + 1), new Vector3Int(x, runStart, z + 1), new Vector3Int(x, runStart, z));
                        }

                        length = 0;
                        runStart = y + 1;

                        continue;
                    }

                    length++;
                }
            }
    }

    private void AppendQuad(Vector3Int tl, Vector3Int tr, Vector3Int bl, Vector3Int br)
    {
        //Add Quad
        vertices.Add(tl);
        vertices.Add(tr);
        vertices.Add(bl);
        vertices.Add(br);

        //Create Triangle
        triangles.AddRange(new int[6]
        {
            vertexCount, //Triangle 1
            vertexCount + 1,
            vertexCount + 2,
            vertexCount, //Triangle 2
            vertexCount + 2,
            vertexCount + 3,
        });

        vertexCount += 4;
    }

    private void GenerateMesh()
    {
        DateTime startTime = DateTime.Now;

        Mesh mesh = new Mesh();
        mesh.Clear();

        //Generate vertices and triangles
        CreateGreedyMesh();

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);

        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;

        //Debug timing
        DateTime currentTime = DateTime.Now;
        TimeSpan duration = currentTime.Subtract(startTime);

        Debug.Log("Vertices: " + vertices.Count.ToString());
        Debug.Log("Triangles: " + (triangles.Count / 3).ToString());
        Debug.Log("Chunk built in " + duration.Milliseconds.ToString() + "ms");
    }

    private bool IsVoxel(int x, int y, int z)
    {
        if (x < 0 || x == CHUNK_SIZE || y < 0 || y == CHUNK_SIZE || z < 0 || z == CHUNK_SIZE)
            return false;

        return data[x, y, z].type != 0;
    }
}
