using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TechTreeController : MonoBehaviour
{
    public TextAsset techTreeDataTextAsset;
    public Transform techTreeCanvas;
    public GameObject techNodePrefab;

    private Dictionary<string, List<string>> techTreePrerequisites;
    private List<TechNode> techNodes;

    private void Start()
    {

        /*
         
        // Load the tech tree data from the resource file.
        LoadTechTreeData();

        // Create a tech node for each technology in the tech tree.
        techNodes = new List<TechNode>();
        foreach (string technology in techTreePrerequisites.Keys)
        {
            GameObject prefb = (GameObject)Resources.Load("prefabs/3d_hex");
            GameObject techNode = Instantiate(prefb, v3, Quaternion.identity);
            TechNode techNodeComponent = techNode.GetComponent<TechNode>();

            // Set the tech node's technology name and prerequisites.
            techNodeComponent.technologyName = technology;
            techNodeComponent.prerequisites = techTreePrerequisites[technology];

            techNodes.Add(techNodeComponent);
        }

        // Position the tech nodes in the tech tree.
        PositionTechNodes();
    }

    private void LoadTechTreeData()
    {

        TextAsset ta;
        ta = Resources.Load("serialize/TechTreeDataAsset") as TextAsset;
        string[] techTreeDataLines = ta.text.Split('|');

        try
        {
            ta = Resources.Load("serialize/TechTreeDataAsset") as TextAsset;
            ta.text.Split('|');
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

        // Read the tech tree data from the resource file.
        // string techTreeData = techTreeDataTextAsset.text;

        // Split the tech tree data into lines.
        // string[] techTreeDataLines = techTreeData.Split('\n');

        // Create a dictionary to store the tech tree data.
        techTreePrerequisites = new Dictionary<string, List<string>>();

        // Iterate over the tech tree data lines.
        foreach (string techTreeDataLine in techTreeDataLines)
        {
            // Split the tech tree data line into columns.
            string[] techTreeDataColumns = techTreeDataLine.Split(',');

            // Get the technology name and prerequisites.
            string technologyName = techTreeDataColumns[0];
            List<string> prerequisites = new List<string>();
            for (int i = 1; i < techTreeDataColumns.Length; i++)
            {
                prerequisites.Add(techTreeDataColumns[i]);
            }

            // Add the technology and prerequisites to the dictionary.
            techTreePrerequisites.Add(technologyName, prerequisites);
        }
    }

    private void PositionTechNodes()
    {
        // Get all of the tech nodes in the tech tree.
        TechNode[] techNodes = techTreeCanvas.GetComponentsInChildren<TechNode>();

        // Position the tech nodes in a hierarchical structure.
        for (int i = 0; i < techNodes.Length; i++)
        {
            TechNode techNode = techNodes[i];

            // Get the tech node's prerequisites.
            List<string> prerequisites = techNode.prerequisites;

            // If the tech node has no prerequisites, position it at the top of the tech tree.
            if (prerequisites.Count == 0)
            {
                techNode.transform.position = techTreeCanvas.position + new Vector3(0, -100, 0);
            }
            // Otherwise, position the tech node below its prerequisites.
            else
            {
                // Get the first prerequisite.
                string prerequisite = prerequisites[0];

                // Find the tech node with the matching prerequisite.
                TechNode prerequisiteTechNode = techNode.FirstOrDefault(prerequisite);

                // Position the tech node below the prerequisite tech node.
                techNode.transform.position = prerequisiteTechNode.transform.position + new Vector3(0, -100, 0);
            }
        }  */
    }

    // Update is called once per frame
    void Update()
    {

    }

}