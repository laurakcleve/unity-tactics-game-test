using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMenus : State {

    public override void Enter(PlayerUnit unit) {
        unit.LogState("PlayerMenus", "ENTER");

        GameManager.instance.cancelButton.gameObject.SetActive(true);
    }

    public override void Exit(PlayerUnit unit) {
        unit.LogState("PlayerMenus", "EXIT");

        GameManager.instance.cancelButton.gameObject.SetActive(false);
        GameManager.instance.abilitiesCanvas.SetActive(false);
    }

}
