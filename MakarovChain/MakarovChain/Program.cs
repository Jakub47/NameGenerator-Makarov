using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.FileIO;


namespace MakarovChain
{
    public class Program
    {
        private static List<string> Names = new List<string>();
        private static Dictionary<string, Dictionary<string, float>> SeriesValue = new Dictionary<string, Dictionary<string, float>>();
        private static List<string> NamesToReturn;
        private static string finalString = "";
        private static Dictionary<string, Dictionary<string, float>> letterValue = new Dictionary<string, Dictionary<string, float>>();
        private static Dictionary<char, Dictionary<char, float>> letterValue2 = new Dictionary<char, Dictionary<char, float>>();
        private static List<string> AllNames = new List<string>();

        static void Main(string[] args)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            Console.WriteLine("Pierwsza litera twojego nicku");
            string choice = char.ToLower(Console.ReadKey().KeyChar).ToString();
            var pathToNames = @"C:\Users\Ragnus\Desktop\PI\MakarovChain\NameGenerator-Makarov\MakarovChain\MakarovChain\imiona_pl.csv"; // Habeeb, "Dubai Media City, Dubai"
            using (TextFieldParser ParserCsv = new TextFieldParser(pathToNames))
            {
                ParserCsv.CommentTokens = new string[] { "#" };
                ParserCsv.SetDelimiters(new string[] { ",", "'" });
                ParserCsv.HasFieldsEnclosedInQuotes = true;

                ParserCsv.ReadLine();

                while (!ParserCsv.EndOfData)
                {
                    string[] entites = ParserCsv.ReadFields();

                    //To make sure i will not get all 60000 records select only words starting with given words
                    Names.Add(entites[1]);
                }
            }

            if (Names.Where(a => a[0].ToString().ToLower() == choice.ToLower()).Count() < 2 || Names.Count == 0)
            {
                Names = new List<string>();
                pathToNames = @"C:\Users\Ragnus\Desktop\PI\MakarovChain\NameGenerator-Makarov\MakarovChain\MakarovChain\babies-first-names-2010-2018.csv"; // Habeeb, "Dubai Media City, Dubai"
                using (TextFieldParser ParserCsv = new TextFieldParser(pathToNames))
                {
                    ParserCsv.CommentTokens = new string[] { "#" };
                    ParserCsv.SetDelimiters(new string[] { ",", "'" });
                    ParserCsv.HasFieldsEnclosedInQuotes = true;

                    ParserCsv.ReadLine();

                    while (!ParserCsv.EndOfData)
                    {
                        string[] entites = ParserCsv.ReadFields();
                        if (char.ToLower(entites[2][0]).ToString() == choice)
                            Names.Add(entites[2]);

                        if (char.ToLower(choice[0]) == 'q' || char.ToLower(choice[0]) == 'x')
                        {
                            if (entites[2].ToLower().Contains(char.ToLower(choice[0])))
                                Names.Add(entites[2]);
                        }
                    }

                }
            }
            //Łatwiejszy sposób na wpisanie liter od a do z zamiast robić letterValue.Add() bierzemy wartość decymalną litery od litery a [97] do z [122]
            for (float i = 97; i <= 122; i++)
            {
                letterValue.Add(((char)i).ToString(), new Dictionary<string, float>());
                letterValue2.Add((char)i, new Dictionary<char, float>());
            }

            var N2 = GetWordsN2Ver(choice);
            AllNames.AddRange(N2);
            var N1 = GetWordsN1Ver(choice);
            AllNames.AddRange(N1);
            var N0 = GetWordsN0Ver(choice[0]);
            AllNames.AddRange(N0);
            stopWatch.Stop();


            Console.WriteLine();

            Console.WriteLine();
            int counter = 0;
            foreach (var item in AllNames)
            {
                Console.Write(counter + ")" + item + "\t");
                counter++;
            }

            string sec = "";
            switch(stopWatch.Elapsed.Seconds)
            {
                case 1: sec = "sekunda"; break;
                case 2:
                case 3: sec = "sekundy"; break;
                default: sec = "sekund"; break;
            }

            Console.WriteLine(); Console.WriteLine();
            Console.WriteLine("Czas na wygenerowanie 90 słów przy wykorzystniu łańcucha markova zajął " + stopWatch.Elapsed.Seconds + " " + sec);
        }

        public static List<string> GetWordsN2Ver(string choice)
        {
            Random rand = new Random();
            NamesToReturn = new List<string>();
            int k = 0;
            //InitialsState == get second letter count all occurence get proper next index
            while (NamesToReturn.Count < 30)
            {
                SeriesValue = new Dictionary<string, Dictionary<string, float>>();
                SeriesValue = InitalState1(rand, choice);
                string getWord = " ";

                int countNuls = 0;
                int counterForNamesToReturn = 0;
                int counterForNames = 0;

                do
                {
                    if (NamesToReturn.Any(a => string.Equals(a, getWord, StringComparison.CurrentCultureIgnoreCase)))
                        counterForNamesToReturn++;

                    if (Names.Any(a => string.Equals(a, getWord, StringComparison.CurrentCultureIgnoreCase)))
                        counterForNames++;

                    getWord = GetWord1(SeriesValue, rand);
                    if (getWord == null)
                        countNuls++;

                    if (countNuls > 10)
                    {
                        SeriesValue = InitalState1(rand, choice);
                        countNuls = 0;
                    }

                    if (countNuls > 100 || counterForNamesToReturn > 100 || counterForNames > 100)
                    {
                        SeriesValue = InitalState1(rand, choice, getWord);
                        countNuls = 0;
                        counterForNamesToReturn = 0;
                        counterForNames = 0;
                    }

                } while (getWord == null || NamesToReturn.Any(a => string.Equals(a, getWord, StringComparison.CurrentCultureIgnoreCase))
                         || Names.Any(a => string.Equals(a, getWord, StringComparison.CurrentCultureIgnoreCase)));

                k++;

                NamesToReturn.Add(getWord);
            }

            return NamesToReturn;
        }
        public static List<string> GetWordsN1Ver(string choice)
        {
            Random rand = new Random();
            NamesToReturn = new List<string>();
            string finalString = "" + choice;
            while (NamesToReturn.Count <= 30)
            {
                var counterForLetter = new Dictionary<string, float>();
                GenereteValuesToKeys2(choice, counterForLetter, letterValue);
                foreach (KeyValuePair<string, Dictionary<string, float>> charWithCharValues in letterValue)
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
                string result = GetWord2(choice);

                if (result != null && !(NamesToReturn.Any(a => a == result)) && !(AllNames.Any(a => a == result)))
                    NamesToReturn.Add(result);
            }

            return NamesToReturn;
        }
        public static List<string> GetWordsN0Ver(char choice)
        {
            Random rand = new Random();
            NamesToReturn = new List<string>();
            while (NamesToReturn.Count <= 30)
            {
                var counterForLetter = new Dictionary<char, float>();
                InitializeLetterValue3(counterForLetter, letterValue2, choice);

                foreach (KeyValuePair<char, Dictionary<char, float>> charWithCharValues in letterValue2)
                {
                    float sum = 0;

                    //First loop for count all instances of letter
                    foreach (KeyValuePair<char, float> yos in charWithCharValues.Value)
                    {
                        sum += yos.Value;
                    }

                    //After loop every value will have percentage of all values meaning that for example a will be 0,2 b will be 0,09
                    for (int i = 0; i < charWithCharValues.Value.Count; i++)
                    {
                        charWithCharValues.Value[charWithCharValues.Value.ElementAt(i).Key] /= sum;
                    }
                }
                string result = GetWord3(choice);

                if (result != null && !(NamesToReturn.Any(a => a == result)) && !(AllNames.Any(a => a == result)))
                    NamesToReturn.Add(result);
            }

            return NamesToReturn;
        }


        /// <summary>
        /// Generate word using pair from initialState.
        /// </summary>
        /// <param name="seriesValue">Dictionary containg given pair and empty new dicionary</param>
        /// <param name="rand">rand object used for probability</param>
        private static string GetWord1(Dictionary<string, Dictionary<string, float>> seriesValue, Random rand)
        {
            string finalString = seriesValue.Keys.First();
            string twoLastLetters = seriesValue.Keys.First();
            int c = 0; int gg = 0; int t = 0;
            string nextLLetter = "";
            bool b1 = false; bool b2 = false;

            while (finalString.Length != 5)
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
        private static Dictionary<string, Dictionary<string, float>> InitalState1(Random rand, string choice, string returnTooMany = null)
        {
            finalString = "";
            SeriesValue.Add(choice, new Dictionary<string, float>());
            Names.ForEach(a =>
            {
                if (a.Count() > 1)
                {
                    string nextLetter = a[1].ToString();

                    if (char.IsLetter(Convert.ToChar(nextLetter)) && !(string.IsNullOrEmpty(nextLetter)))
                    {
                        if (SeriesValue[choice].ContainsKey(nextLetter))
                            SeriesValue[choice][nextLetter]++;
                        else
                            SeriesValue[choice].Add(nextLetter, 1);
                    }
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
                        finalString += currentKeyValue.Key.ToLower();
                        SeriesValue = new Dictionary<string, Dictionary<string, float>>();
                        SeriesValue.Add(choice + finalString, new Dictionary<string, float>());
                        return SeriesValue;
                    }
                }
            }

            return null;
        }

        private static string GetWord2(string choice)
        {
            string finalString = "" + choice;
            Random rand = new Random();

            while (finalString.Length != 5)
            {
                foreach (KeyValuePair<string, float> yos in letterValue[choice])
                {
                    if (rand.NextDouble() < yos.Value)
                    {
                        finalString += yos.Key;
                        choice = yos.Key;
                        break;
                    }
                }
            }

            return finalString;
        }

        private static void GenereteValuesToKeys2(string choice, Dictionary<string, float> counterForLetter, Dictionary<string, Dictionary<string, float>> letterValue)
        {
            Names.ForEach(a =>
            {
                a = a.ToLower();
                a = Regex.Replace(a, "[^0-9a-zA-Z]+", "");
                a = a.Replace(" ", "");




                if (choice.Length > 1)
                {
                    letterValue = new Dictionary<string, Dictionary<string, float>>();

                    if (a.Contains(choice))
                    {
                        string nextValue = a.Substring(a.IndexOf(choice) - 1, choice.Length + 1);
                        if (letterValue.ContainsKey(nextValue))
                            letterValue[choice][nextValue]++;
                        else
                            letterValue[choice].Add(nextValue, 1);
                    }
                }

                else
                {
                    for (int i = 0; i < a.Length - 1; i++)
                    {

                        string n = a[i].ToString();
                        string nnex = a[i + 1].ToString();

                        if (counterForLetter.ContainsKey(n))
                            counterForLetter[n]++;
                        else
                            counterForLetter.Add(n, 1);




                        if (letterValue[n].ContainsKey(nnex))
                            letterValue[n][nnex] += counterForLetter[n];
                        else
                            letterValue[n].Add(nnex, counterForLetter[n]);

                    }
                }
            });
        }

        private static void InitializeLetterValue3(Dictionary<char, float> counterForLetter, Dictionary<char, Dictionary<char, float>> letterValue, char choice)
        {
            Names.ForEach(a =>
            {
                a = a.ToLower();
                a = Regex.Replace(a, "[^0-9a-zA-Z]+", "");
                a = a.Replace(" ", "");

                for (int i = 0; i < a.Length - 1; i++)
                {
                    char n = a[i];
                    char nnex = a[i + 1];

                    if (counterForLetter.ContainsKey(n))
                        counterForLetter[n]++;
                    else
                        counterForLetter.Add(n, 1);

                    if (letterValue[n].ContainsKey(nnex))
                        letterValue[n][nnex] += counterForLetter[n];
                    else
                        letterValue[n].Add(nnex, counterForLetter[n]);


                }
            });
        }

        private static string GetWord3(char choice)
        {
            string finalString = "" + choice;
            Random rand = new Random();

            while (finalString.Length <= 6)
            {
                if (finalString.Length < 2)
                {
                    //for first letter get from choice of user
                    foreach (KeyValuePair<char, float> yos in letterValue2[choice])
                    {
                        if (finalString.Length >= 2) break;

                        if (rand.NextDouble() < yos.Value)
                        {
                            finalString += yos.Key;
                        }
                    }
                }

                foreach (KeyValuePair<char, Dictionary<char, float>> charWithCharValues in letterValue2)
                {
                    foreach (KeyValuePair<char, float> yos in charWithCharValues.Value)
                    {
                        if (finalString.Length >= 6)
                            return finalString;

                        if (rand.NextDouble() < yos.Value && finalString[finalString.Length - 1] != yos.Value)
                        {
                            finalString += yos.Key;
                        }
                    }

                }
            }

            return null;

        }
    }
}
