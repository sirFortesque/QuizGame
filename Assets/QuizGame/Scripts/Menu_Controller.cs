using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Collections;
using System;
using System.Collections;
using System.Collections.Generic;

public class Menu_Controller : MonoBehaviour
{
    public static Menu_Controller   instance;

    [Header("Set in Inspector")]
    public GameObject               numberOfQuestionsInputFieldObject;
    public GameObject               timeLimitInputFieldObject;
    public GameObject               categoryDropdownObject;
    public GameObject               difficultyDropdownObject;
    public GameObject               typeOfQuestionsDropdownObject;    

    public GameObject               databaseErrorWindow;
    public GameObject               integerErrorWindow;

    public GameObject               customDisplay;

    [Header("Set in Editor")]
    public int                      urlNumberOfQuestions;
    public string                   urlCategory;
    public string                   urlDifficulty;
    public string                   urlTypeOfQuestions;
    public int                      timeLimit;    

    public Dropdown                 categoryDropdown;
    Dropdown                        difficultyDropdown;
    Dropdown                        typeOfQuestionsDropdown;
    Data_Controller                 dataController;

    public void Start() {
        instance = GetComponent<Menu_Controller>();

        dataController = Data_Controller.instance;        

        categoryDropdown = categoryDropdownObject.GetComponent<Dropdown>();
        difficultyDropdown = difficultyDropdownObject.GetComponent<Dropdown>();
        typeOfQuestionsDropdown = typeOfQuestionsDropdownObject.GetComponent<Dropdown>();        

        CategoriesAssignment();
        DifficultyAssignment();
        TypeAssignment();
    }

    void TypeAssignment() {

        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        options = typeOfQuestionsDropdown.options;

        options.Clear();
        options.Add(new Dropdown.OptionData("Any Type"));
        options.Add(new Dropdown.OptionData("Multiple Choise"));
        options.Add(new Dropdown.OptionData("True / False"));        
    }

    void DifficultyAssignment() {

        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        options = difficultyDropdown.options;

        options.Clear();
        options.Add(new Dropdown.OptionData("Any Difficulty"));
        options.Add(new Dropdown.OptionData("Easy"));
        options.Add(new Dropdown.OptionData("Medium"));
        options.Add(new Dropdown.OptionData("Hard"));        
    }

    void CategoriesAssignment() {               
 
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        options = categoryDropdown.options;
       
        options.Clear();
        
        options.Add(new Dropdown.OptionData("Any Category"));

        foreach (var item in dataController.listOfCategories) {
            options.Add(new Dropdown.OptionData(item));     
        }
    }

	public void StartGame()
	{        
		UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
	}

    public void RandomQuiz() {
        urlNumberOfQuestions = UnityEngine.Random.Range(10,31);
        timeLimit = urlNumberOfQuestions * 5;

        string url = "https://opentdb.com/api.php?amount=";
        url = url + urlNumberOfQuestions.ToString();

        StartCoroutine(dataController.LoadFromServer(url, dataController.OnResponseReceived_Random));
    }

    public void StartCustomQuizButton() {
        
        //////////////////////categoryDropdown///////////////
        string chosenCategory;

        chosenCategory = categoryDropdown.captionText.text;

        if (chosenCategory == "Any Category") {
            urlCategory = "";

        } else {
            int categoryIndex;

            categoryIndex = dataController.rawTriviaCategories.trivia_categories.Find(x => x.name == chosenCategory).id;
            // opentdb.com API "understands" categorys only by ID 
            urlCategory = "&category=" + categoryIndex;
        }
        /////////////////////////////////////////////////////               


        //////////////////////difficultyDropdown///////////////
        string chosenDifficulty;

        chosenDifficulty = difficultyDropdown.captionText.text;

        switch (chosenDifficulty) {
            case "Any Difficulty":
                urlDifficulty = "";
                break;
            case "Easy":
                urlTypeOfQuestions = "&difficulty=easy";
                break;
            case "Medium":
                urlTypeOfQuestions = "&difficulty=medium";
                break;
            case "Hard":
                urlTypeOfQuestions = "&difficulty=hard";
                break;
        }
        /////////////////////////////////////////////////////


        //////////////////////typeDropdown///////////////
        string chosenType;

        chosenType = typeOfQuestionsDropdown.captionText.text;

        switch (chosenType) {
            case "Any Type":
                urlTypeOfQuestions = "";
                break;
            case "Multiple Choise":
                urlTypeOfQuestions = "&type=multiple";
                break;
            case "True / False":
                urlTypeOfQuestions = "&type=boolean";
                break;
        }
        /////////////////////////////////////////////////////


        /////////////////////NumberOfQuestions/////////////////
        string chosenNumberOfQuestions;

        chosenNumberOfQuestions = numberOfQuestionsInputFieldObject.GetComponent<InputField>().text;

        if (chosenNumberOfQuestions == "Random (10, 50)") {
            urlNumberOfQuestions = UnityEngine.Random.Range(10, 51);
        } else {
            if (!Int32.TryParse(chosenNumberOfQuestions, out urlNumberOfQuestions)) {
                StartCoroutine(IntegerError());
            }
        }
        /////////////////////////////////////////////////////

        

        timeLimit = urlNumberOfQuestions * 5;

        string url = "https://opentdb.com/api.php?amount=";
        url = url + urlNumberOfQuestions.ToString() + urlCategory + urlDifficulty + urlTypeOfQuestions;

        StartCoroutine(dataController.LoadFromServer(url, dataController.OnResponseReceived_Castom));
    }

    public IEnumerator DatabaseError() {
        databaseErrorWindow.SetActive(true);
        yield return new WaitForSecondsRealtime(4);
        databaseErrorWindow.SetActive(false);
    }

    public IEnumerator IntegerError() {
        integerErrorWindow.SetActive(true);
        yield return new WaitForSecondsRealtime(4);
        integerErrorWindow.SetActive(false);
    }

    public void CustomQuizButton() {
        customDisplay.SetActive(true);
    }

    public void BackButton() {
        customDisplay.SetActive(false);
    }

    public void TestButton() {
        StartCoroutine(dataController.LoadFromServer("https://opentdb.com/api_category.php", dataController.OnResponseReceived_DicOfCategories));
    }

}