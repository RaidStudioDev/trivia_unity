using UnityEngine;

public class Reset : MonoBehaviour 
{
	public void ResetChallenge ()
	{
		GameControl.instance.currentscore = 0;
		GameControl.instance.currentquestion = 0;
        Object.Destroy(Quiz.instance);
        Destroy(GameObject.FindWithTag("QuizControllers"));
	}
}

