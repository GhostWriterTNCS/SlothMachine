
using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using UnityEngine;

public class PerlinNoise : MonoBehaviour
{

    [Serializable]
    public class Count
    {
        public int maximum;
        public int minimum;

        public Count(int min, int max)
        {
            maximum = max;
            minimum = min;

        }
    }

    public int dept = 90;
    public int width = 513;
    public int height = 513;

    public Count cactusCount = new Count(160, 350);
    public Count rockCount = new Count(50, 130);
    public GameObject[] cactusTiles;
    public GameObject[] rockTiles;
    public float scale ;
    private List<Vector3> gridPositions = new List<Vector3>(); //for position in the map
    private Transform boardHolder;
    public GameObject ArenaSphere;

    public void Start()
    {
        scale = Random.Range(2.6f, 4.8f);
        
        Terrain terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);

        SpawnObject(terrain.terrainData);
        ArenaSphere = Resources.Load("Prefabs/ArenaSphere") as GameObject;
        spawnArenaSphere();

    }

    TerrainData GenerateTerrain(TerrainData terrainData)
    {
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, dept, height);

        terrainData.SetHeights(0, 0, GenerateHeights());
        return terrainData;
    }

    float [,] GenerateHeights()
    {
        float[,] heights = new float[width, height];

        for(int x=0; x<width; x++)
        {
            for (int y=0; y<height; y++)
            {
                heights[x, y] = CalculateHeight(x, y);
            }
        }
        return heights;
    }

    float CalculateHeight(int x, int y)
    {
        float xCoord = (float)x / width * scale;
        float yCoord = (float)y / height * scale;
        return Mathf.PerlinNoise(xCoord, yCoord);
    }

    public void SpawnObject(TerrainData terrainData)
    {
        gridPositions.Clear();
        for (int x = 0; x < width-1; x++)
        {
            //Within each column, loop through y axis (rows).
            for (int y = 1; y < height-1; y++)
            {
                //At each index add a new Vector3 to our list with the x, y, z coordinates of that position.
                gridPositions.Add(new Vector3(x, terrainData.GetHeight(x,y), y));
            }
        }

        LayoutObjectAtRandom(cactusTiles, cactusCount.minimum, cactusCount.maximum);
        LayoutObjectAtRandom(rockTiles, rockCount.minimum, rockCount.maximum);
    }

    void LayoutObjectAtRandom(GameObject[] tileArray, int min, int max)
    {
        int objectCount = Random.Range(min, max + 1);

        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
            
        }
    }

    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }


    public void spawnArenaSphere()
    {
        Instantiate(ArenaSphere, new Vector3(width/2, GetComponent<Terrain>().terrainData.GetHeight(width / 2, height / 2), height/2), Quaternion.identity);
    }
    
}
