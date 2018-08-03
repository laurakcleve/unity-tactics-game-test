using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTurn : Turn {

  Tile lastHoveredTile;
  Unit lastHoveredUnit;
  bool validDestinationHovered = false;
  bool validTargetHovered = false;
  bool validEffectAreaHovered = false;
  Ray ray;
  RaycastHit hit;
  public Tile tempCurrentTile;


  public override void MouseoverDestination(PlayerUnit unit) {
    validDestinationHovered = false;
    ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    if (Physics.Raycast(ray, out hit)) {
      GameObject hitObj = hit.collider.gameObject;
      if (MouseEnteredNewDestination(hitObj)) {
        DisablePreviousHighlights();
        EnableDestinationHighlight(hitObj);
      }
    }
    if (Input.GetMouseButtonDown(0) && validDestinationHovered) {
      MoveToTile(unit, hit.collider.gameObject.GetComponent<Tile>());
    }
  }


  public override void MouseoverTarget(PlayerUnit unit) {
    validTargetHovered = false;
    validEffectAreaHovered = false;
    ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    if (Physics.Raycast(ray, out hit)) {
      GameObject hitObj = hit.collider.gameObject;

      if (MouseEnteredNewTarget(hitObj)) {
        DisablePreviousHighlights();
        EnableTargetHighlight(hitObj, unit.currentTile);
      }

      if (Input.GetMouseButtonDown(0)) {
        if (validTargetHovered) {
          lastHoveredEffectTiles = null;
          unit.ChangeState(State.playerConfirmTarget);
        }
        else if (validEffectAreaHovered) {
          unit.ChangeState(State.playerConfirmTarget);
        }
      }
    }
  }


  void DisablePreviousHighlights() {
    if (lastHoveredTile != null) {
      lastHoveredTile.DisableHighlight();
    }
    if (lastHoveredUnit != null) {
      lastHoveredUnit.DisableHighlight();
    }
    if (lastHoveredEffectTiles != null) {
      foreach (Tile tile in lastHoveredEffectTiles) {
        tile.DisableHighlight();
        if (tile.currentUnit != null) {
          tile.currentUnit.DisableHighlight();
        }
      }
    }
  }


  bool MouseEnteredNewDestination(GameObject hitObj) {
    return lastHoveredTile != hitObj;
  }


  bool MouseEnteredNewTarget(GameObject hitObj) {
    return lastHoveredTile != hitObj || lastHoveredUnit != hitObj;
  }
  

  void EnableDestinationHighlight(GameObject hitObj) {
    if (hitObj.CompareTag("Tile")) {
      Tile hoveredTile = hitObj.GetComponent<Tile>();
      if (ValidMoveTiles.Contains(hoveredTile)) {
        hoveredTile.EnableHighlight();
        lastHoveredTile = hoveredTile;
        validDestinationHovered = true;
      }
    }
  }

  void EnableTargetHighlight(GameObject hitObj, Tile sourceTile) {
    switch (activeAbility.targetType) {
      case TargetType.Enemy:
        if (hitObj.CompareTag("AIUnit")) {
          AIUnit hoveredUnit = hitObj.GetComponent<AIUnit>();
          if (!hoveredUnit.isDead && ValidAbilityTargets.Contains(hoveredUnit.currentTile)) {
            hoveredUnit.EnableHighlight();
            lastHoveredUnit = hoveredUnit;
            validTargetHovered = true;
          }
        }
        break;

      case TargetType.Area:
      case TargetType.AreaNotSelf:
        if ((hitObj.CompareTag("Tile"))) {
          Tile hoveredTile = hitObj.GetComponent<Tile>();
          if (ValidAbilityTargets.Contains(hoveredTile)) {
            lastHoveredEffectTiles = GetEffectTiles(hoveredTile, activeAbility.effectTiles, sourceTile);
            EnableTileHighlights(lastHoveredEffectTiles);
            lastHoveredTile = hoveredTile;
            validEffectAreaHovered = true;
          }
        }
        else if (hitObj.CompareTag("AIUnit") || hitObj.CompareTag("PlayerUnit")) {
          Unit hoveredUnit = hitObj.GetComponent<Unit>();
          if (ValidAbilityTargets.Contains(hoveredUnit.currentTile)) {
            lastHoveredEffectTiles = GetEffectTiles(hoveredUnit.currentTile, activeAbility.effectTiles, sourceTile);
            EnableTileHighlights(lastHoveredEffectTiles);
            lastHoveredUnit = hoveredUnit;
            validEffectAreaHovered = true;
          }
        }
        break;

      case TargetType.Ally:
        if (hitObj.CompareTag("PlayerUnit")) {
          PlayerUnit hoveredUnit = hitObj.GetComponent<PlayerUnit>();
          if (!hoveredUnit.isDead && ValidAbilityTargets.Contains(hoveredUnit.currentTile)) {
            hoveredUnit.EnableHighlight();
            lastHoveredUnit = hoveredUnit;
            validTargetHovered = true;
          }
        }
        break;

      case TargetType.Self:
        break;

      default:
        break;
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
      else if (validEffectAreaHovered) {
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
