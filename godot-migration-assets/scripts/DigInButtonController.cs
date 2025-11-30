using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class  DigInButtonController : MonoBehaviour, IPointerClickHandler  {

	Button b;

	// Use this for initialization
	void Start () {
		b = GameObject.Find (name).GetComponent<Button>();
	}

	// 
	public void OnPointerClick (PointerEventData eventData) {
		if(!b.interactable)
			return;
			
		Player p = gVars.mc.currentPlayer;
		MapElement me = gVars.mc.currentME;
		if (p == null || me == null || !me.HasUnit())
			return;

		me.funit.DigIn(true);
	}

	// Update is called once per frame
	void Update () {
		Player p = gVars.mc.currentPlayer;
		MapElement me = gVars.mc.currentME;

		if (p == null || me == null || !me.HasUnit())
			{
			b.interactable = false;
			return;	
			}

		// Debug.Log(unittype);
		b.interactable =  !me.funit.HasMoved();
	}

}


