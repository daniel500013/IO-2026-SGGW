using System;
using System.Linq;

public class Rozwiazanie
{
    public int Zadanie1(int a, int b) { return a + b; }
    public int Zadanie2(int a, int b) { return Math.Max(a, b); }
    public int Zadanie3(int[] arr) { return arr.Sum(); }
    public int Zadanie4(int[] arr) { return arr.Max(); }
    public double Zadanie5(int[] arr) { return arr.Average(); }
    public int Zadanie6(int n) { return n == 0 ? 1 : n * Zadanie6(n - 1); }
    public int Zadanie7(int n)
    {
        if (n <= 1) return n;
        return Zadanie7(n - 1) + Zadanie7(n - 2);
    }
    public bool Zadanie8(string s)
    {
        for (int i = 0; i < s.Length / 2; i++)
            if (s[i] != s[s.Length - 1 - i]) return false;
        return true;
    }
    public string Zadanie9(string s) { return new string(s.Reverse().ToArray()); }
    public int Zadanie10(int[] arr) { return arr.Count(x => x % 2 == 0); }
}
