using System;
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


    public void LoseHP(int amount) {
        currentHealth -= amount;
        GameManager.instance.AddMessageToLog($"{gameObject.name} takes {amount} points of damage");
        if (currentHealth <= 0) {
            currentHealth = 0;
            Die();
        }
        GameManager.instance.AddMessageToLog($"{gameObject.name} current health: {currentHealth}");
    }

    public void GainHP(int amount) {
        int healAmount, overhealAmount;
        if (currentHealth + amount > maxHealth) {
            healAmount = maxHealth - currentHealth;
            overhealAmount = currentHealth + amount - maxHealth;
            GameManager.instance.AddMessageToLog($"{gameObject.name} gains {healAmount}  health points ({overhealAmount} overheal)");
        }
        else {
            healAmount = amount;
            overhealAmount = 0;
            GameManager.instance.AddMessageToLog($"{gameObject.name} gains {healAmount} health points");
        }
        currentHealth += healAmount;
        GameManager.instance.AddMessageToLog($"{gameObject.name} current health: {currentHealth}");
    }

    private void Die() {
        GameManager.instance.AddMessageToLog($"{gameObject.name} is dead");
        isDead = true;
        thisRenderer.material = deadMat;
        GameManager.instance.RemoveUnit(turnPosition);
    }

}
