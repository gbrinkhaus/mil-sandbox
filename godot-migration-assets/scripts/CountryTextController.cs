using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CountryTextController : MonoBehaviour, IPointerClickHandler
{

    // Use this for initialization
    void Start()
    {

    }

    // 
    public void OnPointerClick(PointerEventData eventData)
    {

        Text t = gVars.uic.countrySwitch.GetComponent<Text>();
        int i;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if ((i = gVars.countries.IndexOf(t.text)) != -1)
            {
                if (++i >= gVars.countries.Count)
                    i = 0;
            }
            else
            {
                i = 0;
            }

            t.text = gVars.countries[i];
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            t.text = "";
        }

    }


    // Update is called once per frame
    void Update()
    {

    }
}
