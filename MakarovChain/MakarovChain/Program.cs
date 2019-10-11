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
        private static Dictionary<char, Dictionary<char, float>> letterValue = new Dictionary<char, Dictionary<char, float> >();

        static void Main(string[] args)
        {
            
            //Łatwiejszy sposób na wpisanie liter od a do z zamiast robić letterValue.Add() bierzemy wartość decymalną litery od litery a [97] do z [122]
            for (float i = 97; i <= 122; i++)
            {
                letterValue.Add((char)i, new Dictionary<char, float>());
            }

            Console.WriteLine("Pierwsza litera twojego nicku");
            char choice = char.ToLower(Console.ReadKey().KeyChar);

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
                    if ( char.ToLower(entites[2][0]) == choice)
                        Names.Add(entites[2]);
                }

            }

            // W każdym string musimy lecieć po każdej literce i sprawdzać jaka jest jej kolejna litera
            var counterForLetter = new Dictionary<char, float>();

            Names.ForEach(a => 
            {
                a = a.ToLower();
                a  = Regex.Replace(a, "[^0-9a-zA-Z]+", "");
                a = a.Replace(" ", "");

                for (int i = 0;i < a.Length - 1;i++)
                {
                    char n = a[i];
                    char nnex = a[i + 1];

                    if (counterForLetter.ContainsKey(n))
                        counterForLetter[n]++;
                    else
                        counterForLetter.Add(n, 1);

                    


                    if(letterValue[n].ContainsKey(nnex))
                        letterValue[n][nnex] +=  counterForLetter[n];
                    else
                        letterValue[n].Add(nnex, counterForLetter[n]);


                }
            });

            
            foreach (KeyValuePair<char, Dictionary<char, float>> charWithCharValues in letterValue)
            {
                float sum = 0;

                //First loop for count all instances of letter
                foreach(KeyValuePair<char, float> yos in charWithCharValues.Value)
                {
                    sum += yos.Value;
                }

                //After loop every value will have percentage of all values meaning that for example a will be 0,2 b will be 0,09
                for (int i = 0; i < charWithCharValues.Value.Count; i++)
                {
                    charWithCharValues.Value[charWithCharValues.Value.ElementAt(i).Key] /= sum;
                }    
            }


            string finalString = "" + choice;
            Random rand = new Random();


            // Na początku weź pierwszą literę (wybraną przez użytkoniak) z najwikszym prawodopodobieństem wybrania później znaleźć dla niej itd tak do osiągnięccia 
            //Zadanej długościś

            //Zastanowić się nad wzięciem zamiast 1 litery 
            //2 a p otem 3 liter i dla nich zrobić tę pętlę
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
                            goto Found;

                        if (rand.NextDouble() < yos.Value && finalString[finalString.Length - 1] != yos.Value)
                        {
                            finalString += yos.Key;
                        }
                    }

                }
            }

            
            Found:
            Console.WriteLine();
            Console.WriteLine(finalString);
        }
    }
}



