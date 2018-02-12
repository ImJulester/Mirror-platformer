using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class SpriteManager : MonoBehaviour {
    public static SpriteManager instance = null;


    public Color selected;
    public Color deselected;

    public Image[] tools;
    
    public GameObject[] tileUIs;

    public int selectedID;

    public Sprite[] tiles;

    private Block[,] blocks;
    private int gridWidth;
    private int gridHeight;

    public bool madeChanges;

    private enum ToolType {brush,bucket};

    ToolType tool;

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

	// Use this for initialization
	void Start () {
        SetTool(0);
        selectedID = 0;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButton(1))
            {
                Debug.Log(tool);
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);                   
                    if (hit.collider != null)
                    {
                        tileUIs[selectedID].SetActive(false);
                        selectedID = hit.collider.GetComponent<Block>().id;
                        tileUIs[selectedID].SetActive(true);
                    }
                
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                tool = ToolType.bucket;
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                tool = ToolType.brush;
            }

            if (Input.GetMouseButton(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
              


                if (hit.collider != null)
                {
                    if (tool == ToolType.brush)
                    {


                        if (hit.collider.GetComponent<Block>().id != selectedID)
                        {
                            madeChanges = true;
                            hit.collider.GetComponent<Block>().id = selectedID;
                            hit.collider.GetComponent<Block>().ChangeTile();
                        }
                    }
                    else if (tool == ToolType.bucket)
                    {
                        Debug.Log(tool);
                        if (hit.collider.GetComponent<Block>().id != selectedID)
                        {
                            madeChanges = true;
                            Bucket(hit.collider.GetComponent<Block>());
                        }
                    }
                }
            }
        }


	}

    public void SetTool(int i)
    {
        tool = (ToolType)i;
        for (int x = 0; x < tools.Length; x++)
        {
            if (x == i)
            {
                tools[x].color = selected;
            }
            else
            {
                tools[x].color = deselected;
            }
        }
       // Debug.Log(tool);
    }

    public void Bucket(Block b)
    {
        List<Block> change = new List<Block>();
        int id = b.id;
        change.Add(b);
        int count = 0;
        while (count < change.Count)
        {
            int x = change[count].xpos;
            int y = change[count].ypos;

           // Debug.Log(x + "                " + y);
            if (y -1 >= 0 && y +1 <= gridHeight -1)
            {
                if (blocks[x, y -1].id == id)
                {
                    if (!change.Contains(blocks[x, y - 1]))
                    {
                        change.Add(blocks[x, y - 1]);
                    }
                }
                if (blocks[x, y + 1].id == id)
                {
                    if (!change.Contains(blocks[x, y + 1]))
                    {
                        change.Add(blocks[x, y + 1]);
                    }
                   
                }
            }
            if (x - 1 >= 0 && x + 1 <= gridWidth -1)
            {
                if (blocks[x - 1, y].id == id)
                {
                    if (!change.Contains(blocks[x - 1, y]))
                    {
                        change.Add(blocks[x - 1, y]);
                    }
                    
                }
                if (blocks[x + 1, y].id == id)
                {
                    if (!change.Contains(blocks[x + 1, y]))
                    {
                        change.Add(blocks[x + 1, y]);
                    }
                    
                }
            }
            count++;
        }

        foreach (Block tile in change)
        {
            tile.id = selectedID;
            tile.ChangeTile();
        }
    }

    public void ResetBlocks(int width,int height)
    {
        gridWidth = width;
        gridHeight = height;
        blocks = new Block[width, height];
    }
    public void SetBlocks(int x, int y, Block b)
    {
        blocks[x, y] = b;
    }
    public void setSelectedTile(int i)
    {
        tileUIs[selectedID].SetActive(false);
        selectedID = i;
        tileUIs[selectedID].SetActive(true);
    }

}
