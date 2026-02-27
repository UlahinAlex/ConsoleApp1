using System;
using System.Collections.Generic;
using System.Threading;

public class Game
{
    private Player hero;
    private List<Character> enemies;
    private Battle battle;
    private Base gameBase;
    private EnemyFactory enemyFactory;
    private Random random;
    private bool onBase;

    public Game()
    {
        random = new Random();
        enemies = new List<Character>();
        battle = new Battle(random);
        gameBase = new Base();
        enemyFactory = new EnemyFactory(random);
        onBase = true;
    }

    public void Run()
    {
        Console.WriteLine("Добро пожаловать в симулятор боя!");
        Console.WriteLine("Как тебя зовут?");
        string name = Console.ReadLine();

        hero = new Player(name, GameConstants.HERO_START_HP, GameConstants.HERO_START_MIN_DAMAGE, GameConstants.HERO_START_MAX_DAMAGE, GameConstants.HERO_MISS_CHANCE, GameConstants.HERO_CRIT_CHANCE, GameConstants.HERO_START_HP, random);
        Console.WriteLine($">Меня зовут {hero.Name}, у меня {hero.CurrentHP} здоровья.");

        SubscribeHeroEvents();

        while (hero.isAlive())
        {
            if (onBase)
            {
                gameBase.Enter(hero);
                onBase = false;
                enemies.Add(enemyFactory.CreateRandomEnemy(hero.Level));
                continue;
            }

            if (enemies.Count > 0)
            {
                Enemy enemy = (Enemy)enemies[0];
                battle.HandleBattle(hero, enemy);

                if (!enemy.isAlive())
                {
                    bool goForward = battle.HandleVictory(hero, enemy, enemies);
                    if (!goForward) onBase = true;
                }
            }
        }

        if (!hero.isAlive())
        {
            Console.WriteLine($"\n{hero.Name} погиб. Игра окончена.");
        }
    }

    private void SubscribeHeroEvents()
    {
        hero.OnDamageTaken += (damage) => Print($"{hero.Name} получил {damage} урона. Осталось здоровья: {hero.CurrentHP}");
        hero.OnDeath += () => Print($"{hero.Name} погиб!");
        hero.OnMiss += () => Print($"{hero.Name} промахивается!");
        hero.OnCrit += () => Print($"{hero.Name} наносит критический удар!");
        hero.OnBowVsSkeleton += () => Print("Стрелы плохо пробивают кости! Урон снижен.");
        hero.OnWeaponEquipError += () => Print("Ошибка: оружие не выбрано!");
        hero.OnExpGained += (amount) => Print($"{hero.Name} получает {amount} опыта. Всего: {hero.Experience}");
        hero.OnLevelUp += (level, hp, maxDamage) =>
        {
            Console.WriteLine($"Уровень повышен! Теперь {hero.Name} {level} уровня.");
            Print($"HP: {hp} | Урон: {hero.MinDamage}-{maxDamage}");
        };
        hero.OnPoisonDamage += (damage) => Print($"{hero.Name} получает {damage} урона от яда. Осталось здоровья: {hero.CurrentHP}. Ходов яда: {hero.PoisonTurns}");
        hero.OnHeal += (amount) => Print($"{hero.Name} восстанавливает {amount} здоровья. Осталось здоровья: {hero.CurrentHP}");
        hero.OnHealBase += (current) => Print($"{hero.Name} восстановил здоровье. HP: {current}/{hero.MaxHP}");
    }

    private void Print(string message, int delay = 1000)
    {
        Console.WriteLine(message);
        Thread.Sleep(delay);
    }
}