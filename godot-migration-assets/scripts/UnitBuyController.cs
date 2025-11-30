using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UnitBuyController : MonoBehaviour, IPointerClickHandler  {

	int powercost, inducost;
	Button button;
	string unittype;
	ColorBlock colorNormal;
	ColorBlock colorCanBuy;

	// Use this for initialization ******************
	void Start () {
		button = GameObject.Find (name).GetComponent<Button>();
		unittype = name.Substring(0, name.IndexOf("_"));

		colorNormal = button.colors;
		colorCanBuy = button.colors;
		colorCanBuy.disabledColor = new Color(0.2f, 0.2f, 0.2f, 1);

		powercost = Unit.getPowerCost(unittype);
		inducost = Unit.getInduCost(unittype);
	}

	// OnClick ***************************************
	public void OnPointerClick (PointerEventData eventData) {
		if(!button.interactable)
			return;
			
		Player p = gVars.mc.currentPlayer;
		MapElement me = gVars.mc.currentME;
		if (p == null || me == null)
			return;

		if(me.AddUnit(p, unittype)) {
			p.nPower -= powercost; 
			p.nIndustry -= inducost;
		}

		gVars.uic.UpdateMoneyDisplay();
	}

	// Update is called once per frame *****************
	void Update () {
		
		if (gVars.mc.currentPlayer == null )
			return;

		bool canBuy = gVars.mc.currentPlayer.nPower >= powercost && gVars.mc.currentPlayer.nIndustry >= inducost;

		MapElement me = gVars.mc.currentME;
		if (me == null) 
			return;
		// only enable button when on city and enough money
		button.interactable = me.IsCity() && !me.HasUnit() && me.fowner == gVars.mc.currentPlayer && canBuy;

		if(canBuy && !button.interactable)
			button.colors = colorCanBuy;
		else
        	button.colors = colorNormal;

	}

}


