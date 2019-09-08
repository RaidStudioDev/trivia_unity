using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageQuestion : MonoBehaviour {

	public Image theImage;
	public Text theText;
	Color theColor;

	// Use this for initialization
	void Start () 
	{
		theImage = this.GetComponent<UnityEngine.UI.Image>();
		theColor = theImage.color;
		theColor.r = 255;
		theColor.g = 255;
		theColor.b = 255;
		theColor.a = 1f;

		theImage.color = theColor;

		theText = this.GetComponentInChildren<Text>();
		theText.CrossFadeAlpha (0f, 0f, false);

		crossFadeAlpha();
	}

	public void crossFadeAlpha(float alpha = 0f, float duration = 0f)
	{
		theImage.CrossFadeAlpha(alpha, duration, false);
	}
}
