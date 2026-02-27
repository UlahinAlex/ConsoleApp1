using System;
using System.Threading;

//Класс персонажей
public class Character
{
    //Глобальные переменные персонажей
    public string Name; //Имя
    public int CurrentHP; //Текущее значение ХП
    public int MaxHP; //Максимальное значение ХП
    public int MaxDamage; //Максимальный урон персонажа
    public int MinDamage; //Минимальный урон персонажа
    public int PoisonTurns = 0; //Счетчик статуса отравления
    public bool InStunned = false; //Статус оглушения
    public int MissChance; //Шанс увернуться
    public int CritChance; //Шанс нанести критический урон
    public int BonusBlockChance = 0; //Дополнительный шанс поставить блок
    public int BonusDodgeChance = 0; //Дополнительный шанс увернутся
    public Weapon EquippedWeapon; //Экипированное оружие
    public int Level = 1; //Счетчик уровней
    public int Experience = 0; //Счетчик опыта
    private static int[] levelThresholds = { 0, 10, 20, 30, 50, 80, 130, 210, 340, 550 }; // Пороги опыта по Фибоначчи * 10
    public bool IsElite = false; //Усиленный враг
    public int HealTurns = 0; //Переодическое лечение
    public int HealAmount = 0; //На сколько лечит

    //действия
    public Action<int> OnDamageTaken;
    public Action OnDeath;
    public Action OnMiss;
    public Action OnCrit;
    public Action OnBowVsSkeleton;
    public Action<int> OnExpGained;
    public Action<int, int, int> OnLevelUp; // уровень, новое HP, новый урон макс
    public Action<int> OnHeal;
    public Action<int> OnPoisonDamage;
    public Action<int> OnHealBase;
    public Action OnWeaponEquipError;



    public void GainExperience(int amount)
    {
        Experience += amount;
        OnExpGained?.Invoke(amount);
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        if (Level >= levelThresholds.Length) return;  // уже максимальный уровень

        if (Experience >= levelThresholds[Level])
        {
            Level++;
            MaxHP += 20;
            MinDamage = (int)(MinDamage * 1.1f);
            MaxDamage = (int)(MaxDamage * 1.1f);
            OnLevelUp?.Invoke(Level, MaxHP, MaxDamage);
            CheckLevelUp(); // Проверяем снова — вдруг опыта хватает на несколько уровней сразу
        }
    }

    //Метод проверки статусов
    public void ApplyEffects()
    {

        //Статус отравления
        if (PoisonTurns > 0)
        {
            int poisonDamage = random.Next(1, 4);
            CurrentHP -= poisonDamage;
            PoisonTurns--;
            OnPoisonDamage?.Invoke(poisonDamage);
        }

        if (HealTurns > 0)
        {
            CurrentHP = Math.Min(CurrentHP + HealAmount, MaxHP);
            HealTurns--;
            OnHeal?.Invoke(HealAmount);
        }
    }

    //Объявляем локальный рандом
    private static Random random = new Random();

    //Метод проверки эффектов атаки
    public int GetDamage(bool vsSkeleton = false)
    {

        //Шанс промахнутся
        if (random.Next(1, 101) <= MissChance)
        {
            OnMiss?.Invoke();
            return 0;
        }

        int damage = random.Next(MinDamage, MaxDamage + 1);

        //Шанс нанести крит
        if (random.Next(1,101) <= CritChance)
        {
            float critMult = EquippedWeapon != null ? EquippedWeapon.CritMultiplier : 1.5f;
            damage = (int)(damage * critMult);
            OnCrit?.Invoke();
        }

        //Снижение урона луком по скелету
        if (vsSkeleton && EquippedWeapon != null && EquippedWeapon.IsBow)
        {
            damage = (int)(damage * 0.2f);
            OnBowVsSkeleton?.Invoke();
        }

        return damage;        
    }

    //Конструкт персонажа
    public Character(string name, int hp, int minDamage, int maxDamage, int missChance, int critChance, int maxHP) //Имя, текущее ХП, мин. урон, макс. урон, промах, крит, макс. ХП
    {
        Name = name;
        CurrentHP = hp;
        MinDamage = minDamage;
        MaxDamage = maxDamage;
        MissChance = missChance;
        CritChance = critChance;
        MaxHP = hp;
    }

    //Метод получения урона персонажем
    public void TakeDamage(int damage)
    {
        if (damage <= 0) return; // игнорируем нулевой урон

        CurrentHP -= damage;

        if (CurrentHP <= 0)
        {
            CurrentHP = 0;
            OnDeath?.Invoke();
        }
        else
        {
            OnDamageTaken?.Invoke(damage);
        }
    }

    //Метод лечения
    public void HealBase()
    {
        int missing = MaxHP - CurrentHP;
        CurrentHP += missing / 2;
        OnHealBase?.Invoke(CurrentHP);
    }

    //Метод экипировки оружия
    public void EquipWeapon(Weapon weapon)
    {
        if (weapon == null)
        {
            OnWeaponEquipError?.Invoke();
            return;
        }
        EquippedWeapon = weapon;
        MinDamage = weapon.MinDamage;
        MaxDamage = weapon.MaxDamage;
        MissChance -= weapon.DodgeModifier;
    }

    //Проверка живой ли персонаж
    public bool isAlive ()
    {
        return CurrentHP > 0; 
    }
}

//Класс оружия
public class Weapon
{
    public string Name;
    public int MinDamage;
    public int MaxDamage;
    public float CritMultiplier;
    public int DodgeModifier;
    public int DoubleAttackChance;
    public int BlockChance;
    public bool IsBow;

    public Weapon(string name, int minDamage, int maxDamage, float critMultiplier, int dodgeModifier, int doubleAttackChance, int blockChance, bool isBow)
    {
        Name = name;
        MinDamage = minDamage;
        MaxDamage = maxDamage;
        CritMultiplier = critMultiplier;
        DodgeModifier = dodgeModifier;
        DoubleAttackChance = doubleAttackChance;
        BlockChance = blockChance;
        IsBow = isBow;
    }
}

//Основной класс выполнения циклов игшры
class Program
{
    //Типовые сообщения
    static void Print(string message, int delay = 1000)
    {
        Console.WriteLine(message);
        Thread.Sleep(delay);
    }
    //Отдельно выбираем противника
    static Character CreateRandomEnemy(Random random, int heroLevel)
    {
        // Определяем тип врага
        int roll = random.Next(1, 4);
        Character enemy;

        switch (roll)
        {
            case 1:
                enemy = new Character("Гоблин", 30, 3, 8, 15, 10, 30);
                break;
            case 2:
                enemy = new Character("Паук", 20, 1, 5, 5, 25, 20);
                break;
            default:
                enemy = new Character("Скелет", 50, 1, 10, 20, 15, 50);
                break;
        }

        // Проверяем элитность
        if (random.Next(1, 101) <= heroLevel * 10)
        {
            enemy.IsElite = true;
            enemy.MaxHP *= 2;
            enemy.CurrentHP = enemy.MaxHP;
            enemy.MinDamage *= 2;
            enemy.MaxDamage *= 2;
            enemy.CritChance *= 2;
            enemy.MissChance = Math.Max(enemy.MissChance / 2, 1); // элита реже промахивается
        }

        string eliteTag = enemy.IsElite ? " [ЭЛИТА]" : ""; //Пердупреждаем об элите
        Print($"Навстречу тебе выходит {enemy.Name}{eliteTag} [HP: {enemy.MaxHP}]!");

        enemy.OnDamageTaken += (damage) =>
        {
            Print($"{enemy.Name} получил {damage} урона. Осталось здоровья: {enemy.CurrentHP}");
        };

        enemy.OnDeath += () =>
        {
            Print($"{enemy.Name} погиб!");
        };

        enemy.OnMiss += () =>
        {
            Print($"{enemy.Name} промахивается!");
        };

        enemy.OnCrit += () =>
        {
            Print($"{enemy.Name} наносит критический удар!");
        };

        return enemy;
    }

    //Отдельно вынесли процесс выбора оружия
    static Weapon ChooseWeapon(Weapon axe, Weapon swordAndShield, Weapon bow)
    {
        //Выбираем оружие персонажа
        Console.WriteLine("\nВыбери оружие:");
        Console.WriteLine("1. Топор (урон 8-16, х2 крит, 15% двойная атака)");
        Console.WriteLine("2. Меч со щитом (урон 10-15, -5% уворот, 80% блок)");
        Console.WriteLine("3. Лук (урон 12-20, +30% уворот, -80% урон скелету)");
        Weapon chosen = null;
        while (chosen == null)
        {
            string input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    chosen = axe;
                    break;
                case "2":
                    chosen = swordAndShield;
                    break;
                case "3":
                    chosen = bow;
                    break;
                default:
                    Console.WriteLine("Введи 1, 2 или 3!");
                    break;
            }
        }
        return chosen;
    }

    static void HandleBase(Character hero, Weapon axe, Weapon swordAndShield, Weapon bow)
    {
        Console.WriteLine("\n=== БАЗА ===");
        hero.HealBase();

        Weapon chosen = ChooseWeapon(axe, swordAndShield, bow);
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

    static void HandleBattle(Character hero, Character enemy, Random random)
    {
        //определяем если скелета
        bool vsSkeleton = enemy.Name == "Скелет";

        //Срабатывает эффект
        hero.ApplyEffects();

        //Если тут герой погибает, то от яда.
        if (!hero.isAlive())
        {
            Print($"{hero.Name} погиб от яда!");
        }

        //Проверка на оглушение героя
        if (hero.InStunned)
        {
            Print($"{hero.Name} оглушен и пропускает атаку!");
            hero.InStunned = false;
        }
        else //Иначе продолжаем бой
        {
            string action = "";

            //выбор действия в бою
            while (true)
            {
                Console.WriteLine("\nВведи команду (атака / блок / уворот):");
                action = Console.ReadLine().ToLower().Trim();
                if (action == "атака" || action == "блок" || action == "уворот") break;
                else Console.WriteLine("Не могу этого сделать!");
            }

            //отработка действия
            switch (action)
            {
                case "атака":

                    enemy.TakeDamage(hero.GetDamage(vsSkeleton));

                    if (!enemy.isAlive()) return; //Не блъем мертвых

                    if (hero.EquippedWeapon != null && hero.EquippedWeapon.DoubleAttackChance > 0)
                    {
                        if (random.Next(1, 101) <= hero.EquippedWeapon.DoubleAttackChance)
                        {
                            Print("Двойная атака!");
                            enemy.TakeDamage(hero.GetDamage(vsSkeleton));
                            if (!enemy.isAlive()) return;
                        }
                    }
                    break;
                case "блок":
                    int newBlockChance = (hero.EquippedWeapon?.BlockChance ?? 0) + hero.BonusBlockChance + 50;
                    hero.BonusBlockChance = Math.Min(newBlockChance, 99) - (hero.EquippedWeapon?.BlockChance ?? 0);
                    Print($"{hero.Name} принимает защитную стойку!");
                    break;
                case "уворот":
                    hero.BonusDodgeChance = Math.Min(hero.BonusDodgeChance + 50, 99);
                    Print($"{hero.Name} готовится к уворту!");
                    break;
                default:
                    Console.WriteLine("Не могу этого сделать!");
                    break;
            }
        }

        // Атака врага
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
                    hero.PoisonTurns = 5;
                    Print($"{hero.Name} отравлен!");
                    break;
                case "Гоблин":
                    if (random.Next(1, 101) <= 30)
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

    static bool HandleVictory (Character hero, Character enemy, List<Character> enemies, Random random)
    {
        Console.WriteLine($"\n> Я победил {enemy.Name}!");
        Print($"{hero.Name} : {hero.CurrentHP}/{hero.MaxHP} здоровья.");

        enemies.Remove(enemy);

        // Расчитываем за победу
        int expReward = 0;
        switch (enemy.Name)
        {
            case "Гоблин":
                expReward = 5;
                break;
            case "Паук":
                expReward = 4;
                break;
            case "Скелет":
                expReward = 8;
                break;
            default:
                expReward = 0;
                break;
        }

        //Удваиваем опыт за элиту
        if (enemy.IsElite) expReward *= 2;
        //Выдаем опыт
        hero.GainExperience(expReward);

        // Выбор после победы
        while (true)
        {
            Console.WriteLine("\nВведи команду (На базу / Вперед):");
            string cmd = Console.ReadLine().ToLower().Trim();

            switch (cmd)
            {
                case "на базу":
                    
                    enemies.Clear(); // очищаем врагов — новые появятся после базы
                    return false;
                case "вперед":
                    //Встречаем следующего врага
                    enemies.Add(CreateRandomEnemy(random, hero.Level));
                    return true;
                default:
                    Console.WriteLine("Не могу этого сделать!");
                    break;
            }
        }
    }

    static void HandleDeath(Character hero)
    {
        Console.WriteLine($"\n{hero.Name} погиб. Игра окончена.");
    }

    //MAIN!!!
    static void Main(string[] args)
    {
        //Объявляем доступное оружие
        Weapon axe = new Weapon("Топор", 8, 16, 2.0f, 0, 15, 0, false);
        Weapon swordAndShield = new Weapon("Меч со щитом", 10, 15, 1.5f, -5, 0, 80, false);
        Weapon bow = new Weapon("Лук", 12, 20, 1.5f, 30, 0, 0, isBow: true);

        //Объявляем имеющиеся типы врагов
        List<Character> enemies = new List<Character>();

        //Обявляем рандом
        Random random = new Random();

        //Преветсвуем игрока
        Console.WriteLine("Добро пожаловать в симулятор боя!");

        //Создаем персонажа
        Console.WriteLine("Как тебя зовут?");
        string name = Console.ReadLine();
        Character myHero = new Character(name, 100, 10, 15, 30, 5, 100); //Имя, текущее ХП, мин. урон, макс. урон, промах, крит, макс. ХП
        Console.WriteLine($">Меня зовут {myHero.Name}, у меня {myHero.CurrentHP} здоровья.");

        //Блок с экшенами героя
        myHero.OnDamageTaken += (damage) =>
        {
            Print($"{myHero.Name} получил {damage} урона. Осталось здоровья: {myHero.CurrentHP}");
        };

        myHero.OnDeath += () =>
        {
            Print($"{myHero.Name} погиб!");
        };

        myHero.OnMiss += () =>
        {
            Print($"{myHero.Name} промахивается!");
        };

        myHero.OnCrit += () =>
        {
            Print($"{myHero.Name} наносит критический удар!");
        };

        myHero.OnBowVsSkeleton += () =>
        {
            Print("Стрелы плохо пробивают кости! Урон снижен.");
        };
        myHero.OnWeaponEquipError += () =>
        {
            Print("Ошибка: оружие не выбрано!");
        };

        //Блок с повышением уровня
        myHero.OnExpGained += (amount) =>
        {
            Print($"{myHero.Name} получает {amount} опыта. Всего: {myHero.Experience}");
        };

        myHero.OnLevelUp += (level, hp, maxDamage) =>
        {
            Console.WriteLine($"Уровень повышен! Теперь {myHero.Name} {level} уровня.");
            Print($"HP: {hp} | Урон: {myHero.MinDamage}-{maxDamage}");
        };

        //Блок эффектов
        myHero.OnPoisonDamage += (damage) =>
        {
            Print($"{myHero.Name} получает {damage} урона от яда. Осталось здоровья: {myHero.CurrentHP}. Ходов яда: {myHero.PoisonTurns}");
        };

        myHero.OnHeal += (amount) =>
        {
            Print($"{myHero.Name} восстанавливает {amount} здоровья. Осталось здоровья: {myHero.CurrentHP}");
        };

        myHero.OnHealBase += (current) =>
        {
            Print($"{myHero.Name} восстановил здоровье. HP: {current}/{myHero.MaxHP}");
        };

        //Заходим на базу
        bool onBase = true;

        while (myHero.isAlive())
        {
            // База
            if (onBase)
            {
                //Действия на базе
                HandleBase(myHero, axe, swordAndShield, bow);

                onBase = false; //Вышли с базы

                // Создаём первого врага при выходе с базы
                enemies.Add(CreateRandomEnemy(random, myHero.Level));
                continue; // возвращаемся в начало главного цикла
            }

            // Бой
            if (enemies.Count > 0)
            {
                //Локализуем противника
                Character enemy = enemies[0];

                //Запускаем бой
                HandleBattle(myHero, enemy, random);

                if (!enemy.isAlive())
                {
                    bool goForward = HandleVictory(myHero, enemy, enemies, random);
                    if (!goForward) onBase = true;
                }
                
            }
        }

        if (!myHero.isAlive())
        {
            HandleDeath(myHero);
        }
    }
}