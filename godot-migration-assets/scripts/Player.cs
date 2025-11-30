//using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ****************************************************************
public class Player {

	public bool isHuman = false;
	public Color color = Color.white;
	public Color bgColor = Color.white;
	public int nPower = 0, nIndustry = 0;
	public int playernumber = 0;
	public List<Unit> units; 

	public Player(int i) {
		playernumber = i;
		units = new List<Unit>(); 

		switch(i) {
		case 1: 
			color = new Color (0.5F, 0.5F, 1.0F, 1.0F);
			bgColor = new Color (0.5F, 0.6F, 0.9F, 1.0F);
			break;
		case 2: 
			color = new Color (0.8F, 0.7F, 0.2F, 1.0F);
			bgColor = new Color (0.8F, 0.7F, 0.2F, 1.0F);
			break;
		case 3: 
			color = new Color (0.8F, 0.2F, 0.8F, 1f);
			bgColor = new Color (0.8F, 0.2F, 0.8F, 1f);
			break;
		case 4: 
			color = new Color (0.8F, 0.2F, 0.2F, 1f);
			bgColor = new Color (0.7F, 0.3F, 0.3F, 1.0f);
			break;
		// neutral player
		case 99: 
			color = new Color (0.7F, 0.7F, 0.7F, 1f);
			bgColor = new Color (0.7F, 0.7F, 0.7F, 1.0f);
			break;
		}
	}

	// player color ****************************
	public void AddUnit(Unit u) {
		units.Add(u);
	}

	// player color ****************************
	public void RemoveUnit(Unit u) {
		if(!units.Remove(u))
			Debug.Log("Could not find and remove unit: " + u.ToString() );
	}

	// player color ****************************
	public Unit  GetNextMovableUnit() {
		MapElement me = gVars.mc.currentME;
		Unit u = null, usearch = null;
		int i = 0;
		
		if( me != null && me.HasFriendlyUnit(this) ) {
			u = me.funit;
			i = units.FindIndex(match: u.IsEqualTo);
			}
		// if not found, index will be negative, so make safe
		if(i < 0 || i == units.Count - 1)
			i = 0;

		for(; i < units.Count; i++) {
			if(!units[i].HasMoved())
				if(u == null || !units[i].IsEqualTo(u) ) {
					usearch = units[i];
					break;
					}
		}

		return usearch;
	}


    //
    public void CollectIncome()
    {
        int nPower = 0, nIndustry = 0, nRepair = 20, nCountries = 0;
		int nHomeUnits= 0, nHomePExp = 0, nHomeIExp = 0;
		int nAwayUnits = 0, nAwayPExp = 0, nAwayIExp = 0;

        // Collect Income from cities
        for (int i = 0; i < gVars.gridSizeX; i++)
            for (int j = 0; j < gVars.gridSizeY; j++) {
                if (gVars.mc.IsCity(i, j) && gVars.mc.GetPlayerNumber(i, j) == this.playernumber)
                {
                    nCountries++;
					nPower += gVars.mc.GetFieldPower(i, j, gVars.incomeTypePower);
                    nIndustry += gVars.mc.GetFieldPower(i, j, gVars.incomeTypeIndu);
                }
        
                if (gVars.mc.HasFriendlyUnit(i, j, this))
                {
					Unit u = gVars.mc.GetUnit(i, j);
					u.RepairUnit(nRepair);

					if(gVars.mc.IsFriendlyTo(i, j, this)) {
						nHomeUnits ++;
						nHomePExp += u.getPowerCost()/nRepair;
						nHomeIExp += u.getInduCost()/nRepair;
					}
					else {
						nAwayUnits++;
						// Away units more expensive to maintain
						nAwayPExp += u.getPowerCost()/ (nRepair/2);
						nAwayIExp += u.getInduCost()/ (nRepair/2);
					}
                }
			}

        // Debug.Log("Player " + this.playernumber + " nr of units. " + this.units.Count + " Collect income - pwr: " + nPower + " - indu: " + nIndustry);
		
        // pay nrepair% for units
        // alte schleife: for (int i = 0; i < this.units.Count; i++)
        
        // Debug.Log(" Income after units - pwr: " + nPower + " - indu: " + nIndustry);
        
		gVars.uic.UpdateIncomePanel(nCountries, nPower, nIndustry, nHomeUnits, nHomePExp, nHomeIExp, nAwayUnits, nAwayPExp, nAwayIExp);

        AddIncome(nPower - nHomePExp - nAwayPExp, nIndustry - nHomeIExp - nAwayIExp);
    }


	//  ****************************
	public void AddIncome(int pwr, int indu) {
		nPower += pwr;
		nIndustry += indu;

		gVars.uic.UpdateMoneyDisplay();
    }

    // player color ****************************
    public Color GetColor() {
		return color;
	}

	// player color ****************************
	public Color GetBGColor() {
		return bgColor;
	}

}
