using System;
using System.Threading;

public class Character
{
    //Глобальные переменные персонажей
    public string Name; //Имя
    public int CurrentHP; //Текущее значение ХП
    public int MaxHP; //Максимальное значение ХП
    public int MaxDamage; //Максимальный урон персонажа
    public int MinDamage; //Минимальный урон персонажа
    public int MissChance; //Шанс увернуться
    public int CritChance; //Шанс нанести критический урон
    public Weapon EquippedWeapon; //Экипированное оружие


    //действия
    public Action<int> OnDamageTaken;
    public Action OnDeath;
    public Action OnMiss;
    public Action OnCrit;

    //Объявляем наследуемый рандом
    protected Random random;

    //Конструкт персонажа
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
    public int GetDamage()
    {
        if (random.Next(1, 101) <= MissChance)
        {
            OnMiss?.Invoke();
            return 0;
        }
        int damage = random.Next(MinDamage, MaxDamage + 1);
        if (random.Next(1, 101) <= CritChance)
        {
            float critMult = EquippedWeapon != null ? EquippedWeapon.CritMultiplier : GameConstants.DEFAULT_CRIT_MULTIPLIER;
            damage = (int)(damage * critMult);
            OnCrit?.Invoke();
        }
        return damage;
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

    //Проверка живой ли персонаж
    public bool isAlive()
    {
        return CurrentHP > 0;
    }
}