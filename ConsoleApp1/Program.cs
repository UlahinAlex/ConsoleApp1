using System;
using System.ComponentModel.Design;
using System.Threading;
public class Character
{
    //Data
    public string Name;
    public int CurrentHP;
    public int MaxDamage;
    public int MinDamage;
    public int PoisonTurns = 0;
    public bool InStunned = false;
    public int MissChance;
    public int CritChance;
    public int BonusBlockChance = 0;
    public int BonusDodgeChance = 0;

    public void ApplyEffects()
    {
        if (PoisonTurns > 0)
        {
            int poisonDamage = random.Next(1, 4);
            CurrentHP -= poisonDamage;
            PoisonTurns--;
            Console.WriteLine($"{Name} получает {poisonDamage} урона от яда. Осталось здоровья: {CurrentHP}. Ходов отравления: {PoisonTurns}");
            Thread.Sleep(1000);
        }
    }

    private static Random random = new Random();

    public int GetDamage(bool vsSkeletion = false)
    {
        if (random.Next(1, 101) <= MissChance)
        {
            Console.WriteLine($"{Name} промахивается!");
            Thread.Sleep(1000);
            return 0;
        }
        int damage = random.Next(MinDamage, MaxDamage + 1);

        if (random.Next(1,101) <= CritChance)
        {
            float critMult = EquippedWeapon != null ? EquippedWeapon.CritMultiplierl : 1.5f;
            damage = (int)(damage * critMult);
            Console.WriteLine($"{Name} наносит критический удар!");
            Thread.Sleep(1000);
        }

        if (vsSkeletion && EquippedWeapon != null && EquippedWeapon.IsBow)
        {
            damage = (int)(damage * 0.2f);
            Console.WriteLine($"Стрелы плохо пробивают кости! Урон снижен.");
            Thread.Sleep(1000);
        }

        return damage;        
    }
    public Character(string name, int hp, int minDamage, int maxDamage, int missChance, int critChance)
    {
        Name = name;
        CurrentHP = hp;
        MinDamage = minDamage;
        MaxDamage = maxDamage;
        MissChance = missChance;
        CritChance = critChance;
    }
    // Method
    public void Bark()
    {
        Console.WriteLine("Hello World");
    }

    public void TakeDamage(int damage)
    {
        CurrentHP -= damage;

        if (CurrentHP <= 0)
        {
            CurrentHP = 0;
            Console.WriteLine($"{Name} погиб!");
            Thread.Sleep(1000);
        }
        else
        {
            Console.WriteLine($"{Name} получил {damage} урона. Осталось здоровья: {CurrentHP}.");
            Thread.Sleep(1000);
        }
    }

    public Weapon EquippedWeapon;

    public void EquipWeapon(Weapon weapon)
    {
        EquippedWeapon = weapon;
        MinDamage = weapon.MinDamage;
        MaxDamage = weapon.MaxDamage;
        MissChance = weapon.DodgeModifier;
    }

    public bool isAlive ()
    {
        return CurrentHP > 0; 
    }
}

public class Weapon
{
    public string Name;
    public int MinDamage;
    public int MaxDamage;
    public float CritMultiplierl;
    public int DodgeModifier;
    public int DoubleAttackChance;
    public int BlockChance;
    public bool IsBow;

    public Weapon(string name, int minDamage, int maxDamage, float critMultiplierl, int dodgeModifier, int doubleAttackChance, int blockChance, bool isBow)
    {
        Name = name;
        MinDamage = minDamage;
        MaxDamage = maxDamage;
        CritMultiplierl = critMultiplierl;
        DodgeModifier = dodgeModifier;
        DoubleAttackChance = doubleAttackChance;
        BlockChance = blockChance;
        IsBow = isBow;
    }
}
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Добро пожаловать в симулятор боя!");
        Console.WriteLine("Как тебя зовут?");

        string name = "alex";

        name = Console.ReadLine();
    
        Character myHero = new Character(name, 100, 10, 15, 30, 5);

        Character goblin = new Character("Гоблин", 30, 3, 8, 15, 50);
        Character spider = new Character("Паук", 20, 1, 5, 10, 80); 
        Character skeleton = new Character("Скелет", 50, 1, 10, 30, 10);

        List<Character> enemies = new List<Character> { goblin, spider, skeleton };
        Random random = new Random();

        Console.WriteLine($">Меня зовут {myHero.Name}, у меня {myHero.CurrentHP} здоровья.");

        Weapon axe = new Weapon("Топор", 8, 16, 2.0f, 0, 15, 0, false);
        Weapon swordAndShield = new Weapon("Меч со щитом", 10, 15, 1.5f, -5, 0, 80, false);
        Weapon bow = new Weapon("Лук", 12, 20, 1.5f, 30, 0, 0, isBow: true);

        Console.WriteLine("\nВыбери оружие:");
        Console.WriteLine("1. Топор (урон 8-16, х2 крит, 15% двойная атака)");
        Console.WriteLine("2. Меч со щитом (урон 10-15, -5% уворот, 80% блок)");
        Console.WriteLine("3. Лук (урон 12-20, +30% уворот, -80% урон скелету)");

        Weapon chosen = null;
        while (chosen == null)
        {
            string input = Console.ReadLine();
            if (input == "1") chosen = axe;
            else if (input == "2") chosen = swordAndShield;
            else if (input == "3") chosen = bow;
            else Console.WriteLine("Введи 1, 2 или 3!");
        }

        myHero.EquipWeapon(chosen);
        Console.WriteLine($"Ты выбрал: {chosen.Name}!");
        Thread.Sleep(3000);

        while (myHero.isAlive() && enemies.Count > 0)
        {
            Character enemy = enemies[random.Next(enemies.Count)];

            Console.WriteLine($"\nНа тебя нападает {enemy.Name}");
            Thread.Sleep(1000);

            while (myHero.isAlive() && enemy.isAlive())
            {
                myHero.ApplyEffects();

                bool vsSkeleton = enemy == skeleton;

                if (!myHero.isAlive())
                {
                    Console.WriteLine($"{myHero.Name} погиб от яда!");
                    Thread.Sleep(1000);
                    break;
                }

                
                string action = "";

                while (true)
                {
                    Console.WriteLine("\nВведи команду (атака / блок / уворот):");
                    action = Console.ReadLine().ToLower().Trim();

                    if (action == "атака" || action == "блок" || action == "уворот")
                        break;
                    else
                        Console.WriteLine("Не могу этого сделать!");
                }
                if (action == "атака")
                {
                    enemy.TakeDamage(myHero.GetDamage(vsSkeleton));

                    if (myHero.EquippedWeapon != null && myHero.EquippedWeapon.DoubleAttackChance > 0)
                    {
                        if (random.Next(1, 101) <= myHero.EquippedWeapon.DoubleAttackChance)
                        {
                            Console.WriteLine("Двойная атака!");
                            Thread.Sleep(1000);
                            enemy.TakeDamage(myHero.GetDamage(vsSkeleton));
                        }
                    }
                }
                else if (action == "блок")
                {
                    int newBlockChance = (myHero.EquippedWeapon?.BlockChance ?? 0) + myHero.BonusBlockChance + 50;
                    myHero.BonusBlockChance = Math.Min(newBlockChance, 99) - (myHero.EquippedWeapon?.BlockChance ?? 0);
                    Console.WriteLine($"{myHero.Name} принимает защитную стойку! Шанс блока увеличен.");
                    Thread.Sleep(1000);
                }
                else if (action == "уворот")
                {
                    myHero.BonusDodgeChance = Math.Min(myHero.BonusDodgeChance + 50, 99);
                    Console.WriteLine($"{myHero.Name} готовится к уворту! Шанс уклонения увеличен.");
                    Thread.Sleep(1000);
                }

                //if (myHero.InStunned)
                //{
                //    Console.WriteLine($"{myHero.Name} оглушен и пропускает атаку!");
                //    Thread.Sleep(1000);
                //    myHero.InStunned = false;
                //}
                //else
                //{
                //    enemy.TakeDamage(myHero.GetDamage(vsSkeleton));

                //    if (myHero.EquippedWeapon != null && myHero.EquippedWeapon.DoubleAttackChance > 0)
                //    {
                //        if (random.Next(1, 101) <= myHero.EquippedWeapon.DoubleAttackChance)
                //        {
                //            Console.WriteLine("Двойная атака!");
                //            Thread.Sleep(1000);
                //            enemy.TakeDamage(myHero.GetDamage(vsSkeleton));
                //        }
                //    }
                //}

                if (!enemy.isAlive() )
                {
                    Console.WriteLine($"\n>Я победил {enemy.Name}!");
                    Thread.Sleep(3000);
                    enemies.Remove(enemy);
                    break;
                }

                int damage = enemy.GetDamage();

                int totalDodge = myHero.MissChance + myHero.BonusDodgeChance;
                if (myHero.BonusDodgeChance > 0 && random.Next(1, 101) <= totalDodge)
                {
                    Console.WriteLine($"{myHero.Name} уворачивается от атаки!");
                    Thread.Sleep(1000);
                    myHero.BonusDodgeChance = 0;
                    myHero.BonusBlockChance = 0;
                    continue;
                }
                myHero.BonusDodgeChance = 0;

                int totalBlock = (myHero.EquippedWeapon?.BlockChance ?? 0) + myHero.BonusBlockChance;
                if (totalBlock > 0 && random.Next(1, 101) <= totalBlock)
                {
                    Console.WriteLine($"{myHero.Name} блокирует атаку!");
                    Thread.Sleep(1000);
                    myHero.BonusBlockChance = 0;
                    if (enemy == skeleton)
                    {
                        skeleton.MinDamage++;
                        skeleton.MaxDamage++;
                        Console.WriteLine($"Скелет становится сильнее! Урон: {skeleton.MinDamage}-{skeleton.MaxDamage}");
                        Thread.Sleep(1000);
                    }
                    continue;
                }
                myHero.BonusBlockChance = 0;

                if (myHero.EquippedWeapon != null && myHero.EquippedWeapon.BlockChance > 0)
                {
                    if (random.Next(1, 101) <= myHero.EquippedWeapon.BlockChance)
                    {
                        Console.WriteLine($"{myHero.Name} блокирует атаку щитом!");
                        Thread.Sleep(1000);

                        if (enemy == skeleton)
                        {
                            skeleton.MinDamage++;
                            skeleton.MaxDamage++;
                            Console.WriteLine($"Скелет становится сильнее! Урон: {skeleton.MinDamage}-{skeleton.MaxDamage}");
                            Thread.Sleep(1000);
                        }
                        continue;
                    }
                }

                myHero.TakeDamage(damage);

                if (enemy == spider)
                {
                    myHero.PoisonTurns = 5;
                    Console.WriteLine($"{myHero.Name} отравлен!");
                    Thread.Sleep(1000);
                }
                else if (enemy == goblin)
                {
                    if (random.Next(1, 101) <= 30)
                    {
                        myHero.InStunned = true;
                        Console.WriteLine($"{enemy.Name} оглушил {myHero.Name}!");
                        Thread.Sleep(1000);
                    }
                }
                else if (enemy == skeleton)
                {
                    skeleton.MinDamage++;
                    skeleton.MaxDamage++;
                    Console.WriteLine($"Скелет становится сильнее! Урон: {skeleton.MinDamage}-{skeleton.MaxDamage}");
                    Thread.Sleep(1000);
                }
            }
        }

        if (myHero.isAlive())
        {
            Console.WriteLine("\n>Я победил всех врагов!");
        }
                //Console.WriteLine("\nВведите урон:");
                //int damage = int.Parse(Console.ReadLine());
               //int damage = 0;
                //string preDamage = Console.ReadLine();
                //if (preDamage != "")
                //{
                    //damage = int.Parse(preDamage);
                //}
                
                //myHero.TakeDamage(damage);
            //}
    }
}