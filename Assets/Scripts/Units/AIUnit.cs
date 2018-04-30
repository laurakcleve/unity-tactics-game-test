using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIUnit : Unit {

	public override void TakeTurn() {
        Debug.Log(gameObject.name + " taking turn");
        GameManager.instance.PassTurn();
    }
	
}
