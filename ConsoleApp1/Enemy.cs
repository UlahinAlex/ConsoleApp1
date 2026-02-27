using System;
using System.Collections.Generic;
using System.Text;

public class Enemy : Character
{
    public bool IsElite = false; //Усиленный враг

    public Enemy(string name, int hp, int minDamage, int maxDamage, int missChance, int critChance, int maxHP, Random random)
: base(name, hp, minDamage, maxDamage, missChance, critChance, maxHP, random)
    {
    }
}