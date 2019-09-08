using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoadPlayerRange : MonoBehaviour
{
   	private string rankUrl = "LoadRank.php?"; 							
	private string regionRankUrl = "LoadRegionRank.php?"; 				
	private string orgRankUrl = "LoadOrgRank.php?"; 					
	private string rangeUrl = "GetRange.php?"; 							
	private string regionRangeUrl = "GetRangeRegion.php?"; 				
	private string orgRangeUrl = "GetRangeOrg.php?"; 

    void Start()
    {
		GetRank();
		GetRegionRank();
		GetOrgRank();
		LoadRangeScores();
		LoadRegionRangeScores();
		LoadOrgRangeScores();
    }

    public void GetLeaderboardData()
    {
		// Does this function get called??
		// Debug.Log("LoadPlayerRange.GetLeaderboardData()");

		GetRank();
		GetRegionRank();
		GetOrgRank();
		LoadRangeScores();
		LoadRegionRangeScores();
		LoadOrgRangeScores();
    }

    void GetRank()
    {
        var data1 = GameControl.instance.email;

 		WWW RankGrabAttempt = new WWW(GameControl.instance.RootURL + rankUrl + "&email=" + WWW.EscapeURL(data1));
		StartCoroutine(RankGrabAttemptWaitForRequest(RankGrabAttempt));
    }

	IEnumerator RankGrabAttemptWaitForRequest(WWW www)
	{
		yield return www;

		// check for errors
		if (www.error == null)
		{
			// Debug.Log("RankGrabAttemptWaitForRequest Ok!: " + www.text);

			PlayerData[] rank = JsonHelper.FromJsonWrapped<PlayerData> (www.text);

			if (rank.Length > 0) GameControl.instance.UserRank = int.Parse(rank[0].rank);

			// Debug.Log("GrabRank().GameControl.instance.UserRank:" + GameControl.instance.UserRank);
		} 
		else Debug.Log("RankGrabAttemptWaitForRequest WWW Error: "+ www.error);
	}

    void GetRegionRank()
    {
        var data1 = GameControl.instance.email;
        var data2 = GameControl.instance.region;

		WWW RankRegionGrabAttempt = new WWW(GameControl.instance.RootURL + regionRankUrl + "&email=" + WWW.EscapeURL(data1) + "&region=" + WWW.EscapeURL(data2));
		StartCoroutine(RankRegionGrabAttemptWaitForRequest(RankRegionGrabAttempt));
    }

	IEnumerator RankRegionGrabAttemptWaitForRequest(WWW www)
	{
		yield return www;

		// check for errors
		if (www.error == null)
		{
			// Debug.Log("RankRegionGrabAttemptWaitForRequest Ok!: " + www.text);

			PlayerData[] rank = JsonHelper.FromJsonWrapped<PlayerData> (www.text);

			if (rank.Length > 0) GameControl.instance.UserRegionRank = int.Parse(rank[0].rank);

			// Debug.Log("GrabRegionRank().GameControl.instance.UserRegionRank:" + GameControl.instance.UserRegionRank);
		} 
		else Debug.Log("RankRegionGrabAttemptWaitForRequest WWW Error: "+ www.error);
	}

    void GetOrgRank()
    {
        var data1 = GameControl.instance.email;
        var data2 = GameControl.instance.org;

  		WWW RankOrgGrabAttempt = new WWW(GameControl.instance.RootURL + orgRankUrl + "&email=" + WWW.EscapeURL(data1) + "&org=" + WWW.EscapeURL(data2));
		StartCoroutine(RankOrgGrabAttemptWaitForRequest(RankOrgGrabAttempt));
    }

	IEnumerator RankOrgGrabAttemptWaitForRequest(WWW www)
	{
		yield return www;

		// check for errors
		if (www.error == null)
		{
			// Debug.Log("RankOrgGrabAttemptWaitForRequest Ok!: " + www.text);

			PlayerData[] rank = JsonHelper.FromJsonWrapped<PlayerData> (www.text);

			if (rank.Length > 0) GameControl.instance.UserOrgRank = int.Parse(rank[0].rank);

			// Debug.Log("GetOrgRank().GameControl.instance.UserRegionRank:" + GameControl.instance.UserOrgRank);
		} 
		else Debug.Log("RankOrgGrabAttemptWaitForRequest WWW Error: "+ www.error);
	}

    void LoadRangeScores()
    {
        var data1 = GameControl.instance.email;

		WWW GetRangeAttempt = new WWW(GameControl.instance.RootURL + rangeUrl + "&email=" + WWW.EscapeURL(data1));
		StartCoroutine(GetRangeAttemptWaitForRequest(GetRangeAttempt));
    }

	IEnumerator GetRangeAttemptWaitForRequest(WWW www)
	{
		yield return www;

		// check for errors
		if (www.error == null)
		{
			// Debug.Log("GetRangeAttemptWaitForRequest Ok!: " + www.text);

			PlayerData[] users = JsonHelper.FromJsonWrapped<PlayerData> (www.text);

			if (users.Length > 0) 
			{
				string[] NamesRangeList = new string[users.Length];
				string[] ScoresRangeList = new string[users.Length];

				for (int i = 0; i < users.Length; i++) 
				{
					NamesRangeList [i] = users [i].fullname;
					ScoresRangeList [i] = users [i].score;
				}

				var Nameslist = new List<string> ((string[])NamesRangeList);
				var Scorelist = new List<string> ((string[])ScoresRangeList);

				GameControl.instance.PlayersRangeList = Nameslist;
				GameControl.instance.ScoresRangeList = Scorelist;
			} 

			// Debug.Log("LoadRangeScores().GameControl.instance.PlayersRangeList: " + GameControl.instance.PlayersRangeList.Count);
		} 
		else Debug.Log("GetRangeAttemptWaitForRequest WWW Error: "+ www.error);
	}

    void LoadRegionRangeScores()
    {
        var data1 = GameControl.instance.email;
        var data2 = GameControl.instance.region;

    	WWW GetRegionRangeAttempt = new WWW(GameControl.instance.RootURL + regionRangeUrl + "&email=" + WWW.EscapeURL(data1) + "&region=" + WWW.EscapeURL(data2));
		StartCoroutine(GetRegionRangeAttemptWaitForRequest(GetRegionRangeAttempt));
    }

	IEnumerator GetRegionRangeAttemptWaitForRequest(WWW www)
	{
		yield return www;

		// check for errors
		if (www.error == null)
		{
			// Debug.Log("GetRegionRangeAttemptWaitForRequest Ok!: " + www.text);

			PlayerData[] users = JsonHelper.FromJsonWrapped<PlayerData> (www.text);

			if (users.Length > 0) 
			{
				string[] NamesRegionRangeList = new string[users.Length];
				string[] ScoresRegionRangeList = new string[users.Length];

				for (int i = 0; i < users.Length; i++) 
				{
					NamesRegionRangeList [i] = users [i].fullname;
					ScoresRegionRangeList [i] = users [i].score;
				}

				var Nameslist = new List<string> ((string[])NamesRegionRangeList);
				var Scorelist = new List<string> ((string[])ScoresRegionRangeList);

				GameControl.instance.PlayersRegionRangeList = Nameslist;
				GameControl.instance.ScoresRegionRangeList = Scorelist;
			} 

			// Debug.Log("LoadRegionRangeScores().GameControl.instance.PlayersRegionRangeList: " + GameControl.instance.PlayersRegionRangeList.Count);
		} 
		else Debug.Log("GetRegionRangeAttemptWaitForRequest WWW Error: "+ www.error);
	}

    void LoadOrgRangeScores()
    {
        var data1 = GameControl.instance.email;
        var data2 = GameControl.instance.org;
      
		WWW GetOrgRangeAttempt = new WWW(GameControl.instance.RootURL + orgRangeUrl + "&email=" + WWW.EscapeURL(data1) + "&org=" + WWW.EscapeURL(data2));
		StartCoroutine(GetOrgRangeAttemptWaitForRequest(GetOrgRangeAttempt));
    }

	IEnumerator GetOrgRangeAttemptWaitForRequest(WWW www)
	{
		yield return www;

		// check for errors
		if (www.error == null)
		{
			// Debug.Log("GetOrgRangeAttemptWaitForRequest Ok!: " + www.text);

			PlayerData[] users = JsonHelper.FromJsonWrapped<PlayerData> (www.text);

			if (users.Length > 0) 
			{
				string[] NamesOrgRangeList = new string[users.Length];
				string[] ScoresOrgRangeList = new string[users.Length];

				for (int i = 0; i < users.Length; i++) 
				{
					NamesOrgRangeList [i] = users [i].fullname;
					ScoresOrgRangeList [i] = users [i].score;
				}

				var Nameslist = new List<string> ((string[])NamesOrgRangeList);
				var Scorelist = new List<string> ((string[])ScoresOrgRangeList);

				GameControl.instance.PlayersOrgRangeList = Nameslist;
				GameControl.instance.ScoresOrgRangeList = Scorelist;
			} 

			// Debug.Log("LoadOrgRangeScores().GameControl.instance.PlayersOrgRangeList: " + GameControl.instance.PlayersOrgRangeList.Count);
		} 
		else Debug.Log("GetOrgRangeAttemptWaitForRequest WWW Error: "+ www.error);
	}
}