using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

	// members *********************************************************************
	static int instances = 0;

	public Player unitOwner = null;
	// public int moveval = 0, attval = 0, defval = 0,pwrcost=0, inducost=0;
	int ustrength = 0, uexperience = 0, uinstance, pwrcost, inducost;
	public GameObject uRepresentative = null;
	GameObject[] ustars = new GameObject[3], uunits = new GameObject[2]; 
	GameObject ubase = null, ushield = null;
	
	// UnitText uText = null;
	UnitLabel uLabel = null;

	public string uname = "", utype = "";
	public Vector3 targetPos;
	bool hasMoved = true, hasBattled = false, dugIn = false;
	Vector3 oriBaseScale;

	// static Dictionary<string, int> c1 = new Dictionary<string, int> { {"move", 3}, {"att", 80}, {"def", 80} };
	static Dictionary<string, Dictionary<string, int> > unitvalues = new Dictionary<string, Dictionary<string, int> >() { 
		{"Infantry",  new Dictionary<string, int> { {"move", 3}, {"att", 60}, {"def", 100}, {"pwrcost", 20} , {"inducost", 1}, {"xrot", 0}, {"yrot", -90}, {"zrot", 0}  } },
		{"Cavalry",  new Dictionary<string, int> { {"move", 6}, {"att", 100}, {"def", 70}, {"pwrcost", 40} , {"inducost", 5}, {"xrot", 0}, {"yrot", -90}, {"zrot", 0} } },
		{"Artillery",  new Dictionary<string, int> { {"move", 2}, {"att", 150}, {"def", 200}, {"pwrcost", 15} , {"inducost", 50}, {"xrot", 0}, {"yrot", -90}, {"zrot", 0}  } } 
		};

	// Use this for initialization ************************************************
	public void Init (string type, MapElement me, Player p) {
		uinstance = instances++;

		unitOwner = p;
		utype = type;
		pwrcost = unitvalues[type]["pwrcost"];
 		inducost = unitvalues[type]["inducost"];

		// Load Prefab and find subobjects
		GameObject prefb = (GameObject) Resources.Load("prefabs/" + type);
		uRepresentative = Instantiate (prefb, me.GetPosition(), Quaternion.identity);

		for(int i= 0; i < uRepresentative.transform.childCount; i++) {
		 	GameObject go = uRepresentative.transform.GetChild(i).gameObject;
			if(go.name == "Base") {
			  	ubase = go;
				oriBaseScale = ubase.transform.lossyScale;

 				// base klonen und ans Hauptobjekt kleben, Anzeigeplatte etwas größer
				// Instantiate(go, transform, true).transform.SetParent(uRepresentative.transform);
				// ubase.transform.localScale = new Vector3(ubase.transform.lossyScale.x, ubase.transform.lossyScale.y, ubase.transform.lossyScale.z * 19f);

				}
			else if(go.name == "Shield") {
			  	ushield = go;
				}
			else if (go.name.StartsWith("Star")) {
				int j = int.Parse(go.name.Substring(4,1)) - 1;
				ustars[j] = go;
				}
			else if (go.name.StartsWith("Unit")) {
				go.GetComponent<Renderer> ().material.color = unitOwner.GetColor();
				int j = int.Parse(go.name.Substring(4,1)) - 1;
				uunits[j] = go;
				}
			}

		uRepresentative.transform.Rotate(unitvalues[utype]["xrot"], unitvalues[utype]["yrot"], unitvalues[utype]["zrot"]);

		// uText = uRepresentative.AddComponent<UnitText>() as UnitText;
		// uText.Init(uRepresentative);
		uLabel = uRepresentative.AddComponent<UnitLabel>() as UnitLabel;
		uLabel.Init(uRepresentative);

		targetPos = uRepresentative.transform.position;

        setBaseColor(p.color);
		SetStrength(100);
		AddExperience(0);
		DigIn(false);
		HasMoved(true);
	}


	//
	public void OnDestroy () {
		
		unitOwner.RemoveUnit(this);

		if(uRepresentative != null) 
		 	Destroy(uRepresentative);
		// if(uText != null) 
		 	// Destroy(uText);
		if(uLabel != null) 
		 	Destroy(uLabel);
		
	}

	// 
	public bool IsEqualTo (Unit u) {
		return (u.uinstance == uinstance);
	}

	// Sets base transparency to strength
	public void AddExperience(int add) {
		uexperience = Mathf.Min(uexperience + add, 100);

		float[] alpha = new float[3];
		alpha[0] = (float) (uexperience) / 33;
		alpha[1] = (float) Mathf.Max((uexperience - 33) / 33, 0f);
		alpha[2] = (float) Mathf.Max((uexperience - 66) / 33, 0f);

		for(int i =0; i<3; i++){
			Renderer rd = ustars[i].GetComponent<Renderer> ();
			rd.material.color = new Color(rd.material.color.r, rd.material.color.g, rd.material.color.b, alpha[i]);
		}
	}


	//
	public float getExperience() { return (float) (100 + uexperience) / 100f; }


	//		
	public void DigIn(bool b) {
		if(b) {
			dugIn = true;
			ushield.GetComponent<Renderer>().enabled = true;
			HasMoved(true);
		}
		else 
		{
			dugIn = false;
			ushield.GetComponent<Renderer>().enabled = false;
		}
	}


	//		
	public void RepairUnit(int strength) {
		MapElement me = gVars.mc.FindMapElementOfObject(uRepresentative);

		unitOwner.nPower -= getPowerRepairCost(strength); 
		unitOwner.nIndustry -= getInduRepairCost(strength);

		if( me.HasFriendlyUnit(unitOwner) )
			SetStrength(getStrength() + strength);
		else
			SetStrength(getStrength() + (strength / 2) );

		HasMoved(true);
		UpdateLabel();
	}


	// 
	public void UpdateLabel() {
		uLabel.UpdateText(hasMoved);
	}


	// Sets unit strength *********************************
	public void SetStrength(int streng) {
		ustrength = Mathf.Min(streng, 100);
        float fstrength = (float)ustrength / 100;

        uLabel.SetText(ustrength);

		/* Color c = unitOwner.GetColor();
		// c.a =  0.5f + (fstrength / 2);
		float adjust = 0.2f - (fstrength / 5);
		c.r +=  adjust;
		c.g -=  adjust;
		c.b -=  adjust;
		
		uunits[0].GetComponent<Renderer>().material.color = c;
		uunits[1].GetComponent<Renderer>().material.color = c;
		*/

		//go.transform.parent = uRepresentative.transform;
		//go.transform.rotation.x = 20;

		// rd.material.color = new Color(alpha, alpha, alpha, alpha);
		// rd.material.color = new Color(unitOwner.GetColor().r * alpha, unitOwner.GetColor().g * alpha, unitOwner.GetColor().b * alpha, alpha);
		// rd.material.color = new Color(rd.material.color.r * alpha, rd.material.color.g * alpha, rd.material.color.b * alpha, alpha);
	}


    //
    public int getStrength() { return ustrength; }
    
	
    //
	public bool HasMoved() { return hasMoved; }

    //
    public bool HasMoved(bool b)
    {
        // OLD solution: mark moved by unit's color, NEW: by base color
        /*Color c = unitOwner.GetColor();
		//float f = -0.4f;
		hasMoved = b;

        if (hasMoved)
        {
            c.r += f; c.g += f; c.b += f;
        }

        Color c1 = new Color(0f, 0f, 0f, 1f);
        uunits[0].GetComponent<Renderer>().material.color = c;
        uunits[1].GetComponent<Renderer>().material.color = c;

        gVars.mc.UnmarkTiles();*/

        hasMoved = b;
        UpdateLabel();
        return hasMoved; 
		}


    void setBaseColor()
    {
        float basecolor = 0.3f;
        float r = basecolor;// / fstrength;
        float g = basecolor;
        float b = basecolor;
        float alpha = 0.7f; // (fstrength / 4f) + 0.2f;

        Renderer rd = ubase.GetComponent<Renderer>();
        rd.material.color = new Color(r, g, b, alpha);
     } 

    void setBaseColor(Color c)
    {
        float basecolor = -0.1f;
        float r = basecolor + c.r;// / fstrength;
        float g = basecolor + c.g;
        float b = basecolor + c.b;
        float alpha = 0.5f; // (fstrength / 4f) + 0.2f;

        Renderer rd = ubase.GetComponent<Renderer>();
        rd.material.color = new Color(r, g, b, alpha);
     } 


    //
    public bool HasBattled() { return hasBattled; }


    //
    public bool HasBattled(bool b) { return hasBattled = b; }


    // 
    public void TurnTo(float angle) {
        // Debug.Log("angle: "+  angle);
        // Debug.Log("before: " + uRepresentative.transform.eulerAngles);
        uRepresentative.transform.rotation 
			= Quaternion.Euler(uRepresentative.transform.eulerAngles.x, angle + unitvalues[utype]["yrot"], uRepresentative.transform.eulerAngles.z);
        // Debug.Log("after: " + uRepresentative.transform.eulerAngles);
	}

	// 
	public int getMoveval(bool isOnLand) {
		if(isOnLand)
			return unitvalues[utype]["move"];
		// if on water, always move 3
		return 3;
	}

	// 
	public int getAttPower() {
		return unitvalues[utype]["att"];
	}

	// 
	public int getDefPower() {
		int addPower = dugIn ? unitvalues[utype]["def"] / 2 : 0;
		return unitvalues[utype]["def"] + addPower;
	}

	// 
	public  int getPowerCost() {
		return pwrcost;
	}

	// 
	public  int getInduCost() {
		return inducost;
	}

	// 
	public static int getPowerCost(string type) {
		return unitvalues[type]["pwrcost"];
	}

	// 
	public static int getInduCost(string type) {
		return unitvalues[type]["inducost"];
	}

	// repairLevel: How many percent should be repaired
	public int getPowerRepairCost(int repairLevel) {
		return pwrcost / (200 / repairLevel);
	}

	// 
	public int getInduRepairCost(int repairLevel) {
		return inducost / (200 / repairLevel);
	}

	// 
	public GameObject getRepresentative() {
		return uRepresentative;
	}

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
		// improvement might be to check here whether movement has occured
		uLabel.SetPos(new Vector3(uRepresentative.transform.position.x + 0, uRepresentative.transform.position.y + 0.1f, uRepresentative.transform.position.z - 0.15f));
	}

}




	    // Legacy - color base showing strength and move state *************************

        /* tried scaling base due to unit strength
        float scaleLoss = 0.80f + (fstrength / 5);
        ubase.transform.localScale = new Vector3(oriBaseScale.x * scaleLoss, oriBaseScale.y * scaleLoss, oriBaseScale.z);
        */ 

        /* tried scaling units due to unit strength
        Vector3 v = uunits[0].transform.lossyScale;
        uunits[0].transform.localScale = new Vector3(v.x * scaleLoss, v.y * scaleLoss, v.z * scaleLoss);
        */


