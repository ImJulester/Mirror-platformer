using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecentMaps : MonoBehaviour {
    public static RecentMaps instance;


    public List<string> recentMaps;

    public bool doneLoading;
    public int mapCount;
    private string playerPrefcount = "playerPrefCount";
    private string PlayerprefMap = "playerprefMap";


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        mapCount = PlayerPrefs.GetInt(playerPrefcount);

        for (int i = 0; i < mapCount; i++)
        {
            recentMaps.Add(PlayerPrefs.GetString(PlayerprefMap + i.ToString()));
        }

        doneLoading = true;
    }

    public void SetPlayerPrefs()
    {

    }

    void Start () {

	}

    void OnApplicationQuit()
    {
        Debug.Log("set player prefs");
        PlayerPrefs.SetInt(playerPrefcount, mapCount);

        for (int i = 0; i < mapCount; i++)
        {
            PlayerPrefs.SetString(PlayerprefMap + i.ToString(),recentMaps[i]);
        }
    }
    void Update () {
        if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            Debug.Log("Delete all keys");
            PlayerPrefs.DeleteAll();
        }
	}
}
