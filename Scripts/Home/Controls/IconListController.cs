using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO; 

public class IconListController : MonoBehaviour {

	public GameObject mainPanel;
	public GameObject vertPanel;

	public Transform itemHorizPanel;
	public Transform itemButton;
	private string imageRootURL = "https://www.lms.mybridgestoneeducation.com/TriviaPHP/images/";

	void Awake ()
	{
		
	}

	// Use this for initialization
	void Start () {

		// Resize Main panel to Stage
		/*RectTransform rtMain = mainPanel.GetComponent<RectTransform> ();
		RectTransform rtVert = vertPanel.GetComponent<RectTransform> ();
		float sidePadding = rtMain.rect.width - rtVert.rect.width;
		rtVert.sizeDelta = new Vector2 (rtVert.rect.width + sidePadding, rtVert.rect.height);*/

		LoadGameData ();
	}

    private void LoadGameData()
    {
        // Path.Combine combines strings into a file path
        // Application.StreamingAssets points to Assets/StreamingAssets in the Editor, and the StreamingAssets folder in a build
        string filePath = Path.Combine("Assets/Scripts/Layouts/Data", "data.json");

        if (File.Exists(filePath))
        {
            // Read the json from the file into a string
			string dataAsJson = "";// File.ReadAllText(filePath);
            // Pass the json to JsonUtility, and tell it to create a GameData object from it
            GameData loadedData = JsonUtility.FromJson<GameData>(dataAsJson);

            //print("DATA: " + loadedData.allRoundData.Length);

            Transform scrollPanelTrans = vertPanel.GetComponent<ScrollRect>().content.GetComponent<Transform>();

            // Transform scrollPanelTrans = GameObject.Find ("ScrollPanel").GetComponent<Transform> ();

            foreach (GameItemData item in loadedData.allRoundData)
            {
                //print("item: " + item.title);
                //print("imagePath: " + item.imagePath);

                // Add New Category
                StartCoroutine(createHorizontalItem(item, scrollPanelTrans));
            }

        }
        else
        {
            Debug.LogError("Cannot load game data!");
        }
    }

    IEnumerator setButtonImage(Button btn, string url) 
	{
		//Debug.Log("Canvas.setButtonImage() url: " + imageRootURL + url);

		Image btnImage = btn.GetComponentInParent<Image> ();

		//Call the WWW class constructor
		WWW imageURLWWW = new WWW(imageRootURL + url);  

		//Wait for the download
		yield return imageURLWWW;        

		//Simple check to see if there's indeed a texture available
		if(imageURLWWW.texture != null) {

			// Debug.Log("TEXTURE IS LOADED");

			//Construct a new Sprite
			Sprite sprite = Sprite.Create(imageURLWWW.texture, new Rect(0, 0, 182, 182), Vector2.zero);  
			btnImage.sprite = sprite;
		}

		yield return null;
	}

	IEnumerator setButtonStates(Button btn, string over, string down) 
	{
		// Debug.Log("Canvas.setButtonStates() over: " + imageRootURL + over);
		// Debug.Log("Canvas.setButtonStates() down: " + imageRootURL + down);

		SpriteState btnSpriteState = btn.spriteState;

		//Call the WWW class constructor
		WWW overUrlWWW = new WWW(imageRootURL + over);  

		//Wait for the download
		yield return overUrlWWW;        

		//Simple check to see if there's indeed a texture available
		if(overUrlWWW.texture != null) {

			// Debug.Log("TEXTURE IS LOADED");

			//Construct a new Sprite
		    Sprite sprite = Sprite.Create(overUrlWWW.texture, new Rect(0, 0, 182, 182), Vector2.zero);  
			btnSpriteState.highlightedSprite = sprite;

			btn.spriteState = btnSpriteState;
		}

		//Call the WWW class constructor
		WWW downUrlWWW = new WWW(imageRootURL + down);  

		//Wait for the download
		yield return downUrlWWW;      

		//Simple check to see if there's indeed a texture available
		if(downUrlWWW.texture != null) {

			// Debug.Log("TEXTURE IS LOADED");

			//Construct a new Sprite
			Sprite sprite = Sprite.Create(downUrlWWW.texture, new Rect(0, 0, 182, 182), Vector2.zero);  
			btnSpriteState.pressedSprite = sprite;

			btn.spriteState = btnSpriteState;
		}

		yield return null;
	}

	IEnumerator createHorizontalItem(GameItemData item, Transform scrollPanelTrans) 
	{
		// Debug.Log("Canvas.createHorizontalItem()");

		Transform copyItem = Instantiate (itemHorizPanel, itemHorizPanel.position, itemHorizPanel.transform.rotation) as Transform;
		copyItem.position =  new Vector3(0, 0, 0);
		copyItem.transform.localPosition = new Vector3 (0, 0, 0);
		copyItem.SetParent (scrollPanelTrans, false);
		print ("copyItem: " + copyItem.position);

		//Wait for the download
		yield return copyItem;

        Transform scrollPanelTr = copyItem.GetComponent<Transform>();
        Transform scrollContent = scrollPanelTr.Find("ScrollPanel").GetComponent<ScrollerExtend>().content.GetComponent<Transform>();

        // Transform scrollPanel = copyItem.GetComponent<ScrollRect>().content.GetComponent<Transform>();
        //Transform scrollPanel = copyItem.GetComponent<ScrollRect>().content.GetComponent<Transform>();
        print ("scrollPanel: " + scrollContent.transform.position);

        RectTransform headerTransform = copyItem.gameObject.transform.Find("Header") as RectTransform;
        RectTransform textTransform = headerTransform.Find ("CategoryLabel") as RectTransform;
		Text title = textTransform.GetComponent<Text>();
		title.text = item.title;

		foreach (GameDataItems gameDataItem in item.items) 
		{
			print ("resource: " + gameDataItem.resource);

			Transform itemButtonTrans = Instantiate (itemButton, itemButton.position, itemButton.transform.rotation) as Transform;
			itemButtonTrans.position =  new Vector3(0, 0, 0);
			itemButtonTrans.transform.localPosition = new Vector3 (0, 0, 0);
			print ("itemButton: " + itemButtonTrans.position);
			itemButtonTrans.SetParent (scrollContent, false);

			Button itemBtn = itemButtonTrans.GetComponent<Button> ();

			// Update Main Image
			StartCoroutine(setButtonImage(itemBtn, item.imagePath + gameDataItem.icons[0].normal));

			// Update Button States
			StartCoroutine(setButtonStates(itemBtn, item.imagePath + gameDataItem.icons[0].over, item.imagePath + gameDataItem.icons[0].down));
		}

		yield return null;
	}

	
}
