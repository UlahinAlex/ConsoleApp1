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
            MaxHP += GameConstants.LEVEL_UP_HP_BONUS;
            MinDamage = (int)(MinDamage * GameConstants.LEVEL_UP_DAMAGE_MULTIPLIER);
            MaxDamage = (int)(MaxDamage * GameConstants.LEVEL_UP_DAMAGE_MULTIPLIER);
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
    private Random random;

    public Character(string name, int hp, int minDamage, int maxDamage, int missChance, int critChance, int maxHP, Random random)
    {
        Name = name;
        CurrentHP = hp;
        MinDamage = minDamage;
        MaxDamage = maxDamage;
        MissChance = missChance;
        CritChance = critChance;
        MaxHP = hp;
        this.random = random;
    }

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
            float critMult = EquippedWeapon != null ? EquippedWeapon.CritMultiplier : GameConstants.DEFAULT_CRIT_MULTIPLIER;
            damage = (int)(damage * critMult);
            OnCrit?.Invoke();
        }

        //Снижение урона луком по скелету
        if (vsSkeleton && EquippedWeapon != null && EquippedWeapon.IsBow)
        {
            damage = (int)(damage * GameConstants.BOW_VS_SKELETON_MODIFIER);
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
    static void Main(string[] args)
    {
        Game game = new Game();
        game.Run();
    }
}