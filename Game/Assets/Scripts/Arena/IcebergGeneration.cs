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
	public GameObject[] icePlatformTilesInternal = new GameObject[1];
    public GameObject[] icePlatformTilesMedium = new GameObject[1];
    public GameObject[] icePlatformTilesExternal = new GameObject[2];
    public GameObject[] waterPlatformTiles = new GameObject[1];
    public GameObject[] icePlatformSide = new GameObject[1];
    public GameObject[] icePlatformAngle = new GameObject[1];
	private List<Vector3> gridPositions = new List<Vector3>();
	private Transform boardHolder;
	// Start is called before the first frame update

	void Start() {
		probaility = Random.Range(0.25f, 0.60f);
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
		(Instantiate(waterPlatformTiles[0], new Vector3(rows / 2f, 0f, columns / 2f), Quaternion.identity) as GameObject).transform.SetParent(boardHolder);
		boardHolder = new GameObject("Board").transform;
		for (int x = 0; x < columns; x += 5) {
			for (int z = 0; z < rows; z += 5) {
				GameObject toInstance;
                if ((x == 0 || z == 0) || (x == columns || z == rows) || (x < 40 || x > 110 || z < 40 || z > 110))
                {
                    float probabilityIce = Random.RandomRange(0f, 1f);

                    if (0f < probabilityIce && probabilityIce < probaility)
                    {
                        toInstance = icePlatformTilesExternal[Random.Range(0, 2)];
                        //(Instantiate(toInstance, new Vector3(x, 0f, z), Quaternion.identity) as GameObject).transform.SetParent(boardHolder);
                        (Instantiate(toInstance, new Vector3(x, 0f, z), Quaternion.EulerAngles(0, Random.RandomRange(0, 360), 0)) as GameObject).transform.SetParent(boardHolder);
                    }
                }
                else if (x < 50 || x > 100 || z < 50 || z > 100)
                {

                    float probabilityIce = Random.RandomRange(0f, 1f);

                    if (probabilityIce >= 1 - probaility)//(0f <= probabilityIce && probabilityIce <= probaility) || 
                    {
                        toInstance = icePlatformTilesMedium[0];
                        //(Instantiate(toInstance, new Vector3(x, 0f, z), Quaternion.identity) as GameObject).transform.SetParent(boardHolder);
                        (Instantiate(toInstance, new Vector3(x, 0f, z), Quaternion.EulerAngles(0, Random.RandomRange(0, 360), 0)) as GameObject).transform.SetParent(boardHolder);
                    }
                }
                else if (x == 50 || z == 50 || x == 100 || z == 100)
                {
                    if(x==50 && z == 50)
                    {
                        toInstance = icePlatformAngle[0];
                        (Instantiate(toInstance, new Vector3(x, 0f, z), Quaternion.EulerAngles(0, 0, 0)) as GameObject).transform.SetParent(boardHolder);
                    }
                    else if (x == 50 && z == 100)
                    {
                        toInstance = icePlatformAngle[0];
                        (Instantiate(toInstance, new Vector3(x, 0f, z), Quaternion.EulerAngles(0, 90, 0)) as GameObject).transform.SetParent(boardHolder);
                    }
                    else if (x == 100 && z == 50)
                    {
                        toInstance = icePlatformAngle[0];
                        (Instantiate(toInstance, new Vector3(x, 0f, z), Quaternion.EulerAngles(0, -90, 0)) as GameObject).transform.SetParent(boardHolder);
                    }
                    else if (x==100 && z==100)
                    {
                        toInstance = icePlatformAngle[0];
                        (Instantiate(toInstance, new Vector3(x, 0f, z), Quaternion.EulerAngles(0, -180, 0)) as GameObject).transform.SetParent(boardHolder);
                    }else if (x == 50 && z>50)
                    {
                        toInstance = icePlatformSide[0];
                        (Instantiate(toInstance, new Vector3(x, 0f, z), Quaternion.EulerAngles(0, 0, 0)) as GameObject).transform.SetParent(boardHolder);
                    }
                    else if (x == 100 && z > 50)
                    {
                        toInstance = icePlatformAngle[0];
                        (Instantiate(toInstance, new Vector3(x, 0f, z), Quaternion.EulerAngles(0, 180, 0)) as GameObject).transform.SetParent(boardHolder);
                    }
                    else if (x > 50 && z == 50)
                    {
                        toInstance = icePlatformAngle[0];
                        (Instantiate(toInstance, new Vector3(x, 0f, z), Quaternion.EulerAngles(0, -90, 0)) as GameObject).transform.SetParent(boardHolder);
                    }
                    else if (x > 50 && z == 100)
                    {
                        toInstance = icePlatformAngle[0];
                        (Instantiate(toInstance, new Vector3(x, 0f, z), Quaternion.EulerAngles(0, 90, 0)) as GameObject).transform.SetParent(boardHolder);
                    }
                }
                else
                {
                    toInstance = icePlatformTilesInternal[0];
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
