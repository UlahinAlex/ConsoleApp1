using System;
using System.ComponentModel.Design;
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

    //Метод проверки статусов
    public void ApplyEffects()
    {

        //Статус отравления
        if (PoisonTurns > 0)
        {
            int poisonDamage = random.Next(1, 4);
            CurrentHP -= poisonDamage;
            PoisonTurns--;
            Console.WriteLine($"{Name} получает {poisonDamage} урона от яда. Осталось здоровья: {CurrentHP}. Ходов отравления: {PoisonTurns}");
            Thread.Sleep(1000);
        }
    }

    //Объявляем локальный рандом
    private static Random random = new Random();

    //Метод проверки эффектов атаки
    public int GetDamage(bool vsSkeletion = false)
    {

        //Шанс промахнутся
        if (random.Next(1, 101) <= MissChance)
        {
            Console.WriteLine($"{Name} промахивается!");
            Thread.Sleep(1000);
            return 0;
        }
        int damage = random.Next(MinDamage, MaxDamage + 1);

        //Шанс нанести крит
        if (random.Next(1,101) <= CritChance)
        {
            float critMult = EquippedWeapon != null ? EquippedWeapon.CritMultiplierl : 1.5f;
            damage = (int)(damage * critMult);
            Console.WriteLine($"{Name} наносит критический удар!");
            Thread.Sleep(1000);
        }

        //Снижение урона луком по скелету
        if (vsSkeletion && EquippedWeapon != null && EquippedWeapon.IsBow)
        {
            damage = (int)(damage * 0.2f);
            Console.WriteLine($"Стрелы плохо пробивают кости! Урон снижен.");
            Thread.Sleep(1000);
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

    //Метод лечения
    public void Heal()
    {
        CurrentHP = MaxHP;
        Console.WriteLine($"{Name} восстановил здоровье до {MaxHP}!");
        Thread.Sleep(1000);
    }

    //Метод экипировки оружия
    public void EquipWeapon(Weapon weapon)
    {
        if (weapon == null)
        {
            Console.WriteLine("Ошибка: оружие не выбрано!");
            return;
        }
        EquippedWeapon = weapon;
        MinDamage = weapon.MinDamage;
        MaxDamage = weapon.MaxDamage;
        MissChance = weapon.DodgeModifier;
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

//Основной класс выполнения циклов игшры
class Program
{
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
            if (input == "1") chosen = axe;
            else if (input == "2") chosen = swordAndShield;
            else if (input == "3") chosen = bow;
            else Console.WriteLine("Введи 1, 2 или 3!");
        }
        return chosen;
    }

    //MAIN!!!
    static void Main(string[] args)
    {
        //Объявляем доступное оружие
        Weapon axe = new Weapon("Топор", 8, 16, 2.0f, 0, 15, 0, false);
        Weapon swordAndShield = new Weapon("Меч со щитом", 10, 15, 1.5f, -5, 0, 80, false);
        Weapon bow = new Weapon("Лук", 12, 20, 1.5f, 30, 0, 0, isBow: true);

        //Объявляем имеющиеся типы врагов
        List<Character> enemies = new List<Character>
        {
            new Character("Гоблин", 30, 3, 8, 15, 10, 30), //Имя, текущее ХП, мин. урон, макс. урон, промах, крит, макс. ХП
            new Character("Паук", 20, 1, 5, 5, 25, 20), //Имя, текущее ХП, мин. урон, макс. урон, промах, крит, макс. ХП
            new Character("Скелет", 50, 1, 10, 20, 15, 50) //Имя, текущее ХП, мин. урон, макс. урон, промах, крит, макс. ХП
        };
        Character goblin = null;
        Character spider = null;
        Character skeleton = null;


        //Обявляем рандом
        Random random = new Random();

        //Преветсвуем игрока
        Console.WriteLine("Добро пожаловать в симулятор боя!");

        //Создаем персонажа
        Console.WriteLine("Как тебя зовут?");
        string name = "alex";
        name = Console.ReadLine();
        Character myHero = new Character(name, 100, 10, 15, 30, 5, 100); //Имя, текущее ХП, мин. урон, макс. урон, промах, крит, макс. ХП
        Console.WriteLine($">Меня зовут {myHero.Name}, у меня {myHero.CurrentHP} здоровья.");

        //Заходим на базу
        bool onBase = true;

        while (myHero.isAlive())
        {
            // База
            if (onBase)
            {
                Console.WriteLine("\n=== БАЗА ===");
                myHero.Heal();

                Weapon chosen = ChooseWeapon(axe, swordAndShield, bow);
                myHero.EquipWeapon(chosen);
                Console.WriteLine($"Ты выбрал: {chosen.Name}!");
                Thread.Sleep(1000);

                while (true)
                {
                    Console.WriteLine("\nВведи команду (В путь):");
                    string cmd = Console.ReadLine().ToLower().Trim();
                    if (cmd == "в путь") break;
                    else Console.WriteLine("Не могу этого сделать!");
                }

                onBase = false;

                // Создаём первого врага при выходе с базы
                int firstRoll = random.Next(1, 4);
                if (firstRoll == 1) enemies.Add(new Character("Гоблин", 30, 3, 8, 15, 10, 30));
                else if (firstRoll == 2) enemies.Add(new Character("Паук", 20, 1, 5, 5, 25, 20));
                else enemies.Add(new Character("Скелет", 50, 1, 10, 20, 15, 50));

                goblin = enemies.Find(e => e.Name == "Гоблин");
                spider = enemies.Find(e => e.Name == "Паук");
                skeleton = enemies.Find(e => e.Name == "Скелет");

                Console.WriteLine($"Навстречу тебе выходит {enemies[0].Name}!");
                Thread.Sleep(1000);

                continue; // возвращаемся в начало главного цикла
            }

            // Бой
            if (enemies.Count > 0)
            {
                Character enemy = enemies[0];
                bool vsSkeleton = enemy == skeleton;

                myHero.ApplyEffects();

                if (!myHero.isAlive())
                {
                    Console.WriteLine($"{myHero.Name} погиб от яда!");
                    Thread.Sleep(1000);
                    break;
                }

                if (myHero.InStunned)
                {
                    Console.WriteLine($"{myHero.Name} оглушен и пропускает атаку!");
                    Thread.Sleep(1000);
                    myHero.InStunned = false;
                }
                else
                {
                    string action = "";
                    while (true)
                    {
                        Console.WriteLine("\nВведи команду (атака / блок / уворот):");
                        action = Console.ReadLine().ToLower().Trim();
                        if (action == "атака" || action == "блок" || action == "уворот") break;
                        else Console.WriteLine("Не могу этого сделать!");
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
                        Console.WriteLine($"{myHero.Name} принимает защитную стойку!");
                        Thread.Sleep(1000);
                    }
                    else if (action == "уворот")
                    {
                        myHero.BonusDodgeChance = Math.Min(myHero.BonusDodgeChance + 50, 99);
                        Console.WriteLine($"{myHero.Name} готовится к уворту!");
                        Thread.Sleep(1000);
                    }
                }

                if (!enemy.isAlive())
                {
                    Console.WriteLine($"\n> Я победил {enemy.Name}!");
                    Thread.Sleep(1000);
                    enemies.Remove(enemy);

                    // Выбор после победы
                    while (true)
                    {
                        Console.WriteLine("\nВведи команду (На базу / Вперед):");
                        string cmd = Console.ReadLine().ToLower().Trim();

                        if (cmd == "на базу")
                        {
                            onBase = true;
                            enemies.Clear(); // очищаем врагов — новые появятся после базы
                            break;
                        }
                        else if (cmd == "вперед")
                        {
                            int roll = random.Next(1, 4);
                            if (roll == 1) enemies.Add(new Character("Гоблин", 30, 3, 8, 15, 10, 30));
                            else if (roll == 2) enemies.Add(new Character("Паук", 20, 1, 5, 5, 25, 20));
                            else enemies.Add(new Character("Скелет", 50, 1, 10, 20, 15, 50));

                            goblin = enemies.Find(e => e.Name == "Гоблин");
                            spider = enemies.Find(e => e.Name == "Паук");
                            skeleton = enemies.Find(e => e.Name == "Скелет");

                            Console.WriteLine($"Навстречу тебе выходит {enemies[0].Name}!");
                            Thread.Sleep(1000);
                            break;
                        }
                        else Console.WriteLine("Не могу этого сделать!");
                    }
                    continue;
                }

                // Атака врага
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

        if (!myHero.isAlive())
        {
            Console.WriteLine($"\n{myHero.Name} погиб. Игра окончена.");
        }

        //while(myHero.isAlive())
        //{
        //    //Действия на базе
        //    if (onBase)
        //    {
        //        //Восстанавливаем ХП на базе
        //        Console.WriteLine("\n=== БАЗА ===");
        //        Console.WriteLine($"Здоровье восстановлено. HP: {myHero.MaxHP}");
        //        myHero.Heal();

        //        //Выбираем и экипируем оружие
        //        Weapon chosen = ChooseWeapon(axe, swordAndShield, bow);
        //        myHero.EquipWeapon(chosen);
        //        Console.WriteLine($"Ты выбрал: {chosen.Name}!");
        //        Thread.Sleep(1000);

        //        //Ждем команду "В путь"
        //        while (true)
        //        {
        //            Console.WriteLine("\nВведи команду (В путь):");
        //            string cmd = Console.ReadLine().ToLower().Trim();
        //            if (cmd == "в путь") break;
        //            else Console.WriteLine("Не могу этого сделать!");
        //        }

        //        //Уходим с базы
        //        onBase = false;

        //        //Выбор после каждой битвы
        //        while (true)
        //        {
        //            Console.WriteLine("\nВведи команду (На базу / Вперед):");
        //            string cmd = Console.ReadLine().ToLower().Trim();

        //            if (cmd == "на базу")
        //            {
        //                onBase = true;
        //                break;
        //            }
        //            else if (cmd == "вперед")
        //            {
        //                //Добавляем нового случайного врага
        //                int roll = random.Next(1, 4);
        //                if (roll == 1) enemies.Add(new Character("Гоблин", 30, 3, 8, 15, 10, 30)); //Имя, текущее ХП, мин. урон, макс. урон, промах, крит, макс. ХП
        //                else if (roll == 2) enemies.Add(new Character("Паук", 20, 1, 5, 5, 25, 20)); //Имя, текущее ХП, мин. урон, макс. урон, промах, крит, макс. ХП
        //                else enemies.Add(new Character("Скелет", 50, 1, 10, 20, 15, 50)); //Имя, текущее ХП, мин. урон, макс. урон, промах, крит, макс. ХП


        //                //Обновляем ссылки
        //                goblin = enemies.Find(e => e.Name == "Гоблин");
        //                spider = enemies.Find(e => e.Name == "Паук");
        //                skeleton = enemies.Find(e => e.Name == "Скелет");

        //                Console.WriteLine($"Навстречу тебе выходит {enemies[0].Name}!");
        //                Thread.Sleep(1000);

        //                while (myHero.isAlive() && enemies[0].isAlive())
        //                {
        //                    myHero.ApplyEffects();

        //                    bool vsSkeleton = enemies[0] == skeleton;

        //                    if (!myHero.isAlive())
        //                    {
        //                        Console.WriteLine($"{myHero.Name} погиб от яда!");
        //                        Thread.Sleep(1000);
        //                        break;
        //                    }


        //                    string action = "";

        //                    while (true)
        //                    {
        //                        Console.WriteLine("\nВведи команду (атака / блок / уворот):");
        //                        action = Console.ReadLine().ToLower().Trim();

        //                        if (action == "атака" || action == "блок" || action == "уворот")
        //                            break;
        //                        else
        //                            Console.WriteLine("Не могу этого сделать!");
        //                    }
        //                    if (action == "атака")
        //                    {
        //                        enemies[0].TakeDamage(myHero.GetDamage(vsSkeleton));

        //                        if (myHero.EquippedWeapon != null && myHero.EquippedWeapon.DoubleAttackChance > 0)
        //                        {
        //                            if (random.Next(1, 101) <= myHero.EquippedWeapon.DoubleAttackChance)
        //                            {
        //                                Console.WriteLine("Двойная атака!");
        //                                Thread.Sleep(1000);
        //                                enemies[0].TakeDamage(myHero.GetDamage(vsSkeleton));
        //                            }
        //                        }
        //                    }
        //                    else if (action == "блок")
        //                    {
        //                        int newBlockChance = (myHero.EquippedWeapon?.BlockChance ?? 0) + myHero.BonusBlockChance + 50;
        //                        myHero.BonusBlockChance = Math.Min(newBlockChance, 99) - (myHero.EquippedWeapon?.BlockChance ?? 0);
        //                        Console.WriteLine($"{myHero.Name} принимает защитную стойку! Шанс блока увеличен.");
        //                        Thread.Sleep(1000);
        //                    }
        //                    else if (action == "уворот")
        //                    {
        //                        myHero.BonusDodgeChance = Math.Min(myHero.BonusDodgeChance + 50, 99);
        //                        Console.WriteLine($"{myHero.Name} готовится к уворту! Шанс уклонения увеличен.");
        //                        Thread.Sleep(1000);
        //                    }

        //                    if (!enemies[0].isAlive())
        //                    {
        //                        Console.WriteLine($"\n>Я победил {enemies[0].Name}!");
        //                        Thread.Sleep(3000);
        //                        enemies.Remove(enemies[0]);
        //                        break;
        //                    }

        //                    int damage = enemies[0].GetDamage();

        //                    int totalDodge = myHero.MissChance + myHero.BonusDodgeChance;
        //                    if (myHero.BonusDodgeChance > 0 && random.Next(1, 101) <= totalDodge)
        //                    {
        //                        Console.WriteLine($"{myHero.Name} уворачивается от атаки!");
        //                        Thread.Sleep(1000);
        //                        myHero.BonusDodgeChance = 0;
        //                        myHero.BonusBlockChance = 0;
        //                        continue;
        //                    }
        //                    myHero.BonusDodgeChance = 0;

        //                    int totalBlock = (myHero.EquippedWeapon?.BlockChance ?? 0) + myHero.BonusBlockChance;
        //                    if (totalBlock > 0 && random.Next(1, 101) <= totalBlock)
        //                    {
        //                        Console.WriteLine($"{myHero.Name} блокирует атаку!");
        //                        Thread.Sleep(1000);
        //                        myHero.BonusBlockChance = 0;
        //                        if (enemies[0] == skeleton)
        //                        {
        //                            skeleton.MinDamage++;
        //                            skeleton.MaxDamage++;
        //                            Console.WriteLine($"Скелет становится сильнее! Урон: {skeleton.MinDamage}-{skeleton.MaxDamage}");
        //                            Thread.Sleep(1000);
        //                        }
        //                        continue;
        //                    }
        //                    myHero.BonusBlockChance = 0;

        //                    if (myHero.EquippedWeapon != null && myHero.EquippedWeapon.BlockChance > 0)
        //                    {
        //                        if (random.Next(1, 101) <= myHero.EquippedWeapon.BlockChance)
        //                        {
        //                            Console.WriteLine($"{myHero.Name} блокирует атаку щитом!");
        //                            Thread.Sleep(1000);

        //                            if (enemies[0] == skeleton)
        //                            {
        //                                skeleton.MinDamage++;
        //                                skeleton.MaxDamage++;
        //                                Console.WriteLine($"Скелет становится сильнее! Урон: {skeleton.MinDamage}-{skeleton.MaxDamage}");
        //                                Thread.Sleep(1000);
        //                            }
        //                            continue;
        //                        }
        //                    }

        //                    myHero.TakeDamage(damage);

        //                    if (enemies[0] == spider)
        //                    {
        //                        myHero.PoisonTurns = 5;
        //                        Console.WriteLine($"{myHero.Name} отравлен!");
        //                        Thread.Sleep(1000);
        //                    }
        //                    else if (enemies[0] == goblin)
        //                    {
        //                        if (random.Next(1, 101) <= 30)
        //                        {
        //                            myHero.InStunned = true;
        //                            Console.WriteLine($"{enemies[0].Name} оглушил {myHero.Name}!");
        //                            Thread.Sleep(1000);
        //                        }
        //                    }
        //                    else if (enemies[0] == skeleton)
        //                    {
        //                        skeleton.MinDamage++;
        //                        skeleton.MaxDamage++;
        //                        Console.WriteLine($"Скелет становится сильнее! Урон: {skeleton.MinDamage}-{skeleton.MaxDamage}");
        //                        Thread.Sleep(1000);
        //                    }
        //                    break;
        //                }
        //            }
        //            else
        //            {
        //                Console.WriteLine("Не могу этого сделать!");
        //            }
        //        }
        //        break;

        //    }
        //}

        //while (myHero.isAlive() && enemies.Count > 0)
        //{
        //    Character enemy = enemies[random.Next(enemies.Count)];

        //    Console.WriteLine($"\nНа тебя нападает {enemy.Name}");
        //    Thread.Sleep(1000);


        //    }
        //}

        //if (myHero.isAlive())
        //{
        //    Console.WriteLine("\n>Я победил всех врагов!");
        //}
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