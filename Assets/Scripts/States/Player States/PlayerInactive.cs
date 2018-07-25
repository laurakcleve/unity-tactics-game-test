using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInactive : State {

	public override void Enter(PlayerUnit unit) {
		// unit.LogState("PlayerInactive", "ENTER");

        GameManager.instance.moveButton.onClick.RemoveAllListeners();
		GameManager.instance.actButton.onClick.RemoveAllListeners();
        GameManager.instance.cancelButton.onClick.RemoveAllListeners();
        GameManager.instance.confirmButton.onClick.RemoveAllListeners();
        GameManager.instance.endTurnButton.onClick.RemoveAllListeners();

		unit.Turn.DepopulateAbilities();

        unit.gameObject.transform.Find("Spotlight").gameObject.SetActive(false);
	}

	public override void Exit(PlayerUnit unit) {
        // unit.LogState("PlayerInactive", "EXIT");

		GameManager.instance.moveButton.onClick.AddListener(delegate { unit.Turn.ClickMove(unit); });
		GameManager.instance.actButton.onClick.AddListener(delegate { unit.Turn.ClickAct(unit); });
		GameManager.instance.cancelButton.onClick.AddListener(delegate { unit.Turn.Cancel(unit); });
		GameManager.instance.confirmButton.onClick.AddListener(delegate { unit.Turn.Confirm(unit); });
		GameManager.instance.endTurnButton.onClick.AddListener(unit.EndTurn);

		unit.Turn.PopulateAbilities(unit);

		unit.gameObject.transform.Find("Spotlight").gameObject.SetActive(true);

		GameManager.instance.moveButton.interactable = true;
        GameManager.instance.actButton.interactable = true;
	}
	
}
