using System.Text;

public class Solution2
{
    public bool ArrayStringsAreEqual(string[] word1, string[] word2)
    {
        int word1idx = 0;
        int word2idx = 0;
        int char1idx = 0;
        int char2idx = 0;

        while (word1idx < word1.Length && word2idx < word2.Length)
        {
            if (word1[word1idx][char1idx] != word2[word2idx][char2idx])
            {
                return false;
            }

            char1idx++;
            char2idx++;

            if (char1idx == word1[word1idx].Length)
            {
                char1idx = 0;
                word1idx++;
            }

            if (char2idx == word2[word2idx].Length)
            {
                char2idx = 0;
                word2idx++;
            }

            if (word1idx == word1.Length && word2idx == word2.Length)
            {
                return true;
            }
        }

        return false;
    }
}