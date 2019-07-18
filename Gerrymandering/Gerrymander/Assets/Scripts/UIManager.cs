 using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.iOS;

public class UIManager : MonoBehaviour {
    GameManager gameManager;
    public GameObject gmObj;
    public GameObject Goal, Pop, District, GOP, Dem, Ind;
    Text GoalText, PopText, DistrictText, GOPText, DemText, IndText;
    Dictionary<GameObject, Text> textDict;
    public GameObject winPrefab, pausePrefab;
    bool solved = false;
    bool paused = false;
	// Use this for initialization
	void Start () {
        Rect safe = Screen.safeArea;

        if (Device.generation >= DeviceGeneration.iPhoneX && Device.generation <= DeviceGeneration.iPhoneXR)
        {
            gameObject.GetComponent<CanvasScaler>().matchWidthOrHeight = 0.8f;
            GameObject topPanel = transform.Find("TopPanel").gameObject;
            topPanel.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, -50, 0);
            topPanel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 100);
            topPanel.GetComponent<GridLayoutGroup>().padding.top = 50;

            GameObject botPanel = transform.Find("BottomPanel").gameObject;
            botPanel.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 42.5f, 0);
            botPanel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 85);
            botPanel.GetComponent<GridLayoutGroup>().padding.bottom = 25;
        }

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
        if (!solved)
        {
			gameManager.transitionPrefab.GetComponent<Transition>().FadeIn(); 
        }
        solved = true;
    }

    public void ClearClicked()
    {
        gameManager.ClearConnections();
    }

    public void ShowPauseMenu()
    {
        if(pausePrefab != null && !paused)
        {
            GameObject pauseMenu = Instantiate(pausePrefab) as GameObject;
            pauseMenu.transform.SetParent(transform);
            pauseMenu.transform.localPosition = new Vector3(0, 0, 0);
            pauseMenu.transform.localScale = new Vector3(1, 1, 1);
            pauseMenu.name = "PauseMenu";
            Button closeBtn = pauseMenu.transform.Find("CloseButton").Find("Button").GetComponent<Button>() as Button;
            closeBtn.onClick.AddListener(delegate () { HidePauseMenu(); });
            paused = true;
        }
    }

    public void HidePauseMenu()
    {
        Debug.Log("Close Menu");

        if (pausePrefab != null)
        {
            Destroy(transform.Find("PauseMenu").gameObject);
            paused = false;
        }
    }

	// Update is called once per frame
	void Update () {
        if (solved)
        {
            if(gameManager.transitionPrefab.GetComponent<Transition>().readyForNext) {                 
                if (!gameManager.transitionPrefab.GetComponent<Transition>().lastLevel)
                {

                    gameManager.NextLevel();
                }
                else
                {
                    gameManager.MainMenu();
                }
            }
        }
	}


}
