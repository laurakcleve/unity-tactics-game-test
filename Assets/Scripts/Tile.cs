using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

	public int x;
	public int z;
	public Material tileMat;
	public Material tileHighlightMat;
	public Material tileValidMat;
	public Material previousMaterial;
	public List<GameObject> connected;
	public List<GameObject> effectTiles;
	public GameObject currentUnit;



	private void Awake() {
		previousMaterial = tileMat;
	}

	public void Highlight() {
		GetComponent<Renderer>().material = tileHighlightMat;
	}

	public void DisableHighlight() {
		GetComponent<Renderer>().material = previousMaterial;
	}
	
	// private void OnMouseEnter() {
	// 	if (IsValidDestination()) {
	// 		GetComponent<Renderer>().material = tileHighlightMat;
	// 	}
	// 	else if (IsValidTarget()) {
	// 		effectTiles = new List<GameObject>();
	// 		effectTiles.Add(gameObject);
	// 		foreach (string position in GameManager.instance.units[GameManager.instance.activeUnit].GetComponent<PlayerUnit>().activeAbility.effectTiles) {
	// 			string[] values = position.Split(',');
	// 			if (x + Int32.Parse(values[0]) < 10 && z + Int32.Parse(values[1]) < 10) {
	// 				GameObject tile = GameManager.instance.tiles[x + Int32.Parse(values[0]), z + Int32.Parse(values[1])];
	// 				effectTiles.Add(tile);
	// 			}
	// 		}
	// 		foreach (GameObject tile in effectTiles) {
	// 			tile.GetComponent<Renderer>().material = tileHighlightMat;
	// 		}
	// 	}
	// }

	// private void OnMouseExit() {
    //     if (IsValidDestination()) {
	// 		GetComponent<Renderer>().material = previousMaterial;
	// 	}
	// 	else if (IsValidTarget()) {
    //         foreach (GameObject tile in effectTiles) {
    //             tile.GetComponent<Renderer>().material = previousMaterial;
    //         }
	// 	}
	// }

	// private void OnMouseOver() {
	// 	if (Input.GetMouseButtonDown(0) && IsValidDestination()) {
    //         GameManager.instance.units[GameManager.instance.activeUnit].GetComponent<Unit>().MoveToTile(gameObject);
	// 	}
	// }

	// private bool IsValidDestination() {
	// 	PlayerUnit unit = GameManager.instance.units[GameManager.instance.activeUnit].GetComponent<PlayerUnit>();
	// 	return unit.validMoves.Contains(gameObject) && unit.currentState == unit.playerSelectDestination;
	// }

	// private bool IsValidTarget() {
    //     PlayerUnit unit = GameManager.instance.units[GameManager.instance.activeUnit].GetComponent<PlayerUnit>();
	// 	return unit.validAbilityTargets.Contains(gameObject) && unit.currentState == unit.playerSelectTarget;
	// }

}
