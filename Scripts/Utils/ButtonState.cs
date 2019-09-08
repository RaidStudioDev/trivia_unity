using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonState : MonoBehaviour {

    private Button button;

    void Awake()
    {
        button = this.GetComponent<Button>();
    }
	
    public void onPress()
    {
        if (button != null) button.interactable = false;
    }
}
