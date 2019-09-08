using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class NameButton : MonoBehaviour {

    public Text usernamebuttontext;
    void Start()
    {
        if (GameControl.instance == null)
        {
            usernamebuttontext.text = "Michael Jordan";
        }
		else usernamebuttontext.text = GameControl.instance.FormatUserNameDisplay(GameControl.instance.username);

        StartCoroutine(NameDelay());
    }

	private WaitForSeconds _waitDelay = new WaitForSeconds(0.75f);
    private IEnumerator<WaitForSeconds> NameDelay()
    {
		yield return _waitDelay;

        if (GameControl.instance == null)
        {
            usernamebuttontext.text = "Michael Jordan";
        }
		else usernamebuttontext.text = GameControl.instance.FormatUserNameDisplay(GameControl.instance.username);
    }
}
