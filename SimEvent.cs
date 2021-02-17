using System.Collections.Generic;

namespace BurningCrusadeWarlockSim {
    public enum Event {
        CurseOfDoomTick,
        CurseOfAgonyTickLow,
        CurseOfAgonyTickMed,
        CurseOfAgonyTickHi,
        CurseExpires,
        ImmolateInitialDamage,
        ImmolatePeriodicDamage,
        ImmolateExpires,
        CorruptionTick,
        CorruptionExpires,
        SiphonLifeTick,
        SiphonLifeExpires,
        UnstableAfflictionTick,
        UnstableAfflictionExpires,
        ShadowBoltHit,
        PlayerCanAct,

        ImpFirebolt,
        FelguardMeleeAttack,
        FelguardCleave
    }

    class SimEvent {
        public int Tick;
        public Event Event;
        public int SnapshottedSpellPower;
        public bool Crit;

        public override string ToString() {
            if (SnapshottedSpellPower > 0)
                return $"[{Tick}] {Event} (+{SnapshottedSpellPower})";
            else
                return $"[{Tick}] {Event}";
        }
    }

    class Timeline {
        public List<SimEvent> events = new List<SimEvent>();

        public void Enqueue(SimEvent e) {
            events.Add(e);
        }

        public List<SimEvent> GetEventsForTick(int tick) {
            var results = events.FindAll(e => e.Tick == tick);
            events.RemoveAll(e => e.Tick == tick);
            return results;
        }
    }
}
