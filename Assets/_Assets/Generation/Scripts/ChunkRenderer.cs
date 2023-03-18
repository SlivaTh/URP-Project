using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ChunkRenderer : MonoBehaviour
{
    public const int ChunkWidth = 10;
    public const int ChunkHeight = 128;
    public const float BlockScale = 1f;
    
    public ChunkData ChunkData;
    public GameWorld ParentGameWorld;

    private List<Vector3> _vertices = new List<Vector3>();
    private List<int> _triangles = new List<int>();

    private void Start()
    {
        Mesh chunkMesh = new Mesh();
        
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
        
        chunkMesh.Optimize();

        chunkMesh.RecalculateNormals();
        chunkMesh.RecalculateBounds();

        chunkMesh.vertices = _vertices.ToArray();
        chunkMesh.triangles = _triangles.ToArray();
        
        GetComponent<MeshFilter>().mesh = chunkMesh;
        GetComponent<MeshCollider>().sharedMesh = chunkMesh;
    }

    private void GenerateBlock(int x, int y, int z)
    {
        Vector3Int blockPosition = new Vector3Int(x, y, z);
        
        if (GetBlockAtPosition(blockPosition) == 0)
        {
            return;
        }
        
        if(GetBlockAtPosition(blockPosition + Vector3Int.right) == 0) GenerateRightSide(blockPosition);
        if(GetBlockAtPosition(blockPosition + Vector3Int.left) == 0) GenerateLeftSide(blockPosition);
        if(GetBlockAtPosition(blockPosition + Vector3Int.forward) == 0) GenerateFrontSide(blockPosition);
        if(GetBlockAtPosition(blockPosition + Vector3Int.back) == 0) GenerateBackSide(blockPosition);
        if(GetBlockAtPosition(blockPosition + Vector3Int.up) == 0) GenerateTopSide(blockPosition);
        if (GetBlockAtPosition(blockPosition + Vector3Int.down) == 0) GenerateBottomSide(blockPosition);
    }

    private BlockType GetBlockAtPosition(Vector3Int blockPosition)
    {
        if (blockPosition.x >= 0 && blockPosition.x < ChunkWidth &&
            blockPosition.y >= 0 && blockPosition.y < ChunkHeight &&
            blockPosition.z >= 0 && blockPosition.z < ChunkWidth)
        {
            return ChunkData.Blocks[blockPosition.x, blockPosition.y, blockPosition.z];
        }
        else
        {
            if (blockPosition.y < 0 || blockPosition.y >= ChunkHeight)
            {
                return BlockType.Air;
            }
            
            Vector2Int adjacentChunkPosition = ChunkData.ChunkPosition;
            
            if (blockPosition.x < 0)
            {
                adjacentChunkPosition.x--;
                blockPosition.x += ChunkWidth;
            }
            else if (blockPosition.x >= ChunkWidth)
            {
                adjacentChunkPosition.x++;
                blockPosition.x -= ChunkWidth;
            }
            
            if (blockPosition.z < 0)
            {
                adjacentChunkPosition.y--;
                blockPosition.z += ChunkWidth;
            }
            else if (blockPosition.z >= ChunkWidth)
            {
                adjacentChunkPosition.y++;
                blockPosition.z -= ChunkWidth;
            }

            if (ParentGameWorld.ChunkDatas.TryGetValue(adjacentChunkPosition, out ChunkData adjacentChunk))
            {
                return adjacentChunk.Blocks[blockPosition.x, blockPosition.y, blockPosition.z];
            }
            else
            {
                return BlockType.Air;
            }
        }
    }

    private void GenerateRightSide(Vector3Int blockPosition)
    {
        _vertices.Add((new Vector3(1, 0, 0) + blockPosition) * BlockScale);
        _vertices.Add((new Vector3(1, 1, 0) + blockPosition) * BlockScale);
        _vertices.Add((new Vector3(1, 0, 1) + blockPosition) * BlockScale);
        _vertices.Add((new Vector3(1, 1, 1) + blockPosition) * BlockScale);

        AddLastVerticesSquare();
    }
    
    private void GenerateLeftSide(Vector3Int blockPosition)
    {
        _vertices.Add((new Vector3(0, 0, 0) + blockPosition) * BlockScale);
        _vertices.Add((new Vector3(0, 0, 1) + blockPosition) * BlockScale);
        _vertices.Add((new Vector3(0, 1, 0) + blockPosition) * BlockScale);
        _vertices.Add((new Vector3(0, 1, 1) + blockPosition) * BlockScale);

        AddLastVerticesSquare();
    }
    
    private void GenerateFrontSide(Vector3Int blockPosition)
    {
        _vertices.Add((new Vector3(1, 1, 1) + blockPosition) * BlockScale);
        _vertices.Add((new Vector3(0, 1, 1) + blockPosition) * BlockScale);
        _vertices.Add((new Vector3(1, 0, 1) + blockPosition) * BlockScale);
        _vertices.Add((new Vector3(0, 0, 1) + blockPosition) * BlockScale);

        AddLastVerticesSquare();
    }
    
    private void GenerateBackSide(Vector3Int blockPosition)
    {
        _vertices.Add((new Vector3(0, 0, 0) + blockPosition) * BlockScale);
        _vertices.Add((new Vector3(0, 1, 0) + blockPosition) * BlockScale);
        _vertices.Add((new Vector3(1, 0, 0) + blockPosition) * BlockScale);
        _vertices.Add((new Vector3(1, 1, 0) + blockPosition) * BlockScale);

        AddLastVerticesSquare();
    }
    
    private void GenerateTopSide(Vector3Int blockPosition)
    {
        _vertices.Add((new Vector3(0, 1, 0) + blockPosition) * BlockScale);
        _vertices.Add((new Vector3(0, 1, 1) + blockPosition) * BlockScale);
        _vertices.Add((new Vector3(1, 1, 0) + blockPosition) * BlockScale);
        _vertices.Add((new Vector3(1, 1, 1) + blockPosition) * BlockScale);

        AddLastVerticesSquare();
    }
    
    private void GenerateBottomSide(Vector3Int blockPosition)
    {
        _vertices.Add((new Vector3(1, 0, 1) + blockPosition) * BlockScale);
        _vertices.Add((new Vector3(0, 0, 1) + blockPosition) * BlockScale);
        _vertices.Add((new Vector3(1, 0, 0) + blockPosition) * BlockScale);
        _vertices.Add((new Vector3(0, 0, 0) + blockPosition) * BlockScale);

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
