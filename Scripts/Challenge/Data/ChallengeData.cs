// We are using this for deserializing JSON Challenge data coming from the server.  

[System.Serializable]
public class ChallengeData
{
	public bool isUserChallenger;

	public string success;
	public int id;
    public string resourceid;
	public int userid;
	public string username;
	public string useremail;
	public int usercompleted;
    public int userscore;
	public int userhidden;
	public int friendid;
	public string friendemail;
	public string frienduser;
	public int friendaccepted;
	public int friendignored;
	public int friendcompleted;
	public int friendscore;
	public int friendhidden;
	public string auth;
	public string date;			// date on update
	public string creation; 	// date when created
	public string elapsed; 		// duration of time created (string)

	public int notifications;
}