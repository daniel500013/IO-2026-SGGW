# Scenariusze testowe — Sprawdzanie Kolokwiów

**Projekt:** Sprawdzanie Kolokwiów (IO_2026_SGGW)
**Przedmiot:** Inżynieria Oprogramowania 2026 (SGGW)
**Wersja dokumentu:** 1.0 · **Data:** 2026-06-22

---

## 1. Cel i zakres

Dokument opisuje scenariusze testowe weryfikujące poprawność aplikacji do automatycznego
sprawdzania kolokwiów. Scenariusze obejmują poziomy:

- **jednostkowy (J)** — pojedyncze klasy logiki (`AnswerKeyLoader`, `CsFileRules`, `SolutionCompiler`, `SolutionRunner`),
- **integracyjny (I)** — współpraca komponentów w procesie oceny (`GradingService`),
- **akceptacyjny (A)** — interakcja użytkownika z interfejsem (scenariusze manualne).

## 2. Środowisko testowe

| Element | Wartość |
|---------|---------|
| System | Windows 10/11 |
| Platforma | .NET Framework 4.7.2 |
| Framework testowy | xUnit 2.9 |
| Uruchomienie testów automatycznych | `dotnet test IO_2026_SGGW/IO_2026_SGGW.sln` |
| Dane testowe | generowane w locie (tymczasowe pliki XLSX/.cs) |

## 3. Oznaczenia

- **Typ:** J = jednostkowy, I = integracyjny, A = akceptacyjny (manualny).
- **Automatyzacja:** nazwa testu xUnit pokrywającego scenariusz lub „manualny".
- Szczegółowe wyniki wykonania (zaliczony/niezaliczony) znajdują się w osobnym dokumencie
  *Protokoły testowe* / *Rezultaty testów*.

---

## 4. Scenariusze

### 4.1. Wczytywanie klucza odpowiedzi (XLSX)

| ID | Scenariusz | Dane wejściowe / kroki | Oczekiwany rezultat | Typ | Automatyzacja |
|----|------------|------------------------|---------------------|-----|---------------|
| ST-A01 | Wczytanie poprawnego klucza | Plik XLSX: 1 arkusz „Zadanie1", wiersze z parametrami i odpowiedziami | Powstaje `AnswerKey` z 1 zadaniem i właściwą liczbą przypadków | J | `Load_DwaWiersze_WczytujeOba` |
| ST-A02 | Nieistniejący plik | Ścieżka do nieistniejącego pliku | Wyjątek `FileNotFoundException` | J | `Load_ZlaSciezka_RzucaFileNotFound` |
| ST-A03 | Pusty arkusz (brak danych) | Arkusz tylko z nagłówkiem | Wyjątek `InvalidDataException` | J | `Load_PustyPlik_RzucaInvalidData` |
| ST-A04 | Liczba dziesiętna niezależna od kultury | Komórka `3.14`, kultura systemu `pl-PL` | Parametr zapisany jako `"3.14"` (kropka) | J | `Load_Liczba314_CzytanaZKropka_PodKulturaPL` |
| ST-A05 | Komórka typu data | Komórka `2026-04-01` (DateTime) | Parametr w formacie ISO `"2026-04-01"` | J | `Load_KomorkaData_FormatISO` |
| ST-A06 | Wiele przypadków w zadaniu | Arkusz z 2 wierszami danych | `Tasks[0].TestCases.Count == 2` | J | `Load_DwaWiersze_WczytujeOba` |
| ST-A07 | Wybór klucza w GUI | Klik „Wybierz plik XLSX" → wybór poprawnego pliku | Pasek statusu pokazuje nazwę pliku; klucz gotowy | A | manualny |
| ST-A08 | Błędny plik w GUI | Wybór uszkodzonego/niepoprawnego XLSX | Komunikat o błędzie, aplikacja nie przerywa działania, klucz = brak | A | manualny |

### 4.2. Dodawanie i walidacja plików `.cs`

| ID | Scenariusz | Dane wejściowe / kroki | Oczekiwany rezultat | Typ | Automatyzacja |
|----|------------|------------------------|---------------------|-----|---------------|
| ST-B01 | Akceptacja pliku `.cs` | Plik `Student.cs` | Plik zaakceptowany | J | `PlikCs_Akceptowany` |
| ST-B02 | Odrzucenie pliku `.txt` | Plik `notatka.txt` | Plik odrzucony | J | `PlikTxt_Odrzucony` |
| ST-B03 | Odrzucenie duplikatu | Plik już obecny na liście | Plik odrzucony | J | `Duplikat_Odrzucony` |
| ST-B04 | Odrzucenie folderu z „.cs" w nazwie | Katalog `Testy.CS` | Odrzucony (to nie plik) | J | `FolderZ_cs_wNazwie_Odrzucony` |
| ST-B05 | Drag&drop wielu plików | Przeciągnięcie kilku `.cs` na panel | Panel podświetla się na zielono, pliki dodane | A | manualny |
| ST-B06 | Wybór przez okno dialogowe | Klik panelu → multi-select `.cs` | Wszystkie wybrane pliki dodane | A | manualny |
| ST-B07 | Identyfikator studenta | Dodanie pliku `JanKowalski.cs` | Student = `JanKowalski` (nazwa bez rozszerzenia) | A | manualny |

### 4.3. Kompilacja rozwiązań (Roslyn)

| ID | Scenariusz | Dane wejściowe / kroki | Oczekiwany rezultat | Typ | Automatyzacja |
|----|------------|------------------------|---------------------|-----|---------------|
| ST-C01 | Poprawny kod | `public class X { public int F() => 42; }` | Kompilacja OK, niepusty `Assembly` | J | `Compile_PoprawnyKod_Sukces` |
| ST-C02 | Błędna składnia | `public class X { broken }` | Kompilacja nieudana | J | `Compile_BlednaSkladnia_Porazka` |
| ST-C03 | Kod z LINQ | `using System.Linq; ... a.Sum();` | Kompilacja OK | J | `Compile_Linq_Dziala` |
| ST-C04 | Bezpiecznik na groźne API | Kod z `System.Environment.Exit(0)` | Kompilacja odrzucona | J | `Compile_GrozneApi_Odrzucone` |

### 4.4. Wyszukiwanie metody, parsowanie parametrów

| ID | Scenariusz | Dane wejściowe / kroki | Oczekiwany rezultat | Typ | Automatyzacja |
|----|------------|------------------------|---------------------|-----|---------------|
| ST-D01 | Normalizacja nazwy zadania | Zadanie „Zadanie 1a", metoda `Zadanie1a` | Metoda znaleziona (po znormalizowanej nazwie) | J | `FindMethod_NormalizujeNazwe` |
| ST-D02 | Brak metody dla zadania | Zadanie bez odpowiadającej metody | Wynik `null` | J | `FindMethod_BrakMetody_Null` |
| ST-E01 | Parametry skalarne | `"5, 10"` dla `F(int,int)` | Argumenty `[5, 10]` | J | `ParseArgs_Skalary` |
| ST-E02 | Tablica i skalar | `"[1,2,3], 5"` dla `F(int[],int)` | `[1,2,3]` oraz `5` | J | `ParseArgs_TablicaISkalar` |
| ST-E03 | Niepoprawny format parametrów | Parametry niezgodne z sygnaturą metody | Status `ZlyFormatParametrow`, 0 pkt | I | manualny |

### 4.5. Wykonanie z limitem czasu i weryfikacja wyniku

| ID | Scenariusz | Dane wejściowe / kroki | Oczekiwany rezultat | Typ | Automatyzacja |
|----|------------|------------------------|---------------------|-----|---------------|
| ST-F01 | Nieskończona pętla | Metoda `while(true){}`, timeout 200 ms | Status `Timeout` | J | `Invoke_NieskonczonaPetla_Timeout` |
| ST-F02 | Metoda rzuca wyjątek | Metoda `throw new Exception("boom")` | Status `Wyjatek` | J | `Invoke_RzucaWyjatek_StatusWyjatek` |
| ST-G01 | Tolerancja dla liczb zmiennoprzecinkowych | Wynik `1.0000001` vs oczekiwany `1.0` | Uznane za poprawne (tolerancja 1e-6) | J | `IsCorrect_DoubleTolerancja` |
| ST-G02 | Porównanie tablic | Wynik `[1,2,3]` vs `"[1,2,3]"` | Uznane za poprawne | J | `IsCorrect_Tablica` |
| ST-G03 | Bool podany jako „1" | Wynik `true` vs oczekiwany `"1"` | Uznane za poprawne | J | `IsCorrect_BoolJako1` |

### 4.6. Proces oceny end-to-end (`GradingService`)

| ID | Scenariusz | Dane wejściowe / kroki | Oczekiwany rezultat | Typ | Automatyzacja |
|----|------------|------------------------|---------------------|-----|---------------|
| ST-H01 | Poprawne rozwiązanie | `Zadanie1(a,b)=>a+b`, klucz `5,10 → 15` | Status `Ok`, 1 pkt | I | `RunAsync_PoprawneRozwiazanie_StatusOk` |
| ST-H02 | Błąd kompilacji | Kod nie kompiluje się | Wszystkie wiersze `BladKompilacji` | I | `RunAsync_BladKompilacji_WszystkieWiersze` |
| ST-H03 | Brak metody | Brak metody dla zadania | Status `BrakMetody` | I | `RunAsync_BrakMetody` |
| ST-H04 | Raportowanie postępu | Sprawdzenie 1 studenta/1 przypadku | Postęp osiąga 100% | I | `RunAsync_Progres_DochodziDo100` |
| ST-H05 | Izolacja błędnego rozwiązania | Student z pętlą + student poprawny, timeout 300 ms | Pętla → `Timeout`, drugi → `Ok` (nie blokuje) | I | `RunAsync_PetlaNieskonczona_NieBlokujePozostalych` |
| ST-H06 | Błędny wynik | Wynik metody różny od oczekiwanego | Status `Bledny`, 0 pkt | I | manualny |

### 4.7. Prezentacja i eksport wyników (GUI)

| ID | Scenariusz | Dane wejściowe / kroki | Oczekiwany rezultat | Typ | Automatyzacja |
|----|------------|------------------------|---------------------|-----|---------------|
| ST-I01 | Walidacja przed sprawdzaniem | Klik „Sprawdź" bez plików lub bez klucza | Komunikat „Brak plików .cs" / „Wybierz klucz XLSX" | A | manualny |
| ST-I02 | Kolorowanie wierszy wg statusu | Wyniki z różnymi statusami | `Ok`=zielony, `Bledny`=czerwony, `Timeout`=pomarańczowy itd. | A | manualny |
| ST-I03 | Eksport wyników | Klik „Eksportuj do .xlsx" → zapis | Plik XLSX z arkuszami „Wyniki" i „Podsumowanie" | A | manualny |
| ST-I04 | Eksport bez wyników | Klik „Eksportuj" przy pustej tabeli | Komunikat „Brak wyników do eksportu" | A | manualny |

---

## 5. Podsumowanie pokrycia

| Poziom | Liczba scenariuszy | Pokryte testem automatycznym |
|--------|--------------------|------------------------------|
| Jednostkowe (J) | 23 | 23 |
| Integracyjne (I) | 7 | 5 |
| Akceptacyjne (A, manualne) | 9 | 0 |
| **Razem** | **39** | **28** |

Aplikacja posiada **28 testów automatycznych** xUnit. Część scenariuszy mapuje się na ten sam test
(np. ST-A01 i ST-A06 → `Load_DwaWiersze_WczytujeOba`). Scenariusze akceptacyjne (GUI) wykonywane są
manualnie wg kroków z tabel powyżej.

## 6. Uwagi

- Scenariusze **ST-C04** (bezpiecznik na groźne API) oraz **ST-G03** (bool jako „1") opisują
  funkcje **zaplanowane, jeszcze niezaimplementowane** — odpowiadające im testy są celowo „czerwone"
  do czasu wdrożenia (regresja sterowana testami).
- Aktualny stan wykonania testów automatycznych oraz dowody (logi) opisuje dokument
  *Protokoły testowe* / *Rezultaty testów*.
