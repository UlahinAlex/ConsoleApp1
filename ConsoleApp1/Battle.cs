using System;
using System.Threading;

public class Battle
{
    private Random random;



    public Battle(Random random)
    {
        this.random = random;
    }

    public void HandleBattle(Player hero, Character enemy)
    {
        bool vsSkeleton = enemy.Name == "Скелет";

        hero.ApplyEffects();

        if (!hero.isAlive())
        {
            Print($"{hero.Name} погиб от яда!");
            return;
        }

        if (hero.InStunned)
        {
            Print($"{hero.Name} оглушен и пропускает атаку!");
            hero.InStunned = false;
        }
        else
        {
            string action = GetPlayerAction();
            ExecutePlayerAction(action, hero, enemy, vsSkeleton);
        }

        if (!enemy.isAlive()) return;

        ExecuteEnemyAction(hero, enemy);
    }

    public bool HandleVictory(Player hero, Enemy enemy, List<Character> enemies)
    {
        Console.WriteLine($"\n> Я победил {enemy.Name}!");
        Print($"{hero.Name} : {hero.CurrentHP}/{hero.MaxHP} здоровья.");

        enemies.Remove(enemy);

        int expReward = 0;
        switch (enemy.Name)
        {
            case "Гоблин": expReward = GameConstants.GOBLIN_EXP; break;
            case "Паук": expReward = GameConstants.SPIDER_EXP; break;
            case "Скелет": expReward = GameConstants.SKELETON_EXP; break;
        }
        if (enemy.IsElite) expReward *= GameConstants.ELITE_EXP_MULTIPLIER;
        hero.GainExperience(expReward);

        while (true)
        {
            Console.WriteLine("\nВведи команду (На базу / Вперед):");
            string cmd = Console.ReadLine().ToLower().Trim();

            switch (cmd)
            {
                case "на базу":
                    enemies.Clear();
                    return false;
                case "вперед":
                    EnemyFactory factory = new EnemyFactory(random);
                    enemies.Add(factory.CreateRandomEnemy(hero.Level));
                    return true;
                default:
                    Console.WriteLine("Не могу этого сделать!");
                    break;
            }
        }
    }

    private string GetPlayerAction()
    {
        while (true)
        {
            Console.WriteLine("\nВведи команду (атака / блок / уворот):");
            string action = Console.ReadLine().ToLower().Trim();
            if (action == "атака" || action == "блок" || action == "уворот")
                return action;
            Console.WriteLine("Не могу этого сделать!");
        }
    }

    private void ExecutePlayerAction(string action, Player hero, Character enemy, bool vsSkeleton)
    {
        switch (action)
        {
            case "атака":
                enemy.TakeDamage(hero.GetDamage(vsSkeleton));
                if (!enemy.isAlive()) return;

                if (hero.EquippedWeapon != null && hero.EquippedWeapon.DoubleAttackChance > 0)
                {
                    if (random.Next(1, 101) <= hero.EquippedWeapon.DoubleAttackChance)
                    {
                        Print("Двойная атака!");
                        enemy.TakeDamage(hero.GetDamage(vsSkeleton));
                    }
                }
                break;
            case "блок":
                int newBlockChance = (hero.EquippedWeapon?.BlockChance ?? 0) + hero.BonusBlockChance + GameConstants.BONUS_BLOCK_ON_DEFEND;
                hero.BonusBlockChance = Math.Min(newBlockChance, GameConstants.MAX_BLOCK_CHANCE);
                Print($"{hero.Name} принимает защитную стойку!");
                break;
            case "уворот":
                hero.BonusDodgeChance = Math.Min(hero.BonusDodgeChance + GameConstants.BONUS_DODGE_ON_EVADE, GameConstants.MAX_DODGE_CHANCE);
                Print($"{hero.Name} готовится к уворту!");
                break;
        }
    }

    private void ExecuteEnemyAction(Player hero, Character enemy)
    {
        int damage = enemy.GetDamage();

        int totalDodge = hero.MissChance + hero.BonusDodgeChance;
        if (hero.BonusDodgeChance > 0 && random.Next(1, 101) <= totalDodge)
        {
            Print($"{hero.Name} уворачивается от атаки!");
            hero.BonusDodgeChance = 0;
            hero.BonusBlockChance = 0;
            return;
        }
        hero.BonusDodgeChance = 0;

        int totalBlock = (hero.EquippedWeapon?.BlockChance ?? 0) + hero.BonusBlockChance;
        if (totalBlock > 0 && random.Next(1, 101) <= totalBlock)
        {
            Print($"{hero.Name} блокирует атаку!");
            hero.BonusBlockChance = 0;
            if (enemy.Name == "Скелет")
            {
                enemy.MinDamage++;
                enemy.MaxDamage++;
                Print($"Скелет становится сильнее! Урон: {enemy.MinDamage}-{enemy.MaxDamage}");
            }
            return;
        }
        hero.BonusBlockChance = 0;

        hero.TakeDamage(damage);

        if (damage > 0)
        {
            switch (enemy.Name)
            {
                case "Паук":
                    hero.PoisonTurns = GameConstants.SPIDER_POISON_TURNS;
                    Print($"{hero.Name} отравлен!");
                    break;
                case "Гоблин":
                    if (random.Next(GameConstants.GOBLIN_STUN_CHANCE) <= 30)
                    {
                        hero.InStunned = true;
                        Print($"{enemy.Name} оглушил {hero.Name}!");
                    }
                    break;
                case "Скелет":
                    enemy.MinDamage++;
                    enemy.MaxDamage++;
                    Print($"Скелет становится сильнее! Урон: {enemy.MinDamage}-{enemy.MaxDamage}");
                    break;
            }
        }
    }

    private void Print(string message, int delay = 1000)
    {
        Console.WriteLine(message);
        Thread.Sleep(delay);
    }
}