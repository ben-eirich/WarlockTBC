/*
   Mechanics Summary:

   Spell Hit: https://wowwiki.fandom.com/wiki/Spell_hit
      Regular Level-70 mobs:    You have a 4% chance to miss.
      Dungeon boss (+2 levels): You have a 6% chance to miss.
      Raid boss (+3 levels):    You have a 17% chance to miss.

      Miss chance can never be reduced below 1%.
      At level 70, 12.615 hit rating is equivalent to 1% hit.
      At level 80, 26.232 hit rating is equivalent to 1% hit. 
      
      Spells (unlike melee) use a 2-roll system. One roll to determine if you hit or not, and then if you hit, a second roll to determine if you crit.

      TODO: Determine how partial resists factor in.


   Spell Criticals: https://wowwiki.fandom.com/wiki/Spell_critical_strike
      At level 70, 22.08 spell crit rating is equivalent to 1% crit.
      At level 80, 45.91 spell crit rating is equivalent to 1% crit.

      For spell crit, intellect factors in, and there is a class specific constant. For warlocks its 1.701.
      At level 70, spell crit chance is: (Intellect / 81.92) + 1.701 + (Crit Rating / 22.08)
      At level 80, spell crit chance is: (Intellect / 166.6667) + 1.701 + (Crit Rating / 45.91)

      DoT's cannot crit and won't benefit from crit. The initial damage part of Immolate can crit.
      Critical strikes deal 150% damage. With Ruin, your destuction criticals will deal 200% damage.
      In Wrath, the Affliction talent Pandemic can allow Corruption and Unstable Affliction only to crit.


   Spell Power: https://wowwiki.fandom.com/wiki/Spell_power_coefficient
      Coeffecients are spelled out in the linked article.
      AFAIK, talents that reduce casting time do not reduce the spell power coefficient.


   Haste: https://wowwiki.fandom.com/wiki/Casting_Speed
      % Spell Haste at level 70 = (Haste Rating / 15.77)
      % Spell Haste at level 80 = (Haste Rating / 32.79) 
      New Casting Time = Base Casting Time / (1 + (% Spell Haste / 100))
      Prior to 2.4, spell haste did not reduce the global cooldown? We assume TBC classic will reduce the global cooldown with Haste.
      Note that "100%" haste means spellcasting time would be cut in half.
      50% haste rating would reduce the global cooldown to 1 seconds.
      100% haste rating would reduce the global cooldown to 0.75 seconds.
      In TBC/Wrath, Haste does not effect DoT ticks, only cast time/GCD.
      There is reportedly a 1 second GCD hardcap.

      
   Some additional information is here: https://www.wowhead.com/forums&topic=54824
   TODO: figure out how to model Imp and Felguard (melee!) damage.

*/

using System;

namespace BurningCrusadeWarlockSim {
    class Program {

        void Run() {
            var s = new Scenario();
            s.RaidTemplate();
            s.Warlock.UseImmolate = true;
            s.Warlock.UseCorruption = true;
            s.Warlock.Curse = Curse.Doom;
            s.Warlock.Talents = Talents.SM_DS_30_21_10();
            s.Warlock.Pet = Pet.Imp;
            s.Warlock.PreRaid();
            s.IsbUptimePercent = 60;
            s.Validate();

            var metaResult = Simulation.MetaSim(s, 4000);
            metaResult.Summarize();

            //var result = Simulation.Simulate(s);
            //result.Summarize();

            //SimStatScaling(s,5000);
        }

        Scenario SetupScenario_Level70GreensDungeon() {
            var scenario = new Scenario();
            scenario.DungeonTemplate();
            scenario.Warlock.QuestGreens();
            scenario.Warlock.UseImmolate = true;
            scenario.Warlock.UseCorruption = true;
            scenario.Warlock.Curse = Curse.Agony;
            scenario.Warlock.Pet = Pet.None;
            scenario.Warlock.Talents = new Talents();
            scenario.Validate();
            return scenario;
        }

        public void SimStatScaling(Scenario baseline, int battles = 5000) {
            Console.Write("Baseline: ");
            var baseResult = Simulation.MetaSim(baseline, battles);
            Console.WriteLine($"{baseResult.meanDps:N1} DPS");

            {
                Console.Write("SpellPwr: ");
                var spellPowerScenario = baseline.Clone();
                spellPowerScenario.Warlock.ShadowPower += 100;
                spellPowerScenario.Warlock.FirePower += 100;
                var spellPowerResult = Simulation.MetaSim(spellPowerScenario, battles);
                double dpsGain = spellPowerResult.meanDps - baseResult.meanDps;
                Console.WriteLine($"+{dpsGain:N1} DPS - {dpsGain / 100:N4} DPS per spellpower");
            }

            {
                Console.Write("Crit:     ");
                var critScenario = baseline.Clone();
                critScenario.Warlock.CritRating += 100;
                var critResult = Simulation.MetaSim(critScenario, battles);
                double dpsGain = critResult.meanDps - baseResult.meanDps;
                Console.WriteLine($"+{dpsGain:N1} DPS - {dpsGain / 100:N4} DPS per crit rating");
            }

            {
                Console.Write("Hit:      ");
                var hitScenario = baseline.Clone();
                int origHitRating = Math.Min(baseline.Warlock.HitRating, 202);
                int newHitRating = Math.Min(origHitRating + 50, 202);
                int hitRatingAdded = newHitRating - origHitRating;
                hitScenario.Warlock.HitRating = newHitRating;
                var hitResult = Simulation.MetaSim(hitScenario, battles);
                double dpsGain = hitResult.meanDps - baseResult.meanDps;
                Console.WriteLine($"+{dpsGain:N1} DPS - {dpsGain / hitRatingAdded:N4} DPS per hit rating");
            }

            {
                Console.Write("Haste:    ");
                var hasteScenario = baseline.Clone();
                hasteScenario.Warlock.HasteRating += 100;
                var hasteResult = Simulation.MetaSim(hasteScenario, battles);
                double dpsGain = hasteResult.meanDps - baseResult.meanDps;
                Console.WriteLine($"+{dpsGain:N1} DPS - {dpsGain / 100:N4} DPS per haste rating");
            }
        }

        static void Main(string[] args) {
            new Program().Run();
        }
    }
}
