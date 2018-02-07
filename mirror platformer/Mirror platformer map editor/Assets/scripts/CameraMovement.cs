using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {


    public float xbound;
    public float Xbound
    {
        get { return xbound; }
        set { xbound = value; }
    }
    public float ybound;
    public float Ybound
    {
        get { return ybound; }
        set { ybound = value; }
    }


    public float speed;

    private Vector3 pos;
    private Vector3 mousepos;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (transform.position.x >= -xbound && transform.position.x <= xbound && transform.position.y >= -ybound && transform.position.y <= ybound)
        {
            if (Input.GetMouseButtonDown(2))
            {
                pos = transform.position;
                mousepos = Input.mousePosition;
                Debug.Log("pushed mouse wheel");
            }
            if (Input.GetMouseButton(2))
            {
                transform.position = pos + (mousepos - Input.mousePosition);
            }

            if (transform.position.x < -xbound)
            {
                transform.position = new Vector3(-xbound, transform.position.y, transform.position.z);
            }
            if (transform.position.x > xbound)
            {
                transform.position = new Vector3(xbound, transform.position.y, transform.position.z);
            }

            if (transform.position.y < -ybound)
            {
                transform.position = new Vector3(transform.position.x, -ybound, transform.position.z);
            }
            if (transform.position.y > ybound)
            {
                transform.position = new Vector3(transform.position.x, ybound, transform.position.z);
            }

        }



        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            Camera.main.orthographicSize -= speed;
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            Camera.main.orthographicSize += speed;
        }
    }
}
