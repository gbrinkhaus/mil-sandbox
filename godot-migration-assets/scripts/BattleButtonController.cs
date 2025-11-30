using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BattleButtonController : MonoBehaviour, IPointerClickHandler  {

	int powercost, inducost;
	Button b;
	string unittype;

	// Use this for initialization
	void Start () {
		b = GameObject.Find (name).GetComponent<Button>();
	}

	// 
	public void OnPointerClick (PointerEventData eventData) {
		gVars.mc.ConductBattle();
	}

	// Update is called once per frame
	void Update () {
		b.interactable = gVars.mc.attField != null && gVars.mc.defField != null;
		gVars.uic.ShowDefenderPanel(b.interactable);
	}

}


