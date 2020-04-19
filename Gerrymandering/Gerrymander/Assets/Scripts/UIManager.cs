 using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.iOS;

public class UIManager : MonoBehaviour {
    GameManager gameManager;
    public GameObject gmObj;
    public GameObject[] Goal, Pop, District;
    public GameObject GOP, Dem, Ind;
    Text[] GoalText, PopText, DistrictText;
    Text GOPText, DemText, IndText;
    Dictionary<TextType, List<Text>> textDict;
    public GameObject winPrefab, pausePrefab;
    public Text densityText, popsText, yPop, mPop, gPop;
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
        textDict = new Dictionary<TextType, List<Text>>();
        textDict.Add(TextType.Goal, new List<Text>());
        GoalText = new Text[Goal.Length];
        for (int i = 0; i < Goal.Length; i++)
        {
            GoalText[i] = Goal[i].GetComponent<Text>();
            textDict[TextType.Goal].Add(GoalText[i]);
        }

        textDict.Add(TextType.Density, new List<Text>());
        textDict[TextType.Density].Add(densityText);

        textDict.Add(TextType.Pop, new List<Text>());
        PopText = new Text[Pop.Length];
        for (int i = 0; i < Pop.Length; i++)
        {
            PopText[i] = Pop[i].GetComponent<Text>();
            textDict[TextType.Pop].Add(PopText[i]);
        }
        textDict.Add(TextType.District, new List<Text>());
        DistrictText = new Text[Goal.Length];
        for (int i = 0; i < Goal.Length; i++)
        {
            DistrictText[i] = District[i].GetComponent<Text>();
            textDict[TextType.District].Add(DistrictText[i]);
        }
        GOPText = GOP.GetComponent<Text>();
        DemText = Dem.GetComponent<Text>();
        IndText = Ind.GetComponent<Text>();
        textDict.Add(TextType.GOP, new List<Text>());
        textDict[TextType.GOP].Add(GOPText);
        textDict.Add(TextType.Dem, new List<Text>());
        textDict[TextType.Dem].Add(DemText);
        textDict.Add(TextType.Ind, new List<Text>());
        textDict[TextType.Ind].Add(IndText);

        textDict.Add(TextType.PopTot, new List<Text>());
        textDict[TextType.PopTot].Add(popsText);
        textDict.Add(TextType.yPop, new List<Text>());
        textDict[TextType.yPop].Add(yPop);
        textDict.Add(TextType.mPop, new List<Text>());
        textDict[TextType.mPop].Add(mPop);
        textDict.Add(TextType.gPop, new List<Text>());
        textDict[TextType.gPop].Add(gPop);
    }
	
    public void SetText(TextType type, string newText)
    {
        List<Text> texts = textDict[type];
        foreach (Text text in texts)
        {
            text.text = newText;
        }
    }

    public void SetColor(TextType type, Color color)
    {
        List<Text> texts = textDict[type];
        foreach (Text text in texts)
        {
            text.color = color;
        }
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
            if(gameManager.transitionPrefab.GetComponent<Transition>().readyForNext)         
            {
                gameManager.NextLevel();
            }
        }
	}


}
