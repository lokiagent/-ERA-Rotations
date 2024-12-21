using System;
using System.Collections.Generic;
using wShadow.Templates;
using wShadow.Warcraft.Classes;
using wShadow.Warcraft.Defines;
using wShadow.Warcraft.Managers;

public class EraFeralDruidCat : Rotation
{
    private List<string> npcConditions = new List<string>
    {
        "Innkeeper", "Auctioneer", "Banker", "FlightMaster", "GuildBanker",
        "PlayerVehicle", "StableMaster", "Repair", "Trainer", "TrainerClass",
        "TrainerProfession", "Vendor", "VendorAmmo", "VendorFood", "VendorPoison",
        "VendorReagent", "WildBattlePet", "GarrisonMissionNPC", "GarrisonTalentNPC",
        "QuestGiver"
    };

    public override bool PassivePulse()
    {
        var me = Api.Player;
        var target = Api.Target;

        if (me.IsDead() || me.IsGhost() || me.IsChanneling() || me.Auras.Contains("Drink") || me.Auras.Contains("Food"))
            return false;

        if (!target.IsValid() || target.IsDead() || IsNPC(target))
            return false;

        var distance = Api.Distance3D(Api.Me, target);
        var healthPercentage = me.HealthPercent;
        var mana = me.ManaPercent;
        var comboPoints = Api.Player.ComboPoints;
        var catFormCost = Api.Spellbook.SpellCost("Cat Form");

        // Healing logic: Drop Cat Form, heal, and rebuff
        if (healthPercentage < 40)
        {
            if (me.Auras.Contains("Cat Form"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Shifting out of Cat Form to heal");
                Console.ResetColor();
                Api.Spellbook.Cast("Cat Form"); // Shift out of Cat Form
            }

            if (Api.Spellbook.CanCast("Rejuvenation") && mana >= 15)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Rejuvenation");
                Console.ResetColor();
                return Api.Spellbook.Cast("Rejuvenation");
            }

            if (Api.Spellbook.CanCast("Healing Touch") && mana >= 20)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Healing Touch");
                Console.ResetColor();
                return Api.Spellbook.Cast("Healing Touch");
            }

            if (Api.Spellbook.CanCast("Moonfire"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Moonfire");
                Console.ResetColor();
                return Api.Spellbook.Cast("Moonfire");
            }

            // Ensure mana to return to Cat Form
            if (mana >= catFormCost)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Shifting back to Cat Form");
                Console.ResetColor();
                return Api.Spellbook.Cast("Cat Form");
            }

            return false; // Wait for enough mana
        }

        // Ensure Cat Form for combat
        if (!me.Auras.Contains("Cat Form"))
        {
            if (Api.Spellbook.CanCast("Cat Form") && mana >= catFormCost)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Shifting to Cat Form");
                Console.ResetColor();
                return Api.Spellbook.Cast("Cat Form");
            }

            return false; // Wait for enough mana to shift
        }

        // In Cat Form: Melee Combat
        if (distance <= 5)
        {
            // Use Ferocious Bite with 5 Combo Points if learned
            if (Api.Spellbook.CanCast("Ferocious Bite") && comboPoints == 5)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Ferocious Bite");
                Console.ResetColor();
                return Api.Spellbook.Cast("Ferocious Bite");
            }

            // Spam Claw
            if (Api.Spellbook.CanCast("Claw"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Claw");
                Console.ResetColor();
                return Api.Spellbook.Cast("Claw");
            }
        }

        return base.PassivePulse();
    }

    public override void Initialize()
    {
        SlowTick = 600;
        FastTick = 150;
    }

    private bool IsNPC(WowUnit unit)
    {
        if (unit == null || unit.Address == null)
            return false;

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
}
