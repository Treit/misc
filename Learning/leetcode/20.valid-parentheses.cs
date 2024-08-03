/*
 * @lc app=leetcode id=20 lang=csharp
 *
 * [20] Valid Parentheses
 */

// @lc code=start
using System.Collections.Generic;
public class Solution 
{
    public bool IsValid(string s) 
    {
        var stack = new Stack<char>();
        foreach (var c in s)
        {
            switch (c)
            {
                case '(':
                case '[':
                case '{':
                    stack.Push(c);
                    break;
                case ')':
                case ']':
                case '}':
                    if (stack.Count == 0)
                    {
                        return false;
                    }
                    var last = stack.Pop();
                    if (last == '(' && c != ')')
                    {
                        return false;
                    }
                    else if (last == '[' && c != ']')
                    {
                        return false;
                    }
                    else if (last == '{' && c != '}')
                    {
                        return false;
                    }
                    break;
            }
        }

        return stack.Count == 0;
    }
}
// @lc code=end

