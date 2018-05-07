using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUnit : Unit {

    public GameObject abilityButtonPrefab;

    Ray ray;
    RaycastHit hit;
    GameObject lastHoveredTile;
    List<Tile> lastHoveredEffectTiles;
    GameObject lastHoveredUnit;
    bool validDestinationHovered = false;
    bool validTargetHovered = false;
    bool validAreaHovered = false;




    protected override void Awake() {
        base.Awake();
        currentState = State.playerInactive;
    }

    void Update() {
        if (currentState == State.playerSelectDestination || currentState == State.playerSelectTarget) {
            Mouseover();
        }
    }

    void Mouseover() {
        validAreaHovered = false;
        validTargetHovered = false;
        validDestinationHovered = false;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit)) {
            GameObject obj = hit.collider.gameObject;
            if (currentState == State.playerSelectDestination) {
                if (lastHoveredTile != obj && lastHoveredTile != null) {
                    lastHoveredTile.GetComponent<Tile>().DisableHighlight();
                }
                if (obj.CompareTag("Tile") && validMoves.Contains(obj)) {
                    obj.GetComponent<Tile>().Highlight();
                    lastHoveredTile = obj;
                    validDestinationHovered = true;
                }

            }
            else if (currentState == State.playerSelectTarget) {
                switch (activeAbility.targetType) {
                    case TargetType.Enemy:
                        if (lastHoveredUnit != obj && lastHoveredUnit != null) {
                            lastHoveredUnit.GetComponent<Unit>().DisableHighlight();
                        }
                        if (obj.CompareTag("AIUnit") && !obj.GetComponent<Unit>().isDead && validAbilityTargets.Contains(obj.GetComponent<Unit>().currentTile)) {
                            obj.GetComponent<Unit>().EnableHighlight();
                            lastHoveredUnit = obj;
                            validTargetHovered = true;
                        }
                        break;

                    case TargetType.Any:
                        if (lastHoveredTile != obj && lastHoveredEffectTiles != null) {
                            foreach (Tile tile in lastHoveredEffectTiles) {
                                tile.DisableHighlight();
                                if (tile.currentUnit != null) {
                                    tile.currentUnit.GetComponent<Unit>().DisableHighlight();
                                }
                            }
                        }
                        if (obj.CompareTag("Tile") && validAbilityTargets.Contains(obj)) {
                            lastHoveredEffectTiles = GetEffectTiles(obj.GetComponent<Tile>(), activeAbility.effectTiles);
                            HighlightEffectTiles();
                            lastHoveredTile = obj;
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
                    MoveToTile(hit.collider.gameObject);
                }
                else if (validTargetHovered) {
                    lastHoveredEffectTiles = null;
                    ChangeState(State.playerConfirmTarget);
                }
                else if (validAreaHovered) {
                    ChangeState(State.playerConfirmTarget);
                }
            }
        }
    }

    public void ResetHighlights() {
        Debug.Log("resetting highlights");
        if (lastHoveredUnit != null)
            lastHoveredUnit.GetComponent<Unit>().DisableHighlight();
        if (lastHoveredTile != null)
            lastHoveredTile.GetComponent<Tile>().DisableHighlight();
        if (lastHoveredEffectTiles != null) {
            foreach (Tile tile in lastHoveredEffectTiles) {
                if (tile != null){
                    tile.DisableHighlight();
                }
                if (tile.currentUnit != null) {
                    tile.currentUnit.GetComponent<Unit>().DisableHighlight();
                }
            }
        }
    }

    public void HighlightEffectTiles() {
        if (lastHoveredEffectTiles != null) {
            foreach (Tile tile in lastHoveredEffectTiles) {
                tile.Highlight();
                if (tile.currentUnit != null && !tile.currentUnit.GetComponent<Unit>().isDead) {
                    tile.currentUnit.GetComponent<Unit>().EnableHighlight();
                }
            }
        }
    }

    List<Tile> GetEffectTiles(Tile tile, List<string> effectPattern) {
        List<Tile> effectTiles = new List<Tile>();
        foreach (string position in effectPattern) {
            string[] relativeCoords = position.Split(',');

            Tile currentTileScript = currentTile.GetComponent<Tile>();
            if (tile.x > currentTileScript.x) relativeCoords = Rotate90(relativeCoords);
            else if (tile.z < currentTileScript.z) relativeCoords = Rotate180(relativeCoords);
            else if (tile.x < currentTileScript.x) relativeCoords = Rotate270(relativeCoords);

            int x = Int32.Parse(relativeCoords[0]) + tile.x;
            int z = Int32.Parse(relativeCoords[1]) + tile.z;

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

    public void ChangeState(State newState) {
        if (currentState != null) {
            currentState.Exit(this);
        }
        currentState = newState;
        currentState.Enter(this);
    }

    public override void TakeTurn() {

        Debug.Log(gameObject.name + " beginning turn");

        ChangeState(State.playerSelectTurnAction);
    }

    public void ClickMove() {
        ChangeState(State.playerSelectDestination);
    }

    public void ClickAct() {
        ChangeState(State.playerMenus);
        GameManager.instance.abilitiesCanvas.SetActive(true);

    }

    public void ClickAbility(Ability ability) {
        activeAbility = ability;
        validAbilityTargets = GetValidTargetTiles(currentTile, ability);
        ShowTiles(validAbilityTargets);
        ChangeState(State.playerSelectTarget);
    }

    public void PopulateAbilities() {
        foreach (Ability ability in GetComponent<UnitClass>().abilities) {
            GameObject abilityButton = Instantiate(abilityButtonPrefab, Vector3.zero, Quaternion.identity);
            abilityButton.transform.SetParent(GameManager.instance.abilitiesCanvas.transform, false);
            abilityButton.transform.Find("Text").GetComponent<Text>().text = ability.name;
            abilityButton.GetComponent<Button>().onClick.AddListener(delegate { ClickAbility(ability); });
        }
    }

    public void DepopulateAbilities() {
        foreach (Transform child in GameManager.instance.abilitiesCanvas.transform) {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void ShowTiles(List<GameObject> tiles) {
        foreach (GameObject tile in tiles) {
            tile.GetComponent<Renderer>().material = tile.GetComponent<Tile>().tileValidMat;
            tile.GetComponent<Tile>().previousMaterial = tile.GetComponent<Tile>().tileValidMat;
        }
    }

    public void HideTiles(List<GameObject> tiles) {
        foreach (GameObject tile in tiles) {
            tile.GetComponent<Renderer>().material = tile.GetComponent<Tile>().tileMat;
            tile.GetComponent<Tile>().previousMaterial = tile.GetComponent<Tile>().tileMat;
        }
    }

    public void Cancel() {
        if (currentState == State.playerConfirmDestination) {
            transform.position = currentTile.transform.position;
        }
        ResetHighlights();
        ChangeState(State.playerSelectTurnAction);
    }

    public void Confirm() {
        if (currentState == State.playerConfirmDestination) {
            currentTile.GetComponent<Tile>().currentUnit = null;
            currentTile = tempCurrentTile;
            currentTile.GetComponent<Tile>().currentUnit = gameObject;
            GameManager.instance.moveButton.interactable = false;
            ChangeState(State.playerSelectTurnAction);
        }
        else if (currentState == State.playerConfirmTarget) {
            if (validTargetHovered) {
                GameManager.instance.HandleAction(this, lastHoveredUnit.GetComponent<Unit>(), activeAbility);
            }
            else if (validAreaHovered) {
                foreach (Tile tile in lastHoveredEffectTiles) {
                    if (tile.currentUnit != null && !tile.currentUnit.GetComponent<Unit>().isDead) {
                        GameManager.instance.HandleAction(this, tile.currentUnit.GetComponent<Unit>(), activeAbility);
                    }
                }
            }
            ResetHighlights();
            GameManager.instance.actButton.interactable = false;
            ChangeState(State.playerSelectTurnAction);
        }
    }

    public override void MoveToTile(GameObject tile) {
        base.MoveToTile(tile);
        ChangeState(State.playerConfirmDestination);
    }

    public void EndTurn() {
        ChangeState(State.playerInactive);
        GameManager.instance.PassTurn();
    }

}
