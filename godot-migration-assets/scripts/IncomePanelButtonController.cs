using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class IncomePanelButtonController : MonoBehaviour, IPointerClickHandler  {

	// Use this for initialization
	void Start () {

	}

	// 
	public void OnPointerClick (PointerEventData eventData) {
		gVars.uic.ShowPanel("Income", false);
	}

	// Update is called once per frame
	void Update () {

	}
}


