using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SFB;

public class Grid : MonoBehaviour {
    bool quit;
    bool openMap;

    bool firstMap = true;

    public static Grid grid = null;
    public GameObject tile;
    public List<GameObject> allTiles;
    public int width;
    public int height;

    public int[,] map;

    private string currentSavePath;
    private string currentSavePathinfo;
    private Vector2 startpos;

    public GameObject saveChanges;
    public GameObject createNewMap;
    // Use this for initialization

    void Awake()
    {
        if (grid == null)
        {
            grid = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {

        startpos = new Vector2(((-width * 32) / 8) * 3, (height * 32) / 2);
    }

    // Update is called once per frame
    void Update()
    {

    }

    #region mapMethods

    public void ChangeTile(int xpos, int ypos, int value)
    {
        map[xpos, ypos] = value;
    }


    public void CleanMap()
    {
        foreach (GameObject g in allTiles)
        {
            Destroy(g);
        }
        allTiles.Clear();
    }

    void OpenMap()
    {
        currentSavePath = StandaloneFileBrowser.OpenFilePanel("Open map", "", "map", false)[0];
        //currentSavePath = EditorUtility.OpenFilePanel("Open Map", "", "map");
        //currentSavePath = EditorUtility.OpenFilePanel("save directory", "", "bigmap" + ".map", ".map");
        string maptext;
        maptext = File.ReadAllText(currentSavePath);
        if (currentSavePath != null && maptext != null)
        {
            CleanMap();

            string[] mapInfo = File.ReadAllLines(currentSavePath.Replace(".map", "i.map"));
            int.TryParse(mapInfo[0], out width);
            int.TryParse(mapInfo[1], out height);
            map = new int[width, height];

            Camera.main.orthographicSize = height * 20;
            Camera.main.gameObject.GetComponent<CameraMovement>().Xbound = width * 18;
            Camera.main.gameObject.GetComponent<CameraMovement>().Ybound = height * 18;



            char[] separators = { ',', ';', ';','\n'};
            string[] values = maptext.Split(separators);
            int count = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int.TryParse(values[count], out map[x, y]);
                    Vector2 pos = startpos + new Vector2((x * 32) + x, (y * -32) - y);
                    GameObject block = Instantiate(tile, pos, Quaternion.identity) as GameObject;
                    allTiles.Add(block);
                    block.GetComponent<Block>().id = map[x, y];
                    block.GetComponent<Block>().xpos = x;
                    block.GetComponent<Block>().ypos = y;

                    count++;

                    if (y == height / 2 && x == width / 2)
                    {
                        Camera.main.gameObject.GetComponent<Transform>().position = new Vector3(block.GetComponent<Transform>().position.x, block.GetComponent<Transform>().position.y, -10);
                    }
                }
            }

        }


    }

    public void CreateMap(int w, int h)
    {
        if (allTiles.Count > 0)
        {
            CleanMap();
        }

        width = w;
        height = h;
        map = new int[width, height];
        Camera.main.orthographicSize = height * 20;
        //Camera.main.gameObject.GetComponent<Transform>().position += new Vector3(((width * 32) / 8) * 3, 0, 0);
        Camera.main.gameObject.GetComponent<CameraMovement>().Xbound = width * 18;
        Camera.main.gameObject.GetComponent<CameraMovement>().Ybound = height * 18;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                Vector2 pos = startpos + new Vector2((x * 32) + x, (y * -32) - y);
                GameObject block = Instantiate(tile, pos, Quaternion.identity) as GameObject;
                allTiles.Add(block);
                block.GetComponent<Block>().id = 5;
                map[x, y] = 5;
                block.GetComponent<Block>().xpos = x;
                block.GetComponent<Block>().ypos = y;

                if (y == height / 2 && x == width / 2)
                {
                    Camera.main.gameObject.GetComponent<Transform>().position = new Vector3(block.GetComponent<Transform>().position.x, block.GetComponent<Transform>().position.y, -10);
                }
            }
        }
    }

    void SaveMap(int[,] map, int width, int height, bool saveAs)
    {
        SpriteManager.instance.madeChanges = false;
        string[] mapinfo = new string[2];
        mapinfo[0] = width.ToString();
        mapinfo[1] = height.ToString();
        string[] lines = new string[height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] < 10)
                {
                    if (x != width - 1)
                    {
                        lines[y] = lines[y] + "0" + map[x, y] + ",";
                    }
                    else
                    {
                        lines[y] = lines[y] + "0" + map[x, y];
                    }
                }
                else
                {
                    if (x != width - 1)
                    {
                        lines[y] = lines[y] + map[x, y] + ",";
                    }
                    else
                    {
                        lines[y] = lines[y] + map[x, y];
                    }
                }


            }
        }

        if (saveAs)
        {
            currentSavePath = StandaloneFileBrowser.SaveFilePanel("save directory", "", "bigmap" + ".map", "map");
            //currentSavePath = EditorUtility.SaveFilePanel("save directory", "", "bigmap" + ".map", "map");
            currentSavePathinfo = currentSavePath.Replace(".map", "i.map");
        }
        Debug.Log(currentSavePathinfo + " before");
        if (currentSavePath == null || currentSavePathinfo == null)
        {
            currentSavePath = StandaloneFileBrowser.SaveFilePanel("save directory", "", "bigmap" + ".map", "map");
            currentSavePathinfo = currentSavePath.Replace(".map", "i.map");
        }
        Debug.Log(currentSavePathinfo + " after");
        if (currentSavePath != null)
        {
            File.WriteAllLines(currentSavePath, lines);
            File.WriteAllLines(currentSavePathinfo, mapinfo);
        }
    }
    #endregion

    #region Buttons

    public void ButtonOpenMap()
    {
        createNewMap.SetActive(false);
        openMap = true;

        if (firstMap)
        {
            firstMap = false;
            OpenMap();
            return;
        }

        if (!SpriteManager.instance.madeChanges)
        {
            OpenMap();
        }
        else
        {
            saveChanges.SetActive(true);
        }
    }

    public void NewMap()
    {
        openMap = false;

        if (firstMap)
        {
            firstMap = false;
            createNewMap.SetActive(true);
            return;
        }
        if (!SpriteManager.instance.madeChanges)
        {
            createNewMap.SetActive(true);
        }
        else
        {
            saveChanges.SetActive(true);
        }
    }

    public void CancelNewMap()
    {
        saveChanges.SetActive(false);

        if (quit)
        {
            Application.Quit();
        }
    }

    public void DontSave()
    {
        if (quit)
        {
            Application.Quit();
        }

        CleanMap();
        saveChanges.SetActive(false);
        if (openMap)
        {
            OpenMap();
        }
        else
        {
            createNewMap.SetActive(true);
            currentSavePath = null;
        }


    }

    public void SaveChanges()
    {
        SaveMap(map, width, height, false);
        saveChanges.SetActive(false);

        if (quit)
        {
            Application.Quit();
        }

        if (openMap)
        {
            OpenMap();
        }
        else
        {
            createNewMap.SetActive(true);
            currentSavePath = null;
        }
    }

    public void Save()
    {
        SaveMap(map, width, height, false);
    }

    public void SaveAs()
    {
        SaveMap(map, width, height, true);
    }
    #endregion


    void OnApplicationQuit()
    {
        quit = true;
        Application.CancelQuit();

        saveChanges.SetActive(true);
    }

    }
