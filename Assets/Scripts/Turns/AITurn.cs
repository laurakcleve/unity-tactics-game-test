using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITurn : Turn {

    private Unit target;
    private Ability chosenAbility;
    private bool hasActed;

    private bool verbose = false;

    public override void AITakeTurn(AIUnit unit) {
        target = null;
        hasActed = false;
        StartCoroutine(TurnCoroutine(unit));
    }

    IEnumerator TurnCoroutine(AIUnit unit) {

        if (verbose)
            yield return new WaitForSeconds(.5f);

        yield return StartCoroutine(AttackEnemyInRange(unit));

        if (verbose)
            yield return new WaitForSeconds(.5f);

        if (!hasActed) {
            Debug.Log("Moving to nearest enemy");
            MoveToNearestEnemy(unit);
            if (verbose) 
                yield return new WaitForSeconds(1f);
            yield return StartCoroutine(AttackEnemyInRange(unit));
        }

        gameObject.transform.Find("Spotlight").gameObject.SetActive(false);
        GameManager.instance.PassTurn();
    }

    IEnumerator AttackEnemyInRange(AIUnit unit) {
        foreach (Ability ability in GetComponent<UnitClass>().abilities) {
            target = null;
            Debug.Log("Checking ability " + ability.name);
            GetValidTargetTiles(unit.currentTile, ability);
            ShowTiles(ValidAbilityTargets);
            if (verbose)
                yield return new WaitForSeconds(1f);

            foreach (Tile tile in ValidAbilityTargets) {
                if (ability.targetType == TargetType.Area || ability.targetType == TargetType.AreaNotSelf) {
                    List<Tile> currentEffectTiles = GetEffectTiles(tile, ability.effectTiles, unit.currentTile);
                    EnableTileHighlights(currentEffectTiles);

                    foreach (Tile effectTile in currentEffectTiles) {
                        if (effectTile.currentUnit != null && effectTile.currentUnit.gameObject.CompareTag("PlayerUnit") && !effectTile.currentUnit.isDead) {
                            Debug.Log("Found enemy at " + effectTile.x + ", " + effectTile.z);
                            target = effectTile.currentUnit;
                            chosenAbility = ability;
                            break;
                        }
                    }

                    if (verbose)
                        yield return new WaitForSeconds(.5f);
                    DisableTileHighlights(currentEffectTiles);
                }
                if (target != null) break;

                else if (tile.currentUnit != null && tile.currentUnit.gameObject.CompareTag("PlayerUnit") && !tile.currentUnit.isDead) {
                    tile.currentUnit.EnableHighlight();
                    Debug.Log("Found enemy at " + tile.x + ", " + tile.z);
                    target = tile.currentUnit;
                    if (verbose)
                        yield return new WaitForSeconds(.5f);
                    tile.currentUnit.DisableHighlight();
                    chosenAbility = ability;
                    break;
                }

            }
            HideTiles(ValidAbilityTargets);
            if (target != null) break;
        }

        if (target != null) {
            GameManager.instance.HandleAction(unit, target, chosenAbility);
            hasActed = true;
            if (verbose)
                yield return new WaitForSeconds(1f);
        }
    }

    private void MoveToNearestEnemy(AIUnit unit) {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("PlayerUnit");
        Pathfinding pathfinder = unit.currentTile.GetComponent<Pathfinding>();
        List<Tile> path = new List<Tile>();

        int shortestDistance = -1;

        for (int i = 0; i < enemies.Length; i++) {
            Unit enemyUnit = enemies[i].GetComponent<Unit>();
            List<Tile> tempPath = pathfinder.FindPath(unit.currentTile, enemyUnit.currentTile);

            if ((shortestDistance < 0 || shortestDistance > tempPath.Count) && !enemyUnit.isDead) {
                shortestDistance = tempPath.Count;
                target = enemyUnit;
                path = tempPath;
            }
        }

        Tile destinationTile = null;
        // List<Tile> movePath = new List<Tile>();
        GetValidMoveTiles(unit.currentTile, unit.moveRange);

        for (int i = path.Count - 1; i >= 0; i--) {
            if (ValidMoveTiles.Contains(path[i])) {
                destinationTile = path[i];
                // movePath = path.GetRange(0, i + 1);
                break;
            }
        }

        if (destinationTile == null) {
            return;
        }

        MoveToTile(unit, destinationTile);
    }

    public override void MoveToTile(AIUnit unit, Tile tile) {
        base.MoveToTile(unit, tile);
        unit.currentTile.currentUnit = null;
        unit.currentTile = tile;
        tile.currentUnit = unit;
    }

}
