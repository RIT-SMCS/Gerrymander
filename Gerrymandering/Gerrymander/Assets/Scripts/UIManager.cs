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
    public GameObject winPrefab, pausePrefab;
    bool solved = false;
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
	
    public void SetText(GameObject obj, string newText)
    {
        textDict[obj].text = newText;
    }

    public void SetColor(GameObject obj, Color color)
    {
        textDict[obj].color = color;
    }


    public void ShowVictory()
    {
        if (solved)
        {
            GameObject victory = GameObject.Instantiate(winPrefab);
            victory.transform.SetParent(transform);
            victory.transform.localPosition = new Vector3(0, 0, 0);
        }
        solved = true;
    }

    public void ClearClicked()
    {
        gameManager.ClearConnections();
    }

    public void ShowPauseMenu()
    {
        if(pausePrefab != null)
        {
            GameObject pauseMenu = Instantiate(pausePrefab) as GameObject;
            pauseMenu.transform.SetParent(transform);
            pauseMenu.transform.localPosition = new Vector3(0, 0, 0);
            pauseMenu.transform.localScale = new Vector3(1, 1, 1);
            pauseMenu.name = "PauseMenu";
            Button closeBtn = pauseMenu.transform.FindChild("Button").GetComponent<Button>() as Button;
            closeBtn.onClick.AddListener(delegate () { HidePauseMenu(); });
        }
    }

    public void HidePauseMenu()
    {
        Debug.Log("Close Menu");

        if (pausePrefab != null)
        {
            Destroy(transform.FindChild("PauseMenu").gameObject);
        }
    }

	// Update is called once per frame
	void Update () {
	}
}
