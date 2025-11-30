using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Runtime.Serialization.Formatters.Binary;


// MAPCONTROLLER ********************************************************************************************************************************************
public class MapController : MonoBehaviour
{
    Player[] players = new Player[gVars.numPlayers];
    public Player currentPlayer = null;
    public Player neutralPlayer = null;

    MapElement[,] mapFields = new MapElement[gVars.gridSizeX, gVars.gridSizeY];
    public MapElement currentME = null, attField = null, defField = null;

    Camera mainCam;
    GameObject fieldSpot;
    GameObject worldCube, gameBoard, gameBoard2;

    Toggle lightToggle, camToggle, infiniToggle;

    Vector3 ptarget = new Vector3(0, gVars.mapYLevel, 0);

    float xSpeed, zSpeed;

    bool moveCam = false, moveSpot = false;
    bool mapInitialized = false;

    int numHumanPlayers;
    int countMapSwitch = 0;

    string gameMode = "";
    Unit movingUnit = null;

    Material lightSea, darkSea;

    // Use this for initialization *******************************
    void Start()
    {
        // get Object refs
        mainCam = Camera.main;
        fieldSpot = GameObject.Find("Fieldspot");
        lightToggle = GameObject.Find("ToggleLightMove").GetComponent<Toggle>();
        camToggle = GameObject.Find("ToggleCamMove").GetComponent<Toggle>();
        infiniToggle = GameObject.Find("ToggleInfiniMove").GetComponent<Toggle>();
        gVars.mc = this;
        worldCube = GameObject.Find("WorldCube");
        gameBoard = worldCube.transform.Find("Gameboard").gameObject;
        gameBoard2 = worldCube.transform.Find("Gameboard2").gameObject;
        // lightSea = Resources.Load("materials/WaterBasicDaytime", typeof(Material)) as Material;
        // darkSea = Resources.Load("materials/WaterBasicNighttime", typeof(Material)) as Material;
        

        // Load Map and intitialize ***********
        string[] mapList = LoadMap();

        if (mapList != null && mapList.Length > 0)
        {
            // Debug.Log( mapFields.Length );
            for (int i = 0; i < gVars.gridSizeX; i++)
            {
                for (int j = 0; j < gVars.gridSizeY; j++)
                {
                    string fieldParams = mapList[(i) * (gVars.gridSizeY) + (j)];

                    // this loop was used for duplication purposes, but we can let it stand for now
                    for(int x=0; x<=0; x++)
                        for(int y=0; y<=0; y++)                    
                        {
                            //if(j==90 && i == 22)
                                // Debug.Log("Writing to: x: " + (i+x).ToString() + ", y: " + (j+y).ToString());

                            MapElement me = worldCube.AddComponent<MapElement>() as MapElement;
                            me.Init(i+x, j+y, int.Parse(GetMapValue(fieldParams, "fld")), GetMapValue(fieldParams, "ctr"), 
                                GetMapValue(fieldParams, "cty"), worldCube);
                            mapFields[i+x, j+y] = me;
                        }
                }
            }
        }

        if (mapFields != null && mapFields.Length > 0)
            mapInitialized = true;

    } // Start()


    // UPDATE *******************************
    void Update()
    {
        // Debug.Log ("MapController");

        // MOUSEHANDLER -- IsPointerOverGameObject means over UI
        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                HandleMapClick();
            }
        }

        CheckCamMove();
        CheckSpotMove();
        CheckUnitMove();
        // CheckButtonState();

    } // Update()


    //
    public void OnDestroy()
    {
        // if(lightSea != null)
        // 	Destroy(lightSea);
        // if(darkSea != null)
        // 	Destroy(darkSea);
    }

    // GAME CONTROLLER ********************************************************************************************************************************************

    // 
    void StartRound()
    {
        gVars.uic.ShowPanel("Player", true);
        gameMode = "normalPlay";

        // first player, so dont count up
        NextMove(false);
    }


    // 
    public void NextMove(bool stepUp)
    {

        if (stepUp)
            currentPlayer = GetNextPlayer(currentPlayer);
//        else
//            currentPlayer = players[0];
            
        currentPlayer.CollectIncome();
        gVars.uic.ShowPanel("Income", true);

        ResetPlayerMoves();
        gVars.uic.ShowCurrentPLayer(currentPlayer);
    }


    //BATTLEFUNKTIONEN ********************************************************************************************************************************************

    // 
    void CaptureCity(MapElement me)
    {
        SetCountryOwner(me.fcountry, currentPlayer);
        RefreshMap();
    }


    void RaiseRebels(MapElement me) 
        {
            // choose a random player for the rebels to join
            System.Random rnd = new System.Random();
            int newOwner = rnd.Next(1, gVars.numPlayers-1);
            // this is done to not select the attacking player himself
            if(newOwner >= currentPlayer.playernumber)
                newOwner += currentPlayer.playernumber;
            
            Player newPlayer = null;

            if(newOwner <= gVars.numPlayers) 
                newPlayer = players[newOwner-1];
            else 
                return;

            SetCountryOwner(me.fcountry, newPlayer);

            if(GetCapitalOfCountry(me.fcountry).HasUnit()) {
                GetCapitalOfCountry(me.fcountry).funit.unitOwner = newPlayer;
            }
                // Destroy(GetCapitalOfCountry(me.fcountry).funit);
            else
                GetCapitalOfCountry(me.fcountry).AddUnit(players[newOwner-1], "Infantry");
        }
        


    MapElement GetCapitalOfCountry(string country) 
        {
        if (mapInitialized)
            for (int i = 0; i < gVars.gridSizeX; i++)
                for (int j = 0; j < gVars.gridSizeY; j++)
                    if (mapFields[i, j].fcountry == country && mapFields[i, j].IsCity())
                        return mapFields[i, j];
        return null;
        }


/* delegate to avoid repetition
    //
    void IterateMapFields(Action toExecute)
    {
        if (mapInitialized)
            for (int i = 0; i < gVars.gridSizeX; i++)
                for (int j = 0; j < gVars.gridSizeY; j++)
                    toExecute();
    }
*/


    // 
    void PrepareBattle(MapElement srcme, MapElement trgtme)
    {
        if(srcme == null || trgtme == null || !srcme.HasUnit() || !trgtme.HasUnit())
            return;

        attField = srcme;
        defField = trgtme;

        srcme.TurnUnitTo(trgtme);
        trgtme.TurnUnitTo(srcme);

        gameMode = "prepareBattle";
        RefreshMap();
    }

    //
    void StopBattle() 
    {
        if(attField != null)  {
            currentME = attField;
            if(attField.HasUnit() && !attField.funit.HasMoved() ) {
                ShowMoves(currentME);
            }
            else
                UnmarkTiles();
        }

        attField = null;
        defField = null;
        gameMode = "normalPlay";
        RefreshMap();
    }

    // retval = true, if attacker wins
    public bool ConductBattle()
    {
        if(attField == null || defField == null || !attField.HasUnit() || !defField.HasUnit())
            return false;

        Unit attacker = attField.funit, defender = defField.funit;
        bool retval = false;

        // if we are not next to enemy, move there
		MapElement me = FindClosestNeighbour(attField, defField);
		if(me != null) {
        	MoveUnitTo(attacker, me);
            attacker.HasMoved(true);
            attField = me;
			return ConductBattle();
		}

        string debugString = "";

        float attackUpLand = !attField.IsLandField() && defField.IsLandField() ? 2f : 1f;
        debugString += "attackUpLand Malus: " + attackUpLand.ToString();
        float attackToSea = attField.IsLandField() && !defField.IsLandField() ? 0.5f : 1f;
        debugString += " / attackToSea Bonus: " + attackToSea.ToString();

        float attbonus = UnityEngine.Random.Range(0.9f, 1.2f);
        debugString += " / attbonus (rnd): " + attbonus.ToString() ;
        float defbonus = 0.7f + defField.movecost * 0.3f;
        debugString += " / defbonus: (landscape)" + defbonus.ToString() ;

        float attpower = attacker.getAttPower() * attacker.getStrength() * attbonus * attacker.getExperience() / attackUpLand;
        debugString += " / attpower: " + attpower.ToString() ;
        float defpower = defender.getDefPower() * defender.getStrength() * defbonus * attackToSea * defender.getExperience() / attbonus ;
        debugString += " / defpower: " + defpower.ToString() ;
        float attratio = attpower / defpower;
        debugString += " / attratio: " + attratio.ToString() ;

        int attremains = attacker.getStrength() - (int)((float)attacker.getStrength() / 3f / attratio);
        debugString += " / attstr before: " + attacker.getStrength().ToString() + " / attremains: " + attremains.ToString() ;
        attacker.SetStrength(attremains);

        int defremains = defender.getStrength() - (int)((float)defender.getStrength() / 2f * attratio);
        debugString += " / defstr before: " + defender.getStrength().ToString() + " / defremains: " + defremains.ToString() ;
        defender.SetStrength( defremains );

        Debug.Log(debugString);

        defField.PlayExplosion();

        defender.AddExperience(10);
        attacker.AddExperience(10);
        defender.HasBattled(true);
        attacker.HasBattled(true);
        
        if (defender.getStrength() <= 0)
        {
            Destroy(defender);
            defField.funit = null;
            defField.PlayExplosion();

            if (attacker != null)
            {
                MoveUnitTo(attacker, defField);
                attacker.HasMoved(true);
            }
            retval = true;
        }
        
        if (attacker.getStrength() <= 0) {
        	Destroy(attacker);
        	attField.funit = null;
        	attField.PlayExplosion();
        }

        StopBattle();

        return retval;
    }

    // KARTENFUNKTIONEN ********************************************************************************************************************************************

    void MoveUnitTo(Unit funit, MapElement trgtme)
    {
        if (funit == null || trgtme == null || funit.HasMoved())
            return;

        funit.DigIn(false);

        movingUnit = funit;
        MapElement srcme = FindMapElementOfObject(funit.getRepresentative());
        if (srcme == null)
            return;

        if (!infiniToggle.isOn)
            funit.HasMoved(true);

        // if target tile is empty or can be captured
        if (!trgtme.HasUnit())
        {
            srcme.TurnUnitTo(trgtme);

            funit.targetPos = trgtme.ftile.transform.position;
            trgtme.funit = funit;

            srcme.funit = null;
            currentME.funit = null;

            if (trgtme.IsCity())
                CaptureCity(trgtme);
        }

        UnmarkTiles();
        RefreshMap();
    }


    //
    public bool IsCity(int x, int y)
    {
        return mapFields[x, y].IsCity();
    }
    
    
    //
    public int GetPlayerNumber(int x, int y)
    {
        if(mapFields[x, y].fowner != null)
            return mapFields[x, y].fowner.playernumber;
        return -1;
    }
    

    //
    public int GetFieldPower(int x, int y, int type)
    {
        if(type == gVars.incomeTypePower)
            return mapFields[x, y].fpower;
        else if(type == gVars.incomeTypeIndu)
            return mapFields[x, y].findu;

        return 0;
    }
    
    //
    public bool HasFriendlyUnit(int x, int y, Player player)
    {
        return mapFields[x, y].HasFriendlyUnit(player);
    }
    
    //
    public bool IsFriendlyTo(int x, int y, Player player)
    {
        return mapFields[x, y].fowner == player;
    }
    
    //
    public Unit GetUnit(int x, int y)
    {
        return mapFields[x, y].funit;
    }
    
    //
    public MapElement FindMapElementOfObject(GameObject go)
    {
        for (int i = 0; i < gVars.gridSizeX; i++)
        {
            for (int j = 0; j < gVars.gridSizeY; j++)
            {
                if (mapFields[i, j].ftile == go || mapFields[i, j].GetUnitRepresentative() == go)
                    return mapFields[i, j];
            }
        }

        return null;
    }


    //
    public void UnmarkTiles()
    {
        for (int i = 0; i < gVars.gridSizeX; i++)
        {
            for (int j = 0; j < gVars.gridSizeY; j++)
            {
                mapFields[i, j].marked = false;
            }
        }
        
        RefreshMap();
    }


    //
    void RefreshMap()
    {
        MapElement me;

        for (int i = 0; i < gVars.gridSizeX; i++)
        {
            for (int j = 0; j < gVars.gridSizeY; j++)
            {

                me = mapFields[i, j];

                if (mapFields[i, j].fowner != null)
                {
                    mapFields[i, j].frenderer.material.color = mapFields[i, j].fowner.GetBGColor();
                }
                else

                /*
                // ALTERNATIVE 1: COLOR - either set color or set dark/lightsea
                */
                {
                    mapFields[i, j].frenderer.material.color = gVars.defaultColor;
                }

                if (mapFields[i, j].marked)
                {
                    Color c = mapFields[i, j].frenderer.material.color;
                    Color newc = new Color(c.r + 0.3f, c.g + 0.3f, c.b + 0.3f, c.a);
                    mapFields[i, j].frenderer.material.color = newc;
                }

                if (mapFields[i, j] == attField || mapFields[i, j] == defField )
                {
                    Color c = mapFields[i, j].frenderer.material.color;
                    Color newc = new Color(c.r + 0.3f, c.g - 0.1f, c.b - 0.1f, c.a);
                    mapFields[i, j].frenderer.material.color = newc;
                }
                

                /*
                // ALTERNATIVE 2: SEA - set dark/lightsea
                
                {
                    if (mapFields[i, j].fname != "sea")
                        mapFields[i, j].frenderer.material.color = gVars.defaultColor;
                    else
                        mapFields[i, j].frenderer.material = darkSea;
                }

                if (mapFields[i, j].marked)
                {
                    if ( mapFields [i, j].fname != "sea") {
                        Color c = mapFields[i, j].frenderer.material.color;
                        Color newc = new Color(c.r + 0.3f, c.g + 0.3f, c.b + 0.3f, c.a);
                        mapFields[i, j].frenderer.material.color = newc;
                        }
                    else
                        mapFields [i, j].frenderer.material = lightSea;
                }

                if (mapFields[i, j] == attField || mapFields[i, j] == defField)
                {
                    if ( mapFields [i, j].fname != "sea") {
                        Color c = mapFields[i, j].frenderer.material.color;
                        Color newc = new Color(c.r + 0.3f, c.g - 0.1f, c.b - 0.1f, c.a);
                        mapFields[i, j].frenderer.material.color = newc;
                        }
                    else
                        mapFields [i, j].frenderer.material = lightSea;
                }
                */

            }
        }
    }

    //  gets closest neighbour of target fieldd as seen from src field - null if they are neighbours already
    MapElement FindClosestNeighbour(MapElement srcme, MapElement trgtme)
    {
        List<MapElement> neighs = GetNeighbours(trgtme);
        // lets see if they are direct neighbs
        MapElement me = neighs.Find(x => x.findex == srcme.findex);

        if (me == null && neighs.Count > 0)
        {
            float angle1 = trgtme.GetAngleTo(srcme);
            int hit = 0;
            float minscore = 360f, angle2;

            for (int i = 0; i < neighs.Count; i++)
            {
                angle2 = trgtme.GetAngleTo(neighs[i]); 

                // only calculate field if marked = attacker can reach it
                if (neighs[i].marked && Math.Abs(angle1 - angle2) < minscore)
                {
                    hit = i;
                    minscore = Math.Abs(angle1 - angle2);
                }
            }
            
            // Debug.Log("chosen: " + neighs[hit].findex.ToString());
            return neighs[hit];
        }
        else
            return null;
    }

    // Movemöglichkeiten anzeigen
    void ShowMoves(MapElement me)
    {
        int i = 0, j = 0, k = 0, l = 0, secure = 0, startentry = 0;
        bool isStillMoveable = true, comesFromLand = me.IsLandField();

        UnmarkTiles();

        // moveremain resetten, DANN Neighbours holen
        for (i = 0; i < gVars.gridSizeX; i++)
            for (j = 0; j < gVars.gridSizeY; j++)
                mapFields[i, j].moveremain = 0;

        List<MapElement> moveable = getMovableNeighbours(me, me.funit.getMoveval(comesFromLand), comesFromLand, true);
        List<MapElement> tmp = new List<MapElement>();

        // List mit Feldern füllen
        while (isStillMoveable)
        {

            isStillMoveable = false;

            // loop could blow up if moveable inflates - we should start where last cycle ended
            for (i = startentry, l = moveable.Count; i < l; i++)
            {

                // if possible to move further, look mmore
                if (moveable[i].moveremain >= 1f)
                {
                    tmp = getMovableNeighbours(moveable[i], moveable[i].moveremain, comesFromLand, false);

                    // new fields found, attach
                    for (k = 0; k < tmp.Count; k++)
                    {
                        // if newfound field is != origin and not in list already
                        if (tmp[k] != me && moveable.IndexOf(tmp[k]) == -1)
                        {
                            moveable.Add(tmp[k]);
                            // we must know how much was added
                            startentry++;
                        }
                    }

                    if (secure < 20)
                        isStillMoveable = true;
                }
            }

            startentry = moveable.Count - startentry;
            secure++;
        }

        for (i = 0; i < moveable.Count; i++)
        {
            moveable[i].marked = true;
        }
        me.marked = true;

        RefreshMap();
    }


    // Nachbarn mit Bewegungswerten finden
    List<MapElement> getMovableNeighbours(MapElement me, float moveremains, bool comesFromLand, bool innerCircle)
    {
        List<MapElement> neighbs = GetNeighbours(me);
        List<MapElement> tmp = new List<MapElement>();
        int i = 0;

        // copy all valid fields
        for (i = 0; i < neighbs.Count; i++)
        {
            // write into movetmp how many movepts we would have left after moving there - Max value because of unwanted resetting of neighbours otherwise
            neighbs[i].moveremain = Mathf.Max(neighbs[i].moveremain, moveremains - neighbs[i].movecost);
            // if enemy has field occupied, no more moves are possible
            if(neighbs[i].HasUnit() && !neighbs[i].HasFriendlyUnit(currentPlayer))
                neighbs[i].moveremain = 0f;

            // if possible to move here, enable field
            if ((innerCircle && !neighbs[i].HasFriendlyUnit(currentPlayer)) ||
                (neighbs[i].moveremain >= 0f && neighbs[i].IsLandField() == comesFromLand && neighbs[i].IsLandField() == me.IsLandField() && !neighbs[i].HasFriendlyUnit(currentPlayer)))
            {
                tmp.Add(neighbs[i]);
            }
        }

        return tmp;
    }


    // Alle Nachbarn eines Hex finden
    List<MapElement> GetNeighbours(MapElement me)
    {
        List<MapElement> neighs = new List<MapElement>();
        int[,] nArray;
        int x, y;

        if ((me.fy % 2) == 0)
            nArray = new int[,] { { 0, -2 }, { 0, -1 }, { 0, 1 }, { 0, 2 }, { -1, 1 }, { -1, -1 } };
        else
            nArray = new int[,] { { 0, -2 }, { 1, -1 }, { 1, 1 }, { 0, 2 }, { 0, 1 }, { 0, -1 } };

        for (int i = 0; i < 6; i++)
        {
            x = nArray[i, 0] + me.fx;
            y = nArray[i, 1] + me.fy;

            
            if (x >= 0 && x < gVars.gridSizeX && y >= 0 && y < gVars.gridSizeY)
            {
                neighs.Add(mapFields[x, y]);
            }
            // check for jump over world edge here
            else if (x == -1 && y >= 0 && y < gVars.gridSizeY)
            {
                neighs.Add(mapFields[gVars.gridSizeX - 1, y]);
            }
            else if (x == gVars.gridSizeX && y >= 0 && y < gVars.gridSizeY)
            {
                neighs.Add(mapFields[0, y]);
            }
        }

        return neighs;
    }

    // Karte laden
    string[] LoadMap()
    {
        TextAsset ta;

        try {
             ta = Resources.Load("serialize/ori-map") as TextAsset;
/*          Stream s = new MemoryStream(ta.bytes);
            BinaryFormatter formatter = new BinaryFormatter();
            var v = formatter.Deserialize(s);
            content  = (string) v;
            s.Close();
*/    
            return ta.text.Split('|');
        } 
        catch (Exception e){
            Debug.Log(e.Message);
        }

        return null;
    }


    // Karte speichern
    void SaveMap()
    {
        string destination = "Assets/Resources/serialize/save-map.bytes";
        string content = "";
        MapElement me;

        if (mapInitialized)
            for (int i = 0; i < gVars.gridSizeX; i++)
                for (int j = 0; j < gVars.gridSizeY; j++)
                {
                    me = mapFields[i, j];
                    content += "fld:" + me.ftype + "*ctr:" + me.fcountry + "*cty:" + me.fcity + "*pwr:" + me.fpower + "*ind:" + me.findu + "*|\r\n";
                }


        if (File.Exists(destination))
            File.Delete(destination);

        StreamWriter writer = new StreamWriter(destination, true);
        writer.WriteLine(content);

        //		mapFields
        writer.Close();
    }


    // Zum Parsen des Parameter-Strings
    public static string GetMapValue(string unparsed, string key)
    {

        int m, n = unparsed.IndexOf(key);

        if (n >= 0)
        {
            string s2 = unparsed.Substring(n + key.Length + 1);
            m = s2.IndexOf("*");
            if (m >= 0)
            {
                return s2.Substring(0, m);
            }
        }

        return "";
    }


    //
    void SetCountryOwner(string fcountry, Player p)
    {
        if (mapInitialized)
        {
            for (int i = 0; i < gVars.gridSizeX; i++)
            {
                for (int j = 0; j < gVars.gridSizeY; j++)
                {
                    if (mapFields[i, j].fcountry == fcountry)
                    {
                        mapFields[i, j].fowner = p;
                    }
                }
            }
        }

        RefreshMap();
    }


    //	
    void SwitchMapField(MapElement me)
    {
        if (me == null) return;

        string pwrstr, type, industr, ctry, city;

        type = gVars.uic.fieldName.text.ToString();
        int ftype = gVars.fieldNames.IndexOf(type);
        if (ftype < 0) ftype = gVars.fieldNames.IndexOf(me.fname);

        ctry = gVars.uic.countrySwitch.text.ToString();
        ctry = ctry == "" ? me.fcountry : ctry;

        city = gVars.uic.cityInput.text.ToString();
        city = city == "" ? me.fcity : city;

        pwrstr = gVars.uic.pwrInput.text.ToString();
        industr = gVars.uic.induInput.text.ToString();

        int pwr = pwrstr == "" ? 0 : int.Parse(pwrstr);
        int indu = industr == "" ? 0 : int.Parse(industr);

        MapElement me2 = worldCube.AddComponent<MapElement>() as MapElement;
        me2.Init(me.fx, me.fy, ftype, ctry, city, worldCube);

        mapFields[me.fx, me.fy] = me2;
        currentME = me2;
        Destroy(me);
    }


    //	
    void SwitchMat(MapElement me)
    {
        if (me == null) return;

        string type;

        type = gVars.uic.fieldName.text.ToString();
        int ftype = gVars.fieldNames.IndexOf(type);
        if (ftype < 0) ftype = gVars.fieldNames.IndexOf(me.fname);


        // hier künftig die texture statt das material switchen wie in MapELemets:z.145

        me.frenderer.material = Resources.Load("materials/hex-mat-default", typeof(Material)) as Material;
        // me.frenderer.material = Resources.Load("materials/hex-mat-" + type, typeof(Material)) as Material;
    }


    // PLAYER Funktionen ********************************************************************************************************************************************
    //
    void ResetPlayerMoves()
    {
        for (int i = 0; i < gVars.gridSizeX; i++)
            for (int j = 0; j < gVars.gridSizeY; j++)
            {
                if (mapFields[i, j].HasUnit() && mapFields[i, j].funit.unitOwner == currentPlayer)
                {
                    mapFields[i, j].funit.HasMoved(false);
                }
                else if(mapFields[i, j].HasUnit())
                {
                    mapFields[i, j].funit.HasMoved(true);
                }
            }
    }


    // No. of players chosen
    public void InitPlayers()
    {
        numHumanPlayers = int.Parse(gVars.uic.playnumText.text);

        gVars.numPlayers = numHumanPlayers;

        gVars.uic.ShowPanel("Prepare", true);

        for (int i = 0; i < gVars.numPlayers; i++)
        {
            players[i] = new Player(i + 1);
            players[i].isHuman = i < numHumanPlayers;
        }

        // neutralPlayer owns all units on other countries
        neutralPlayer = new Player(99);

        gameMode = "selectCapital";
        currentPlayer = players[0];
        PreparePlayer();
    }


    // 
    public void PreparePlayer()
    {

        gVars.uic.prepareText.text = "Player " + currentPlayer.playernumber.ToString() + ", please select a capital!";

        if (currentME != null && currentME.IsCity() && !currentME.HasUnit())
        {
            currentME.AddUnit(currentPlayer, "Infantry");
            CaptureCity(currentME);

            currentPlayer = GetNextPlayer(currentPlayer);
            gVars.uic.prepareText.text = "Player " + currentPlayer.playernumber.ToString() + ", please select a capital!";

            // if all players are chosen, give next capitals to neutral player
            if (currentPlayer.playernumber == 1) 
            {
                if (mapInitialized)
                    for (int i = 0; i < gVars.gridSizeX; i++)
                        for (int j = 0; j < gVars.gridSizeY; j++)
                            if (mapFields[i, j].IsCity() && !mapFields[i, j].HasUnit())
                                mapFields[i, j].AddUnit(neutralPlayer, "Infantry");

                StartRound();
            }
        }
    }


    // which player is next?
    Player GetNextPlayer(Player p)
    {

        // if player is not last player
        if (p.playernumber < gVars.numPlayers)
            return players[currentPlayer.playernumber - 1 + 1];
        // else go back to nr. 1
        else
            return players[0];
    }


    // Update Check-Funktionen ********************************************************************************************************************************************
    // CHECK BUY UNIT BUTTONS  ****************************
    // void CheckButtonState()
    // {

    // }

    // MOVE UNIT  ****************************
    void CheckUnitMove()
    {

        if (movingUnit != null)
        {
            if (movingUnit.uRepresentative.transform.position.x == movingUnit.targetPos.x && movingUnit.uRepresentative.transform.position.z == movingUnit.targetPos.z)
            {//reached?
                movingUnit = null;// stop moving
            }
            else
            {
                SmoothMove(movingUnit.uRepresentative, movingUnit.targetPos, gVars.unitSpeedDivisor);
            }
        } //
    }

    // MOVE CAM (Kamera bewegen) ****************************
    void CheckCamMove()
    {
        // return;

        if (moveCam)
        {
            Vector3 pVPort, pTemp;
            float tolerance = 0.1f;

            getViewportCenterPoint(out pVPort);

            // Debug.Log("Viewport center is: " + pVPort);
            // Debug.Log("Target on map is: " + ptarget);
            // Debug.Log("CamPos: " + mainCam.transform.position);

            float xDiff = ptarget.x - pVPort.x, zDiff = ptarget.z - pVPort.z ;
            float mX = xDiff / (gVars.camSpeedDivisor), mZ = zDiff / (gVars.camSpeedDivisor);
            // Debug.Log("Difference is: " + xDiff.ToString() + ", zDiff: " + zDiff.ToString());
            // Debug.Log("Moving: " + mX.ToString() + ", zDiff: " + mZ.ToString());

            mainCam.GetComponent<CamMove>().MoveCam(mX, 0, mZ);

            // float xSlowCam = 0.1f + Math.Min(3f, Math.Abs(xDiff)); // Must add 0.1 to avoid DIV/0 !
            // float zSlowCam = 0.1f + Math.Min(3f, Math.Abs(zDiff));

            // Debug.Log("xDiff: " + xDiff.ToString() + ", zDiff: " + zDiff.ToString() + ", slowCamX: " + xSlowCam + ", slowCamZ: " + zSlowCam);

            if (Math.Abs(xDiff) < tolerance  && Math.Abs(zDiff) < tolerance)
            {//reached?
                moveCam = false;// stop moving
                Debug.Log("Position reached! ********************************");
            }
        } //
    }


    // MOVE SPOT (Licht bewegen) ****************************
    void CheckSpotMove()
    {

        if (moveSpot)
        {
            if (fieldSpot.transform.position.x == ptarget.x && fieldSpot.transform.position.z == ptarget.z)
            {//reached?
                moveSpot = false;// stop moving
            }
            else
            {
                SmoothMove(fieldSpot, ptarget, gVars.lightSpeedDivisor);
            }
        }
    }
    //				float xDiff = fieldSpot.transform.position.x - ptarget.x, zDiff = fieldSpot.transform.position.z - ptarget.z;
    //				pTemp = new Vector3 (fieldSpot.transform.position.x - xDiff / gVars.lightSpeedFactor, fieldSpot.transform.position.y, fieldSpot.transform.position.z - zDiff / gVars.lightSpeedFactor);
    //
    //				fieldSpot.transform.position = pTemp;


    // MOVE (only 2D) ****************************
    void SmoothMove(GameObject toMove, Vector3 target, int speedDiv)
    {
        float xDiff = toMove.transform.position.x - target.x, zDiff = toMove.transform.position.z - target.z;
        toMove.transform.position = new Vector3(toMove.transform.position.x - xDiff / speedDiv, toMove.transform.position.y, toMove.transform.position.z - zDiff / speedDiv);
    }

    // MOVE (only 2D) ****************************
    public void SmoothMove3D(GameObject toMove, Vector3 target, int speedDiv)
    {
        float xDiff = toMove.transform.position.x - target.x, yDiff = toMove.transform.position.y - target.y, zDiff = toMove.transform.position.z - target.z;
        toMove.transform.position = new Vector3(toMove.transform.position.x - xDiff / speedDiv, toMove.transform.position.y - yDiff / speedDiv, toMove.transform.position.z - zDiff / speedDiv);
    }


    // Get Center of viewport ****************************
    public bool getViewportCenterObject(out Vector3 v3)
    {
        RaycastHit hit;

        if (getClickHit(new Vector3(Screen.width / 2, Screen.height / 2, 0), out hit))
        {
            v3 = hit.transform.position;
            return true;
        }

        v3 = new Vector3();
        return false;
    }


    // Get Center of viewport ****************************
    public bool getViewportCenterPoint(out Vector3 v3)
    {
        RaycastHit hit;

        if (getClickHit(new Vector3(Screen.width / 2, Screen.height / 2, 0), out hit))
        {
            v3 = hit.point;
            return true;
        }

        v3 = new Vector3();
        return false;
    }


    // Get Object that was hit by clickpos ****************************
    bool getClickHit(Vector3 v3, out RaycastHit rh)
    {
        RaycastHit hit = new RaycastHit();

        // Get Center of viewport
        Ray ray = mainCam.ScreenPointToRay(v3);
        if (Physics.Raycast(ray, out hit))
        {
            rh = hit;
            // Debug.Log(hit.point);
            return true;

        }

        rh = hit;
        return false;
    }


    // *************************************************************************************************************************
    // Input Events
    public void setMapEdit(bool b)
    {
        if (b)
        {
            gameMode = "editMap";
            gVars.uic.ShowPanel("Player", true);
        }
        else
            InitPlayers();
    }

    // Key Handler *******************************
    void OnGUI()
    {
        Event e = Event.current;

        if (e.type == EventType.KeyDown && e.alt && e.control && e.keyCode == KeyCode.S)
        {
            SaveMap();
            e.Use();
        }
        else if (e.type == EventType.KeyDown && e.alt && e.control && e.keyCode == KeyCode.Y)
        {
            SwitchMapField(currentME);
            e.Use();
        }
        else if (e.type == EventType.KeyDown && e.alt && e.control && e.keyCode == KeyCode.X)
        {
            SwitchMat(currentME);
            e.Use();
        }
        else if (e.type == EventType.KeyDown && e.alt && e.control && e.keyCode == KeyCode.B)
        {
            gameBoard.SetActive((++countMapSwitch % 3 == 1));
            gameBoard2.SetActive((countMapSwitch % 3 == 2));
            e.Use();
        }
        else if (e.type == EventType.KeyDown && e.alt && e.control && e.keyCode == KeyCode.C)
        {
            currentME.fcountry = gVars.uic.countrySwitch.text;
            RefreshMap();
            e.Use();
        }
        else if (e.type == EventType.KeyDown && e.alt && e.control && e.keyCode == KeyCode.G)
        {
            gVars.uic.countrySwitch.text = currentME.fcountry;
            e.Use();
        }
    }


    // MAIN CLICK HANDLER ***********************
    void HandleMapClick()
    {
        RaycastHit mouseHit;

        moveCam = false;
        moveSpot = false;

        if (getClickHit(Input.mousePosition, out mouseHit))
        {
            // get desired Position by Getting Position of Collider
            bool selectField = false;
            GameObject go = mouseHit.collider.gameObject;
            MapElement clickedme = FindMapElementOfObject(go);

            //	Debug.Log (go.name);

            // if mapfield was found
            if (clickedme != null)
            {

                // GAMEMODE: Prepare & select cap **********************
                if (gameMode == "selectCapital" && clickedme.IsCity())
                {
                    selectField = true;
                }

                // GAMEMODE: Normal ************************************
                else if (gameMode == "prepareBattle")
                {
                    if(clickedme == defField)
                    {
                        ConductBattle();
                    }
                    else 
                    {
                        StopBattle();
                    }
                }

                // GAMEMODE: Normal ************************************
                else if (gameMode == "normalPlay")
                {
                    // if player's unit is clicked
                    if (clickedme.HasUnit() && !clickedme.funit.HasMoved() && clickedme.funit.unitOwner == currentPlayer)
                    {
                        // select field if not selected
                        if (!clickedme.marked)
                        {
                            ShowMoves(clickedme);
                            selectField = true;
                        }
                        else
                        {
                            UnmarkTiles();
                            selectField = true;
                        }
                    }
                    // if player's unit shows range and a target field is clicked
                    else if (currentME != null && currentME.HasUnit() && clickedme.marked)
                    {
                        // if field is empty but has a country, it will defend itself
                        if (!clickedme.HasUnit() && (clickedme.fowner == null && clickedme.fcountry != "") )
                        {
                            RaiseRebels(clickedme);
                        }
                        // if field has been  neutral
                        else if (!clickedme.HasUnit())
                        {
                            MoveUnitTo(currentME.funit, clickedme);
                            selectField = true;
                        }
                        else if (!clickedme.HasFriendlyUnit(currentPlayer))
                        {
                            PrepareBattle(currentME, clickedme);
                            // if (ConductBattle(currentME, clickedme))
                            // {
                            //     MoveUnitTo(currentME.funit, clickedme);
                            //     selectField = true;
                            // }
                        }
                    }
                    // if an empty city field is clicked
                    else // if (!clickedme.HasUnit() && clickedme.IsCity() && clickedme.fowner == currentPlayer)
                    {
                        UnmarkTiles();
                        selectField = true;
                    }
                }

                // GAMEMODE: EDIT MAP ************************************
                else if (gameMode == "editMap")
                {
                    if(currentME != null)
                        Debug.Log(currentME.GetAngleTo(clickedme));
                    selectField = true;
                }
                else
                {
                    selectField = false;
                }

                // wenn das Tile angeleuchtet werden soll... *********
                if (selectField)
                {
                    SelectME(clickedme, go);
                }

            }
            else
            {
                Debug.Log("Mouseclick detection failed");
            }
        }
    }

    // overload
    public void SelectME(Unit u) {
        SelectME(FindMapElementOfObject(u.getRepresentative()), u.getRepresentative());
    }


    // func to highlight a non-selected field
    public void SelectME(MapElement clickedme, GameObject go)
    {
        currentME = clickedme;
        ptarget.x = go.transform.position.x;
        ptarget.z = go.transform.position.z;

        if (camToggle.isOn)
        {
            moveCam = true;
        }
        if (lightToggle.isOn)
        {
            moveSpot = true;
        }

        gVars.uic.fieldIndex.text = clickedme.findex.ToString() + ", x: " + clickedme.fx + ", y: " + clickedme.fy;
        // gVars.uic.fieldName.text = clickedme.fname;
        gVars.uic.fieldCity.text = clickedme.fcity;
        gVars.uic.fieldCountry.text = clickedme.fcountry;
        gVars.uic.fieldPower.text = clickedme.fpower.ToString();
        gVars.uic.fieldIndu.text = clickedme.findu.ToString();

        if( clickedme.HasUnit() ) {
            gVars.uic.unitType.text = clickedme.funit.utype;
            gVars.uic.unitName.text = clickedme.funit.uname;
            gVars.uic.unitStrength.text = clickedme.funit.getStrength().ToString() + " %"; 
            gVars.uic.unitMove.text = clickedme.funit.getMoveval(true).ToString() + " Steps";
        }

        // special for edit mode
        if (gameMode == "editMap") 
        {
            if(currentME.IsCity()) {
                for (int i = 0; i < gVars.gridSizeX; i++)
                    for (int j = 0; j < gVars.gridSizeY; j++)
                        if(mapFields[i, j].IsCity() && mapFields[i, j].fcountry != null)
                            SetCountryOwner(mapFields[i, j].fcountry, null);
                SetCountryOwner(currentME.fcountry, players[0]);
                }
            
        }
    }

} // Class MapController

