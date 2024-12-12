using System;
using System.Threading;
using wShadow.Templates;
using System.Collections.Generic;
using wShadow.Warcraft.Classes;
using wShadow.Warcraft.Defines;
using wShadow.Warcraft.Managers;
using wShadow.WowBots;
using wShadow.WowBots.PartyInfo;





public class EraRetPala : Rotation
{
    private List<string> npcConditions = new List<string>
    {
        "Innkeeper", "Auctioneer", "Banker", "FlightMaster", "GuildBanker",
        "PlayerVehicle", "StableMaster", "Repair", "Trainer", "TrainerClass",
        "TrainerProfession", "Vendor", "VendorAmmo", "VendorFood", "VendorPoison",
        "VendorReagent", "WildBattlePet", "GarrisonMissionNPC", "GarrisonTalentNPC",
        "QuestGiver"
    };
    public bool IsValid(WowUnit unit)
    {
        if (unit == null || unit.Address == null)
        {
            return false;
        }
        return true;
    }
    private bool HasEnchantment(EquipmentSlot slot, string enchantmentName)
    {
        return Api.Equipment.HasEnchantment(slot, enchantmentName);
    }
    private CreatureType GetCreatureType(WowUnit unit)
    {
        return unit.Info.GetCreatureType();
    }
    private bool HasItem(object item) => Api.Inventory.HasItem(item);
    private int debugInterval = 5; // Set the debug interval in seconds
    private DateTime lastDebugTime = DateTime.MinValue;




    public override void Initialize()
    {
        // Can set min/max levels required for this rotation.

        lastDebugTime = DateTime.Now;
        LogPlayerStats();
        // Use this method to set your tick speeds.
        // The simplest calculation for optimal ticks (to avoid key spam and false attempts)

        // Assuming wShadow is an instance of some class containing UnitRatings property
        SlowTick = 750;
        FastTick = 500;

        // You can also use this method to add to various action lists.

        // This will add an action to the internal passive tick.
        // bool: needTarget -> If true action will not fire if player does not have a target
        // Func<bool>: function -> Action to attempt, must return true or false.
        PassiveActions.Add((true, () => false));

        // This will add an action to the internal combat tick.
        // bool: needTarget -> If true action will not fire if player does not have a target
        // Func<bool>: function -> Action to attempt, must return true or false.
        CombatActions.Add((true, () => false));



    }
    public override bool PassivePulse()
    {
        var me = Api.Player;
        var healthPercentage = me.HealthPercent;
        var mana = me.ManaPercent;
        var target = Api.Target;
        var targetDistance = target.Position.Distance2D(me.Position);





        if (me.IsDead() || me.IsGhost() || me.IsCasting() || me.IsMoving() || me.IsChanneling() || me.IsMounted() || me.Auras.Contains("Drink") || me.Auras.Contains("Food")) return false;

        if ((DateTime.Now - lastDebugTime).TotalSeconds >= debugInterval)
        {
            LogPlayerStats();
            lastDebugTime = DateTime.Now; // Update lastDebugTime
        }

        bool CanSelfBuff = false;

        //if (target.Guid.Equals(me.Guid))
          //  CanSelfBuff = true;

        //if (target.Guid.IsEmpty())
          //  CanSelfBuff = true;



        if (me.IsValid())
        {
            if (Api.Spellbook.CanCast("Holy Light") && healthPercentage <= 50 && mana > 20)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Holy Light");
                Console.ResetColor();
                if (Api.Spellbook.Cast("Holy Light"))
                {
                    return true;
                }
            }
            var hasDisease = me.Auras.Contains("Contagion of Rot") ||
                                  me.Auras.Contains("Bonechewer Rot") ||
                                  me.Auras.Contains("Ghoul Rot") ||
                                  me.Auras.Contains("Maggot Slime") ||
                                  me.Auras.Contains("Corrupted Strength") ||
                                  me.Auras.Contains("Corrupted Agility") ||
                                  me.Auras.Contains("Corrupted Intellect") ||
                                  me.Auras.Contains("Corrupted Stamina") ||
                                  me.Auras.Contains("Black Rot") ||
                                  me.Auras.Contains("Volatile Infection") ||
                                  me.Auras.Contains("Ghoul Plague") ||
                                  me.Auras.Contains("Corrupting Plague") ||
                                  me.Auras.Contains("Lacerating Bite") ||
                                  me.Auras.Contains("Sporeskin") ||
                                  me.Auras.Contains("Cadaver Worms") ||
                                  me.Auras.Contains("Rabies") ||
                                  me.Auras.Contains("Diseased Shot") ||
                                  me.Auras.Contains("Tetanus") ||
                                  me.Auras.Contains("Dredge Sickness") ||
                                  me.Auras.Contains("Noxious Catalyst") ||
                                  me.Auras.Contains("Spirit Decay") ||
                                  me.Auras.Contains("Withered Touch") ||
                                  me.Auras.Contains("Putrid Enzyme") ||
                                  me.Auras.Contains("Infected Wound") ||
                                  me.Auras.Contains("Infected Spine") ||
                                  me.Auras.Contains("Black Sludge") ||
                                  me.Auras.Contains("Silithid Pox") ||
                                  me.Auras.Contains("Festering Rash") ||
                                  me.Auras.Contains("Dark Sludge") ||
                                  me.Auras.Contains("Fevered Fatigue") ||
                                  me.Auras.Contains("Muculent Fever") ||
                                  me.Auras.Contains("Infected Bite") ||
                                  me.Auras.Contains("Fungal Decay") ||
                                  me.Auras.Contains("Diseased Spit") ||
                                  me.Auras.Contains("Choking Vines") ||
                                  me.Auras.Contains("Fevered Disease") ||
                                  me.Auras.Contains("Lingering Vines") ||
                                  me.Auras.Contains("Festering Wound") ||
                                  me.Auras.Contains("Creeping Vines") ||
                                  me.Auras.Contains("Parasite") ||
                                  me.Auras.Contains("Wandering Plague") ||
                                  me.Auras.Contains("Irradiated") ||
                                  me.Auras.Contains("Dark Plague") ||
                                  me.Auras.Contains("Plague Mind") ||
                                  me.Auras.Contains("Diseased Slime") ||
                                  me.Auras.Contains("Putrid Stench") ||
                                  me.Auras.Contains("Wither") ||
                                  me.Auras.Contains("Seething Plague") ||
                                  me.Auras.Contains("Death's Door") ||
                                  me.Auras.Contains("Plague Strike");

            if (hasDisease && Api.Spellbook.CanCast("Purify") && mana > 32)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Have poison debuff casting Purify");
                Console.ResetColor();
                if (Api.Spellbook.Cast("Purify"))

                    return true;
            }


            if (Api.Spellbook.CanCast("Blessing of Might") && !me.Auras.Contains("Blessing of Might") && mana > 15)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Blessing of Might");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Blessing of Might"))
                {
                    return true;
                }
            }


            if (Api.Spellbook.CanCast("Sanctity Aura") && !me.Auras.Contains("Sanctity Aura", false))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Sanctity Aura");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Sanctity Aura"))
                {
                    return true;
                }
            }
            else
            if (Api.Spellbook.CanCast("Devotion Aura") && !me.Auras.Contains("Devotion Aura", false) && !me.Auras.Contains("Sanctity Aura", false))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Devotion Aura");
                Console.ResetColor();

                if (Api.Spellbook.Cast("Devotion Aura"))
                {
                    return true;
                }
            }

        }
        if (!me.Auras.Contains("Seal of Command") && Api.Spellbook.CanCast("Seal of Command"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Seal of Command");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Seal of Command"))
                return true;
        }
        if (!me.Auras.Contains("Seal of Wisdom") && Api.Spellbook.CanCast("Seal of Wisdom") && !Api.Spellbook.OnCooldown("Seal of Wisdom") && !me.Auras.Contains("Seal of Command") && mana > 15)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Seal of Wisdom");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Seal of Wisdom"))

                return true;

        }
        else if (!me.Auras.Contains("Seal of Righteousness") && Api.Spellbook.CanCast("Seal of Righteousness") && !Api.Spellbook.OnCooldown("Seal of Righteousness") && !me.Auras.Contains("Seal of Wisdom") && !me.Auras.Contains("Seal of Command") && mana > 15)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Seal of Righteousness");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Seal of Righteousness"))

                return true;

        }
        var reaction = me.GetReaction(target);
        if (target.IsValid())
        {
            if (!target.IsDead() &&
            (reaction != UnitReaction.Friendly &&
             reaction != UnitReaction.Honored &&
             reaction != UnitReaction.Revered &&
             reaction != UnitReaction.Exalted) &&
            mana > 20 && !IsNPC(target))
                if (Api.Spellbook.CanCast("Judgement") && targetDistance > 5 && targetDistance <10 && targetDistance < 10 && !Api.Spellbook.OnCooldown("Judgement"))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Casting Judgement");
                    Console.ResetColor();

                    if (Api.Spellbook.Cast("Judgement"))
                        return true;
                }
            else  if (Api.Spellbook.CanCast("Attack") && !me.IsAutoAttacking())
                {
                    Api.Spellbook.Cast("Attack");
                    Console.WriteLine("Attacking");
                    return true;
                }

            }
        

            return base.PassivePulse();
    }



    public override bool CombatPulse()
    {
        var me = Api.Player;
        var healthPercentage = me.HealthPercent;
        var mana = me.ManaPercent;
        var target = Api.Target;
        var targethp = target.HealthPercent;
        var targetHealth = Api.Target.HealthPercent;
        var targetDistance = target.Position.Distance2D(me.Position);

        if (!me.IsValid() || !target.IsValid() || me.IsDead() || me.IsGhost() || me.IsCasting() || me.IsMoving() || me.IsChanneling() || me.IsMounted() || me.Auras.Contains("Drink") || me.Auras.Contains("Food")) return false;


        string[] HP = { "Major Healing Potion", "Superior Healing Potion", "Greater Healing Potion", "Healing Potion", "Lesser Healing Potion", "Minor Healing Potion" };
        string[] MP = { "Major Mana Potion", "Superior Mana Potion", "Greater Mana Potion", "Mana Potion", "Lesser Mana Potion", "Minor Mana Potion" };

        foreach (string hpot in HP)
        {
            if (HasItem(hpot) && (!Api.Inventory.OnCooldown(HP) || !Api.Inventory.OnCooldown(MP)) && healthPercentage < 70)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Using {hpot}");
                Console.ResetColor();

                if (Api.Inventory.Use(hpot))
                {
                    return true;
                }
            }
        }

        foreach (string mpot in MP)
        {
            if (HasItem(mpot) && (!Api.Inventory.OnCooldown(MP) || !Api.Inventory.OnCooldown(HP)) && mana < 50)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Using {mpot}");
                Console.ResetColor();

                if (Api.Inventory.Use(mpot))
                {
                    return true;
                }
            }
        }





        if (Api.Spellbook.CanCast("Sanctity Aura") && !me.Auras.Contains("Sanctity Aura", false))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Sanctity Aura");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Sanctity Aura"))
            {
                return true;
            }
        }
        else
         if (Api.Spellbook.CanCast("Devotion Aura") && !me.Auras.Contains("Devotion Aura", false) && !me.Auras.Contains("Sanctity Aura", false))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Devotion Aura");
            Console.ResetColor();

            if (Api.Spellbook.Cast("Devotion Aura"))
            {
                return true;
            }
        }
        if (Api.Spellbook.CanCast("Divine Protection") && Api.Player.HealthPercent < 45 && !me.IsCasting() && !Api.Player.Auras.Contains("Forbearance", false) && !Api.Spellbook.OnCooldown("Divine Protection"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Divine Protection");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Divine Protection"))
            {
                return true;
            }
        }

        if (me.Auras.Contains("Divine Protection") && healthPercentage <= 50 && Api.Spellbook.CanCast("Holy Light"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Holy Light");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Holy Light"))
            {
                return true;
            }
        }

        if (Api.Spellbook.CanCast("Blessing of Protection") && Api.Player.HealthPercent < 30 && !me.IsCasting() && !Api.Player.Auras.Contains("Forbearance", false) && !Api.Spellbook.OnCooldown("Blessing of Protection"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Blessing of Protection");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Blessing of Protection"))
            {
                return true;
            }
        }

        if (Api.Player.Auras.Contains("Blessing of Protection") && healthPercentage <= 35 && Api.Spellbook.CanCast("Holy Light") && mana > 20)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Holy Light");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Holy Light"))
            {
                return true;
            }
        }
        if (healthPercentage <= 10 && Api.Spellbook.CanCast("Lay on Hands") && !Api.Spellbook.OnCooldown("Lay on Hands"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Lay on Hands");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Lay on Hands"))
            {
                return true;
            }
        }

        if (Api.Spellbook.CanCast("Holy Light") && healthPercentage <= 50 && mana > 20)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Holy Light");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Holy Light"))

                return true;

        }
        if (Api.Spellbook.CanCast("Exorcism")  && targetDistance <= 30)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Exorcism");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Exorcism"))
                return true;
        }
        if (Api.Spellbook.CanCast("Hammer of Wrath") && targetHealth <= 20)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Hammer of Wrath");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Hammer of Wrath"))
            {
                return true;
            }
        }

        if (Api.Spellbook.CanCast("Consecration") && !Api.Spellbook.OnCooldown("Consecration") && targethp >= 30 && mana > 30 && Api.UnfriendlyUnitsNearby(5, true) >= 2)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Consecration");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Consecration"))

                return true;

        }
        if (!me.Auras.Contains("Seal of Command") && Api.Spellbook.CanCast("Seal of Command"))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Seal of Command");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Seal of Command"))
                return true;
        }
        if (!me.Auras.Contains("Seal of Wisdom") && Api.Spellbook.CanCast("Seal of Wisdom") && !Api.Spellbook.OnCooldown("Seal of Wisdom") && !me.Auras.Contains("Seal of Command") && mana > 15)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Seal of Wisdom");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Seal of Wisdom"))

                return true;

        }
        else if (!me.Auras.Contains("Seal of Righteousness") && Api.Spellbook.CanCast("Seal of Righteousness") && !Api.Spellbook.OnCooldown("Seal of Righteousness") && !me.Auras.Contains("Seal of Wisdom") && !me.Auras.Contains("Seal of Command") && mana > 15)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Seal of Righteousness");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Seal of Righteousness"))

                return true;

        }

        if (Api.Spellbook.CanCast("Hammer of Justice") && mana > 10 && !Api.Spellbook.OnCooldown("Hammer of Justice") && (target.IsCasting() || target.IsChanneling()))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Hammer of Justice");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Hammer of Justice"))
            {
                return true;
            }
        }


        if (Api.Spellbook.CanCast("Judgement") && mana > 15 && !Api.Spellbook.OnCooldown("Judgement") && (me.Auras.Contains("Seal of Righteousness") || me.Auras.Contains("Seal of Wisdom")))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Judgement");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Judgement"))
            {
                return true;
            }
        }

        
        if (Api.Spellbook.CanCast("Attack") && !me.IsAutoAttacking())
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Attack");
            Console.ResetColor();
            if (Api.Spellbook.Cast("Attack"))
            {
                return true;
            }
        }

        //DPS rotation

        return base.CombatPulse();
    }


    private bool IsNPC(WowUnit unit)
    {
        if (!IsValid(unit))
        {
            // If the unit is not valid, consider it not an NPC
            return false;
        }

        foreach (var condition in npcConditions)
        {
            switch (condition)
            {
                case "Innkeeper" when unit.IsInnkeeper():
                case "Auctioneer" when unit.IsAuctioneer():
                case "Banker" when unit.IsBanker():
                case "FlightMaster" when unit.IsFlightMaster():
                case "GuildBanker" when unit.IsGuildBanker():
                case "StableMaster" when unit.IsStableMaster():
                case "Trainer" when unit.IsTrainer():
                case "Vendor" when unit.IsVendor():
                case "QuestGiver" when unit.IsQuestGiver():
                    return true;
            }
        }

        return false;
    }
    private void LogPlayerStats()
    {
        var me = Api.Player;

        var mana = me.Mana;
        var healthPercentage = me.HealthPercent;


        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"{mana} Mana available");
        Console.WriteLine($"{healthPercentage}% Health available");

        // Assuming me is an instance of a player character
        var hasPoisonDebuff = me.Auras.Contains("Poison");

        if (hasPoisonDebuff)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Have poison debuff");
            Console.ResetColor();
        }




    }
}
