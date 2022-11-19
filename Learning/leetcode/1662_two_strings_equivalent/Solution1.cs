using System.Text;

public class Solution1
{
    public bool ArrayStringsAreEqual(string[] word1, string[] word2)
    {
        var strA = new StringBuilder();
        var strB = new StringBuilder();

        foreach (var str in word1)
        {
            strA.Append(str);
        }

        foreach (var str in word2)
        {
            strB.Append(str);
        }

        return strA.ToString().Equals(strB.ToString());
    }
}