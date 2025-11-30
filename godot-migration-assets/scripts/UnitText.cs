// using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// OBSOLETE?

public class UnitText : MonoBehaviour
{
    bool isShown = false;
    GameObject textObject;
    int tval = 0;
    Vector3 targetPos;
    System.DateTime startTime = System.DateTime.UtcNow;

    // Start is called before the first frame update
    void Start()
    {
        
    }

	//
	public void OnDestroy () {
		
		if(textObject != null) 
		 	Destroy(textObject);
	}


    // 
    public void Init(GameObject uRepresentative)
    {
        textObject = (GameObject) Instantiate ( Resources.Load("prefabs/UnitText"), uRepresentative.transform.position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        if(isShown)
        {
            float msElapsed = ( System.DateTime.UtcNow.Second - startTime.Second ) * 1000 + System.DateTime.UtcNow.Millisecond - startTime.Millisecond;
            float dur = 2500.0f;
            textObject.SetActive(true);
            gVars.mc.SmoothMove3D(textObject, targetPos, (int) dur / 10); // gVars.unitSpeedDivisor);
            gVars.uic.SetColor(textObject, 0.0f, 0.0f, 0.0f, System.Math.Max(1.0f - (msElapsed/dur), 0.0f)  );

    //       Debug.Log("Updated Pos (UnitText): ");
      //     Debug.Log(textObject.transform.position);
        //    Debug.Log(textObject.transform.rotation);

            /*Renderer r = textObject.GetComponent<Renderer> ();
            Color c = r.material.color;
            c.a = 1.0f - ((float) msElapsed / ( dur) );
            r.material.color = c; */

            if(msElapsed > dur) {
                textObject.SetActive(false);
                isShown = false;
            }
        }
        
        // textObject.transform.Rotate(100 * Time.deltaTime, 0, 0);
    }

    // Show text at parent object's position
    public void ShowText(/*Transform t*/ Vector3 t, int value) 
    {
        tval = value;
        isShown = true;
        startTime = System.DateTime.UtcNow;

        textObject.GetComponent<TextMesh>().text = tval.ToString();
        textObject.transform.position = t;
        targetPos = new Vector3(t.x, t.y + 5, t.z);

        // gVars.uic.SetColor(textObject, 1.0f, 1.0f, 1.0f, 1.0f);

        gVars.uic.SetColor(textObject, 0.0f, 0.0f, 0.0f, 1.0f );

    }
   
}
