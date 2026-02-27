using System;
using System.Threading;

public class EnemyFactory
{
    private Random random;

    public EnemyFactory(Random random)
    {
        this.random = random;
    }

    public Enemy CreateRandomEnemy(int heroLevel)
    {
        int roll = random.Next(1, 4);
        Enemy enemy;

        switch (roll)
        {
            case 1:
                enemy = new Enemy("Гоблин", GameConstants.GOBLIN_HP, GameConstants.GOBLIN_MIN_DAMAGE, GameConstants.GOBLIN_MAX_DAMAGE, GameConstants.GOBLIN_MISS_CHANCE, GameConstants.GOBLIN_CRIT_CHANCE, GameConstants.GOBLIN_EXP, random);
                break;
            case 2:
                enemy = new Enemy("Паук", GameConstants.SPIDER_HP, GameConstants.SPIDER_MIN_DAMAGE, GameConstants.SPIDER_MAX_DAMAGE, GameConstants.SPIDER_MISS_CHANCE, GameConstants.SPIDER_CRIT_CHANCE, GameConstants.SPIDER_EXP, random);
                break;
            default:
                enemy = new Enemy("Скелет", GameConstants.SKELETON_HP, GameConstants.SKELETON_MIN_DAMAGE, GameConstants.SKELETON_MAX_DAMAGE, GameConstants.SKELETON_MISS_CHANCE, GameConstants.SKELETON_CRIT_CHANCE, GameConstants.SKELETON_EXP, random);
                break;
        }

        if (random.Next(1, 101) <= heroLevel * 10)
        {
            enemy.IsElite = true;
            enemy.MaxHP *= GameConstants.ELITE_HP_MULTIPLIER;
            enemy.CurrentHP = enemy.MaxHP;
            enemy.MinDamage *= GameConstants.ELITE_DAMAGE_MULTIPLIER;
            enemy.MaxDamage *= GameConstants.ELITE_DAMAGE_MULTIPLIER;
            enemy.CritChance *= GameConstants.ELITE_CRIT_MULTIPLIER;
            enemy.MissChance = Math.Max(enemy.MissChance / 2, 1);
        }

        string eliteTag = enemy.IsElite ? " [ЭЛИТА]" : "";
        Print($"Навстречу тебе выходит {enemy.Name}{eliteTag} [HP: {enemy.MaxHP}]!");

        SubscribeEnemyEvents(enemy);

        return enemy;
    }

    private void SubscribeEnemyEvents(Enemy enemy)
    {
        enemy.OnDamageTaken += (damage) => Print($"{enemy.Name} получил {damage} урона. Осталось здоровья: {enemy.CurrentHP}");
        enemy.OnDeath += () => Print($"{enemy.Name} погиб!");
        enemy.OnMiss += () => Print($"{enemy.Name} промахивается!");
        enemy.OnCrit += () => Print($"{enemy.Name} наносит критический удар!");
    }

    private void Print(string message, int delay = 1000)
    {
        Console.WriteLine(message);
        Thread.Sleep(delay);
    }
}