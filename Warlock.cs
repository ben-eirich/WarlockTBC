namespace BurningCrusadeWarlockSim {
    enum Pet {
        None,
        Imp,
        Felguard,
        SacImp,
        SacSuccubus
    }

    enum Curse {
        Agony,
        Doom, // includes smart downgrade to Agony at low HP
        Other
    }

    class Warlock {
        public Talents Talents = new Talents();
        public int Mana;
        public int Intellect; 

        public int ShadowPower;
        public int FirePower;
        public int HitRating;
        public int CritRating;
        public int HasteRating;

        // Rotation / AI
        public Pet Pet;
        public Curse Curse;
        public bool UseCorruption;
        public bool UseImmolate;
        public bool UseSiphonLife;
        public bool UseUnstableAffliction;
        public bool T6_4Piece;

        public double CritChance   { get => (Intellect / 81.92) + 1.701 + (CritRating / 22.08); }
        public double HastePercent { get => HasteRating / 15.77; }
        public double HitPercent   { get => HitRating / 12.615; }

        public bool HasSoulLink {
            get => (Pet == Pet.Imp || Pet == Pet.Felguard) && Talents.SoulLink == 1;
        }

        public Warlock Naked() {
            ShadowPower = 0;
            FirePower = 0;
            CritRating = 0;
            HitRating = 0;
            HasteRating = 0;
            Intellect = 133;
            Mana = 4459;
            T6_4Piece = false;
            return this;
        }

        public Warlock QuestGreens() {
            // Head:      +47 spellpower (Ata'mal Crown)
            // Neck:      +26 spellpower +11 crit (Pendant of the Battle-Mage)
            // Shoulder:  +34 spellpower (Spiritbinder's Mantle)
            // Cloak:     +28 spellpower (Torn-heart Cloak)
            // Chest:     +29 spellpower +23 crit (Warpthread Vest)
            // Gloves:    +24 spellpower (Warpweaver's gloves)
            // Wrist:     +24 spellpower (Aldor Ceremonial Wraps)
            // Waist:     +34 spellpower (Chief Engineer's Belt - blue!)
            // Legs:      +31 spellpower +21 crit (Netherfarer's Leggings)
            // Boots:     +34 spellpower (Sinister Area 52 Boots, Boots of the Beneficent)
            // Ring 1:    +30 spellpower (Signet of the Violet Tower)
            // Ring 2:    +30 spellpower +10 crit (Evoker's Mark of the Redemption - blue!)
            // Trinket 1: +28 spellpower (Generic ?? Oshu'gun Relic)
            // Trinket 2: +28 spellpower (Generic ?? Oshu'gun Relic)
            // Weapon(s): +82 spellpower +27 crit (Conjurer's Staff)
            // Wand:      +24 spellpower (Spellheart Baton)
            // Total from equipment: 533 Spellpower, 80 crit rating, 0 hit
                                    
            ShadowPower = 533;
            FirePower = 533;
            CritRating = 80;
            HitRating = 0;
            HasteRating = 0;
            Intellect = 295;
            Mana = 6962;
            T6_4Piece = false;
            return this;
        }

        public Warlock PreRaid() {
            // See google docs spreadsheet for detail breakdown
            ShadowPower = 1221;
            FirePower = 1053;
            CritRating = 138;
            HitRating = 152;
            HasteRating = 30;
            Intellect = 414;
            Mana = 8796;
            T6_4Piece = false;
            return this;
        }

        public Warlock Tier4() {
            // See google docs spreadsheet for detail breakdown
            ShadowPower = 1318;
            FirePower = 1195;
            CritRating = 124;
            HitRating = 190;
            HasteRating = 30;
            Intellect = 454;
            Mana = 9421;
            T6_4Piece = false;
            return this;
        }

        public Warlock Tier5() {
            // See google docs spreadsheet for detail breakdown
            ShadowPower = 1347;
            FirePower = 1124;
            CritRating = 257;
            HitRating = 190;
            HasteRating = 30;
            Intellect = 454;
            Mana = 9421;
            T6_4Piece = false;
            return this;
        }

        public Warlock UltimateGear() {
            // See google docs spreadsheet for detail breakdown
            ShadowPower = 1600;
            FirePower = 1540;
            CritRating = 288;
            HitRating = 202;
            HasteRating = 384;
            Intellect = 561;
            Mana = 10000;
            T6_4Piece = true;
            return this;
        }

        // TODO: Take a closer look at T4 and T5 tier sets in particular. Not much difference between them.

        public Warlock ImaginaryGear() {
            ShadowPower = 2000;
            FirePower = 2000;
            CritRating = 600;
            HitRating = 202;
            HasteRating = 400;
            Intellect = 600;
            Mana = 10000;
            T6_4Piece = true;
            return this;
        }

        public Warlock Clone() {
            var c = new Warlock();
            c.Talents = Talents.Clone();
            c.Mana = Mana;
            c.Intellect = Intellect;
            c.ShadowPower = ShadowPower;
            c.FirePower = FirePower;
            c.HitRating = HitRating;
            c.CritRating = CritRating;
            c.HasteRating = HasteRating;
            c.Pet = Pet;
            c.Curse = Curse;
            c.UseCorruption = UseCorruption;
            c.UseImmolate = UseImmolate;
            c.UseSiphonLife = UseSiphonLife;
            c.UseUnstableAffliction = UseUnstableAffliction;
            return c;
        }
    }
}
