using System;
using System.Collections.Generic;
using NetEscapades.EnumGenerators;

namespace Cs_TikTikToe_Challenge_11._2._22_D.H
{
    public class TikTaKToe
    {
        [EnumExtensions]
        public enum SpielFelder
        {
            ObenLinks = 0, ObenMitte = 1, ObenRechts = 2,
            MitteLinks = 3, MitteMitte = 4, MitteRechts = 5,
            UntenLinks = 6, UntenMitte = 7, UntenRechts = 8
        }

        // Konstruktoren
        public TikTaKToe() { }
        public TikTaKToe(bool spielerXBeginnt) => this.SpielerXBeginnt = spielerXBeginnt;
        public TikTaKToe(bool spielerXBeginnt, bool welcomeScreen)
        {
            this.SpielerXBeginnt = spielerXBeginnt;

            if (welcomeScreen)
            {
                WelcomeScreen();
            }
        }
        public TikTaKToe(bool spielerXBeginnt, bool welcomeScreen, bool tutorial)
        {
            this.SpielerXBeginnt = spielerXBeginnt;

            if (welcomeScreen)
            {
                WelcomeScreen();
            }

            if (tutorial)
            {
                ShowTutorial();
            }
        }


        // Public Eigenschaften/Konstanten
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


        // Public Methods
        public void ShowZugAnzahl()
            => Console.Write("SpielZügeAnzahl: \t" + SpielZügeAnzahl);
        
        public void ShowAllEigenschaften()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine();

            Console.WriteLine("GameStop: \t\t" + GameStop);
            Console.WriteLine("MaxSpielZügeAnzahl: \t" + MaxSpielZügeAnzahl);
            Console.WriteLine("SpielerXBeginnt: \t" + SpielerXBeginnt);
            ShowZugAnzahl();

            Console.Write("\nSpielZügeVerlauf: \t");
            foreach (var item in SpielZügeVerlauf)
            {
                Console.Write(item);
            }
            Console.WriteLine();

            Console.Write("SpielFelderVerlauf: \t");
            foreach (var item in SpielFelderVerlauf)
            {
                Console.Write(item);
            }

            Console.WriteLine("\n");
            Console.ResetColor();
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
                Console.Write($"\nPlayer: {playerLetter}, Input Position: ");
                var inputPos = Console.ReadLine();
                if (int.TryParse(inputPos, out var tryParseNumber))
                {
                    var outputNbr = NumberSchutzSpielFeldSet(tryParseNumber);
                    var spielFeldOutC = Convert.ToChar(SpielFeldOutput(tryParseNumber));
                    if (char.IsDigit(spielFeldOutC))
                    {
                        return outputNbr;
                    }
                    
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nFEHLER!");
                    Console.WriteLine("Spielzug wurde bereits von einem anderen Spieler gemacht!\n");
                    Console.ResetColor();
                    continue;
                }
                
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nFEHLER!");
                Console.WriteLine("Spielzug konnte nicht entgegengenommen werden!\n");
                Console.ResetColor();
            }
        }

        public void ShowGame()
        {
            Console.WriteLine();

            var spielFeldNummer = 0;
            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    // Färbe alle Spieler auf dem Spielfeld
                    var spielFeldOutput = SpielFeldOutput(spielFeldNummer);
                    if (!char.IsDigit(Convert.ToChar(spielFeldOutput)))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("\t" + spielFeldOutput);
                        Console.ResetColor();
                        spielFeldNummer++;
                        continue;
                    }
                    Console.Write("\t" + spielFeldOutput);
                    spielFeldNummer++;
                }
                Console.WriteLine("\n");
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


        // Private Methods
        private void ShowTutorial()
        {
            Console.WriteLine("\n\tTutorial:\n");
            Console.WriteLine("Sie sind gleich einer von 2 Spieler. X oder 0.");
            Console.WriteLine("Geben Sie eine Zahl gleich ein,");
            Console.WriteLine("wo sie ihren Zug platzieren möchten.");
            Console.WriteLine("Wenn 3 Kreutze oder Kreise in der Reihe,");
            Console.WriteLine("oder diagonal platziert wurden, hat der jenige gewonnen.");
            Console.WriteLine("\tViel Erfolg!\n");
            Console.WriteLine("Drücken Sie eine Taste um fortzufahren...\n");
            Console.ReadKey();
            Console.Clear();
        }

        private void WelcomeScreen()
        {
            Console.WriteLine("\n---------------------------------------");
            Console.WriteLine("\tWillkommen zu TikTakToe!");
            Console.WriteLine("---------------------------------------\n");
        }

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
            Console.Clear();
            Console.WriteLine("\n-----------------------------");
            Console.WriteLine("\tUnentschieden!");
            Console.WriteLine("-----------------------------\n");
            ShowGame();
            GameStop = true;
        }

        private void SpielerWon(char spielerBuchstabe)
        {
            Console.Clear();
            Console.WriteLine("\n-----------------------------");
            Console.WriteLine($"\tSpieler: {spielerBuchstabe} hat gewonnen!");
            Console.WriteLine("-----------------------------\n");
            ShowGame();
            Console.ReadKey();
            GameStop = true;
        }
    }
}