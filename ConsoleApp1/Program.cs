using System;
using System.Threading;

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