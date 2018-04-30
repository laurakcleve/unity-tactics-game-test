using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInactive : State {

	public override void Enter(PlayerUnit unit) {
		unit.LogState("PlayerInactive", "ENTER");

        GameManager.instance.moveButton.onClick.RemoveAllListeners();
		GameManager.instance.actButton.onClick.RemoveAllListeners();
        GameManager.instance.cancelButton.onClick.RemoveAllListeners();
        GameManager.instance.confirmButton.onClick.RemoveAllListeners();
        GameManager.instance.endTurnButton.onClick.RemoveAllListeners();

		unit.DepopulateAbilities();

        unit.gameObject.transform.Find("Spotlight").gameObject.SetActive(false);
	}

	public override void Exit(PlayerUnit unit) {
        unit.LogState("PlayerInactive", "EXIT");

		GameManager.instance.moveButton.onClick.AddListener(unit.ClickMove);
		GameManager.instance.actButton.onClick.AddListener(unit.ClickAct);
		GameManager.instance.cancelButton.onClick.AddListener(unit.Cancel);
		GameManager.instance.confirmButton.onClick.AddListener(unit.Confirm);
		GameManager.instance.endTurnButton.onClick.AddListener(unit.EndTurn);

		unit.PopulateAbilities();

		unit.gameObject.transform.Find("Spotlight").gameObject.SetActive(true);

		GameManager.instance.moveButton.interactable = true;
	}
	
}
