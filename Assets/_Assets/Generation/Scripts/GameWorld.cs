using System.Collections.Generic;
using UnityEngine;

public class GameWorld : MonoBehaviour
{
    public Dictionary<Vector2Int, ChunkData> ChunkDatas = new Dictionary<Vector2Int, ChunkData>();

    public ChunkRenderer chunkPrefab;

    private void Start()
    {
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                float xPos = x * ChunkRenderer.ChunkWidth * ChunkRenderer.BlockScale;
                float zPos = y * ChunkRenderer.ChunkWidth * ChunkRenderer.BlockScale;
                
                ChunkData chunkData = new ChunkData();
                chunkData.ChunkPosition = new Vector2Int(x, y);
                chunkData.Blocks = TerrainGenerator.GenerateTerrain(xPos, zPos);
                ChunkDatas.Add(new Vector2Int(x,y), chunkData);
                
                var chunk = Instantiate(chunkPrefab, new Vector3(xPos, 0f, zPos), Quaternion.identity, transform);
                chunk.ChunkData = chunkData;
                chunk.ParentGameWorld = this;
            }
        }
    }
}
