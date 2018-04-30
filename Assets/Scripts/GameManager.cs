using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public static GameManager instance = null;
	public GameObject tilePrefab;
	public GameObject[] unitPrefabs;
	public GameObject[] units;
	public GameObject turnCanvas;
	public GameObject abilitiesCanvas;
	public Button moveButton;
	public Button actButton;
	public Button cancelButton;
	public Button confirmButton;
	public Button endTurnButton;
	public int activeUnit = 0;

	public GameObject[,] tiles;


	void Awake() {
        if (instance == null) {
            instance = this;
        }
        else if (instance != this) {
            Destroy(this);
        }

		tiles = CreateTiles();
		CreateUnits();
	}

	void Start() {
		units[activeUnit].GetComponent<Unit>().TakeTurn();
	}

	GameObject[,] CreateTiles() {

		GameObject[,] tempTiles = new GameObject[10,10];

		for (int z = 0; z < 10; z++) {
			for (int x = 0; x < 10; x++) {
				GameObject tileInstance = Instantiate(
					tilePrefab,
					new Vector3(-4.5f + x, 0, -4.5f + z),
					Quaternion.Euler(new Vector3(90, 0, 0))
				) as GameObject;

				Tile tileScript = tileInstance.GetComponent<Tile>();
				tileScript.x = x;
				tileScript.z = z;

				tempTiles[x,z] = tileInstance;
			}
		}

		for (int z = 0; z < 10; z++) {
			for (int x = 0; x < 10; x++) {
				List<GameObject> connectedTiles = tempTiles[z,x].GetComponent<Tile>().connected;
                if (z > 0)
                    connectedTiles.Add(tempTiles[z - 1, x]);
                if (x < 9)
                    connectedTiles.Add(tempTiles[z, x + 1]);
                if (z < 9)
                    connectedTiles.Add(tempTiles[z + 1, x]);
                if (x > 0)
                    connectedTiles.Add(tempTiles[z, x - 1]);
			}
		}

		return tempTiles;
	}

	void CreateUnits() {

		units = new GameObject[unitPrefabs.Length];

		for (int i = 0; i < unitPrefabs.Length; i++) {
			Unit unitScript = unitPrefabs[i].GetComponent<Unit>();
			GameObject tile = tiles[unitScript.startingX, unitScript.startingZ];
			GameObject unitInstance = Instantiate(
				unitPrefabs[i],
				tile.transform.position,
				Quaternion.identity
			) as GameObject;
			unitInstance.GetComponent<Unit>().currentTile = tile;
			tile.GetComponent<Tile>().currentUnit = unitInstance;
			units[i] = unitInstance;
		}
	}

	public void PassTurn() {
		activeUnit = (activeUnit + 1) % units.Length;
		units[activeUnit].GetComponent<Unit>().TakeTurn();
	}

}
