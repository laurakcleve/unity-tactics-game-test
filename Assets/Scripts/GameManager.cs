using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public static GameManager instance = null;
	public GameObject tilePrefab;
	public GameObject[] unitPrefabs;
	public GameObject turnCanvas;
	public GameObject abilitiesCanvas;
	public Button moveButton;
	public Button actButton;
	public Button cancelButton;
	public Button confirmButton;
	public Button endTurnButton;
	public int activeUnit = 0;

	public List<Unit> units;
	public Tile[,] tiles;


	void Awake() {
        if (instance == null) {
            instance = this;
        }
        else if (instance != this) {
            Destroy(this);
        }

		CreateTiles();
		CreateUnits();
	}

	void Start() {
		units[activeUnit].GetComponent<Unit>().TakeTurn();
	}

	void CreateTiles() {
		tiles = new Tile[10,10];

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

				tiles[x,z] = tileScript;
			}
		}

		for (int z = 0; z < 10; z++) {
			for (int x = 0; x < 10; x++) {
				List<Tile> connectedTiles = tiles[z,x].connected;
                if (z > 0)
                    connectedTiles.Add(tiles[z - 1, x]);
                if (x < 9)
                    connectedTiles.Add(tiles[z, x + 1]);
                if (z < 9)
                    connectedTiles.Add(tiles[z + 1, x]);
                if (x > 0)
                    connectedTiles.Add(tiles[z, x - 1]);
			}
		}

	}

	void CreateUnits() {
		units = new List<Unit>();

		for (int i = 0; i < unitPrefabs.Length; i++) {
			Unit unitScript = unitPrefabs[i].GetComponent<Unit>();
			Tile startingTile = tiles[unitScript.startingX, unitScript.startingZ];
			GameObject unitInstance = Instantiate(
				unitPrefabs[i],
				startingTile.transform.position,
				Quaternion.identity
			) as GameObject;
			Unit instanceUnitScript = unitInstance.GetComponent<Unit>();
			instanceUnitScript.currentTile = startingTile;
			startingTile.currentUnit = instanceUnitScript;
			instanceUnitScript.turnPosition = i;
			unitInstance.name = unitPrefabs[i].name;
			units.Add(instanceUnitScript);
		}
	}

	public void PassTurn() {
		activeUnit++;
		if (activeUnit >= units.Count) {
			activeUnit = 0;
		}
		units[activeUnit].TakeTurn();
	}

	public void HandleAction(Unit actor, Unit target, Ability ability) {
		Debug.Log(actor.name + " using " + ability.name + " on " + target.name);
		target.TakeDamage(ability.damage);
	}

	public void RemoveUnit(int position) {
		units.RemoveAt(position);
		for (int i = 0; i < units.Count; i++) {
			units[i].turnPosition = i;
		}
	}

}
