using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreedyMeshAlgo : MonoBehaviour
{
    // Code ported from https://0fps.net/2012/06/30/meshing-in-a-minecraft-game/

    // Note this implementation does not support different block types or block normals
    // The original author describes how to do this here: https://0fps.net/2012/07/07/meshing-minecraft-part-2/

    const int CHUNK_SIZE = 5;

    public Voxel[,,] data = new Voxel[CHUNK_SIZE, CHUNK_SIZE, CHUNK_SIZE];

    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();

    private void Start()
    {
        //For testing only!!
        for (int y = 0; y < CHUNK_SIZE; y++)
            for (int z = 0; z < CHUNK_SIZE; z++)
                for (int x = 0; x < CHUNK_SIZE; x++)
                {
                    data[x, y, z].type = (byte)UnityEngine.Random.Range(0, 2);
                }

        GenerateMesh();
    }

    public void GenerateMesh()
    {
        DateTime startTime = DateTime.Now;

        Mesh mesh = new Mesh();
        mesh.Clear();

        GreedyMesh(); //create the mesh??
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

    private void AppendQuad(Vector3 tl, Vector3 tr, Vector3 bl, Vector3 br)
    {
        vertices.Add(tl);
        vertices.Add(tr);
        vertices.Add(bl);
        vertices.Add(br);
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

    private bool IsBlockAt(int x, int y, int z)
    {
        return (data[x, y, z].type != 0);
    }

    public void GreedyMesh()
    {
        // Sweep over each axis (X, Y and Z)
        for (var d = 0; d < 3; ++d)
        {
            int i, j, k, l, w, h;
            int u = (d + 1) % 3;
            int v = (d + 2) % 3;
            var x = new int[3];
            var q = new int[3];

            var mask = new bool[CHUNK_SIZE * CHUNK_SIZE];
            q[d] = 1;

            // Check each slice of the chunk one at a time
            for (x[d] = -1; x[d] < CHUNK_SIZE;)
            {
                // Compute the mask
                var n = 0;
                for (x[v] = 0; x[v] < CHUNK_SIZE; ++x[v])
                {
                    for (x[u] = 0; x[u] < CHUNK_SIZE; ++x[u])
                    {
                        // q determines the direction (X, Y or Z) that we are searching
                        // m.IsBlockAt(x,y,z) takes global map positions and returns true if a block exists there

                        bool blockCurrent = 0 <= x[d] ? IsBlockAt(x[0], x[1], x[2]) : true;
                        bool blockCompare = x[d] < CHUNK_SIZE - 1 ? IsBlockAt(x[0] + q[0], x[1] + q[1], x[2] + q[2]) : true;

                        // The mask is set to true if there is a visible face between two blocks,
                        //   i.e. both aren't empty and both aren't blocks
                        mask[n++] = blockCurrent != blockCompare;
                    }
                }

                ++x[d];

                n = 0;

                // Generate a mesh from the mask using lexicographic ordering,      
                //   by looping over each block in this slice of the chunk
                for (j = 0; j < CHUNK_SIZE; ++j)
                {
                    for (i = 0; i < CHUNK_SIZE;)
                    {
                        if (mask[n])
                        {
                            // Compute the width of this quad and store it in w                        
                            //   This is done by searching along the current axis until mask[n + w] is false
                            for (w = 1; i + w < CHUNK_SIZE && mask[n + w]; w++) { }

                            // Compute the height of this quad and store it in h                        
                            //   This is done by checking if every block next to this row (range 0 to w) is also part of the mask.
                            //   For example, if w is 5 we currently have a quad of dimensions 1 x 5. To reduce triangle count,
                            //   greedy meshing will attempt to expand this quad out to CHUNK_SIZE x 5, but will stop if it reaches a hole in the mask

                            var done = false;
                            for (h = 1; j + h < CHUNK_SIZE; h++)
                            {
                                // Check each block next to this quad
                                for (k = 0; k < w; ++k)
                                {
                                    // If there's a hole in the mask, exit
                                    if (!mask[n + k + h * CHUNK_SIZE])
                                    {
                                        done = true;
                                        break;
                                    }
                                }

                                if (done)
                                    break;
                            }

                            x[u] = i;
                            x[v] = j;

                            // du and dv determine the size and orientation of this face
                            var du = new int[3];
                            du[u] = w;

                            var dv = new int[3];
                            dv[v] = h;

                            // Create a quad for this face. Colour, normal or textures are not stored in this block vertex format.
                            AppendQuad(new Vector3(x[0],                 x[1],                 x[2]),                 // Top-left vertice position
                                       new Vector3(x[0] + du[0],         x[1] + du[1],         x[2] + du[2]),         // Top right vertice position
                                       new Vector3(x[0] + dv[0],         x[1] + dv[1],         x[2] + dv[2]),         // Bottom left vertice position
                                       new Vector3(x[0] + du[0] + dv[0], x[1] + du[1] + dv[1], x[2] + du[2] + dv[2])  // Bottom right vertice position
                                       );

                            // Clear this part of the mask, so we don't add duplicate faces
                            for (l = 0; l < h; ++l)
                                for (k = 0; k < w; ++k)
                                    mask[n + k + l * CHUNK_SIZE] = false;

                            // Increment counters and continue
                            i += w;
                            n += w;
                        }
                        else
                        {
                            i++;
                            n++;
                        }
                    }
                }
            }
        }
    }
}