using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Networking;

namespace InvicibleXGuest.Scripts.UI
{
    public class TestingNeet : MonoBehaviour
    {
        public void Start()
        {
            // var result = HasDuplicate(new int[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10});
            // Debug.Log(result);
            // bool resultAnagram = ValidAnagram("bbcc", "ccbc");
            // Debug.Log(resultAnagram);

            //LongestConsecutive(new int[] { 2,20,4,10,3,4,5 });

            //IsPalindromeOrNot("Was it a car or a cat I saw?");
            // var result = IsPalindromeOrNot("Was it a car or a cat I saw?");
            // Debug.Log(result);
            
            
            //Two Integer Sum II
            // int[] result = TwoSum(new int[] { 1,2,3,4}, 3);
            // Debug.Log(result[0] + " " + result[1]);
            
            //Container with most water
            //var result = MaxArea(new int[] { 1,7,2,5,4,7,3,6 });
            
            
            //Buy and Sell stock
            //int result = BuyAndSellStock(new int[]{7,1,5,3,6,4});
            
            //Longest substring without repeating characters
            int result = LengthOfLongestSubstring("abcabcbb");
            Debug.Log(result);
        }

        private int BuyAndSellStock(int[] prices)
        {
            int result = 0;
            int l = 0, r = 1;

            while (r < prices.Length)
            {
                if (prices[l] < prices[r])
                {
                    
                }
                else
                {
                    l = r;
                }
                r++;
            }
            
            return result;
        }
        
        private int LengthOfLongestSubstring(string s)
        {
            HashSet<char> charSet = new HashSet<char>(s.ToCharArray());
            foreach (var val in charSet)
            {
                if(!charSet.Contains(val))
                {
                    charSet.Remove(val);
                }
            }

            return charSet.Count;
        }

        //Brute Force Alog
        private int MaxArea(int[] heights) 
        {
            int maxArea = 0;
            for (int i = 0; i < heights.Length; i++)
            {
                for (int j = i + 1; j < heights.Length; j++)
                {
                    int minHeight = Math.Min(heights[i] , heights[j]);
                    int area = minHeight * (j - i);
                    maxArea = Math.Max(maxArea, area);
                }
            }
            return maxArea;
        }
        
        //Two Pointer Approach
        private int MaxAreaTwoPointerApproach(int[] heights)
        {
            int l = 0;
            int r = heights.Length - 1;
            int result = 0;

            while (l < r)
            {
                int minHeight = Math.Min(heights[l], heights[r]);
                int area = minHeight * (r - l);
                result = Math.Max(area, result);

                if (minHeight < heights[r])
                {
                    l++;
                }
                else
                {
                    r--;
                }
            }
            return result;
        }
        
        

        private int[] TwoSum(int[] numbers, int target)
        {
            int leftPointer = 0;
            int rightPointer = numbers.Length - 1;

            while (leftPointer < rightPointer)
            {
                int sumOfTwoNumbers = numbers[leftPointer] + numbers[rightPointer];
                
                if(sumOfTwoNumbers < target)
                {
                    leftPointer++;
                }
                else if(sumOfTwoNumbers > target)
                {
                    rightPointer--;
                }
                else
                {
                    return new int[] {leftPointer + 1, rightPointer + 1};
                }
            }
            return new int[0];
        }

        //Testing Neet HasDuplicate
        private bool HasDuplicate(int[] nums)
        {
            HashSet<int> countedNumbers = new HashSet<int>();
            foreach (var num in nums)
            {
                if (!countedNumbers.Add(num))
                {
                    return true;
                }
            }

            return false;
        }

        private bool ValidAnagram(string s, string t)
        {
            if(s.Length != t.Length)
            {
                Debug.Log("Not Anagram");
                return false;
            }
            
            int[] count = new int[26];
            for (int i = 0; i < s.Length; i++)
            {
                count[s[i] - 'a']++;
                count[t[i] - 'a']--;
            }

            foreach (var val in count)
            {
                if (val != 0)
                {
                    return false;
                }
            }
            return true;
        }

        //[0,2,4,5,9,7,5,2,6]
        //[1,2,3,4,5,6,7,8,9]
        private int LongestConsecutive(int[] nums)
        {
            HashSet<int> numSet = new HashSet<int>(nums);
            int longest = 0;
            
            foreach (int num in numSet) 
            {
                if (!numSet.Contains(num - 1)) 
                {
                    int length = 1;
                    while (numSet.Contains(num + length)) 
                    {
                        length++;
                    }
                    longest = Math.Max(longest, length);
                }
            }
            return longest;  
        }

        private bool IsPalindromeOrNot(string s)
        {
            int leftPointer = 0;
            int rightPointer = s.Length - 1;
            
            //Handle the cases 
            //Make sure all are Lower 
            s = s.ToLower();

            while (leftPointer < rightPointer)
            {
                while (leftPointer < rightPointer && !char.IsLetterOrDigit(s[leftPointer]))
                {
                    leftPointer++;
                }

                while (leftPointer < rightPointer && !char.IsLetterOrDigit(s[rightPointer]))
                {
                    rightPointer--;
                }
                
                if (s[leftPointer] != s[rightPointer])
                {
                    return false;
                }
                leftPointer++;
                rightPointer--;
            }
            
            return true;
            
        }
    }
}