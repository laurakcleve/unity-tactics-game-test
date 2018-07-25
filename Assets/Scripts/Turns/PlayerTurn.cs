using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTurn : Turn {

    
    Tile lastHoveredTile;
    Unit lastHoveredUnit;
    bool validDestinationHovered = false;
    bool validTargetHovered = false;
    bool validAreaHovered = false;
    Ray ray;
    RaycastHit hit;
    public Tile tempCurrentTile;

    public override void Mouseover(PlayerUnit unit) {
        validAreaHovered = false;
        validTargetHovered = false;
        validDestinationHovered = false;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit)) {
            GameObject obj = hit.collider.gameObject;
			Tile hoveredTile = obj.GetComponent<Tile>();
            // If player is selecting a tile to move to
            if (unit.currentState == State.playerSelectDestination) {
                // If mouse has left a tile
                if (lastHoveredTile != obj && lastHoveredTile != null) {
                    // Disable highlight on the tile just left
                    lastHoveredTile.GetComponent<Tile>().DisableHighlight();
                }
                // If mouse is on a valid tile
                if (obj.CompareTag("Tile") && ValidMoveTiles.Contains(hoveredTile)) {
                    hoveredTile.EnableHighlight();
                    lastHoveredTile = hoveredTile;
                    validDestinationHovered = true;
                }

            }
            // If player is selecting a unit to target
            else if (unit.currentState == State.playerSelectTarget) {
                switch (activeAbility.targetType) {
                    case TargetType.Enemy:
                        if (lastHoveredUnit != obj && lastHoveredUnit != null) {
                            lastHoveredUnit.GetComponent<Unit>().DisableHighlight();
                        }
                        if (obj.CompareTag("AIUnit") && !obj.GetComponent<Unit>().isDead && ValidAbilityTargets.Contains(obj.GetComponent<Unit>().currentTile)) {
                            obj.GetComponent<Unit>().EnableHighlight();
                            lastHoveredUnit = obj.GetComponent<Unit>();
                            validTargetHovered = true;
                        }
                        break;

                    case TargetType.Area:
                    case TargetType.AreaNotSelf:
                        // If we are not hovering over the same tile anymore, disable highlight on that tile and its area of effect
                        if (lastHoveredTile != obj && lastHoveredEffectTiles != null) {
                            foreach (Tile tile in lastHoveredEffectTiles) {
                                tile.DisableHighlight();
                                if (tile.currentUnit != null) {
                                    tile.currentUnit.GetComponent<Unit>().DisableHighlight();
                                }
                            }
                        }
                        // If we are hovering a tile and it's within the range of the current ability, enable highlight on it and its area of effect
                        if ((obj.CompareTag("Tile") && ValidAbilityTargets.Contains(hoveredTile)) || ((obj.CompareTag("AIUnit") || obj.CompareTag("PlayerUnit")) && ValidAbilityTargets.Contains(obj.GetComponent<Unit>().currentTile))) {
                            lastHoveredEffectTiles = obj.CompareTag("Tile") ? GetEffectTiles(obj.GetComponent<Tile>(), activeAbility.effectTiles, unit.currentTile) : GetEffectTiles(obj.GetComponent<Unit>().currentTile.GetComponent<Tile>(), activeAbility.effectTiles, unit.currentTile);
                            EnableTileHighlights(lastHoveredEffectTiles);
                            lastHoveredTile = hoveredTile;
                            validAreaHovered = true;
                        }
                        break;

                    case TargetType.Ally:
                        break;

                    case TargetType.Self:
                        break;

                    default:
                        break;
                }
            }

            if (Input.GetMouseButtonDown(0)) {
                if (validDestinationHovered) {
                    MoveToTile(unit, hit.collider.gameObject.GetComponent<Tile>());
                }
                else if (validTargetHovered) {
                    lastHoveredEffectTiles = null;
                    unit.ChangeState(State.playerConfirmTarget);
                }
                else if (validAreaHovered) {
                    unit.ChangeState(State.playerConfirmTarget);
                }
            }
        }
    }

    public override void MoveToTile(PlayerUnit unit, Tile tile) {
        base.MoveToTile(unit, tile);
        tempCurrentTile = tile;
        unit.ChangeState(State.playerConfirmDestination);
    }

    public void ResetHighlights() {
        if (lastHoveredUnit != null)
            lastHoveredUnit.GetComponent<Unit>().DisableHighlight();
        if (lastHoveredTile != null)
            lastHoveredTile.GetComponent<Tile>().DisableHighlight();
        if (lastHoveredEffectTiles != null) {
            DisableTileHighlights(lastHoveredEffectTiles);
        }
    }

    public override void ClickMove(PlayerUnit unit) {
        unit.ChangeState(State.playerSelectDestination);
    }

    public override void ClickAct(PlayerUnit unit) {
        unit.ChangeState(State.playerMenus);
        GameManager.instance.abilitiesCanvas.SetActive(true);

    }

    public override void ClickAbility(Ability ability, PlayerUnit unit) {
        activeAbility = ability;
        GetValidTargetTiles(unit.currentTile, ability);
        ShowTiles(ValidAbilityTargets);
        unit.ChangeState(State.playerSelectTarget);
    }

    public override void PopulateAbilities(PlayerUnit unit) {
        foreach (Ability ability in unit.gameObject.GetComponent<UnitClass>().abilities) {
            GameObject abilityButton = Instantiate(unit.abilityButtonPrefab, Vector3.zero, Quaternion.identity);
            abilityButton.transform.SetParent(GameManager.instance.abilitiesCanvas.transform, false);
            abilityButton.transform.Find("Text").GetComponent<Text>().text = ability.name;
            abilityButton.GetComponent<Button>().onClick.AddListener(delegate { ClickAbility(ability, unit); });
        }
    }

    public override void DepopulateAbilities() {
        foreach (Transform child in GameManager.instance.abilitiesCanvas.transform) {
            GameObject.Destroy(child.gameObject);
        }
    }

    public override void Cancel(PlayerUnit unit) {
        if (unit.currentState == State.playerConfirmDestination) {
            transform.position = unit.currentTile.transform.position;
        }
        ResetHighlights();
        unit.ChangeState(State.playerSelectTurnAction);
    }

    public override void Confirm(PlayerUnit unit) {
        if (unit.currentState == State.playerConfirmDestination) {
            unit.currentTile.currentUnit = null;
            unit.currentTile = tempCurrentTile;
            unit.currentTile.currentUnit = unit;
            GameManager.instance.moveButton.interactable = false;
            unit.ChangeState(State.playerSelectTurnAction);
        }
        else if (unit.currentState == State.playerConfirmTarget) {
            if (validTargetHovered) {
                GameManager.instance.HandleAction(unit, lastHoveredUnit, activeAbility);
            }
            else if (validAreaHovered) {
                foreach (Tile tile in lastHoveredEffectTiles) {
                    if (tile.currentUnit != null && !tile.currentUnit.isDead) {
                        GameManager.instance.HandleAction(unit, tile.currentUnit, activeAbility);
                    }
                }
            }
            ResetHighlights();
            GameManager.instance.actButton.interactable = false;
            unit.ChangeState(State.playerSelectTurnAction);
        }
    }

}
