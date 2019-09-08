using UnityEngine;
using System.Collections.Generic;


public class GetPlayerData : MonoBehaviour
{
    public static GetPlayerData instance;
    private static Dictionary<string, string> parameters = new Dictionary<string, string>();

    void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public static string GetValue(string key)
    {
		if (parameters.ContainsKey(key)) return parameters[key];

		return "";
    }
    
	public static void SetRequestParameters(string parametersString)
    {
		// Debug.Log("SetRequestParameters: " + parametersString);

		if (parametersString == "" && Application.isEditor && !GameControl.instance.isDevelopment)
        {
			if (GameControl.instance.isChangeUser) 
			{
				GetPlayerData.parameters.Add("fullname", "Rafael Emmanuelli");
				GetPlayerData.parameters.Add("email", "rafael.emmanuelli@sweetrush.com");
				GetPlayerData.parameters.Add("profile_field_region", "SweetRush");
				GetPlayerData.parameters.Add("profile_field_stateprovince", "SR");
				// GetPlayerData.parameters.Add("auth", "a2a789176460abea819b8cd1a080b890");
			} 
			else 
			{
				GetPlayerData.parameters.Add("fullname", "Hernan Emmanuelli");
				GetPlayerData.parameters.Add("email", "fania4life@gmail.com");
				GetPlayerData.parameters.Add("profile_field_region", "SweetRush");
				GetPlayerData.parameters.Add("profile_field_stateprovince", "SR");
			}
            return;
        }

        char[] parameterDelimiters = new char[] { '?', '&' };
        string[] parameters = parametersString.Split(parameterDelimiters, System.StringSplitOptions.RemoveEmptyEntries);

        // Debug.Log("parameters.Length: " + parameters.Length);

        if (parameters.Length == 0 || parameters.Length == 1)
        {
            //GetPlayerData.parameters.Add("fullname", "Guest");
            //GetPlayerData.parameters.Add("email", "guest@bfusa.com");
			//GetPlayerData.parameters.Add("profile_field_region", "GuestRegion");
			//GetPlayerData.parameters.Add("profile_field_stateprovince", "GuestOrg");
            return;
        }

        char[] keyValueDelimiters = new char[] { '=' };
        for (int i = 0; i < parameters.Length; ++i)
        {
            string[] keyValue = parameters[i].Split(keyValueDelimiters, System.StringSplitOptions.None);

            if (keyValue.Length >= 2)
            {
                GetPlayerData.parameters.Add(WWW.UnEscapeURL(keyValue[0]), WWW.UnEscapeURL(keyValue[1]));
            }
            else if (keyValue.Length == 1)
            {
                GetPlayerData.parameters.Add(WWW.UnEscapeURL(keyValue[0]), "");
            }
        }
    }

    public static void GetURLData()
	{
		//Debug.Log("***************************** GetPlayerData.GetURLData()");

		if (!parameters.ContainsKey("fullname")) SetRequestParameters(GameControl.instance.URL);

        string NameValue = GetValue("fullname");
		string EmailValue = GetValue("email");
		string RegionValue = GetValue("profile_field_region");
		string OrgValue = GetValue("profile_field_stateprovince");
		string ChallengeAuth = GetValue("auth");
		string ChallengeAccepted = GetValue("accepted_challenge");

		// When user clicks play in the email, we pass in a challenge token 
		// We can test by setting the token and bool
		// You will need the token for the challenge...
		// ChallengeAuth = "a951423f0e760407ccca78e94ae67f4d";
		// ChallengeAccepted = "1";

		// Check if we have a challenge
		if (ChallengeAuth.Length > 1) 
		{
			// print ("Challenge Auth: " + ChallengeAuth);
			// print ("Challenge Auth: " + ChallengeAccepted);
			GameControl.instance.challengeAuth = ChallengeAuth;
			GameControl.instance.challengeAccepted = (ChallengeAccepted == "1") ? true : false;
		}

        GameControl.instance.username = NameValue;
        GameControl.instance.email = EmailValue;
	
		if (RegionValue.Length > 1) GameControl.instance.region = RegionValue;
		if (OrgValue.Length > 1) GameControl.instance.org = OrgValue;

		string isRandom = GetValue("rand");
		if (isRandom.Length > 1 && isRandom == "false") GameControl.instance.isRandomized = false;

		string isMaxQuestions = GetValue("max");
		if (isMaxQuestions.Length > 1 && isMaxQuestions == "false") GameControl.instance.isMaxQuestions = false;

		string isTimer = GetValue("timer");
		if (isTimer.Length > 1 && isTimer == "false") GameControl.instance.isTimer = false;
    }
}