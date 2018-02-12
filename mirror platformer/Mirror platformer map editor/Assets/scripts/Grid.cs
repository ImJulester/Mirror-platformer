using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SFB;
using UnityEngine.UI;
using UnityEngine.Events;
public class Grid : MonoBehaviour {
    bool quit;
    bool openMap;
    bool canceled;
    bool firstMap = true;

    public static Grid grid = null;
    public GameObject tile;
    public List<GameObject> allTiles;
    public int width;
    public int height;

    public int[,] map;
    private Block[,] tiles;
    public string currentSavePath;
    private string currentSavePathinfo;
    private Vector2 startpos;

    public Button[] preview;

    public GameObject saveChanges;
    public GameObject createNewMap;
    public GameObject recentUsedMaps;
    public GameObject TilesPanel;
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
        MakePreview();
        TilesPanel.SetActive(false);
        recentUsedMaps.SetActive(true);
        startpos = new Vector2(((-width * 32) / 8) * 3, (height * 32) / 2);
    }

    // Update is called once per frame
    void Update()
    {


        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log(currentSavePath);
        }
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

    int[,] OpenMapFile(string path)
    {

        int[,] ReturnMap = new int[0, 0];
        currentSavePath = path;
        currentSavePathinfo = currentSavePath.Replace(".map", ".mapinfo");
        //currentSavePath = EditorUtility.OpenFilePanel("Open Map", "", "map");
        //currentSavePath = EditorUtility.OpenFilePanel("save directory", "", "bigmap" + ".map", ".map");
        string maptext;
        if (!File.Exists(currentSavePath) || !File.Exists(currentSavePathinfo))
        {
            return null;
        }
        maptext = File.ReadAllText(currentSavePath);
        if (currentSavePath != null && maptext != null)
        {
       
            string[] mapInfo = File.ReadAllLines(currentSavePath.Replace(".map", ".mapinfo"));
            int.TryParse(mapInfo[0], out width);
            int.TryParse(mapInfo[1], out height);
            ReturnMap = new int[width, height];

            Camera.main.orthographicSize = height * 20;
            Camera.main.gameObject.GetComponent<CameraMovement>().Xbound = width * 18;
            Camera.main.gameObject.GetComponent<CameraMovement>().Ybound = height * 18;



            char[] separators = { ',', ';', ';', '\n' };
            string[] values = maptext.Split(separators);
            int count = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int.TryParse(values[count], out ReturnMap [x, y]);
                    count++;
                }
            }

        }

        return ReturnMap;
    }

    void OpenMap(string s)
    {
        firstMap = false;
        TilesPanel.SetActive(true);
        recentUsedMaps.SetActive(false);
        currentSavePath = s;
        currentSavePathinfo = currentSavePath.Replace(".map", ".mapinfo");
        Debug.Log(currentSavePath);
        //currentSavePath = EditorUtility.OpenFilePanel("Open Map", "", "map");
        //currentSavePath = EditorUtility.OpenFilePanel("save directory", "", "bigmap" + ".map", ".map");
        string maptext;
        maptext = File.ReadAllText(currentSavePath);
        if (currentSavePath != null && maptext != null)
        {
            CleanMap();

            string[] mapInfo = File.ReadAllLines(currentSavePath.Replace(".map", ".mapinfo"));
            int.TryParse(mapInfo[0], out width);
            int.TryParse(mapInfo[1], out height);
            map = new int[width, height];
            SpriteManager.instance.ResetBlocks(width, height);

            Camera.main.orthographicSize = height * 20;
            Camera.main.gameObject.GetComponent<CameraMovement>().Xbound = width * 18;
            Camera.main.gameObject.GetComponent<CameraMovement>().Ybound = height * 18;



            char[] separators = { ',', ';', ';', '\n' };
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

                    SpriteManager.instance.SetBlocks(x, y, block.GetComponent<Block>());
                }
            }

        }

        if (!RecentMaps.instance.recentMaps.Contains(currentSavePath))
        {
            RecentMaps.instance.mapCount++;
            RecentMaps.instance.recentMaps.Insert(0, currentSavePath);
        }
        else
        {
            RecentMaps.instance.recentMaps.Remove(currentSavePath);
            RecentMaps.instance.recentMaps.Insert(0, currentSavePath);
        }
    }

    void OpenMap()
    {
        TilesPanel.SetActive(true);
        recentUsedMaps.SetActive(false);
        currentSavePath = StandaloneFileBrowser.OpenFilePanel("Open map", "", "map", false)[0];
        currentSavePathinfo = currentSavePath.Replace(".map", ".mapinfo");
        Debug.Log(currentSavePath);
        //currentSavePath = EditorUtility.OpenFilePanel("Open Map", "", "map");
        //currentSavePath = EditorUtility.OpenFilePanel("save directory", "", "bigmap" + ".map", ".map");
        string maptext;
        maptext = File.ReadAllText(currentSavePath);
        if (currentSavePath != null && maptext != null)
        {
            CleanMap();

            string[] mapInfo = File.ReadAllLines(currentSavePath.Replace(".map", ".mapinfo"));
            int.TryParse(mapInfo[0], out width);
            int.TryParse(mapInfo[1], out height);
            map = new int[width, height];
            SpriteManager.instance.ResetBlocks(width, height);

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
                    SpriteManager.instance.SetBlocks(x, y, block.GetComponent<Block>());
                }
            }

        }

        if (!RecentMaps.instance.recentMaps.Contains(currentSavePath))
        {
            RecentMaps.instance.mapCount++;
            RecentMaps.instance.recentMaps.Insert(0, currentSavePath);
        }
        else
        {
            RecentMaps.instance.recentMaps.Remove(currentSavePath);
            RecentMaps.instance.recentMaps.Insert(0, currentSavePath);
        }
    }

    public void CreateMap(int w, int h,bool blank)
    {
        if (allTiles.Count > 0)
        {
            CleanMap();
        }

        width = w;
        height = h;
        map = new int[width, height];

        SpriteManager.instance.ResetBlocks(width, height);
        //tiles = new Block[width, height];
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
                if (!blank && y+1 > height/2)
                {
                    block.GetComponent<Block>().id = 1;
                    map[x, y] = 1;
                }
                else
                {
                    block.GetComponent<Block>().id = 5;
                    map[x, y] = 5;
                }
               
                block.GetComponent<Block>().xpos = x;
                block.GetComponent<Block>().ypos = y;

                if (y == height / 2 && x == width / 2)
                {
                    Camera.main.gameObject.GetComponent<Transform>().position = new Vector3(block.GetComponent<Transform>().position.x, block.GetComponent<Transform>().position.y, -10);
                }
                SpriteManager.instance.SetBlocks(x, y, block.GetComponent<Block>());
                //tiles[x, y] = block.GetComponent<Block>();
            }
        }
    }

    void SaveMap(int[,] map, int width, int height, bool saveAs)
    {
        SpriteManager.instance.madeChanges = false;

        if (map == null)
            return;


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
            currentSavePathinfo = currentSavePath.Replace(".map", ".mapinfo");
        }
        if (currentSavePath == null || currentSavePath == "" || currentSavePathinfo == "" || currentSavePathinfo == null)
        {
            currentSavePath = StandaloneFileBrowser.SaveFilePanel("save directory", "", "bigmap" + ".map", "map");
            currentSavePathinfo = currentSavePath.Replace(".map", ".mapinfo");
        }
        if (currentSavePath != null || currentSavePath != "")
        {
            File.WriteAllLines(currentSavePath, lines);
            File.WriteAllLines(currentSavePathinfo, mapinfo);
        }

        if (!RecentMaps.instance.recentMaps.Contains(currentSavePath))
        {
            RecentMaps.instance.mapCount++;
            RecentMaps.instance.recentMaps.Insert(0,currentSavePath);
        }
        else
        {
            RecentMaps.instance.recentMaps.Remove(currentSavePath);
            RecentMaps.instance.recentMaps.Insert(0,currentSavePath);
        }
    }
    #endregion

    #region Buttons

    public void ButtonOpenMap()
    {
        recentUsedMaps.SetActive(false);
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
        TilesPanel.SetActive(true);
    }

    public void NewMap()
    {
        openMap = false;
        recentUsedMaps.SetActive(false);
        if (firstMap)
        {
            firstMap = false;
            createNewMap.SetActive(true);
            currentSavePath = null;
            TilesPanel.SetActive(true);
            return;
        }
        if (!SpriteManager.instance.madeChanges)
        {
            CleanMap();
            createNewMap.SetActive(true);
            currentSavePath = null;
            TilesPanel.SetActive(true);
        }
        else
        {
            StartCoroutine(OpenAfterSave(createNewMap,TilesPanel));
            TilesPanel.SetActive(true);
            currentSavePath = null;
        }
    }

    public void CancelNewMap()
    {
        saveChanges.SetActive(false);
        canceled = true;
        if (quit)
        {
            quit = false;
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
            //createNewMap.SetActive(true);
            currentSavePath = null;
        }


    }

    public void ToggleTilePanel(Toggle t)
    {
        TilesPanel.GetComponent<Animator>().SetBool("Open", t.isOn);
        RectTransform[] tr = t.GetComponentsInChildren<RectTransform>();
        tr[1].localRotation = Quaternion.Euler(0, 0, t.isOn ? 0 : 180);
    }

    public IEnumerator OpenAfterSave(GameObject g)
    {
        saveChanges.SetActive(true);
        while (saveChanges.activeSelf == true)
        {
            yield return new WaitForSeconds(0.5f);
        }
        TilesPanel.SetActive(false);
        recentUsedMaps.SetActive(true);
    }

    public IEnumerator OpenAfterSave(GameObject g,GameObject t)
    {
        saveChanges.SetActive(true);
        while (saveChanges.activeSelf == true)
        {
            yield return new WaitForSeconds(0.5f);
        }
        if (canceled)
        {
            canceled = false;
            yield break;
        }
        CleanMap();
        t.SetActive(false);
        g.SetActive(true);
    }

    public void RecentMap()
    {
        if (!SpriteManager.instance.madeChanges)
        {
            CleanMap();
            TilesPanel.SetActive(false);
            recentUsedMaps.SetActive(true);
        }
        else
        {
            CleanMap();
            StartCoroutine(OpenAfterSave(recentUsedMaps, TilesPanel));
        }

    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SaveChanges()
    {
        SaveMap(map, width, height, false);
        saveChanges.SetActive(false);

        if (quit)
        {
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #else
            Application.Quit();
    #endif
            //Application.Quit();
        }

        if (openMap)
        {
            OpenMap();
        }
        else
        {
            //createNewMap.SetActive(true);
            //currentSavePath = null;
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


    #region ImageCreating
    public Sprite MakeSprite(string s)
    {
        if (s != "")
        {
            int[,] currentMap = OpenMapFile(s);
            if (currentMap == null)
            {
                return null;
            }
            Texture2D texture = new Texture2D(currentMap.GetLength(0), currentMap.GetLength(1));

            Debug.Log(currentMap.GetLength(0));
            Debug.Log(currentMap.GetLength(1));
            for (int y = 0; y < currentMap.GetLength(1); y++)
            {
                for (int x = 0; x < currentMap.GetLength(0); x++)
                {
                    switch (currentMap[x, y])
                    {
                        case 1:
                            texture.SetPixel(x, y, Color.black);
                            break;
                        case 5:
                            texture.SetPixel(x, y, Color.white);
                            break;
                        case 8:
                            texture.SetPixel(x, y, Color.red);
                            break;
                        default:
                            texture.SetPixel(x, y, Color.white);
                            break;
                    }
                }
            }
            texture.Apply();
            return Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        }

        else
        {
            Debug.Log("failed to open image");
            return null;
        }
    }


    void MakePreview()
    {
        for (int i = 0; i < RecentMaps.instance.mapCount; i++)
        {
            if (i < 6)
            {
                if (i < preview.Length)
                {
                    Sprite s = MakeSprite(RecentMaps.instance.recentMaps[i]);
                    if (s == null)
                    {
                        Destroy(preview[i].gameObject);
                        RecentMaps.instance.recentMaps.Remove(RecentMaps.instance.recentMaps[i]);
                        continue;
                    }
                    preview[i].image.sprite = MakeSprite(RecentMaps.instance.recentMaps[i]);
                    if (preview[i].image.sprite != null)
                    {
                        preview[i].gameObject.SetActive(true);
                        preview[i].image.preserveAspect = true;
                        Button tempButton = preview[i];
                        string tempString = RecentMaps.instance.recentMaps[i];
                        tempButton.onClick.AddListener(() => OpenMap(tempString));

                        char[] separators = {Path.DirectorySeparatorChar};
                        string[] name = currentSavePath.Split(separators);
                        tempButton.gameObject.GetComponentInChildren<Text>().text = name[name.Length-1];
                    }
                    else
                    {
                        Debug.Log("failed to make button bcuz no sprite");
                    }
                }
            }
        }
        currentSavePath = null;
    }
    #endregion

    void OnApplicationQuit()
    {
        openMap = false;
        if (!quit)
        {
            quit = true;
            if (SpriteManager.instance.madeChanges)
            {
                Application.CancelQuit();

                saveChanges.SetActive(true);
            }
        }

    }

    }
