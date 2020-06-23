using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MyGameManager : MonoBehaviour
{
    
    public Transform player, level1pos, level2pos;//to spawn player wrt level
    public Transform dust, myCamera;
    public GameObject[] levels;
    public Text LevelText;
    public GameObject TapToStartObj;
    public GameObject currentLevel, currentPlayer;//hold to restore
    public static MyGameManager Instance;
    void Start()
    {
        Instance = this;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        currentLevel = Instantiate(levels[0]);
        currentPlayer = Instantiate(player.gameObject);
        currentPlayer.transform.position = level1pos.position;
        dust.GetComponent<ParticleFollower>().target = currentPlayer.transform;
        myCamera.GetComponent<CameraBehaviour>().target = currentPlayer.transform.GetChild(0);
        TapToStartObj.SetActive(true);
        TapToStartObj.GetComponent<TaptoStartBehaviour>().oneTime = true;
    }
    public void loadLevel(int i)
    {
        Destroy(currentPlayer);
        Destroy(currentLevel);
        GameObject[] cubeArray = GameObject.FindGameObjectsWithTag("Collect");
        for (int z = 0; z < cubeArray.Length; z++)
        {
            Destroy(cubeArray[z]);
        }
        GameObject[] playerArray = GameObject.FindGameObjectsWithTag("Player");
        for (int z = 0; z < playerArray.Length; z++)
        {
            Destroy(playerArray[z]);
        }

        currentLevel = Instantiate(levels[i]);
        currentPlayer = Instantiate(player.gameObject);
        if (i == 0) currentPlayer.transform.position = level1pos.position;
        if (i == 1) currentPlayer.transform.position = level2pos.position;
        dust.GetComponent<ParticleFollower>().target = currentPlayer.transform;
        myCamera.GetComponent<CameraBehaviour>().target = currentPlayer.transform.GetChild(0);
        LevelText.text = "Level " + (i + 1).ToString();
        TapToStartObj.SetActive(true);
        TapToStartObj.GetComponent<TaptoStartBehaviour>().oneTime = true;
    }

    public void resetLevel()
    {
        if (currentLevel.name.Contains("1"))
        {
            loadLevel(0);
        }
        else
        {
            loadLevel(1);
        }
    }
    public void changeLevel()
    {
        if (currentLevel.name.Contains("1"))
        {
            loadLevel(1);
        }
        else
        {
            loadLevel(0);
        }
    }
}
