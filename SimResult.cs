using System;

namespace BurningCrusadeWarlockSim {
    class SimResult {
        public int TotalDamage;
        public int ManaSpent;

        public int immolateCasts;
        public int immolateCastTime;
        public int immolateDamage;
        public int immolateCrits;

        public int corruptionCasts;
        public int corruptionCastTime;
        public int corruptionDamage;

        public int curseOfDoomCasts;
        public int curseOfDoomCastTime;
        public int curseOfDoomDamage;

        public int curseOfAgonyCasts;
        public int curseOfAgonyCastTime;
        public int curseOfAgonyDamage;

        public int siphonLifeCasts;
        public int siphonLifeCastTime;
        public int siphonLifeDamage;

        public int uaCasts;
        public int uaCastTime;
        public int uaDamage;

        public int shadowBoltCasts;
        public int shadowBoltCastTime;
        public int shadowBoltDamage;
        public int shadowBoltCrits;

        public int lifeTapCasts;
        public int lifeTapCastTime;
        public int lifeTapManaGained;

        public int fireboltCasts;
        public int fireboltDamage;

        public int felguardMeleeDamage;
        public int felguardMeleeCasts;

        public int felguardCleaveDamage;
        public int felguardCleaveCasts;

        public int misses;

        public int fightSeconds;

        public SimResult(int fightSeconds) {
            this.fightSeconds = fightSeconds;
        }

        public void Tabulate() {
            TotalDamage = 0;
            TotalDamage += immolateDamage;
            TotalDamage += corruptionDamage;
            TotalDamage += curseOfDoomDamage;
            TotalDamage += curseOfAgonyDamage;
            TotalDamage += siphonLifeDamage;
            TotalDamage += shadowBoltDamage;
            TotalDamage += fireboltDamage;
            TotalDamage += felguardMeleeDamage;
            TotalDamage += felguardCleaveDamage;
        }

        public void Summarize() {
            PrintKeyStats();
            Console.WriteLine();
            PrintSpellStats();
        }

        public void PrintKeyStats() {
                                         Console.WriteLine($"Total      : {TotalDamage,12:N0} ({(TotalDamage / fightSeconds),4} DPS) Casts: {totalCasts(),4}   Miss: {misses,3} ({pct(misses, missableCasts),5:P1})");
            if (shadowBoltCasts     > 0) Console.WriteLine($"Shadow Bolt: {shadowBoltDamage,12:N0} ({pctDmg(shadowBoltDamage),5:P1})    Casts: {shadowBoltCasts,4}  Crits: {shadowBoltCrits,3} ({pct(shadowBoltCrits, shadowBoltCasts),5:P1})");
            if (immolateCasts       > 0) Console.WriteLine($"Immolate   : {immolateDamage,12:N0} ({pctDmg(immolateDamage),5:P1})    Casts: {immolateCasts,4}  Crits: {immolateCrits,3} ({pct(immolateCrits, immolateCasts),5:P1})");
            if (corruptionCasts     > 0) Console.WriteLine($"Corruption : {corruptionDamage,12:N0} ({pctDmg(corruptionDamage),5:P1})    Casts: {corruptionCasts,4}");
            if (curseOfDoomCasts    > 0) Console.WriteLine($"Curse Doom : {curseOfDoomDamage,12:N0} ({pctDmg(curseOfDoomDamage),5:P1})    Casts: {curseOfDoomCasts,4}");
            if (curseOfAgonyCasts   > 0) Console.WriteLine($"Curse Agony: {curseOfAgonyDamage,12:N0} ({pctDmg(curseOfAgonyDamage),5:P1})    Casts: {curseOfAgonyCasts,4}");
            if (siphonLifeCasts     > 0) Console.WriteLine($"Siphon Life: {siphonLifeDamage,12:N0} ({pctDmg(siphonLifeDamage),5:P1})    Casts: {siphonLifeCasts,4}");
            if (uaCasts             > 0) Console.WriteLine($"Unstable Af: {uaDamage,12:N0} ({pctDmg(uaDamage),5:P1})    Casts: {uaCasts,4}");
            if (fireboltCasts       > 0) Console.WriteLine($"Firebolt   : {fireboltDamage,12:N0} ({pctDmg(fireboltDamage),5:P1})    Casts: {fireboltCasts,4}");
            if (felguardMeleeCasts  > 0) Console.WriteLine($"FG Melee   : {felguardMeleeDamage,12:N0} ({pctDmg(felguardMeleeDamage),5:P1})    Casts: {felguardMeleeCasts,4}");
            if (felguardCleaveCasts > 0) Console.WriteLine($"FG Cleave  : {felguardCleaveDamage,12:N0} ({pctDmg(felguardCleaveDamage),5:P1})    Casts: {felguardCleaveCasts,4}");
            if (lifeTapCasts        > 0) Console.WriteLine($"Lifetap    : {(lifeTapManaGained / lifeTapCasts),12:N0} (MPT)      Casts: {lifeTapCasts,4}  ({lifeTapCastTime / 100d:N1}s) ");
        }

        public void PrintSpellStats() {
            if (shadowBoltCasts   > 0) Console.WriteLine($"Damage Per Cast Time   (Shadow Bolt): {(double)shadowBoltDamage   / (shadowBoltCastTime   / 100d),8:N1}");
            if (corruptionCasts   > 0) Console.WriteLine($"Damage Per Cast Time   (Corruption ): {(double)corruptionDamage   / (corruptionCastTime   / 100d),8:N1}");
            if (immolateCasts     > 0) Console.WriteLine($"Damage Per Cast Time   (Immolate   ): {(double)immolateDamage     / (immolateCastTime     / 100d),8:N1}");
            if (curseOfDoomCasts  > 0) Console.WriteLine($"Damage Per Cast Time   (Curse Doom ): {(double)curseOfDoomDamage  / (curseOfDoomCastTime  / 100d),8:N1}");
            if (curseOfAgonyCasts > 0) Console.WriteLine($"Damage Per Cast Time   (Curse Agony): {(double)curseOfAgonyDamage / (curseOfAgonyCastTime / 100d),8:N1}");
            if (siphonLifeCasts   > 0) Console.WriteLine($"Damage Per Cast Time   (Siphon Life): {(double)siphonLifeDamage   / (siphonLifeCastTime   / 100d),8:N1}");
            if (uaCasts           > 0) Console.WriteLine($"Damage Per Cast Time   (Unstable Af): {(double)uaDamage           / (uaCastTime           / 100d),8:N1}");
                                       Console.WriteLine();
            if (shadowBoltCasts   > 0) Console.WriteLine($"Damage Per Cast        (Shadow Bolt): {(double)shadowBoltDamage   / shadowBoltCasts,8:N1}");
            if (corruptionCasts   > 0) Console.WriteLine($"Damage Per Cast        (Corruption ): {(double)corruptionDamage   / corruptionCasts,8:N1}");
            if (immolateCasts     > 0) Console.WriteLine($"Damage Per Cast        (Immolate   ): {(double)immolateDamage     / immolateCasts,8:N1}");
            if (curseOfDoomCasts  > 0) Console.WriteLine($"Damage Per Cast        (Curse Doom ): {(double)curseOfDoomDamage  / curseOfDoomCasts,8:N1}");
            if (curseOfAgonyCasts > 0) Console.WriteLine($"Damage Per Cast        (Curse Agony): {(double)curseOfAgonyDamage / curseOfAgonyCasts,8:N1}");
            if (siphonLifeCasts   > 0) Console.WriteLine($"Damage Per Cast        (Siphon Life): {(double)siphonLifeDamage   / siphonLifeCasts,8:N1}");
            if (uaCasts           > 0) Console.WriteLine($"Damage Per Cast        (Unstable Af): {(double)uaDamage           / uaCasts,8:N1}");
                                       Console.WriteLine();
            if (shadowBoltCasts   > 0) Console.WriteLine($"Damage Per Mana        (Shadow Bolt): {(double)shadowBoltDamage   / (shadowBoltCasts   * 420),5:N2}");
            if (corruptionCasts   > 0) Console.WriteLine($"Damage Per Mana        (Corruption ): {(double)corruptionDamage   / (corruptionCasts   * 370),5:N2}");
            if (immolateCasts     > 0) Console.WriteLine($"Damage Per Mana        (Immolate   ): {(double)immolateDamage     / (immolateCasts     * 445),5:N2}");
            if (curseOfDoomCasts  > 0) Console.WriteLine($"Damage Per Mana        (Curse Doom ): {(double)curseOfDoomDamage  / (curseOfDoomCasts  * 380),5:N2}");
            if (curseOfAgonyCasts > 0) Console.WriteLine($"Damage Per Mana        (Curse Agony): {(double)curseOfAgonyDamage / (curseOfAgonyCasts * 265),5:N2}");
            if (siphonLifeCasts   > 0) Console.WriteLine($"Damage Per Mana        (Siphon Life): {(double)siphonLifeDamage   / (siphonLifeCasts   * 410),5:N2}");
            if (uaCasts           > 0) Console.WriteLine($"Damage Per Mana        (Unstable Af): {(double)uaDamage           / (uaCasts           * 400),5:N2}");
        }

        double pctDmg(int damage) => (double)damage / (double)TotalDamage;
        double pct(int v1, int v2) => (double) v1 / (double) v2;
        int missableCasts => totalCasts() - lifeTapCasts;

        int totalCasts() {
            return immolateCasts +
                corruptionCasts +
                curseOfAgonyCasts +
                curseOfDoomCasts +
                siphonLifeCasts +
                shadowBoltCasts +
                lifeTapCasts;
        }

        public override string ToString() {
            return TotalDamage.ToString();
        }
    }
}
