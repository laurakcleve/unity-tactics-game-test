using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour {

	public int startingX;
    public int startingZ;
    public int moveRange;
    public GameObject currentTile;
    public GameObject tempCurrentTile;
    public List<GameObject> validMoves;
    public List<GameObject> validAbilityTargets;
    public Ability activeAbility;
    public State currentState;
    public Material originalMat;
    public Material highlightMat;



    public abstract void TakeTurn();
	
    public void LogState(string state, string action) {
        Debug.Log(gameObject.name + ": " + state + " | " + action);
    }

    public List<GameObject> GetValidMoveTiles(GameObject startTile, int range) {
        List<GameObject> validTiles = new List<GameObject>();
        CheckNextTile(validTiles, startTile, range);
        validTiles.Remove(startTile);
        return validTiles;
    }

    public List<GameObject> GetValidTargetTiles(GameObject startTile, Ability ability) {
        List<GameObject> validTiles = new List<GameObject>();

        if (ability.targetType == TargetType.Self) {
            validTiles.Add(startTile);
            return validTiles;
        }

        CheckNextTile(validTiles, startTile, ability.range);

        switch(ability.targetType) {
            case TargetType.Ally:
                break;
            case TargetType.Enemy:
                break;
            case TargetType.Any:
                break;
            default:
                break;
        }

        return validTiles;
    }

    private List<GameObject> CheckNextTile(List<GameObject> validTiles, GameObject startTile, int range) {
        validTiles.Add(startTile);
        List<GameObject> connectedTiles = startTile.GetComponent<Tile>().connected;
        foreach (GameObject tile in connectedTiles) {
            Tile currentTileScript = currentTile.GetComponent<Tile>();
            Tile targetTileScript = tile.GetComponent<Tile>();
            if ((Mathf.Abs(targetTileScript.x - currentTileScript.x) + Mathf.Abs(targetTileScript.z - currentTileScript.z) <= range) && !validTiles.Contains(tile)) {
                CheckNextTile(validTiles, tile, range);
            }
        }
        return validTiles;
    }

    public virtual void MoveToTile(GameObject tile) {
        transform.position = tile.transform.position;
        tempCurrentTile = tile;
    }

    public void EnableHighlight() {
        GetComponent<Renderer>().material = highlightMat;
    }

    public void DisableHighlight() {
        GetComponent<Renderer>().material = originalMat;
    }
}
