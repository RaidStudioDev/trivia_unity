using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AlertPanel : MonoBehaviour {

	public Text title;
	public Text description;
	public Button closeBtn;

	public bool IsCloseEnabled = false;

	// Use this for initialization
	void Start () 
	{
		if (!IsCloseEnabled) closeBtn.interactable = false;
	}
	
	public void ClosePanel()
	{

	}
}
