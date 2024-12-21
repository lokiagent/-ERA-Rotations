using System;
using System.Collections.Generic;
using wShadow.Templates;
using wShadow.Warcraft.Classes;
using wShadow.Warcraft.Defines;
using wShadow.Warcraft.Managers;

public class EraFeralDruidBear : Rotation
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
        var bearFormCost = Api.Spellbook.SpellCost("Bear Form");

        // Healing logic: Drop Bear Form, heal, and rebuff
        if (healthPercentage < 50)
        {
            if (me.Auras.Contains("Bear Form"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Shifting out of Bear Form to heal");
                Console.ResetColor();
                Api.Spellbook.Cast("Bear Form"); // Shift out of Bear Form
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

            // Ensure mana to return to Bear Form
            if (mana >= bearFormCost)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Shifting back to Bear Form");
                Console.ResetColor();
                return Api.Spellbook.Cast("Bear Form");
            }

            return false; // Wait for enough mana
        }

        // Ensure Bear Form for combat
        if (!me.Auras.Contains("Bear Form"))
        {
            if (Api.Spellbook.CanCast("Bear Form") && mana >= bearFormCost)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Shifting to Bear Form");
                Console.ResetColor();
                return Api.Spellbook.Cast("Bear Form");
            }

            return false; // Wait for enough mana to shift
        }

        // In Bear Form: Melee Combat
        if (distance <= 5)
        {
            // Use Mangle if the debuff isn't applied
            if (Api.Spellbook.CanCast("Mangle") && !target.Auras.Contains("Mangle (Bear)"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Mangle");
                Console.ResetColor();
                return Api.Spellbook.Cast("Mangle");
            }

            // Spam Maul
            if (Api.Spellbook.CanCast("Maul"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Maul");
                Console.ResetColor();
                return Api.Spellbook.Cast("Maul");
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
