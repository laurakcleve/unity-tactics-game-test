using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelectDestination : State {

    public override void Enter(PlayerUnit unit) {
        // unit.LogState("PlayerSelectDestination", "ENTER");

        GameManager.instance.cancelButton.gameObject.SetActive(true);

		unit.Turn.GetValidMoveTiles(unit.currentTile, unit.moveRange);
		unit.Turn.ShowTiles(unit.Turn.ValidMoveTiles);
    }

    public override void Exit(PlayerUnit unit) {
        // unit.LogState("PlayerSelectDestination", "EXIT");

        GameManager.instance.cancelButton.gameObject.SetActive(false);

        unit.Turn.HideTiles(unit.Turn.ValidMoveTiles);
    }

}
