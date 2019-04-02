using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Networking;
using System;

public class Data_Controller : MonoBehaviour 
{
    public static Data_Controller instance;

    [Header("Set in Editor")]
	public Round_Data[] allRoundData;
    public TriviaCategories rawTriviaCategories;
    public List<string> listOfCategories;
	
	void Start ()  
	{

        if (instance == null) {

            DontDestroyOnLoad(gameObject);

            instance = GetComponent<Data_Controller>();

            OnResponseReceived_Random += HandleOnResponseReceived_Random;
            OnResponseReceived_Castom += HandleOnResponseReceived_Castom;
            OnResponseReceived_DicOfCategories += HandleOnResponseReceived_DicOfCategories;

            StartCoroutine(LoadFromServer("https://opentdb.com/api_category.php", OnResponseReceived_DicOfCategories));            

            //SceneManager.LoadScene("MenuScene");
        } else {
            Destroy(gameObject);
        }
		  
	}
	
	public Round_Data GetCurrentRoundData()
	{
		return allRoundData [0];
	}   

    public IEnumerator LoadFromServer(string url, Action<string> response) {

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError) {
            Debug.LogError(request.error);
        } else {
            response(request.downloadHandler.text);
        }

        request.Dispose();
    }




    public Action<string> OnResponseReceived_Random;
    private void HandleOnResponseReceived_Random(string response) {

        var fetchedData = new Opentdbcom_Class();
        JsonUtility.FromJsonOverwrite(response, fetchedData);        

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
        roundData._name = Menu_Controller.instance.urlCategory; 
        roundData._pointsAddedForCorrectAnswer = 10;
        roundData._timeLimitInSeconds = Menu_Controller.instance.timeLimit;
        roundData._questions = new Question_Data[Menu_Controller.instance.urlNumberOfQuestions];                   
        int k = 0;
        foreach (var result in fetchedData.results) {

            // m - number of answers
            int m = 0;
            foreach (var ans in result.incorrect_answers) {
                m++;
            }

            // questionText and answers[] initialization
            var question = new Question_Data(result.category, result.type, result.difficulty, result.question, m+1);            

            // answers[] filling
            int j = 0;
            foreach (var ans in result.incorrect_answers) {
                var qaj = question._answers[j];
		        qaj._answerText = ans;
                qaj._isCorrect = false;
                question._answers[j] = qaj;
                j++;
	        }
            var qajPlus1 = question._answers[j];
            qajPlus1._answerText = result.correct_answer;
            qajPlus1._isCorrect = true;
            question._answers[j] = qajPlus1;

            // QuestionData = questionText + answers[]
            roundData._questions[k] = question;
            k++;
        }


        /*
        var i = 0;
        foreach (var question in roundData._questions) {
            Debug.Log("================" + i + "=============");

            Debug.Log("CATEGORY: " + question._category + "\n"
                   + "type: " + question._type + "\n"
                   + "difficulty: " + question._difficulty + "\n"
                   + "question: " + question._questionText + "\n");

            Debug.Log("ANSWERS: \n");

            foreach (var item in question._answers) {
                Debug.Log(item + "\n");
            }

            Debug.Log("======================================");

            i++;
        } 
         */
        



        allRoundData[0] = roundData;

        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");

    }



    public Action<string> OnResponseReceived_Castom;
    private void HandleOnResponseReceived_Castom(string response) {

        var fetchedData = new Opentdbcom_Class();
        JsonUtility.FromJsonOverwrite(response, fetchedData);

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

        switch (fetchedData.response_code) {
            case 0:
                break;
            case 1:
                StartCoroutine(Menu_Controller.instance.DatabaseError());
                return;
            case 2:

                return;
        }

        var roundData = new Round_Data();
        roundData._name = Menu_Controller.instance.categoryDropdown.captionText.text;
        roundData._pointsAddedForCorrectAnswer = 10;
        roundData._timeLimitInSeconds = Menu_Controller.instance.timeLimit;
        roundData._questions = new Question_Data[Menu_Controller.instance.urlNumberOfQuestions];

        //var question = new Question_Data();
        //var answer = new Answer_Data();        

        int k = 0;
        foreach (var result in fetchedData.results) {

            // m - number of answers
            int m = 0;
            foreach (var ans in result.incorrect_answers) {
                m++;
            }

            // questionText and answers[] initialization
            var question = new Question_Data(result.category, result.type, result.difficulty, result.question, m + 1);

            /*
            // questionText
            question._questionText = result.question;                        
            
            // answers[] initialization
            question._answers = new Answer_Data[m+1];
             */

            // answers[] filling
            int j = 0;
            foreach (var ans in result.incorrect_answers) {
                var qaj = question._answers[j];
                qaj._answerText = ans;
                qaj._isCorrect = false;
                question._answers[j] = qaj;
                j++;
            }
            var qajPlus1 = question._answers[j];
            qajPlus1._answerText = result.correct_answer;
            qajPlus1._isCorrect = true;
            question._answers[j] = qajPlus1;

            // QuestionData = questionText + answers[]
            roundData._questions[k] = question;
            k++;

            //question = new Question_Data();
        }

        allRoundData[0] = roundData;

        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");

    }



    public Action<string> OnResponseReceived_DicOfCategories;
    private void HandleOnResponseReceived_DicOfCategories(string response) {              
        JsonUtility.FromJsonOverwrite(response, rawTriviaCategories);

        //filling listOfCategories
        foreach (var item in rawTriviaCategories.trivia_categories) {
            listOfCategories.Add(item.name);
        }        

        SceneManager.LoadScene("MenuScene");

        /*
        foreach (var item in listOfCategories) {
            Debug.Log(item);
        }
         */

        /*
        Debug.Log(rawTriviaCategories.trivia_categories.Capacity);

        
        foreach (var item in rawTriviaCategories.trivia_categories) {
            Debug.Log("ID: " + item.id + "\n"
                    + "NAME: " + item.name + "\n");            
        }
         */         
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



[System.Serializable]
public class TriviaCategories {
    public List<DicOfCategories> trivia_categories;

    /*
    public TriviaCategories(int numberOfCategories) {
        //DicOfCategories category = new DicOfCategories();

        List<DicOfCategories> lst = new List<DicOfCategories>();

        for (int i = 0; i < numberOfCategories; i++) {
            DicOfCategories category = new DicOfCategories();
            lst.Add(category);
        }

        this._trivia_categories = lst;
    }

    public TriviaCategories() : this (0) {        
    }
     */
}

[System.Serializable]
public class DicOfCategories {
    public int id;
    public string name;

    /*
    public DicOfCategories(int id, string name) {
        this._id = id;
        this._name = name;
    }

    public DicOfCategories() : this (0, "notext") {        
    }
     */
}




