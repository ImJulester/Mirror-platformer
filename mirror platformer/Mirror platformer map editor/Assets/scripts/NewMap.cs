using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class NewMap : MonoBehaviour {

    public Grid grid;
    public InputField width;
    public InputField height;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void CreateNewMap()
    {
        grid.CreateMap(int.Parse(width.text), int.Parse(height.text));
    }
}
