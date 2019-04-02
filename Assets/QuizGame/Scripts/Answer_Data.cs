[System.Serializable]
public class Answer_Data
{
	public string _answerText;
	public bool _isCorrect;

    public Answer_Data(string answerText, bool isCorrect) {
        this._answerText = answerText;
        this._isCorrect = isCorrect;
    }

    public Answer_Data() : this ("notext", false) {
    }
}

