# Sprawdzanie Kolokwiów

Desktopowa aplikacja **Windows Forms (.NET Framework 4.7.2)** do **automatycznego sprawdzania kolokwiów** z programowania w języku C#. Prowadzący wczytuje klucz odpowiedzi z pliku Excel, dodaje pliki źródłowe `.cs` studentów, a aplikacja kompiluje je w pamięci (Roslyn), uruchamia odpowiednie metody na zestawie przypadków testowych, ocenia wyniki i prezentuje je w tabeli z możliwością eksportu do Excela.

Projekt realizowany w ramach przedmiotu **Inżynieria Oprogramowania 2026 (SGGW)**.

---

## Wymagania

- **Windows** (aplikacja okienkowa WinForms)
- **.NET Framework 4.7.2** (do uruchomienia gotowej aplikacji)
- Do budowania ze źródeł - jedno z:
  - **Visual Studio 2022** z obsługą .NET desktop development, albo
  - **.NET SDK** (`dotnet`) - wystarczy CLI; pakiety reference assemblies dla net472 pobierają się automatycznie z NuGet

Zależności (NuGet, pobierane automatycznie przy `restore`):

| Pakiet | Wersja | Rola |
|--------|--------|------|
| `Microsoft.CodeAnalysis.CSharp` (Roslyn) | 4.8.0 | kompilacja kodu studentów w pamięci |
| `ClosedXML` | 0.102.2 | odczyt klucza i eksport wyników (XLSX) |
| `System.Resources.Extensions` | 8.0.0 | zasoby WinForms przy budowaniu net472 |
| `Microsoft.NETFramework.ReferenceAssemblies` | 1.0.3 | budowanie net472 bez instalowania targeting packa |

---

## Budowanie i uruchamianie

### Visual Studio
1. Otwórz `IO_2026_SGGW/IO_2026_SGGW.sln`.
2. Ustaw `IO_2026_SGGW` jako projekt startowy.
3. Naciśnij **F5** (uruchom) lub **Ctrl+Shift+B** (zbuduj).

### Wiersz poleceń (dotnet CLI)
```bash
# Budowanie całej solucji
dotnet build IO_2026_SGGW/IO_2026_SGGW.sln

# Uruchomienie aplikacji (po zbudowaniu)
IO_2026_SGGW/bin/Debug/net472/IO_2026_SGGW.exe
```

---

## Uruchamianie testów

Projekt testów: `IO_2026_SGGW.Tests` (xUnit).

```bash
dotnet test IO_2026_SGGW/IO_2026_SGGW.sln
```

---

## Instrukcja obsługi

### Okno główne

Po uruchomieniu aplikacji pojawia się jedno okno z wszystkimi funkcjami:

![Okno główne aplikacji (stan początkowy)](IO_2026_SGGW/s1.png)

| Element | Opis |
|---------|------|
| **Panel „Przeciągnij pliki .cs tutaj"** (lewy górny) | Strefa upuszczania plików studentów; kliknięcie otwiera również okno wyboru plików |
| **„Wybierz plik XLSX"** | Wczytanie pliku z kluczem odpowiedzi |
| **„Sprawdź"** (zielony) | Uruchomienie sprawdzania |
| **„Załadowane pliki: N \| XLSX: …"** | Pasek statusu: liczba wczytanych plików `.cs` oraz nazwa wczytanego klucza |
| **„Timeout (s)"** | Limit czasu na pojedynczą metodę (1–60 s, domyślnie **3**) |
| **Tabela wyników** | Kolumny: *Student, Zadanie, Parametry, Oczekiwany, Uzyskany, Pkt, Status* |
| **„Eksportuj do .xlsx"** (czerwony) | Zapis wyników do pliku Excel |

### Krok po kroku

1. **Wczytaj klucz odpowiedzi** - kliknij **„Wybierz plik XLSX"** i wskaż plik `.xlsx` z kluczem (format opisany niżej).
2. **Dodaj rozwiązania studentów** (`.cs`) na jeden z dwóch sposobów:
   - **przeciągnij i upuść** pliki na panel „Przeciągnij pliki .cs tutaj" (panel podświetla się na zielono, gdy upuszczenie jest dozwolone), lub
   - **kliknij ten panel**, aby otworzyć okno wyboru plików (można zaznaczyć wiele plików naraz).
   - Jeden plik `.cs` = jeden student; nazwa pliku (bez rozszerzenia) jest identyfikatorem studenta. Pliki już dodane oraz foldery są pomijane.
3. **Ustaw limit czasu** w polu **„Timeout (s)"** - zakres 1–60 sekund, domyślnie **3 s** (limit na wykonanie pojedynczej metody).
4. **Uruchom sprawdzanie** - kliknij **„Sprawdź"**. Pasek postępu pokazuje stan; interfejs jest na ten czas zablokowany. Po zakończeniu pojawia się łączna liczba punktów.
5. **Przejrzyj wyniki** w tabeli (kolumny: *Student, Zadanie, Parametry, Oczekiwany, Uzyskany, Pkt, Status*). Wiersze są kolorowane wg statusu (patrz niżej).
6. **Wyeksportuj wyniki** - kliknij **„Eksportuj do .xlsx"** i wskaż lokalizację. Domyślna nazwa: `wyniki_RRRR-MM-DD_GG-mm.xlsx` (arkusze „Wyniki" oraz „Podsumowanie"). Po zapisie można od razu otworzyć folder z plikiem.

### Interpretacja wyników

Po sprawdzeniu pasek postępu wypełnia się, a tabela pokazuje wynik każdego przypadku testowego. Wiersze są kolorowane według statusu:

![Wyniki sprawdzania z kolorami statusów](IO_2026_SGGW/s2.png)

Na powyższym przykładzie:

- **zielone wiersze** (`Ok`) - rozwiązania studenta *Kowalski_Jan* zwróciły wartość zgodną z kolumną *Oczekiwany* (po **1 pkt**),
- **czerwone wiersze** (`Bledny`) - rozwiązania studenta *Kowalczyk_Tomek* zwróciły inną wartość niż oczekiwana, np. `KOLOKWIUM` zamiast `muiwkolok` (**0 pkt**).

Kolumny *Oczekiwany* i *Uzyskany* pozwalają od razu zobaczyć, gdzie wynik się rozjechał. Pełną legendę kolorów i wszystkich statusów znajdziesz w sekcji [Statusy wyników](#statusy-wyników).

---

## Format pliku klucza odpowiedzi (XLSX)

- Każdy **arkusz** pliku to jedno zadanie; **nazwa arkusza = nazwa szukanej metody** (dopasowanie po nazwie znormalizowanej - bez spacji/podkreśleń, bez rozróżniania wielkości liter).
- **Wiersz 1** jest nagłówkiem i jest pomijany.
- **Kolumna B** - parametry wejściowe przypadku testowego.
- **Kolumna C** - oczekiwany wynik.
- Tablice zapisuje się w nawiasach kwadratowych, np. `[1, 2, 3]`.
- Separatorem dziesiętnym jest **kropka** (niezależnie od ustawień regionalnych).
- Tablice wielowymiarowe nie są obsługiwane.

---

## Statusy wyników

| Status | Znaczenie | Kolor wiersza |
|--------|-----------|---------------|
| `Ok` | wynik zgodny z kluczem (1 pkt) | zielony |
| `Bledny` | wynik niezgodny z kluczem (0 pkt) | czerwony |
| `Timeout` | przekroczono limit czasu | pomarańczowy |
| `BrakMetody` | nie znaleziono metody dla zadania | jasnoszary |
| `Wyjatek` | metoda rzuciła wyjątek | jasnożółty |
| `BladKompilacji` | kod studenta się nie skompilował | ciemnoszary |
| `ZlyFormatParametrow` | nie udało się sparsować parametrów | jasnoszary |

Punktacja: **1 punkt za każdy poprawny (`Ok`) przypadek testowy.**

---

## Struktura projektu

```
IO-2026-SGGW/
├─ IO_2026_SGGW/              # aplikacja główna (WinForms, net472)
│  ├─ Core/                   # logika: kompilacja, uruchamianie, ocena, eksport
│  ├─ MainForm.cs             # interfejs użytkownika
│  ├─ Program.cs              # punkt wejścia
│  ├─ Doxyfile                # konfiguracja dokumentacji
│  └─ docs/                   # diagramy + wygenerowana dokumentacja Doxygen
├─ IO_2026_SGGW.Tests/        # testy jednostkowe (xUnit, net472)
└─ _repro/                    # pomocnicze narzędzie diagnostyczne (poza solucją)
```

Warstwa logiki znajduje się w przestrzeni nazw `IO_2026_SGGW.Core` (m.in. `AnswerKeyLoader`, `GradingService`, `SolutionCompiler`, `SolutionRunner`, `ResultsExporter`). `MainForm` odpowiada wyłącznie za interfejs i orkiestrację.

---

## Dokumentacja (Doxygen)

Dokumentacja API w formacie HTML generowana jest Doxygenem. Z katalogu `IO_2026_SGGW/`:

```bash
doxygen Doxyfile
```

Wynik: `IO_2026_SGGW/docs/doxygen/html/index.html`.

---

## Rozwiązywanie problemów

- Aplikacja zapisuje log diagnostyczny **`io_debug.log` na pulpicie** (m.in. szczegóły błędów wczytywania klucza i pomijanych plików).
- „Błąd wczytywania klucza" - sprawdź, czy plik to poprawny `.xlsx` i czy układ kolumn jest zgodny z opisanym formatem.
- Brak wyników mimo wczytanych plików - upewnij się, że nazwy arkuszy w kluczu odpowiadają nazwom metod w kodzie studentów.
