using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NextRoundButtonController : MonoBehaviour, IPointerClickHandler  {

	// Use this for initialization
	void Start () {

	}

	// 
	public void OnPointerClick (PointerEventData eventData) {
		gVars.mc.NextMove(true);
	}

	// Update is called once per frame
	void Update () {

	}
}


