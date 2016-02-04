using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    GameManager gameManager;
    public GameObject gmObj;
    public GameObject Goal, Pop, District, GOP, Dem, Ind;
    public Text GoalText, PopText, DistrictText, GOPText, DemText, IndText;
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
	}
	
	// Update is called once per frame
	void Update () {
	}
}
