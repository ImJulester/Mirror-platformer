using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecentMaps : MonoBehaviour {
    public static RecentMaps instance;


    public List<string> recentMaps;
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
            if (PlayerPrefs.GetString(PlayerprefMap + i.ToString()) != "")
            {
                recentMaps.Add(PlayerPrefs.GetString(PlayerprefMap + i.ToString()));
            }

        }

    }

    public void SetPlayerPrefs()
    {

    }

    void Start () {

	}

    void OnApplicationQuit()
    {
        Debug.Log("set player prefs");
        mapCount = recentMaps.Count;
        PlayerPrefs.SetInt(playerPrefcount, mapCount);

        for (int i = 0; i < mapCount; i++)
        {
            PlayerPrefs.DeleteKey(PlayerprefMap + i.ToString());
            PlayerPrefs.SetString(PlayerprefMap + i.ToString(),recentMaps[i]);
        }
    }
    public void ResetRecentMaps()
    {
        mapCount = 0;
        recentMaps = new List<string>();
        PlayerPrefs.DeleteAll();
    }
}
