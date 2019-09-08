using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class SendingRandomChallengeSubPanel : ScreenController 
{
	public event OnCloseEventHandler OnCloseEvent;

	// panel elements
	// private RectTransform _thisTransform;
	private RectTransform _thisViewport;
	private RectTransform _thisContent;
	// private Button _closeBtn;
	private Text _emailTxt;
	private Button _playBtn;
	private RectTransform _progressPanel;
	private RectTransform _invitePanel;

	private RectTransform _gameIcon;

	// Popup alert box with options
	private ModalPanel _modalPanel;
	private UnityAction _mPlayYesAction;
	private UnityAction _mPlayNoAction;

	private string _iconPath;

	public void Start()
	{
		// base.StartInit();

		// InitAlertBox();
		// Initialize();
	}

	public void Initialize()
	{
		InitAlertBox();

		_myTransform = GetComponent<RectTransform>();
		_thisViewport = _myTransform.Find ("Viewport").GetComponent<RectTransform> ();
		_thisContent = _myTransform.GetComponentInChildren<ChallengePlaySubContent>().GetComponent<RectTransform>();

		UpdateLayout();

		RectTransform loadingPanel = _thisContent.Find ("LoadingPanel").GetComponent<RectTransform>();
		_progressPanel = loadingPanel.Find ("ProgressPanel").GetComponent<RectTransform>();
		_invitePanel = loadingPanel.Find ("InviteSentPanel").GetComponent<RectTransform>();
		_invitePanel.GetComponent<CanvasGroup> ().alpha = 1;
		_invitePanel.gameObject.SetActive (false);

		// if (_closeBtn) _closeBtn.onClick.RemoveAllListeners ();
		// _closeBtn = _thisContent.Find("CloseBtn").GetComponentInChildren<Button>();
		// _closeBtn.onClick.AddListener(() => OnPanelCloseClick());

		if (_playBtn) _playBtn.onClick.RemoveAllListeners ();
		_playBtn = _thisContent.Find("PlayBtn").GetComponentInChildren<Button>();
		_playBtn.onClick.AddListener(() => OnPanelCloseClick());
		_playBtn.gameObject.SetActive (false);

		_emailTxt = _thisContent.Find("EmailText").GetComponentInChildren<Text>();
		_emailTxt.text = "RANDOM USER"; // GameControl.instance.selectedChallengedPlayer.email;

		_gameIcon = _thisContent.Find("GameIcon").GetComponent<RectTransform>();

		// Update Hack!
		UpdateRectransform(_thisViewport);
	}

	private void InitAlertBox()
	{
		_modalPanel = ModalPanel.Instance();

		// Initialize Actions
		_mPlayYesAction = new UnityAction(OnPlayYesAction);
		_mPlayNoAction = new UnityAction(OnPlayNoAction);
	}

	public void Show(GameDataItems gameItem, string srcPath)
	{
		base.StartInit();

		Initialize ();

		string url = srcPath + gameItem.icons [0].normal.Substring (0, gameItem.icons [0].normal.Length - 4);

		_iconPath = url;

		ShowTransition ();
	}

	public void ShowTransition()
	{
		Texture2D inputTexture = Resources.Load<Texture2D>(_iconPath);

		Sprite sprite = Sprite.Create(inputTexture, new Rect(0, 0, 182, 182), Vector2.zero, 100f);
		Button gameIconBtn = _gameIcon.GetComponentInChildren<Button>();
		gameIconBtn.interactable = false;
		Image gameIconImg = gameIconBtn.GetComponent<Image>();
		gameIconImg.sprite = sprite;

		InitTransition();

		StartCoroutine(StartTransition());
	}

	private void InitTransition()
	{
		// LeanTween.scale(_gameIcon, new Vector3(0f, 0f, 0f), 0f);
		// LeanTween.rotateAroundLocal(_gameIcon, Vector3.back, -360f, 0f);
	}

	IEnumerator StartTransition() 
	{
		yield return new WaitForEndOfFrame();

		// LeanTween.scale(_gameIcon, new Vector3(1.2f, 1.2f, 1.2f), 0.85f).setEase(LeanTweenType.easeOutCubic);

		yield return new WaitForSeconds (0.10f);

		LeanTween.alpha(_gameIcon, 1f, 0.95f)
			.setEase(LeanTweenType.easeOutCubic)
			.setDelay(0.1f)
			.setOnComplete(() => {

				GameControl.instance.OnRandomChallengeComplete += OnRandomChallengeComplete;
				GameControl.instance.RandomChallengeToServer();

			});
	}

	void OnRandomChallengeComplete(bool success)
	{
		GameControl.instance.OnSendChallengeComplete -= OnRandomChallengeComplete;

		_stage.HideLoader();

		// _invitePanel.GetComponent<CanvasGroup> ().alpha = 1;
		if (_progressPanel != null) 
		{
			_progressPanel.gameObject.SetActive(false);
			_invitePanel.gameObject.SetActive(true);
			_playBtn.gameObject.SetActive (true);
		}
	}

	private void OnPanelCloseClick()
	{
		GameControl.instance.soundManager.PlayLowToneButton();

		if (OnCloseEvent != null) OnCloseEvent();
	}

	private void OnPlayClick() 
	{
		_modalPanel.Choice("", "Play your game now?", _mPlayYesAction, _mPlayNoAction);
	}

	private void OnPlayYesAction() 
	{
		print ("OnPlayYesAction");
	}

	private void OnPlayNoAction() 
	{
		print ("OnPlayNoAction");
	}

	public void Dispose()
	{
		print ("ChallengePlaySubPanel.Dispose()");

		_modalPanel = null;

		// if (_closeBtn != null) _closeBtn.onClick.RemoveAllListeners();
		_playBtn.onClick.RemoveAllListeners();
	}
}
