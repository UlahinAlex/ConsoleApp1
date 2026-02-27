using System;
using System.Threading;

public class Base
{
    private Weapon axe;
    private Weapon swordAndShield;
    private Weapon bow;

    public Base()
    {
        axe = new Weapon("Топор", GameConstants.AXE_MIN_DAMAGE, GameConstants.AXE_MAX_DAMAGE, GameConstants.AXE_CRIT_MULTIPLIER, GameConstants.AXE_DODGE_MODIFIER, GameConstants.AXE_DOUBLE_ATTACK_CHANCE, GameConstants.AXE_BLOCK_CHANCE, false);

        swordAndShield = new Weapon("Меч со щитом", GameConstants.SWORD_MIN_DAMAGE, GameConstants.SWORD_MAX_DAMAGE, GameConstants.SWORD_CRIT_MULTIPLIER, GameConstants.SWORD_DODGE_MODIFIER, GameConstants.SWORD_DOUBLE_ATTACK_CHANCE, GameConstants.SWORD_BLOCK_CHANCE, false);
        
        bow = new Weapon("Лук", GameConstants.BOW_MIN_DAMAGE, GameConstants.BOW_MAX_DAMAGE, GameConstants.BOW_CRIT_MULTIPLIER, GameConstants.BOW_DODGE_MODIFIER, GameConstants.BOW_DOUBLE_ATTACK_CHANCE, GameConstants.BOW_BLOCK_CHANCE, isBow: true);
    }

    public void Enter(Character hero)
    {
        Console.WriteLine("\n=== БАЗА ===");
        hero.HealBase();

        Weapon chosen = ChooseWeapon();
        hero.EquipWeapon(chosen);
        Print($"Ты выбрал: {chosen.Name}!");

        while (true)
        {
            Console.WriteLine("\nВведи команду (В путь / Выход):");
            string cmd = Console.ReadLine().ToLower().Trim();
            if (cmd == "в путь") break;
            else if (cmd == "выход")
            {
                Console.WriteLine("До свидания!");
                Environment.Exit(0);
            }
            else Console.WriteLine("Не могу этого сделать!");
        }
    }

    private Weapon ChooseWeapon()
    {
        Console.WriteLine("\nВыбери оружие:");
        Console.WriteLine("1. Топор (урон 8-16, х2 крит, 15% двойная атака)");
        Console.WriteLine("2. Меч со щитом (урон 10-15, -5% уворот, 80% блок)");
        Console.WriteLine("3. Лук (урон 12-20, +30% уворот, -80% урон скелету)");

        Weapon chosen = null;
        while (chosen == null)
        {
            string input = Console.ReadLine().Trim();
            switch (input)
            {
                case "1": chosen = axe; break;
                case "2": chosen = swordAndShield; break;
                case "3": chosen = bow; break;
                default: Console.WriteLine("Введи 1, 2 или 3!"); break;
            }
        }
        return chosen;
    }

    private void Print(string message, int delay = 1000)
    {
        Console.WriteLine(message);
        Thread.Sleep(delay);
    }
}