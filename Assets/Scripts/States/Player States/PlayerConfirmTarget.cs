using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerConfirmTarget : State {

    public override void Enter(PlayerUnit unit) {
        // unit.LogState("PlayerConfirmTarget", "ENTER");

        GameManager.instance.cancelButton.gameObject.SetActive(true);
        GameManager.instance.confirmButton.gameObject.SetActive(true);

        if (unit.Turn.lastHoveredEffectTiles != null)
            unit.Turn.EnableTileHighlights(unit.Turn.lastHoveredEffectTiles);
    }

    public override void Exit(PlayerUnit unit) {
        // unit.LogState("PlayerConfirmTarget", "EXIT");

        GameManager.instance.cancelButton.gameObject.SetActive(false);
        GameManager.instance.confirmButton.gameObject.SetActive(false);
    }

}
