using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MinusbuttonController : MonoBehaviour, IPointerClickHandler {

	private Text t;

	// Use this for initialization
	void Start () {
		t = GameObject.Find ("Playernumber").GetComponent<Text>();
	}

	// 
	public void OnPointerClick (PointerEventData eventData) {
//		Debug.Log ("-");

		int number = int.Parse (t.text) - 1;
		if (number > 0) {
			t.text = number.ToString ();
		}
	}

	// Update is called once per frame
	void Update () {

	}
}
