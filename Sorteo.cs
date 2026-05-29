using System;
using System.Linq;

namespace BalotoAppOnline
{
    public class Sorteo
    {
        public DateTime Fecha { get; set; }
        public int[] Numeros { get; set; }

        public Sorteo()
        {
            Numeros = new int[6];
        }

        public bool EsValido()
        {
            if (Numeros.Length != 6) return false;
            for (int i = 0; i < 5; i++)
                if (Numeros[i] < 1 || Numeros[i] > 43) return false;
            if (Numeros[5] < 1 || Numeros[5] > 16) return false;

            var firstFive = Numeros.Take(5).ToArray();
            return firstFive.Distinct().Count() == 5;
        }

        public override bool Equals(object obj)
        {
            if (obj is Sorteo other)
                return Fecha == other.Fecha && Numeros.SequenceEqual(other.Numeros);
            return false;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + Fecha.GetHashCode();
            for (int i = 0; i < Numeros.Length; i++)
                hash = hash * 31 + Numeros[i];
            return hash;
        }
    }
}