using UnityEngine;
using System.Collections;

public class CameraColor : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		Camera cam = GetComponent<Camera>();
		cam.backgroundColor = Color.red;
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
