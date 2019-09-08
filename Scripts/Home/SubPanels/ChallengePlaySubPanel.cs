using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using HeathenEngineering.OSK.v2;

public delegate void OnSelectedEventHandler(int selectedType);
public delegate void OnCloseEventHandler();

public class ChallengePlaySubPanel : ScreenController 
{
	public event OnSelectedEventHandler OnSelectedEvent;
	public event OnCloseEventHandler OnCloseEvent;

	// selected id types
	public const int SINGLE_PLAYER = 0;
	public const int CHALLENGE_PLAYER = 1;
	public const int RANDOM_PLAYER = 2;

	// panel elements
	// private RectTransform _thisTransform;
	private RectTransform _thisViewport;
	private RectTransform _thisContent;
	private Button _closeBtn;
	private Button _singlePlayerBtn;
	private Button _randomPlayerBtn;
	private Button _challengePlayerBtn;

	private RectTransform _gameIcon;
	// private RectTransform _playText;
	// private RectTransform _chooseText;
	private RectTransform _searchInputPanel;
	private InputField _searchInputField;
	private RectTransform _searchInputButton;
	private Text _searchInputButtonText;
	private RectTransform _blackPanel;
	private RectTransform _searchContent;
	private string _inputMessage = "Search your friends";

	// Popup alert box with options
	private ModalPanel _modalPanel;
	private UnityAction _mRandomChallengeYesAction;
	private UnityAction _mChallengeYesAction;
	private UnityAction _mUserChallengeYesAction;
	private UnityAction _mNoAction;

	private Transform _searchListItem;
	private int _selectedUserIndex = -1;

	private string _dataSearchUrl = "SearchUsers.php?s=";
	private List<Transform> _userList = new List<Transform>();
	private PlayerData[] _searchUserList;
	private string _iconPath;

	private bool _isKeyboardUp = false;
	private KeyboardController _keyboardControl;

	void Start()
	{
		// Initialize();
	}

	public void Initialize()
	{
		InitAlertBox();

		if (!_searchListItem) _searchListItem = Instantiate(Resources.Load("HomeScreen/SearchListItemPanel", typeof(Transform))) as Transform;

		_myTransform = GetComponent<RectTransform>();
		_thisViewport = _myTransform.Find ("Viewport").GetComponent<RectTransform> ();
		_thisContent = _myTransform.GetComponentInChildren<ChallengePlaySubContent>().GetComponent<RectTransform>();

		UpdateLayout();

		if (_closeBtn) _closeBtn.onClick.RemoveAllListeners ();
		_closeBtn = _thisContent.Find("CloseBtn").GetComponentInChildren<Button>();
		_closeBtn.onClick.AddListener(() => OnPanelCloseClick());

		if (_singlePlayerBtn) _singlePlayerBtn.onClick.RemoveAllListeners ();
		_singlePlayerBtn = _thisContent.Find("SinglePlayerBtn").GetComponentInChildren<Button>();
		_singlePlayerBtn.onClick.AddListener(() => OnSinglePlayerClick());

		if (_randomPlayerBtn) _randomPlayerBtn.onClick.RemoveAllListeners ();
		_randomPlayerBtn = _thisContent.Find("RandomOponentBtn").GetComponentInChildren<Button>();
		_randomPlayerBtn.onClick.AddListener(() => OnRandomPlayerClick());

		_gameIcon = _thisContent.Find("GameIcon").GetComponent<RectTransform>();
		//_playText = _thisContent.Find("PlayText").GetComponent<RectTransform>();
		// _chooseText = _thisContent.Find("ChooseOponentText").GetComponent<RectTransform>();

		_searchInputPanel = _thisContent.Find("SearchForFriendsInput").GetComponent<RectTransform>();
		_blackPanel = _myTransform.Find("BlackPanel").GetComponent<RectTransform>();

		if (GameControl.instance.isMobile) 
		{
			_keyboardControl = KeyboardController.Instance();

			_searchInputPanel.Find ("InputButton").gameObject.SetActive (true);
			_searchInputPanel.Find ("InputField").gameObject.SetActive (false);

			_searchInputButton = _searchInputPanel.Find("InputButton").GetComponent<RectTransform>();
			_searchInputButton.GetComponent<Button>().onClick.AddListener(() => OnSearchButtonClick());
			_searchInputButtonText = _searchInputButton.GetComponentInChildren<Text>();
			_searchInputButtonText.text = _inputMessage;
			_keyboardControl.outputText = _searchInputButtonText;

			_blackPanel.GetComponent<Button>().onClick.AddListener(() => OnBlackPanelClick());
		} 
		else 
		{
			_searchInputPanel.Find ("InputButton").gameObject.SetActive (false);
			_searchInputPanel.Find ("InputField").gameObject.SetActive (true);

			_searchInputField = _searchInputPanel.GetComponentInChildren<InputField>();
			_searchInputField.text = "";

			_searchInputField.onEndEdit.AddListener (delegate {
				OnSearchInputEnd(_searchInputField);
			});
			_searchInputField.onValueChanged.AddListener (delegate {
				OnSearchInputChanged(_searchInputField);
			});
		}

		if (!_searchContent) _searchContent = _thisContent.Find("SearchList").GetComponent<RectTransform>().GetComponentInChildren<SearchLayoutContent>().GetComponent<RectTransform>();
	
		// Update Hack!
		UpdateRectransform(_thisViewport);
	}

	private void OnBlackPanelClick()
	{
		_blackPanel.GetComponent<Button>().interactable = false;

		OnSearchButtonClick ();
	}


	private void OnSearchButtonClick()
	{
		_searchInputButton.GetComponent<Button> ().interactable = false;

		SlideKeyboard ();
	}

	private void SlideKeyboard()
	{
		float slideAmount = 400f;

		if (!_isKeyboardUp) 
		{
			_isKeyboardUp = true;

			_keyboardControl.gameObject.SetActive (true);

			_keyboardControl.keyboard.gameObject.SetActive (true);
			_keyboardControl.keyboard.KeyPressed += new KeyboardEventHandler(OnKeyboardKeyPressed);

			_keyboardControl.keyboard.transform.SetAsLastSibling ();

			Vector3 upSlideTo = _thisViewport.anchoredPosition3D + Vector3.up * slideAmount;

			LeanTween.move(_thisViewport, upSlideTo, 0.75f)
				.setEase(LeanTweenType.easeOutCubic)
				.setDelay(0.15f)
				.setOnComplete(() => {

					_searchInputButton.GetComponent<Button> ().interactable = true;
					_blackPanel.GetComponent<Button>().interactable = true;
				});
		} 
		else 
		{
			_blackPanel.GetComponent<Button>().interactable = false;

			_isKeyboardUp = false;
			_keyboardControl.gameObject.SetActive (false);
			_keyboardControl.keyboard.gameObject.SetActive (false);
			_keyboardControl.keyboard.KeyPressed -= OnKeyboardKeyPressed;

			Vector3 downSlideTo = _thisViewport.anchoredPosition3D + Vector3.down * slideAmount;
			LeanTween.move(_thisViewport, downSlideTo, 0.75f)
				.setEase(LeanTweenType.easeOutCubic)
				.setDelay(0.15f)
				.setOnComplete(() => {
					_searchInputButton.GetComponent<Button> ().interactable = true;
				});
		}
	}

	private void InitAlertBox()
	{
		_modalPanel = ModalPanel.Instance();

		// Initialize Actions
		_mRandomChallengeYesAction = new UnityAction(OnRandomChallengeYesAction);
		_mChallengeYesAction = new UnityAction(OnChallengePlayerYesAction);
		_mNoAction = new UnityAction(OnNoAction);
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
		LeanTween.alpha(_blackPanel, 0f, 0f);
		LeanTween.scale(_gameIcon, new Vector3(0f, 0f, 0f), 0f);
		LeanTween.rotateAroundLocal(_gameIcon, Vector3.back, -360f, 0f);
	}

	IEnumerator StartTransition() 
	{
		yield return new WaitForEndOfFrame();

		LeanTween.alpha(_blackPanel, 0.75f, 0.5f).setEase(LeanTweenType.easeOutQuad);

		LeanTween.rotateAroundLocal(_gameIcon, Vector3.back, 360f, 0.95f).setEase(LeanTweenType.easeOutSine);

		yield return new WaitForSeconds (0.16f);

		LeanTween.scale(_gameIcon, new Vector3(1f, 1f, 1f), 0.85f).setEase(LeanTweenType.easeOutBack);

		yield return new WaitForSeconds (0.20f);

		LeanTween.alpha(_gameIcon, 1f, 0.75f)
	        .setEase(LeanTweenType.easeInOutCubic)
	        .setDelay(0.75f)
			.setOnComplete(() => {



			});
	}

	private void OnPanelCloseClick()
	{
		GameControl.instance.soundManager.PlayLowToneButton();

		OnCloseEvent();
	}

	private void OnSinglePlayerClick() 
	{
		if (_isKeyboardUp) { SlideKeyboard(); return; }

		OnSelectedEvent(SINGLE_PLAYER);
	}

	private void OnRandomPlayerClick() 
	{
		GameControl.instance.soundManager.PlayLowToneButton();

		if (_isKeyboardUp) { SlideKeyboard(); return; }

		_modalPanel.Choice("Challenging a", "RANDOM USER", _mRandomChallengeYesAction, _mNoAction);
	}

	private void OnRandomChallengeYesAction() 
	{
		GameControl.instance.soundManager.PlayLowToneButton();

		OnSelectedEvent(RANDOM_PLAYER);
	}

	private void OnChallengePlayerYesAction() 
	{
		GameControl.instance.soundManager.PlayLowToneButton();

		GameControl.instance.selectedChallengedPlayer = _searchUserList[_selectedUserIndex];

		// print ("OnChallengePlayerYesAction");

		OnSelectedEvent(CHALLENGE_PLAYER);
	}

	private void OnNoAction() 
	{
		// print ("OnModalChallengeNoClick");
		GameControl.instance.soundManager.PlayLowToneButton();
	}

	private void OnSearchInputEnd(InputField input) 
	{
		input.DeactivateInputField();
	}

	private void OnSearchInputChanged(InputField input)
	{
		Search(input.text);
	}
		
	private bool _isSearching = false;
	private void Search(string keyword)
	{
		if (_isSearching) return;

		RemoveSearchListItems();

		if (keyword.Length == 0) 
		{
			_disableKeyboard = false;
			CreateSearchItem(-1);
			return;
		}

		// Debug.Log("Search: " + (GameControl.instance.RootURL + _dataSearchUrl + keyword));

		WWW GetSearchAttempt = new WWW(GameControl.instance.RootURL + _dataSearchUrl + keyword + "&e=" + GameControl.instance.email);
		StartCoroutine(GetSearchAttemptWaitForRequest(GetSearchAttempt));
	}

	IEnumerator GetSearchAttemptWaitForRequest(WWW www)
	{
		_isSearching = true;

		yield return www;

		// check for errors
		if (www.error == null)
		{
			// Debug.Log("GetSearchAttemptWaitForRequest Ok!: " + www.text);

			_searchUserList = JsonHelper.FromJsonWrapped<PlayerData>(www.text);

			if (_searchUserList.Length > 0) 
			{
				int itemsLen = _searchUserList.Length;

				for (int i = 0; i < itemsLen; i++) CreateSearchItem(i); 
			} 
			else // No Users Found
			{
				RemoveSearchListItems();

				CreateSearchItem(-1);
			}

			_disableKeyboard = false;
			_isSearching = false;
		} 
		else Debug.Log("WWW Error: "+ www.error);
	}

	private void RemoveSearchListItems()
	{
		if (_userList.Count > 0) 
		{
			for (int i = 0; i < _userList.Count; i++) 
			{
				_userList[i].GetComponentInChildren<Button>().onClick.RemoveAllListeners();
				Destroy(_userList[i].gameObject);
			}

			_searchUserList = null;
			_userList.Clear();
		}
	}
		
	void CreateSearchItem(int index)
	{
		if (index == -1) return;
		if (_searchListItem == null) return;

		int userIndex = (index != -1) ? index : -1;
		string email = (index != -1) ? _searchUserList[index].email : "No users found.";

		Transform copyItem = Instantiate(_searchListItem, _searchListItem.position, _searchListItem.transform.rotation) as Transform;
		copyItem.position = new Vector3(0, 0, 0);
		copyItem.transform.localPosition = new Vector3(0, 0, 0);
		copyItem.SetParent(_searchContent, false); 
		copyItem.GetComponentInChildren<Text>().text = (index != -1) ? email.ToLower() : email;

		// If no users, dont add listener
		if (index != -1)
			copyItem.GetComponentInChildren<Button> ().onClick.AddListener (() => OnSearchItemClick (userIndex));
		else
			copyItem.GetComponentInChildren<Button> ().interactable = false;

		_userList.Add(copyItem);
	}

	void OnSearchItemClick(int userIndex)
	{
		GameControl.instance.soundManager.PlayLowToneButton();

		if (GameControl.instance.isMobile) SlideKeyboard();

		_selectedUserIndex = userIndex;

		// print ("fullname: " + _searchUserList[_selectedUserIndex].fullname.ToUpper());

		string displayName = GameControl.instance.FormatUserNameDisplay (_searchUserList[_selectedUserIndex].fullname);

		_modalPanel.Choice("Challenging", displayName.ToUpper(), _mChallengeYesAction, _mNoAction);
	}

	public void Dispose()
	{
		print ("ChallengePlaySubPanel.Dispose()");

		RemoveSearchListItems();

		DestroyImmediate (_searchListItem.gameObject);

		// Remove Events
		_blackPanel.GetComponent<Button>().onClick.RemoveAllListeners();
		_closeBtn.onClick.RemoveAllListeners();
		_randomPlayerBtn.onClick.RemoveAllListeners();
		_singlePlayerBtn.onClick.RemoveAllListeners();

		if (GameControl.instance.isMobile) 
		{
			if (_searchInputButtonText.text == _inputMessage) _searchInputButtonText.text = ""; 

			_searchInputButton.GetComponent<Button> ().onClick.RemoveAllListeners ();
		}
		else 
		{
			_searchInputField.text = "";
			_searchInputField.onEndEdit.RemoveAllListeners ();
			_searchInputField.onValueChanged.RemoveAllListeners ();
		}

		InitTransition();
	}


	// KEYBOARD
	private bool _disableKeyboard = false;
	void OnKeyboardKeyPressed(OnScreenKeyboard sender, OnScreenKeyboardArguments args)
	{
		// print ("OnKeyboardKeyPressed.type: " + args.KeyPressed.type);

		if (_disableKeyboard) return;

		if (GameControl.instance.isMobile && _searchInputButtonText.text == _inputMessage) 
		{
			_searchInputButtonText.text = "";
		}

		switch (args.KeyPressed.type) 
		{
		case KeyClass.Backspace:
			if (_searchInputButtonText.text.Length > 0) 
			{
				_disableKeyboard = true;

				_searchInputButtonText.text = _searchInputButtonText.text.Substring(0, _searchInputButtonText.text.Length -1);

				Search (_searchInputButtonText.text);
			}
			break;
		case KeyClass.Return:
			OnSearchButtonClick();
			break;
		case KeyClass.Shift:
			break;
		case KeyClass.String:
			_searchInputButtonText.text += args.KeyPressed.ToString();

			Search (_searchInputButtonText.text);	
			break;
		}
	}
}
