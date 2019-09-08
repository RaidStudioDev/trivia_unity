using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.IO; 
using HeathenEngineering.OSK.v2;

public class HomeScreenController : ScreenController {

	private GameData loadedData;

	public Transform fullViewItemPanel;
	public Transform categoryItemPanel;
	public Transform itemPanel;

	private GameObject _itemPool;
	private Dictionary<string, Transform> _itemPanels;
	private List<Transform> _itemIconButtons;
	private List<Transform> _categoryPanels;
	private List<Transform> _fullViewCategoryPanels;

	private string homeScreenIconPath = "HomeScreen/";

	private ChallengePlaySubPanel _challengeSubPanel;
	private SendingChallengeSubPanel _sendingChallengeSubPanel;
	private SendingRandomChallengeSubPanel _sendingRandomChallengeSubPanel;
	private BlackPanel _blackPanel;

	// Vertical Scroller Panel - Contains our category items
	private Transform _verticalScrollPanel;
	private Transform _verticalScrollPanelContent;
	private ScrollRect _verticalScrollRect;

	private int _selectedCatIndex = -1;
	private int _selectedItemIndex = -1;
	private float _currentVerticalSrollPosition = 1f;	// 0 == Bottom
	private bool _isFullViewMode = false;

	private ModalPanel _modalPanel;
	private UnityAction _mChallengeYesAction;
	private UnityAction _mChallengeNoAction;

	private WaitForEndOfFrame _waitForFrame = new WaitForEndOfFrame();
	private WaitForSeconds _waitCheckChallenge = new WaitForSeconds(1.15f);
	private WaitForSeconds _waitInitCatItems = new WaitForSeconds(0.2f);

	private RectTransform _notifications;
	private Text _notificationsText;

	void Start()
	{
		base.StartInit();

		Initialize();

		InitAlertBox();

		InitKeyboardPanel();

		UpdateLayout();

		StartCoroutine(StartGameCategoryDataLoad());
	}

	void Initialize()
	{
		if (!categoryItemPanel) categoryItemPanel = Instantiate(Resources.Load("HomeScreen/CategoryItemPanel", typeof(Transform))) as Transform;
		if (!itemPanel) itemPanel = Instantiate(Resources.Load("HomeScreen/IconPanel", typeof(Transform))) as Transform;
		if (!fullViewItemPanel) fullViewItemPanel = Instantiate(Resources.Load("HomeScreen/FullViewItemPanel", typeof(Transform))) as Transform;

		// Get Challenge Play Screen
		// _challengeSubPanel = _stage.transform.Find("ChallengePlaySubScreen").GetComponent<ChallengePlaySubPanel>();

		// list of all our created items
		_fullViewCategoryPanels = new List<Transform>();
		_categoryPanels = new List<Transform>();
		_itemPanels = new Dictionary<string, Transform>();

		// create a simple item pool via gameObject, we will store some items here and reuse if available
		_itemPool = new GameObject();
		_itemPool.name = "ItemPool";

		// init main vert scroller
		Transform _challengeBtn = this.transform.Find("SubHeader").Find("ChallengesBtn");
		_notifications = _challengeBtn.Find("Notification").GetComponent<RectTransform>();
		_notificationsText = _notifications.GetComponentInChildren<Text>();
		_notifications.GetComponent<CanvasGroup> ().alpha = 0f;

		_verticalScrollPanel = this.transform.Find("VerticalScrollPanel").GetComponent<Transform>();
		_verticalScrollRect = _verticalScrollPanel.GetComponent<ScrollRect> ();
		_verticalScrollPanelContent = _verticalScrollPanel.GetComponent<ScrollRect>().content.GetComponent<Transform>();
	}

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
		_mChallengeYesAction = new UnityAction(OnChallengeYesAction);
		_mChallengeNoAction = new UnityAction(OnChallengeNoAction);
	}

	void InitKeyboardPanel()
	{
		if (!GameControl.instance.isMobile) return;

		OnScreenKeyboard keyboardPanel = Instantiate(Resources.Load("HomeScreen/OnScreenKeyboard", typeof(OnScreenKeyboard))) as OnScreenKeyboard;
		keyboardPanel.gameObject.SetActive (false);
		keyboardPanel.transform.SetParent(_stage.transform, false); 

		KeyboardController.Instance().keyboard = keyboardPanel;
	}

	void LoadGameData()
	{
		TextAsset jsonDataFile = Resources.Load<TextAsset>("HomeScreen/homeScreenItems");
		loadedData = JsonUtility.FromJson<GameData>(jsonDataFile.text);
	}

	void InitializeCategoryPanels() 
	{
		int dataLen = loadedData.allRoundData.Length;

		for (int i = 0; i < dataLen; i++) InitializeHorizontalItem(i);

		StartCoroutine(FadeInCategoryContentPanel());
	}

	void ShowCategoryPanels() 
	{
		int dataLen = loadedData.allRoundData.Length;
		for (int i = 0; i < dataLen; i++) ShowHorizontalItem(i);

		StartCoroutine(FadeInCategoryContentPanel());

		StartCoroutine(SetScrollerPosition(_currentVerticalSrollPosition));
	}

	private WaitForEndOfFrame waitScrollPosFrame = new WaitForEndOfFrame();

	IEnumerator SetScrollerPosition(float scrollPos)
	{
		yield return waitScrollPosFrame;

		_verticalScrollRect.verticalNormalizedPosition = scrollPos;      
	}

	void ShowHorizontalItem(int catIndex)
	{
		Transform categoryPanelTrans = _categoryPanels[catIndex];
		categoryPanelTrans.SetParent(_verticalScrollPanelContent); 
	}

	void InitializeHorizontalItem(int catIndex) 
	{
		GameItemData dataCategoryItems = loadedData.allRoundData [catIndex];

		Transform copyItem = Instantiate (categoryItemPanel, categoryItemPanel.position, categoryItemPanel.transform.rotation) as Transform;
		copyItem.position =  new Vector3(0, 0, 0);
		copyItem.transform.localPosition = new Vector3 (0, 0, 0);
		copyItem.SetParent(_verticalScrollPanelContent, false); 

		Transform scrollPanelTr = copyItem.GetComponent<Transform>();
		Transform scrollContent = scrollPanelTr.Find("ScrollPanel").GetComponent<ScrollerExtend>().content.GetComponent<Transform>();

		_categoryPanels.Add(scrollPanelTr);

        RectTransform headerTransform = copyItem.gameObject.transform.Find("Header") as RectTransform;
		RectTransform textTransform = headerTransform.Find ("CategoryLabel") as RectTransform;
		Text title = textTransform.GetComponent<Text>();
		title.text = dataCategoryItems.title.ToUpper();

		// Get See All Button
		Button seeAllBtn = headerTransform.Find ("Button").GetComponent<Button>();
		seeAllBtn.onClick.AddListener (() => OnSeeAllClick (catIndex));

		Transform itemButtonTrans;
		Button itemBtn;
		GameDataItems gameDataItem;
		ItemIcon itemIcon;

		int catItemLen = dataCategoryItems.items.Length;

		// if we have less than 4 items, disable see all
		if (catItemLen < 4) 
		{
			seeAllBtn.gameObject.SetActive (false);
		}

		for (int i = 0; i < catItemLen; i++) 
		{
			gameDataItem = dataCategoryItems.items[i];

			itemButtonTrans = Instantiate (itemPanel, itemPanel.position, itemPanel.transform.rotation) as Transform;
			itemButtonTrans.position =  new Vector3(0, 0, 0);
			itemButtonTrans.transform.localPosition = new Vector3 (0, 0, 0);
			itemButtonTrans.SetParent(scrollContent, false); 

			itemIcon = itemButtonTrans.GetComponent<ItemIcon> ();
			itemIcon.catIndex = catIndex;
			itemIcon.index = i;

			_itemPanels.Add(gameDataItem.resource, itemButtonTrans);

			int itemIndex = i;
			itemBtn = _itemPanels [gameDataItem.resource].GetComponent<Button> ();
			itemBtn.onClick.AddListener (() => OnItemClick (catIndex, itemIndex));

			// Update Main Image
			SetIconButtonImage(itemBtn, dataCategoryItems.imagePath + gameDataItem.icons[0].normal);
		}
	}

	IEnumerator StartGameCategoryDataLoad()
	{
		yield return _waitForFrame;

		LoadGameData();

		InitializeCategoryPanels();

		yield return _waitInitCatItems;

		// Check if we are coming from email
		// Debug.Log("HomeScreenController().challengeAccepted: " + GameControl.instance.challengeAccepted);

		if (GameControl.instance.challengeAccepted) 
		{
			StartCoroutine(CheckForChallengeStartUp());
		} 
		else 
		{
			// check if we have a auth key, if so this means user clicked on play in email
			if (GameControl.instance.challengeAuth.Length > 0) 
			{
				StartCoroutine(CheckForPlayChallengeStartUp());
			}
			else StartCoroutine(GetChallengeNotifications()); // get notifications if we do not have a challenge
		}
	}

	IEnumerator GetChallengeNotifications()
	{
		yield return _waitCheckChallenge;

		GameControl.instance.OnNotificationsComplete += OnNotificationsComplete;
		GameControl.instance.GetChallengeNotificationsFromServer();
	}

	void OnNotificationsComplete(bool success, ChallengeData info)
	{
		if (success && info.notifications > 0) 
		{
			if (_notificationsText != null) _notificationsText.text = info.notifications.ToString();
	
			LeanTween.alphaCanvas(_notifications.GetComponent<CanvasGroup>(), 1f, 0.75f).setEase(LeanTweenType.easeOutCubic);
		} 
	}

	IEnumerator CheckForPlayChallengeStartUp()
	{
		StartCoroutine (ShowBlackLoaderPanel("Updating Challenge..."));

		yield return _waitCheckChallenge;

		GameControl.instance.OnPlayChallengeComplete += OnPlayChallengeComplete;
		GameControl.instance.PlayChallengeToServer();
	}

	void OnPlayChallengeComplete(bool success, ChallengeData info)
	{
		HideBlackLoaderPanel ();

		GameControl.instance.OnPlayChallengeComplete -= OnPlayChallengeComplete;
		GameControl.instance.selectedResource = info.resourceid;

		if (success) // first time accepting challenge check
		{
			_modalPanel.Choice ("Play with " + info.frienduser.Split(' ')[0], "Challenge Now?", _mChallengeYesAction, _mChallengeNoAction);
		} 
		else // it will fail if we already accepted previously
		{
			if (info.usercompleted == 1)  // check if we already played
			{
				//Debug.Log("Challenge Completed");

			} 
			else // If we are here, this means user already completed the challenge
			{
				// _modalPanel.Choice ("Challenge Already Played", "Play Now?", _mChallengeYesAction, _mChallengeNoAction);
			}
		}
	}

	IEnumerator CheckForChallengeStartUp()
	{
		StartCoroutine (ShowBlackLoaderPanel("Updating Challenge..."));

		yield return _waitCheckChallenge;

		GameControl.instance.OnChallengeAcceptedComplete += OnChallengeAcceptedComplete;
		GameControl.instance.ChallengeAcceptToServer();
	}

	void OnChallengeAcceptedComplete(bool success, ChallengeData info)
	{
		HideBlackLoaderPanel ();

		GameControl.instance.OnChallengeAcceptedComplete -= OnChallengeAcceptedComplete;
		GameControl.instance.selectedResource = info.resourceid;

		if (success) // first time accepting challenge check
		{
			_modalPanel.Choice ("Challenge Accepted", "Play Now?", _mChallengeYesAction, _mChallengeNoAction);
		} 
		else // it will fail if we already accepted previously
		{
			if (info.friendcompleted == 1)  // check if we already played
			{
				//Debug.Log("Challenge Completed");

			} 
			else // If we are here, this means user already accepted challenge
			{
				// _modalPanel.Choice ("Challenge Already Accepted", "Play Now?", _mChallengeYesAction, _mChallengeNoAction);
			}
		}
	}

	private void OnChallengeYesAction() 
	{
		//print ("OnChallengePlayerYesAction");

		GameControl.instance.isChallengeMode = true;

		_challengeManager.LoadChallengeSelectByResource(GameControl.instance.selectedResource);

		_loadScene.LoadQuiz();

		DisposeCategoryPanels();
	}

	private void OnChallengeNoAction() 
	{
		//print ("OnChallengeNoAction");

		// Clear Challenge Info
		GameControl.instance.ClearChallengeInfo();

		StartCoroutine(GetChallengeNotifications());
	}

	void SetCatPanelsToItemPool() 
	{
		// Remove Category Panels and move them to the pool
		for (int i = 0; i < _categoryPanels.Count; i++) _categoryPanels[i].SetParent(_itemPool.transform);
	}

	void DisposeCategoryPanels() 
	{
		//Debug.Log ("DisposeCategoryPanels");

		int catItemLen = loadedData.allRoundData.Length;
		int itemLen = 0;

		GameItemData dataCategoryItems;
		GameDataItems gameDataItem;
		Button itemBtn;

		// Remove Listeners
		for (int catIndex = 0; catIndex < catItemLen; catIndex++) 
		{
			dataCategoryItems = loadedData.allRoundData [catIndex];

			itemLen = dataCategoryItems.items.Length;

			for (int itemIndex = 0; itemIndex < itemLen; itemIndex++) 
			{
				gameDataItem = dataCategoryItems.items [itemIndex];

				itemBtn = _itemPanels [gameDataItem.resource].GetComponent<Button> ();
				itemBtn.onClick.RemoveAllListeners ();

			}
		}

		// Remove Category Panels and remove See All Listeners
		RectTransform headerTransform;
		Button seeAllBtn;
		for (int i = 0; i < _categoryPanels.Count; i++) 
		{
			headerTransform = _categoryPanels[i].gameObject.transform.Find("Header") as RectTransform;
			seeAllBtn = headerTransform.Find ("Button").GetComponent<Button>();
			seeAllBtn.onClick.RemoveAllListeners ();

			Destroy(_categoryPanels[i].gameObject);
		}

		// Reset Lists and Dictionaries
		_categoryPanels.Clear();
		_itemPanels.Clear();
	}

	void ShowFullViewPanel() 
	{
		ShowFullViewItem(_selectedCatIndex);

		StartCoroutine(FadeInCategoryContentPanel());
	}

	void ShowFullViewItem(int catIndex)
	{
		GameItemData item = loadedData.allRoundData[catIndex];

		// Set CatPanel parent
		Transform categoryPanelTrans = _fullViewCategoryPanels[0];	
		categoryPanelTrans.SetParent(_verticalScrollPanelContent); 

		RectTransform scrollContent = categoryPanelTrans.gameObject.transform.Find("Content") as RectTransform;
		RectTransform headerTransform = categoryPanelTrans.Find("Header") as RectTransform;
		RectTransform textTransform = headerTransform.Find ("CategoryLabel") as RectTransform;
		Text title = textTransform.GetComponent<Text>();
		title.text = item.title.ToUpper();

		Transform itemButtonTrans;
		Image itemImage;
		GameDataItems gameDataItem;

		for (int i = 0; i < item.items.Length; i++) 
		{
			gameDataItem = item.items[i];

			_itemPanels[gameDataItem.resource].SetParent(scrollContent);
			itemButtonTrans = _itemPanels[gameDataItem.resource];

			itemImage = itemButtonTrans.GetComponent<Image> ();
			itemImage.preserveAspect = true;
		}

		StartCoroutine(ScrollToTop());
	}

	private WaitForEndOfFrame _waitEndOfFrame = new WaitForEndOfFrame();
	IEnumerator ScrollToTop()
	{
		yield return _waitEndOfFrame;

		_verticalScrollRect.verticalNormalizedPosition = 1f;
	}

	void InitializeFullViewItem(int catIndex) 
	{
		GameItemData item = loadedData.allRoundData [catIndex];

		Transform copyItem = Instantiate (fullViewItemPanel, fullViewItemPanel.position, fullViewItemPanel.transform.rotation) as Transform;
		copyItem.position =  new Vector3(0, 0, 0);
		copyItem.transform.localPosition = new Vector3 (0, 0, 0);
		copyItem.SetParent (_verticalScrollPanelContent, false); 

		RectTransform scrollContent = copyItem.gameObject.transform.Find("Content") as RectTransform;
		Transform scrollPanelTr = copyItem.GetComponent<Transform>();

		_fullViewCategoryPanels.Add(scrollPanelTr);

		RectTransform headerTransform = copyItem.gameObject.transform.Find("Header") as RectTransform;
		RectTransform textTransform = headerTransform.Find ("CategoryLabel") as RectTransform;
		Text title = textTransform.GetComponent<Text>();
		title.text = item.title.ToUpper();

		// Get Close Button
		Button closeBtn = headerTransform.Find ("Button").GetComponent<Button>();
		closeBtn.onClick.AddListener (() => OnCloseClick (catIndex));

		Transform itemButtonTrans;
		Image itemImage;
		GameDataItems gameDataItem;

		for (int i = 0; i < item.items.Length; i++) 
		{
			gameDataItem = item.items[i];

			_itemPanels[gameDataItem.resource].SetParent(scrollContent);
			itemButtonTrans = _itemPanels[gameDataItem.resource];

			itemImage = itemButtonTrans.GetComponent<Image> ();
			itemImage.preserveAspect = true;
		}

		StartCoroutine (FadeInCategoryContentPanel());
	}

	void SetFullViewToPool() 
	{
		GameItemData dataCategoryItems;
		GameDataItems gameDataItem;
		Transform scrollContent;

		// Remove Listeners
		dataCategoryItems = loadedData.allRoundData [_selectedCatIndex];

		int itemLen = dataCategoryItems.items.Length;

		for (int itemIndex = 0; itemIndex < itemLen; itemIndex++) 
		{
			gameDataItem = dataCategoryItems.items [itemIndex];

			// Put Icon back to the Item Pool Cat Panel
			scrollContent = _categoryPanels[_selectedCatIndex].Find("ScrollPanel").GetComponent<ScrollerExtend>().content.GetComponent<Transform>();
			_itemPanels[gameDataItem.resource].SetParent(scrollContent);
		}

		for (int i = 0; i < _fullViewCategoryPanels.Count; i++) 
		{
			_fullViewCategoryPanels[i].SetParent(_itemPool.transform);
		}

		_selectedCatIndex = -1;
	}


	void DisposeCategoryFullViewPanel() 
	{
		GameItemData dataCategoryItems;
		GameDataItems gameDataItem;
		Button itemBtn;

		// Remove Listeners
		dataCategoryItems = loadedData.allRoundData [_selectedCatIndex];

		int itemLen = dataCategoryItems.items.Length;

		for (int itemIndex = 0; itemIndex < itemLen; itemIndex++) 
		{
			gameDataItem = dataCategoryItems.items [itemIndex];

			itemBtn = _itemPanels [gameDataItem.resource].GetComponent<Button> ();
			itemBtn.onClick.RemoveAllListeners ();

		}

		// Remove Category Panels and remove See All Listeners
		RectTransform headerTransform;
		Button closeBtn;

		for (int i = 0; i < _categoryPanels.Count; i++) 
		{
			headerTransform = _categoryPanels[i].gameObject.transform.Find("Header") as RectTransform;
			closeBtn = headerTransform.Find ("Button").GetComponent<Button>();
			closeBtn.onClick.RemoveAllListeners ();

			Destroy (_categoryPanels[i].gameObject);
		}

		// Reset Lists and Dictionaries
		_categoryPanels.Clear ();
		_itemPanels.Clear ();

		_selectedCatIndex = -1;
	}

	// BUTTON EVENT LISTENERS ******************************************************************************

	void OnCloseClick (int catIndex)
	{
		GameControl.instance.soundManager.PlayLowToneButton();

		_isFullViewMode = false;

		StartCoroutine(TransitionFromFullViewToCategory());
	}

	void OnSeeAllClick (int catIndex) 
	{
		GameControl.instance.soundManager.PlayLowToneButton();

		_isFullViewMode = true;

		_selectedCatIndex = catIndex;

		_currentVerticalSrollPosition = _verticalScrollRect.verticalNormalizedPosition;

		StartCoroutine(TransitionFromCategoryToFullView());
	}


	void OnItemClick(int catIndex, int index) 
	{
		// print ("Home.OnItemClick");
		GameControl.instance.soundManager.PlayLowToneButton();

		_selectedCatIndex = catIndex;
		_selectedItemIndex = index;

		ShowChallengeSubScreen();
	}

	void ShowChallengeSubScreen()
	{
		GameItemData dataCategoryItems = loadedData.allRoundData[_selectedCatIndex];
		GameDataItems gameDataItem = dataCategoryItems.items[_selectedItemIndex];

		GameObject subScreen = (GameObject)Instantiate (Resources.Load ("HomeScreen/screens/ChallengePlaySubScreen"));  
		_challengeSubPanel = subScreen.GetComponent<ChallengePlaySubPanel> ();
		_challengeSubPanel.gameObject.transform.SetParent(_stage.transform, false);
		_challengeSubPanel.Show(gameDataItem, homeScreenIconPath + dataCategoryItems.imagePath);
		_challengeSubPanel.OnSelectedEvent += OnChallengeSelected;
		_challengeSubPanel.OnCloseEvent += OnChallengePanelClose;
	}

	void OnChallengePanelClose() 
	{
		_challengeSubPanel.OnSelectedEvent -= OnChallengeSelected;
		_challengeSubPanel.OnCloseEvent -= OnChallengePanelClose;
		_challengeSubPanel.Dispose();

		DestroyImmediate (_challengeSubPanel.gameObject);

		_challengeSubPanel = null;
	}

	void OnChallengeSelected(int challengeSelectId) 
	{
		//print ("Home.OnChallengeSelected.challengeSelectId: " + challengeSelectId);

		switch (challengeSelectId) 
		{
		case ChallengePlaySubPanel.SINGLE_PLAYER:
			//print ("SINGLE_PLAYER");
			OnSinglePlaySelected ();
			break;

		case ChallengePlaySubPanel.CHALLENGE_PLAYER:
			//print ("CHALLENGE_PLAYER");
			OnChallengePlaySelected ();
			break;

		case ChallengePlaySubPanel.RANDOM_PLAYER:
			//print ("RANDOM_PLAYER");
			OnRandomPlaySelected();
			break;
		}
	}

	public void OnRandomPlaySelected() 
	{
		GameItemData dataCategoryItems = loadedData.allRoundData[_selectedCatIndex];
		GameDataItems gameDataItem = dataCategoryItems.items[_selectedItemIndex];

		GameControl.instance.isChallengeMode = true;
		GameControl.instance.selectedResource = gameDataItem.resource;
		GameControl.instance.selectedResourceIconPath = dataCategoryItems.imagePath + gameDataItem.icons[0].normal;

		Transform subPanelContent = _challengeSubPanel.GetComponent<ScrollRect>().content.GetComponent<Transform>();

		subPanelContent.GetComponent<FadeMe>().startFadeOut (0.5f);

		LeanTween.delayedCall (0.25f, ShowRandomChallengeSubScreen);
	}

	void ShowRandomChallengeSubScreen()
	{
		StartCoroutine (LoadRandomChallengeSubScreen ());
	}

	IEnumerator LoadRandomChallengeSubScreen()
	{
		GameItemData dataCategoryItems = loadedData.allRoundData[_selectedCatIndex];
		GameDataItems gameDataItem = dataCategoryItems.items[_selectedItemIndex];

		GameObject subScreen = (GameObject)Instantiate (Resources.Load ("HomeScreen/screens/SendingRandomChallengeSubScreen"));  
		_sendingRandomChallengeSubPanel = subScreen.GetComponent<SendingRandomChallengeSubPanel>();
		_sendingRandomChallengeSubPanel.gameObject.transform.SetParent(_stage.transform, false);
		_sendingRandomChallengeSubPanel.Show(gameDataItem, homeScreenIconPath + dataCategoryItems.imagePath);
		_sendingRandomChallengeSubPanel.OnCloseEvent += OnRandomChallengePanelClose;

		yield return subScreen;

		Transform sendingSubPanelContent = _sendingRandomChallengeSubPanel.GetComponent<ScrollRect>().content.GetComponent<Transform>();
		sendingSubPanelContent.GetComponent<FadeMe> ().startFadeIn (0.5f);

		yield return _waitLoadAnimation;

		_stage.ShowLoader();
	}

	void OnRandomChallengePanelClose() 
	{
		_stage.HideLoader();

		_sendingRandomChallengeSubPanel.OnCloseEvent -= OnRandomChallengePanelClose;
		_sendingRandomChallengeSubPanel.Dispose();

		DestroyImmediate (_sendingRandomChallengeSubPanel.gameObject);

		_sendingRandomChallengeSubPanel = null;

		Transform subPanelContent = _challengeSubPanel.GetComponent<ScrollRect>().content.GetComponent<Transform>();
		subPanelContent.GetComponent<FadeMe> ().startFadeIn (0.5f);

		// LeanTween.delayedCall (0.25f, ShowSendingChallengeSubScreen);
	}

	public void OnSinglePlaySelected() 
	{
		//print ("Home.OnSinglePlaySelected");

		GameControl.instance.isChallengeMode = false;
		GameItemData dataCategoryItems = loadedData.allRoundData[_selectedCatIndex];
		string resource = dataCategoryItems.items[_selectedItemIndex].resource;
		_challengeManager.LoadChallengeSelectByResource(resource);
		// _challengeManager.LoadChallengeSelectByResource("141_SB_C12_Potenza_RE97AS_FINAL.csv");

		_loadScene.LoadQuiz();

		DisposeCategoryPanels();
	}

	public void OnChallengePlaySelected()
	{
		// print ("Home.OnChallengePlaySelected");

		GameControl.instance.soundManager.PlayLowToneButton();

		GameItemData dataCategoryItems = loadedData.allRoundData[_selectedCatIndex];
		GameDataItems gameDataItem = dataCategoryItems.items[_selectedItemIndex];

		GameControl.instance.isChallengeMode = true;
		GameControl.instance.selectedResource = gameDataItem.resource;
		GameControl.instance.selectedResourceIconPath = dataCategoryItems.imagePath + gameDataItem.icons[0].normal;

		Transform subPanelContent = _challengeSubPanel.GetComponent<ScrollRect>().content.GetComponent<Transform>();

		subPanelContent.GetComponent<FadeMe>().startFadeOut (0.5f);

		LeanTween.delayedCall (0.25f, ShowSendingChallengeSubScreen);
	}

	void ShowSendingChallengeSubScreen()
	{
		StartCoroutine (LoadSendingChallengeSubScreen ());
	}

	private WaitForSeconds _waitLoadAnimation = new WaitForSeconds(0.25f);
	IEnumerator LoadSendingChallengeSubScreen()
	{
		GameItemData dataCategoryItems = loadedData.allRoundData[_selectedCatIndex];
		GameDataItems gameDataItem = dataCategoryItems.items[_selectedItemIndex];

		GameObject subScreen = (GameObject)Instantiate (Resources.Load ("HomeScreen/screens/SendingChallengeSubScreen"));  
		_sendingChallengeSubPanel = subScreen.GetComponent<SendingChallengeSubPanel> ();
		_sendingChallengeSubPanel.gameObject.transform.SetParent(_stage.transform, false);
		_sendingChallengeSubPanel.Show(gameDataItem, homeScreenIconPath + dataCategoryItems.imagePath);
		_sendingChallengeSubPanel.OnCloseEvent += OnSendingChallengePanelClose;

		yield return subScreen;

		Transform sendingSubPanelContent = _sendingChallengeSubPanel.GetComponent<ScrollRect>().content.GetComponent<Transform>();
		sendingSubPanelContent.GetComponent<FadeMe> ().startFadeIn (0.5f);

		yield return _waitLoadAnimation;

		_stage.ShowLoader();
	}

	void OnSendingChallengePanelClose() 
	{
		_stage.HideLoader();

		_sendingChallengeSubPanel.OnCloseEvent -= OnSendingChallengePanelClose;
		_sendingChallengeSubPanel.Dispose();

		DestroyImmediate (_sendingChallengeSubPanel.gameObject);

		_sendingChallengeSubPanel = null;

		Transform subPanelContent = _challengeSubPanel.GetComponent<ScrollRect>().content.GetComponent<Transform>();
		subPanelContent.GetComponent<FadeMe> ().startFadeIn (0.5f);

		// LeanTween.delayedCall (0.25f, ShowSendingChallengeSubScreen);
	}


	void DisposeScreen()
	{
		if (_isFullViewMode) SetFullViewToPool();

	}


	// TRANSITION FUNCTIONS  ******************************************************************************

	private float _fadeInTime = 0.95f;
	private WaitForSeconds _fadeInDelay = new WaitForSeconds(1.25f);
	IEnumerator FadeInCategoryContentPanel()
	{
		_verticalScrollPanelContent.GetComponent<FadeMe>().startFadeIn(_fadeInTime);

		yield return _fadeInDelay;

		_verticalScrollPanelContent.GetComponent<CanvasGroup>().interactable = true;


        /*
         * _verticalScrollPanel = this.transform.Find("VerticalScrollPanel").GetComponent<Transform>();
		_verticalScrollRect = _verticalScrollPanel.GetComponent<ScrollRect> ();
		_verticalScrollPanelContent = _verticalScrollPanel.GetComponent<ScrollRect>().content.GetComponent<Transform>();
         * */

        LayoutElement[] items = _verticalScrollPanel.GetComponent<ScrollRect>().content.GetComponentsInChildren<LayoutElement>();

        int dataLen = items.Length;// loadedData.allRoundData.Length;

        for (int i = 0; i < dataLen; i++)
        {
            // Debug.Log("items" + items[i].name);
            //Transform content = scrollPanelTr.Find("ScrollPanel").GetComponent<ScrollerExtend>().content;
            //content.GetComponent<ContentSizeFitter>().gameObject.SetActive(false);
        }

        
	}

	private float _fullToCatFadeTime = 0.35f;
	private WaitForSeconds _fullToCatDelay = new WaitForSeconds(0.55f);
	IEnumerator TransitionFromFullViewToCategory()
	{
		_verticalScrollPanelContent.GetComponent<CanvasGroup> ().interactable = false;
		_verticalScrollPanelContent.GetComponent<FadeMe>().startFadeOut(_fullToCatFadeTime);

		yield return _fullToCatDelay;

		SetFullViewToPool();

		ShowCategoryPanels();
	}

	private float _catToFullFadeTime = 0.25f;
	private WaitForSeconds _catToFullDelay = new WaitForSeconds(0.45f);
	IEnumerator TransitionFromCategoryToFullView()
	{
		_verticalScrollPanelContent.GetComponent<CanvasGroup> ().interactable = false;
		_verticalScrollPanelContent.GetComponent<FadeMe>().startFadeOut(_catToFullFadeTime);

		yield return _catToFullDelay;

		SetCatPanelsToItemPool();

		if (_fullViewCategoryPanels.Count == 0) InitializeFullViewItem(_selectedCatIndex);
		else ShowFullViewPanel();
	}

	// END OF TRANSITION FUNCTIONS  ******************************************************************************

	// BUTTON INIT & STATE FUNCTIONS  ****************************************************************************

	void SetIconButtonImage(Button btn, string url) 
	{
		string texture = homeScreenIconPath + url.Substring(0, url.Length - 4);
		Texture2D inputTexture = Resources.Load<Texture2D>(texture);

        Sprite sprite = Sprite.Create(inputTexture, new Rect(0, 0, 182, 182), Vector2.zero, 100f);

		Image btnImage = btn.GetComponentInParent<Image>();
		btnImage.sprite = sprite;
	}

    /*void SetIconButtonStates(Button btn, string over, string down) 
	{
		SpriteState btnSpriteState = btn.spriteState;

		Texture2D overTexture = Resources.Load<Texture2D>(homeScreenIconPath + over.Substring(0, over.Length - 4));
		Sprite overSprite = Sprite.Create(overTexture, new Rect(0, 0, 182, 182), Vector2.zero, 100f);
		btnSpriteState.highlightedSprite = overSprite;
		btn.spriteState = btnSpriteState;

		Texture2D downTexture = Resources.Load<Texture2D>(homeScreenIconPath + down.Substring(0, down.Length - 4));
		Sprite downSprite = Sprite.Create(downTexture, new Rect(0, 0, 182, 182), Vector2.zero, 100f);
		btnSpriteState.pressedSprite = downSprite;
		btn.spriteState = btnSpriteState;
	}

	IEnumerator setButtonImage(Button btn, string url) 
	{
		string texture = homeScreenIconPath + url.Substring(0, url.Length - 4);
		Texture2D inputTexture = Resources.Load<Texture2D>(texture);

		Sprite sprite = Sprite.Create(inputTexture, new Rect(0, 0, 182, 182), Vector2.zero, 100f);

		Image btnImage = btn.GetComponentInParent<Image>();
		btnImage.sprite = sprite;

		yield return null;
	}

	IEnumerator setButtonStates(Button btn, string over, string down) 
	{
		SpriteState btnSpriteState = btn.spriteState;

		Texture2D overTexture = Resources.Load<Texture2D>(homeScreenIconPath + over.Substring(0, over.Length - 4));
		Sprite overSprite = Sprite.Create(overTexture, new Rect(0, 0, 182, 182), Vector2.zero, 100f);
		btnSpriteState.highlightedSprite = overSprite;
		btn.spriteState = btnSpriteState;

		Texture2D downTexture = Resources.Load<Texture2D>(homeScreenIconPath + down.Substring(0, down.Length - 4));
		Sprite downSprite = Sprite.Create(downTexture, new Rect(0, 0, 182, 182), Vector2.zero, 100f);
		btnSpriteState.pressedSprite = downSprite;
		btn.spriteState = btnSpriteState;

		yield return null;
	}*/
    // END OF BUTTON INIT & STATE FUNCTIONS  ****************************************************************************

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
		_stage.HideLoader();

		DestroyImmediate (_blackPanel.gameObject);

		_blackPanel = null;
	}

	void Destroy()
	{
		loadedData = null;
	}
}
