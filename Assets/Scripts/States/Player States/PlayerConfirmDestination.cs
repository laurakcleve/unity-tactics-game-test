using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerConfirmDestination : State {

    public override void Enter(PlayerUnit unit) {
        // unit.LogState("PlayerConfirmDestination", "ENTER");

		GameManager.instance.cancelButton.gameObject.SetActive(true);
		GameManager.instance.confirmButton.gameObject.SetActive(true);
    }

    public override void Exit(PlayerUnit unit) {
        // unit.LogState("PlayerConfirmDestination", "EXIT");

        GameManager.instance.cancelButton.gameObject.SetActive(false);
        GameManager.instance.confirmButton.gameObject.SetActive(false);
    }
	
}
