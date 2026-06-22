# Protokoły testowe — Sprawdzanie Kolokwiów

**Projekt:** Sprawdzanie Kolokwiów (IO_2026_SGGW)
**Przedmiot:** Inżynieria Oprogramowania 2026 (SGGW)
**Wersja dokumentu:** 1.0

---

## 1. Informacje o przebiegu testów

| Pole | Wartość |
|------|---------|
| Data i godzina | 2026-06-22, 22:04:09 – 22:04:16 |
| Środowisko | Windows 10, .NET Framework 4.7.2 |
| Narzędzia | .NET SDK 9.0.313, xUnit 2.9.2 |
| Polecenie | `dotnet test IO_2026_SGGW/IO_2026_SGGW.sln --logger "trx;LogFileName=protokol.trx"` |
| Surowy raport (dowód) | `docs/protokol.trx` |
| Rodzaj testów | jednostkowe i integracyjne (automatyczne) |

## 2. Wynik zbiorczy

| Metryka | Wartość |
|---------|---------|
| Liczba testów | 28 |
| Zaliczone | **24** |
| Niezaliczone | **4** |
| Pominięte | 0 |
| Czas wykonania | ~3 s |

Powiązanie scenariuszy z testami opisuje dokument *Scenariusze testowe* (kolumna „Automatyzacja").

---

## 3. Wyniki szczegółowe

Legenda wyniku: **Z** = zaliczony, **N** = niezaliczony.

### 3.1. Wczytywanie klucza odpowiedzi (`AnswerKeyLoaderTests`)

| Lp | Test | Scenariusz | Wynik | Czas [ms] |
|----|------|-----------|:-----:|----------:|
| 1 | Load_ZlaSciezka_RzucaFileNotFound | ST-A02 | Z | 0 |
| 2 | Load_PustyPlik_RzucaInvalidData | ST-A03 | Z | 10 |
| 3 | Load_DwaWiersze_WczytujeOba | ST-A01 / ST-A06 | Z | 20 |
| 4 | Load_Liczba314_CzytanaZKropka_PodKulturaPL | ST-A04 | **N** | 2820 |
| 5 | Load_KomorkaData_FormatISO | ST-A05 | **N** | 40 |

### 3.2. Walidacja plików `.cs` (`CsFileRulesTests`)

| Lp | Test | Scenariusz | Wynik | Czas [ms] |
|----|------|-----------|:-----:|----------:|
| 6 | PlikCs_Akceptowany | ST-B01 | Z | 0 |
| 7 | PlikTxt_Odrzucony | ST-B02 | Z | 0 |
| 8 | Duplikat_Odrzucony | ST-B03 | Z | 390 |
| 9 | FolderZ_cs_wNazwie_Odrzucony | ST-B04 | Z | 0 |

### 3.3. Silnik: kompilacja, wyszukiwanie, parsowanie, wykonanie (`SolutionEngineTests`)

| Lp | Test | Scenariusz | Wynik | Czas [ms] |
|----|------|-----------|:-----:|----------:|
| 10 | Compile_PoprawnyKod_Sukces | ST-C01 | Z | 30 |
| 11 | Compile_BlednaSkladnia_Porazka | ST-C02 | Z | 10 |
| 12 | Compile_Linq_Dziala | ST-C03 | Z | 140 |
| 13 | Compile_GrozneApi_Odrzucone | ST-C04 | **N** | 2720 |
| 14 | FindMethod_NormalizujeNazwe | ST-D01 | Z | 20 |
| 15 | FindMethod_BrakMetody_Null | ST-D02 | Z | 20 |
| 16 | ParseArgs_Skalary | ST-E01 | Z | 60 |
| 17 | ParseArgs_TablicaISkalar | ST-E02 | Z | 20 |
| 18 | Invoke_NieskonczonaPetla_Timeout | ST-F01 | Z | 250 |
| 19 | Invoke_RzucaWyjatek_StatusWyjatek | ST-F02 | Z | 140 |
| 20 | IsCorrect_DoubleTolerancja | ST-G01 | Z | 0 |
| 21 | IsCorrect_Tablica | ST-G02 | Z | 0 |
| 22 | IsCorrect_BoolJako1 | ST-G03 | **N** | 0 |

### 3.4. Proces oceny end-to-end (`GradingServiceTests`)

| Lp | Test | Scenariusz | Wynik | Czas [ms] |
|----|------|-----------|:-----:|----------:|
| 23 | RunAsync_PoprawneRozwiazanie_StatusOk | ST-H01 | Z | 70 |
| 24 | RunAsync_BladKompilacji_WszystkieWiersze | ST-H02 | Z | 40 |
| 25 | RunAsync_BrakMetody | ST-H03 | Z | 20 |
| 26 | RunAsync_Progres_DochodziDo100 | ST-H04 | Z | 20 |
| 27 | RunAsync_PetlaNieskonczona_NieBlokujePozostalych | ST-H05 | Z | 3070 |

### 3.5. Pozostałe

| Lp | Test | Scenariusz | Wynik | Czas [ms] |
|----|------|-----------|:-----:|----------:|
| 28 | UnitTest1.Test1 | (szablon) | Z | 0 |

---

## 4. Rejestr niepowodzeń

### N-1. Load_Liczba314_CzytanaZKropka_PodKulturaPL (ST-A04) — defekt

- **Oczekiwano:** `"3.14"`
- **Uzyskano:** `"3,14"`
- **Przyczyna:** liczba z komórki jest formatowana wg bieżącej kultury (`pl-PL` → przecinek) zamiast kultury niezmiennej (kropka).
- **Zalecenie:** formatować wartości liczbowe przez `CultureInfo.InvariantCulture`.

### N-2. Load_KomorkaData_FormatISO (ST-A05) — defekt

- **Oczekiwano:** `"2026-04-01"`
- **Uzyskano:** `"4/1/2026 12:00:00 AM"`
- **Przyczyna:** data jest formatowana domyślnym formatem kultury zamiast ISO `yyyy-MM-dd`.
- **Zalecenie:** dla komórek typu data użyć formatu `yyyy-MM-dd`.

### N-3. Compile_GrozneApi_Odrzucone (ST-C04) — funkcja zaplanowana, niewdrożona

- **Oczekiwano:** `False` (kompilacja odrzucona)
- **Uzyskano:** `True` (kod z `System.Environment.Exit(0)` kompiluje się)
- **Status:** test regresyjny celowo „czerwony" do czasu wdrożenia bezpiecznika na groźne API (zgodnie z dokumentem scenariuszy).

### N-4. IsCorrect_BoolJako1 (ST-G03) — funkcja zaplanowana, niewdrożona

- **Oczekiwano:** `True` (wartość `true` zgodna z kluczem `"1"`)
- **Uzyskano:** `False`
- **Status:** test regresyjny celowo „czerwony” do czasu wdrożenia porównywania wartości logicznych zapisanych jako `0/1`.

---

## 5. Wnioski

- **24 z 28 testów (86%) przechodzi.** Rdzeń logiki (kompilacja, wyszukiwanie metod, parsowanie, limit czasu,
  obsługa wyjątków, proces oceny) działa poprawnie.
- **2 defekty** (N-1, N-2) dotyczą formatowania zależnego od kultury przy wczytywaniu klucza — proste do naprawy.
- **2 niepowodzenia** (N-3, N-4) to świadomie zaplanowane funkcje jeszcze niezaimplementowane (rozwój sterowany testami), nie regresja.
- Po usunięciu 2 defektów i wdrożeniu 2 funkcji oczekiwany wynik to **28/28**.
