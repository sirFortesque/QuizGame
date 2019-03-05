using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Answer_Button : MonoBehaviour 
{
	public Text answerText;

	private Game_Controller gameController;
	private Answer_Data answerData;

	void Start()
	{
		gameController = FindObjectOfType<Game_Controller>();
	}

	public void SetUp(Answer_Data data)
	{
		answerData = data;
		answerText.text = answerData.answerText;
	}

	public void HandleClick()
	{
		gameController.AnswerButtonClicked(answerData.isCorrect);
	}
}
