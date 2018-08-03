using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Turn : MonoBehaviour {

    public List<Tile> lastHoveredEffectTiles;

    protected List<Tile> validMoveTiles;
    protected List<Tile> validAbilityTargets;
    protected Ability activeAbility;

    public List<Tile> ValidMoveTiles { get; set; }
    public List<Tile> ValidAbilityTargets { get; set; }

    public virtual void MouseoverDestination(PlayerUnit unit) {}
    public virtual void MouseoverTarget(PlayerUnit unit) {}
    public virtual void ClickMove(PlayerUnit unit) {}
    public virtual void ClickAct(PlayerUnit unit) {}
    public virtual void ClickAbility(Ability ability, PlayerUnit unit) {}
    public virtual void PopulateAbilities(PlayerUnit unit) {}
    public virtual void DepopulateAbilities() {}
    public virtual void Cancel(PlayerUnit unit) {}
    public virtual void Confirm(PlayerUnit unit) {}

    public virtual void AITakeTurn(AIUnit unit) {}

    
    public void GetValidMoveTiles(Tile startTile, int range) {
        ValidMoveTiles = new List<Tile>();
        CheckNextTile(ValidMoveTiles, startTile, range, startTile);
        foreach (Unit unit in GameManager.instance.units) {
            Tile tile = unit.currentTile;
            if (ValidMoveTiles.Contains(tile)) {
                ValidMoveTiles.Remove(tile);
            }
        }
    }

    public void GetValidTargetTiles(Tile startTile, Ability ability) {
        ValidAbilityTargets = new List<Tile>();

        if (ability.targetType == TargetType.Self) {
            ValidAbilityTargets.Add(startTile);
            return;
        }

        CheckNextTile(ValidAbilityTargets, startTile, ability.range, startTile);

        if (ability.targetType == TargetType.AreaNotSelf) {
            ValidAbilityTargets.Remove(startTile);
        }
    }

    private void CheckNextTile(List<Tile> validTiles, Tile startTile, int range, Tile unitCurrentTile) {
        validTiles.Add(startTile);
        List<Tile> connectedTiles = startTile.connected;
        foreach (Tile tile in connectedTiles) {
            if ((Mathf.Abs(tile.x - unitCurrentTile.x) + Mathf.Abs(tile.z - unitCurrentTile.z) <= range) && !validTiles.Contains(tile)) {
                CheckNextTile(validTiles, tile, range, unitCurrentTile);
            }
        }
    }

    protected List<Tile> GetEffectTiles(Tile tile, List<string> effectPattern, Tile unitCurrentTile) {
        List<Tile> effectTiles = new List<Tile>();
        foreach (string position in effectPattern) {
            // effectPattern is an array of positions, set in the editor
            // position is a string in the form "X,Z"
            string[] relativeCoords = position.Split(',');

            Tile currentTileScript = unitCurrentTile.GetComponent<Tile>();

            // Check the hovered tile's position in relation to the currently controlled unit's position, and rotate the effect tiles so they are "in front" of the unit
            if (tile.x > currentTileScript.x) relativeCoords = Rotate90(relativeCoords);
            else if (tile.z < currentTileScript.z) relativeCoords = Rotate180(relativeCoords);
            else if (tile.x < currentTileScript.x) relativeCoords = Rotate270(relativeCoords);

            // Change relative position to absolute on the grid
            int x = Int32.Parse(relativeCoords[0]) + tile.x;
            int z = Int32.Parse(relativeCoords[1]) + tile.z;

            // Make sure tiles are in the bounds of the grid
            if (x < 10 && x >= 0 && z < 10 && z >= 0) {
                effectTiles.Add(GameManager.instance.tiles[x, z].GetComponent<Tile>());
            }
        }
        return effectTiles;
    }

    string[] Rotate90(string[] relativeCoords) {
        relativeCoords[0] = (-Int32.Parse(relativeCoords[0])).ToString();
        string temp = relativeCoords[0];
        relativeCoords[0] = relativeCoords[1];
        relativeCoords[1] = temp;
        return relativeCoords;
    }

    string[] Rotate180(string[] relativeCoords) {
        relativeCoords[0] = (-Int32.Parse(relativeCoords[0])).ToString();
        relativeCoords[1] = (-Int32.Parse(relativeCoords[1])).ToString();
        return relativeCoords;
    }

    string[] Rotate270(string[] relativeCoords) {
        relativeCoords[1] = (-Int32.Parse(relativeCoords[1])).ToString();
        string temp = relativeCoords[0];
        relativeCoords[0] = relativeCoords[1];
        relativeCoords[1] = temp;
        return relativeCoords;
    }

    public void EnableTileHighlights(List<Tile> tiles) {
        foreach (Tile tile in tiles) {
            tile.EnableHighlight();
            if (tile.currentUnit != null && !tile.currentUnit.GetComponent<Unit>().isDead) {
                tile.currentUnit.EnableHighlight();
            }
        }
    }

    public void DisableTileHighlights(List<Tile> tiles) {
        foreach (Tile tile in tiles) {
            tile.DisableHighlight();
            if (tile.currentUnit != null) {
                tile.currentUnit.DisableHighlight();
            }
        }
    }

    public virtual void MoveToTile(AIUnit unit, Tile tile) {
        unit.transform.position = tile.transform.position;
    }

    public virtual void MoveToTile(PlayerUnit unit, Tile tile) {
        unit.transform.position = tile.transform.position;
    }

    public void ShowTiles(List<Tile> tiles) {
        foreach (Tile tile in tiles) {
            tile.EnableValid();
        }
    }

    public void HideTiles(List<Tile> tiles) {
        foreach (Tile tile in tiles) {
            tile.DisableValid();
        }
    }

    

}
