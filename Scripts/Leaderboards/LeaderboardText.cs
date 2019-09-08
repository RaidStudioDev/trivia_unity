using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class LeaderboardText : MonoBehaviour
{


    public Canvas Canvasname;
    public GameObject BaseText;

    public Button NationalButton;
    public Button TopButton;
    public Button RegionButton;
    public Button StoreButton;
    public Color buttongrey = new Color(.882f, .882f, .882f, 1f);

    private int Highrank;

    void Start()
    {
        if (GameControl.instance != null) Top10();
    }

    public void National()
    {
        Destroytext();
        GameControl.instance.Leaderboardfilter = 2;
        NationalButton.GetComponent<Image>().color = Color.red;
        NationalButton.GetComponentInChildren<Text>().color = Color.white;
        TopButton.GetComponent<Image>().color = buttongrey;
        TopButton.GetComponentInChildren<Text>().color = Color.black;
        StoreButton.GetComponent<Image>().color = buttongrey;
        StoreButton.GetComponentInChildren<Text>().color = Color.black;
        RegionButton.GetComponent<Image>().color = buttongrey;
        RegionButton.GetComponentInChildren<Text>().color = Color.black;
        updateLeaderboard();
    }

    public void Top10()
    {
        Destroytext();
        GameControl.instance.Leaderboardfilter = 1;
        NationalButton.GetComponent<Image>().color = buttongrey;
        NationalButton.GetComponentInChildren<Text>().color = Color.black;
        TopButton.GetComponent<Image>().color = Color.red;
        TopButton.GetComponentInChildren<Text>().color = Color.white;
        StoreButton.GetComponent<Image>().color = buttongrey;
        StoreButton.GetComponentInChildren<Text>().color = Color.black;
        RegionButton.GetComponent<Image>().color = buttongrey;
        RegionButton.GetComponentInChildren<Text>().color = Color.black;
        updateLeaderboard();
    }

    public void Region()
    {
        Destroytext();
        GameControl.instance.Leaderboardfilter = 3;
        NationalButton.GetComponent<Image>().color = buttongrey;
        NationalButton.GetComponentInChildren<Text>().color = Color.black;
        TopButton.GetComponent<Image>().color = buttongrey;
        TopButton.GetComponentInChildren<Text>().color = Color.black;
        StoreButton.GetComponent<Image>().color = buttongrey;
        StoreButton.GetComponentInChildren<Text>().color = Color.black;
        RegionButton.GetComponent<Image>().color = Color.red;
        RegionButton.GetComponentInChildren<Text>().color = Color.white;
        updateLeaderboard();
    }

    public void Store()
    {
        Destroytext();
        GameControl.instance.Leaderboardfilter = 4;
        NationalButton.GetComponent<Image>().color = buttongrey;
        NationalButton.GetComponentInChildren<Text>().color = Color.black;
        TopButton.GetComponent<Image>().color = buttongrey;
        TopButton.GetComponentInChildren<Text>().color = Color.black;
        StoreButton.GetComponent<Image>().color = Color.red;
        StoreButton.GetComponentInChildren<Text>().color = Color.white;
        RegionButton.GetComponent<Image>().color = buttongrey;
        RegionButton.GetComponentInChildren<Text>().color = Color.black;
        updateLeaderboard();
    }

    void Destroytext()
    {
        var objectstodestroy = GameObject.FindGameObjectsWithTag("GUItextobject");
        for (var i = 0; i < objectstodestroy.Length; i++)
        {
            Destroy(objectstodestroy[i]);
        }

    }

    public void updateLeaderboard()
    {
        if (GameControl.instance.Leaderboardfilter == 1)
        {
            Vector3 LeftTextPosition = new Vector3(-280, 165, 0);
            Vector3 RightTextPosition = new Vector3(280, 165, 0);
            Vector3 CenterTextPosition = new Vector3(0, 165, 0);

            GameObject Top10header = Instantiate(BaseText) as GameObject;
            Top10header.transform.SetParent(Canvasname.transform, false);
            Top10header.transform.localPosition = new Vector3(0, 265, 0);
            Top10header.GetComponent<Text>().text = "Top Players";
            //Top10header.GetComponent<Text>().anchor = TextAnchor.MiddleCenter;
            Top10header.GetComponent<Text>().fontSize = 50;

            GameObject Scoreheader = Instantiate(BaseText) as GameObject;
            Scoreheader.transform.SetParent(Canvasname.transform, false);
            Scoreheader.GetComponent<Text>().text = "SCORE";
            Scoreheader.transform.localPosition = RightTextPosition;
            Scoreheader.GetComponent<Text>().color = Color.red;
            //Scoreheader.GetComponent<Text>().anchor = TextAnchor.MiddleCenter;
            Scoreheader.GetComponent<Text>().fontSize = 45;

            GameObject Nameheader = Instantiate(BaseText) as GameObject;
            Nameheader.transform.SetParent(Canvasname.transform, false);
            Nameheader.transform.localPosition = CenterTextPosition;
            Nameheader.GetComponent<Text>().text = "NAME";
            Nameheader.GetComponent<Text>().color = Color.red;
            //Nameheader.GetComponent<Text>().anchor = TextAnchor.MiddleCenter;
            Nameheader.GetComponent<Text>().fontSize = 45;

            GameObject Rankheader = Instantiate(BaseText) as GameObject;
            Rankheader.transform.SetParent(Canvasname.transform, false);
            Rankheader.transform.localPosition = LeftTextPosition;
            Rankheader.GetComponent<Text>().text = "RANK";
            Rankheader.GetComponent<Text>().color = Color.red;
            //Rankheader.GetComponent<Text>().anchor = TextAnchor.MiddleCenter;
            Rankheader.GetComponent<Text>().fontSize = 45;

            LeftTextPosition -= new Vector3(0, 100f, 0);
            RightTextPosition -= new Vector3(0, 100f, 0);
            CenterTextPosition -= new Vector3(0, 100f, 0);

            for (int i = 0; i < GameControl.instance.Top10Scorelist.Count; i++)
            {
                GameObject Score = Instantiate(BaseText) as GameObject;
                Score.transform.SetParent(Canvasname.transform, false);
                Score.transform.localPosition = RightTextPosition;
                Score.GetComponent<Text>().text = (GameControl.instance.Top10Scorelist[i]);
                Score.GetComponent<Text>().fontSize = 30;
                //Score.GetComponent<Text>().anchor = TextAnchor.MiddleCenter;

                GameObject Name = Instantiate(BaseText) as GameObject;
                Name.transform.SetParent(Canvasname.transform, false);
                Name.transform.localPosition = CenterTextPosition;
                Name.GetComponent<Text>().text = (GameControl.instance.Top10Nameslist[i]);
                Name.GetComponent<Text>().fontSize = 30;
                //Name.GetComponent<Text>().anchor = TextAnchor.MiddleCenter;

                GameObject Rank = Instantiate(BaseText) as GameObject;
                Rank.transform.SetParent(Canvasname.transform, false);
                Rank.transform.localPosition = LeftTextPosition;
                Rank.GetComponent<Text>().text = "" + (i + 1);
                Rank.GetComponent<Text>().fontSize = 30;
                //Rank.GetComponent<Text>().anchor = TextAnchor.MiddleCenter;

                if (Name.GetComponent<Text>().text == GameControl.instance.username) //Color player score.
                {
                    Score.GetComponent<Text>().color = Color.red;
                    Name.GetComponent<Text>().color = Color.red;
                    Rank.GetComponent<Text>().color = Color.red;
                }

                LeftTextPosition -= new Vector3(0, 60f, 0);
                RightTextPosition -= new Vector3(0, 60f, 0);
                CenterTextPosition -= new Vector3(0, 60f, 0);

            }
        }

        else if (GameControl.instance.Leaderboardfilter == 2)
        {
            Vector3 LeftTextPosition = new Vector3(-280, 165, 0);
            Vector3 RightTextPosition = new Vector3(280, 165, 0);
            Vector3 CenterTextPosition = new Vector3(0, 165, 0);


            GameObject Top10header = Instantiate(BaseText) as GameObject;
            Top10header.transform.SetParent(Canvasname.transform, false);
            Top10header.transform.localPosition = new Vector3(0, 265, 0);
            Top10header.GetComponent<Text>().text = "National";
            //Top10header.GetComponent<Text>().anchor = TextAnchor.MiddleCenter;
            Top10header.GetComponent<Text>().fontSize = 50;

            GameObject Scoreheader = Instantiate(BaseText) as GameObject;
            Scoreheader.transform.SetParent(Canvasname.transform, false);
            Scoreheader.GetComponent<Text>().text = "SCORE";
            Scoreheader.transform.localPosition = RightTextPosition;
            Scoreheader.GetComponent<Text>().color = Color.red;
            //Scoreheader.GetComponent<Text>().anchor = TextAnchor.MiddleCenter;
            Scoreheader.GetComponent<Text>().fontSize = 45;

            GameObject Nameheader = Instantiate(BaseText) as GameObject;
            Nameheader.transform.SetParent(Canvasname.transform, false);
            Nameheader.transform.localPosition = CenterTextPosition;
            Nameheader.GetComponent<Text>().text = "NAME";
            Nameheader.GetComponent<Text>().color = Color.red;
            //Nameheader.GetComponent<Text>().anchor = TextAnchor.MiddleCenter;
            Nameheader.GetComponent<Text>().fontSize = 45;

            GameObject Rankheader = Instantiate(BaseText) as GameObject;
            Rankheader.transform.SetParent(Canvasname.transform, false);
            Rankheader.transform.localPosition = LeftTextPosition;
            Rankheader.GetComponent<Text>().text = "RANK";
            Rankheader.GetComponent<Text>().color = Color.red;
            //Rankheader.GetComponent<Text>().anchor = TextAnchor.MiddleCenter;
            Rankheader.GetComponent<Text>().fontSize = 45;

            LeftTextPosition -= new Vector3(0, 100f, 0);
            RightTextPosition -= new Vector3(0, 100f, 0);
            CenterTextPosition -= new Vector3(0, 100f, 0);

            if (GameControl.instance.UserRank < 10)
            {
                for (int i = 0; i < GameControl.instance.Top10Scorelist.Count; i++)
                {
                    GameObject Score = Instantiate(BaseText) as GameObject;
                    Score.transform.SetParent(Canvasname.transform, false);
                    Score.transform.localPosition = RightTextPosition;
                    Score.GetComponent<Text>().text = (GameControl.instance.Top10Scorelist[i]);
                    Score.GetComponent<Text>().fontSize = 30;
                    //Score.GetComponent<Text>().anchor = TextAnchor.MiddleCenter;

                    GameObject Name = Instantiate(BaseText) as GameObject;
                    Name.transform.SetParent(Canvasname.transform, false);
                    Name.transform.localPosition = CenterTextPosition;
                    Name.GetComponent<Text>().text = (GameControl.instance.Top10Nameslist[i]);
                    Name.GetComponent<Text>().fontSize = 30;
                    //Name.GetComponent<Text>().anchor = TextAnchor.MiddleCenter;

                    GameObject Rank = Instantiate(BaseText) as GameObject;
                    Rank.transform.SetParent(Canvasname.transform, false);
                    Rank.transform.localPosition = LeftTextPosition;
                    Rank.GetComponent<Text>().text = "" + (i + 1);
                    Rank.GetComponent<Text>().fontSize = 30;

                    if (Name.GetComponent<Text>().text == GameControl.instance.username) //Color player score.
                    {
                        Score.GetComponent<Text>().color = Color.red;
                        Name.GetComponent<Text>().color = Color.red;
                        Rank.GetComponent<Text>().color = Color.red;
                    }

                    LeftTextPosition -= new Vector3(0, 60f, 0);
                    RightTextPosition -= new Vector3(0, 60f, 0);
                    CenterTextPosition -= new Vector3(0, 60f, 0);
                }
            }
            else
            {
                for (int i = 0; i < GameControl.instance.ScoresRangeList.Count; i++)
                {
                    GameObject Score = Instantiate(BaseText) as GameObject;
                    Score.transform.SetParent(Canvasname.transform, false);
                    Score.transform.localPosition = RightTextPosition;
                    Score.GetComponent<Text>().text = (GameControl.instance.ScoresRangeList[i]);
                    Score.GetComponent<Text>().fontSize = 30;
                    //Score.GetComponent<Text>().anchor = TextAnchor.MiddleCenter;

                    GameObject Name = Instantiate(BaseText) as GameObject;
                    Name.transform.SetParent(Canvasname.transform, false);
                    Name.transform.localPosition = CenterTextPosition;
                    Name.GetComponent<Text>().text = (GameControl.instance.PlayersRangeList[i]);
                    Name.GetComponent<Text>().fontSize = 30;
                    //Name.GetComponent<Text>().anchor = TextAnchor.MiddleCenter;

                    GameObject Rank = Instantiate(BaseText) as GameObject;
                    Rank.transform.SetParent(Canvasname.transform, false);
                    Rank.transform.localPosition = LeftTextPosition;
                    Rank.GetComponent<Text>().text = "" + ((GameControl.instance.UserRank - 5) + (i + 1));
                    Rank.GetComponent<Text>().fontSize = 30;

                    if (Name.GetComponent<Text>().text == GameControl.instance.username) //Color player score.
                    {
                        Score.GetComponent<Text>().color = Color.red;
                        Name.GetComponent<Text>().color = Color.red;
                        Rank.GetComponent<Text>().color = Color.red;
                    }

                    LeftTextPosition -= new Vector3(0, 60f, 0);
                    RightTextPosition -= new Vector3(0, 60f, 0);
                    CenterTextPosition -= new Vector3(0, 60f, 0);
                }
            }
        }
        else if (GameControl.instance.Leaderboardfilter == 3)
        {
            Vector3 LeftTextPosition = new Vector3(-280, 165, 0);
            Vector3 RightTextPosition = new Vector3(280, 165, 0);
            Vector3 CenterTextPosition = new Vector3(0, 165, 0);


            GameObject Top10header = Instantiate(BaseText) as GameObject;
            Top10header.transform.SetParent(Canvasname.transform, false);
            Top10header.transform.localPosition = new Vector3(0, 265, 0);
            Top10header.GetComponent<Text>().text = GameControl.instance.region;
            //Top10header.GetComponent<Text>().anchor = TextAnchor.MiddleCenter;
            Top10header.GetComponent<Text>().fontSize = 50;

            GameObject Scoreheader = Instantiate(BaseText) as GameObject;
            Scoreheader.transform.SetParent(Canvasname.transform, false);
            Scoreheader.GetComponent<Text>().text = "SCORE";
            Scoreheader.transform.localPosition = RightTextPosition;
            Scoreheader.GetComponent<Text>().color = Color.red;
            //Scoreheader.GetComponent<Text>().anchor = TextAnchor.MiddleCenter;
            Scoreheader.GetComponent<Text>().fontSize = 45;

            GameObject Nameheader = Instantiate(BaseText) as GameObject;
            Nameheader.transform.SetParent(Canvasname.transform, false);
            Nameheader.transform.localPosition = CenterTextPosition;
            Nameheader.GetComponent<Text>().text = "NAME";
            Nameheader.GetComponent<Text>().color = Color.red;
            //Nameheader.GetComponent<Text>().anchor = TextAnchor.MiddleCenter;
            Nameheader.GetComponent<Text>().fontSize = 45;

            GameObject Rankheader = Instantiate(BaseText) as GameObject;
            Rankheader.transform.SetParent(Canvasname.transform, false);
            Rankheader.transform.localPosition = LeftTextPosition;
            Rankheader.GetComponent<Text>().text = "RANK";
            Rankheader.GetComponent<Text>().color = Color.red;
            //Rankheader.GetComponent<Text>().anchor = TextAnchor.MiddleCenter;
            Rankheader.GetComponent<Text>().fontSize = 45;

            LeftTextPosition -= new Vector3(0, 100f, 0);
            RightTextPosition -= new Vector3(0, 100f, 0);
            CenterTextPosition -= new Vector3(0, 100f, 0);

            for (int i = 0; i < GameControl.instance.ScoresRegionRangeList.Count; i++)
            {
                GameObject Score = Instantiate(BaseText) as GameObject;
                Score.transform.SetParent(Canvasname.transform, false);
                Score.transform.localPosition = RightTextPosition;
                Score.GetComponent<Text>().text = (GameControl.instance.ScoresRegionRangeList[i]);
                Score.GetComponent<Text>().fontSize = 30;
                //Score.GetComponent<Text>().anchor = TextAnchor.MiddleCenter;

                GameObject Name = Instantiate(BaseText) as GameObject;
                Name.transform.SetParent(Canvasname.transform, false);
                Name.transform.localPosition = CenterTextPosition;
                Name.GetComponent<Text>().text = (GameControl.instance.PlayersRegionRangeList[i]);
                Name.GetComponent<Text>().fontSize = 30;
                //Name.GetComponent<Text>().anchor = TextAnchor.MiddleCenter;

                GameObject Rank = Instantiate(BaseText) as GameObject;
                Rank.transform.SetParent(Canvasname.transform, false);
                Rank.transform.localPosition = LeftTextPosition;

                if (GameControl.instance.UserRegionRank < 10)
                {
                    Rank.GetComponent<Text>().text = "" + (i + 1);
                }
                else
                {
                    Rank.GetComponent<Text>().text = "" + ((GameControl.instance.UserRegionRank - 5) + (i + 1));
                }
                Rank.GetComponent<Text>().fontSize = 30;
                //Rank.GetComponent<Text>().anchor = TextAnchor.MiddleCenter;

                if (Name.GetComponent<Text>().text == GameControl.instance.username) //Color player score.
                {
                    Score.GetComponent<Text>().color = Color.red;
                    Name.GetComponent<Text>().color = Color.red;
                    Rank.GetComponent<Text>().color = Color.red;
                }

                LeftTextPosition -= new Vector3(0, 60f, 0);
                RightTextPosition -= new Vector3(0, 60f, 0);
                CenterTextPosition -= new Vector3(0, 60f, 0);
                //Increment the positions again
            }
        }
        else if (GameControl.instance.Leaderboardfilter == 4)
        {
            Vector3 LeftTextPosition = new Vector3(-280, 165, 0);
            Vector3 RightTextPosition = new Vector3(280, 165, 0);
            Vector3 CenterTextPosition = new Vector3(0, 165, 0);


            GameObject Top10header = Instantiate(BaseText) as GameObject;
            Top10header.transform.SetParent(Canvasname.transform, false);
            Top10header.transform.localPosition = new Vector3(0, 265, 0);
            Top10header.GetComponent<Text>().text = GameControl.instance.org;
            //Top10header.GetComponent<Text>().anchor = TextAnchor.MiddleCenter;
            Top10header.GetComponent<Text>().fontSize = 50;

            GameObject Scoreheader = Instantiate(BaseText) as GameObject;
            Scoreheader.transform.SetParent(Canvasname.transform, false);
            Scoreheader.GetComponent<Text>().text = "SCORE";
            Scoreheader.transform.localPosition = RightTextPosition;
            Scoreheader.GetComponent<Text>().color = Color.red;
            //Scoreheader.GetComponent<Text>().anchor = TextAnchor.MiddleCenter;
            Scoreheader.GetComponent<Text>().fontSize = 45;

            GameObject Nameheader = Instantiate(BaseText) as GameObject;
            Nameheader.transform.SetParent(Canvasname.transform, false);
            Nameheader.transform.localPosition = CenterTextPosition;
            Nameheader.GetComponent<Text>().text = "NAME";
            Nameheader.GetComponent<Text>().color = Color.red;
            //Nameheader.GetComponent<Text>().anchor = TextAnchor.MiddleCenter;
            Nameheader.GetComponent<Text>().fontSize = 45;

            GameObject Rankheader = Instantiate(BaseText) as GameObject;
            Rankheader.transform.SetParent(Canvasname.transform, false);
            Rankheader.transform.localPosition = LeftTextPosition;
            Rankheader.GetComponent<Text>().text = "RANK";
            Rankheader.GetComponent<Text>().color = Color.red;
            //Rankheader.GetComponent<Text>().anchor = TextAnchor.MiddleCenter;
            Rankheader.GetComponent<Text>().fontSize = 45;

            LeftTextPosition -= new Vector3(0, 100f, 0);
            RightTextPosition -= new Vector3(0, 100f, 0);
            CenterTextPosition -= new Vector3(0, 100f, 0);

            for (int i = 0; i < GameControl.instance.ScoresOrgRangeList.Count; i++)
            {
                GameObject Score = Instantiate(BaseText) as GameObject;
                Score.transform.SetParent(Canvasname.transform, false);
                Score.transform.localPosition = RightTextPosition;
                Score.GetComponent<Text>().text = (GameControl.instance.ScoresOrgRangeList[i]);
                Score.GetComponent<Text>().fontSize = 30;
                //Score.GetComponent<Text>().anchor = TextAnchor.MiddleCenter;

                GameObject Name = Instantiate(BaseText) as GameObject;
                Name.transform.SetParent(Canvasname.transform, false);
                Name.transform.localPosition = CenterTextPosition;
                Name.GetComponent<Text>().text = (GameControl.instance.PlayersOrgRangeList[i]);
                Name.GetComponent<Text>().fontSize = 30;
                //Name.GetComponent<Text>().anchor = TextAnchor.MiddleCenter;

                GameObject Rank = Instantiate(BaseText) as GameObject;
                Rank.transform.SetParent(Canvasname.transform, false);
                Rank.transform.localPosition = LeftTextPosition;

                if (GameControl.instance.UserOrgRank < 10)
                {
                    //var Ranking = Mathf.Abs(GameControl.instance.UserOrgRank -5);
                    Rank.GetComponent<Text>().text = "" + (i + 1);
                }
                else
                {
                    Rank.GetComponent<Text>().text = "" + ((GameControl.instance.UserOrgRank - 5) + (i + 1));
                }

                //var Ranking = Mathf.Abs(GameControl.instance.UserOrgRank);
                //Rank.GetComponent<Text>().text = "" + (Ranking + (i));
                Rank.GetComponent<Text>().fontSize = 30;
                //Rank.GetComponent<Text>().anchor = TextAnchor.MiddleCenter;

                if (Name.GetComponent<Text>().text == GameControl.instance.username) //Color player score.
                {
                    Score.GetComponent<Text>().color = Color.red;
                    Name.GetComponent<Text>().color = Color.red;
                    Rank.GetComponent<Text>().color = Color.red;
                }

                LeftTextPosition -= new Vector3(0, 60f, 0);
                RightTextPosition -= new Vector3(0, 60f, 0);
                CenterTextPosition -= new Vector3(0, 60f, 0);
                //Increment the positions again
            }
        }
    }

}


