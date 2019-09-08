using UnityEngine;
using System.Collections;

public class PlayAnim : MonoBehaviour
{

    public string AnimToPlay;

    public void PlayAnimation()
    {
        this.GetComponent<Animation>().Play(AnimToPlay, PlayMode.StopAll);
    }
}