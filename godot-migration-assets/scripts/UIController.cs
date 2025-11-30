using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{

	private InputField fieldCoal;

	private GameObject selectPanel, preparePanel, playerPanel, topPanel, defenderPanel, incomePanel, techTreePanel;
    public Text playnumText, prepareText, fieldName, fieldCity, fieldCountry, fieldIndex, powerText, induText, fieldPower;
	public Text fieldIndu, countrySwitch, playerText, playerIcon, infantryText, cavalryText, artyText;
	Text incomePlayerText, incomePlayerIcon, incomeText, exp1Text, exp2Text, incomeSumText;
	public Text unitType, unitName, unitStrength, unitMove, defenderType, defenderName, defenderStrength;
    public InputField induInput, pwrInput, cityInput;

	// Use this for initialization
	void Start ()
	{
		selectPanel = FindInactiveObject ("Canvas", "SelectPlayersPanel");
		preparePanel = FindInactiveObject ("Canvas", "PreparePanel");
		playerPanel = FindInactiveObject ("Canvas", "PlayerPanel");
		// infoPanel = FindInactiveObject (playerPanel, "InfoPanel");
		topPanel = FindInactiveObject ("Canvas", "TopPanel");
		incomePanel = FindInactiveObject ("Canvas", "IncomePanel");

        techTreePanel = FindInactiveObject("AltCanvas", "TechTreePanel");

        ShowAllPanels(true);

// IncomePanel **************************
        incomePlayerText = GameObject.Find ("IncomePlayernrText").GetComponent<Text> ();
        incomePlayerIcon = GameObject.Find ("IncomePlayerIcon").GetComponent<Text> ();

        incomeText = GameObject.Find ("Income").GetComponent<Text> ();
        exp1Text = GameObject.Find ("Expenses").GetComponent<Text> ();
        exp2Text = GameObject.Find ("Expenses2").GetComponent<Text> ();
        incomeSumText = GameObject.Find ("Sum").GetComponent<Text> ();

// InfoPanel **************************
        playnumText = GameObject.Find ("Playernumber").GetComponent<Text> ();
		prepareText = GameObject.Find ("PrepareText").GetComponent<Text> ();
		powerText = GameObject.Find ("PlayerPowerText").GetComponent<Text> ();
		induText = GameObject.Find ("PlayerInduText").GetComponent<Text> ();
        playerText = GameObject.Find ("PlayernrText").GetComponent<Text> ();
        playerIcon = GameObject.Find ("PlayerIcon").GetComponent<Text> ();

		infantryText = GameObject.Find ("InfCostText").GetComponent<Text> ();
        cavalryText = GameObject.Find ("CavCostText").GetComponent<Text> ();
        artyText = GameObject.Find ("ArtCostText").GetComponent<Text> ();
		infantryText.text = Unit.getPowerCost("Infantry") + "\n" + Unit.getInduCost("Infantry");
		cavalryText.text = Unit.getPowerCost("Cavalry") + "\n" + Unit.getInduCost("Cavalry");
		artyText.text = Unit.getPowerCost("Artillery") + "\n" + Unit.getInduCost("Artillery");


		fieldCity = GameObject.Find ("FieldCity").GetComponent<Text> ();
		fieldCountry = GameObject.Find ("FieldCountry").GetComponent<Text> ();
		fieldPower = GameObject.Find ("FieldPower").GetComponent<Text> ();
		fieldIndu = GameObject.Find ("FieldIndu").GetComponent<Text> ();

		unitType = GameObject.Find ("UnitType").GetComponent<Text> ();
		unitName = GameObject.Find ("UnitName").GetComponent<Text> ();
		unitStrength = GameObject.Find ("UnitStrength").GetComponent<Text> ();
		unitMove = GameObject.Find ("UnitMove").GetComponent<Text> ();

		defenderPanel = GameObject.Find ("DefenderPanel");
		defenderType = GameObject.Find ("DefenderType").GetComponent<Text> ();
		defenderName = GameObject.Find ("DefenderName").GetComponent<Text> ();
		defenderStrength = GameObject.Find ("DefenderStrength").GetComponent<Text> ();

// TopPanel **************************
		fieldIndex = GameObject.Find ("FieldIndex").GetComponent<Text> ();
		fieldName = GameObject.Find ("FieldName").GetComponent<Text> ();
        countrySwitch = GameObject.Find("FieldCountrySwitch").GetComponent<Text>();

        pwrInput = GameObject.Find("FieldPowerInput").GetComponent<InputField>();
        induInput = GameObject.Find("FieldInduInput").GetComponent<InputField>();
        cityInput = GameObject.Find("FieldCityInput").GetComponent<InputField>();


        fieldCity.text = fieldCountry.text = "";

        ShowAllPanels(false);

        selectPanel.SetActive (true);

		gVars.uic = this;
	}

	// Update is called once per frame
	void Update ()
	{
		
	}

	//
	public void UpdateMoneyDisplay() {
        powerText.text = gVars.mc.currentPlayer.nPower.ToString();
        induText.text = gVars.mc.currentPlayer.nIndustry.ToString();
	}

	//
	public void UpdateIncomePanel(int nCountries, int nPower, int nIndustry, int nHomeUnits, int nHomePExp, int nHomeIExp, int nAwayUnits, int nAwayPExp, int nAwayIExp) {
		incomePlayerText.text = "Player " + gVars.mc.currentPlayer.playernumber.ToString();
		incomePlayerIcon.color = gVars.mc.currentPlayer.GetColor();

        incomeText.text = "Income\n\nCountries: " + nCountries + "\n Manpower: +" + nPower + "\nIndustrial Power: +" + nIndustry;
        exp1Text.text  = "Def. Expenses\n\nGuard units: " + nHomeUnits + "\nManpower: -" + nHomePExp + "\nIndustrial Power: -" + nHomeIExp;
        exp2Text.text  = "Off. Expenses\n\nOffensive units: " + nAwayUnits + "\nManpower: -" + nAwayPExp + "\nIndustrial Power: -" + nAwayIExp;
        incomeSumText.text = "Sum\n\n\nManpower: +" + (nPower - nHomePExp - nAwayPExp) + "\nIndustrial Power: +" + (nIndustry - nHomeIExp - nAwayIExp);
	}

	//
	void ShowAllPanels (bool show)
	{
		selectPanel.SetActive (show);
		preparePanel.SetActive (show);
		playerPanel.SetActive (show);
		topPanel.SetActive (show);
        incomePanel.SetActive(show);
        techTreePanel.SetActive(show);
    }

    public void ShowDefenderPanel (bool b)
	{
		if(b && gVars.mc.defField != null && gVars.mc.defField.HasUnit() ) {
			defenderType.text = gVars.mc.defField.funit.utype;
			defenderName.text = gVars.mc.defField.funit.uname;
			defenderStrength.text = gVars.mc.defField.funit.getStrength().ToString() + " %"; 
			}

		defenderPanel.SetActive (b);
	}

	public void ShowCurrentPLayer (Player p) 
	{
		playerText.text = "Player " + p.playernumber.ToString();

		// playerText.color = gVars.mc.currentPlayer.GetColor();
		playerIcon.color = gVars.mc.currentPlayer.GetColor();

	}


	public void ShowPanel (string panel, bool onOff)
	{
		ShowAllPanels(false);
		selectPanel.SetActive (false);
		preparePanel.SetActive (false);
		topPanel.SetActive (true);
		playerPanel.SetActive (false);
		incomePanel.SetActive (false);

        switch (panel) {
			case "Select":
				selectPanel.SetActive (onOff);
				break;
			case "Prepare":
				preparePanel.SetActive (onOff);
				break;
			case "Income":
				incomePanel.SetActive (onOff);
				playerPanel.SetActive (true);
				break;
			case "Player":
				playerPanel.SetActive (onOff);
				defenderPanel.SetActive (false);
				break;
			case "TechTree":
				techTreePanel.SetActive(onOff);
				break;
		}

    }

	//
	GameObject FindInactiveObject (string objparent, string objname)
	{
		GameObject go = GameObject.Find(objparent).transform.Find(objname).gameObject;
		if (go == null)
			Debug.Log("Obj: " + objname + " not found!");
        return go;
	}

	//
	GameObject FindInactiveObject (GameObject objparent, string objname)
	{
		return objparent.transform.Find (objname).gameObject;
	}


	// 
	public void ChangeColor(GameObject go, float r, float g, float b, float a)
	{
		Renderer rd = go.GetComponent<Renderer> ();
        Color c = rd.material.color;
//        Color c = new Color( rd.material.color.r, rd.material.color.g, rd.material.color.b, rd.material.color.a );

		c.r += r; c.g += g; c.b += b; c.a += a;
        rd.material.color = c;
	}


	// 
	public void SetColor(GameObject go, float r, float g, float b, float a)
	{
		Renderer rd = go.GetComponent<Renderer> ();
        Color c = new Color( r, g, b, a );

		// c.r = r; c.g = g; c.b = b; c.a = a;
        rd.material.color = c;
	}

}
