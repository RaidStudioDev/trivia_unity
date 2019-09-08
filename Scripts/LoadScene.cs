using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void LoadSignin()
    {
		SceneManager.LoadScene("SignIn");
	}

   	public void LoadChallengeSelect()
    {
		GameControl.instance.soundManager.PlayLowToneButton();

		LeanTween.delayedCall (0.5f, () => {
			SceneManager.LoadScene("Home_Updated");
		});
		GameControl.instance.currentquestion += 1;
	}

	public void LoadChallenges()
	{
		GameControl.instance.soundManager.PlayLowToneButton();

		LeanTween.delayedCall (0.5f, () => {
			SceneManager.LoadScene("ChallengesHome");
		});
	}

    public void LoadQuiz()
    {
		GameControl.instance.soundManager.PlayLowToneButton();

		LeanTween.delayedCall (0.5f, () => {
			SceneManager.LoadScene("Quiz_Updated");
		});
	}

    public void LoadLeaderboard()
    {
		GameControl.instance.soundManager.PlayLowToneButton();

		LeanTween.delayedCall (0.5f, () => {
			SceneManager.LoadScene("LeaderboardHome");
		});
	}

	public void LoadHome()
	{
		GameControl.instance.soundManager.PlayLowToneButton();

		GameControl.instance.ClearChallengeInfo();

		LeanTween.delayedCall (0.5f, () => {
			SceneManager.LoadScene("Home_Updated");
		});
	}

    public void LBback()
    {
		GameControl.instance.soundManager.PlayLowToneButton();

		LeanTween.delayedCall (0.5f, () => {
			SceneManager.LoadScene("Home_Updated");
		});
    }

    public void LoadQuiz1()
    {
		// SceneManager.LoadScene("Quiz");
		// SceneManager.LoadScene("Quiz_Updated");
		GameControl.instance.currentquestion += 1;

		LeanTween.delayedCall (0.5f, () => {
			SceneManager.LoadScene("Quiz_Updated");
		});

	}
}
