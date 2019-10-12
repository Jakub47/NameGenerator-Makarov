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
    class Program2
    {
        private static List<string> Names = new List<string>();
        private static Dictionary<string, Dictionary<string, float>> letterValue = new Dictionary<string, Dictionary<string, float>>();

        static void Main2(string[] args)
        {
            Random rand = new Random();

            //Łatwiejszy sposób na wpisanie liter od a do z zamiast robić letterValue.Add() bierzemy wartość decymalną litery od litery a [97] do z [122]
            for (float i = 97; i <= 122; i++)
            {
                letterValue.Add(((char)i).ToString(), new Dictionary<string, float>());
            }


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


            //// W każdym string musimy lecieć po każdej literce i sprawdzać jaka jest jej kolejna litera
            var counterForLetter = new Dictionary<string, float>();
            string finalString = "" + choice;

            while (finalString.Length != 5)
            {
                //If choice is len larger than 1 it means that word is for example na and now we have to store values for na not single key
                if (choice.Length > 1)
                    letterValue = new Dictionary<string, Dictionary<string, float>>();

                


                Names.ForEach(a =>
                {
                    a = a.ToLower();
                    a = Regex.Replace(a, "[^0-9a-zA-Z]+", "");
                    a = a.Replace(" ", "");

                    if (choice.Length > 1)
                    {
                        if (choice.Length > 3)
                        {
                            string tempChoice = choice.Substring(choice.Length - 3, 3);
                            if (a.Contains(tempChoice))
                            {
                                if (a.IndexOf(tempChoice) + tempChoice.Length + 1 < a.Length)
                                {

                                    string nextValue = a.Substring(a.IndexOf(tempChoice), tempChoice.Length + 1);
                                    if (!letterValue.ContainsKey(tempChoice))
                                        letterValue.Add(tempChoice, new Dictionary<string, float>());


                                    if (letterValue[tempChoice].ContainsKey(nextValue))
                                        letterValue[tempChoice][nextValue]++;
                                    else
                                    {
                                        letterValue[tempChoice].Add(nextValue, 1);
                                    }
                                }
                            }
                        }


                        if (a.Contains(choice))
                        {
                            if (a.IndexOf(choice) + choice.Length + 1 < a.Length)
                            {

                                string nextValue = a.Substring(a.IndexOf(choice), choice.Length + 1);
                                if (!letterValue.ContainsKey(choice))
                                    letterValue.Add(choice, new Dictionary<string, float>());


                                if (letterValue[choice].ContainsKey(nextValue))
                                    letterValue[choice][nextValue]++;
                                else
                                {
                                    letterValue[choice].Add(nextValue, 1);
                                }
                            }
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

                if (choice.Length > 1)
                {
                    Console.WriteLine("yos");
                }

                foreach (KeyValuePair<string, float> yos in letterValue[choice])
                {
                    if (rand.NextDouble() < yos.Value)
                    {
                        string LastLetter = yos.Key.Last().ToString(); 
                        finalString += LastLetter;
                        choice += LastLetter;
                        break;
                    }
                }

            }

            Console.WriteLine();
            Console.WriteLine(finalString);
        }

        private static void GenereteValuesToKeys(string choice, Dictionary<string, float> counterForLetter, Dictionary<string, Dictionary<string, float>> letterValue)
        {
            //If choice is len larger than 1 it means that word is for example na and now we have to store values for na not single key
            if (choice.Length > 1)
                letterValue = new Dictionary<string, Dictionary<string, float>>();

            Names.ForEach(a =>
            {
                a = a.ToLower();
                a = Regex.Replace(a, "[^0-9a-zA-Z]+", "");
                a = a.Replace(" ", "");

                if (choice.Length > 1)
                {
                    if (a.Contains(choice))
                    {
                        if (a.IndexOf(choice) + choice.Length + 1 < choice.Length)
                        {

                            string nextValue = a.Substring(a.IndexOf(choice), choice.Length + 1);
                            if (!letterValue.ContainsKey(choice))
                                letterValue.Add(choice, new Dictionary<string, float>());


                            if (letterValue[choice].ContainsKey(nextValue))
                                letterValue[choice][nextValue]++;
                            else
                            {
                                letterValue[choice].Add(nextValue, 1);
                            }
                        }
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

    }
}
