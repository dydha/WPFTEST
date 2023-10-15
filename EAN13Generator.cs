using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFTEST
{
   public   class EAN13Generator
    {
        private readonly Random random = new Random();
        private readonly List<string> generatedCodes = new List<string>();
        private static EAN13Generator? instance;

        private EAN13Generator() { }

        public static EAN13Generator Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EAN13Generator();
                }
                return instance;
            }
        }
        public  string GenerateRandomEAN13()
        {
            while (true)
            {
                string randomCode = GenerateRandomDigits(12);
                string checkDigit = CalculateEAN13CheckDigit(randomCode);

                string ean13 = randomCode + checkDigit;

                if (!generatedCodes.Contains(ean13))
                {
                    generatedCodes.Add(ean13);
                    return ean13;
                }
            }
        }

        private string GenerateRandomDigits(int length)
        {
            string digits = "";
            for (int i = 0; i < length; i++)
            {
                digits += random.Next(10).ToString();
            }
            return digits;
        }

        private string CalculateEAN13CheckDigit(string code)
        {
            if (code.Length != 12)
                throw new ArgumentException("EAN-13 code should be 12 digits long.");

            int sumEven = 0;
            int sumOdd = 0;

            for (int i = 0; i < code.Length; i++)
            {
                int digit = int.Parse(code[i].ToString());
                if ((i % 2) == 0)
                    sumEven += digit;
                else
                    sumOdd += digit;
            }

            int totalSum = (sumEven * 3) + sumOdd;
            int checkDigit = 10 - (totalSum % 10);
            if (checkDigit == 10)
                checkDigit = 0;

            return checkDigit.ToString();
        }
    }
}
