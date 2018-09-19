using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUnit : Unit {

    public GameObject abilityButtonPrefab;


    protected override void Awake() {
        base.Awake();
        
        Turn = gameObject.AddComponent(typeof(PlayerTurn)) as PlayerTurn;

        currentState = State.playerInactive;
    }

    void Update() {
        if (currentState == State.playerSelectDestination) {
            Turn.MouseoverDestination(this);
        }
        else if (currentState == State.playerSelectTarget) {
            Turn.MouseoverTarget(this);
        }
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
        GameManager.instance.AddMessageToLog(gameObject.name + " beginning turn");

        ChangeState(State.playerSelectTurnAction);
    }

    

   

    public void EndTurn() {
        ChangeState(State.playerInactive);
        GameManager.instance.PassTurn();
    }

}
