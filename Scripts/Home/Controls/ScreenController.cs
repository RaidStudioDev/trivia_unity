using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenController : MonoBehaviour {

	protected CanvasStage _stage;
	protected ChallengeManager _challengeManager;
	protected GameControl _gameControl;
	protected LoadScene _loadScene;
	protected RectTransform _myTransform;

	// Use this for initialization
	void Awake () 
	{ 
		//print ("ScreenController.Awake()");

		StartInit ();
	}

	public void StartInit()
	{
		_myTransform = GetComponent<RectTransform>();
		_stage = GameObject.FindObjectOfType<CanvasStage>();
		_gameControl = GameControl.instance;
		_challengeManager = GameObject.FindObjectOfType<ChallengeManager>();
		_loadScene = GameObject.FindObjectOfType<LoadScene>();
	}

	// RESIZE FUNCTIONS  ****************************************************************************

	void OnRectTransformDimensionsChange() 
	{
		//print ("ScreenController.OnRectTransformDimensionsChange()");

		UpdateLayout ();
	}

	protected void UpdateLayout()
	{
		if (!_myTransform) return;

        Debug.Log ("************************Screen.width: " + Screen.width);


        /*bool isPhoneDetected = false;
		if (!Application.isEditor) {

			#if UNITY_WEBGL_API
			isPhoneDetected = GameControl.IsPhoneDetected();
			#endif
		}

		
		if (Screen.width >= 768 && !isPhoneDetected) 
		{
			//Debug.Log ("************************UpdateLayout");

			// Centered / Stretch Vertically
			_myTransform.anchorMin = new Vector2(0.5f, 0f);
			_myTransform.anchorMax = new Vector2(0.5f, 1f);
			_myTransform.pivot = new Vector2(0.5f, 0.5f);
			_myTransform.sizeDelta = new Vector2 (750, 0);
		} 
		else 
		{
			if (isPhoneDetected)
			{
				// Stretch Horizontally & Vertically
				_myTransform.anchorMin = new Vector2(0f, 0f);
				_myTransform.anchorMax = new Vector2(1f, 1f);
				_myTransform.pivot = new Vector2(0.5f, 0.5f);
				_myTransform.sizeDelta = new Vector2 (0, 0);
			}
			else 
			{
				if (_myTransform.sizeDelta.x != Screen.width && !Application.isEditor) 
				{
					_myTransform.sizeDelta = new Vector2 (Screen.width, 0);	
				}
			}
		}*/
	}

	// Update Hack!  How to force UI Layout to fix itself
	// Without this call, some panels would not refresh on Update
	protected void UpdateRectransform(RectTransform rect)
	{
		//if (GameControl.instance.isMobile) 
		//{
			//rect.sizeDelta = new Vector2 (750, 0);
			//rect.sizeDelta = new Vector2 (0, 0);
		//}

		UpdateLayout ();
	}

	void Destroy() 
	{
		_stage = null;
		_challengeManager = null;
		_loadScene = null;
		_gameControl = null;
	}
}
