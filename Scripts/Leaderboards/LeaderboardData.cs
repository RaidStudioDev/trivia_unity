using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LeaderboardData : MonoBehaviour
{
	private string dataUrl = "LoadTop10.php?t=02032018&";

    public Canvas Canvasname;
    public GameObject BaseText;

    public Toggle NationalButton;
    public Toggle TopButton;
    public Toggle RegionButton;
    public Toggle StoreButton;
    public Color buttongrey = new Color(.882f, .882f, .882f, 1f);

    public TitlePanel titlePanel;
    public DataColumnsPanel data;

    private RankPanel rankPanel;
    private NamePanel namePanel;
    private ScorePanel scorePanel;

    private Color theRedColor = new Vector4(255.0f / 255.0f, 0f, 0f, 1);

    private GameObject[] listRankItems;
    private GameObject[] listNamesItems;
    private GameObject[] listScoreItems;

    private bool isFirstTimeLoaded = false;

    // Use this for initialization
    void Start()
    {
        rankPanel = data.GetComponentInChildren<RankPanel>();
        namePanel = data.GetComponentInChildren<NamePanel>();
        scorePanel = data.GetComponentInChildren<ScorePanel>();

        GameControl.instance.Leaderboardfilter = 1;

        updateTitleHeaderFields();

        LoadTopScores();
    }

    public void LoadTopScores()
    {
		WWW GetTopScoresAttempt = new WWW(GameControl.instance.RootURL + dataUrl);
		StartCoroutine(GetTopScoresAttemptWaitForRequest(GetTopScoresAttempt));
    }

	IEnumerator GetTopScoresAttemptWaitForRequest(WWW www)
	{
		yield return www;

		// check for errors
		if (www.error == null)
		{
			// Debug.Log("GetTopScoresAttemptWaitForRequest Ok!: " + www.text);

			PlayerData[] users = JsonHelper.FromJsonWrapped<PlayerData> (www.text);

			if (users.Length > 0)
			{
				string[] Names = new string[users.Length];
				string[] Scores = new string[users.Length];
				for (int i = 0; i < users.Length; i++)
				{
					Names [i] = users [i].fullname;
					Scores [i] = users [i].score;
				}
				var Nameslist = new List<string>((string[])Names);
				var Scorelist = new List<string>((string[])Scores);

				GameControl.instance.Top10Nameslist = Nameslist;
				GameControl.instance.Top10Scorelist = Scorelist;

				updateLeaderboard();
			}
		} 
		else Debug.Log("WWW Error: "+ www.error);
	}

    private void updateTitleHeaderFields()
    {
        Text titlePanelText = titlePanel.GetComponentInChildren<Text>();

        switch (GameControl.instance.Leaderboardfilter)
        {
            case 1:
                titlePanelText.text = "TOP PLAYERS";
                break;

            case 2:
                titlePanelText.text = "NATIONAL";
                break;

            case 3:
                titlePanelText.text = "REGION";
                break;

            case 4:
                titlePanelText.text = "STATE";
                break;
        }
    }

    public void updateLeaderboard()
    {
        Text titlePanelText = titlePanel.GetComponentInChildren<Text>();

        switch (GameControl.instance.Leaderboardfilter)
        {
            case 1:
				titlePanelText.text = "TOP PLAYERS";
                populateTopTenList();
                break;

            case 2:
				titlePanelText.text = "NATIONAL";
                populateNationalList();
                break;

            case 3:
				titlePanelText.text = "REGION";
                populateTopRegionList();
                break;

            case 4:
				titlePanelText.text = "STATE";
                populateOrgList();
                break;
        }
    }

    private void populateTopTenList()
    {
        listRankItems = new GameObject[GameControl.instance.Top10Scorelist.Count];
        listNamesItems = new GameObject[GameControl.instance.Top10Scorelist.Count];
        listScoreItems = new GameObject[GameControl.instance.Top10Scorelist.Count];

        for (int i = 0; i < GameControl.instance.Top10Scorelist.Count; i++)
        {
            GameObject Rank = Instantiate(BaseText) as GameObject;
            Rank.GetComponent<Text>().text = (i + 1).ToString();
            Rank.transform.SetParent(rankPanel.transform, false);

            GameObject Name = Instantiate(BaseText) as GameObject;
			Name.GetComponent<Text>().text = (GameControl.instance.Top10Nameslist[i]).ToUpper();
            Name.transform.SetParent(namePanel.transform, false);

            GameObject Score = Instantiate(BaseText) as GameObject;
			Score.GetComponent<Text>().text = int.Parse(GameControl.instance.Top10Scorelist[i]).ToString("n0");
            Score.transform.SetParent(scorePanel.transform, false);

            listRankItems[i] = Rank;
            listNamesItems[i] = Name;
            listScoreItems[i] = Score;

            // Color player score.
			if (Name.GetComponent<Text> ().text == GameControl.instance.username) {
				Rank.GetComponent<Text> ().color = theRedColor;
				Rank.GetComponent<Text> ().fontSize = 50;

				Name.GetComponent<Text> ().color = theRedColor;
				Name.GetComponent<Text> ().fontSize = 50;

				Score.GetComponent<Text> ().color = theRedColor;
				Score.GetComponent<Text> ().fontSize = 50;
			} 
			else 
			{
				// Color First Place score.
				if (i == 0)
				{
					Rank.GetComponent<Text>().fontSize = 50;
					Name.GetComponent<Text>().fontSize = 50;
					Score.GetComponent<Text>().fontSize = 50;
				}
			}
        }

        if (!isFirstTimeLoaded)
        {
            isFirstTimeLoaded = true;

            data.gameObject.AddComponent<FadeMe>();
            data.gameObject.GetComponentInChildren<FadeMe>().startFadeIn(0.75f);
        }
    }

    private void populateNationalList()
    {
        // Check if Player Rank is higher than 10
        // If Player is less then 10, this means Player is in Top Ten List
        if (GameControl.instance.UserRank < 10)
        {
            populateTopTenList();
        }
        else
        {
            listRankItems = new GameObject[GameControl.instance.ScoresRangeList.Count];
            listNamesItems = new GameObject[GameControl.instance.PlayersRangeList.Count];
            listScoreItems = new GameObject[GameControl.instance.ScoresRangeList.Count];

            // List Scores by Range
            for (int i = 0; i < GameControl.instance.ScoresRangeList.Count; i++)
            {
                GameObject Rank = Instantiate(BaseText) as GameObject;
                // Rank.GetComponent<Text>().text = (i + 1).ToString();
                Rank.GetComponent<Text>().text = ((GameControl.instance.UserRank - 5) + (i + 1)).ToString();
                Rank.transform.SetParent(rankPanel.transform, false);

                GameObject Name = Instantiate(BaseText) as GameObject;
				Name.GetComponent<Text>().text = (GameControl.instance.PlayersRangeList[i]).ToUpper();
                Name.transform.SetParent(namePanel.transform, false);

                GameObject Score = Instantiate(BaseText) as GameObject;
                Score.GetComponent<Text>().text = int.Parse(GameControl.instance.ScoresRangeList[i]).ToString("n0");
                Score.transform.SetParent(scorePanel.transform, false);

                listRankItems[i] = Rank;
                listNamesItems[i] = Name;
                listScoreItems[i] = Score;

		        // Color player score.
				if (GameControl.instance.PlayersRangeList[i] == GameControl.instance.username)
                {
                    Rank.GetComponent<Text>().color = theRedColor;
                    Rank.GetComponent<Text>().fontSize = 50;

                    Name.GetComponent<Text>().color = theRedColor;
                    Name.GetComponent<Text>().fontSize = 50;

                    Score.GetComponent<Text>().color = theRedColor;
                    Score.GetComponent<Text>().fontSize = 50;
                }
            }
        }
    }

    private void populateTopRegionList()
    {
        listRankItems = new GameObject[GameControl.instance.ScoresRegionRangeList.Count];
        listNamesItems = new GameObject[GameControl.instance.PlayersRegionRangeList.Count];
        listScoreItems = new GameObject[GameControl.instance.ScoresRegionRangeList.Count];

        for (int i = 0; i < GameControl.instance.ScoresRegionRangeList.Count; i++)
        {
            GameObject Rank = Instantiate(BaseText) as GameObject;
            if (GameControl.instance.UserRegionRank < 10) Rank.GetComponent<Text>().text = (i + 1).ToString();
            else Rank.GetComponent<Text>().text = ((GameControl.instance.UserRegionRank - 5) + (i + 1)).ToString();
            Rank.transform.SetParent(rankPanel.transform, false);

            GameObject Name = Instantiate(BaseText) as GameObject;
			Name.GetComponent<Text>().text = (GameControl.instance.PlayersRegionRangeList[i]).ToUpper();
            Name.transform.SetParent(namePanel.transform, false);

            GameObject Score = Instantiate(BaseText) as GameObject;
			Score.GetComponent<Text>().text = int.Parse(GameControl.instance.ScoresRegionRangeList[i]).ToString("n0");
		    Score.transform.SetParent(scorePanel.transform, false);

            listRankItems[i] = Rank;
            listNamesItems[i] = Name;
            listScoreItems[i] = Score;

            // Color player score.
			if (GameControl.instance.PlayersRegionRangeList[i] == GameControl.instance.username)
            {
                Rank.GetComponent<Text>().color = theRedColor;
                Rank.GetComponent<Text>().fontSize = 50;

                Name.GetComponent<Text>().color = theRedColor;
                Name.GetComponent<Text>().fontSize = 50;

                Score.GetComponent<Text>().color = theRedColor;
                Score.GetComponent<Text>().fontSize = 50;
            }
        }
    }

    private void populateOrgList()
    {
        listRankItems = new GameObject[GameControl.instance.ScoresOrgRangeList.Count];
        listNamesItems = new GameObject[GameControl.instance.ScoresOrgRangeList.Count];
        listScoreItems = new GameObject[GameControl.instance.ScoresOrgRangeList.Count];

        for (int i = 0; i < GameControl.instance.ScoresOrgRangeList.Count; i++)
        {
            GameObject Rank = Instantiate(BaseText) as GameObject;
            if (GameControl.instance.UserOrgRank < 10) Rank.GetComponent<Text>().text = (i + 1).ToString();
            else Rank.GetComponent<Text>().text = ((GameControl.instance.UserOrgRank - 5) + (i + 1)).ToString();
            Rank.transform.SetParent(rankPanel.transform, false);

            GameObject Name = Instantiate(BaseText) as GameObject;
			Name.GetComponent<Text>().text = (GameControl.instance.PlayersOrgRangeList[i]).ToUpper();
            Name.transform.SetParent(namePanel.transform, false);

            GameObject Score = Instantiate(BaseText) as GameObject;
			Score.GetComponent<Text>().text = int.Parse(GameControl.instance.ScoresOrgRangeList[i]).ToString("n0");
		    Score.transform.SetParent(scorePanel.transform, false);

            listRankItems[i] = Rank;
            listNamesItems[i] = Name;
            listScoreItems[i] = Score;

            // Color player score.
			if (GameControl.instance.PlayersOrgRangeList[i] == GameControl.instance.username)
            {
                Rank.GetComponent<Text>().color = theRedColor;
                Rank.GetComponent<Text>().fontSize = 50;

                Name.GetComponent<Text>().color = theRedColor;
                Name.GetComponent<Text>().fontSize = 50;

                Score.GetComponent<Text>().color = theRedColor;
                Score.GetComponent<Text>().fontSize = 50;
            }
        }

    }

    // Button Events 

    public void Top10()
    {
		GameControl.instance.soundManager.PlayLowToneButton();

        Destroytext();
        GameControl.instance.Leaderboardfilter = 1;
        NationalButton.GetComponentInChildren<Text>().color = Color.black;
        TopButton.GetComponentInChildren<Text>().color = Color.white;
        StoreButton.GetComponentInChildren<Text>().color = Color.black;
        RegionButton.GetComponentInChildren<Text>().color = Color.black;

        updateLeaderboard();
    }

    public void National()
    {
		GameControl.instance.soundManager.PlayLowToneButton();

        Destroytext();
        GameControl.instance.Leaderboardfilter = 2;
        NationalButton.GetComponentInChildren<Text>().color = Color.white;
        TopButton.GetComponentInChildren<Text>().color = Color.black;
        StoreButton.GetComponentInChildren<Text>().color = Color.black;
        RegionButton.GetComponentInChildren<Text>().color = Color.black;

        updateLeaderboard();
    }

    public void Region()
    {
		GameControl.instance.soundManager.PlayLowToneButton();

        Destroytext();
        GameControl.instance.Leaderboardfilter = 3;
        NationalButton.GetComponentInChildren<Text>().color = Color.black;
        TopButton.GetComponentInChildren<Text>().color = Color.black;
        StoreButton.GetComponentInChildren<Text>().color = Color.black;
        RegionButton.GetComponentInChildren<Text>().color = Color.white;

        updateLeaderboard();
    }

    public void Store()
    {
		GameControl.instance.soundManager.PlayLowToneButton();

        Destroytext();
        GameControl.instance.Leaderboardfilter = 4;
        NationalButton.GetComponentInChildren<Text>().color = Color.black;
        TopButton.GetComponentInChildren<Text>().color = Color.black;
        StoreButton.GetComponentInChildren<Text>().color = Color.white;
        RegionButton.GetComponentInChildren<Text>().color = Color.black;

        updateLeaderboard();
    }

    void Destroytext()
    {
        if (listRankItems == null) return;

        // Debug.Log("Length: " + listRankItems.Length);

        for (int i = listRankItems.Length - 1; i >= 0; i--)
        {
            // Debug.Log("Count: " + i.ToString());
            Destroy(listRankItems[i]);
            Destroy(listNamesItems[i]);
            Destroy(listScoreItems[i]);
        }

        // re-initialize to clear
        listRankItems = new GameObject[1];
        listNamesItems = new GameObject[1];
        listScoreItems = new GameObject[1];
    }
}
