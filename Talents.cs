using System;

namespace BurningCrusadeWarlockSim {
    class Talents {
        // Affliction
        public int ImprovedCorruption;  // X FULLY IMPLEMENTED. (Note: We don't pre-cast corruption, sorry)
        public int ImprovedLifeTap;     // X FULLY IMPLEMENTED
        public int ImprovedAgony;       // X FULLY IMPLEMENTED
        public int EmpoweredCorruption; // X FULLY IMPLEMENTED
        public int ShadowMastery;       // X FULLY IMPLEMENTED

        // Demonology
        public int ImprovedImp;         // X SKETCH IMPLEMENTED
        public int DemonicAegis;        // X FULLY IMPLEMENTED but be careful inputting your +dmg... Never have Fel Armor up
        public int UnholyPower;         // X FULLY SKETCH IMPLEMENTED ... Test FG cleave, fix pet damage models
        public int MasterDemonologist;  // x Implemented for Felguard
        public int SoulLink;            // X FULLY IMPLEMENTED
        public int DemonicKnowledge;    // X SKETCH IMPLEMENTED - Estimated at 66 spell damage for 3/3
        public int DemonicTactics;      // - PARTIAL: Player crit chance implemented, but not pet

        // Destruction
        public int ImprovedShadowBolt;  // ? Implemented as a Raid buff. This talent here doesnt actually do anything.
        public int Bane;                // X FULLY IMPLEMENTED
        public int ImprovedFirebolt;    // X FULLY IMPLEMENTED
        public int Devastation;         // X FULLY IMPLEMENTED
        public int ImprovedImmolate;    // X FULLY IMPLEMENTED
        public int Ruin;                // X IMPLEMENTED FOR SHADOWBOLT AND IMMOLATE. NOT INCINERATE, CONFLAGRATE...
        public int Emberstorm;          // X PARTIAL: Fire Damage Bonus implemented NOT SURE ABOUT FIREBOLT
        public int Backlash;            // X FULLY IMPLEMENTED
        public int ShadowAndFlame;      // X FULLY IMPLEMENTED - well, for Shadowbolt anyhow

        public static Talents Afflic_41_3_17() {
            return new Talents {
                ImprovedCorruption = 5,
                ImprovedLifeTap = 2,
                ImprovedAgony = 2,
                EmpoweredCorruption = 3,
                ShadowMastery = 5,
                ImprovedImp = 3,
                ImprovedShadowBolt = 5,
                Bane = 5,
                ImprovedFirebolt = 2,
                Devastation = 5
            };
        }

        public static Talents Afflic_31_3_27() {
            return new Talents {
                ImprovedCorruption = 5,
                ImprovedLifeTap = 2,
                ImprovedAgony = 2,
                EmpoweredCorruption = 3,
                ShadowMastery = 5,
                ImprovedImp = 3,
                ImprovedShadowBolt = 5,
                Bane = 5,
                ImprovedFirebolt = 2,
                Devastation = 5,
                Ruin = 1,
                ImprovedImmolate = 4,
                Emberstorm = 2
            };
        }

        public static Talents SM_DS_30_21_10() {
            return new Talents {
                ImprovedCorruption = 5,
                ImprovedLifeTap = 2,
                ImprovedAgony = 2,
                EmpoweredCorruption = 3,
                ShadowMastery = 5,
                DemonicAegis = 3,
                ImprovedShadowBolt = 5,
                Bane = 5                
            };
        }

        public static Talents Hybrid_6_34_21() {
            return new Talents {
                ImprovedCorruption = 5,
                ImprovedLifeTap = 1,
                ImprovedImp = 3,
                DemonicAegis = 3,
                UnholyPower = 5,
                MasterDemonologist = 5,
                DemonicKnowledge = 3,
                SoulLink = 1,
                ImprovedShadowBolt = 5,
                Bane = 5, 
                ImprovedFirebolt = 2,
                Devastation = 5,
                Ruin = 1
            };
        }

        public static Talents Felguard_5_41_15() {
            return new Talents {
                ImprovedCorruption = 5,
                DemonicAegis = 3,
                UnholyPower = 5,
                MasterDemonologist = 5,
                DemonicKnowledge = 3,
                SoulLink = 1,
                DemonicTactics = 5,
                ImprovedShadowBolt = 5,
                Bane = 5,
                Devastation = 5
            };
        }

        public static Talents CritSpec_1_39_21() {
            return new Talents {
                ImprovedCorruption = 1,
                ImprovedImp = 3,
                DemonicAegis = 3,
                UnholyPower = 5,
                MasterDemonologist = 5,
                DemonicKnowledge = 3,
                SoulLink = 1,
                DemonicTactics = 4,
                ImprovedShadowBolt = 5,
                Bane = 5,
                Devastation = 5,
                ImprovedFirebolt = 2,
                Ruin = 1
            };
        }

        public static Talents ShadowNuke_0_21_40() {
            return new Talents {
                DemonicAegis = 3,
                ImprovedShadowBolt = 5,
                Bane = 5,
                Devastation = 5,
                Ruin = 1,
                ImprovedImmolate = 5,
                Emberstorm = 5,
                Backlash = 3,
                ShadowAndFlame = 5
            };
        }

        public void Validate() {
            if (ImprovedCorruption > 5)  throw new Exception("Invalid value for Improved Corruption");
            if (ImprovedLifeTap > 2)     throw new Exception("Invalid value for Improved Life Tap");
            if (ImprovedAgony > 2)       throw new Exception("Invalid value for Improved Agony");
            if (EmpoweredCorruption > 3) throw new Exception("Invalid value for Empowered Corruption");
            if (ShadowMastery > 5)       throw new Exception("Invalid value for Shadow Mastery");

            if (ImprovedImp > 3)         throw new Exception("Invalid value for Improved Imp");
            if (DemonicAegis > 3)        throw new Exception("Invalid value for Demonic Aegis");
            if (UnholyPower > 5)         throw new Exception("Invalid value for Unholy Power");
            if (MasterDemonologist > 5)  throw new Exception("Invalid value for Master Demonologist");
            if (SoulLink > 1)            throw new Exception("Invalid value for Soul Link");
            if (DemonicKnowledge > 3)    throw new Exception("Invalid value for Demonic Knowledge");
            if (DemonicTactics > 5)      throw new Exception("Invalid value for Demonic Tactics");

            if (ImprovedShadowBolt > 5)  throw new Exception("Invalid value for Improved Shadow Bolt");
            if (Bane > 5)                throw new Exception("Invalid value for Bane");
            if (ImprovedFirebolt > 2)    throw new Exception("Invalid value for Improved Firebolt");
            if (Devastation > 5)         throw new Exception("Invalid value for Devastation");
            if (ImprovedImmolate > 5)    throw new Exception("Invalid value for Improved Immolate");
            if (Ruin > 1)                throw new Exception("Invalid value for Ruin");
            if (Emberstorm > 5)          throw new Exception("Invalid value for Emberstorm");
            if (Backlash > 3)            throw new Exception("Invalid value for Backlash");
            if (ShadowAndFlame > 5)      throw new Exception("Invalid value for ShadowAndFlame");
        }

        public Talents Clone() {
            var c = new Talents();
            // Affliction
            c.ImprovedCorruption = ImprovedCorruption;
            c.ImprovedLifeTap = ImprovedLifeTap;
            c.ImprovedAgony = ImprovedAgony;
            c.EmpoweredCorruption = EmpoweredCorruption;
            c.ShadowMastery = ShadowMastery;
            // Demonology
            c.ImprovedImp = ImprovedImp;
            c.DemonicAegis = DemonicAegis;
            c.UnholyPower = UnholyPower;
            c.MasterDemonologist = MasterDemonologist;
            c.SoulLink = SoulLink;
            c.DemonicKnowledge = DemonicKnowledge;
            c.DemonicTactics = DemonicTactics;
            // Destruction
            c.ImprovedShadowBolt = ImprovedShadowBolt;
            c.Bane = Bane;
            c.ImprovedFirebolt = ImprovedFirebolt;
            c.Devastation = Devastation;
            c.ImprovedImmolate = ImprovedImmolate;
            c.Ruin = Ruin;
            c.Emberstorm = Emberstorm;
            c.Backlash = Backlash;
            c.ShadowAndFlame = ShadowAndFlame;
            return c;
        }
    }
}
