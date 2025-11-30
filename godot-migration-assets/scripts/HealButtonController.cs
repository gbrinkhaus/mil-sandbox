using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class  HealButtonController : MonoBehaviour, IPointerClickHandler  {

	Button b;
	int repairLevel = 50;

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

		me.funit.RepairUnit(repairLevel);
		gVars.uic.UpdateMoneyDisplay();
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

		int powercost = me.funit.getPowerRepairCost(repairLevel), inducost = me.funit.getInduRepairCost(repairLevel);

		// Debug.Log(unittype);
		b.interactable =  me.funit.getStrength() < 100 && !me.funit.HasMoved() && !me.funit.HasBattled() && me.fowner == p && p.nPower >= powercost && p.nIndustry >= inducost ;
	}

}


