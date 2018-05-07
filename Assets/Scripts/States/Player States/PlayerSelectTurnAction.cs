using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelectTurnAction : State {

    public override void Enter(PlayerUnit unit) {
        // unit.LogState("PlayerSelectTurnAction", "ENTER");

        GameManager.instance.turnCanvas.SetActive(true);
    }

    public override void Exit(PlayerUnit unit) {
        // unit.LogState("PlayerSelectTurnAction", "EXIT");

        GameManager.instance.turnCanvas.SetActive(false);
    }

}
