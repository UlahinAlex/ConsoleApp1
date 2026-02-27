using System;
using System.Collections.Generic;
using System.Text;

public class Player : Character
{
    public int PoisonTurns = 0; //Счетчик статуса отравления
    public bool InStunned = false; //Статус оглушения
    public int BonusBlockChance = 0; //Дополнительный шанс поставить блок
    public int BonusDodgeChance = 0; //Дополнительный шанс увернутся
    public int Level = 1; //Счетчик уровней
    public int Experience = 0; //Счетчик опыта
    private static int[] levelThresholds = { 0, 10, 20, 30, 50, 80, 130, 210, 340, 550 }; // Пороги опыта по Фибоначчи * 10
    public int HealTurns = 0; //Переодическое лечение
    public int HealAmount = 0; //На сколько лечит

    public Action OnBowVsSkeleton;
    public Action<int> OnExpGained;
    public Action<int, int, int> OnLevelUp; // уровень, новое HP, новый урон макс
    public Action<int> OnHeal;
    public Action<int> OnPoisonDamage;
    public Action<int> OnHealBase;
    public Action OnWeaponEquipError;

    public Player(string name, int hp, int minDamage, int maxDamage, int missChance, int critChance, int maxHP, Random random)
    : base(name, hp, minDamage, maxDamage, missChance, critChance, maxHP, random)
    {
    }

    public new int GetDamage(bool vsSkeleton = false)
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
        if (vsSkeleton && EquippedWeapon != null && EquippedWeapon.IsBow)
        {
            damage = (int)(damage * GameConstants.BOW_VS_SKELETON_MODIFIER);
            OnBowVsSkeleton?.Invoke();
        }
        return damage;
    }

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
}
