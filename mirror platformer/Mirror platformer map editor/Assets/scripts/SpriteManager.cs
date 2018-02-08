using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class SpriteManager : MonoBehaviour {
    public static SpriteManager instance = null;
    
   
    
    public GameObject[] tileUIs;

    public int selectedID;

    public Sprite[] tiles;

    public bool madeChanges;

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
        selectedID = 0;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButton(1))
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

                if (hit.collider != null)
                {
                    tileUIs[selectedID].SetActive(false);
                    selectedID = hit.collider.GetComponent<Block>().id;
                    tileUIs[selectedID].SetActive(true);
                }
            }

                if (Input.GetMouseButton(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

                if (hit.collider != null)
                {
                    if (hit.collider.GetComponent<Block>().id != selectedID)
                    {
                        madeChanges = true;
                        hit.collider.GetComponent<Block>().id = selectedID;
                        hit.collider.GetComponent<Block>().ChangeTile();
                    }

                }
            }
        }


	}

    public void setSelectedTile(int i)
    {
        tileUIs[selectedID].SetActive(false);
        selectedID = i;
        tileUIs[selectedID].SetActive(true);
    }

}
