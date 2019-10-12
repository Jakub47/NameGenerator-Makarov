using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.FileIO;

namespace MakarovChain
{
    class Program
    {
        private static List<string> Names = new List<string>();
        private static Dictionary<string, Dictionary<string, float>> SeriesValue = new Dictionary<string, Dictionary<string, float>>();
        private static List<string> NamesToReturn = new List<string>();
        private static string finalString = "";

        static void Main(string[] args)
        {
           
            Random rand = new Random();


            Console.WriteLine("Pierwsza litera twojego nicku");
            string choice = char.ToLower(Console.ReadKey().KeyChar).ToString();

            var pathToNames = @"C:\Users\Ragnus\Downloads\babies-first-names-2010-2018.csv"; // Habeeb, "Dubai Media City, Dubai"
            using (TextFieldParser ParserCsv = new TextFieldParser(pathToNames))
            {
                ParserCsv.CommentTokens = new string[] { "#" };
                ParserCsv.SetDelimiters(new string[] { "," });
                ParserCsv.HasFieldsEnclosedInQuotes = true;

                ParserCsv.ReadLine();

                while (!ParserCsv.EndOfData)
                {
                    string[] entites = ParserCsv.ReadFields();

                    //To make sure i will not get all 60000 records select only words starting with given words
                    if (char.ToLower(entites[2][0]).ToString() == choice)
                        Names.Add(entites[2]);
                }

            }


            //Choice == first letter to start from , Names = Names starting with choice


            //InitialsState == get second letter count all occurence get proper next index
            while (NamesToReturn.Count < 100)
            {
                SeriesValue = new Dictionary<string, Dictionary<string, float>>();
                SeriesValue = InitalState(rand, choice);
                string getWord = " ";

                do
                {
                    getWord = GetWord(SeriesValue, rand);
                } while ( getWord == null || NamesToReturn.Any(a => string.Equals(a, getWord, StringComparison.CurrentCultureIgnoreCase)) 
                         || Names.Any(a => string.Equals(a, getWord, StringComparison.CurrentCultureIgnoreCase)));

                

                NamesToReturn.Add(getWord);
            }

            Console.WriteLine();
            Console.WriteLine("---------------------------------------------------------------------------------------------------");
            NamesToReturn.ForEach(a => Console.Write(a + ","));
        }

        /// <summary>
        /// Generate word using pair from initialState.
        /// </summary>
        /// <param name="seriesValue">Dictionary containg given pair and empty new dicionary</param>
        /// <param name="rand">rand object used for probability</param>
        private static string GetWord(Dictionary<string, Dictionary<string, float>> seriesValue, Random rand)
        {
            string finalString = seriesValue.Keys.First();
            string twoLastLetters = seriesValue.Keys.First();
            int c = 0; int gg = 0; int t = 0;
            string nextLLetter = "";
            bool b1 = false; bool b2 = false;

            while (finalString.Length != 7)
            {
                Names.ForEach(a =>
                {
                    a = a.ToLower();
                    a = Regex.Replace(a, "[^0-9a-zA-Z]+", "");
                    a = a.Replace(" ", "");

                    if (a.Contains(twoLastLetters))
                    {
                        c = a.IndexOf(twoLastLetters);
                        gg = twoLastLetters.Length;
                        t = a.Length;

                        if (a.IndexOf(twoLastLetters) + twoLastLetters.Length < a.Length)
                        {
                            string nextLetter = a.Substring(a.IndexOf(twoLastLetters) + twoLastLetters.Length, 1);
                            nextLLetter = nextLetter;
                            b1 = char.IsLetter(Convert.ToChar(nextLetter));
                            b2 = !(string.IsNullOrEmpty(nextLetter));

                            if (char.IsLetter(Convert.ToChar(nextLetter)) && !(string.IsNullOrEmpty(nextLetter)))
                            {
                                if (seriesValue[twoLastLetters].ContainsKey(nextLetter))
                                    seriesValue[twoLastLetters][nextLetter]++;
                                else
                                    seriesValue[twoLastLetters].Add(nextLetter, 1);
                            }

                        }
                    }
                });

                foreach (KeyValuePair<string, Dictionary<string, float>> charWithCharValues in seriesValue)
                {
                    float sum = 0;

                    //First loop for count all instances of letter
                    foreach (KeyValuePair<string, float> yos in charWithCharValues.Value)
                    {
                        sum += yos.Value;
                    }

                    //After loop every value will have percentage of all values meaning that for example a will be 0,2 b will be 0,09
                    for (int i = 0; i < charWithCharValues.Value.Count; i++)
                    {
                        charWithCharValues.Value[charWithCharValues.Value.ElementAt(i).Key] /= sum;
                    }
                }

                if (seriesValue[twoLastLetters].Values.Count == 0)
                    return null;

                while (true)
                {
                    foreach (KeyValuePair<string, float> currentKeyValue in seriesValue[twoLastLetters])
                    {
                        if (rand.NextDouble() < currentKeyValue.Value)
                        {
                            finalString += currentKeyValue.Key;
                            goto Founded;
                        }
                    }
                }

            Founded:
                twoLastLetters = finalString.Substring(finalString.Length - 2);
                seriesValue = new Dictionary<string, Dictionary<string, float>>();
                seriesValue.Add(twoLastLetters, new Dictionary<string, float>());
            }
            return finalString;
        }

        /// <summary>
        /// Create second letter to first written by user using probality. Return new SeriesOfValues With selected letter as key
        /// </summary>
        /// <param name="rand">random obj which will generate propability</param>
        /// <param name="choice">The first letter written by user</param>
        private static Dictionary<string, Dictionary<string, float>> InitalState(Random rand, string choice)
        {
            finalString = "";
            SeriesValue.Add(choice, new Dictionary<string, float>());
            Names.ForEach(a =>
            {
                string nextLetter = a[1].ToString();

                if (char.IsLetter(Convert.ToChar(nextLetter)) && !(string.IsNullOrEmpty(nextLetter)))
                {
                    if (SeriesValue[choice].ContainsKey(nextLetter))
                        SeriesValue[choice][nextLetter]++;
                    else
                        SeriesValue[choice].Add(nextLetter, 1);
                }
            });

            foreach (KeyValuePair<string, Dictionary<string, float>> charWithCharValues in SeriesValue)
            {
                float sum = 0;

                //First loop for count all instances of letter
                foreach (KeyValuePair<string, float> yos in charWithCharValues.Value)
                {
                    sum += yos.Value;
                }

                //After loop every value will have percentage of all values meaning that for example a will be 0,2 b will be 0,09
                for (int i = 0; i < charWithCharValues.Value.Count; i++)
                {
                    charWithCharValues.Value[charWithCharValues.Value.ElementAt(i).Key] /= sum;
                }
            }

            while (finalString.Length < 2)
            {
                foreach (KeyValuePair<string, float> currentKeyValue in SeriesValue[choice])
                {
                    if (rand.NextDouble() < currentKeyValue.Value)
                    {
                        finalString += currentKeyValue.Key;
                        SeriesValue = new Dictionary<string, Dictionary<string, float>>();
                        SeriesValue.Add(choice + finalString, new Dictionary<string, float>());
                        return SeriesValue;
                    }
                }
            }

            return null;
        }
    }
}



