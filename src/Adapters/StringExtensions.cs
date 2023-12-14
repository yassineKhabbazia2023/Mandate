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
            if (!IsValid(chaine))
            {
                throw new InvalidOperationException($"{nameof(StringExtensions)} - la chaine {chaine} n'est pas valide");
            }

            string[] mots = chaine.Split(' ');

            // Le premier mot est le sexe
            string sexe = mots[0];
            mots = mots.ToList().Skip(1).ToArray();

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

        private static bool IsValid(string chaine)
        {
            // Critère 1: Au moins 3 mots séparés par des espaces
            string[] mots = chaine.Split(' ');
            if (mots.Length < 3)
            {
                return false;
            }

            // Critère 2: Le premier mot doit avoir "m" ou "mme" comme valeurs
            if (!(mots[0].Equals("m", StringComparison.OrdinalIgnoreCase) || mots[0].Equals("mme", StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            // Critère 3: Au moins un mot en majuscules et un mot en minuscules
            if (!mots.Any(m => m.Any(char.IsUpper)) || !mots.Any(m => m.Any(char.IsLower)))
            {
                return false;
            }

            return true;
        }
    }
}
