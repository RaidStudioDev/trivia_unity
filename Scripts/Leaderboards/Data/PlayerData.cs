// We are using this for deserializing JSON data coming from the server.  
// We will store the user's game data [ fullname, score, region, org ], to display in leaderboards

[System.Serializable]
public class PlayerData
{
	public string id;
    public string userid;
    public string score;
    public string fullname;
    public string region;
    public string org;
	public string email;
	public string rank;
}