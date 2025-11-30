using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TechNode : MonoBehaviour
{
    public string technologyName;

    // Start is called before the first frame update
    void Start()
    {
    }

    void Init(string name)
    {
        // Get the tech node's text UI element.
        technologyName = name;
    }

}
