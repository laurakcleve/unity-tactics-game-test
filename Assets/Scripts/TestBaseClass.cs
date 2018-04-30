using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBaseClass : MonoBehaviour {

	[SerializeField]
	protected List<TestOtherClass> testField;

	void Awake () {
	}
}
