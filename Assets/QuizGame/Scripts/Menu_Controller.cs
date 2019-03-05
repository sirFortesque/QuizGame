using UnityEngine;

public class Menu_Controller : MonoBehaviour
{
    Data_Controller dataController;

	public void StartGame()
	{
        //dataController = FindObjectOfType<Data_Controller>();
        //dataController.RandomDataExtract();
		UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
	}

    public void Test() {
        dataController = FindObjectOfType<Data_Controller>();
        //dataController.LoadFromServerMethod();
        StartCoroutine(dataController.LoadFromServer("https://opentdb.com/api.php?amount=10"));
    }


}