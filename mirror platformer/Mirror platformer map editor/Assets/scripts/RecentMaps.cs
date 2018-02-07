using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecentMaps : MonoBehaviour {
    public static RecentMaps instance;


    public List<string> recentMaps;

    private int mapCount;
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
    }

    void Start () {
        mapCount = PlayerPrefs.GetInt(playerPrefcount);

        for (int i = 0; i < mapCount; i++)
        {
            recentMaps.Add(PlayerPrefs.GetString(PlayerprefMap + i.ToString()));
        }
	}

    void OnApplicationQuit()
    {
        PlayerPrefs.SetInt(playerPrefcount, mapCount);
    }
    void Update () {
		
	}
}
