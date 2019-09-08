using UnityEngine;


public class ButtonScaleAnim : MonoBehaviour {

    public void PressButton()
    {
        this.GetComponent<Animation>().Play("ButtonScale", PlayMode.StopAll);
    }
}
