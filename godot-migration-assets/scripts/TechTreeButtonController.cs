using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TechTreeButtonController : MonoBehaviour, IPointerClickHandler
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("tt button clicked");
        gVars.uic.ShowPanel("TechTree", true);
    }

}
