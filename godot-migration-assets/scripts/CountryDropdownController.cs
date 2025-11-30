using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountryDropdownController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Dropdown m_Dropdown;
        m_Dropdown = GetComponent<Dropdown>();

        m_Dropdown.ClearOptions();
        //Add the options created in the List above
        m_Dropdown.AddOptions(gVars.countries);
        //Add listener for when the value of the Dropdown changes, to take action
        m_Dropdown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(m_Dropdown);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Ouput the new value of the Dropdown into Text
    void DropdownValueChanged(Dropdown change)
    {
        gVars.uic.countrySwitch.text =  gVars.countries[change.value];
    }
}
