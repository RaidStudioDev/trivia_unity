using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PendingChallengeItem : MonoBehaviour 
{
	[HideInInspector] public ChallengeData challengeData;

	public Text date; 
	public Text challengeUser; 
	public Image icon;

	// Use this for initialization
	void Start () {
	
	}
}
