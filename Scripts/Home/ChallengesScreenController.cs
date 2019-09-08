using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.IO; 

public class ChallengesScreenController : ScreenController {

	private GameData _loadedData;
	private GameObject _itemPool;

	private string homeScreenIconPath = "HomeScreen/";

	private ChallengePlaySubPanel _challengeSubPanel;
	private SendingChallengeSubPanel _sendingChallengeSubPanel;
	private BlackPanel _blackPanel;

	// Vertical Scroller Panel - Contains our category items
	private Transform _verticalScrollPanel;
	private Transform _verticalScrollPanelContent;

	private ModalPanel _modalPanel;
	private UnityAction _mChallengeYesAction;
	private UnityAction _mChallengeNoAction;

	private UnityAction _mAcceptChallengeYesAction;
	private UnityAction _mIngoreChallengeYesAction;
	private UnityAction _mCurrentPlayChallengeYesAction;
	private UnityAction _mRematchChallengeYesAction;
	private UnityAction _mClearChallengesYesAction;
	private UnityAction _mChallengeAcceptedPlayNowYesAction;

	private void InitAlertBox()
	{
		_modalPanel = ModalPanel.Instance();

		Transform modalPanel = Instantiate(Resources.Load("HomeScreen/ModalYesNoPanel", typeof(Transform))) as Transform;
		modalPanel.position =  new Vector3(0, 0, 0);
		modalPanel.transform.localPosition = new Vector3 (0, 0, 0);
		modalPanel.gameObject.SetActive (false);
		modalPanel.SetParent(_stage.transform, false); 

		GameObject panel = modalPanel.transform.Find("Panel").gameObject;
		_modalPanel.title = panel.transform.Find("TitleText").GetComponent<Text>();
		_modalPanel.message = panel.transform.Find("MessageText").GetComponent<Text>();
		GameObject buttonsPanel = panel.transform.Find("ButtonsPanel").gameObject;
		_modalPanel.yesButton = buttonsPanel.transform.Find("YesBtn").GetComponent<Button>();
		_modalPanel.noButton = buttonsPanel.transform.Find("NoBtn").GetComponent<Button>();
		_modalPanel.modalPanelObject = modalPanel.gameObject;

		// Initialize Actions
		_mAcceptChallengeYesAction = new UnityAction(OnAcceptChallengeYesAction);
		_mIngoreChallengeYesAction = new UnityAction(OnIgnoreChallengeYesAction);
		_mCurrentPlayChallengeYesAction = new UnityAction(OnCurrentPlayChallengeYesAction);
		_mRematchChallengeYesAction = new UnityAction(OnRematchChallengeYesAction);
		_mClearChallengesYesAction = new UnityAction(OnClearChallengesYesAction);
		_mChallengeAcceptedPlayNowYesAction = new UnityAction(OnChallengeAcceptedPlayNowYesAction);
		_mChallengeNoAction = new UnityAction(OnChallengeNoAction);
	}

	void Destroy()
	{
		// print ("Destorying ChallengesScreen");

		_loadedData = null;
	}

	void Start()
	{
		base.StartInit();

		Initialize();

		InitAlertBox();

		UpdateLayout();

		StartCoroutine(StartGameCategoryDataLoad());
	}

	void Initialize()
	{
		// create a simple item pool via gameObject, we will store some items here and reuse if available
		_itemPool = new GameObject();
		_itemPool.name = "ItemPool";

		// init main vert scroller
		_verticalScrollPanel = this.transform.Find("VerticalScrollPanel").GetComponent<Transform>();
		_verticalScrollPanelContent = _verticalScrollPanel.GetComponent<ScrollRect>().content.GetComponent<Transform>();

		// Disable Temp Items on Scroll Content
		GameControl.instance.SetActiveAllChildren (_verticalScrollPanelContent, false);
	}

	private WaitForEndOfFrame _waitForFrame = new WaitForEndOfFrame();
	private WaitForSeconds _waitCheckChallenge = new WaitForSeconds(1.15f);
	// private WaitForSeconds _waitInitCatItems = new WaitForSeconds(0.2f);
	IEnumerator StartGameCategoryDataLoad()
	{
		yield return _waitForFrame;

		LoadGameData();

		LoadGameChallengesData();
	}

	void LoadGameData()
	{
		TextAsset jsonDataFile = Resources.Load<TextAsset>("HomeScreen/homeScreenItems");

		_loadedData = JsonUtility.FromJson<GameData>(jsonDataFile.text);
	}

	string GetIconFromGameDataByResourceID(string resourceId)
	{
		string icon = "";
		string dataResId = "";

		for (int i = 0; i < _loadedData.allRoundData.Length; i++) 
		{
			for (int j = 0; j < _loadedData.allRoundData[i].items.Length; j++) 
			{
				dataResId = _loadedData.allRoundData[i].items[j].resource;
				if (dataResId.IndexOf (".csv") != -1) dataResId = dataResId.Substring (0, dataResId.Length - 4); 
			
				if (resourceId == dataResId) 
				{
					icon = _loadedData.allRoundData[i].imagePath + _loadedData.allRoundData[i].items[j].icons [0].normal;

					break;
				}
			}
		}

		return icon;
	}

	void LoadGameChallengesData()
	{
		StartCoroutine (GetChallengesFromServer());
	}

	IEnumerator GetChallengesFromServer()
	{
		StartCoroutine (ShowBlackLoaderPanel("Updating Challenge..."));

		yield return _waitCheckChallenge;

		GameControl.instance.OnGetChallengesComplete += OnGetChallengesComplete;
		GameControl.instance.GetChallengesFromServer();
	}

	IEnumerator ResetContent()
	{
		// Clear Challenge Info
		GameControl.instance.ClearChallengeInfo();

		yield return _waitForFrame;

		DisposePanels();

		// yield return _waitCheckChallenge;

		LoadGameChallengesData();
	}

	void DisposePanels() 
	{
		// Debug.Log ("DisposePanels");

		// clear pending items
		int itemsLen = _challengePendingItemList.Count;
		for (int i = 0; i < itemsLen; i++) 
		{
			DestroyImmediate(_challengePendingItemList[i].gameObject);
		}

		_challengePendingItemList.Clear();

		// clear new items
		itemsLen = _challengeCurrentNewItemList.Count;
		for (int i = 0; i < itemsLen; i++) 
		{
			DestroyImmediate(_challengeCurrentNewItemList[i].gameObject);
		}

		_challengeCurrentNewItemList.Clear();

		// clear active games
		itemsLen = _challengeCurrentActiveItemList.Count;
		for (int i = 0; i < itemsLen; i++) 
		{
			DestroyImmediate(_challengeCurrentActiveItemList[i].gameObject);
		}

		_challengeCurrentActiveItemList.Clear();

		// clear completed games
		itemsLen = _challengeCompletedItemList.Count;
		for (int i = 0; i < itemsLen; i++) 
		{
			DestroyImmediate(_challengeCompletedItemList[i].gameObject);
		}

		_challengeCompletedItemList.Clear();

		// clear challenge data items
		_challengePendingList.Clear();
		_challengeCurrentNewList.Clear();
		_challengeCurrentActiveList.Clear();
		_challengeCompletedList.Clear();

		DestroyImmediate( _pendingChallengeTitle.gameObject );
		if (_newChallengeTitle != null) DestroyImmediate( _newChallengeTitle.gameObject );
		if (_activeChallengeTitle != null) DestroyImmediate( _activeChallengeTitle.gameObject );
		if (_completedChallengeTitle != null) DestroyImmediate( _completedChallengeTitle.gameObject );
	}

	private Transform _pendingChallengeTitle;
	private Transform _newChallengeTitle;
	private Transform _activeChallengeTitle;
	private Transform _completedChallengeTitle;

	private Transform _noChallengesItem;

	private List<ChallengeData> _challengePendingList;
	private List<ChallengeData> _challengeCurrentNewList;
	private List<ChallengeData> _challengeCurrentActiveList;
	private List<ChallengeData> _challengeCompletedList;

	private List<PendingChallengeItem> _challengePendingItemList;
	private List<NewChallengeItem> _challengeCurrentNewItemList;
	private List<CurrentChallengeItem> _challengeCurrentActiveItemList;
	private List<CompletedChallengeItem> _challengeCompletedItemList;
	void OnGetChallengesComplete(bool success, ChallengeData[] info)
	{
		GameControl.instance.OnGetChallengesComplete -= OnGetChallengesComplete;

		// Debug.Log("OnGetChallengesComplete info.Length: " + info.Length);

		// Parse Challenges - we need to seperate the challenge list
		// into 3 parts - 
		// 1. Pending Challenges - are challenges you made but are not accepted by challenged user
		// 2. Current Challenges - are challenges you were invited by another user
		//		a. Invited Challenges - are challenges you were invited by another user, you can accept or ignore
		//		b. Active Challenges - challenges already accepted and active
		// 3. Completed Challenges - challenges that both parties have played

		_challengePendingList = new List<ChallengeData>();
		_challengeCurrentNewList = new List<ChallengeData>();
		_challengeCurrentActiveList = new List<ChallengeData>();
		_challengeCompletedList = new List<ChallengeData>();

		_challengePendingItemList = new List<PendingChallengeItem>();
		_challengeCurrentNewItemList = new List<NewChallengeItem>();
		_challengeCurrentActiveItemList = new List<CurrentChallengeItem>();
		_challengeCompletedItemList = new List<CompletedChallengeItem>();

		int itemsLen = info.Length;
		string userEmail = GameControl.instance.email;
		string displayName = "";

		for (int i = 0; i < itemsLen; i++) 
		{
			// Completed Game
			if (info[i].usercompleted == 1 && info[i].friendcompleted == 1) 
			{
				_challengeCompletedList.Add(info[i]);
			} 
			else 
			{
				// Pending Challenges this user has sent
				if (info[i].useremail == userEmail && info[i].friendaccepted == 0) 
				{
					_challengePendingList.Add(info[i]);
				}

				// New Invited Challenges from a user, and this user has not accepted yet
				if (info[i].friendemail == userEmail && info[i].friendaccepted == 0 && info[i].friendignored == 0) 
				{
					_challengeCurrentNewList.Add(info[i]);
				}

				// Current Game from a friend challenge OR Current Game challenged by user
				if ((info[i].friendemail == userEmail && info[i].friendaccepted == 1) 
					|| (info[i].useremail == userEmail && info[i].friendaccepted == 1))
				{
					_challengeCurrentActiveList.Add(info[i]);
				} 
			}
		}

		// Now create the item panels for each List

		// Add Pending Challenges Title Panel
		if (_pendingChallengeTitle == null) 
		{
			_pendingChallengeTitle = Instantiate(Resources.Load("ChallengeScreen/PendingChallengesTitlePanel", typeof(Transform))) as Transform;
			_pendingChallengeTitle.position =  new Vector3(0, 0, 0);
			_pendingChallengeTitle.transform.localPosition = new Vector3 (0, 0, 0);
			_pendingChallengeTitle.SetParent(_verticalScrollPanelContent, false); 
			_pendingChallengeTitle.GetComponentInChildren<Text>().text = "YOUR PENDING CHALLENGES (" + _challengePendingList.Count.ToString() + ")";
			_pendingChallengeTitle.GetComponentInChildren<Button>().onClick.AddListener(() => OnPendingChallengeClick());
			_pendingChallengeTitle.GetComponentInChildren<Button> ().interactable = (_challengePendingList.Count > 0);
		}

		// Check if we have any challenges available
		if (_challengeCurrentNewList.Count == 0 && _challengeCurrentActiveList.Count == 0 && _challengeCompletedList.Count == 0) 
		{
			_noChallengesItem = Instantiate(Resources.Load("ChallengeScreen/NoChallengesItemPanel", typeof(Transform))) as Transform;
			_noChallengesItem.position =  new Vector3(0, 0, 0);
			_noChallengesItem.transform.localPosition = new Vector3 (0, 0, 0);
			_noChallengesItem.SetParent(_verticalScrollPanelContent, false); 

			HideBlackLoaderPanel();

			return;
		}

		// Create New Current Challenges if any
		itemsLen = _challengeCurrentNewList.Count;

		// Add New Challenge Title Panel
		if (itemsLen > 0) 
		{
			_newChallengeTitle = Instantiate(Resources.Load("ChallengeScreen/NewChallengesTitlePanel", typeof(Transform))) as Transform;
			_newChallengeTitle.position =  new Vector3(0, 0, 0);
			_newChallengeTitle.transform.localPosition = new Vector3 (0, 0, 0);
			_newChallengeTitle.SetParent(_verticalScrollPanelContent, false); 
			_newChallengeTitle.GetComponentInChildren<Text> ().text = "NEW CHALLENGES (" + _challengeCurrentNewList.Count.ToString() + ")";
		}

		// print (" System.DateTime.Now: " +  System.DateTime.Now);
		for (int i = 0; i < itemsLen; i++) 
		{
			NewChallengeItem newChallengeItem = Instantiate(Resources.Load("ChallengeScreen/NewChallengesItemPanel", typeof(NewChallengeItem))) as NewChallengeItem;
			newChallengeItem.transform.position =  new Vector3(0, 0, 0);
			newChallengeItem.transform.localPosition = new Vector3 (0, 0, 0);
			newChallengeItem.transform.SetParent(_verticalScrollPanelContent, false); 
		
			displayName = GameControl.instance.FormatUserNameDisplay(_challengeCurrentNewList[i].username);
			newChallengeItem.username.text = displayName;

			newChallengeItem.message.text = "CHALLENGED YOU!";
			newChallengeItem.challengeData = _challengeCurrentNewList[i];

			newChallengeItem.date.text = _challengeCurrentNewList[i].elapsed;

			ChallengeData chData = _challengeCurrentNewList [i];
			newChallengeItem.playBtn.onClick.AddListener (() => OnNewChallengeItemAcceptClick (chData));
			newChallengeItem.ignoreBtn.onClick.AddListener (() => OnNewChallengeItemIgnoreClick (chData));

			SetIconImage(newChallengeItem.icon, _challengeCurrentNewList [i].resourceid);

			_challengeCurrentNewItemList.Add(newChallengeItem);
		}

		string winStatus = "WIN!"; // post status added to winner label

		// Create Active Current Challenges if any
		itemsLen = _challengeCurrentActiveList.Count;

		// Add Active Challenge Title Panel
		if (itemsLen > 0) 
		{
			_activeChallengeTitle = Instantiate(Resources.Load("ChallengeScreen/CurrentChallengesTitlePanel", typeof(Transform))) as Transform;
			_activeChallengeTitle.position =  new Vector3(0, 0, 0);
			_activeChallengeTitle.localPosition = new Vector3 (0, 0, 0);
			_activeChallengeTitle.SetParent(_verticalScrollPanelContent, false); 
			_activeChallengeTitle.GetComponentInChildren<Text> ().text = "CURRENT GAMES (" + _challengeCurrentActiveList.Count.ToString() + ")";
		}

		for (int i = 0; i < itemsLen; i++) 
		{
			CurrentChallengeItem activeChallengeItem = Instantiate(Resources.Load("ChallengeScreen/CurrentChallengeItemPanel", typeof(CurrentChallengeItem))) as CurrentChallengeItem;
			activeChallengeItem.transform.position =  new Vector3(0, 0, 0);
			activeChallengeItem.transform.localPosition = new Vector3 (0, 0, 0);
			activeChallengeItem.transform.SetParent(_verticalScrollPanelContent, false); 

			activeChallengeItem.challengeData = _challengeCurrentActiveList[i];
			ChallengeData chData = _challengeCurrentActiveList [i];
			activeChallengeItem.playBtn.onClick.AddListener (() => OnCurrentChallengeItemPlayClick (chData));
		
			SetIconImage(activeChallengeItem.icon, _challengeCurrentActiveList[i].resourceid);

			// check if this is the player, here the player is friend ( Challenged )
			if (_challengeCurrentActiveList[i].frienduser == GameControl.instance.username) 
			{
				displayName = GameControl.instance.FormatUserNameDisplay(_challengeCurrentActiveList[i].username);
				activeChallengeItem.challengeUser.text = displayName;

				if (_challengeCurrentActiveList [i].usercompleted == 1 && _challengeCurrentActiveList [i].friendcompleted == 0) 
				{
					activeChallengeItem.challengeScore.text = "HIDDEN";
				} 
				else 
				{
					if (_challengeCurrentActiveList [i].usercompleted == 0) 
					{
						activeChallengeItem.challengeScore.text = "WAITING";
					} 
					else 
					{
						activeChallengeItem.challengeScore.text = _challengeCurrentActiveList[i].userscore.ToString();
					}
				}

				activeChallengeItem.userName.text = "YOU";
				activeChallengeItem.userScore.text = _challengeCurrentActiveList[i].friendscore.ToString();

				// play button disable if this user has completed the game
				activeChallengeItem.playBtn.interactable = (_challengeCurrentActiveList[i].friendcompleted == 0);

			}
			else // player is user ( Challenger )
			{
				displayName = GameControl.instance.FormatUserNameDisplay(_challengeCurrentActiveList[i].frienduser);
				activeChallengeItem.challengeUser.text = displayName;

				if (_challengeCurrentActiveList [i].friendcompleted == 1 && _challengeCurrentActiveList [i].usercompleted == 0) 
				{
					activeChallengeItem.challengeScore.text = "HIDDEN";
				} 
				else 
				{
					if (_challengeCurrentActiveList [i].friendcompleted == 0) 
					{
						activeChallengeItem.challengeScore.text = "WAITING";
					} 
					else 
					{
						activeChallengeItem.challengeScore.text = _challengeCurrentActiveList[i].friendscore.ToString();
					}
				}

				activeChallengeItem.userName.text = "YOU";
				activeChallengeItem.userScore.text = _challengeCurrentActiveList[i].userscore.ToString();

				// play button disable if this user has completed the game
				activeChallengeItem.playBtn.interactable = (_challengeCurrentActiveList[i].usercompleted == 0);
			}

			_challengeCurrentActiveItemList.Add(activeChallengeItem);
		}

		// Create Completed Challenges if any
		itemsLen = _challengeCompletedList.Count;

		// Add Active Challenge Title Panel
		if (itemsLen > 0) 
		{
			_completedChallengeTitle = Instantiate(Resources.Load("ChallengeScreen/CompletedGamesTitlePanel", typeof(Transform))) as Transform;
			_completedChallengeTitle.position =  new Vector3(0, 0, 0);
			_completedChallengeTitle.transform.localPosition = new Vector3 (0, 0, 0);
			_completedChallengeTitle.SetParent(_verticalScrollPanelContent, false); 
			_completedChallengeTitle.GetComponentInChildren<Text> ().text = "COMPLETED GAMES (" + _challengeCompletedList.Count.ToString() + ")";

			ChallengeClearButton challengeClearButton = _completedChallengeTitle.GetComponentInChildren<ChallengeClearButton>();
			challengeClearButton.GetComponent<Button>().onClick.AddListener(() => OnCompleteClearChallengesClick());
		}

		for (int i = 0; i < itemsLen; i++) 
		{
			CompletedChallengeItem completedChallengeItem = Instantiate(Resources.Load("ChallengeScreen/CompletedGameItemPanel", typeof(CompletedChallengeItem))) as CompletedChallengeItem;
			completedChallengeItem.transform.position =  new Vector3(0, 0, 0);
			completedChallengeItem.transform.localPosition = new Vector3 (0, 0, 0);
			completedChallengeItem.transform.SetParent(_verticalScrollPanelContent, false); 

			completedChallengeItem.challengeData = _challengeCompletedList[i];
			ChallengeData chData = _challengeCompletedList[i];
			completedChallengeItem.playBtn.onClick.AddListener(() => OnCompleteChallengeItemRematchClick(chData));

			SetIconImage(completedChallengeItem.icon, _challengeCompletedList[i].resourceid);

			// check if challenged(friend) declined
			if (_challengeCompletedList[i].frienduser == GameControl.instance.username) 
			{
				if (_challengeCompletedList [i].friendscore > _challengeCompletedList [i].userscore) 
				{
					winStatus = " WIN!";
				} 
				else if (_challengeCompletedList [i].friendscore == _challengeCompletedList [i].userscore)
				{
					 winStatus = " TIED!";
				}	
				else if (_challengeCompletedList [i].friendscore < _challengeCompletedList [i].userscore)
				{
					winStatus = " LOST!";
				}

				displayName = GameControl.instance.FormatUserNameDisplay(_challengeCompletedList[i].username);
				completedChallengeItem.challengeUser.text = displayName;
				completedChallengeItem.challengeScore.text = _challengeCompletedList[i].userscore.ToString();

				completedChallengeItem.userName.text = "YOU" + winStatus;
				completedChallengeItem.userScore.text = _challengeCompletedList[i].friendscore.ToString();
			}
			else
			{
				displayName = GameControl.instance.FormatUserNameDisplay(_challengeCompletedList[i].frienduser);
				completedChallengeItem.challengeUser.text = displayName;
				completedChallengeItem.challengeScore.text = _challengeCompletedList[i].friendscore.ToString();

				if (_challengeCompletedList [i].userscore > _challengeCompletedList [i].friendscore) 
				{
					winStatus = " WIN!";
				} 
				else if (_challengeCompletedList [i].userscore == _challengeCompletedList [i].friendscore)
				{
					winStatus = " TIED!";
				}	
				else if (_challengeCompletedList [i].userscore < _challengeCompletedList [i].friendscore)
				{
					winStatus = " LOST!";
				}

				completedChallengeItem.userName.text = "YOU" + winStatus;
				completedChallengeItem.userScore.text = _challengeCompletedList[i].userscore.ToString();
			}

			if (_challengeCompletedList[i].friendignored == 1) 
			{
				completedChallengeItem.challengeScore.text = "DECLINED";
				completedChallengeItem.userName.text = "YOU";
				completedChallengeItem.userScore.text = "DECLINED";
			}

			_challengeCompletedItemList.Add(completedChallengeItem);
		}

		HideBlackLoaderPanel();
	}

	void SetIconImage(Image icon, string resourceid)
	{
		string iconLocation = GetIconFromGameDataByResourceID(resourceid);
		if (iconLocation.Length > 0) 
		{
			string texture = homeScreenIconPath + iconLocation.Substring(0, iconLocation.Length - 4);
			Texture2D inputTexture = Resources.Load<Texture2D>(texture);
			Sprite sprite = Sprite.Create(inputTexture, new Rect(0, 0, 182, 182), Vector2.zero, 100f);

			icon.sprite = sprite;
		}
	}

	private Color _pendingNormalClr = new Vector4(90.0f / 255.0f, 90.0f / 255.0f, 90.0f / 255.0f, 1);
	void OnPendingChallengeClick()
	{
		GameControl.instance.soundManager.PlayLowToneButton();

		Button btn = _pendingChallengeTitle.GetComponentInChildren<Button>();
		ColorBlock cb = btn.colors;

		if (_challengePendingItemList.Count == 0) 
		{
			cb = btn.colors;
			cb.normalColor = Color.black;
			btn.colors = cb;

			ShowPendingItems();
		} 
		else 
		{
			cb = btn.colors;
			cb.normalColor = _pendingNormalClr;
			btn.colors = cb;

			RemovePendingItems();
		}
	}

	void ShowPendingItems()
	{
		// Create Pending Challenges if any
		int itemsLen = _challengePendingList.Count;

		for (int i = 0; i < itemsLen; i++) 
		{
			PendingChallengeItem pendingChallengeItem = Instantiate(Resources.Load("ChallengeScreen/PendingChallengeItemPanel", typeof(PendingChallengeItem))) as PendingChallengeItem;
			pendingChallengeItem.transform.position =  new Vector3(0, 0, 0);
			pendingChallengeItem.transform.localPosition = new Vector3 (0, 0, 0);

			pendingChallengeItem.transform.SetParent(_verticalScrollPanelContent, false); 
			pendingChallengeItem.transform.SetSiblingIndex (1);
			pendingChallengeItem.challengeUser.text = _challengePendingList[i].frienduser.ToUpper();
			pendingChallengeItem.challengeData = _challengePendingList[i];
			pendingChallengeItem.date.text = _challengePendingList[i].elapsed;

			SetIconImage(pendingChallengeItem.icon, _challengePendingList[i].resourceid);

			_challengePendingItemList.Add(pendingChallengeItem);
		}
	}

	void RemovePendingItems()
	{
		int itemsLen = _challengePendingList.Count;

		for (int i = 0; i < itemsLen; i++) 
		{
			DestroyImmediate(_challengePendingItemList[i].gameObject);
		}

		_challengePendingItemList.Clear();
	}

	// accept button for New Challenges
	void OnNewChallengeItemAcceptClick(ChallengeData challengeData)
	{
		// print ("OnNewChallengeItemAcceptClick() " + challengeData.username + " a: " + challengeData.auth);

		GameControl.instance.soundManager.PlayLowToneButton();

		GameControl.instance.challengeAuth = challengeData.auth;

		_modalPanel.Choice ("Accept challenge against", challengeData.username.Split(' ')[0] + "?", _mAcceptChallengeYesAction, _mChallengeNoAction);
	}

	// ignore button for New Challenges
	void OnNewChallengeItemIgnoreClick(ChallengeData challengeData)
	{
		// print ("OnNewChallengeItemIgnoreClick() " + challengeData.username + " a: " + challengeData.auth);
		GameControl.instance.soundManager.PlayLowToneButton();

		GameControl.instance.challengeAuth = challengeData.auth;

		_modalPanel.Choice ("Ignore game with", challengeData.username.Split(' ')[0] + "?", _mIngoreChallengeYesAction, _mChallengeNoAction);
	}

	// play button for Current Challenges
	void OnCurrentChallengeItemPlayClick(ChallengeData challengeData)
	{
		//print ("OnCurrentChallengeItemPlayClick() " + challengeData.username + " a: " + challengeData.auth);
		GameControl.instance.soundManager.PlayLowToneButton();

		GameControl.instance.challengeAuth = challengeData.auth;
		GameControl.instance.selectedResource = challengeData.resourceid;

		string displayName = "";

		if (GameControl.instance.email == challengeData.useremail) displayName = challengeData.frienduser;
		else displayName = challengeData.username;

		_modalPanel.Choice ("Play game with", displayName.Split(' ')[0] + "?", _mCurrentPlayChallengeYesAction, _mChallengeNoAction);
	}

	// rematch button for Completed Challenges
	void OnCompleteChallengeItemRematchClick(ChallengeData challengeData)
	{
		//print ("OnCompleteChallengeItemRematchClick() " + challengeData.username + " a: " + challengeData.auth);
		GameControl.instance.soundManager.PlayLowToneButton();

		GameControl.instance.selectedChallengedPlayer = new PlayerData();

		string displayName = "";
		if (challengeData.username == GameControl.instance.username) 
		{
			displayName = challengeData.frienduser;
			GameControl.instance.selectedChallengedPlayer.email = challengeData.friendemail;
		} 
		else 
		{
			displayName = challengeData.username;
			GameControl.instance.selectedChallengedPlayer.email = challengeData.useremail;
		}

		GameControl.instance.selectedResource = challengeData.resourceid;

		// "caticons/02_Performance_Tires/T_icon_C13_Potenza_RE970AS_normal.png"
		GameControl.instance.selectedResourceIconPath = GetIconFromGameDataByResourceID(challengeData.resourceid);

		//print ("challengeData.resourceid: " + challengeData.resourceid);
		//print ("GameControl.instance.selectedResourceIconPath: " + GameControl.instance.selectedResourceIconPath);

		_modalPanel.Choice ("Play again with", displayName.Split(' ')[0] + "?", _mRematchChallengeYesAction, _mChallengeNoAction);
	}

	private void OnAcceptChallengeYesAction() 
	{
		//print ("OnRematchChallengeYesAction");
		GameControl.instance.soundManager.PlayLowToneButton();

		StartCoroutine (AcceptGameChallengeFromServer());
	}

	IEnumerator AcceptGameChallengeFromServer()
	{
		StartCoroutine (ShowBlackLoaderPanel("Accepting Challenge..."));

		yield return _waitCheckChallenge;

		GameControl.instance.OnChallengeAcceptedComplete += OnChallengeAcceptedComplete;
		GameControl.instance.ChallengeAcceptToServer();
	}

	void OnChallengeAcceptedComplete(bool success, ChallengeData info)
	{
		HideBlackLoaderPanel ();

		GameControl.instance.OnChallengeAcceptedComplete -= OnChallengeAcceptedComplete;

		/*Debug.Log("OnChallengeAcceptedComplete success: " + success);
		Debug.Log("OnChallengeAcceptedComplete resourceid: " + info.resourceid);
		Debug.Log("OnChallengeAcceptedComplete friendaccepted: " + info.friendaccepted);
		Debug.Log("OnChallengeAcceptedComplete friendcompleted: " + info.friendcompleted);*/

		GameControl.instance.selectedResource = info.resourceid;
		GameControl.instance.selectedResourceIconPath = GetIconFromGameDataByResourceID(info.resourceid);

		if (success) // first time accepting challenge check
		{
			_modalPanel.Choice ("Challenge Accepted", "Play Now?", _mChallengeAcceptedPlayNowYesAction, _mChallengeNoAction);
		} 
		else // it will fail if we already accepted previously
		{
			if (info.friendcompleted == 1)  // check if we already played
			{
				// Debug.Log("Challenge Completed");

			} 
			else // If we are here, this means user already accepted challenge
			{
				// _modalPanel.Choice ("Challenge Already Accepted", "Play Now?", _mChallengeYesAction, _mChallengeNoAction);
			}
		}
	}

	private void OnIgnoreChallengeYesAction() 
	{
		// print ("OnIgnoreChallengeYesAction");
		GameControl.instance.soundManager.PlayLowToneButton();

		StartCoroutine (IgnoreGameChallengeFromServer());
	}

	IEnumerator IgnoreGameChallengeFromServer()
	{
		StartCoroutine (ShowBlackLoaderPanel("Declining Challenge..."));

		yield return _waitCheckChallenge;

		GameControl.instance.OnChallengeIgnoredComplete += OnChallengeIgnoredComplete;
		GameControl.instance.ChallengeIgnoredToServer();
	}

	void OnChallengeIgnoredComplete(bool success, ChallengeData info)
	{
		GameControl.instance.OnChallengeIgnoredComplete -= OnChallengeIgnoredComplete;

		HideBlackLoaderPanel ();

		StartCoroutine (ResetContent());
	}

	private void OnCurrentPlayChallengeYesAction() 
	{
		//print ("OnCurrentPlayChallengeYesAction");
		GameControl.instance.soundManager.PlayLowToneButton();

		LoadPlayChallenge();
	}

	private void OnRematchChallengeYesAction() 
	{
		//print ("OnRematchChallengeYesAction");
		GameControl.instance.soundManager.PlayLowToneButton();

		StartCoroutine (CreateRematchGameChallengeFromServer());
	}

	IEnumerator CreateRematchGameChallengeFromServer()
	{
		StartCoroutine (ShowBlackLoaderPanel("Creating Rematch..."));

		yield return _waitCheckChallenge;

		GameControl.instance.OnSendChallengeComplete += OnSendChallengeComplete;
		GameControl.instance.SendChallengeToServer();
	}

	void OnSendChallengeComplete(bool success)
	{
		GameControl.instance.OnSendChallengeComplete -= OnSendChallengeComplete;

		HideBlackLoaderPanel();

		//print ("OnSendChallengeComplete");

		StartCoroutine (ResetContent());
	}

	private void OnChallengeAcceptedPlayNowYesAction()
	{
		//print ("OnChallengeAcceptedPlayNowYesAction");
		GameControl.instance.soundManager.PlayLowToneButton();

		LoadPlayChallenge();
	}

	private void OnChallengeNoAction() 
	{
		//print ("OnChallengeNoAction");
		GameControl.instance.soundManager.PlayLowToneButton();

		StartCoroutine (ResetContent());
	}

	private void LoadPlayChallenge()
	{
		GameControl.instance.isChallengeMode = true;

		_challengeManager.LoadChallengeSelectByResource(GameControl.instance.selectedResource);

		_loadScene.LoadQuiz();

		DisposePanels();
	}

	IEnumerator ShowBlackLoaderPanel(string message = "Checking Challenge...")
	{
		GameObject subScreen = (GameObject)Instantiate (Resources.Load ("HomeScreen/BlackPanel"));  

		yield return subScreen;

		_blackPanel = subScreen.GetComponent<BlackPanel> ();
		_blackPanel.GetComponent<CanvasGroup> ().alpha = 0; 
		_blackPanel.gameObject.transform.SetParent(_stage.transform, false);
		_blackPanel.GetComponent<FadeMe>().startFadeIn (0.85f);

		_blackPanel.GetComponentInChildren<Text> ().text = message;

		LeanTween.delayedCall (0.1f, _stage.ShowLoader);
	}

	void HideBlackLoaderPanel()
	{
		//print ("HideBlackLoaderPanel ------------------------------------");

		_stage.HideLoader();

		DestroyImmediate (_blackPanel.gameObject);

		_blackPanel = null;
	}

	void OnCompleteClearChallengesClick()
	{
		GameControl.instance.selectedChallengedPlayer = new PlayerData();

		_modalPanel.Choice ("Clear your completed games", "", _mClearChallengesYesAction, _mChallengeNoAction);
	}

	void OnClearChallengesYesAction()
	{
		StartCoroutine (ClearCompletedChallengesFromServer ());
	}

	IEnumerator ClearCompletedChallengesFromServer()
	{
		StartCoroutine (ShowBlackLoaderPanel("Clearing Challenges..."));

		yield return _waitCheckChallenge;

		GameControl.instance.OnClearChallengesComplete += OnClearChallengesComplete;
		GameControl.instance.ClearChallengesToServer();
	}

	void OnClearChallengesComplete(bool success)
	{
		GameControl.instance.OnClearChallengesComplete -= OnClearChallengesComplete;

		HideBlackLoaderPanel();

		// print ("OnClearChallengesComplete");

		StartCoroutine(ResetContent());
	}
}
