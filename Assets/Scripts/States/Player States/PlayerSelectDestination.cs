using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelectDestination : State {

    public override void Enter(PlayerUnit unit) {
        unit.LogState("PlayerSelectDestination", "ENTER");

        GameManager.instance.cancelButton.gameObject.SetActive(true);

		unit.validMoves = unit.GetValidMoveTiles(unit.currentTile, unit.moveRange);
		unit.ShowTiles(unit.validMoves);
    }

    public override void Exit(PlayerUnit unit) {
        unit.LogState("PlayerSelectDestination", "EXIT");

        GameManager.instance.cancelButton.gameObject.SetActive(false);

        unit.HideTiles(unit.validMoves);
    }

}
