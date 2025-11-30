// using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UnitLabel : MonoBehaviour
{
    GameObject textObject;
    TMP_Text theText;
    bool hasMoved = true;

    int tval = 0;
    bool updateLabel = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

	//
	public void OnDestroy () {

		/* if(textObject != null) 
		 	Destroy(textObject); */

        Debug.Log("Unit label destroyed");
	}


    // 
    public void Init(GameObject uRepresentative)
    {
        textObject = (GameObject) Instantiate ( Resources.Load("prefabs/UnitLabel"), uRepresentative.transform.position, Quaternion.identity);
        theText = textObject.GetComponent<TMP_Text>();

        textObject.transform.Rotate(30f, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        // Mechanism to repeat label update when text is not yet available
        if(updateLabel || System.DateTime.UtcNow.Millisecond % 10 == 0) {
            updateLabel = false;
            UpdateText(hasMoved);
        }
    } 


    // Reformat text styles
    public void UpdateText(bool unitMoved)
    {
        hasMoved = unitMoved;

        float stren = (float) tval / 100;
        // set health percentage from red to green
        SetTextColor(0, 3, Color.gray);

        if(hasMoved)
            SetTextColor(4, 5, Color.gray);
        else
            SetTextColor(4, 5, Color.white);

        SetTextColor(7, 8, new Color(1.0f - stren, 0.0f + stren, 0.3f - (stren-.5f)*(stren-.5f), 1f));
        //Debug.Log(Color.gray);
    }


    // Set new value of text
    public void SetText(int value)
    {
        tval = value;
        // textObject.SetActive(true);
        theText.text = "★★★\n◉  " /* + tval.ToString().PadLeft(3, ' ') */ + "✚";
        UpdateText(hasMoved);
    }
   

    // Set text color, toChar = -1 means all chars
    // !!! TMPro obviously has an error in word splitting, thus use chars only
    public void SetTextColor(int fromChar, int toChar, Color c) 
    {
        if(theText == null || theText.textInfo == null) {
            // Debug.Log("SetTextColor called but no text initiated");
            updateLabel = true;
            return;
            }

        TMP_TextInfo info = theText.textInfo;

        // if text not loaded yet by Unity
        if(info.characterCount < toChar) {
            // Debug.Log("SetTextColor called but Text length is: " + info.characterCount.ToString());
            updateLabel = true;
            return;
            }

        // WORKAROUND FOR WORDCOUNT BUG
        // Debug.Log("wordNr: " + wordNr.ToString());
        // Debug.Log("toChar: " + toChar.ToString());
        // wordNr = Mathf.Min(wordNr, theText.textInfo.wordInfo.Length);
        // TMP_WordInfo info = theText.textInfo.wordInfo[wordNr];

    	fromChar = Mathf.Min(fromChar, info.characterCount);
        toChar = (toChar == -1) ? info.characterCount - 1 : Mathf.Min(toChar, info.characterCount);

        for (int i = fromChar; i < toChar; ++i)
        {
            int meshIndex = info.characterInfo[i].materialReferenceIndex;
            int vertexIndex = info.characterInfo[i].vertexIndex;
        
            Color32[] vertexColors = theText.textInfo.meshInfo[meshIndex].colors32;
            for (int j = 0; j < 4; ++j)
                vertexColors[vertexIndex + j] = c;
        }
        
      theText.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
    }
   


    // Show text at parent object's position
    public void SetPos(Vector3 t) 
    {
        textObject.transform.position = t;
    }
   
}
