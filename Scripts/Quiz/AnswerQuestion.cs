using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AnswerQuestion : MonoBehaviour
{
    public string points;
    public int UIdelay = 3;
    public float FadeTimeDelay = 1f;
	public bool isSummaryTest = false;

	private WaitForSeconds FadeTimeDelayWait = new WaitForSeconds(1f);
	private WaitForSeconds FadeTimeDelayWait2 = new WaitForSeconds(0.25f);
	private WaitForSeconds AnswerDelay = new WaitForSeconds(0.5f);
	private WaitForSeconds AnswerDelay2 = new WaitForSeconds(0.1f);

	private Button thisButton;
	private Text thisText;
	private string itemText;
	private string thePointsPostFix;

	private string rightAnswerText;
	private string answer1Text;
	private string answer2Text;
	private string answer3Text;
	private string answer4Text;

	void Start()
	{
		// Debug.Log("AnswerQuestion.Start()");

		thisButton = this.GetComponentInChildren<Button>();
		thisText = this.GetComponentInChildren<Text>();
		thePointsPostFix = "";
	}

    public void answerquestion()
    {
        isSummaryTest = false;

		Quiz.instance.DisableInteractivity();

        if (isSummaryTest)
        {
			// EnableInteractivity ();
            Quiz.instance.Summary();
        }
        else
        {
			itemText = thisText.text;
			if (Quiz.instance.getQuestionType () == 1) itemText = itemText.ToUpper();

			if (itemText.Equals(Quiz.instance.rightanswer))
		    {
                Quiz.instance.Scorecounting = false;
                Quiz.instance.LowerPanel.blocksRaycasts = false;
                Quiz.instance.PointsGroup.ShowCanvasGroup();

				float thePoints = Mathf.Round (Quiz.instance.currentquestionscore);
				thePointsPostFix = " Points!";
				if (thePoints < 2) thePointsPostFix = " Point!";
				points = "+" + thePoints + thePointsPostFix;
			
                SetButtonState(this.GetComponent<Button>(), true);

				if (Quiz.instance.getQuestionType() == 2) 
				{
					Quiz.instance.getImageQuestion().crossFadeAlpha();
					Quiz.instance.getImageQuestion().theText.text = "";
				}
				else Quiz.instance.questionPanel.GetComponentInChildren<Text>().text = "";

                Quiz.instance.pointstext.text = points;
				Quiz.instance.PointsTextAnimation.Play("PointsScale", PlayMode.StopAll);

				thisText.color = Color.white;

                Quiz.instance.audiosource.PlayOneShot(Quiz.instance.correct, 1f);

                GameControl.instance.currentscore += Quiz.instance.currentquestionscore;
                Quiz.instance.scorebarshrinker.speed = 0;
                Quiz.instance.EndScoreCount(true);
				Quiz.instance.RightAnswerParticles.Play();

                StartCoroutine(RightDelay());
            }
            else
            {
                Quiz.instance.Scorecounting = false;
                Quiz.instance.LowerPanel.blocksRaycasts = false;
                Quiz.instance.TFQues.blocksRaycasts = false;
				if (Quiz.instance.getQuestionType() == 2) 
				{
					Quiz.instance.getImageQuestion().crossFadeAlpha();
					Quiz.instance.getImageQuestion().theText.text = "";
				}
				else Quiz.instance.QuestionPanelText.text = "";

				SetButtonState(thisButton, false);

                Quiz.instance.audiosource.PlayOneShot(Quiz.instance.incorrect, 1f);
                Quiz.instance.scorebarshrinker.speed = 0;
                Quiz.instance.ScorebarCanvasGroup.HideCanvasGroup();
                Quiz.instance.EndScoreCount();
				Quiz.instance.WrongAnswerParticles.Play();

                StartCoroutine(WrongDelay());
            }
        }
    }

    public void ResetButtonState(Button button)
    {
        button.GetComponentInChildren<CanvasGroup>().alpha = 1f;
        button.GetComponentInChildren<QuizButtonDown>().GetComponentInChildren<Image>().color = Color.white;
        button.GetComponentInChildren<QuizButtonDown>().GetComponent<CanvasGroup>().alpha = 0;
        button.GetComponentInChildren<Text>().color = Color.black;
    }

    public void SetButtonState(Button button, bool enabled)
    {
        if (enabled)
        {
            button.GetComponentInChildren<QuizButtonDown>().GetComponentInChildren<Image>().color = Quiz.instance.theGreenColor;
            button.GetComponentInChildren<QuizButtonDown>().GetComponent<CanvasGroup>().alpha = 1;
            button.GetComponentInChildren<Text>().color = Color.white;
        }
        else
        {
            button.GetComponentInChildren<QuizButtonDown>().GetComponentInChildren<Image>().color = Quiz.instance.theRedColor;
            button.GetComponentInChildren<QuizButtonDown>().GetComponent<CanvasGroup>().alpha = 1;
            button.GetComponentInChildren<Text>().color = Color.white;
        }
    }

	public IEnumerator<WaitForSeconds> WrongDelay()
    {
		for (int ii = 0; ii < 1; ii++)
        {
			yield return AnswerDelay;
        }

		rightAnswerText = Quiz.instance.rightanswer.ToUpper();

		answer1Text = Quiz.instance.Answer1Text.text.ToUpper();
		answer2Text = Quiz.instance.Answer2Text.text.ToUpper();
		answer3Text = Quiz.instance.Answer3Text.text.ToUpper();
		answer4Text = Quiz.instance.Answer4Text.text.ToUpper();

		//Debug.Log ("WrongDelay() RIGHT ANSWER: " + rightAnswerText);
		//Debug.Log ("ans1: " + answer1Text);
		//Debug.Log ("ans2: " + answer2Text);
		//Debug.Log ("ans3: " + answer3Text);
		//Debug.Log ("ans4: " + answer4Text);

		if (answer1Text.Equals(rightAnswerText))
        {
            SetButtonState(Quiz.instance.answer1, true);
        }
		if (answer2Text.Equals(rightAnswerText))
        {
            SetButtonState(Quiz.instance.answer2, true);
        }
		if (answer3Text.Equals(rightAnswerText))
        {
            SetButtonState(Quiz.instance.answer3, true);
        }
		if (answer4Text.Equals(rightAnswerText))
        {
            SetButtonState(Quiz.instance.answer4, true);
        }

        // Wait 2 seconds before fade out
        yield return FadeTimeDelayWait;

		float timeDelay = 0.5f;
        float overlapTimeDelay = 0.1f;

        // Start Fade Out

		if (Quiz.instance.getQuestionType() == 2) 
		{
			Quiz.instance.getImageQuestion().crossFadeAlpha (0f, timeDelay);
			Quiz.instance.getImageQuestion().theText.CrossFadeAlpha (0f, timeDelay, false);
		} 
		else 
		{
			Quiz.instance.QuestionPanelText.CrossFadeAlpha(0f, timeDelay, false);
		}

        timeDelay += overlapTimeDelay;  // increase for overlap

		Quiz.instance.Answer1FadeMe.startFadeOut(timeDelay);
     
        timeDelay += overlapTimeDelay;  // increase for overlap

		Quiz.instance.Answer2FadeMe.startFadeOut(timeDelay);
    
        timeDelay += overlapTimeDelay;  // increase for overlap

		Quiz.instance.Answer3FadeMe.startFadeOut(timeDelay);
        
        timeDelay += overlapTimeDelay;

		Quiz.instance.Answer4FadeMe.startFadeOut(timeDelay);
   
        for (int ii = 0; ii < 1; ii++)
        {
			yield return AnswerDelay;
        }

        // Debug.Log("Question Fade Out Complete");
		ResetButtonState(Quiz.instance.answer1);
        ResetButtonState(Quiz.instance.answer2);
        ResetButtonState(Quiz.instance.answer3);
        ResetButtonState(Quiz.instance.answer4);

        Quiz.instance.LowerPanel.HideCanvasGroup();
        Quiz.instance.TFQues.HideCanvasGroup();
		Quiz.instance.CheckNextQuestion();
    }

	public IEnumerator<WaitForSeconds> RightDelay()
    {
        for (int ii = 0; ii < 1; ii++)
        {
			yield return AnswerDelay2;
        }

		Quiz.instance.ScorePanelText.text = GameControl.instance.currentscore.ToString();
		Quiz.instance.ScorePanelAnimator.Play("ScoreNumberScale", -1, 0f);

        for (int ii = 0; ii < 1; ii++)
        {
			yield return AnswerDelay;
        }

        // Quiz.instance.pointstext.CrossFadeAlpha(0f, 0.5f, false);

        if (Quiz.instance.Answer1Text.text.Equals(Quiz.instance.rightanswer))
        {
            SetButtonState(Quiz.instance.answer1, true);
        }
		if (Quiz.instance.Answer2Text.text.Equals(Quiz.instance.rightanswer))
        {
            SetButtonState(Quiz.instance.answer2, true);
        }
		if (Quiz.instance.Answer3Text.text.Equals(Quiz.instance.rightanswer))
        {
            SetButtonState(Quiz.instance.answer3, true);
        }
		if (Quiz.instance.Answer4Text.text.Equals(Quiz.instance.rightanswer))
        {
            SetButtonState(Quiz.instance.answer4, true);
        }
        
		for (int ii = 0; ii < UIdelay; ii++)
        {
			yield return AnswerDelay2;
        }

        yield return FadeTimeDelayWait;

        float timeDelay = 0.25f;
        float overlapTimeDelay = 0.1f;

        // Start Fade Out
		if (Quiz.instance.getQuestionType() == 2) 
		{
			Quiz.instance.getImageQuestion().crossFadeAlpha (0f, timeDelay);
			Quiz.instance.getImageQuestion().theText.CrossFadeAlpha (0f, timeDelay, false);
		} 
		else 
		{
			Quiz.instance.QuestionPanelText.CrossFadeAlpha(0f, timeDelay, false);
		}

		Quiz.instance.pointstext.CrossFadeAlpha(0f, 0.5f, false);

        timeDelay += overlapTimeDelay;  // increase for overlap

		Quiz.instance.Answer1FadeMe.startFadeOut(timeDelay);

        timeDelay += overlapTimeDelay;  // increase for overlap

		Quiz.instance.Answer2FadeMe.startFadeOut(timeDelay);
       
        timeDelay += overlapTimeDelay;  // increase for overlap

		Quiz.instance.Answer3FadeMe.startFadeOut(timeDelay);
       
        timeDelay += overlapTimeDelay;

		Quiz.instance.Answer4FadeMe.startFadeOut(timeDelay);
       
        for (int ii = 0; ii < 1; ii++)
        {
			yield return FadeTimeDelayWait2;
        }

        // Debug.Log("Queston Fade Out Complete");

        Quiz.instance.pointstext.text = "";
        Quiz.instance.pointstext.CrossFadeAlpha(1f, 0f, false);

        ResetButtonState(Quiz.instance.answer1);
        ResetButtonState(Quiz.instance.answer2);
        ResetButtonState(Quiz.instance.answer3);
        ResetButtonState(Quiz.instance.answer4);

        Quiz.instance.LowerPanel.HideCanvasGroup();
        Quiz.instance.TFQues.HideCanvasGroup();

        for (int ii = 0; ii < UIdelay; ii++)
        {
			yield return FadeTimeDelayWait2;
        }

  		Quiz.instance.CheckNextQuestion();
    }
}