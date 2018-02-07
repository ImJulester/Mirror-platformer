using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class Grid : MonoBehaviour {
    public static Grid grid = null;
    public GameObject tile;
    public List<GameObject> allTiles;
    public int width;
    public int height;

    public int[,] map;


    private string currentSavePath;
    private string currentSavePathinfo;
    private Vector2 startpos;
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
    void Start() {

        startpos = new Vector2(((-width * 32) / 8) * 3, (height * 32) / 2);

        map = new int[width, height];
        Camera.main.orthographicSize = height * 20;
        Camera.main.gameObject.GetComponent<Transform>().position += new Vector3(((width * 32) / 8) * 3, 0, 0);
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
            }
        }
        // SaveToFile.save.SaveMap(map, width, height);


    }

    // Update is called once per frame
    void Update() {

    }

    public void ChangeTile(int xpos, int ypos, int value)
    {
        map[xpos, ypos] = value;
    }

    public void Save()
    {
        SaveMap(map, width, height,false);
    }
    public void SaveAs()
    {
        SaveMap(map, width, height, true);
    }

    public void OpenMap()
    {
        currentSavePath = EditorUtility.OpenFilePanel("Open Map", "", "map");
        //currentSavePath = EditorUtility.OpenFilePanel("save directory", "", "bigmap" + ".map", ".map");
        string maptext;
        maptext = File.ReadAllText(currentSavePath);
        if (currentSavePath != null && maptext != null)
        {
            foreach (GameObject g in allTiles)
            {
                Destroy(g);
            }
            allTiles.Clear();
            string[] mapInfo = File.ReadAllLines(currentSavePath.Replace(".map", "i.map"));
            int.TryParse(mapInfo[0], out width);
            int.TryParse(mapInfo[1], out height);
            map = new int[width, height];
            char[] separators = { ',', ';', ';' };
            string[] values = maptext.Split(separators);
            int count = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width -1; x++)
                {
                    int.TryParse(values[count], out map[x, y]);
                    Vector2 pos = startpos + new Vector2((x * 32) + x, (y * -32) - y);
                    GameObject block = Instantiate(tile, pos, Quaternion.identity) as GameObject;
                    allTiles.Add(block);
                    block.GetComponent<Block>().id = map[x, y];
                    block.GetComponent<Block>().xpos = x;
                    block.GetComponent<Block>().ypos = y;

                    count++;
                }
            }

        }


    }

        void SaveMap(int[,] map, int width, int height, bool saveAs)
    {
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
            currentSavePath = EditorUtility.SaveFilePanel("save directory", "", "bigmap" + ".map", ".map");
            currentSavePathinfo = currentSavePath.Replace(".map", "i.map");
        }
        if (currentSavePath != null)
        {
            File.WriteAllLines(currentSavePath, lines);
            File.WriteAllLines(currentSavePathinfo, mapinfo);
        }
    }
}
