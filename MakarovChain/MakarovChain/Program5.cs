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
    /// The 2 vers version where a[n] look for a[n-1]
    /// </summary>
    class Program5
    {
        private static List<string> Names = new List<string>();
        private static Dictionary<string, Dictionary<string, float>> letterValue = new Dictionary<string, Dictionary<string, float>>();
        private static List<string> NamesToReturn = new List<string>();


        static void Main(string[] args)
        {
            Random rand = new Random();

            //Łatwiejszy sposób na wpisanie liter od a do z zamiast robić letterValue.Add() bierzemy wartość decymalną litery od litery a [97] do z [122]
            for (float i = 97; i <= 122; i++)
            {
                letterValue.Add(((char)i).ToString(), new Dictionary<string, float>());
            }


            Console.WriteLine("Pierwsza litera twojego nicku");
            string choice = char.ToLower(Console.ReadKey().KeyChar).ToString();


            //var pathToNames = @"C:\Users\Ragnus\Downloads\babies-first-names-2010-2018.csv"; // Habeeb, "Dubai Media City, Dubai"
            //using (TextFieldParser ParserCsv = new TextFieldParser(pathToNames))
            //{
            //    ParserCsv.CommentTokens = new string[] { "#" };
            //    ParserCsv.SetDelimiters(new string[] { "," });
            //    ParserCsv.HasFieldsEnclosedInQuotes = true;

            //    ParserCsv.ReadLine();

            //    while (!ParserCsv.EndOfData)
            //    {
            //        string[] entites = ParserCsv.ReadFields();

            //        //To make sure i will not get all 60000 records select only words starting with given words
            //        if (char.ToLower(entites[2][0]).ToString() == choice)
            //            Names.Add(entites[2]);
            //    }

            //}

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


            //// W każdym string musimy lecieć po każdej literce i sprawdzać jaka jest jej kolejna litera
            string finalString = "" + choice;

            while (NamesToReturn.Count <= 30)
            {
                var counterForLetter = new Dictionary<string, float>();
                GenereteValuesToKeys(choice, counterForLetter, letterValue);
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
                string result = GetWord(choice);

                if (result != null && !(NamesToReturn.Any(a => a == result)))
                    NamesToReturn.Add(result);
            }


            Console.WriteLine("======================");
            foreach (var item in NamesToReturn)
            {
                Console.Write(item + ",");
            }
        }

        private static void GenereteValuesToKeys(string choice, Dictionary<string, float> counterForLetter, Dictionary<string, Dictionary<string, float>> letterValue)
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


        private static string GetWord(string choice)
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
    }
}


