using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EditMapController : MonoBehaviour, IPointerClickHandler {

	private Toggle t;

	// Use this for initialization
	void Start () {
		t = GameObject.Find ("ToggleMapEdit").GetComponent<Toggle>();
	}
	
	public void OnPointerClick (PointerEventData eventData) {

		gVars.mc.setMapEdit(t.isOn);
	}


	// Update is called once per frame
	void Update () {
		
	}
}
