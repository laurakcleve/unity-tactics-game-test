using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ability", menuName = "Ability")]
public class Ability : ScriptableObject {

	public new string name;
	public int damage;
	public int range;
	public TargetType targetType;

	[SerializeField]
	public List<string> effectTiles = new List<string>();




	public void Initiate(PlayerUnit unit) {
		Debug.Log("Initiating " + name + " with " + unit.name);
		
	}

	public void Execute() {
		Debug.Log("Executing " + name);
	}

}

public enum TargetType {
    Self, Ally, Enemy, Area, AreaNotSelf
}
