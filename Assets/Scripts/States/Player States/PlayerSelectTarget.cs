using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelectTarget : State {

    public override void Enter(PlayerUnit unit) {
        unit.LogState("PlayerSelectTarget", "ENTER");

        GameManager.instance.cancelButton.gameObject.SetActive(true);
    }

    public override void Exit(PlayerUnit unit) {
        unit.LogState("PlayerSelectTarget", "EXIT");

        unit.HideTiles(unit.validAbilityTargets);

        GameManager.instance.cancelButton.gameObject.SetActive(false);
    }
}
