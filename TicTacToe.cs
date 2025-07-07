using System.Numerics;
using static System.Console;

namespace TicTacToe;

public sealed class TicTacToe
{
    public enum SpielFelder
    {
        ObenLinks = 0, ObenMitte = 1, ObenRechts = 2,
        MitteLinks = 3, MitteMitte = 4, MitteRechts = 5,
        UntenLinks = 6, UntenMitte = 7, UntenRechts = 8
    }

    public TicTacToe() { }
    public TicTacToe(bool spielerXBeginnt) => this.SpielerXBeginnt = spielerXBeginnt;
    public TicTacToe(bool spielerXBeginnt, bool welcomeScreen)
    {
        this.SpielerXBeginnt = spielerXBeginnt;

        if (welcomeScreen)
        {
            PrintWelcomeScreen();
        }
    }
    public TicTacToe(bool spielerXBeginnt, bool welcomeScreen, bool tutorial)
    {
        this.SpielerXBeginnt = spielerXBeginnt;

        if (welcomeScreen)
        {
            PrintWelcomeScreen();
        }

        if (tutorial)
        {
            ShowTutorial();
        }
    }


    public bool GameStop { get; private set; } = false;
    public bool SpielerXBeginnt { get; private set; } = true;
    public byte SpielZügeAnzahl { get; private set; } = 0;
    public List<char> SpielZügeVerlauf { get; private set; } = new List<char>(MaxSpielZügeAnzahl);
    public List<SpielFelder> SpielFelderVerlauf { get; private set; } = new List<SpielFelder>(MaxSpielZügeAnzahl);
    public SpielFelder[,] GewinnKombinationen { get; } =
    {
        // Horizontal
        { SpielFelder.ObenLinks, SpielFelder.ObenMitte, SpielFelder.ObenRechts },
        { SpielFelder.MitteLinks, SpielFelder.MitteMitte, SpielFelder.MitteRechts },
        { SpielFelder.UntenLinks, SpielFelder.UntenMitte, SpielFelder.UntenRechts },
        // Vertical
        { SpielFelder.ObenLinks, SpielFelder.MitteLinks, SpielFelder.UntenLinks },
        { SpielFelder.ObenMitte, SpielFelder.MitteMitte, SpielFelder.UntenMitte },
        { SpielFelder.ObenRechts, SpielFelder.MitteRechts, SpielFelder.UntenRechts },
        // Diagonal
        { SpielFelder.ObenLinks, SpielFelder.MitteMitte, SpielFelder.UntenRechts },
        { SpielFelder.UntenLinks, SpielFelder.MitteMitte, SpielFelder.ObenRechts }
    };

    public const byte MaxSpielZügeAnzahl = 9;


    public void ShowZugAnzahl()
        => Write("SpielZügeAnzahl: \t" + SpielZügeAnzahl);

    public void ShowAllEigenschaften()
    {
        ForegroundColor = ConsoleColor.Cyan;
        WriteLine();

        WriteLine("GameStop: \t\t" + GameStop);
        WriteLine("MaxSpielZügeAnzahl: \t" + MaxSpielZügeAnzahl);
        WriteLine("SpielerXBeginnt: \t" + SpielerXBeginnt);
        ShowZugAnzahl();

        Write("\nSpielZügeVerlauf: \t");
        foreach (var item in SpielZügeVerlauf)
        {
            Write(item);
        }
        WriteLine();

        Write("SpielFelderVerlauf: \t");
        foreach (var item in SpielFelderVerlauf)
        {
            Write(item);
        }

        WriteLine("\n");
        ResetColor();
    }

    public void StartGame() => StartGameInternal(this.SpielerXBeginnt);
    public void StartGame(bool spielerXBeginnt) => StartGameInternal(spielerXBeginnt);

    public void ResetGame() => ResetGameInternal(this.SpielerXBeginnt);
    public void ResetGame(bool spielerXBeginnt) => ResetGameInternal(spielerXBeginnt);

    public void SpielerZug(char spielZugBuchstabe, byte zugPosition)
        => SpielerZugInternal(spielZugBuchstabe, (SpielFelder)NumberSchutzSpielFeldSet(zugPosition));
    public void SpielerZug(char spielZugBuchstabe, SpielFelder zugPosition)
        => SpielerZugInternal(spielZugBuchstabe, zugPosition);

    public byte GetSpielerPos(char playerLetter)
    {
        while (true)
        {
            Write($"\nPlayer: {playerLetter}, Input Position: ");
            var inputPos = ReadLine();
            if (int.TryParse(inputPos, out var tryParseNumber))
            {
                var outputNbr = NumberSchutzSpielFeldSet(tryParseNumber);
                var spielFeldOutC = Convert.ToChar(SpielFeldOutput(tryParseNumber));
                if (char.IsDigit(spielFeldOutC))
                {
                    return outputNbr;
                }

                ForegroundColor = ConsoleColor.Red;
                WriteLine("\nFEHLER!");
                WriteLine("Spielzug wurde bereits von einem anderen Spieler gemacht!\n");
                ResetColor();
                continue;
            }

            ForegroundColor = ConsoleColor.Red;
            WriteLine("\nFEHLER!");
            WriteLine("Spielzug konnte nicht entgegengenommen werden!\n");
            ResetColor();
        }
    }

    public void ShowGame()
    {
        WriteLine();

        var spielFeldNummer = 0;
        for (var i = 0; i < 3; i++)
        {
            for (var j = 0; j < 3; j++)
            {
                // Färbe alle Spieler auf dem Spielfeld
                var spielFeldOutput = SpielFeldOutput(spielFeldNummer);
                if (!char.IsDigit(Convert.ToChar(spielFeldOutput)))
                {
                    ForegroundColor = ConsoleColor.Yellow;
                    Write("\t" + spielFeldOutput);
                    ResetColor();
                    spielFeldNummer++;
                    continue;
                }
                Write("\t" + spielFeldOutput);
                spielFeldNummer++;
            }
            WriteLine("\n");
        }
    }

    public string SpielFeldOutput(int spielFeldNbr)
    {
        for (var i = 0; i < SpielFelderVerlauf.Count; i++)
        {
            if (spielFeldNbr == (int)SpielFelderVerlauf[i])
            {
                return SpielZügeVerlauf[i].ToString();
            }
        }

        return spielFeldNbr.ToString();
    }


    private void ShowTutorial()
    {
        WriteLine("\n\tTutorial:\n");
        WriteLine("Sie sind gleich einer von 2 Spieler. X oder 0.");
        WriteLine("Geben Sie eine Zahl gleich ein,");
        WriteLine("wo sie ihren Zug platzieren möchten.");
        WriteLine("Wenn 3 Kreuze oder Kreise in der Reihe,");
        WriteLine("oder diagonal platziert wurden, hat derjenige gewonnen.");
        WriteLine("\tViel Erfolg!\n");
        WriteLine("Drücken Sie eine Taste um fortzufahren...\n");
        ReadKey();
        Clear();
    }

    private static void PrintHeader(string headerText)
    {
        const string HeaderSeparator = "----------------------------------------";

        WriteLine("\n" + HeaderSeparator);
        WriteLine($"\t{headerText}");
        WriteLine(HeaderSeparator + "\n");
    }

    private static void PrintWelcomeScreen() => PrintHeader("Willkommen zu TicTacToe!");

    private byte NumberSchutzSpielFeldSet(int outputNumber)
        => outputNumber >= MaxSpielZügeAnzahl ? (byte)(MaxSpielZügeAnzahl - 1) : (byte)outputNumber;

    private void SpielerZugInternal(char spielZugBuchstabe, SpielFelder zugPosition)
    {
        SpielZügeVerlauf.Add(spielZugBuchstabe);
        SpielFelderVerlauf.Add(zugPosition);
        SpielZügeAnzahl++;
        CheckGewinn(spielZugBuchstabe);
    }

    private void StartGameInternal(bool spielerXBeginnt)
    {
        ResetGameInternal(spielerXBeginnt);

        ShowGame();

        if (spielerXBeginnt)
        {
            ShowZugAnzahl();
            SpielerZug('X', GetSpielerPos('X'));
            ShowGame();
        }

        do
        {
            ShowZugAnzahl();
            SpielerZug('O', GetSpielerPos('O'));
            if (GameStop)
            {
                ResetGame();
                return;
            }
            ShowGame();

            ShowZugAnzahl();
            SpielerZug('X', GetSpielerPos('X'));
            if (GameStop)
            {
                ResetGame();
                return;
            }
            ShowGame();

        } while (SpielZügeAnzahl != MaxSpielZügeAnzahl);
    }

    private void ResetGameInternal(bool spielerXBeginnt)
    {
        GameStop = false;
        SpielerXBeginnt = spielerXBeginnt;
        SpielZügeAnzahl = 0;
        SpielZügeVerlauf.Clear();
        SpielFelderVerlauf.Clear();
    }

    private void CheckGewinn(char plyrLtr)
    {
        for (var i = 0; i < GewinnKombinationen.GetLength(0); i++)
        {
            if (CheckGewinnRow(plyrLtr, GewinnKombinationen[i, 0], GewinnKombinationen[i, 1], GewinnKombinationen[i, 2]))
            {
                SpielerWon(plyrLtr);
                return;
            }
            else if (SpielZügeAnzahl == MaxSpielZügeAnzahl)
            {
                EndGameUnentschieden();
                return;
            }
        }
    }

    private bool CheckGewinnRow(char playerLetr, SpielFelder f1, SpielFelder f2, SpielFelder f3)
        => CheckPlyrOnFieldNbr(f1, playerLetr) && CheckPlyrOnFieldNbr(f2, playerLetr) && CheckPlyrOnFieldNbr(f3, playerLetr);

    private bool CheckPlyrOnFieldNbr(SpielFelder field, char playerLetr)
        => Convert.ToChar(SpielFeldOutput((int)field)) == playerLetr;

    private void EndGameUnentschieden()
    {
        Clear();
        PrintHeader("Unentschieden!");
        ShowGame();
        GameStop = true;
    }

    private void SpielerWon(char spielerBuchstabe)
    {
        Clear();
        PrintHeader($"Spieler: {spielerBuchstabe} hat gewonnen!");
        ShowGame();
        ReadKey();
        GameStop = true;
    }
}
