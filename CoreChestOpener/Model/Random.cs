using System;
using System.Security.Cryptography;

namespace CoreChestOpener.Model
{
    public class Random
    {
        public int decimalCases { get; set; }
        private static int encapsulatedDecimalCase;
        public Random(int decimalCases)
        {
            this.decimalCases = decimalCases;
            encapsulatedDecimalCase = decimalCases;
        }
        public static int Generate()
        {
            return RNGCryptoServiceProvider.GetInt32((int)(100 * Math.Pow(10, encapsulatedDecimalCase)));
        }
    }
}