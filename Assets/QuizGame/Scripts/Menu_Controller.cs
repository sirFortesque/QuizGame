using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Collections;
using UnityEngine.SceneManagement;
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
    public GameObject               encodingDropdownObject;

    public GameObject               databaseErrorWindow;
    public GameObject               numberOfQuestionsInputErrorWindow;
    public GameObject               timeLimitInputErrorWindow;
    //public GameObject               networkErrorWindow;

    public GameObject               customDisplay;

    [Header("Set in Editor")]
    public int                      urlNumberOfQuestions;
    public string                   urlCategory;
    public string                   urlDifficulty;
    public string                   urlTypeOfQuestions;
    //public string                   urlEncoding;
    public int                      timeLimit;    

    public Dropdown                 categoryDropdown;
    Dropdown                        difficultyDropdown;
    Dropdown                        typeOfQuestionsDropdown;
    //Dropdown                        encodingDropdown;
    Data_Controller                 dataController;

    public void Start() {
        instance = GetComponent<Menu_Controller>();

        dataController = Data_Controller.instance;        

        categoryDropdown = categoryDropdownObject.GetComponent<Dropdown>();
        difficultyDropdown = difficultyDropdownObject.GetComponent<Dropdown>();
        typeOfQuestionsDropdown = typeOfQuestionsDropdownObject.GetComponent<Dropdown>();
        //encodingDropdown = encodingDropdownObject.GetComponent<Dropdown>();        

        CategoriesAssignment();
        DifficultyAssignment();
        TypeAssignment();
        //EncodingAssignment();
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

    /*
    void EncodingAssignment() {

        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        options = encodingDropdown.options;

        options.Clear();
        options.Add(new Dropdown.OptionData("Default Encoding"));
        options.Add(new Dropdown.OptionData("Legacy URL Encoding"));
        options.Add(new Dropdown.OptionData("URL Encoding (RFC 3986)"));
        options.Add(new Dropdown.OptionData("Base64 Encoding"));
    }
     */

	public void StartGame()
	{        
		UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
	}

    public void RandomQuiz() {
        /*
        string chosenEncoding;

        chosenEncoding = encodingDropdown.captionText.text;

        switch (chosenEncoding) {
            case "Default Encoding":
                urlEncoding = "";
                break;
            case "Legacy URL Encoding":
                urlEncoding = "&encode=urlLegacy";
                break;
            case "URL Encoding (RFC 3986)":
                urlEncoding = "&encode=url3986";
                break;
            case "Base64 Encoding":
                urlEncoding = "&encode=base64";
                break;
        }
         */

        urlNumberOfQuestions = UnityEngine.Random.Range(10,31);
        timeLimit = urlNumberOfQuestions * 5;

        string url = "https://opentdb.com/api.php?amount=";
        url = url + urlNumberOfQuestions.ToString();

        StartCoroutine(dataController.LoadFromServer(url, dataController.OnResponseReceived_Random));
    }

    public void StartCustomQuizButton() {
        /*
        //////////////////////encodingDropdown///////////////
        string chosenEncoding;

        chosenEncoding = encodingDropdown.captionText.text;

        switch (chosenEncoding) {
            case "Default Encoding":
                urlEncoding = "";
                break;
            case "Legacy URL Encoding":
                urlEncoding = "&encode=urlLegacy";
                break;
            case "URL Encoding (RFC 3986)":
                urlEncoding = "&encode=url3986";
                break;
            case "Base64 Encoding":
                urlEncoding = "&encode=base64";
                break;
        }
        /////////////////////////////////////////////////////
         */
        
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
                urlDifficulty = "&difficulty=easy";
                break;
            case "Medium":
                urlDifficulty = "&difficulty=medium";
                break;
            case "Hard":
                urlDifficulty = "&difficulty=hard";
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

        // if placeholder IsActive "Random(1, 50)"
        if (numberOfQuestionsInputFieldObject.GetComponent<InputField>().placeholder.GetComponent<Text>().IsActive()) {
            urlNumberOfQuestions = UnityEngine.Random.Range(10, 51);
        } else {
            string chosenNumberOfQuestions;

            chosenNumberOfQuestions = numberOfQuestionsInputFieldObject.GetComponent<InputField>().text;

            // if "chosenNumberOfQuestions" IS NOT integer OR not in the range [1;50]
            // then show IntegerErrorWindow
            if ((!Int32.TryParse(chosenNumberOfQuestions, out urlNumberOfQuestions)) || (urlNumberOfQuestions <= 0) || (urlNumberOfQuestions > 50)) {
                StartCoroutine(NumberOfQuestionsInputError());
                return;
            } 

            // else assign "urlNumberOfQuestions = chosenNumberOfQuestions"
        }
        /////////////////////////////////////////////////////

        ///////////////////////TimeLimit/////////////////////

        // if placeholder IsActive "1ques.=5sec."
        if (timeLimitInputFieldObject.GetComponent<InputField>().placeholder.GetComponent<Text>().IsActive()) {
            timeLimit = urlNumberOfQuestions * 5;
        } else {
            string chosenTimeLimit;

            chosenTimeLimit = timeLimitInputFieldObject.GetComponent<InputField>().text;

            // if "chosenTimeLimit" IS NOT integer OR "timeLimit <= 0"
            // then show IntegerErrorWindow
            if ((!Int32.TryParse(chosenTimeLimit, out timeLimit)) || (timeLimit <= 0)) {
                StartCoroutine(TimeLimitInputError());
                return;
            }

            // else assign "timeLimit = chosenTimeLimit"        
        }

        /////////////////////////////////////////////////////

        string url = "https://opentdb.com/api.php?amount=";
        url = url + urlNumberOfQuestions.ToString() + urlCategory + urlDifficulty + urlTypeOfQuestions;

        StartCoroutine(dataController.LoadFromServer(url, dataController.OnResponseReceived_Castom));
    }

    public IEnumerator DatabaseError() {
        databaseErrorWindow.SetActive(true);
        yield return new WaitForSecondsRealtime(4);
        databaseErrorWindow.SetActive(false);
    }

    public IEnumerator NumberOfQuestionsInputError() {
        numberOfQuestionsInputErrorWindow.SetActive(true);
        yield return new WaitForSecondsRealtime(4);
        numberOfQuestionsInputErrorWindow.SetActive(false);
    }

    public IEnumerator TimeLimitInputError() {
            timeLimitInputErrorWindow.SetActive(true);
            yield return new WaitForSecondsRealtime(4);
            timeLimitInputErrorWindow.SetActive(false);
    }

    public void CustomQuizButton() {
        customDisplay.SetActive(true);
    }

    public void BackButton() {
        customDisplay.SetActive(false);
    }

    public void RefreshButton() {
        Destroy(Data_Controller.instance.gameObject);
        SceneManager.LoadScene("PersistentScene");
    }

    public void BackToMenuButton() {
        SceneManager.LoadScene("MenuScene");
    }

    public void TestButton() {
        StartCoroutine(dataController.LoadFromServer("https://opentdb.com/api_category.php", dataController.OnResponseReceived_DicOfCategories));
    }

}