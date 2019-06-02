using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class IcebergGeneration : MonoBehaviour {

	[Serializable]
	public class Count {
		public int maximum;
		public int minimum;

		public Count(int min, int max) {
			maximum = max;
			minimum = min;

		}
	}


	public int columns = 150;
	public int rows = 150;
	public float probaility;
	public GameObject[] iceRocksTiles = new GameObject[1];
	public GameObject[] icePlatformTiles = new GameObject[1];
	public GameObject[] waterPlatformTiles = new GameObject[1];
	private List<Vector3> gridPositions = new List<Vector3>();
	private Transform boardHolder;
	// Start is called before the first frame update

	void Start() {
		probaility = Random.Range(0.25f, 0.65f);
		icePlatformTiles[0] = Resources.Load("Prefabs/Arena/IcePlatform") as GameObject;
		waterPlatformTiles[0] = Resources.Load("Prefabs/Arena/WaterPlatform") as GameObject;
		iceRocksTiles[0] = Resources.Load("Prefabs/Arena/Cactus02") as GameObject;
		setIceBlock();
		FindObjectOfType<ArenaManager>().arenaReady = true;
	}

	void InitialiseList() {
		//Clear our list gridPositions.
		gridPositions.Clear();

		//Loop through x axis (columns).
		for (int x = 0; x < columns - 1; x++) {
			//Within each column, loop through y axis (rows).
			for (int y = 0; y < rows - 1; y++) {
				//At each index add a new Vector3 to our list with the x and y coordinates of that position.
				gridPositions.Add(new Vector3(x, y, 0f));
			}
		}
	}

	/*void setIceBlock()
    {
        boardHolder = new GameObject("Board").transform;
        for (int x = 0; x < columns + 1; x++)
        {
            for (int z = 0; z < rows + 1; z++)
            {
                GameObject toInstance; 
                if (x == 0 || x == columns || z == 0 || z == rows)
                {
                    float probabilityIce = Random.RandomRange(0, 1);
                    if( 0f < probabilityIce || probabilityIce < probaility)
                    {
                        toInstance = icePlatformTiles[0];
                        (Instantiate(toInstance, new Vector3(x, 0f, z), Quaternion.identity) as GameObject).transform.SetParent(boardHolder);                      
                    }
                }
                else if((x >= 1 && z >= 1) && (x <= 20 && z<= 20) )
                {
                    float probabilityIce = Random.RandomRange(0, 1);
                    if (0f < probabilityIce || probabilityIce < 1-probaility)
                    {
                        toInstance = icePlatformTiles[0];
                        (Instantiate(toInstance, new Vector3(x, 0f, z), Quaternion.identity) as GameObject).transform.SetParent(boardHolder);
                    }
                }
                else
                {
                    toInstance = icePlatformTiles[0];
                    (Instantiate(toInstance, new Vector3(x, 0f, z), Quaternion.identity) as GameObject).transform.SetParent(boardHolder);
                }                    
            }

        }
    }*/

	void setIceBlock() {
		(Instantiate(Resources.Load("Prefabs/Arena/DishOfWater") as GameObject, new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject).transform.SetParent(boardHolder);
		boardHolder = new GameObject("Board").transform;
		for (int x = 0; x < columns; x += 2) {
			for (int z = 0; z < rows; z += 2) {
				GameObject toInstance;
				if (x == columns - 1 || z == rows - 1 || ((x >= -38 && z >= -38) && (x <= -22 && z <= -2) || ((x <= 38 && z <= 38) && (x >= 22 && z >= 22)) || ((x >= -38 && z <= 38) && (x <= -22 && z >= 22)) || ((x <= 38 && z >= -38) && (x >= 22 && z <= -22)))) {
					float probabilityIce = Random.RandomRange(0f, 1f);

					if (0f < probabilityIce && probabilityIce < probaility) {
						toInstance = icePlatformTiles[0];
						(Instantiate(toInstance, new Vector3(x, 0f, z), Quaternion.identity) as GameObject).transform.SetParent(boardHolder);
					}
				} else if (((x >= -15 && z >= -15) && (x <= -7 && z <= -7)) || ((x > 12 && z > 12) && (x <= 15 && z <= 15)) || ((x >= -15 && z > 12) && (x <= -7 && z <= 15)) || ((x > 12 && z >= -15) && (x <= 15 && z <= -7))) {

					float probabilityIce = Random.RandomRange(0f, 1f);

					if ((0f <= probabilityIce && probabilityIce <= probaility) || probabilityIce >= 1 - probaility) {
						toInstance = icePlatformTiles[0];
						(Instantiate(toInstance, new Vector3(x, 0f, z), Quaternion.identity) as GameObject).transform.SetParent(boardHolder);
					}
				} else {
					toInstance = icePlatformTiles[0];
					(Instantiate(toInstance, new Vector3(x, 0f, z), Quaternion.identity) as GameObject).transform.SetParent(boardHolder);
				}
			}

		}
	}

	Vector3 RandomPosition() {
		int randomIndex = Random.Range(0, gridPositions.Count);
		Vector3 randomPosition = gridPositions[randomIndex];
		gridPositions.RemoveAt(randomIndex);
		return randomPosition;
	}

	void LayoutObjectAtRandom(GameObject[] tileArray, int min, int max) {
		int objectCount = Random.Range(min, max + 1);

		for (int i = 0; i < objectCount; i++) {
			Vector3 randomPosition = RandomPosition();
			GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
			Instantiate(tileChoice, randomPosition, Quaternion.identity);
		}
	}




}
