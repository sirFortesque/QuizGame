using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Networking;
using System;

public class Data_Controller : MonoBehaviour 
{
	public Round_Data[] allRoundData;
	
	void Start ()  
	{
		DontDestroyOnLoad (gameObject);

        OnResponseReceived += HandleOnResponseReceived;

		SceneManager.LoadScene ("MenuScene");
	}
	
	public Round_Data GetCurrentRoundData()
	{
		return allRoundData [0];
	}   

    public IEnumerator LoadFromServer(string url) {

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError) {
            Debug.LogError(request.error);
        } else {
            Debug.Log(request.downloadHandler.text);
            OnResponseReceived(request.downloadHandler.text);           
        }

        request.Dispose();
    }

    public Action<string> OnResponseReceived;
    private void HandleOnResponseReceived(string response) {
       
        var randomData = new Opentdbcom_Class();
        JsonUtility.FromJsonOverwrite(response, randomData);        

        /*
        var i = 0;
        foreach (var result in randomData.results) {
            Debug.Log("================" + i + "=============");

            Debug.Log("CATEGORY: " + result.category + "\n"
                   + "type: " + result.type + "\n"
                   + "difficulty: " + result.difficulty + "\n"
                   + "question: " + result.question + "\n"                  
                   + "correct_answer: " + result.correct_answer + "\n");

            Debug.Log("INCORRECT_ANSWERS: \n");

            foreach (var item in result.incorrect_answers) {
                Debug.Log(item + "\n");
            }

            Debug.Log("======================================");

            i++;
        }  
         */

        var roundData = new Round_Data();
        roundData.questions = new Question_Data[10] { 
            new Question_Data(), new Question_Data(),
            new Question_Data(), new Question_Data(),
            new Question_Data(), new Question_Data(),
            new Question_Data(), new Question_Data(),
            new Question_Data(), new Question_Data()
        };    

        var question = new Question_Data();
        var answer = new Answer_Data();

        roundData.name = "RANDOM";
        roundData.pointsAddedForCorrectAnswer = 10;
        roundData.timeLimitInSeconds = 30;

        int k = 0;
        foreach (var result in randomData.results) {

            // questionText
            question.questionText = result.question;            

            // m - number of answers
            int m = 0;
            foreach (var ans in result.incorrect_answers) {
                m++;
            }

             
            question.answers = new Answer_Data[] {
                new Answer_Data(),
                new Answer_Data(),
                new Answer_Data(),
                new Answer_Data()
            };
            /*
            foreach (var ans in result.incorrect_answers) {
                Debug.Log(ans);
            }

            
            for (int i = 0; i < m+1; i++) {
                Debug.Log(question.answers[i].GetType().ToString());
            }
             */

            // answers[]
            int j = 0;
            foreach (var ans in result.incorrect_answers) {
                var qaj = question.answers[j];
		        qaj.answerText = ans;
                qaj.isCorrect = false;
                question.answers[j] = qaj;
                j++;
	        }
            var qajPlus1 = question.answers[j];
            qajPlus1.answerText = result.correct_answer;
            qajPlus1.isCorrect = true;
            question.answers[j] = qajPlus1;

            // QuestionData = questionText + answers[]
            roundData.questions[k] = question;
            k++;

            question = new Question_Data();
        }

        allRoundData[0] = roundData;

        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");

    }

}


[System.Serializable]
    public class Opentdbcom_Class {
        public int response_code;                
        public List<Result> results;     
  
    
 
    }

[System.Serializable]

public class Result {
    public string category;
    public string type;
    public string difficulty;
    public string question;
    public string correct_answer;
    public List<string> incorrect_answers;
}      



