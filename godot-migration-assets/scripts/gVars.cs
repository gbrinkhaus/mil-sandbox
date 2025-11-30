using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ****************************************************************
public class gVars {

	public static MapController mc;
	public static UIController uic;

	public static Color defaultColor = new Color (0.68f, 0.68f, 0.68f, 1f);

	public const int incomeTypePower = 0, incomeTypeIndu = 1;

	public const float hexagonWidth = 0.60f;
	public const float hexagonHeight = 0.52f;
	// public const int hexagonXOverlap = 20;
	public const float sectorWidth = hexagonWidth / 4 * 3;

	public const int gridSizeX = 46;
	public const int gridSizeY = 92;

	public const int mapX = -10;
	public const int mapY = 5;

	public const int unitSpeedDivisor = 10;
	public const int camSpeedDivisor = 2;
	public const int lightSpeedDivisor = 10;

	public const float mapYLevel = 0.51f;

	// nicht ändern, array sizer!
	public static int numPlayers = 4; 

	public static readonly List <string> fieldNames =  new List <string> () {"grass", "forest", "desert", "sea", "mountain", 
		"city", "snow", "jungle", "seaice"};

	public static readonly List <string> countries = new List <string> () { "Ägypten", "Afghanistan", "Alaska", "Alberta/BC", "Argentinien", "Brasilien", "China", "Großbritannien", "Grönland", "Indien", "Indonesien", "Irkutsk", "Island", "Jakutien", "Japan", "Kamtschatka", "Kongo", "Madagaskar", "Mittelamerika", "Mitteleuropa", "Mittlerer Osten", "Mongolei", "Nordwestafrika", "Ontario/Manitoba", "Ost-Australien",  "Ostafrika", "Oststaaten", "Peru", "Philippinen/Guinea", "Quebec/Neufundland", "Siam", "Sibirien", "Skandinavien", "Südafrika", "Südeuropa", "Ukraine", "Ural", "Venezuela", "West-Australien", "Westeuropa", "Westrussland", "Weststaaten", "Yukon/Nordwest"} ;

	public static readonly Dictionary<string, Dictionary<string, int> > cityvalues 
		= new Dictionary<string, Dictionary<string, int> >() 
		{ 
		{"Algiers",  		new Dictionary<string, int> { {"power", 10}, {"indu",  1} } },
		{"Anchorage",  		new Dictionary<string, int> { {"power",  0}, {"indu",  0} } },
		{"Antananarivo",  	new Dictionary<string, int> { {"power",  0}, {"indu",  0} } },
		{"Bangkok",  		new Dictionary<string, int> { {"power", 12}, {"indu",  1} } },
		{"Beijing",  		new Dictionary<string, int> { {"power", 26}, {"indu",  3} } },
		{"Berlin",  		new Dictionary<string, int> { {"power", 18}, {"indu", 16} } },
		{"Bratsk",  		new Dictionary<string, int> { {"power",  0}, {"indu",  0} } },
		{"Buenos Aires",  	new Dictionary<string, int> { {"power", 10}, {"indu",  1} } },
		{"Caracas",  		new Dictionary<string, int> { {"power", 10}, {"indu",  1} } },
		{"Daressalam",  	new Dictionary<string, int> { {"power",  3}, {"indu",  0} } },
		{"Edmonton",  		new Dictionary<string, int> { {"power",  0}, {"indu",  1} } },
		{"Istanbul",  		new Dictionary<string, int> { {"power", 15}, {"indu",  1} } },
		{"Jakarta",  		new Dictionary<string, int> { {"power", 17}, {"indu",  2} } },
		{"Jekaterinburg",  	new Dictionary<string, int> { {"power", 17}, {"indu",  3} } },
		{"Kabul",  			new Dictionary<string, int> { {"power",  6}, {"indu",  1} } },
		{"Kairo",  			new Dictionary<string, int> { {"power", 10}, {"indu",  1} } },
		{"Kalkutta",  		new Dictionary<string, int> { {"power", 24}, {"indu",  3} } },
		{"Kapstadt",  		new Dictionary<string, int> { {"power",  8}, {"indu",  1} } },
		{"Kiew",  			new Dictionary<string, int> { {"power", 17}, {"indu",  2} } },
		{"Kinshasa",  		new Dictionary<string, int> { {"power",  5}, {"indu",  0} } },
		{"Lima",  			new Dictionary<string, int> { {"power",  9}, {"indu",  0} } },
		{"London",  		new Dictionary<string, int> { {"power", 16}, {"indu", 21} } },
		{"Los Angeles", 	new Dictionary<string, int> { {"power", 15}, {"indu", 11} } },
		{"Magadan",  		new Dictionary<string, int> { {"power",  0}, {"indu",  0} } },
		{"Manila",  		new Dictionary<string, int> { {"power",  9}, {"indu",  1} } },
		{"Mexiko-Stadt",  	new Dictionary<string, int> { {"power", 12}, {"indu",  1} } },
		{"Novosibirks",  	new Dictionary<string, int> { {"power",  5}, {"indu",  0} } },
		{"Nuuk",  			new Dictionary<string, int> { {"power",  0}, {"indu",  0} } },
		{"Ottawa",  		new Dictionary<string, int> { {"power",  0}, {"indu",  1} } },
		{"Paris",  			new Dictionary<string, int> { {"power", 19}, {"indu",  7} } },
		{"Perth",  			new Dictionary<string, int> { {"power",  3}, {"indu",  1} } },
		{"Quebec",  		new Dictionary<string, int> { {"power",  3}, {"indu",  1} } },
		{"Reykjavik",  		new Dictionary<string, int> { {"power",  0}, {"indu",  0} } },
		{"Rio de Janeiro",  new Dictionary<string, int> { {"power", 13}, {"indu",  1} } },
		{"St. Petersburg",  new Dictionary<string, int> { {"power", 18}, {"indu",  3} } },
		{"Stockholm",  		new Dictionary<string, int> { {"power", 10}, {"indu",  3} } },
		{"Sydney",  		new Dictionary<string, int> { {"power",  3}, {"indu",  1} } },
		{"Tokyo",  			new Dictionary<string, int> { {"power", 16}, {"indu",  2} } },
		{"Ulanbataar",  	new Dictionary<string, int> { {"power",  0}, {"indu",  0} } },
		{"Washington",  	new Dictionary<string, int> { {"power", 17}, {"indu", 16} } },
		{"Wien",  			new Dictionary<string, int> { {"power", 20}, {"indu",  9} } },
		{"Yakutsk",  		new Dictionary<string, int> { {"power",  0}, {"indu",  0} } },
		{"Yellowknife", 	new Dictionary<string, int> { {"power",  0}, {"indu",  0} } }
		};

	// public const float pxScale = 100.0f;

	//	private string lastCountry = "";
	//	private int nrMapFields = (gridSizeX * gridSizeY) / 2;
	//	private int mapWidth = (hexagonWidth - hexagonXOverlap) * gridSizeX;
	//	private int mapHeight = hexagonHeight * gridSizeY / 2;
	//	private int[] columns = {" gridSizeY / 2, gridSizeY / 2 };
	//	private int sectorHeight = hexagonHeight;}

}