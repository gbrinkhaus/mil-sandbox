using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapElement : MonoBehaviour
{

    //	public GameObject parentobj;
    public string fcountry, fcity, fname, ftype;
    public int findex, fx, fy, fpower = 0, findu = 0;
    public Player fowner = null;
    public Unit funit = null;
    public float movecost = 1.0f, moveremain = 0f;
    public float elevation = 0f;
    public GameObject ftile;
    public bool marked = false;
    public Renderer frenderer;


    // neue Unit erzeugen
    public bool AddUnit(Player p, string type)
    {
        fowner = p;

        if (funit == null)
        {
            funit = ftile.AddComponent<Unit>() as Unit;
            funit.Init(type, this, p);
            p.AddUnit(funit);
            return true;
        }

        return false;
    }


    // Unit-Objekt zurückgeben
    public GameObject GetUnitRepresentative()
    {
        if (funit != null)
        {
            return funit.getRepresentative();
        }
        return null;
    }

    // public void Kill (MapElement me) {
    // 	if(funit != null)
    // 		Destroy(funit);
    // 	if(ftile != null)
    // 		Destroy(ftile);
    // 	Destroy(me);
    // }

    public void OnDestroy()
    {
        if (funit != null)
            Destroy(funit);
        if (ftile != null)
            Destroy(ftile);
    }

    public void PlayExplosion()
    {
		GameObject prefb = (GameObject)Resources.Load("prefabs/ExplosionMobile");
		GameObject explosion  = Instantiate (prefb, GetPosition(), Quaternion.identity);
        // explosion.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        
		Destroy(explosion, explosion.GetComponentInChildren<ParticleSystem>().main.duration);
    }

    // Field Init
    public void Init(int x, int y, int fieldtype, string country, string city, GameObject worldCube)
    {
        fcountry = country;
        fcity = "";
        fpower = 0;
        findu = 0;
        fname = gVars.fieldNames[fieldtype];

        fx = x;
        fy = y;
        findex = x * gVars.gridSizeY + y;
        ftype = fieldtype.ToString();
        fname = gVars.fieldNames[fieldtype];

        switch (fname)
        {
            case "grass":
                movecost = 1.0f;
                break;
            case "forest":
                movecost = 1.8f;
                break;
            case "desert":
                movecost = 1.8f;
                break;
            case "sea":
                movecost = 1.0f;
                elevation = -0.03f;
				fcountry = "";
                break;
            case "seaice":
                movecost = 1.8f;
                elevation = -0.03f;
				fcountry = "";
                break;
            case "mountain":
                movecost = 2.1f;
                elevation = 0.0f;
                break;
            case "city":
                movecost = 1.5f;
                elevation = 0.0f;
				fpower = gVars.cityvalues[city]["power"];
				findu = gVars.cityvalues[city]["indu"];
                fcity = city;
                break;
            case "snow":
                movecost = 1.8f;
                elevation = 0.0f;
                break;
            case "jungle":
                movecost = 2.0f;
                elevation = 0.0f;
                break;
        }

        float xpos = (gVars.hexagonWidth * x * 1.5f + (gVars.sectorWidth) * (y % 2)) + gVars.mapX;
        float ypos = -(gVars.hexagonHeight * y / 2) + gVars.mapY;
        Vector3 v3 = new Vector3(xpos, gVars.mapYLevel, ypos);
        v3.y += elevation;

        // hexTile erzeugen
        // GameObject prefb = (GameObject)Resources.Load("prefabs/3d_hex_" + fname);
        GameObject prefb = (GameObject)Resources.Load("prefabs/3d_hex");
        ftile = Instantiate(prefb, v3, Quaternion.identity);
        ftile.transform.Rotate(0, 180, 0);
        ftile.name = findex.ToString();

        frenderer = ftile.GetComponentInChildren<Renderer>();

        // new version: tested what happens with default material + texture change
        //frenderer.material = Resources.Load("materials/hex-mat-" + fname, typeof(Material) ) as Material;

        if(fname == "sea")
            frenderer.material = Resources.Load("materials/hex-water-default", typeof(Material)) as Material;
        else
            frenderer.material = Resources.Load("materials/hex-mat-default", typeof(Material)) as Material;


        frenderer.material.mainTexture = Resources.Load("maps/hex_" + fname + "_map", typeof(Texture)) as Texture;

        //		MapElement me = prefb.AddComponent<MapElement> ();
        //		prefb.GetComponent<MapElement> ().Init (x, y, ftype, fparams);
        //		prefb.transform.position = v3;

    }

    // Position hängt am Tile
    public Vector3 GetPosition()
    {
        return new Vector3(ftile.transform.position.x, ftile.transform.position.y + 0.05f, ftile.transform.position.z);
    }

    public bool HasUnit()
    {
        return funit != null;
    }

    public bool HasFriendlyUnit(Player p) 
    {
        if(HasUnit() && funit.unitOwner == p)
            return true;
            
        return false;
    }

    public bool IsLandField()
    {
        return fname != "sea";
    }

    public bool IsCity()
    {
        return fname == "city";
    }

    public void TurnUnitTo(MapElement me){
        if(!HasUnit())
            return;
        
        funit.TurnTo( GetAngleTo(me) );
    }

    public float GetAngleTo(MapElement turnToME) {
        float angle = Vector2.SignedAngle (new Vector2(0, 1), 
            new Vector2(turnToME.ftile.transform.position.x - ftile.transform.position.x , turnToME.ftile.transform.position.z - ftile.transform.position.z ) );
        // Debug.Log("from: " + findex.ToString() + " to: " + turnToME.findex.ToString() + " angle: " + angle.ToString() );

        // use this to turn clockwise and not countercw
        return 180 - angle;
    }

}
