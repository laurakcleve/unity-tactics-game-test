﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour {

	public int startingX;
    public int startingZ;
    public int moveRange;
    public int maxHealth;
    public int currentHealth;
    public int turnPosition;
    public bool isDead = false;
    public Tile currentTile;
    public State currentState;
    public Material defaultMat;
    public Material highlightMat;
    public Material deadMat;

    private Renderer thisRenderer;
    private Turn turn;

    public Turn Turn { get; set; }

    protected virtual void Awake() {
        currentHealth = maxHealth;
        thisRenderer = GetComponent<Renderer>();
    }

    public abstract void TakeTurn();
	
    public void LogState(string state, string action) {
        Debug.Log(gameObject.name + ": " + state + " | " + action);
    }

    public void EnableHighlight() {
        thisRenderer.material = highlightMat;
    }

    public void DisableHighlight() {
        if (!isDead) {
            thisRenderer.material = defaultMat;
        }
    }

    public void TakeDamage(int damage) {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " takes " + damage + " points of damage");
        if (currentHealth <= 0) {
            currentHealth = 0;
            Die();
        }
        Debug.Log(gameObject.name + " current health: " + currentHealth);
    }

    private void Die() {
        Debug.Log(gameObject.name + " is dead");
        isDead = true;
        thisRenderer.material = deadMat;
        GameManager.instance.RemoveUnit(turnPosition);
    }

}
