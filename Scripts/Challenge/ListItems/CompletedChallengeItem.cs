using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CompletedChallengeItem : MonoBehaviour 
{
	[HideInInspector] public ChallengeData challengeData;

	public Text challengeUser; 
	public Text challengeScore; 
	public Text userName; 
	public Text userScore;
	public Image icon;
	public Button playBtn;

	// Use this for initialization
	void Start () {
	
	}
}
