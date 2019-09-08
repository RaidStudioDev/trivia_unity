using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class Quiz : MonoBehaviour {

    public static Quiz instance;

    public Color theGreenColor = new Vector4(17.0f / 255.0f, 132.0f / 255.0f, 20.0f / 255.0f, 1);
    public Color theRedColor = new Vector4(255.0f / 255.0f, 0f, 0f, 1);
    public int UIdelay = 1;
    public Button answer1;
    public Button answer2;
    public Button answer3;
    public Button answer4;
    public Text EndScoreTxt;
    public GameObject questionPanel;
    public GameObject ScorePanel;
    public GameObject ChallengeNamePanel;
    public CanvasGroup SummaryPanel;
    public CanvasGroup LowerPanel;
    public CanvasGroup MidPanel;
    public CanvasGroup PointsGroup;
    public CanvasGroup TFQues;
    public CanvasGroup ScorebarCanvasGroup;
    public string score = "Score:";
    public string hidenumber = "";
    public Image Scorebar;
    public string rightanswer;
    public Text scorebartext;
    public Text answerline2;
    public float fill;
    public Animator scorebarshrinker;
    public Animator buttonanim;
    public int currentquestionscore;
    public Text pointstext;
	public GameObject rightfx;
    public GameObject wrongfx;
    public GameObject summaryfx;
    public GameObject tooslowfx;
    public GameObject tooslow;
    public GameObject countdownBG;
    public AudioSource audiosource;
    public AudioClip incorrect;
    public AudioClip correct;
    public AudioClip summary;
    public AudioClip summaryIncorrect;
    public bool Counting = false;
    public bool Scorecounting = false;
    public GameObject counter;
    public int countdownint;
    public Image GameLogo;

    private int[] indicies;
    private static IEnumerable<int> SeqUpTo(int i) { for (int c = 0; c < i; ++c) yield return c; }
    private static System.Random random = new System.Random();
    private string question;
    private string questiontype;
    private string wronganswer1;
    private string wronganswer2;
    private string wronganswer3;
	private string questionimage;
	private ImageQuestion imageQuestion;
	private string imageRootURL; // = "https://www.lms.mybridgestoneeducation.com/TriviaPHP/images/";
    private float time;
    private int correctAnswers = 0;
  
	public Text challengeRedMsg;
	public Text challengeBlackMsg;
	public RectTransform newChallengeBtn;
	public RectTransform leaderBoardsBtn;

	// GAME PARTICLES
	private ParticleSystem smallStarParticles;
	private ParticleSystem blobParticles;
	private ParticleSystem faceHappyParticles;
	private ParticleSystem faceNeutralParticles;
	private ParticleSystem faceSadParticles;
	private ParticleSystem tooSlowParticles;
	private ParticleSystem rightAnswerParticles;
	public ParticleSystem RightAnswerParticles 
	{
		get {
			return rightAnswerParticles;
		}
	}
	private ParticleSystem wrongAnswerParticles;
	public ParticleSystem WrongAnswerParticles 
	{
		get {
			return wrongAnswerParticles;
		}
	}

	// wait props
	private WaitForSeconds playSummaryWait = new WaitForSeconds(0.75f);
	private WaitForSeconds playSummarySeq2Wait = new WaitForSeconds(0.25f);
	private WaitForSeconds fadeInQuestionWait = new WaitForSeconds (0.2f);
	private WaitForSeconds fadeInQuestionSeqInitWait = new WaitForSeconds (0.6f);
	private WaitForSeconds fadeInQuestionSeqWait = new WaitForSeconds (0.1f);
	private WaitForSeconds waitForSec = new WaitForSeconds(1);
	private WaitForSeconds countDownExpiredWait = new WaitForSeconds(0.8f);
	private WaitForSeconds countDownExpiredSeq1Wait = new WaitForSeconds(0.05f);
	private WaitForSeconds countDownExpiredSeq2Wait = new WaitForSeconds(0.2f);

	private Text questionPanelText;
	public Text QuestionPanelText 
	{
		get {
			return questionPanelText;
		}
	}


	private Image questionPanelImage;

	private Text answer1Text;
	public Text Answer1Text { get { return answer1Text; } }

	private Text answer2Text;
	public Text Answer2Text { get { return answer2Text; } }

	private Text answer3Text;
	public Text Answer3Text { get { return answer3Text; } }

	private Text answer4Text;
	public Text Answer4Text { get { return answer4Text; } }

	private Image answer1Image;
	private Image answer2Image;
	private Image answer3Image;
	private Image answer4Image;
	private AnswerQuestion answer1Question;
	private AnswerQuestion answer2Question;
	private AnswerQuestion answer3Question;
	private AnswerQuestion answer4Question;
	private FadeMe answer1FadeMe;
	public FadeMe Answer1FadeMe { get { return answer1FadeMe; } }

	private FadeMe answer2FadeMe;
	public FadeMe Answer2FadeMe { get { return answer2FadeMe; } }

	private FadeMe answer3FadeMe;
	public FadeMe Answer3FadeMe { get { return answer3FadeMe; } }

	private FadeMe answer4FadeMe;
	public FadeMe Answer4FadeMe { get { return answer4FadeMe; } }

	private Text scorePanelText;
	public Text ScorePanelText { get { return scorePanelText; } }

	private Animator scorePanelAnimator;
	public Animator ScorePanelAnimator { get { return scorePanelAnimator; } }

	private Animation pointsTextAnimation;
	public Animation PointsTextAnimation { get { return pointsTextAnimation; } }

	private Text challengeNamePanelText;
	private bool isGameCompleted = false;
	private Text counterText;
	private Image countdownBGImage;

	private List<string> answerlist;

	public int getQuestionType()
	{
		return System.Int32.Parse(questiontype);
	}

	public ImageQuestion getImageQuestion()
	{
		return imageQuestion;
	}

    void Awake()
    {
		if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;

			countdownBGImage = countdownBG.GetComponentInChildren<Image>();
			countdownBGImage.CrossFadeAlpha(0f, 0f, false);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
		answerlist = new List<string>();

		challengeRedMsg.gameObject.SetActive (false);
		challengeBlackMsg.gameObject.SetActive (false);
		newChallengeBtn.gameObject.SetActive (false);
		leaderBoardsBtn.gameObject.SetActive (false);

		imageRootURL = GameControl.instance.ImageRootURL;

		// cache counter text
		counterText = counter.GetComponent<Text>();

		// cache question panel components
		imageQuestion = questionPanel.GetComponentInChildren<ImageQuestion>();
		questionPanelText = questionPanel.GetComponentInChildren<Text>();
		questionPanelImage = questionPanel.GetComponent<Image>();

		// cache answer components
		answer1Text = answer1.GetComponentInChildren<Text>();
		answer2Text = answer2.GetComponentInChildren<Text>();
		answer3Text = answer3.GetComponentInChildren<Text>();
		answer4Text = answer4.GetComponentInChildren<Text>();
		answer1Image = answer1.GetComponentInChildren<Image>();
		answer2Image = answer2.GetComponentInChildren<Image>();
		answer3Image = answer3.GetComponentInChildren<Image>();
		answer4Image = answer4.GetComponentInChildren<Image>();
		answer1FadeMe = answer1.GetComponentInChildren<FadeMe>();
		answer2FadeMe = answer2.GetComponentInChildren<FadeMe>();
		answer3FadeMe = answer3.GetComponentInChildren<FadeMe>();
		answer4FadeMe = answer4.GetComponentInChildren<FadeMe>();
		answer1Question = answer1.GetComponent<AnswerQuestion>();
		answer2Question = answer2.GetComponent<AnswerQuestion>();
		answer3Question = answer3.GetComponent<AnswerQuestion>();
		answer4Question = answer4.GetComponent<AnswerQuestion>();

		// cache score, challenge name panels
		scorePanelText = ScorePanel.GetComponentInChildren<Text>();
		scorePanelAnimator = ScorePanel.GetComponentInChildren<Animator>();
		challengeNamePanelText = ChallengeNamePanel.GetComponentInChildren<Text>();

		pointsTextAnimation = pointstext.GetComponent<Animation>();

  		if (GameControl.instance.isRandomized) indicies = SeqUpTo(CSVreader.instance.grid.GetLength(1)).OrderBy(x => random.Next()).ToArray(); 
		else indicies = SeqUpTo(CSVreader.instance.grid.GetLength(1)).ToArray();
	
		// cache game particles
		smallStarParticles = summaryfx.transform.Find("Small Stars").GetComponent<ParticleSystem>();
		blobParticles = summaryfx.transform.Find("Blobs").GetComponent<ParticleSystem>();
		faceHappyParticles = summaryfx.transform.Find("Face_happy").GetComponent<ParticleSystem>();
		faceNeutralParticles = summaryfx.transform.Find("Face_neutral").GetComponent<ParticleSystem>();
		faceSadParticles = summaryfx.transform.Find("Face_sad").GetComponent<ParticleSystem>();
		tooSlowParticles = tooslowfx.GetComponentInChildren<ParticleSystem>();
		rightAnswerParticles = rightfx.GetComponentInChildren<ParticleSystem>();
		wrongAnswerParticles = wrongfx.GetComponentInChildren<ParticleSystem>();

		// start countdown
	    CountdowntoStart();
    }

    void RefreshQuestions()
    {
		if (GameControl.instance.isRandomized) indicies = SeqUpTo(CSVreader.instance.grid.GetLength(1)).OrderBy(x => random.Next()).ToArray(); 
		else indicies = SeqUpTo(CSVreader.instance.grid.GetLength(1)).ToArray();
    }

    void SetUIforQuestion()
    {
		// Debug.Log ("SetUIforQuestion");

        SummaryPanel.HideCanvasGroup();
        PointsGroup.HideCanvasGroup();
   
		questionPanelText.color = Color.black;
		questionPanelText.fontSize = 60;
		questionPanelImage.color = Color.clear;
      
		answer1Text.color = Color.black;
		answer2Text.color = Color.black;
		answer3Text.color = Color.black;
		answer4Text.color = Color.black;

		answer1Image.color = Color.white;
		answer2Image.color = Color.white;
		answer3Image.color = Color.white;
		answer4Image.color = Color.white;

        LowerPanel.ShowCanvasGroup();
        TFQues.ShowCanvasGroup();

		scorePanelText.text = score + GameControl.instance.currentscore;

		updateChallengePanel();

        ScorebarCanvasGroup.ShowCanvasGroup();
    }

    void SetUIforTF()
    {
		// Debug.Log ("SetUIforTF");

        SummaryPanel.HideCanvasGroup();
        PointsGroup.HideCanvasGroup();
    
   		questionPanelText.color = Color.black;
		questionPanelText.fontSize = 60;
		questionPanelImage.color = Color.clear;

		answer1Text.color = Color.black;
		answer2Text.color = Color.black;
		answer3Text.color = Color.black;
		answer4Text.color = Color.black;

		answer1Image.color = Color.white;
		answer2Image.color = Color.white;
		answer3Image.color = Color.white;
		answer4Image.color = Color.white;

        LowerPanel.ShowCanvasGroup();
        TFQues.HideCanvasGroup();
       	
		scorePanelText.text = score + GameControl.instance.currentscore;

		updateChallengePanel();

		ScorebarCanvasGroup.ShowCanvasGroup();
    }

    void CountdowntoStart()
    {
        LowerPanel.HideCanvasGroup();
        TFQues.HideCanvasGroup();

		scorePanelText.text = score + GameControl.instance.currentscore;

		updateChallengePanel();

        Invoke("InitializeCountdown", 0.25f);
    }

	void updateChallengePanel()
	{
		int currentQuestionIndex = GameControl.instance.currentquestion + 1;
		int totalQuestions = GameControl.instance.questionsinchallenge;

		challengeNamePanelText.text = currentQuestionIndex + " of " + totalQuestions;
	}

    void GetQuestionData() 
	{
		// Debug.Log("Quiz.GetQuestionData()");

		questiontype = CSVreader.instance.grid[0, indicies[GameControl.instance.currentquestion]];
		question = CSVreader.instance.grid[1, indicies[GameControl.instance.currentquestion]];
		rightanswer = CSVreader.instance.grid[2, indicies[GameControl.instance.currentquestion]];
		wronganswer1 = CSVreader.instance.grid[3, indicies[GameControl.instance.currentquestion]];
		wronganswer2 = CSVreader.instance.grid[4, indicies[GameControl.instance.currentquestion]];
		wronganswer3 = CSVreader.instance.grid[5, indicies[GameControl.instance.currentquestion]];

		if (questiontype == "2") 
		{
			questionimage = CSVreader.instance.grid[6, indicies[GameControl.instance.currentquestion]];
		}
    }


    public void QuizQuestion()
    {
		answerlist.Clear();

		DisableInteractivity ();

		currentquestionscore = 10;
		counterText.text = "10";
		Scorecounting = true;

		// Start Fade Out
		questionPanelText.CrossFadeAlpha(0f, 0f, false);

		answer1FadeMe.startFadeOut(0f);
		answer2FadeMe.startFadeOut(0f);
		answer3FadeMe.startFadeOut(0f);
		answer4FadeMe.startFadeOut(0f);

        time = 3f;
        GetQuestionData();

        if (questiontype == "1") // TRUE / FALSE TYPE
        {
            SetUIforTF();
            
			questionPanelText.text = question;

			answerlist.Add(rightanswer);
			answerlist.Add(wronganswer1);
			if (GameControl.instance.isRandomized) answerlist.ListShuffle();
		
			answer1Text.text = answerlist.ElementAt(0).Substring(0, 1).ToUpper() + answerlist.ElementAt(0).Substring(1).ToLower();
			answer2Text.text = answerlist.ElementAt(1).Substring(0, 1).ToUpper() + answerlist.ElementAt(1).Substring(1).ToLower();

            // Start Fade In
            StartCoroutine(FadeInQuestions(1));
        }
		else if (questiontype == "0") // MULTIPLE CHOICE QUESTION TYPE
        {
            SetUIforQuestion();

            // adds question to questionpaneltext
			questionPanelText.text = question;
            
            //shuffles order of questions and adds to answer buttons
		    answerlist.Add(rightanswer);
            answerlist.Add(wronganswer1);
            answerlist.Add(wronganswer2);
            answerlist.Add(wronganswer3);

			if (GameControl.instance.isRandomized) answerlist.ListShuffle();
		
			answer1Text.text = answerlist.ElementAt(0);
			answer2Text.text = answerlist.ElementAt(1);
			answer3Text.text = answerlist.ElementAt(2);
			answer4Text.text = answerlist.ElementAt(3);

            // Start Fade In
            StartCoroutine(FadeInQuestions(2));
        }
		else if (questiontype == "2") // MULTIPLE CHOICE IMAGE/QUESTION TYPE
		{
			SetUIforQuestion();

			// adds question to questionpaneltext
			questionPanelText.text = "";

			imageQuestion.theText.CrossFadeAlpha(0, 0, false);
			imageQuestion.theText.text = question;

			//shuffles order of questions and adds to answer buttons
			answerlist.Add(rightanswer);
			answerlist.Add(wronganswer1);
			answerlist.Add(wronganswer2);
			answerlist.Add(wronganswer3);

			if (GameControl.instance.isRandomized) answerlist.ListShuffle();
		
			answer1Text.text = answerlist.ElementAt(0);
			answer2Text.text = answerlist.ElementAt(1);
			answer3Text.text = answerlist.ElementAt(2);
			answer4Text.text = answerlist.ElementAt(3);

			StartCoroutine(setImage(questionimage));

			EnableInteractivity();
		}
    }
	
	IEnumerator setImage(string url) 
	{
		// Debug.Log("Quiz.setImage() url: " + imageRootURL + url);

		imageQuestion.crossFadeAlpha();

		// A URL where the image is stored
		// url = "https://www.lms.mybridgestoneeducation.com/TriviaPHP/images/test/cat1_driveguard_q_22.png";  

		//Call the WWW class constructor
		WWW imageURLWWW = new WWW(imageRootURL + url);  

		// Wait for the download
		yield return imageURLWWW;        

		// Simple check to see if there's indeed a texture available
		if (imageURLWWW.texture != null)
		{

			// Create a new sprite using the Texture2D from the url. 
			// Note that the 400 parameter is the width and height. 
			// Adjust accordingly
			Sprite sprite = Sprite.Create(imageURLWWW.texture, new Rect(0, 0, 640, 256), Vector2.zero);  

			// Assign the sprite to the Image Component
			imageQuestion.theImage.sprite = sprite;  
			imageQuestion.crossFadeAlpha(1f, 0.95f);
			imageQuestion.theText.CrossFadeAlpha(1f, 0.95f, false);

			StartCoroutine(FadeInQuestions(3));
		}

		yield return null;
	}

    public void startFadeOut(int type)
    {
        StartCoroutine(FadeOutQuestions(type));
    }

	IEnumerator FadeInQuestions(int type)
    {
		yield return fadeInQuestionWait;

        float fadeTimeDelay = 0.5f;
     
        // Start Fade In
		questionPanelText.CrossFadeAlpha(1f, fadeTimeDelay, false);

		yield return fadeInQuestionSeqInitWait;

		answer1FadeMe.startFadeIn(fadeTimeDelay);
    
		yield return fadeInQuestionSeqWait;

		answer2FadeMe.startFadeIn(fadeTimeDelay);
    
		if (type == 2 || type == 3) yield return fadeInQuestionSeqWait;

		answer3FadeMe.startFadeIn(fadeTimeDelay);
    
		if (type == 2 || type == 3) yield return fadeInQuestionSeqWait;

		answer4FadeMe.startFadeIn(fadeTimeDelay);
    
		EnableInteractivity();

		BeginScoreCount();
    }

    IEnumerator FadeOutQuestions(int type)
    {
		yield return fadeInQuestionWait;

        float fadeTimeDelay = 0.5f;
      
        // Start Fade In
		if (type == 3) 
		{
			imageQuestion.crossFadeAlpha (0f, fadeTimeDelay);
			imageQuestion.theText.CrossFadeAlpha (0f, fadeTimeDelay, false);
		} 
		else 
		{
			questionPanelText.CrossFadeAlpha(0f, fadeTimeDelay, false);
		}

		yield return fadeInQuestionSeqInitWait;

		answer1FadeMe.startFadeIn(fadeTimeDelay);
     
		yield return fadeInQuestionSeqWait;

		answer2FadeMe.startFadeIn(fadeTimeDelay);
     
		if (type == 2 || type == 3) yield return fadeInQuestionSeqWait;

		answer3FadeMe.startFadeIn(fadeTimeDelay);
     
		if (type == 2 || type == 3) yield return fadeInQuestionSeqWait;

		answer4FadeMe.startFadeIn(fadeTimeDelay);
     
		if (isGameCompleted) 
		{
			DisableQuizButtons();
			ClearQuizButtons();
		}
    }

    void InitializeCountdown()
    {
        StartCoroutine(FadeInCountDownSpiner());
    }
    
	IEnumerator FadeInCountDownSpiner()
    {
        isCountdownSpinning = true;

		if (countdownBG) countdownBGImage.CrossFadeAlpha(1f, 0.5f, false);

		yield return waitForSec;

		LeanTween.delayedCall (0f, BeginCountdown);
    }

	private Color beginQuestionPanelColor = new Color (1, 1, 1, 0);
	void BeginCountdown()
    {
        time = 4f;
        Counting = true;
        fill = 1f;

		questionPanelText.color = beginQuestionPanelColor;
		questionPanelText.fontSize = 200;
		questionPanelText.fontStyle = 0;

		RefreshQuestions();
    }

    void EndCountdown()
    {
        time = 4f;

        Counting = false;

		if (countdownBG) countdownBGImage.CrossFadeAlpha(0f, 0.5f, false);

        StartCoroutine(StartQuizQuestion());
    }

    IEnumerator StartQuizQuestion()
    {
		yield return playSummaryWait;

        isCountdownSpinning = false;

        QuizQuestion();
    }

    void BeginScoreCount()
    {
		Scorecounting = GameControl.instance.isTimer;
	    scorebarshrinker.Play("ScoreBarShrink2", -1, 0f);
        scorebarshrinker.speed = 1;

        StartCoroutine(Countdown());
    }

    public void EndScoreCount(bool correct = false)
    {
        if (correct) correctAnswers++;

        Scorecounting = false;
    }
    
    IEnumerator Countdown()
    {
		yield return waitForSec;

		counterText.text = (System.Int32.Parse(counterText.text) - 1).ToString();
		currentquestionscore = System.Int32.Parse(counterText.text);

        if (Scorecounting && currentquestionscore != 0)
        {
			if (currentquestionscore == 3) GameControl.instance.soundManager.PlayCountdown();
			if (currentquestionscore == 2) GameControl.instance.soundManager.PlayCountdown();
			if (currentquestionscore == 1) GameControl.instance.soundManager.PlayCountdown();

            yield return StartCoroutine(Countdown());
        }
        else if (Scorecounting && currentquestionscore == 0)
        {
			GameControl.instance.soundManager.PlayTooSlow();

            EndScoreCount();

            LowerPanel.blocksRaycasts = false;
            
			if (questiontype == "2") 
			{
				imageQuestion.crossFadeAlpha();
				imageQuestion.theText.text = "";
			} 
			else questionPanelText.text = "";

			tooSlowParticles.Play();
            
            ScorebarCanvasGroup.HideCanvasGroup();
            
            StartCoroutine(CountDownExpiredDelay());
        }
    }

    private bool isCountdownSpinning = true;
    private float spinCount = 1f;
	private bool[] isPlayedSoundCheck = {false, false, false, false};
	private bool isGoSoundCheck = false;
	private Color goGreenColor = new Color(17f / 255f, 132f / 255f, 20f / 255f, 1);

    void Update()
    {
        if (isCountdownSpinning)
        {
            if (countdownBG)
            {
                countdownBG.transform.rotation = Quaternion.Euler(0, 0, spinCount * 360);

                spinCount += Time.deltaTime;
            }
        }
        
        if (Counting)
        {
		    time -= Time.deltaTime;

			if (Mathf.Round(time).ToString() != "4") 
			{
				if (questionPanelText.color != Color.black) 
				{
					questionPanelText.color = Color.black;

				}

				int rndTime = (int)Mathf.Round(time);
				if (rndTime > 0 && isPlayedSoundCheck [rndTime] == false) 
				{
					isPlayedSoundCheck [rndTime] = true;
					GameControl.instance.soundManager.PlayCountdownHighPitchVibrato(0.45f);
				}
			}

			questionPanelText.text = (Mathf.Round(time).ToString());
        }


		if (time < .5f && !Counting)
        {
			questionPanelText.text = hidenumber;
        }

		if (time < .5f && Counting && questionPanelText.text != "GO") 
		{
			questionPanelText.color = goGreenColor;
			questionPanelText.text = "GO!";

			if (!isGoSoundCheck) 
			{
				isGoSoundCheck = true;
				GameControl.instance.soundManager.PlayCountdownHighPitchEndVibrato ();
			}
		}

        if (time < 0)
        {
            EndCountdown();
        }
    }
    
	IEnumerator<WaitForSeconds> CountDownExpiredDelay()
    {
        for (int ii = 0; ii < 1; ii++)
        {
			yield return countDownExpiredWait;
        }

		if (answer1Text.text.Equals(rightanswer))
        {
			answer1Question.SetButtonState(answer1, true);
        }
		if (answer2Text.text.Equals(rightanswer))
        {
			answer2Question.SetButtonState(answer2, true);
        }
		if (answer3Text.text.Equals(rightanswer))
        {
			answer3Question.SetButtonState(answer3, true);
        }
		if (answer4Text.text.Equals(rightanswer))
        {
			answer4Question.SetButtonState(answer4, true);
        }

		yield return countDownExpiredSeq1Wait;

		if (questiontype == "2") StartCoroutine(FadeOutQuestions(3)); else StartCoroutine(FadeOutQuestions(2));

		yield return countDownExpiredWait;

        tooslowfx.GetComponentInChildren<ParticleSystem>().Stop();

        for (int ii = 0; ii < 6; ii++)
        {
			yield return countDownExpiredSeq2Wait;
        }

		answer1Question.ResetButtonState(answer1);
		answer2Question.ResetButtonState(answer2);
		answer3Question.ResetButtonState(answer3);
		answer4Question.ResetButtonState(answer4);

		CheckNextQuestion();
    }

	public void CheckNextQuestion()
	{
		int qindex = GameControl.instance.currentquestion;
		int qtotal = GameControl.instance.questionsinchallenge;

		//Debug.Log("CheckNextQuestion.Current Question: " + (qindex + 1));
		//Debug.Log("CheckNextQuestion.Total Questions: " + qtotal);

		if (qindex < qtotal - 1)
		{
			GameControl.instance.currentquestion += 1;

			QuizQuestion();
		}
		else
		{
			Summary();
		}
	}

	public void Summary()
    {
        // Debug.Log("********************* Quiz.Summary");

		isGameCompleted = true;

        // Prepare FadeMe script for crossfades
        SummaryPanel.gameObject.AddComponent<FadeMe>();

		// questiontype = 2 is Multiple Choice Image/Question
		if (questiontype == "2") startFadeOut(3); else startFadeOut(2);

        StartCoroutine(PlaySummary());
    }

	private void DisableQuizButtons()
	{
		LowerPanel.blocksRaycasts = true;

		answer1.interactable = false;
		answer2.interactable = false;
		answer3.interactable = false;
		answer4.interactable = false;
	}

	private void ClearQuizButtons()
	{
		answer1.gameObject.SetActive (false);
		answer2.gameObject.SetActive (false);
		answer3.gameObject.SetActive (false);
		answer4.gameObject.SetActive (false);

		LowerPanel.gameObject.SetActive (false);
	}

	public IEnumerator<WaitForSeconds> PlaySummary()
    {
        // Wait a few, so that the questions and buttons fade out
		yield return playSummaryWait;
        
		// play particles
		smallStarParticles.Play();
		blobParticles.Play();

		// check score and play appropriate particles
		if (correctAnswers >= 8) faceHappyParticles.Play();
		else if (correctAnswers >= 5) faceNeutralParticles.Play();
		else faceSadParticles.Play();

	    // Play Coungratulations Audio
        if (correctAnswers >= 5) audiosource.PlayOneShot(summary, 1f);
        else audiosource.PlayOneShot(summaryIncorrect, 1f);

        // Hide Questions and Buttons
        LowerPanel.HideCanvasGroup();
        TFQues.HideCanvasGroup();

        int debugScore = 100;
        if (GameControl.instance == null) EndScoreTxt.text = debugScore.ToString() + " Points!";
        else
        {
            EndScoreTxt.text = GameControl.instance.currentscore + " Points!";
            GameControl.instance.totalscore = GameControl.instance.totalscore + GameControl.instance.currentscore;
        }

        pointstext.text = hidenumber;

        // Wait a few
		yield return playSummaryWait;

        // Fade In Points Text Objects
        SummaryPanel.blocksRaycasts = true;   // Enable Tap
        SummaryPanel.GetComponentInChildren<FadeMe>().startFadeIn();

        // Wait a few
		yield return playSummarySeq2Wait;

		// Save score
        // Don't save score if we are in debug mode
		if (!GameControl.instance.isMaxQuestions) 
		{
			ShowBottomButtons();
		} 
		else 
		{
			InitializeSaveData();
		}

        GameControl.instance.PlayersRangeList.Clear();
        GameControl.instance.ScoresRangeList.Clear();
        GameControl.instance.Leaderboardfilter = 1;

		EnableInteractivity ();
    }

    /// <summary>
	/// InitializeSaveData function
    /// </summary>
    private void InitializeSaveData()
    {
		string data1 = WWW.EscapeURL(GameControl.instance.username);
		string data2 = GameControl.instance.currentscore.ToString();
		string data3 = WWW.EscapeURL(GameControl.instance.email);
		string data4 = WWW.EscapeURL(GameControl.instance.region);
		string data5 = WWW.EscapeURL(GameControl.instance.org);

		string saveUrl = GameControl.instance.RootURL + "SaveData.php?t=8";
		saveUrl += "&f=" + data1 + "&s=" + data2 + "&e=" + data3 + "&r=" + data4 + "&o=" + data5;

		// Debug.Log ("saveUrl: " + saveUrl);

		WWW SaveUserDataAttempt = new WWW(saveUrl);
		StartCoroutine(SaveUserDataAttemptWaitForRequest(SaveUserDataAttempt));
    }

	IEnumerator SaveUserDataAttemptWaitForRequest(WWW www)
	{
		yield return www;

		// check for errors
		if (www.error == null) 
		{
			// Debug.Log("SaveUserDataAttemptWaitForRequest Ok!: " + www.text);

			// Check if we are in a challenge 
			if (GameControl.instance.isChallengeMode) 
			{
				GameControl.instance.OnUpdateScoreChallengeComplete += OnUpdateScoreChallengeComplete;
				GameControl.instance.UpdateScoreChallengeToServer();
			}
			else ShowBottomButtons();
			// Debug.Log("SaveUserDataAttemptWaitForRequest Ok! | result.success: " + result[0].success);
		} 
		else 
		{
			// Debug.Log("SaveUserDataAttemptWaitForRequest WWW Error: "+ www.error);
		}
	}

	void OnUpdateScoreChallengeComplete(bool success, ChallengeData info)
	{
		// Debug.Log("OnUpdateScoreChallengeComplete Ok! | success: " + success);

		GameControl.instance.OnUpdateScoreChallengeComplete -= OnUpdateScoreChallengeComplete;

		ShowBottomButtons ();

		if (GameControl.instance.isChallengeMode) 
		{
			challengeRedMsg.gameObject.SetActive (true);
			challengeBlackMsg.gameObject.SetActive (true);

			UpdateChallengeStatus(info);

			// Clear Challenge Info
			GameControl.instance.ClearChallengeInfo();
		}
	}

	void UpdateChallengeStatus(ChallengeData info)
	{
		if (info.isUserChallenger) 
		{
			if (info.friendcompleted == 1) { // friend has completed challenge

				// compare scores
				if (info.userscore == info.friendscore)  // tie
				{
					challengeRedMsg.text = "IT'S A DRAW!";
					challengeBlackMsg.text = info.frienduser + " SCORED " + info.friendscore;
				} 
				else 
				{
					if (info.userscore > info.friendscore) // user challenger won
					{ 
						challengeRedMsg.color = Color.red;
						challengeRedMsg.text = "YOU WON THE CHALLENGE!";
						challengeBlackMsg.text = info.frienduser + " SCORED " + info.friendscore;
					} 
					else // challenged won
					{ 
						challengeRedMsg.color = Color.black;
						challengeRedMsg.text = "YOU LOST THE CHALLENGE!";
						challengeBlackMsg.text = info.frienduser + " SCORED " + info.friendscore;
					}
				}
			} 
			else // friend did not play yet
			{ 
				challengeRedMsg.color = Color.red;
				challengeRedMsg.text = info.frienduser;
				challengeBlackMsg.text = "HAS NOT PLAYED YET";
			}
		}
		else 
		{
			if (info.usercompleted == 1) { // user challenger has completed challenge

				// compare scores
				if (info.friendscore == info.userscore)  // tie
				{
					challengeRedMsg.text = "IT'S A DRAW!";
					challengeBlackMsg.text = info.username + " SCORED " + info.userscore;
				} 
				else 
				{
					if (info.friendscore > info.userscore) // challenged user won
					{ 
						challengeRedMsg.color = Color.red;
						challengeRedMsg.text = "YOU WON THE CHALLENGE!";
						challengeBlackMsg.text = info.username + " SCORED " + info.userscore;
					} 
					else // challenger won
					{ 
						challengeRedMsg.color = Color.black;
						challengeRedMsg.text = "YOU LOST THE CHALLENGE!";
						challengeBlackMsg.text = info.username + " SCORED " + info.userscore;
					}
				}
			} 
			else // friend did not play yet
			{ 
				challengeRedMsg.color = Color.red;
				challengeRedMsg.text = info.username;
				challengeBlackMsg.text = "HAS NOT PLAYED YET";
			}
		}
	}

	void ShowBottomButtons()
	{
		Color btnColor = newChallengeBtn.GetComponent<Image>().color;
		newChallengeBtn.GetComponent<Image>().color = new Color (btnColor.r, btnColor.g, btnColor.b, 0);
		leaderBoardsBtn.GetComponent<Image>().color = new Color (btnColor.r, btnColor.g, btnColor.b, 0);

		newChallengeBtn.gameObject.SetActive (true);
		leaderBoardsBtn.gameObject.SetActive (true);

		LeanTween.alpha(newChallengeBtn, 1f, 0.75f)
			.setEase(LeanTweenType.easeOutCubic)
			.setDelay(0.15f)
			.setOnComplete(() => {

			});

		LeanTween.alpha(leaderBoardsBtn, 1f, 0.75f)
			.setEase(LeanTweenType.easeOutCubic)
			.setDelay(0.45f)
			.setOnComplete(() => {

			});
	}

	public void EnableInteractivity()
	{
		answer1.interactable = true;
		answer2.interactable = true;
		answer3.interactable = true;
		answer4.interactable = true;

		LowerPanel.interactable = true;
		LowerPanel.blocksRaycasts = true;
		MidPanel.interactable = true;
		MidPanel.blocksRaycasts = true;
	}

	public void DisableInteractivity()
	{
		answer1.interactable = false;
		answer2.interactable = false;
		answer3.interactable = false;
		answer4.interactable = false;

		LowerPanel.blocksRaycasts = false;
		LowerPanel.interactable = false;
		MidPanel.interactable = false;
		MidPanel.blocksRaycasts = false;
	}
}
