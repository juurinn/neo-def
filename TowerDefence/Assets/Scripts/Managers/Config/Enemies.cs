public class Enemies {

    // WIP
    public static float SPRINT_MULTIPLIER = 3f;
    public static float SPRINT_DURATION = 1f;
    public static float HEAL_PERCENTAGE = 0.05f;

    public static EnemyBlueprint Normal = new EnemyBlueprint() {
        hp = 200,
        speed = 1.75f,
        reward = 30,
        weight = 50,
        index = 0,
    };

    public static EnemyBlueprint Slow = new EnemyBlueprint() {
        hp = 350,
        speed = 1.25f,
        reward = 40,
        weight = 50,
        index = 1,
    };

    public static EnemyBlueprint Fast = new EnemyBlueprint() {
        hp = 100,
        speed = 3f,
        reward = 25,
        weight = 45,
        index = 2,
    };

    public static EnemyBlueprint Healer = new EnemyBlueprint() {
        hp = 125,
        speed = 1.45f,
        reward = 50,
        weight = 50,
        index = 3,
        canHeal = true,
    };

    public static EnemyBlueprint Sprinter = new EnemyBlueprint() {
        hp = 200,
        speed = 1.1f,
        reward = 75,
        weight = 55,
        index = 4,
        canSprint = true,
    };

    public static EnemyBlueprint Unslowable = new EnemyBlueprint() {
        hp = 250,
        speed = 1.4f,
        reward = 125,
        weight = 75,
        index = 5,
        isImmuneToSlow = true,
    };

    public static EnemyBlueprint SuperFast = new EnemyBlueprint() {
        hp = 20,
        speed = 5f,
        reward = 20,
        weight = 10,
        index = 6,
        canHeal = true,
        isImmuneToSlow = true,
    };

    public static EnemyBlueprint NormalBoss = new EnemyBlueprint() {
        hp = 1000,
        speed = 1f,
        reward = 500,
        index = 7,
    };

    public static EnemyBlueprint FastBoss = new EnemyBlueprint() {
        hp = 500,
        speed = 2.25f,
        reward = 500,
        index = 8,
    };

    public static EnemyBlueprint HealerBoss = new EnemyBlueprint() {
        hp = 400,
        speed = 1.75f,
        reward = 1000,
        index = 9,
        canHeal = true,
    };

    public static EnemyBlueprint UnslowableBoss = new EnemyBlueprint() {
        hp = 1000,
        speed = 1.5f,
        reward = 1000,
        weight = 75,
        index = 10,
        isImmuneToSlow = true,
    };

    public static EnemyBlueprint SprinterBoss = new EnemyBlueprint() {
        hp = 900,
        speed = 1.5f,
        reward = 2500,
        weight = 400,
        index = 11,
        canSprint = true,
    };

    public static EnemyBlueprint Pekka = new EnemyBlueprint() {
        hp = 2150,
        speed = 1.5f,
        reward = 69, // ++vacation with pekka
        index = 12,
        canSprint = true,
        isImmuneToSlow = true,
        canHeal = true,
    };


    /// <summary>
    /// All enemies ordered by ascending enemy codes.
    /// </summary>
    public static EnemyBlueprint[] all = new EnemyBlueprint[] {
        //              Index Wave
        Normal,         // 0  0
        Slow,           // 1  7
        Fast,           // 2  14
        Healer,         // 3  21
        Sprinter,       // 4  28
        Unslowable,     // 5  35
        SuperFast,      // 6  42
        NormalBoss,     // 7
        FastBoss,       // 8
        HealerBoss,     // 9
        UnslowableBoss, // 10
        SprinterBoss,   // 11
        Pekka           // 12
    };
}