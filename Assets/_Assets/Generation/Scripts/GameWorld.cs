using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWorld : MonoBehaviour
{
    private const int ViewRadius = 5;
    
    public Dictionary<Vector2Int, ChunkData> ChunkDatas = new Dictionary<Vector2Int, ChunkData>();
    
    [Header("References")]
    public ChunkRenderer chunkPrefab;
    public TerrainGenerator terrainGenerator;

    private Camera _mainCamera;
    private Transform _mainCameraTransform;
    private Vector2Int _currentPlayerChunk;

    private void Start()
    {
        _mainCamera = Camera.main;
        if (_mainCamera != null)
        {
            _mainCameraTransform = _mainCamera.transform;
        }

        StartCoroutine(Generate(false));
    }

    private void Update()
    {
        Vector3Int playerWorldPos = Vector3Int.FloorToInt(_mainCameraTransform.position / ChunkRenderer.BlockScale);
        Vector2Int playerChunk = GetChunkContainingBlock(playerWorldPos);
        if (playerChunk != _currentPlayerChunk)
        {
            _currentPlayerChunk = playerChunk;
            StartCoroutine(Generate(true));
        }
        
        CheckInput();
    }

    private IEnumerator Generate(bool wait)
    {
        for (int x = _currentPlayerChunk.x - ViewRadius; x < _currentPlayerChunk.x + ViewRadius; x++)
        {
            for (int y = _currentPlayerChunk.y - ViewRadius; y < _currentPlayerChunk.y + ViewRadius; y++)
            {
                Vector2Int chunkPosition = new Vector2Int(x, y);
                
                if (ChunkDatas.ContainsKey(chunkPosition))
                {
                    continue;
                }
                
                LoadChunk(chunkPosition);

                if (wait)
                {
                    yield return new WaitForSecondsRealtime(0.2f);
                }
            }
        }
    }
    
    [ContextMenu("Regenerate World")]
    public void Regenerate()
    {
        terrainGenerator.Init();
        
        foreach (var chunkData in ChunkDatas)
        {
            Destroy(chunkData.Value.Renderer.gameObject);
        }
        ChunkDatas.Clear();

        StartCoroutine(Generate(false));
    }

    private void LoadChunk(Vector2Int chunkPosition)
    {
        float xPos = chunkPosition.x * ChunkRenderer.ChunkWidth * ChunkRenderer.BlockScale;
        float zPos = chunkPosition.y * ChunkRenderer.ChunkWidth * ChunkRenderer.BlockScale;
                
        ChunkData chunkData = new ChunkData();
        chunkData.ChunkPosition = chunkPosition;
        chunkData.Blocks = terrainGenerator.GenerateTerrain(xPos, zPos);
        ChunkDatas.Add(chunkPosition, chunkData);
                
        var chunk = Instantiate(chunkPrefab, new Vector3(xPos, 0f, zPos), Quaternion.identity, transform);
        chunk.ChunkData = chunkData;
        chunk.ParentGameWorld = this;

        chunkData.Renderer = chunk;
    }

    private void CheckInput()
    {
        if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(0))
        {
            bool isDestroying = Input.GetMouseButtonDown(0);
            
            Ray ray = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

            if (Physics.Raycast(ray, out var hitInfo))
            {
                Vector3 blockCenter;
                if (isDestroying)
                {
                    blockCenter = hitInfo.point - hitInfo.normal * ChunkRenderer.BlockScale / 2;
                }
                else
                {
                    blockCenter = hitInfo.point + hitInfo.normal * ChunkRenderer.BlockScale / 2;
                }
                Vector3Int blockWorldPos = Vector3Int.FloorToInt(blockCenter / ChunkRenderer.BlockScale);
                Vector2Int chunkPos = GetChunkContainingBlock(blockWorldPos);
                if (ChunkDatas.TryGetValue(chunkPos, out ChunkData chunkData))
                {
                    Vector3Int chunkOrigin = new Vector3Int(chunkPos.x, 0, chunkPos.y) * ChunkRenderer.ChunkWidth;
                    if (isDestroying)
                    {
                        chunkData.Renderer.DestroyBlock(blockWorldPos - chunkOrigin);
                    }
                    else
                    {
                        chunkData.Renderer.SpawnBlock(blockWorldPos - chunkOrigin);
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public Vector2Int GetChunkContainingBlock(Vector3Int blockWorldPos)
    {
        Vector2Int chunkPosition = new Vector2Int(blockWorldPos.x / ChunkRenderer.ChunkWidth,
            blockWorldPos.z / ChunkRenderer.ChunkWidth);
        
        if (blockWorldPos.x < 0) chunkPosition.x--;
        if (blockWorldPos.z < 0) chunkPosition.y--;

        if (blockWorldPos.x > ChunkRenderer.ChunkWidth) chunkPosition.x++;
        if (blockWorldPos.z > ChunkRenderer.ChunkWidth) chunkPosition.y++;
        
        return chunkPosition;
    }
}
