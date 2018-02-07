using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {

    public int xpos;
    public int ypos;

    public int id;

    public Sprite[] tiles;

    private SpriteRenderer sr;
	// Use this for initialization
	void Start ()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = tiles[id];
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ChangeTile()
    {
        sr.sprite = tiles[id];
        Grid.grid.ChangeTile(xpos, ypos, id);
    }
}
