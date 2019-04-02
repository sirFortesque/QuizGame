[System.Serializable]
public class Question_Data
{
    public string _category;
    public string _type;
    public string _difficulty;
	public string _questionText;
	public Answer_Data[] _answers;

    public Question_Data(string category, string type, string difficulty, string questionText, int numberOfAnswers) {

        this._category = category;

        this._type = type;

        this._difficulty = difficulty;

        this._questionText = questionText;

        var answers = new Answer_Data[numberOfAnswers];

        for (int i = 0; i < numberOfAnswers; i++) {
            answers[i] = new Answer_Data();
        }

        this._answers = answers;
    }

    public Question_Data()
        : this("notext", "notext", "notext", "notext", 0) {        
    }
}