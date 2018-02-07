using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpriteManager : MonoBehaviour {
    public static SpriteManager instance = null;

    public int selectedID;

    public Sprite[] tiles;

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
            if (Input.GetMouseButton(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

                if (hit.collider != null)
                {
                    hit.collider.GetComponent<Block>().id = selectedID;
                    hit.collider.GetComponent<Block>().ChangeTile();
                    Debug.Log(hit.collider.GetComponent<Transform>().position);
                }
            }
        }


	}

    public void setSelectedTile(int i)
    {
        selectedID = i;
    }

}
