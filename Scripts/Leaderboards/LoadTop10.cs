using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoadTop10 : MonoBehaviour {

	private string dataUrl = "LoadTop10.php?t=02032018&";

    void Start()
    {
        GameControl.instance.Leaderboardfilter = 1;
        
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

			if (users.Length > 0) {

				string[] Names = new string[users.Length];
				string[] Scores = new string[users.Length];
				for (int i = 0; i < users.Length; i++) {
					Names [i] = users [i].fullname;
					Scores [i] = users [i].score;
				}
				var Nameslist = new List<string> ((string[])Names);
				var Scorelist = new List<string> ((string[])Scores);

				GameControl.instance.Top10Nameslist = Nameslist;
				GameControl.instance.Top10Scorelist = Scorelist;
			} 

		} 
		else Debug.Log("GetTopScoresAttemptWaitForRequest WWW Error: "+ www.error);
	}
}