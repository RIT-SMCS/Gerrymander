using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class LevelGenerator {
    private static System.Array affiliations = System.Enum.GetValues(typeof(Affiliation));

    public static string GenerateNewLevel(Affiliation winner, int numUnits, int numDistricts)
    {
        if (numUnits % numDistricts != 0 || numDistricts < 3 || numUnits < numDistricts*3)
        {
            throw new System.Exception("INVALID GENERATION: incorrect unit-to-district ration");
        }
        
        //generated level format string
        string lvl = "";
        //calculated required amounts of districts and units per district required to satisfy the desired win condition
        int unitsPerDistrict = numUnits / numDistricts;
        int minWinnableDistricts = ((numDistricts + 1) / 3) + 1;
        int minWinnableVotersPerDistrict = ((unitsPerDistrict + 1) / 3) + 1;
        //( (n+1) / 3) +1
        
        List<District> districts = new List<District>();
        //generate nodes for the districts
        //first district is made by going in a loop.
        //second district is made by starting at one node of the first and going in a loop until it intersects the first.
        //additional districts follow second district algorithm
        List<Node> nodes = new List<Node>();
        Vector3 pos = new Vector3();
        float theta = 0.0f;
        while (theta < 2 * Mathf.PI)
        {
            float dist = Random.Range(0.5f, 1.5f);
            Vector3 translation = dist * new Vector3(Mathf.Cos(theta), 0.0f, Mathf.Sin(theta));
            pos += translation;
            Node n = new Node();
            //n.transform.position = pos;
            nodes.Add(n); 
        }
        
        //generate units for the districts
        List<Unit> units = new List<Unit>();
        for (int d = 0; d < minWinnableDistricts; ++d) // for each winnable district
        {
            Unit unit = new Unit();
            for (int u = 0; u < minWinnableVotersPerDistrict; ++u) //for each unit that must match said winnable district
            {
                unit.affiliation = winner;
            }
            for (int u = minWinnableVotersPerDistrict; u < unitsPerDistrict; ++u) //fill the rest randomly
            {
                unit.affiliation = GetRandomAffiliation();
            }
        }
        for (int d = minWinnableDistricts; d < numDistricts; ++d) //for the rest of the districts, fill randomly
        {
            Unit unit = new Unit();
            for (int u = 0; u < unitsPerDistrict; ++unitsPerDistrict)
            {
                unit.affiliation = GetRandomAffiliation();
            }
        }

        //OR generate random nodes. connect all exterior nodes. cut up the interior into n districts. 
        //remove unused nodes
        //populate districts with units

        //finally translate and rotate all nodes/units so that they fit on the screen.
        return lvl;
    }
    private static Affiliation GetRandomAffiliation()
    {
        return (Affiliation)affiliations.GetValue((int)Random.Range(0, affiliations.Length));
    }
}
