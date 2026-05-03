namespace NumberWords.Api.Models;

public class NumberWordItem 
{
    public long OriginalNumber { set; get; }
    public string Word { set; get; }
    public bool IsOver9000 { set; get; }

    public NumberWordItem(long originalNumber, string word, bool isOver9000){
        OriginalNumber = originalNumber;
        Word = word;
        IsOver9000 = isOver9000;
    }
}