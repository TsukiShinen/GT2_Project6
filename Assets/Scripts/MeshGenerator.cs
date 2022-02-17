using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator
{
    public static MeshData GenerateMesh(int width, int height)
    {
        MeshData meshData = new MeshData(width, height);
        int vertexIndex = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                meshData.m_vertices[vertexIndex] = new Vector3(x, y, 0);
                meshData.m_uvs[vertexIndex] = new Vector2(x/(float)(width-1), y/(float)(height-1));

                if (x < width - 1 && y < height - 1) {
                    meshData.AddTriangle(vertexIndex, vertexIndex + width + 1, vertexIndex + width);
                    meshData.AddTriangle(vertexIndex + width + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }

        return meshData;
    }
}

public class MeshData
{
    public Vector3[] m_vertices;
    public int[] m_triangles;
    public Vector2[] m_uvs;

    int _triangleIndex;

    public MeshData(int meshWidth, int meshHeight)
    {
        m_vertices = new Vector3[meshWidth * meshHeight];
        m_uvs = new Vector2[meshWidth * meshHeight];
        m_triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
    }

    public void AddTriangle(int a, int b, int c)
    {
        m_triangles[_triangleIndex] = c;
        m_triangles[_triangleIndex+1] = b;
        m_triangles[_triangleIndex+2] = a;
        _triangleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = m_vertices;
        mesh.triangles = m_triangles;
        mesh.uv = m_uvs;
        mesh.RecalculateNormals();
        return mesh;
    }
}