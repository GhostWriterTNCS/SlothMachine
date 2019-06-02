using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

public class PerlinNoise : NetworkBehaviour {

	[Serializable]
	public class Count {
		public int maximum;
		public int minimum;

		public Count(int min, int max) {
			maximum = max;
			minimum = min;

		}
	}

	public int dept = 90;
	public int width = 513;
	public int height = 513;
	public float scaleMin = 1.6f;
	public float scaleMax = 3.2f;

	public Count cactusCount = new Count(160, 350);
	public Count rockCount = new Count(50, 130);
	public GameObject[] cactusTiles = new GameObject[3];
	public GameObject[] rockTiles = new GameObject[2];
	public float scale;
	private List<Vector3> gridPositions = new List<Vector3>(); //for position in the map
	private Transform boardHolder;
	public GameObject ArenaSphere;

	[SyncVar]
	public int randomSeed = 0;

	public void Start() {
		Random.seed = randomSeed;
		scale = Random.Range(scaleMin, scaleMax);

		Terrain terrain = GetComponent<Terrain>();
		terrain.terrainData = GenerateTerrain(terrain.terrainData);
		/*cactusTiles[0] = Resources.Load("Prefabs/Arena/Cactus01") as GameObject;
		cactusTiles[1] = Resources.Load("Prefabs/Arena/Cactus02") as GameObject;
		cactusTiles[2] = Resources.Load("Prefabs/Arena/Plant01") as GameObject;
		rockTiles[0] = Resources.Load("Prefabs/Arena/Rock01") as GameObject;
		rockTiles[1] = Resources.Load("Prefabs/Arena/Bone") as GameObject;
		ArenaSphere = Resources.Load("Prefabs/ArenaSphere") as GameObject;*/
		SpawnObject(terrain.terrainData);

		//spawnArenaSphere();

		foreach (NetworkStartPosition pos in FindObjectsOfType<NetworkStartPosition>()) {
			Vector3 v3 = pos.transform.position;
			pos.transform.position = new Vector3(v3.x, terrain.terrainData.GetHeight((int)v3.x, (int)v3.z) + 3.5f, v3.z);
		}
		FindObjectOfType<ArenaManager>().arenaReady = true;
		CmdFixWormPosition();

	}

	[Command]
	public void CmdFixWormPosition() {
		WormDecisionTree worm = FindObjectOfType<WormDecisionTree>();
		Vector3 v3 = worm.transform.position;
		worm.transform.position = new Vector3(v3.x, FindObjectOfType<Terrain>().terrainData.GetHeight((int)v3.x, (int)v3.z) + 3, v3.z);
	}

	TerrainData GenerateTerrain(TerrainData terrainData) {
		terrainData.heightmapResolution = width + 1;
		terrainData.size = new Vector3(width, dept, height);

		terrainData.SetHeights(0, 0, GenerateHeights());
		return terrainData;
	}

	float[,] GenerateHeights() {
		float[,] heights = new float[width, height];

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				heights[x, y] = CalculateHeight(x, y);
			}
		}
		return heights;
	}

	float CalculateHeight(int x, int y) {
		float xCoord = (float)x / width * scale;
		float yCoord = (float)y / height * scale;
		return Mathf.PerlinNoise(xCoord, yCoord);
	}

	public void SpawnObject(TerrainData terrainData) {
		gridPositions.Clear();
		for (int x = 0; x < width - 1; x++) {
			//Within each column, loop through y axis (rows).
			for (int y = 1; y < height - 1; y++) {
				//At each index add a new Vector3 to our list with the x, y, z coordinates of that position.
				gridPositions.Add(new Vector3(x, terrainData.GetHeight(x, y), y));
			}
		}

		LayoutObjectAtRandom(cactusTiles, cactusCount.minimum, cactusCount.maximum);
		LayoutObjectAtRandom(rockTiles, rockCount.minimum, rockCount.maximum);
	}

	void LayoutObjectAtRandom(GameObject[] tileArray, int min, int max) {
		int objectCount = Random.Range(min, max + 1);

		for (int i = 0; i < objectCount; i++) {
			Vector3 randomPosition = RandomPosition();
			GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
			Instantiate(tileChoice, randomPosition, Quaternion.identity, transform);

		}
	}

	Vector3 RandomPosition() {
		int randomIndex = Random.Range(0, gridPositions.Count);
		Vector3 randomPosition = gridPositions[randomIndex];
		gridPositions.RemoveAt(randomIndex);
		return randomPosition;
	}


	public void spawnArenaSphere() {
		Instantiate(ArenaSphere, new Vector3(width / 2, GetComponent<Terrain>().terrainData.GetHeight(width / 2, height / 2), height / 2), Quaternion.identity);
	}

}
