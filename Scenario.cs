namespace BurningCrusadeWarlockSim {
    enum BossType {
        DungeonBoss,
        RaidBoss
    }

    class Scenario {
        public string Name = "Untitled scenario";
        public Warlock Warlock = new Warlock();
        public BossType Boss = BossType.RaidBoss;
        public int FightDurationSeconds = 10 * 60;

        // Raid/party buffs 
        public bool BaseCurseOfElements;   // 10% base
        public bool ImpCurseOfElements;    // 13% with Malediction
        public bool ShadowWeaving;         // 10% +Shadow
        public bool ImprovedScorch;        // 15% +Fire
        public bool CurseOfReck;           // Would benefit Felguard DPS if up
        public bool WrathOfAir;            // 101 spell damage
        public int IsbUptimePercent;       // Integer-value ISB uptime percentage

        public void Validate() {
            Warlock.Talents.Validate();
            if (ImpCurseOfElements)
                BaseCurseOfElements = false;
        }

        public Scenario DungeonTemplate() {
            Boss = BossType.DungeonBoss;
            FightDurationSeconds = 3 * 60;
            BaseCurseOfElements = false;
            ImpCurseOfElements = false;
            ShadowWeaving = false;
            ImprovedScorch = false;
            CurseOfReck = false;
            WrathOfAir = false;
            IsbUptimePercent = 0;
            return this;
        }

        public Scenario RaidTemplate() {
            Boss = BossType.RaidBoss;
            FightDurationSeconds = 10 * 60;
            BaseCurseOfElements = false;
            ImpCurseOfElements = true;
            ShadowWeaving = true;
            ImprovedScorch = true;
            CurseOfReck = false;
            WrathOfAir = true;
            IsbUptimePercent = 40;
            return this;
        }

        public Scenario Clone() {
            var c = new Scenario();
            c.Name = Name;
            c.Warlock = Warlock.Clone();
            c.Boss = Boss;
            c.FightDurationSeconds = FightDurationSeconds;
            c.BaseCurseOfElements = BaseCurseOfElements;
            c.ImpCurseOfElements = ImpCurseOfElements;
            c.ShadowWeaving = ShadowWeaving;
            c.ImprovedScorch = ImprovedScorch;
            c.CurseOfReck = CurseOfReck;
            c.WrathOfAir = WrathOfAir;
            c.IsbUptimePercent = IsbUptimePercent;
            return c;
        }
    }
}
