// <copyright file="StringExtensions.cs" company="KPMG">
// Copyright (c) KPMG. All rights reserved.
// </copyright>

namespace KPMG.Pulse.Back.Accounting.Mandate.Adapters
{
    public static class StringExtensions
    {
        public static string Extract(this string input, int index, int len)
        {
            if (string.IsNullOrEmpty(input) || input.Length < len)
            {
                return input;
            }

            return input.Substring(index, len);
        }

        public static bool AllCharactersUpperCase(string chaine)
        {
            if (string.IsNullOrEmpty(chaine))
            {
                return false;
            }

            foreach (char character in chaine)
            {
                if (!char.IsUpper(character))
                {
                    return false;
                }
            }

            return true;
        }

        public static (string sexe, string nom, string prenom) ExtractPersonInfo(this string chaine)
        {
            string[] mots = chaine.Split(' ');

            // Le premier mot est le sexe
            string sexe = mots[0];
            mots = mots.AsEnumerable().Skip(1).ToArray();

            List<string> nomArray = new List<string>();
            List<string> prenomArray = new List<string>();

            for (int i = 0; i < mots.Length; i++)
            {
                if (AllCharactersUpperCase(mots[i]))
                {
                    nomArray.Add(mots[i]);
                }
                else
                {
                    prenomArray.Add(mots[i]);
                }
            }

            string nom = string.Join(" ", nomArray);
            string prenom = string.Join(" ", prenomArray);

            return (sexe, nom, prenom);
        }

        public static string Sanitize(this string str)
        {
            return str.Replace("'", " ");
        }
    }
}
