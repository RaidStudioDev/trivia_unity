using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopUpPanel : MonoBehaviour {

	public Image thePopUpImage;
	private Color theColor;

	// Use this for initialization
	void Start () 
	{
		
	}

	public void InitPopUp()
	{
		if (!GameControl.instance.isPopUpComplete) LoadPopUp(); else DestroyPopUp();
	}

	public void LoadPopUp()
	{
		this.gameObject.SetActive(true);

		GameControl.instance.isPopUpComplete = true;

		theColor = thePopUpImage.color;
		theColor.r = 255;
		theColor.g = 255;
		theColor.b = 255;
		theColor.a = 1f;

		thePopUpImage.color = theColor;

		thePopUpImage.CrossFadeAlpha(0f, 0f, false);

		StartCoroutine(setImage("popup/popup_tire_sizes.jpg"));
	}

	IEnumerator setImage(string url) 
	{
		//Debug.Log("PopUpPanel.setImage() url: " + GameControl.instance.ImageRootURL + url);

		//Call the WWW class constructor
		WWW imageURLWWW = new WWW(GameControl.instance.ImageRootURL + url);  

		//Wait for the download
		yield return imageURLWWW;        

		//Simple check to see if there's indeed a texture available
		if(imageURLWWW.texture != null) {

			//Debug.Log("POPUP TEXTURE IS LOADED");

			Sprite sprite = Sprite.Create(imageURLWWW.texture, new Rect(0, 0, 460, 777), Vector2.zero);  

			thePopUpImage.sprite = sprite;  

			thePopUpImage.CrossFadeAlpha(1f, 0.95f, false);
		}

		yield return null;
	}

	public void DestroyPopUp()
	{
		thePopUpImage.sprite = null;

		Destroy (this.gameObject);
	}
}
