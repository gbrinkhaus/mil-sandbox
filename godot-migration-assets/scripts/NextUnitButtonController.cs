using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NextUnitButtonController : MonoBehaviour, IPointerClickHandler  {

	Button b;

	// Use this for initialization
	void Start () {
		b = GameObject.Find (name).GetComponent<Button>();
	}

	// 
	public void OnPointerClick (PointerEventData eventData) {
		if(!b.interactable)
			return;
			
		Player p;
		Unit u;

		if((p = gVars.mc.currentPlayer) != null)
			 if((u = p.GetNextMovableUnit() )!= null)
				gVars.mc.SelectME(u);
	}

	// Update is called once per frame
	void Update () {
		Player p = gVars.mc.currentPlayer;

		if (p == null)
			{
			b.interactable = false;
			return;	
			}
		
		b.interactable = p.GetNextMovableUnit() != null;
	}

}


