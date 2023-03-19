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

    private Mesh _chunkMesh;

    private List<Vector3> _vertices = new List<Vector3>();
    private List<Vector2> _uvs = new List<Vector2>();
    private List<int> _triangles = new List<int>();

    private void Start()
    {
        _chunkMesh = new Mesh();
        
        RegenerateMesh();

        GetComponent<MeshFilter>().mesh = _chunkMesh;
    }

    private void RegenerateMesh()
    {
        _vertices.Clear();
        _uvs.Clear();
        _triangles.Clear();
        
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

        _chunkMesh.triangles = Array.Empty<int>();
        _chunkMesh.vertices = _vertices.ToArray();
        _chunkMesh.uv = _uvs.ToArray();
        _chunkMesh.triangles = _triangles.ToArray();
        
        _chunkMesh.Optimize();
        
        _chunkMesh.RecalculateNormals();
        _chunkMesh.RecalculateBounds();
        
        GetComponent<MeshCollider>().sharedMesh = _chunkMesh;
    }

    public void SpawnBlock(Vector3Int blockPosition)
    {
        ChunkData.Blocks[blockPosition.x, blockPosition.y, blockPosition.z] = BlockType.Stone;
        RegenerateMesh();
    }
    
    public void DestroyBlock(Vector3Int blockPosition)
    {
        ChunkData.Blocks[blockPosition.x, blockPosition.y, blockPosition.z] = BlockType.Air;
        RegenerateMesh();
    }

    private void GenerateBlock(int x, int y, int z)
    {
        Vector3Int blockPosition = new Vector3Int(x, y, z);
        
        if (GetBlockAtPosition(blockPosition) == 0)
        {
            return;
        }

        if (GetBlockAtPosition(blockPosition + Vector3Int.right) == 0)
        {
            GenerateRightSide(blockPosition);
            AddUvs(GetBlockAtPosition(blockPosition));
        }

        if (GetBlockAtPosition(blockPosition + Vector3Int.left) == 0)
        {
            GenerateLeftSide(blockPosition);
            AddUvs(GetBlockAtPosition(blockPosition));
        }

        if (GetBlockAtPosition(blockPosition + Vector3Int.forward) == 0)
        {
            GenerateFrontSide(blockPosition);
            AddUvs(GetBlockAtPosition(blockPosition));
        }

        if (GetBlockAtPosition(blockPosition + Vector3Int.back) == 0)
        {
            GenerateBackSide(blockPosition);
            AddUvs(GetBlockAtPosition(blockPosition));
        }

        if (GetBlockAtPosition(blockPosition + Vector3Int.up) == 0)
        {
            GenerateTopSide(blockPosition);
            AddUvs(GetBlockAtPosition(blockPosition));
        }

        if (GetBlockAtPosition(blockPosition + Vector3Int.down) == 0)
        {
            GenerateBottomSide(blockPosition);
            AddUvs(GetBlockAtPosition(blockPosition));
        }
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

    private void AddUvs(BlockType blockType)
    {
        switch (blockType)
        {
            case BlockType.Air:
                break;
            case BlockType.Grass:
                _uvs.Add(new Vector2(64f / 256,240f / 256));
                _uvs.Add(new Vector2(64f / 256,1));
                _uvs.Add(new Vector2(80f / 256,240f / 256));
                _uvs.Add(new Vector2(80f / 256,1));
                break;
            case BlockType.Stone:
                _uvs.Add(new Vector2(16f / 256,240f / 256));
                _uvs.Add(new Vector2(16f / 256,1));
                _uvs.Add(new Vector2(32f / 256,240f / 256));
                _uvs.Add(new Vector2(32f / 256,1));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(blockType), blockType, null);
        }
    }
}
