using System;
using System.Linq;

public class Rozwiazanie
{
    public int Zadanie1(int a, int b) { return a - b; }  // BLAD: odejmowanie
    public int Zadanie2(int a, int b) { return a + b; }  // BLAD: suma zamiast max
    public int Zadanie3(int[] arr) { return arr.Length; }  // BLAD: zwraca dlugosc
    public int Zadanie4(int[] arr) { return arr[0]; }  // BLAD: zwraca pierwszy element
    public double Zadanie5(int[] arr) { return 0.0; }  // BLAD: zawsze 0
    public int Zadanie6(int n) { return n; }  // BLAD: zwraca n
    public int Zadanie7(int n) { return 1; }  // BLAD: zawsze 1
    public bool Zadanie8(string s) { return false; }  // BLAD: zawsze false
    public string Zadanie9(string s) { return s.ToUpper(); }  // BLAD: upper zamiast reverse
    public int Zadanie10(int[] arr) { return 0; }  // BLAD: zawsze 0
}
