using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    GameManager gameManager;
    public GameObject gmObj;
    public GameObject Goal, Pop, District, GOP, Dem, Ind;
    Text GoalText, PopText, DistrictText, GOPText, DemText, IndText;
    Dictionary<GameObject, Text> textDict;
	// Use this for initialization
	void Start () {
	    if(gmObj != null)
        {
            gameManager = gmObj.GetComponent<GameManager>();
        }
        GoalText = Goal.GetComponent<Text>();
        PopText = Pop.GetComponent<Text>();
        DistrictText = District.GetComponent<Text>();
        GOPText = GOP.GetComponent<Text>();
        DemText = Dem.GetComponent<Text>();
        IndText = Ind.GetComponent<Text>();

        textDict = new Dictionary<GameObject, Text>();
        textDict.Add(Goal, GoalText);
        textDict.Add(Pop, PopText);
        textDict.Add(District, DistrictText);
        textDict.Add(GOP, GOPText);
        textDict.Add(Dem, DemText);
        textDict.Add(Ind, IndText);
	}
	
    public void setText(GameObject obj, string newText)
    {
        textDict[obj].text = newText;
    }

	// Update is called once per frame
	void Update () {
	}
}
