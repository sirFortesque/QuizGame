using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Game_Controller : MonoBehaviour
{
    [Header("Set in Inspector")]
	public Simple_ObjectPool    answerButtonObjectPool;
	public Text                 questionText;    
	public Text                 scoreDisplay;
	public Text                 timeRemainingDisplay;
	public Transform            answerButtonParent;
	public GameObject           questionDisplay;
	public GameObject           roundEndDisplay;
	public Text                 highScoreDisplay;

    public Text                 questionNumber;
    public Text                 questionCategory;
    public Text                 questionType;
    public Text                 questionDifficulty;

    /// //////////////////////////////////////////////
    
	private Data_Controller     dataController;
	private Round_Data          currentRoundData;
	private Question_Data[]     questionPool;
	private bool                isRoundActive = false;
	private float               timeRemaining;
	private int                 playerScore;
	private int                 questionIndex;
	private List<GameObject>    answerButtonGameObjects = new List<GameObject>();

	void Start()
	{
		dataController = FindObjectOfType<Data_Controller>();								// Store a reference to the DataController so we can request the data we need for this round

		currentRoundData = dataController.GetCurrentRoundData();							// Ask the DataController for the data for the current round. At the moment, we only have one round - but we could extend this
		questionPool = currentRoundData._questions;											// Take a copy of the questions so we could shuffle the pool or drop questions from it without affecting the original RoundData object        

		timeRemaining = currentRoundData._timeLimitInSeconds;								// Set the time limit for this round based on the RoundData object
		UpdateTimeRemainingDisplay();
		playerScore = 0;
		questionIndex = 0;

		ShowQuestion();
		isRoundActive = true;
	}

	void Update()
	{
		if (isRoundActive)
		{
			timeRemaining -= Time.deltaTime;												// If the round is active, subtract the time since Update() was last called from timeRemaining
			UpdateTimeRemainingDisplay();

			if (timeRemaining <= 0f)														// If timeRemaining is 0 or less, the round ends
			{
				EndRound();
			}
		}
	}

	void ShowQuestion()
	{
		RemoveAnswerButtons();        

        // Get the QuestionData for the current question
		Question_Data questionData = questionPool[questionIndex];

        //questionNumber.text = "Question: " + questionPool.Length.ToString();
        questionNumber.text = "Question: " + (questionPool.Length - questionIndex);

        /*
        Debug.Log("====================" + "\n");
        for (int i = 0; i < questionData._answers.Length; i++) {
            Debug.Log(i + " : " + questionData._answers[i]._answerText + " - " + questionData._answers[i]._isCorrect + "\n");
        }
        Debug.Log("====================" + "\n");
         */


        /*
        Debug.Log(questionData._category + "\n");
        Debug.Log("====================" + "\n");
        questionCategory.text = "Category: " + questionData._category;

        Debug.Log(questionData._type + "\n");
        Debug.Log("====================" + "\n");
        questionType.text = "Type: " + questionData._type;

        Debug.Log(questionData._difficulty + "\n");
        Debug.Log("====================" + "\n");
        questionDifficulty.text = "Difficulty: " + questionData._difficulty;
         */

        // Update questionText with the correct text
        questionText.text = questionData._questionText;										

		for (int i = 0; i < questionData._answers.Length; i ++)								// For every AnswerData in the current QuestionData...
		{
			GameObject answerButtonGameObject = answerButtonObjectPool.GetObject();			// Spawn an AnswerButton from the object pool
			answerButtonGameObjects.Add(answerButtonGameObject);
			answerButtonGameObject.transform.SetParent(answerButtonParent);
			answerButtonGameObject.transform.localScale = Vector3.one;

			Answer_Button answerButton = answerButtonGameObject.GetComponent<Answer_Button>();
			answerButton.SetUp(questionData._answers[i]);									// Pass the AnswerData to the AnswerButton so the AnswerButton knows what text to display and whether it is the correct answer
		}
	}

	void RemoveAnswerButtons()
	{
		while (answerButtonGameObjects.Count > 0)											// Return all spawned AnswerButtons to the object pool
		{
			answerButtonObjectPool.ReturnObject(answerButtonGameObjects[0]);
			answerButtonGameObjects.RemoveAt(0);
		}
	}

	public void AnswerButtonClicked(bool isCorrect)
	{
		if (isCorrect)
		{
			playerScore += currentRoundData._pointsAddedForCorrectAnswer;					// If the AnswerButton that was clicked was the correct answer, add points
			scoreDisplay.text = playerScore.ToString();
		}

		if(questionPool.Length > questionIndex + 1)											// If there are more questions, show the next question
		{
			questionIndex++;
			ShowQuestion();
		}
		else																				// If there are no more questions, the round ends
		{
            questionNumber.text = "Question: " + (questionPool.Length - questionIndex).ToString();
			EndRound();
		}
	}

	private void UpdateTimeRemainingDisplay()
	{
		timeRemainingDisplay.text = Mathf.Round(timeRemaining).ToString();
	}

	public void EndRound()
	{
		isRoundActive = false;

        if (playerScore > PlayerPrefs.GetInt("HighScore")) {
            PlayerPrefs.SetInt("HighScore", playerScore);            
        }
        highScoreDisplay.text = PlayerPrefs.GetInt("HighScore").ToString();

		questionDisplay.SetActive(false);
		roundEndDisplay.SetActive(true);
	}

	public void ReturnToMenu()
	{
		SceneManager.LoadScene("MenuScene");
	}
}