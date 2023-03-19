using System;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public float BaseHeight = 8;
    public NoiseOctaveSettings[] Octaves;
    public NoiseOctaveSettings DomainWarp;
    
    [System.Serializable]
    public class NoiseOctaveSettings
    {
        public FastNoiseLite.NoiseType NoiseType;
        public float Frequency = 0.2f;
        public float Amplitude = 1;
    }

    private FastNoiseLite[] _octaveNoises;
    private FastNoiseLite _warpNoise;
    
    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        _octaveNoises = new FastNoiseLite[Octaves.Length];
        for (int i = 0; i < Octaves.Length; i++)
        {
            _octaveNoises[i] = new FastNoiseLite();
            _octaveNoises[i].SetNoiseType(Octaves[i].NoiseType);
            _octaveNoises[i].SetFrequency(Octaves[i].Frequency);
        }

        _warpNoise = new FastNoiseLite();
        _warpNoise.SetNoiseType(DomainWarp.NoiseType);
        _warpNoise.SetFrequency(DomainWarp.Frequency);
        _warpNoise.SetDomainWarpAmp(DomainWarp.Amplitude);
    }

    public BlockType[,,] GenerateTerrain(float xOffset, float zOffset)
    {
        BlockType[,,] result = new BlockType[ChunkRenderer.ChunkWidth, ChunkRenderer.ChunkHeight, ChunkRenderer.ChunkWidth];
        
        for (int x = 0; x < ChunkRenderer.ChunkWidth; x++)
        {
            for (int z = 0; z < ChunkRenderer.ChunkWidth; z++)
            {
                float height = GetHeight(x * ChunkRenderer.BlockScale + xOffset, z * ChunkRenderer.BlockScale + zOffset);
                float grassLayerHight = 1;
                float bedrockLayerHight = 0.5f;
                
                for (int y = 0; y < height / ChunkRenderer.BlockScale; y++)
                {
                    if (height - y * ChunkRenderer.BlockScale < grassLayerHight)
                    {
                        result[x, y, z] = BlockType.Grass;
                    }
                    else if(y * ChunkRenderer.BlockScale < bedrockLayerHight)
                    {
                        result[x, y, z] = BlockType.Wood;
                    }
                    else
                    {
                        result[x, y, z] = BlockType.Stone;
                    }
                }
                
            }
        }

        return result;
    }

    private float GetHeight(float x, float y)
    {
        _warpNoise.DomainWarp(ref x, ref y);
        
        float result = BaseHeight;

        for (int i = 0; i < Octaves.Length; i++)
        {
           float noise = _octaveNoises[i].GetNoise(x, y);
           result += noise * Octaves[i].Amplitude / 2;
        }
        
        return result;
    }
}
