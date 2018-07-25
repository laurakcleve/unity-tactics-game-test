using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIUnit : Unit {

    protected override void Awake() {
        base.Awake();

        Turn = gameObject.AddComponent(typeof(AITurn)) as AITurn;
    }

	public override void TakeTurn() {
        Debug.Log(gameObject.name + " taking turn");
        gameObject.transform.Find("Spotlight").gameObject.SetActive(true);
        Turn.AITakeTurn(this);
    }

}
