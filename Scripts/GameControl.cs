using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System;

public delegate void OnUpdateUserProfileToServer(bool success);
public delegate void OnSendChallengeToServer(bool success);
public delegate void OnRandomChallengeToServer(bool success);
public delegate void OnChallengeAccepted(bool success, ChallengeData info);
public delegate void OnChallengeIgnored(bool success, ChallengeData info);
public delegate void OnPlayChallenge(bool success, ChallengeData info);
public delegate void OnUpdateScoreChallenge(bool success, ChallengeData info);
public delegate void OnGetChallenges(bool success, ChallengeData[] info);
public delegate void OnClearChallenges(bool success);
public delegate void OnNotifications(bool success, ChallengeData info);
public delegate void OnUpdateNotifications(bool success);

public class GameControl : MonoBehaviour {

	public event OnUpdateUserProfileToServer OnUpdateUserProfileComplete;
	public event OnSendChallengeToServer OnSendChallengeComplete;
	public event OnRandomChallengeToServer OnRandomChallengeComplete;
	public event OnChallengeAccepted OnChallengeAcceptedComplete;
	public event OnChallengeIgnored OnChallengeIgnoredComplete;
	public event OnPlayChallenge OnPlayChallengeComplete;
	public event OnUpdateScoreChallenge OnUpdateScoreChallengeComplete;
	public event OnGetChallenges OnGetChallengesComplete;
	public event OnClearChallenges OnClearChallengesComplete;
	public event OnNotifications OnNotificationsComplete;
	public event OnUpdateNotifications OnUpdateNotificationsComplete;

	public static GameControl instance;

	[HideInInspector] public SoundManager soundManager;
	[HideInInspector] public bool IsRunningIE = false;
		
	public bool isMobile = false;
	public bool isChangeUser = false;
	public bool isDevelopment;// = true;
	public bool isRandomized;// = true;
	public bool isMaxQuestions;// = true;
	public bool isTimer;// = true;

	[HideInInspector] public TextAsset CSVtoread;
	public int currentscore = 0;
	public int currentquestion = -1;
	public int questionsinchallenge = 10;
	public int selectedchallenge;
    public int totalscore;
    
	public string email { get; set; }
	public string username { get; set; }
	public string region { get; set; }
	public string org { get; set; }

	public int UserRank { get; set; }
	public int UserRegionRank { get; set; }
	public int UserOrgRank { get; set; }
	public int Leaderboardfilter { get; set; }
	public string URL { get; set; }
	public string RootURL { get; set; }
	public string ImageRootURL { get; set; }
	public string SaveDataURL { get; set; }
	public float menuScrollPosition { get; set; } // Default to top; this will record scrollbar pos when user goes to play game

	[HideInInspector] public List<string> Top10Nameslist;
	[HideInInspector] public List<string> Top10Scorelist;
	[HideInInspector] public List<string> PlayersRangeList;
	[HideInInspector] public List<string> ScoresRangeList;
	[HideInInspector] public List<string> PlayersRegionRangeList;
	[HideInInspector] public List<string> ScoresRegionRangeList;
	[HideInInspector] public List<string> PlayersOrgRangeList;
	[HideInInspector] public List<string> ScoresOrgRangeList;

	[HideInInspector] public PopUpPanel popUpPanel;
	[HideInInspector] public bool isPopUpComplete = false;		// Show popup if false

	[HideInInspector] public bool isChallengeMode = false;
	[HideInInspector] public string selectedResource;
	[HideInInspector] public string selectedResourceIconPath;
	[HideInInspector] public PlayerData selectedChallengedPlayer;

	// These values are populated when user accepts a challenge from email only
	[HideInInspector] public string challengeAuth;
	[HideInInspector] public bool challengeAccepted;

	#if UNITY_WEBGL_API
	[DllImport("__Internal")] public static extern string GetBrowserVersion();
	[DllImport("__Internal")] public static extern bool IsPhoneDetected();
	[DllImport("__Internal")] public static extern bool IsIPadDetected();
	#endif

	public void ClearChallengeInfo()
	{
		isChallengeMode = false;
		selectedResource = "";
		selectedResourceIconPath = "";
		selectedChallengedPlayer = null;
		challengeAuth = "";
		challengeAccepted = false;
	}

    void Awake ()
    {
		if (instance == null)
		{
			DontDestroyOnLoad (gameObject);
			instance = this;
			soundManager = new SoundManager();
		}
		else if (instance != this)
		{
			Destroy (gameObject);
		}

		#if !UNITY_EDITOR && UNITY_WEBGL

			isMobile = IsPhoneDetected();
			string browserVersion = GetBrowserVersion();

			// check if we are running on Internet Explorer 11
			if (browserVersion.IndexOf("Trident") > -1) 
			{
				IsRunningIE = true;
				Debug.Log("GameControl.browserVersion: " + browserVersion);
			}

			// if (browserVersion == "IE") Debug.Log("GameControl.browserVersion: " + browserVersion);
			// else Debug.Log("GameControl.browserVersion: " + browserVersion);

		#endif

		if (isDevelopment) 
		{
			//SaveDataURL = "SaveData.php";
			RootURL = "http://localhost:8080/apps/bridgestone/TriviaPHP/api/";
			ImageRootURL = "http://localhost:8080/apps/bridgestone/TriviaPHP/images/";
			URL = Application.absoluteURL;

			// Cleave's server
			// RootURL = "http://50.73.103.10/trivia/leaderboard/";
		} 
		else 
		{
            // RootURL = "https://www.lms.mybridgestoneeducation.com/TriviaPHP/__staging/api/"; 							
            RootURL = "https://www.lms.mybridgestoneeducation.com/TriviaPHP/__staging/api/"; 		
			ImageRootURL = "https://www.lms.mybridgestoneeducation.com/TriviaPHP/__staging/images/";
			URL = Application.absoluteURL;
		}

        GetPlayerData.GetURLData();

		if (!String.IsNullOrEmpty(username) && !String.IsNullOrEmpty(email) && !String.IsNullOrEmpty(region) && !String.IsNullOrEmpty(org)) 
		{
			//Debug.Log ("IsNullOrEmpty");

			if (!IsEmail (email)) 
			{
				//Debug.Log ("INVALID EMAIL");

				StartCoroutine(ShowAlertPanel("INVALID EMAIL"));
			}
			else UpdateUserProfileToServer();
		} 
		else 
		{
			// something has happened with the user's profile - we need to get 4 properties.
			// if any of these are missing we need to let the user know

			// check each property

			//print ("username: " + username + " len: " + username.Length);

			if (String.IsNullOrEmpty (username) && String.IsNullOrEmpty (email) && String.IsNullOrEmpty (region) && String.IsNullOrEmpty (org)) 
			{
				StartCoroutine(ShowAlertPanel("MISSING USER INFO"));
			}
			else if (String.IsNullOrEmpty(username))
			{
				StartCoroutine(ShowAlertPanel("MISSING USERNAME"));
			}
			else if (String.IsNullOrEmpty(email))
			{
				StartCoroutine(ShowAlertPanel("MISSING EMAIL"));
			}
			else if (String.IsNullOrEmpty(region) || String.IsNullOrEmpty(org))
			{
				StartCoroutine(ShowAlertPanel("MISSING REGION/STATE"));
			}
		}
    }

	public void UpdateUserProfileToServer()
	{
		string query = "UpdateUserProfile.php?t=8";
		query += "&f=" + WWW.EscapeURL(username);
		query += "&e=" + WWW.EscapeURL(email);
		query += "&r=" + WWW.EscapeURL(region);
		query += "&o=" + WWW.EscapeURL(org);

		//Debug.Log("UpdateUserProfileToServer email: " + email);

		WWW UpdateUserProfileAttempt = new WWW(GameControl.instance.RootURL + query);
		StartCoroutine(UpdateUserProfileAttemptWaitForRequest(UpdateUserProfileAttempt));
	}

	IEnumerator UpdateUserProfileAttemptWaitForRequest(WWW www)
	{
		yield return new WaitForSeconds(2f);

		yield return www;

		// check for errors
		if (www.error == null)
		{
			ChallengeData[] resultData = JsonHelper.FromJsonWrapped<ChallengeData>(www.text);

			bool success = (resultData [0].success == "true") ? true : false;

			if (OnUpdateUserProfileComplete != null) 
			{
				OnUpdateUserProfileComplete(success);
			}

			//Debug.Log("UpdateUserProfileAttemptWaitForRequest resultData.success: " + success);

		} 
		else
		{
			//Debug.Log ("UpdateUserProfileAttemptWaitForRequest | WWW Error: " + www.error);
		}
	}


	// SEND CHALLENGE
	// creates a new challenge and invites user via email

	public void SendChallengeToServer()
	{
		string res = selectedResource;
		if (res.IndexOf (".csv") != -1) res = selectedResource.Substring (0, selectedResource.Length - 4); 

		string query = "challenge/SendChallenge.php?t=8";
		query += "&che=" + WWW.EscapeURL(selectedChallengedPlayer.email);
		query += "&ue=" + WWW.EscapeURL(email);
		query += "&res=" + res;
		query += "&resImg=" + WWW.EscapeURL(selectedResourceIconPath);

		WWW SendChallengeAttempt = new WWW(GameControl.instance.RootURL + query);
		StartCoroutine(SendChallengeAttemptWaitForRequest(SendChallengeAttempt));
	}

	IEnumerator SendChallengeAttemptWaitForRequest(WWW www)
	{
		yield return www;

		// check for errors
		if (www.error == null) 
		{
			SaveUserData[] resultData = JsonHelper.FromJsonWrapped<SaveUserData> (www.text);

			//Debug.Log("SendChallengeAttemptWaitForRequest resultData.success: " + www.text);

			if (OnSendChallengeComplete != null) 
			{
				bool success = (resultData [0].success == "true") ? true : false;

				OnSendChallengeComplete (success);
			}
		} 
		else 
		{
			// Debug.Log("WWW Error: "+ www.error);
		}
	}

	// RANDOM CHALLENGE
	// create challenge from a random user
	public void RandomChallengeToServer()
	{
		string res = selectedResource;
		if (res.IndexOf (".csv") != -1) res = selectedResource.Substring (0, selectedResource.Length - 4);
	
		string query = "challenge/SendRandomChallenge.php?t=8";
		query += "&ue=" + WWW.EscapeURL(email);
		query += "&res=" + res;
		query += "&resImg=" + selectedResourceIconPath;

		WWW SendRandomChallengeAttempt = new WWW(GameControl.instance.RootURL + query);
		StartCoroutine(SendRandomChallengeAttemptWaitForRequest(SendRandomChallengeAttempt));
	}

	IEnumerator SendRandomChallengeAttemptWaitForRequest(WWW www)
	{
		yield return www;

		// check for errors
		if (www.error == null) 
		{
			SaveUserData[] resultData = JsonHelper.FromJsonWrapped<SaveUserData> (www.text);

			// Debug.Log("SendRandomChallengeAttemptWaitForRequest resultData.success: " + resultData[0].success);

			if (OnRandomChallengeComplete != null) 
			{
				bool sucess = (resultData [0].success == "true") ? true : false;

				OnRandomChallengeComplete (sucess);
			}
		} 
		else 
		{
			Debug.Log("WWW Error: "+ www.error);
		}
	}



	// PLAY CHALLENGE
	// challenger plays game from email

	public void PlayChallengeToServer()
	{
		string query = "challenge/PlayChallenge.php?t=8";
		query += "&a=" + challengeAuth;

		// print ("PlayChallengeToServer() challengeAuth: " + challengeAuth);

		WWW PlayChallengeAttempt = new WWW(GameControl.instance.RootURL + query);
		StartCoroutine(PlayChallengeAttemptWaitForRequest(PlayChallengeAttempt));
	}

	IEnumerator PlayChallengeAttemptWaitForRequest(WWW www)
	{
		yield return www;

		// check for errors
		if (www.error == null) 
		{
			ChallengeData[] resultData = JsonHelper.FromJsonWrapped<ChallengeData> (www.text);

			// Debug.Log("PlayChallengeAttemptWaitForRequest resultData.success: " + resultData[0].success);
			// Debug.Log("PlayChallengeAttemptWaitForRequest resultData.resourceid: " + resultData[0].resourceid);

			if (OnPlayChallengeComplete != null) 
			{
				bool success = (resultData [0].success == "true") ? true : false;

				// Debug.Log("PlayChallengeAttemptWaitForRequest success: " + success);

				OnPlayChallengeComplete (success, resultData [0]);
			}

		} 
		else 
		{
			// Debug.Log("WWW Error: "+ www.error);
		}	
	}


	// ACCEPT CHALLENGE
	// confirms challenge from user, returns challenge info

	public void ChallengeAcceptToServer()
	{
		string query = "challenge/ConfirmChallenge.php?t=8";
		query += "&a=" + challengeAuth;

		// print ("ChallengeAcceptToServer() challengeAuth: " + challengeAuth);

		WWW AcceptChallengeAttempt = new WWW(GameControl.instance.RootURL + query);
		StartCoroutine(AcceptChallengeAttemptWaitForRequest(AcceptChallengeAttempt));
	}

	IEnumerator AcceptChallengeAttemptWaitForRequest(WWW www)
	{
		yield return www;

		// check for errors
		if (www.error == null) 
		{
			ChallengeData[] resultData = JsonHelper.FromJsonWrapped<ChallengeData> (www.text);

			// Debug.Log("AcceptChallengeAttemptWaitForRequest resultData.success: " + resultData[0].success);
			// Debug.Log("AcceptChallengeAttemptWaitForRequest resultData.resourceid: " + resultData[0].resourceid);

			if (OnChallengeAcceptedComplete != null) 
			{
				bool success = (resultData [0].success == "true") ? true : false;

				// Debug.Log("AcceptChallengeAttemptWaitForRequest success: " + success);

				OnChallengeAcceptedComplete (success, resultData [0]);
			}
		} 
		else 
		{
			// Debug.Log ("WWW Error: " + www.error);
		}
	}

	// IGNORE CHALLENGE
	// user declined challenge

	public void ChallengeIgnoredToServer()
	{
		string query = "challenge/IgnoreChallenge.php?t=8";
		query += "&a=" + challengeAuth;

		// print ("ChallengeAcceptToServer() challengeAuth: " + challengeAuth);

		WWW IgnoreChallengeAttempt = new WWW(GameControl.instance.RootURL + query);
		StartCoroutine(IgnoreChallengeAttemptWaitForRequest(IgnoreChallengeAttempt));
	}

	IEnumerator IgnoreChallengeAttemptWaitForRequest(WWW www)
	{
		yield return www;

		// check for errors
		if (www.error == null)
		{
			ChallengeData[] resultData = JsonHelper.FromJsonWrapped<ChallengeData>(www.text);

			//Debug.Log("IgnoreChallengeAttemptWaitForRequest resultData.success: " + resultData[0].success);
			//Debug.Log("IgnoreChallengeAttemptWaitForRequest resultData.resourceid: " + resultData[0].resourceid);
			//Debug.Log("IgnoreChallengeAttemptWaitForRequest resultData.friendignored: " + resultData[0].friendignored);

			if (OnChallengeIgnoredComplete != null) 
			{
				bool success = (resultData [0].success == "true") ? true : false;

				//Debug.Log("IgnoreChallengeAttemptWaitForRequest success: " + success);

				OnChallengeIgnoredComplete (success, resultData[0]);
			}
		} 
		else 
		{
			//Debug.Log ("WWW Error: " + www.error);
		}
	}

	// UPDATE SCORE CHALLENGE
	// 

	public void UpdateScoreChallengeToServer()
	{
		string query = "challenge/UpdateScoreChallenge.php?t=8";
		query += "&a=" + challengeAuth;
		query += "&s=" + currentscore.ToString();
		query += "&e=" + WWW.EscapeURL(email);

		// print ("UpdateScoreChallengeToServer() challengeAuth: " + challengeAuth);

		WWW UpdateScoreChallengeAttempt = new WWW(GameControl.instance.RootURL + query);
		StartCoroutine(UpdateScoreChallengeAttemptWaitForRequest(UpdateScoreChallengeAttempt));
	}

	IEnumerator UpdateScoreChallengeAttemptWaitForRequest(WWW www)
	{
		yield return www;

		// check for errors
		if (www.error == null)
		{
			ChallengeData[] resultData = JsonHelper.FromJsonWrapped<ChallengeData>(www.text);

			if (OnUpdateScoreChallengeComplete != null) 
			{
				bool success = (resultData [0].success == "true") ? true : false;

				OnUpdateScoreChallengeComplete (success, resultData[0]);
			}

			//Debug.Log("UpdateScoreChallengeAttemptWaitForRequest resultData.success: " + resultData[0].success);

		} 
		else
		{
			//Debug.Log ("WWW Error: " + www.error);
		}
	}


	// GET CHALLENGE LIST

	public void GetChallengesFromServer()
	{
		string query = "challenge/GetChallenges.php?t=8";
		query += "&e=" + WWW.EscapeURL(email);

		// print ("GetChallengesFromServer() email: " + email);

		WWW GetChallengesAttempt = new WWW(GameControl.instance.RootURL + query);
		StartCoroutine(GetChallengesAttemptWaitForRequest(GetChallengesAttempt));
	}

	IEnumerator GetChallengesAttemptWaitForRequest(WWW www)
	{
		yield return www;

		// check for errors
		if (www.error == null)
		{
			ChallengeData[] resultData = JsonHelper.FromJsonWrapped<ChallengeData>(www.text);

			bool success = (resultData.Length > 0) ? true : false;

			if (OnGetChallengesComplete != null) 
			{
				OnGetChallengesComplete (success, resultData);
			}

			//Debug.Log("GetChallengesAttemptWaitForRequest success: " + success);
			//Debug.Log("GetChallengesAttemptWaitForRequest resultData.Length: " + resultData.Length);

		} 
		else 
		{
			//Debug.Log ("WWW Error: " + www.error);
		}
	}

	// CLEAR COMPLETED CHALLENGES

	public void ClearChallengesToServer()
	{
		string query = "challenge/HideCompletedChallenges.php?t=8";
		query += "&e=" + email;

		// print ("GetChallengesFromServer() email: " + email);

		WWW ClearCompletedChallengesAttempt = new WWW(GameControl.instance.RootURL + query);
		StartCoroutine(ClearCompletedChallengesAttemptWaitForRequest(ClearCompletedChallengesAttempt));
	}

	IEnumerator ClearCompletedChallengesAttemptWaitForRequest(WWW www)
	{
		yield return www;

		// check for errors
		if (www.error == null)
		{
			ChallengeData[] resultData = JsonHelper.FromJsonWrapped<ChallengeData>(www.text);

			bool success = (resultData.Length > 0) ? true : false;

			if (OnClearChallengesComplete != null) 
			{
				OnClearChallengesComplete(success);
			}

			// Debug.Log("ClearCompletedChallengesAttemptWaitForRequest success: " + success);
		} 
		else 
		{
			//Debug.Log ("WWW Error: " + www.error);
		}
	}


	// GET CHALLENGE NOTIFICATIONS

	public void GetChallengeNotificationsFromServer()
	{
		string query = "challenge/GetNewChallengesByUser.php?t=8";
		query += "&e=" + GameControl.instance.email;

		WWW GetChallengeNotificationsAttempt = new WWW(GameControl.instance.RootURL + query);
		StartCoroutine(GetChallengeNotificationsAttemptWaitForRequest(GetChallengeNotificationsAttempt));
	}

	IEnumerator GetChallengeNotificationsAttemptWaitForRequest(WWW www)
	{
		yield return www;

		// check for errors
		if (www.error == null) 
		{
			// print("www.text: " + www.text);

			ChallengeData[] resultData = JsonHelper.FromJsonWrapped<ChallengeData> (www.text);

			/// Debug.Log("GetChallengeNotificationsAttemptWaitForRequest resultData.success: " + resultData[0].success);
			// Debug.Log("GetChallengeNotificationsAttemptWaitForRequest resultData.notifications: " + resultData[0].notifications);

			if (OnNotificationsComplete != null) 
			{
				bool success = (resultData [0].success == "true") ? true : false;

				// Debug.Log("GetChallengeNotificationsAttemptWaitForRequest success: " + success);

				OnNotificationsComplete(success, resultData[0]);
			}
		} 
		else 
		{
			// Debug.Log("WWW Error: "+ www.error);
		}	
	}


	// UPDATE CHALLENGE NOTIFICATIONS

	public void UpdateChallengeNotificationsToServer()
	{
		string query = "challenge/UpdateNotifications.php?t=8";
		query += "&e=" + GameControl.instance.email;

		WWW UpdateChallengeNotificationsAttempt = new WWW(GameControl.instance.RootURL + query);
		StartCoroutine(UpdateChallengeNotificationsAttemptWaitForRequest(UpdateChallengeNotificationsAttempt));
	}

	IEnumerator UpdateChallengeNotificationsAttemptWaitForRequest(WWW www)
	{
		yield return www;

		// check for errors
		if (www.error == null) 
		{
			// print("www.text: " + www.text);

			ChallengeData[] resultData = JsonHelper.FromJsonWrapped<ChallengeData> (www.text);

			// Debug.Log("UpdateChallengeNotificationsAttemptWaitForRequest resultData.success: " + resultData[0].success);

			if (OnUpdateNotificationsComplete != null) 
			{
				bool success = (resultData [0].success == "true") ? true : false;

				// Debug.Log("UpdateChallengeNotificationsAttemptWaitForRequest success: " + success);

				OnUpdateNotificationsComplete(success);
			}
		} 
		else 
		{
			// Debug.Log("WWW Error: "+ www.error);
		}	
	}

	// TOOLS

	public void SetActiveAllChildren(Transform transform, bool value)
	{
		foreach (Transform child in transform)
		{
			child.gameObject.SetActive(value);

			SetActiveAllChildren(child, value);
		}
	}

	public string FormatUserNameDisplay(string username, bool isUpperCase = true)
	{
		if (String.IsNullOrEmpty (username)) return String.Empty;

		string formattedName = "";
		if (username.Split (' ').Length == 1) formattedName = username;
		else 
		{
			formattedName = username.Split(' ')[0] + " " + username.Split(' ')[1].Substring(0, 1) + ".";
		}

		return (isUpperCase) ? formattedName.ToUpper() : formattedName;
	}
	
	public const string MatchEmailPattern =
            @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
            + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
              + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
            + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";
			
	public static bool IsEmail(string email)
	{
		if (email != null) return Regex.IsMatch(email, MatchEmailPattern);
		else return false;
	}
	
	
	// ALERT PANEL

	private AlertPanel _alertPanel;
	IEnumerator ShowAlertPanel(string title, string message = "")
	{
		//Debug.Log ("ShowAlertPanel: " + title);

		HomeScreenController home = GameObject.FindObjectOfType<HomeScreenController>();
		// stage.GetComponent<Canvas>().referencePixelsPerUnit = 326;

		yield return new WaitForSeconds(1.0f);

		yield return new WaitForEndOfFrame();

		if (String.IsNullOrEmpty(message)) 
		{
			message = "Your user information is incomplete.  Make sure you have your user profile updated.\n\nPlease close window and restart again. ";
		} 

		GameObject subScreen = (GameObject)Instantiate (Resources.Load ("HomeScreen/AlertPanel"));  

		yield return subScreen;

		_alertPanel = subScreen.GetComponent<AlertPanel>();
		_alertPanel.title.text = title;
		_alertPanel.description.text = message;
		_alertPanel.GetComponent<CanvasGroup>().alpha = 0; 
		_alertPanel.gameObject.transform.SetParent(home.transform, false);
		_alertPanel.GetComponent<FadeMe>().startFadeIn (0.6f);
	}

	void HideAlertPanel()
	{
		//print("HideAlertPanel ------------------------------------");

		DestroyImmediate(_alertPanel.gameObject);

		_alertPanel = null;
	}

}