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
    /// <summary>
    /// The simples/quickest version where a[n] does not look for a[n-1] get 30 such names
    /// *1
    /// </summary>
    class Program3
    {
        private static List<string> Names = new List<string>();
        private static Dictionary<char, Dictionary<char, float>> letterValue = new Dictionary<char, Dictionary<char, float>>();
        private static List<string> NamesToReturn = new List<string>();

        static void Main3(string[] args)
        {

            //Łatwiejszy sposób na wpisanie liter od a do z zamiast robić letterValue.Add() bierzemy wartość decymalną litery od litery a [97] do z [122]
            for (float i = 97; i <= 122; i++)
            {
                letterValue.Add((char)i, new Dictionary<char, float>());
            }

            Console.WriteLine("Pierwsza litera twojego nicku");
            char choice = char.ToLower(Console.ReadKey().KeyChar);

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


            while (NamesToReturn.Count <= 30)
            {
                var counterForLetter = new Dictionary<char, float>();
                InitializeLetterValue3(counterForLetter, letterValue, choice);

                foreach (KeyValuePair<char, Dictionary<char, float>> charWithCharValues in letterValue)
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

                if (result != null && !(NamesToReturn.Any(a => a == result)))
                    NamesToReturn.Add(result);
            }

            Console.WriteLine("======================");
            foreach (var item in NamesToReturn)
            {
                Console.Write(item + ",");
            }
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
                    foreach (KeyValuePair<char, float> yos in letterValue[choice])
                    {
                        if (finalString.Length >= 2) break;

                        if (rand.NextDouble() < yos.Value)
                        {
                            finalString += yos.Key;
                        }
                    }
                }

                foreach (KeyValuePair<char, Dictionary<char, float>> charWithCharValues in letterValue)
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


