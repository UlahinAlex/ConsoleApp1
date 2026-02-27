public static class GameConstants
{
    public const float DEFAULT_CRIT_MULTIPLIER = 1.5f;

    // Герой
    public const int HERO_START_HP = 100;
    public const int HERO_START_MIN_DAMAGE = 10;
    public const int HERO_START_MAX_DAMAGE = 15;
    public const int HERO_MISS_CHANCE = 30;
    public const int HERO_CRIT_CHANCE = 5;

    // Враги
    public const int GOBLIN_HP = 30;
    public const int GOBLIN_MIN_DAMAGE = 3;
    public const int GOBLIN_MAX_DAMAGE = 8;
    public const int GOBLIN_MISS_CHANCE = 15;
    public const int GOBLIN_CRIT_CHANCE = 10;
    public const int GOBLIN_STUN_CHANCE = 30;
    public const int GOBLIN_EXP = 5;

    public const int SPIDER_HP = 20;
    public const int SPIDER_MIN_DAMAGE = 1;
    public const int SPIDER_MAX_DAMAGE = 5;
    public const int SPIDER_MISS_CHANCE = 5;
    public const int SPIDER_CRIT_CHANCE = 25;
    public const int SPIDER_POISON_TURNS = 5;
    public const int SPIDER_EXP = 4;

    public const int SKELETON_HP = 50;
    public const int SKELETON_MIN_DAMAGE = 1;
    public const int SKELETON_MAX_DAMAGE = 10;
    public const int SKELETON_MISS_CHANCE = 20;
    public const int SKELETON_CRIT_CHANCE = 15;
    public const int SKELETON_EXP = 8;

    // Элита
    public const int ELITE_HP_MULTIPLIER = 2;
    public const int ELITE_DAMAGE_MULTIPLIER = 2;
    public const int ELITE_CRIT_MULTIPLIER = 2;
    public const int ELITE_EXP_MULTIPLIER = 2;

    // Механики боя
    public const int MAX_BLOCK_CHANCE = 99;
    public const int MAX_DODGE_CHANCE = 99;
    public const int BONUS_BLOCK_ON_DEFEND = 50;
    public const int BONUS_DODGE_ON_EVADE = 50;

    // Прокачка
    public const int LEVEL_UP_HP_BONUS = 20;
    public const float LEVEL_UP_DAMAGE_MULTIPLIER = 1.1f;

    // Константы оружия
    // Топор
    public const int AXE_MIN_DAMAGE = 8;
    public const int AXE_MAX_DAMAGE = 16;
    public const float AXE_CRIT_MULTIPLIER = 2.0f;
    public const int AXE_DODGE_MODIFIER = 0;
    public const int AXE_DOUBLE_ATTACK_CHANCE = 15;
    public const int AXE_BLOCK_CHANCE = 0;

    // Меч со щитом
    public const int SWORD_MIN_DAMAGE = 10;
    public const int SWORD_MAX_DAMAGE = 15;
    public const float SWORD_CRIT_MULTIPLIER = 1.5f;
    public const int SWORD_DODGE_MODIFIER = -5;
    public const int SWORD_DOUBLE_ATTACK_CHANCE = 0;
    public const int SWORD_BLOCK_CHANCE = 80;

    // Лук
    public const int BOW_MIN_DAMAGE = 12;
    public const int BOW_MAX_DAMAGE = 20;
    public const float BOW_CRIT_MULTIPLIER = 1.5f;
    public const int BOW_DODGE_MODIFIER = 30;
    public const int BOW_DOUBLE_ATTACK_CHANCE = 0;
    public const int BOW_BLOCK_CHANCE = 0;
    public const float BOW_VS_SKELETON_MODIFIER = 0.2f;
}