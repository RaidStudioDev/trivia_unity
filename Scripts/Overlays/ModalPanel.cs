using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class ModalPanel : MonoBehaviour {

	protected RectTransform _myTransform;

    public Text title;
	public Text message;
    public Button yesButton;
    public Button noButton;
    public GameObject modalPanelObject;
    
    private static ModalPanel modalPanel;
    
    public static ModalPanel Instance () {
        if (!modalPanel) {
            modalPanel = FindObjectOfType(typeof (ModalPanel)) as ModalPanel;
            if (!modalPanel)
                Debug.LogError ("There needs to be one active ModalPanel script on a GameObject in your scene.");
        }
        
        return modalPanel;
    }

    // Yes/No/Cancel: A string, a Yes event, a No event and Cancel event
	public void Choice (string title, string question, UnityAction yesEvent, UnityAction noEvent) {
        modalPanelObject.SetActive (true);
        
		//_myTransform = GetComponent<RectTransform>();

		UpdateLayout();

        yesButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener (yesEvent);
        yesButton.onClick.AddListener (ClosePanel);
        
        noButton.onClick.RemoveAllListeners();
        noButton.onClick.AddListener (noEvent);
        noButton.onClick.AddListener (ClosePanel);
        
        this.title.text = title;
		this.message.text = question;

        yesButton.gameObject.SetActive (true);
        noButton.gameObject.SetActive (true);
    }

    void ClosePanel () {
        modalPanelObject.SetActive (false);
    }

	public void UpdateLayout()
	{
		_myTransform = modalPanelObject.GetComponent<RectTransform> ();

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

		// UpdateRectransform (_myTransform);

		// LayoutRebuilder.ForceRebuildLayoutImmediate(_myTransform);
	}

	// Update Hack!  How to force UI Layout to fix itself
	// Without this call, some panels would not refresh on Update
	public void UpdateRectransform(RectTransform rect)
	{
		//if (GameControl.instance.isMobile) 
		//{
		rect.sizeDelta = new Vector2 (750, 0);
		rect.sizeDelta = new Vector2 (0, 0);
		//}

		UpdateLayout ();
	}
}