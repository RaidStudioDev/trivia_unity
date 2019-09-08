using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class ChallengeManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip selectChallenge;

	// Holds which panel has been expanded, so shrink before re-doing the other
	private int panelThatIsExpanded = 0;

	// Number of buttons in a row
	int numItemsInRow = 4;

	// Height of a row of buttons
	float buttonRowHeight = 230f;

    // Use this for initialization
    void Start()
    {
		// scroll to top
		// ScrollPanel(GameControl.instance.menuScrollPosition);
    }

	public void LoadChallengeSelectByResource(string resource)
	{
		// Check if it has an extension
		// If it does not, it means this is coming from the server
		if (resource.IndexOf (".csv") != -1) resource = resource.Substring (0, resource.Length - 4); 
	
		GameControl.instance.CSVtoread = Resources.Load("ResourceData/" + resource) as TextAsset;

		GameControl.instance.currentquestion = 0;

		CSVreader.instance.ReadCSV();
	}

    public void ChallengeSelectByID(int buttonID)
    {
        Debug.Log("ChallengeSelectByID: " + buttonID);

		RecordScrollPosition (); // remember user's position

        audioSource.PlayOneShot(selectChallenge, 1f);

        GameControl.instance.selectedchallenge = buttonID;

        GameControl.instance.CSVtoread = new TextAsset();
        switch (GameControl.instance.selectedchallenge)
        {
            case 0:
                GameControl.instance.CSVtoread = Resources.Load("DriveGuardQuestions") as TextAsset;
                break;
            case 1:
                GameControl.instance.CSVtoread = Resources.Load("CompanyQuestions") as TextAsset;
                break;
            case 2:
				GameControl.instance.CSVtoread = Resources.Load("AllThingsBridgestone") as TextAsset;
                break;
			case 3:
				GameControl.instance.CSVtoread = Resources.Load("AllThingsFirestone") as TextAsset;
				break;
			case 4:
				GameControl.instance.CSVtoread = Resources.Load("TireInflation") as TextAsset;
				break;
			case 5:
				// GameControl.instance.CSVtoread = Resources.Load("Halloween_2016") as TextAsset;
				// GameControl.instance.CSVtoread = Resources.Load("NFL_2016") as TextAsset;
				GameControl.instance.CSVtoread = Resources.Load("NHL_2016") as TextAsset;
				break;
			case 6:
				GameControl.instance.CSVtoread = Resources.Load("TireFundamentals") as TextAsset;
				break;
			case 7:
				GameControl.instance.CSVtoread = Resources.Load("TireTermsSymbols") as TextAsset;
				break;
			case 8:
				GameControl.instance.CSVtoread = Resources.Load("TireSizes") as TextAsset;
				break;
			case 9:
				GameControl.instance.CSVtoread = Resources.Load("Sidewall") as TextAsset;
				break;
			case 10:
				GameControl.instance.CSVtoread = Resources.Load("TreadType") as TextAsset;
				break;
			case 11:
				GameControl.instance.CSVtoread = Resources.Load("Other_10") as TextAsset;
				break;

			// 06_Commercial_Light_Truck_Tires
			case 12:
				GameControl.instance.CSVtoread = Resources.Load("141_SB_C43_Duravis_R500HD_FINAL") as TextAsset;
				break;
			case 13:
				GameControl.instance.CSVtoread = Resources.Load("141_SB_C44_Duravis_M700HD_FINAL") as TextAsset;
				break;
			case 14:
				GameControl.instance.CSVtoread = Resources.Load("141_SB_C45_Duravis_R238_FINAL") as TextAsset;
				break;
			case 15:
				GameControl.instance.CSVtoread = Resources.Load("141_SB_C46_Transforce_HT_FINAL") as TextAsset;
				break;
			case 16:
				GameControl.instance.CSVtoread = Resources.Load("141_SB_C47_Transforce_AT2_FINAL") as TextAsset;
				break;
			case 17:
				GameControl.instance.CSVtoread = Resources.Load("141_SB_C48_Commercial_Light_Truck_FINAL") as TextAsset;
				break;
			case 18:
				GameControl.instance.CSVtoread = Resources.Load("141_SB_C49_Destination_MT2_FINAL") as TextAsset;
				break;


			// 02_Performance_Tires
			case 19:
				GameControl.instance.CSVtoread = Resources.Load("141_SB_C12_Potenza_RE97AS_FINAL") as TextAsset;
				break;
			case 20:
				GameControl.instance.CSVtoread = Resources.Load("141_SB_C13_Potenza_RE970AS_PP_FINAL") as TextAsset;
				break;
			case 21:
				GameControl.instance.CSVtoread = Resources.Load("141_SB_C14_Potenza_S04_PP_FINAL") as TextAsset;
				break;
			case 22:
				GameControl.instance.CSVtoread = Resources.Load("141_SB_C15_Potenza_RE960AS_PP_RFT_FINAL") as TextAsset;
				break;
			case 23:
				GameControl.instance.CSVtoread = Resources.Load("141_SB_C16_Potenza_RE11_FINAL") as TextAsset;
				break;
			case 24:
				GameControl.instance.CSVtoread = Resources.Load("141_SB_C17_Potenza_RE71R_FINAL") as TextAsset;
				break;
			case 25:
				GameControl.instance.CSVtoread = Resources.Load("141_SB_C19_Performance_Tires_FINAL") as TextAsset;
				break;
			case 100:
				GameControl.instance.CSVtoread = Resources.Load("141_SB_C23_Firehawk_AS_FINAL") as TextAsset;
				break;
			case 101:
				GameControl.instance.CSVtoread = Resources.Load("141_SB_C25_Firehawk_Indy_500_FINAL") as TextAsset;
				break;

			// 03_Touring_Tires
			case 26:
				GameControl.instance.CSVtoread = Resources.Load("141_SB_C1_DriveGuard_FINAL") as TextAsset;
				break;
			case 27:
				GameControl.instance.CSVtoread = Resources.Load("141_SB_C20_Turanza_Serenity_Plus_FINAL") as TextAsset;
				break;
			case 28:
				GameControl.instance.CSVtoread = Resources.Load("141_SB_C21_Ecopia_EP422_Plus_FINAL") as TextAsset;
				break;
			case 29:
				GameControl.instance.CSVtoread = Resources.Load("141_SB_C22_TouringTires_FINAL") as TextAsset;
				break;
			case 30:
				GameControl.instance.CSVtoread = Resources.Load("141_SB_C24_ChampionFF_FINAL") as TextAsset;
				break;
			case 31:
				GameControl.instance.CSVtoread = Resources.Load("141_SB_C26_FirestoneAS_FINAL") as TextAsset;
				break;

			// 08_Winter_Tires
			case 32:
				GameControl.instance.CSVtoread = Resources.Load("141_SB_C50_Blizzak_DMV2_FINAL") as TextAsset;
				break;
			case 33:
				GameControl.instance.CSVtoread = Resources.Load("141_SB_C51_Blizzak_LM32_FINAL") as TextAsset;
				break;
			case 34:
				GameControl.instance.CSVtoread = Resources.Load("141_SB_C52_Blizzak_W965_FINAL") as TextAsset;
				break;
			case 35:
				GameControl.instance.CSVtoread = Resources.Load("141_SB_C53_Blizzak_WS80_FINAL") as TextAsset;
				break;
			case 36:
				GameControl.instance.CSVtoread = Resources.Load("141_SB_C54_Winterforce2_FINAL") as TextAsset;
				break;
			case 37:
				GameControl.instance.CSVtoread = Resources.Load("141_SB_C55_Winterforce2_UV_FINAL") as TextAsset;
				break;
			case 38:
				GameControl.instance.CSVtoread = Resources.Load("141_SB_C56_Winterforce_LT_FINAL") as TextAsset;
				break;
			case 39:
				GameControl.instance.CSVtoread = Resources.Load("141_SB_C57_WinterTires_FINAL") as TextAsset;
				break;

			// 05_Light_Truck_Tires
			case 40:
				GameControl.instance.CSVtoread = Resources.Load("141_SB_C29_Dueler_HL422_Ecopia_FINAL") as TextAsset;
				break;
			case 41:
				GameControl.instance.CSVtoread = Resources.Load("141_SB_C30_Dueler_HP_Sport_AS_FINAL") as TextAsset;
				break;
			case 42:
				GameControl.instance.CSVtoread = Resources.Load("141_SB_C31_Dueler_HL_Alenza_Plus_FINAL") as TextAsset;
				break;
			case 43:
				GameControl.instance.CSVtoread = Resources.Load("141_SB_C32_Dueler_AT_Revo2_FINAL") as TextAsset;
				break;
			case 44:
				GameControl.instance.CSVtoread = Resources.Load("141_SB_C33_Dueler_HP_Sport_FINAL") as TextAsset;
				break;
			case 45:
				GameControl.instance.CSVtoread = Resources.Load("141_SB_C36_Dueler_HT_685_FINAL") as TextAsset;
				break;
			case 46:
				GameControl.instance.CSVtoread = Resources.Load("141_SB_C38_Destination_LE2_FINAL") as TextAsset;
				break;
			case 47:
				GameControl.instance.CSVtoread = Resources.Load("141_SB_C39_Destination_AT_FINAL") as TextAsset;
				break;
			case 48:
				GameControl.instance.CSVtoread = Resources.Load("141_SB_C40_Destination_AT_SE_FINAL") as TextAsset;
				break;
			case 49:
				GameControl.instance.CSVtoread = Resources.Load("141_SB_C42_Light_Truck_Tires_FINAL") as TextAsset;
				break;
    	}

        GameControl.instance.currentquestion = 0;

        CSVreader.instance.ReadCSV();
    }

	// SEEALLITEMS: User clicked a "See All" button in a panel
	public void SeeAllItems (int which)
	{

		int itemCount = 0;
		int numRows = 0;
		LayoutElement le;

		// Get the panel the button is in
		GameObject itemPanel = GameObject.Find ("ListItemPanel" + which.ToString ());

		if (itemPanel == null) {
			// Debug.Log ("Could not find panel ListItemPanel" + which.ToString () + ", aborting");
			return;
		}

		// If panel is already expanded, shrink it, else shrink any other that's expanded and expand the current one
		if (which == panelThatIsExpanded) {

			// Panel is already expanded, so shrink it
			le = itemPanel.GetComponent<LayoutElement> ();
			le.minHeight = buttonRowHeight;
			le.preferredHeight = 50f + buttonRowHeight;	// first for header, second is row
			panelThatIsExpanded = 0;

	
		} else {

			// Otherwise expand the panel

			// Assemble a list of all children that have the named tag in the current panel
			//  Will use this to determine number of rows, and hence height of panel
			RectTransform[] panelItems = itemPanel.GetComponentsInChildren<RectTransform> ();

			foreach (RectTransform child in panelItems) {
				if (child.gameObject.tag == "QuizCategory") {
					itemCount++;
				}
			}
		
			// Figure out number of rows in panel
			numRows = Mathf.FloorToInt ((itemCount - 1) / numItemsInRow) + 1;

			// If 1 row, no need to expand, so exit
			if (numRows == 1)
				return;

			// If another panel is expanded, shrink it
			if (panelThatIsExpanded != 0) {
				GameObject pnl = GameObject.Find ("ListItemPanel" + panelThatIsExpanded);
				if (pnl != null) {
					// Shrink the container to show them all
					le = pnl.GetComponent<LayoutElement> ();
					le.minHeight = 50f + buttonRowHeight;
					le.preferredHeight = 50f + buttonRowHeight;
				}
			}

			// Record old scroll position for adjustment
			float oldScrollPosition = GetScrollPosition ();

			// Enlarge the size of the container to show all Quiz buttons
			//   Problem: expansion is abrupt, also scrollbar position is thrown off for some reason
			le = itemPanel.GetComponent<LayoutElement> ();
			float panelHeight = 50f + (numRows * buttonRowHeight); //55 is space for category title and see all button
			le.minHeight = panelHeight;
			le.preferredHeight = panelHeight;

			// Since the panel is now bigger, and the scroll position is a percentage, the panel will
			//  naturally scroll up a bit on expansion. So, shift it down a bit to compensate
			// This is a hack: if the section has 3 rows, it doesn't work as well; if another panel is expanded
			//  shift isn't necessary, and if we ever add items, the percent of the shift will be off. But
			//  works good enough for now
			ScrollPanel (oldScrollPosition + 0.1f);

			// Mark that the panel is now expanded
			panelThatIsExpanded = which;
	
		}

	}

	private void ScrollPanel (float pos)
	{
		GameObject cPanel = GameObject.Find ("ChallengePanelList");
		ScrollRect sBar = cPanel.GetComponent<ScrollRect> ();

		sBar.verticalNormalizedPosition = pos;

	}

	private void RecordScrollPosition ()
	{
		GameObject cPanel = GameObject.Find ("ChallengePanelList");
		ScrollRect sBar = cPanel.GetComponent<ScrollRect> ();
		GameControl.instance.menuScrollPosition = sBar.verticalNormalizedPosition;


	}

	private float GetScrollPosition ()
	{
		GameObject cPanel = GameObject.Find ("ChallengePanelList");
		ScrollRect sBar = cPanel.GetComponent<ScrollRect> ();
		return sBar.verticalNormalizedPosition;
	}


	// 

	public void InviteUserToGame()
	{


	}
}