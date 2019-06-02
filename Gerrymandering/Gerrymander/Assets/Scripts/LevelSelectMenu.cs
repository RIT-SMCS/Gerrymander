using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectMenu : MonoBehaviour {
    int levelCount = 0;
    List<bool> levelsCompleted = new List<bool>();
    List<GameObject> levelButtons = new List<GameObject>();
    public GameObject levelButtonPrefab;

	// Use this for initialization
	void Start () {
        for (int i =0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string sceneName = SceneUtility.GetScenePathByBuildIndex(i);
            sceneName = sceneName.Remove(0,21);
            sceneName = sceneName.Remove(sceneName.IndexOf("."));
            if (sceneName.StartsWith("Lvl_") && int.Parse(sceneName.Substring(4)) > 0)
            {
                levelCount += 1;
                levelsCompleted.Add(PlayerPrefs.GetInt(levelCount.ToString(), defaultValue: 0) > 0);
            }

        }

        for (int j = 0; j < levelCount; j++)
        {
            GameObject tempButton = levelButtonPrefab;
            LevelButton levelButton = levelButtonPrefab.GetComponent<LevelButton>();
            levelButton.levelNumber = j + 1;
            levelButton.completed = levelsCompleted[j];
            
            levelButtons.Add(Instantiate(tempButton));
            levelButtons[levelButtons.Count - 1].transform.SetParent(transform);

        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
