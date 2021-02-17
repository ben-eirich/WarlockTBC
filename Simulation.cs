using System;

// TODO: Implement Firelock
// TODO: Might be nice to be able to turn off FG Cleave and sim that, cause sometimes you do have to turn it off.

// TODO: BETTER PET DAMAGE
// TODO: would be really cool to factor in some kind of 'forced movement' into fights somehow... should weight dot stuff (and demo) a bit higher than pure nuking, where your dps is zero while moving.
// TODO: Demonic Knowledge does not disable itself if you sac your demon. In practice you would never sac if you
//       went this far down the Demonology tree though...
// TODO: Unholy Power: Its not clear if this impacts Felguard Cleave or not. 
//       Can PROBABLY test this on Whitemane.  Seems like the answer SHOULD be yes (based on googling but not testing).
// TODO: It is NOT CLEAR whether or not Emberstorm affects Imp Firebolt or not.

namespace BurningCrusadeWarlockSim {

    enum PlayerAction {
        Immolate,
        Corruption,
        ShadowBolt,
        CurseOfDoom,
        CurseOfAgony,
        SiphonLife,
        UnstableAffliction,
        LifeTap,
    }

    class Simulation {
        Scenario scenario;
        int currentTick; 
        int finalTick;
        int currentMana;
        
        bool playerAbleToAct;
        bool curseApplied;
        bool immolateApplied;
        bool corruptionApplied;
        bool siphonLifeApplied;
        bool unstableAfflictionApplied;

        Warlock warlock;
        Talents talents;
        Timeline timeline;
        SimResult results;
        
        SimResult simulate(Scenario scenario) {
            //logActions = logEvents = logPet = true;
            this.scenario = scenario;
            warlock = scenario.Warlock;
            talents = warlock.Talents;
            timeline = new Timeline();
            results = new SimResult(scenario.FightDurationSeconds);
            currentTick = 0;
            finalTick = scenario.FightDurationSeconds * 100;
            currentMana = warlock.Mana;
            playerAbleToAct = true;
            curseApplied = false;
            immolateApplied = false;
            corruptionApplied = false;
            siphonLifeApplied = false;
            unstableAfflictionApplied = false;

            if (warlock.Pet == Pet.Imp) {
                Schedule(Event.ImpFirebolt, FireboltCastTime());
            }
            if (warlock.Pet == Pet.Felguard) {
                Schedule(Event.FelguardCleave, 0);
                Schedule(Event.FelguardMeleeAttack, 200);
            }

            while (currentTick < finalTick) {
                ProcessEvents();
                if (playerAbleToAct) {
                    DoAction();
                }
                currentTick++;
            }
            results.Tabulate();
            return results;
        }

        void ProcessEvents() {
            var events = timeline.GetEventsForTick(currentTick);

            foreach (var e in events) {
                if (e.Event == Event.PlayerCanAct) {
                    playerAbleToAct = true;
                } else if (e.Event == Event.CurseExpires) {
                    curseApplied = false;
                } else if (e.Event == Event.ImmolateExpires) {
                    immolateApplied = false;
                } else if (e.Event == Event.CorruptionExpires) {
                    corruptionApplied = false;
                } else if (e.Event == Event.SiphonLifeExpires) {
                    siphonLifeApplied = false;
                } else if (e.Event == Event.UnstableAfflictionExpires) {
                    unstableAfflictionApplied = false;
                }
                
                else if (e.Event == Event.ShadowBoltHit) {
                    int damage = RandBetween(541, 604);
                    double spellPowerCoefficient = IncreaseByPercent(0.8571, talents.ShadowAndFlame * 4);
                    damage += (int) (e.SnapshottedSpellPower * spellPowerCoefficient);
                    if (warlock.T6_4Piece) damage = IncreaseByPercent(damage, 6);

                    string action = "hits";
                    if (e.Crit) {
                        action = "crits!";
                        damage = (int) (damage * CritBonusFactor());
                        results.shadowBoltCrits++;
                    }
                    damage = ApplyModifiersToShadowDamage(damage);
                    EventLog($"Shadowbolt {action} for {damage}");
                    results.shadowBoltDamage += damage;
                }

                else if (e.Event == Event.CorruptionTick) {
                    double spellPowerCoefficient = 1.2 + (talents.EmpoweredCorruption * 0.12);
                    int bonusDamage = (int) (e.SnapshottedSpellPower * spellPowerCoefficient);
                    int totalDamage = ApplyModifiersToShadowDamage(900 + bonusDamage);
                    int tickDamage = totalDamage / 6;
                    EventLog($"Corruption ticked for {tickDamage}");
                    results.corruptionDamage += tickDamage;
                }

                else if (e.Event == Event.ImmolateInitialDamage) {
                    int damage = 327 + (int) (e.SnapshottedSpellPower * 0.2);
                    damage = IncreaseByPercent(damage, talents.ImprovedImmolate * 5);
                    string action = "hits"; 
                    if (e.Crit) {
                        action = "crits!";
                        damage = (int) (damage * CritBonusFactor());
                        results.immolateCrits++;
                    }
                    damage = ApplyModifiersToFireDamage(damage);
                    EventLog($"Immolate {action} for {damage}");
                    results.immolateDamage += damage;
                    immolateApplied = true;
                }

                else if (e.Event == Event.ImmolatePeriodicDamage) {
                    int totalDamage = ApplyModifiersToFireDamage(123 + e.SnapshottedSpellPower);
                    int tickDamage = totalDamage / 5;
                    EventLog($"Immolate ticks for {tickDamage}");
                    results.immolateDamage += tickDamage;
                }

                else if (e.Event == Event.CurseOfDoomTick) {
                    int damage = ApplyModifiersToShadowDamage(4200 + (e.SnapshottedSpellPower * 2));
                    EventLog($"Curse of Doom ticks for {damage}");
                    results.curseOfDoomDamage += damage;
                }

                else if (e.Event == Event.CurseOfAgonyTickLow) {
                    int bonusDamage = (int) (e.SnapshottedSpellPower * 1.2);
                    int totalDamage = ApplyModifiersToShadowDamage(1356 + bonusDamage);
                    totalDamage = IncreaseByPercent(totalDamage, talents.ImprovedAgony * 10);
                    int tickDamage = (int)(totalDamage / 12 / 2);
                    EventLog($"Curse of Agony ticks(L) for {tickDamage}");
                    results.curseOfAgonyDamage += tickDamage;
                }
                
                else if (e.Event == Event.CurseOfAgonyTickMed) {
                    int bonusDamage = (int)(e.SnapshottedSpellPower * 1.2);
                    int totalDamage = ApplyModifiersToShadowDamage(1356 + bonusDamage);
                    totalDamage = IncreaseByPercent(totalDamage, talents.ImprovedAgony * 10);
                    int tickDamage = (int)(totalDamage / 12);
                    EventLog($"Curse of Agony ticks(M) for {tickDamage}");
                    results.curseOfAgonyDamage += tickDamage;
                }
                
                else if (e.Event == Event.CurseOfAgonyTickHi) {
                    int bonusDamage = (int) (e.SnapshottedSpellPower * 1.2);
                    int totalDamage = ApplyModifiersToShadowDamage(1356 + bonusDamage);
                    totalDamage = IncreaseByPercent(totalDamage, talents.ImprovedAgony * 10);
                    int tickDamage = (int) ((totalDamage / 12) * 1.5);
                    EventLog($"Curse of Agony ticks(H) for {tickDamage}");
                    results.curseOfAgonyDamage += tickDamage;
                }
                
                else if (e.Event == Event.SiphonLifeTick) {
                    int bonusDamage = e.SnapshottedSpellPower / 2;
                    int totalDamage = ApplyModifiersToShadowDamage(630 + bonusDamage);
                    int tickDamage = totalDamage / 10;
                    EventLog($"Siphon Life ticks for {tickDamage}");
                    results.siphonLifeDamage += tickDamage;
                }

                else if (e.Event == Event.UnstableAfflictionTick) {
                    int totalDamage = ApplyModifiersToShadowDamage(1050 + e.SnapshottedSpellPower);
                    int tickDamage = totalDamage / 6;
                    EventLog($"Unstable Affliction ticks for {tickDamage}");
                    results.uaDamage += tickDamage;
                }

                else if (e.Event == Event.ImpFirebolt) {
                    // TODO: calculate imp damage, hit%chance, crit%chance...
                    int damage = 236;
                    damage = IncreaseByPercent(damage, talents.ImprovedImp * 10);   // Improved Imp
                    damage = IncreaseByPercent(damage, talents.UnholyPower * 4);    // Unholy Power
                    damage = ApplyModifiersToFireDamage(damage);

                    results.fireboltCasts += 1;
                    results.fireboltDamage += damage;
                    PetLog($"Imp's Firebolt hits for {damage} damage");
                    Schedule(Event.ImpFirebolt, FireboltCastTime());
                }

                else if (e.Event == Event.FelguardMeleeAttack) {
                    int damage = RandBetween(468, 624); // TODO miss chance, crit chance, armor dmg reduction...
                    damage = IncreaseByPercent(damage, talents.UnholyPower * 4);    // Unholy Power
                    damage = IncreaseByPercent(damage, talents.MasterDemonologist); // Master Demonologist
                    if (warlock.HasSoulLink) damage = IncreaseByPercent(damage, 5); // Soul Link
                    results.felguardMeleeCasts += 1;
                    results.felguardMeleeDamage += damage;
                    PetLog($"Felguard melee hits for {damage} damage");
                    Schedule(Event.FelguardMeleeAttack, 200);
                }

                else if (e.Event == Event.FelguardCleave) {
                    int damage = RandBetween(468, 624) + 78; // TODO can this miss? it can definitely crit.
                    damage = IncreaseByPercent(damage, talents.UnholyPower * 4);    // Unholy Power
                    damage = IncreaseByPercent(damage, talents.MasterDemonologist); // Master Demonologist
                    if (warlock.HasSoulLink) damage = IncreaseByPercent(damage, 5); // Soul Link
                    results.felguardCleaveCasts += 1;
                    results.felguardCleaveDamage += damage;
                    PetLog($"Felguard Cleave hits for {damage} damage");
                    Schedule(Event.FelguardCleave, 800);
                }

                else {
                    Console.WriteLine($"!!!!!!!!!!!!!!!!!!!!!!!!! unhandled event {e.Event}");
                }
            }
        }

        void DoAction() {
            var action = DecidePlayerAction();
            ActionLog($"[MP: {currentMana}]  Next action: {action}");
            DispatchPlayerAction(action); //... do thing.
            playerAbleToAct = false;
        }

        // Our action priority list is currently hardcoded, except that spells can be excluded from the rotation, but the relative priority is always the same.
        // 1. Curse of Doom/Curse of Agony
        // 2. Immolate
        // 3. Corruption (which should be higher, corr or imm?)
        // 4. Life tap mixed in when mana is "low" and we're in filler state. Always lifetap during filler, keeping enough buffer that we can cast all "required" spells without running out of mana.
        // 5. Shadow bolt as filler
        // ??? Siphon Life
        // ??? Unstable Affliction Not clear where they slot into priority list (I guess look at DPCT)
        PlayerAction DecidePlayerAction() {
            // Priority #1 - Keep Curses up:
            if (warlock.Curse != Curse.Other && !curseApplied) {
                if (warlock.Curse == Curse.Agony) return PlayerAction.CurseOfAgony;
                if (warlock.Curse == Curse.Doom) return (FightTimeRemaining > 60 * 100) ? PlayerAction.CurseOfDoom : PlayerAction.CurseOfAgony;
                // We dont model the impact of having to be on CoE or CoR duty at this time. For most fights, it would be at most 2 GCDs.
                // If you're on CoE duty, you're going to do it at T=0 anyway in order to give the tank time to get threat, so the first GCD doesn't count...
            }

            // Priority #2: Keep Immolate up
            if (warlock.UseImmolate && !immolateApplied)
                return PlayerAction.Immolate;

            // Priority #3: Keep Corruption up
            if (warlock.UseCorruption && !corruptionApplied)
                return PlayerAction.Corruption;

            // Keep Unstable Affliction up
            if (warlock.UseUnstableAffliction && !unstableAfflictionApplied)
                return PlayerAction.UnstableAffliction;

            // Keep Siphon Life up, I guess, seems like a garbage spell
            if (warlock.UseSiphonLife && !siphonLifeApplied)
                return PlayerAction.SiphonLife;

            // Priority #4: Lifetap when mana is below a certain threshold. For now, 40%. /shrug
            if (currentMana < scenario.Warlock.Mana * 0.4)
                return PlayerAction.LifeTap;

            // Filler: Shadowbolt. We don't model FireDestro yet.
            return PlayerAction.ShadowBolt;
        }

        void DispatchPlayerAction(PlayerAction action) {
            if (action == PlayerAction.CurseOfDoom) {
                SpendMana(380);
                Schedule(Event.PlayerCanAct, gcd);
                if (DidSpellHit()) {
                    Schedule(Event.CurseOfDoomTick, 6000, ShadowPower);
                    Schedule(Event.CurseExpires, 6000);
                    curseApplied = true;
                }
                results.curseOfDoomCasts += 1;
                results.curseOfDoomCastTime += gcd;
            } 
            
            else if (action == PlayerAction.CurseOfAgony) {
                SpendMana(265);
                Schedule(Event.PlayerCanAct, gcd);
                if (DidSpellHit()) {
                    Schedule(Event.CurseOfAgonyTickLow, 200, ShadowPower);
                    Schedule(Event.CurseOfAgonyTickLow, 400, ShadowPower);
                    Schedule(Event.CurseOfAgonyTickLow, 600, ShadowPower);
                    Schedule(Event.CurseOfAgonyTickLow, 800, ShadowPower);
                    Schedule(Event.CurseOfAgonyTickMed, 1000, ShadowPower);
                    Schedule(Event.CurseOfAgonyTickMed, 1200, ShadowPower);
                    Schedule(Event.CurseOfAgonyTickMed, 1400, ShadowPower);
                    Schedule(Event.CurseOfAgonyTickMed, 1600, ShadowPower);
                    Schedule(Event.CurseOfAgonyTickHi, 1800, ShadowPower);
                    Schedule(Event.CurseOfAgonyTickHi, 2000, ShadowPower);
                    Schedule(Event.CurseOfAgonyTickHi, 2200, ShadowPower);
                    Schedule(Event.CurseOfAgonyTickHi, 2400, ShadowPower);
                    Schedule(Event.CurseExpires, 2400);
                    curseApplied = true;
                }
                results.curseOfAgonyCasts += 1;
                results.curseOfAgonyCastTime += gcd;
            }
            
            else if (action == PlayerAction.Immolate) {
                int castTime = ImmolateCastTime();
                SpendMana(445);
                Schedule(Event.PlayerCanAct, castTime);
                if (DidSpellHit()) {
                    Schedule(Event.ImmolateInitialDamage, castTime, FirePower, DidSpellCrit());
                    Schedule(Event.ImmolatePeriodicDamage, castTime + 300, FirePower);
                    Schedule(Event.ImmolatePeriodicDamage, castTime + 600, FirePower);
                    Schedule(Event.ImmolatePeriodicDamage, castTime + 900, FirePower);
                    Schedule(Event.ImmolatePeriodicDamage, castTime + 1200, FirePower);
                    Schedule(Event.ImmolatePeriodicDamage, castTime + 1500, FirePower);
                    Schedule(Event.ImmolateExpires, castTime + 1400);
                    // Note: ImmolateExpires would normally be scheduled at castTime+1500, but by reducing this 1 second we allow
                    // for Immolate pre-casts when theres less than 1.5 seconds left before it expires. This is a 2-4 DPS gain. /shrug
                } else {
                    EventLog("Immolate misses!");
                    results.misses++;
                }
                
                results.immolateCasts += 1;
                results.immolateCastTime += castTime;
            } 
            
            else if (action == PlayerAction.Corruption) {
                int castTime = CorruptionCastTime();
                SpendMana(370);
                Schedule(Event.PlayerCanAct, CorruptionGcd());
                if (DidSpellHit()) { 
                    Schedule(Event.CorruptionTick, castTime + 300, ShadowPower);
                    Schedule(Event.CorruptionTick, castTime + 600, ShadowPower);
                    Schedule(Event.CorruptionTick, castTime + 900, ShadowPower);
                    Schedule(Event.CorruptionTick, castTime + 1200, ShadowPower);
                    Schedule(Event.CorruptionTick, castTime + 1500, ShadowPower);
                    Schedule(Event.CorruptionTick, castTime + 1800, ShadowPower);
                    Schedule(Event.CorruptionExpires, 1801); 
                    // Precasting corruption was about a 2 DPS increase. It's complicated to model so I didnt.
                    corruptionApplied = true;
                } else {
                    EventLog("Corruption misses!");
                    results.misses++;
                }
                results.corruptionCasts += 1;
                results.corruptionCastTime += CorruptionGcd();
            }

            else if (action == PlayerAction.SiphonLife) {
                SpendMana(410);
                Schedule(Event.PlayerCanAct, gcd);
                if (DidSpellHit()) {
                    Schedule(Event.SiphonLifeTick, 300, ShadowPower);
                    Schedule(Event.SiphonLifeTick, 600, ShadowPower);
                    Schedule(Event.SiphonLifeTick, 900, ShadowPower);
                    Schedule(Event.SiphonLifeTick, 1200, ShadowPower);
                    Schedule(Event.SiphonLifeTick, 1500, ShadowPower);
                    Schedule(Event.SiphonLifeTick, 1800, ShadowPower);
                    Schedule(Event.SiphonLifeTick, 2100, ShadowPower);
                    Schedule(Event.SiphonLifeTick, 2400, ShadowPower);
                    Schedule(Event.SiphonLifeTick, 2700, ShadowPower);
                    Schedule(Event.SiphonLifeTick, 3000, ShadowPower);
                    Schedule(Event.SiphonLifeExpires, 3001);
                    siphonLifeApplied = true;
                }
                results.siphonLifeCasts += 1;
                results.siphonLifeCastTime += gcd;
            }

            else if (action == PlayerAction.UnstableAffliction) {
                SpendMana(400);
                int castTime = gcd;
                Schedule(Event.PlayerCanAct, castTime);
                if (DidSpellHit()) {
                    Schedule(Event.UnstableAfflictionTick, castTime + 300, ShadowPower);
                    Schedule(Event.UnstableAfflictionTick, castTime + 600, ShadowPower);
                    Schedule(Event.UnstableAfflictionTick, castTime + 900, ShadowPower);
                    Schedule(Event.UnstableAfflictionTick, castTime + 1200, ShadowPower);
                    Schedule(Event.UnstableAfflictionTick, castTime + 1500, ShadowPower);
                    Schedule(Event.UnstableAfflictionTick, castTime + 1800, ShadowPower);
                    Schedule(Event.UnstableAfflictionExpires, castTime + 1700); // Allows for precast
                    unstableAfflictionApplied = true;
                }
                results.uaCasts += 1;
                results.uaCastTime += castTime;
            }

            else if (action == PlayerAction.ShadowBolt) {
                int castTime = ShadowBoltCastTime();
                SpendMana(420);
                Schedule(Event.PlayerCanAct, castTime);
                if (DidSpellHit()) {
                    Schedule(Event.ShadowBoltHit, castTime, ShadowPower, DidSpellCrit());
                } else {
                    EventLog("Shadowbolt misses!");
                    results.misses++;
                }
                
                results.shadowBoltCasts += 1;
                results.shadowBoltCastTime += castTime;
            }

            else if (action == PlayerAction.LifeTap) {
                // Verified that formula is (base tap + spellpwr/2) * ImpLifetapFactor (0%/10%/20%)
                int manaGain = 580 + (ShadowPower / 2);
                manaGain = (manaGain * (10 + talents.ImprovedLifeTap)) / 10;
                EventLog($"Lifetap: Lose 580 HP and gain {manaGain} MP");
                currentMana += manaGain;
                Schedule(Event.PlayerCanAct, gcd);
                results.lifeTapCasts += 1;
                results.lifeTapCastTime += gcd;
                results.lifeTapManaGained += manaGain;
            }

            else {
                Console.WriteLine($"!! - not programmed action {action}");
            }
        }

        void Schedule(Event e, int ticksFromNow, int spellPower = 0, bool crit = false) {
            var simEvent = new SimEvent { 
                Event = e, 
                Tick = currentTick + ticksFromNow, 
                SnapshottedSpellPower = spellPower,
                Crit = crit
            };
            timeline.Enqueue(simEvent);
        }
        
        void SpendMana(int amt) {
            currentMana -= amt;
            results.ManaSpent += amt;
        }

        int FightTimeRemaining { get => finalTick - currentTick; }
        int gcd { get => AdjustCastTimeForHaste(150); }
        int BossHitChance()      => (scenario.Boss == BossType.DungeonBoss) ? 94 : 83;
        int CorruptionGcd()      => Math.Max(CorruptionCastTime(), gcd);
        int CorruptionCastTime() => AdjustCastTimeForHaste(200 - (talents.ImprovedCorruption * 40));
        int ShadowBoltCastTime() => AdjustCastTimeForHaste(300 - (talents.Bane * 10));
        int ImmolateCastTime()   => AdjustCastTimeForHaste(200 - (talents.Bane * 10));
        int FireboltCastTime()   => 200 - (talents.ImprovedFirebolt * 25);

        int ShadowPower {
            get {
                int power = warlock.ShadowPower;
                power += 100; // Fel Armor
                power += warlock.Talents.DemonicAegis * 10; // Demonic Aegis
                power += warlock.Talents.DemonicKnowledge * 22; // Demonic Knowledge ghetto estimate
                if (scenario.WrathOfAir) power += 101;
                return power;
            }
        }

        int FirePower {
            get {
                int power = warlock.FirePower;
                power += 100; // Fel Armor
                power += warlock.Talents.DemonicAegis * 10; // Demonic Aegis
                power += warlock.Talents.DemonicKnowledge * 22; // Demonic Knowledge ghetto estimate
                if (scenario.WrathOfAir) power += 101; // Wrath of Air totem
                return power;
            }
        }

        double CritBonusFactor() => talents.Ruin == 1 ? 2.0 : 1.5; 

        int AdjustCastTimeForHaste(int ticks) {
            ticks = (int) (ticks / (1.0 + warlock.HastePercent / 100.0));
            return ticks; // TODO also spells cant be reduced below 1second I guess
        }

        bool DidSpellHit() {
            int bossHitChance = BossHitChance() * 100;
            int hitModifier = (int) (warlock.HitPercent * 100);
            int totalHitChance = Math.Min(bossHitChance + hitModifier, 9900);
            int dice = RandBetween(0, 9999);
            return dice < totalHitChance;
        }

        bool DidSpellCrit() {
            int critChance = (int) (warlock.CritChance * 100);
            critChance += talents.Devastation * 100;
            critChance += talents.Backlash * 100;
            critChance += talents.DemonicTactics * 100;
            int dice = RandBetween(0, 9999);
            return dice < critChance;
        }

        bool IsIsbUp() {
            int dice = RandBetween(1, 100);
            return dice < scenario.IsbUptimePercent;
        }

        static Random rand = new Random();
        static int RandBetween(int min, int max) {
            return rand.Next(min, max + 1);
        }

        int ApplyModifiersToShadowDamage(int damage) {
            if (scenario.BaseCurseOfElements)   damage = IncreaseByPercent(damage, 10);
            if (scenario.ImpCurseOfElements)    damage = IncreaseByPercent(damage, 13);
            if (scenario.ShadowWeaving)         damage = IncreaseByPercent(damage, 10);
            if (warlock.HasSoulLink)            damage = IncreaseByPercent(damage, 5);
            if (warlock.Pet == Pet.SacSuccubus) damage = IncreaseByPercent(damage, 15);
            if (warlock.Pet == Pet.Felguard)    damage = IncreaseByPercent(damage, talents.MasterDemonologist);
            if (IsIsbUp())                      damage = IncreaseByPercent(damage, 20);
            damage = IncreaseByPercent(damage, talents.ShadowMastery * 2);
            return damage;
        }

        int ApplyModifiersToFireDamage(int damage) {
            if (scenario.BaseCurseOfElements)   damage = IncreaseByPercent(damage, 10);
            if (scenario.ImpCurseOfElements)    damage = IncreaseByPercent(damage, 13);
            if (scenario.ImprovedScorch)        damage = IncreaseByPercent(damage, 15);
            if (warlock.HasSoulLink)            damage = IncreaseByPercent(damage, 5);
            if (warlock.Pet == Pet.SacImp)      damage = IncreaseByPercent(damage, 15);
            if (warlock.Pet == Pet.Felguard)    damage = IncreaseByPercent(damage, talents.MasterDemonologist);

            // TODO: Not clear if Emberstorm should affect Imp Firebolt
            damage = IncreaseByPercent(damage, talents.Emberstorm * 2);
            return damage;
        }

        static int IncreaseByPercent(int value, int percent) {
            return (value * (100 + percent)) / 100;
        }

        static double IncreaseByPercent(double value, int percent) {
            return (value * (100d + percent)) / 100d;
        }

        bool logActions = false;
        bool logEvents = false;
        bool logPet = false;

        void ActionLog(string note) {
            if (!logActions) return;
            int minutes = currentTick / (100 * 60);
            int seconds = (currentTick / 100) % 60;
            int fraction = currentTick % 100;
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"[{minutes:D2}:{seconds:D2}.{fraction:D2}] ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(note);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        void EventLog(string note) {
            if (!logEvents) return;
            int minutes = currentTick / (100 * 60);
            int seconds = (currentTick / 100) % 60;
            int fraction = currentTick % 100;
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"[{minutes:D2}:{seconds:D2}.{fraction:D2}]         ");

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(note);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        void PetLog(string note) {
            if (!logPet) return;
            int minutes = currentTick / (100 * 60);
            int seconds = (currentTick / 100) % 60;
            int fraction = currentTick % 100;
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write($"[{minutes:D2}:{seconds:D2}.{fraction:D2}]         ");

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(note);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public static SimResult Simulate(Scenario scenario) {
            return new Simulation().simulate(scenario);
        }

        public static MetaResult MetaSim(Scenario scenario, int battles) {
            var sim = new Simulation();
            var meta = new MetaResult();
            for (int i = 0; i < battles; i++) {
                meta.Results.Add(sim.simulate(scenario));
            }

            meta.Complete();
            return meta;
        }

        public static void CompareScenarios(Scenario s1, Scenario s2, int battles) {
            Console.WriteLine($"Simming {s1.Name}");
            var m1 = MetaSim(s1, battles);
            m1.Summarize();

            Console.WriteLine($"\nSimming {s2.Name}");
            var m2 = MetaSim(s2, battles);
            m2.Summarize();

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine();
            Console.WriteLine("======================= COMPARISON SUMMARY ======================= ");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"{s1.Name,15} : {m1.meanDamage,12:N0} Damage ({m1.meanDps:N0} DPS)");
            Console.WriteLine($"{s2.Name,15} : {m2.meanDamage,12:N0} Damage ({m2.meanDps:N0} DPS)");
        }
    }
}
