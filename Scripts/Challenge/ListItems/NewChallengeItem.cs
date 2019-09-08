using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NewChallengeItem : MonoBehaviour 
{
	[HideInInspector] public ChallengeData challengeData;

	public Text date; 
	public Text username; 
	public Text message; 
	public Image icon;
	public Button playBtn;
	public Button ignoreBtn;


	// Use this for initialization
	void Start () {
	
	}
}
