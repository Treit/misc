/*
 * @lc app=leetcode id=26 lang=csharp
 *
 * [26] Remove Duplicates from Sorted Array
 */

// @lc code=start
public class Solution 
{
    public int RemoveDuplicates(int[] nums) 
    {
        int writeIndex = 0;
        int prev = nums[0];
        int currentRun = 0;
        int totalRemoved = 0;

        for (int i = 1; i < nums.Length; i++)
        {
            int current = nums[i];

            if (current == prev)
            {
                totalRemoved++;
                currentRun++;
                continue;
            }

            writeIndex++;
            currentRun = 0;
            // New value.
            nums[writeIndex] = current;
            prev = current;
        }

        return nums.Length - totalRemoved;
    }
}
// @lc code=end

