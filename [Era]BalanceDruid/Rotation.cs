using System;
using System.Threading;
using wShadow.Templates;
using System.Collections.Generic;
using wShadow.Warcraft.Classes;
using wShadow.Warcraft.Defines;
using wShadow.Warcraft.Managers;

public class EraBalanceDruid : Rotation
{
    private List<string> npcConditions = new List<string>
    {
        "Innkeeper", "Auctioneer", "Banker", "FlightMaster", "GuildBanker",
        "StableMaster", "Trainer", "Vendor", "QuestGiver"
    };

    private int debugInterval = 20; 
    private DateTime lastDebugTime = DateTime.MinValue;

    public override void Initialize()
    {
        lastDebugTime = DateTime.Now;
        LogPlayerStats();
        SlowTick = 600;
        FastTick = 150;
        PassiveActions.Add((true, () => false));
        CombatActions.Add((true, () => false));
    }

    public override bool PassivePulse()
    {
        var me = Api.Player;

        if (!IsPlayerReady(me)) return false;

        if ((DateTime.Now - lastDebugTime).TotalSeconds >= debugInterval)
        {
            LogPlayerStats();
            lastDebugTime = DateTime.Now;
        }

        if (me.IsValid())
        {
            if (BuffSelf("Mark of the Wild", me)) return true;
            if (BuffSelf("Thorns", me)) return true;
            if (BuffSelf("Omen of Clarity", me)) return true;
            if (HealSelf("Rejuvenation", me, 60)) return true;
            if (HealSelf("Regrowth", me, 40)) return true;
            if (HealSelf("Healing Touch", me, 30)) return true;
            if (TransformToMoonkin(me)) return true;
        }

        var target = Api.Target;
        if (target.IsValid() && IsHostileTarget(target) && me.ManaPercent > 20)
        {
            if (CCOrAttackTarget(target, me)) return true;
        }

        return base.PassivePulse();
    }

    public override bool CombatPulse()
    {
        var me = Api.Player;
        var target = Api.Target;

        if (!IsPlayerReady(me) || !target.IsValid()) return false;

        UseConsumables(me);

        if (HealSelf("Rejuvenation", me, 70)) return true;
        if (HealSelf("Healing Touch", me, 45)) return true;
        if (TransformToMoonkin(me)) return true;

        if (AttackWithSpells(target, me)) return true;

        return base.CombatPulse();
    }

    private bool IsPlayerReady(WowUnit me)
    {
        return !(me.IsDead() || me.IsGhost() || me.IsCasting() || me.IsMoving() || me.IsChanneling() || me.IsMounted() || me.Auras.Contains("Drink") || me.Auras.Contains("Food"));
    }

    private bool BuffSelf(string spellName, WowUnit me)
    {
        if (Api.Spellbook.CanCast(spellName) && !me.Auras.Contains(spellName))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Casting {spellName}");
            Console.ResetColor();
            return Api.Spellbook.Cast(spellName, Api.GUIDs.Myself);
        }
        return false;
    }

    private bool HealSelf(string spellName, WowUnit me, int healthThreshold)
    {
        if (Api.Spellbook.CanCast(spellName) && me.HealthPercent <= healthThreshold && !me.Auras.Contains(spellName))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Casting {spellName}");
            Console.ResetColor();
            return Api.Spellbook.Cast(spellName, Api.GUIDs.Myself);
        }
        return false;
    }

    private bool TransformToMoonkin(WowUnit me)
    {
        if (Api.Spellbook.CanCast("Moonkin Form") && !me.Auras.Contains("Moonkin Form", false))
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Moonkin Form");
            Console.ResetColor();
            return Api.Spellbook.Cast("Moonkin Form");
        }
        return false;
    }

  private bool CCOrAttackTarget(WowUnit target, WowUnit me)
{
    double distance = Api.Distance3D(Api.Me, target);

    // Cast Entangling Roots if target is between 5 and 30 yards
    if (Api.Spellbook.CanCast("Entangling Roots") &&
        !target.Auras.Contains("Entangling Roots") &&
        distance <= 30 && distance > 5)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Casting Entangling Roots at {distance:F1} yards");
        Console.ResetColor();
        return Api.Spellbook.Cast("Entangling Roots");
    }

    // Cast Moonfire if not already applied
    if (Api.Spellbook.CanCast("Moonfire") && !target.Auras.Contains("Moonfire"))
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Casting Moonfire");
        Console.ResetColor();
        return Api.Spellbook.Cast("Moonfire");
    }

    // Cast Wrath as fallback
    if (Api.Spellbook.CanCast("Wrath"))
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Casting Wrath");
        Console.ResetColor();
        return Api.Spellbook.Cast("Wrath");
    }

    return false;
}


    private bool AttackWithSpells(WowUnit target, WowUnit me)
    {
        if (Api.Spellbook.CanCast("Starfire") && me.Auras.Contains(417157) && me.ManaPercent > 10)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Starfire");
            Console.ResetColor();
            return Api.Spellbook.Cast("Starfire");
        }

        if (Api.Spellbook.CanCast("Wrath") && me.Auras.Contains(408248) && me.ManaPercent > 10)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Wrath with Eclipse");
            Console.ResetColor();
            return Api.Spellbook.Cast("Wrath");
        }

        if (Api.Spellbook.CanCast("Attack") && !me.IsAutoAttacking())
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Casting Attack");
            Console.ResetColor();
            return Api.Spellbook.Cast("Attack");
        }

        return false;
    }

    private void UseConsumables(WowUnit me)
    {
        string[] healthPotions = { "Major Healing Potion", "Superior Healing Potion", "Greater Healing Potion" };
        string[] manaPotions = { "Major Mana Potion", "Superior Mana Potion", "Greater Mana Potion" };

        if (me.HealthPercent <= 70)
        {
            UseFirstAvailableItem(healthPotions);
        }

        if (me.ManaPercent <= 50)
        {
            UseFirstAvailableItem(manaPotions);
        }
    }

    private void UseFirstAvailableItem(string[] items)
    {
        foreach (string item in items)
        {
            if (Api.Inventory.HasItem(item) && Api.Inventory.Use(item))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Using {item}");
                Console.ResetColor();
                break;
            }
        }
    }

    private bool IsHostileTarget(WowUnit target)
    {
        var reaction = Api.Player.GetReaction(target);
        return reaction != UnitReaction.Friendly &&
               reaction != UnitReaction.Honored &&
               reaction != UnitReaction.Revered &&
               reaction != UnitReaction.Exalted;
    }

    private void LogPlayerStats()
    {
        var me = Api.Player;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"{me.ManaPercent}% Mana available");
        Console.WriteLine($"{me.HealthPercent}% Health available");
        Console.ResetColor();
    }
}
