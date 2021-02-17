using System;
using System.Collections.Generic;
using System.Linq;

namespace BurningCrusadeWarlockSim {
    class MetaResult {
        public List<SimResult> Results = new List<SimResult>();

        public SimResult Worst;
        public SimResult Median;
        public SimResult Best;

        public long totalDamage;
        public int meanDamage;
        public double meanDps;

        public void Complete() {
            Results = Results.OrderBy(r => r.TotalDamage).ToList();
            Worst = Results[0];
            Best = Results[Results.Count - 1];
            Median = Results[Results.Count / 2];

            foreach (var battle in Results) {
                totalDamage += battle.TotalDamage;
                meanDamage = (int)(totalDamage / Results.Count);
                meanDps = (double) meanDamage / Median.fightSeconds;
            }
        }

        public void Summarize() {
            double lowestPct = (double) Worst.TotalDamage / Median.TotalDamage;
            double highestPct = (double) Best.TotalDamage / Median.TotalDamage;
            double spread = highestPct - lowestPct;

            Console.Write($"Mean for {Results.Count} Fights: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"{meanDps:N1}");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(" DPS (");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write($"{meanDamage:N0}");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($" dmg)\n");
            
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"  Worst  : {Worst.TotalDamage,12:N0}   ({Worst.TotalDamage / Worst.fightSeconds} DPS)");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  Median : {Median.TotalDamage,12:N0}   ({Median.TotalDamage / Median.fightSeconds} DPS)");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"  Best   : {Best.TotalDamage,12:N0}   ({Best.TotalDamage / Best.fightSeconds} DPS)");
            Console.ForegroundColor = ConsoleColor.Gray;

            Console.WriteLine($"\nVARIANCE:  {lowestPct:P1} to {highestPct:P1} of median ({spread:P1} spread)");

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("\n============================== WORST  FIGHT ==============================\n");
            Console.ForegroundColor = ConsoleColor.Gray;
            Worst.PrintKeyStats();

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("\n============================== MEDIAN FIGHT ==============================\n");
            Console.ForegroundColor = ConsoleColor.Gray;
            Median.PrintKeyStats();

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("\n============================== BEST   FIGHT ==============================\n");
            Console.ForegroundColor = ConsoleColor.Gray;
            Best.PrintKeyStats();
        }
    }
}
