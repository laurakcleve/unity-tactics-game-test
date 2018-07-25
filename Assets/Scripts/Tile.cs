using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

	public int x;
	public int z;
	public Material defaultMat;
	public Material validMat;
	public Material highlightMat;
	public List<Tile> connected;
	public List<Tile> effectTiles;
	public Unit currentUnit;

	private Renderer thisRenderer;
	private Material previousMaterial;
    private int gCost;
    private int hCost;
    private int fCost;
    private Tile parent;

    public int GCost { get; set; }
    public int HCost { get; set; }
    public int FCost { get; set; }
    public Tile Parent { get; set; }


	private void Awake() {
		thisRenderer = GetComponent<Renderer>();
		previousMaterial = defaultMat;
	}

	public void EnableValid() {
		thisRenderer.material = validMat;
		previousMaterial = validMat;
	}

	public void DisableValid() {
        thisRenderer.material = defaultMat;
		previousMaterial = defaultMat;
	}

	public void EnableHighlight() {
        thisRenderer.material = highlightMat;
	}

	public void DisableHighlight() {
        thisRenderer.material = previousMaterial;
	}

    public void SetCosts(Tile start, Tile end) {
        GCost = Mathf.Abs(x - start.x) + Mathf.Abs(z - start.z);
        HCost = Mathf.Abs(x - end.x) + Mathf.Abs(z - end.z);
    }

}
