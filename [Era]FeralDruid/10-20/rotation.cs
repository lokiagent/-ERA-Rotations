using System;
using System.Collections.Generic;
using wShadow.Templates;
using wShadow.Warcraft.Classes;
using wShadow.Warcraft.Defines;
using wShadow.Warcraft.Managers;

public class EraFeralDruid : Rotation
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

        // Heal if health is below 50% and ensure mana to shift back
        if (healthPercentage <= 50)
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

            return false; // Wait until healed
        }

        // Cast at range
        if (distance > 5 && distance <= 30)
        {
            if (Api.Spellbook.CanCast("Entangling Roots") && !target.Auras.Contains("Entangling Roots"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Entangling Roots");
                Console.ResetColor();
                return Api.Spellbook.Cast("Entangling Roots");
            }

            if (Api.Spellbook.CanCast("Moonfire") && !target.Auras.Contains("Moonfire"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Moonfire");
                Console.ResetColor();
                return Api.Spellbook.Cast("Moonfire");
            }

            if (Api.Spellbook.CanCast("Wrath"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Wrath");
                Console.ResetColor();
                return Api.Spellbook.Cast("Wrath");
            }
        }

        // Melee combat
        if (distance <= 5)
        {
            // Shift to Bear Form if not already in it
            if (!me.Auras.Contains("Bear Form"))
            {
                if (Api.Spellbook.CanCast("Bear Form") && mana >= 10)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Shifting to Bear Form");
                    Console.ResetColor();
                    return Api.Spellbook.Cast("Bear Form");
                }

                return false; // Wait until enough mana to shift
            }

            // Bear Form abilities
            if (Api.Spellbook.CanCast("Mangle") && !target.Auras.Contains("Mangle"))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Casting Mangle");
                Console.ResetColor();
                return Api.Spellbook.Cast("Mangle");
            }

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
