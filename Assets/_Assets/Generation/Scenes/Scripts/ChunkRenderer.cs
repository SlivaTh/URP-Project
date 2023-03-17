using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ChunkRenderer : MonoBehaviour
{
    private const int ChunkWidth = 10;
    private const int ChunkHeight = 128;
    
    public int[,,] Blocks = new int[ChunkWidth, ChunkHeight, ChunkWidth];

    private List<Vector3> _vertices = new List<Vector3>();
    private List<int> _triangles = new List<int>();

    private void Start()
    {
        Mesh chunkMesh = new Mesh();

        Blocks[0, 0, 0] = 1;

        for (int y = 0; y < ChunkHeight; y++)
        {
            for (int x = 0; x < ChunkWidth; x++)
            {
                for (int z = 0; z < ChunkWidth; z++)
                {
                    GenerateBlock(x,y,z);
                }
            }
        }

        chunkMesh.RecalculateNormals();
        chunkMesh.RecalculateBounds();

        chunkMesh.vertices = _vertices.ToArray();
        chunkMesh.triangles = _triangles.ToArray();
        
        GetComponent<MeshFilter>().mesh = chunkMesh;
    }

    private void GenerateBlock(int x, int y, int z)
    {
        if (Blocks[x, y, z] == 0)
        {
            return;
        }

        Vector3Int blockPosition = new Vector3Int(x, y, z);
        GenerateRightSide(blockPosition);
        GenerateLeftSide(blockPosition);
        GenerateFrontSide(blockPosition);
        GenerateTopSide(blockPosition);
        GenerateBottomSide(blockPosition);
        GenerateBackSide(blockPosition);
    }

    private void GenerateRightSide(Vector3Int blockPosition)
    {
        _vertices.Add(new Vector3(0, 0, 0) + blockPosition);
        _vertices.Add(new Vector3(0, 0, 1) + blockPosition);
        _vertices.Add(new Vector3(0, 1, 0) + blockPosition);
        _vertices.Add(new Vector3(0, 1, 1) + blockPosition);

        AddLastVerticesSquare();
    }
    
    private void GenerateLeftSide(Vector3Int blockPosition)
    {
        _vertices.Add(new Vector3(1, 0, 0) + blockPosition);
        _vertices.Add(new Vector3(1, 1, 0) + blockPosition);
        _vertices.Add(new Vector3(1, 0, 1) + blockPosition);
        _vertices.Add(new Vector3(1, 1, 1) + blockPosition);

        AddLastVerticesSquare();
    }
    
    private void GenerateFrontSide(Vector3Int blockPosition)
    {
        _vertices.Add(new Vector3(0, 0, 1) + blockPosition);
        _vertices.Add(new Vector3(1, 0, 1) + blockPosition);
        _vertices.Add(new Vector3(0, 1, 1) + blockPosition);
        _vertices.Add(new Vector3(1, 1, 1) + blockPosition);

        AddLastVerticesSquare();
    }
    
    private void GenerateBackSide(Vector3Int blockPosition)
    {
        _vertices.Add(new Vector3(0, 0, 0) + blockPosition);
        _vertices.Add(new Vector3(0, 1, 0) + blockPosition);
        _vertices.Add(new Vector3(1, 0, 0) + blockPosition);
        _vertices.Add(new Vector3(1, 1, 0) + blockPosition);

        AddLastVerticesSquare();
    }
    
    private void GenerateTopSide(Vector3Int blockPosition)
    {
        _vertices.Add(new Vector3(0, 1, 1) + blockPosition);
        _vertices.Add(new Vector3(1, 1, 1) + blockPosition);
        _vertices.Add(new Vector3(0, 1, 0) + blockPosition);
        _vertices.Add(new Vector3(1, 1, 0) + blockPosition);

        AddLastVerticesSquare();
    }
    
    private void GenerateBottomSide(Vector3Int blockPosition)
    {
        _vertices.Add(new Vector3(1, 0, 1) + blockPosition);
        _vertices.Add(new Vector3(0, 0, 1) + blockPosition);
        _vertices.Add(new Vector3(1, 0, 0) + blockPosition);
        _vertices.Add(new Vector3(0, 0, 0) + blockPosition);

        AddLastVerticesSquare();
    }

    private void AddLastVerticesSquare()
    {
        _triangles.Add(_vertices.Count - 4);
        _triangles.Add(_vertices.Count - 3);
        _triangles.Add(_vertices.Count - 2);

        _triangles.Add(_vertices.Count - 3);
        _triangles.Add(_vertices.Count - 1);
        _triangles.Add(_vertices.Count - 2);
    }
}
