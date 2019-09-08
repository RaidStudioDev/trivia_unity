using UnityEngine;
using System.Collections;

public class OverlayLayoutController : MonoBehaviour {

	protected RectTransform _myTransform;

	// Use this for initialization
	void Start () {
	
		_myTransform = GetComponent<RectTransform> ();

		//print ("OverlayLayoutController->Start");

		UpdateLayout ();
	}

	void OnEnable()
	{
		//print ("OverlayLayoutController->OnEnable");
	}
	
	public void UpdateLayout()
	{
		//print ("OverlayLayoutController->UpdateLayout");
		// _myTransform = modalPanelObject.GetComponent<RectTransform> ();

		if (!_myTransform) return;

		bool isPhoneDetected = false;
		if (!Application.isEditor) {

			#if UNITY_WEBGL_API
			isPhoneDetected = GameControl.IsPhoneDetected();
			#endif
		}

		if (Screen.width >= 768 && !isPhoneDetected) 
		{
			// Centered / Stretch Vertically
			_myTransform.anchorMin = new Vector2(0.5f, 0f);
			_myTransform.anchorMax = new Vector2(0.5f, 1f);
			_myTransform.pivot = new Vector2(0.5f, 0.5f);
			_myTransform.sizeDelta = new Vector2 (750f, 0f);
		} 
		else 
		{
			if (isPhoneDetected)
			{
				// Stretch Horizontally & Vertically
				_myTransform.anchorMin = new Vector2(0f, 0f);
				_myTransform.anchorMax = new Vector2(1f, 1f);
				_myTransform.pivot = new Vector2(0.5f, 0.5f);
				_myTransform.sizeDelta = new Vector2 (0f, 0f);
			}
			else 
			{
				if (_myTransform.sizeDelta.x != Screen.width && !Application.isEditor) 
				{
					_myTransform.sizeDelta = new Vector2 (Screen.width, 0);	
				}
			}
		}
	}
}
