using UnityEngine;

/// <summary>
/// Initialise and store blueprints for turrets and upgrade tiers for them.
/// </summary>
public class TurretConfig : MonoBehaviour {

    // Turrets consist of base stat blueprint + 4 upgrade tier blueprints
    public static TurretBlueprint[] basic = new TurretBlueprint[7];
    public static TurretBlueprint[] laser = new TurretBlueprint[7];
    public static TurretBlueprint[] bleed = new TurretBlueprint[7];
    public static TurretBlueprint[] cryo = new TurretBlueprint[7];
    public static TurretBlueprint[] pulse = new TurretBlueprint[7];
    public static TurretBlueprint[] javelin = new TurretBlueprint[7];

    /// <summary>
    /// Assign hardcoded tower values to blueprints. 
    /// </summary>
    public static void Initialise() {

        //  ---- BASIC TOWER ----
        // Base Tower | DPS: 67
        // Total cost: 800
        basic[0].name = "Basic Tower";
        basic[0].type = TowerType.Basic;
        basic[0].cost = 800;
        basic[0].dmg = 50f;
        basic[0].range = 3.5f;
        basic[0].fireRate = 1.3f;
        basic[0].description = "Gets the job done, good for picking out low health targets.";

        // Left Side | Shoots fast, lower damage
        // DPS: 100
        // Total cost: 1200
        basic[1].name = "Fast Draw";
        basic[1].cost = 400;
        basic[1].fireRate = 2f;
        basic[1].description = "Shoot first, ask questions later. Shoots almost double as fast as the previous one.";
        // Left side 2
        // DPS: 300
        // Total cost: 2700
        basic[3].cost = 1500;
        basic[3].dmg = 125f;
        basic[3].description = "Bigger guns? Yay! Still keeps the fast fire rate but now hits even harder.";
        // Left side 3
        // DPS: 900
        // Total cost: 9200
        basic[5].cost = 6500;
        basic[5].dmg = 300f;
        basic[5].fireRate = 3f;
        basic[5].range = 4f;
        basic[5].description = "More speed! AND DAMAGE? Nothing can stop this guy from decimating circles from the map.";

        // Right Side -- Slow shooter, heavy hits
        // DPS: 150
        // Total cost: 2000
        basic[2].name = "Radiance";
        basic[2].cost = 1200;
        basic[2].dmg = 200f;
        basic[2].fireRate = 0.75f;
        basic[2].range = 4.5f;
        basic[2].description = "This tower actually packs a punch. Really. Just doesn't do it as often.";
        // Right Side 2
        // DPS: 225
        // Total cost: 4800
        basic[4].cost = 2800;
        basic[4].dmg = 450f;
        basic[4].description = "Made the tower punchier while keeping it slow.";
        // Right Side 3
        // DPS: 1125
        // Total cost: 12500
        basic[6].cost = 7700;
        basic[6].dmg = 1500f;
        basic[6].description = "Hits like a truck... a very small truck.";

        // ---- LASER TOWER ----
        // BASE TOWER DPS: 300
        // Total cost: 2500
        laser[0].name = "Laser Tower";
        laser[0].type = TowerType.Laser;
        laser[0].cost = 2500;
        laser[0].dmg = 175f;
        laser[0].range = 2.5f;
        laser[0].fireRate = 2f;
        laser[0].laser = new LaserProperties { enabled = true, penetration = 1 };
        laser[0].description = "Shoots laser beams. High damage output.";
        // LEFT SIDE 1
        // DPS: 500 max
        // Total cost: 4700
        laser[1].name = "Penetrator"; // me + your mom
        laser[1].cost = 2200;
        laser[1].fireRate = 0.5f;
        laser[1].dmg = 250f;
        laser[1].laser = new LaserProperties { enabled = true, penetration = 5 };
        laser[1].description = "Yeah funny name. Heh heh. Get over it. Shoots a laser beam that goes through targets in a straight line.";
        // LEFT SIDE 2
        // DPS: 1000
        // Total cost: 9700
        laser[3].cost = 5000;
        laser[3].dmg = 350f;
        laser[3].laser = new LaserProperties { enabled = true, penetration = 10 };
        laser[3].description = "Penetrating enemies felt good? Lets do it more!";
        // LEFT SIDE 3
        // DPS: 15750
        // Total cost: 45700
        laser[5].cost = 36000;
        laser[5].dmg = 3500;
        laser[5].range = 3.5f;
        laser[5].description = "Finally we can penetrate everything as long as they come in a straight line.";
        laser[5].laser = new LaserProperties { enabled = true, penetration = 90 };
        // RIGHT Side 1
        // DPS: 375
        // Total cost: 4300
        laser[2].name = "Inferno";
        laser[2].cost = 1800;
        laser[2].range = 2f;
        laser[2].dmg = 250f;
        laser[2].fireRate = 1f;
        laser[2].laser = new LaserProperties { beamEnabled = true, beamMultiplier = 0.25f };
        laser[2].description = "This is what you want for those single high health enemies. Latches on to a single target as long as the target stays in range or until it dies. They usually die.";
        // RIGHT Side 2
        // DPS: 450
        // Total cost: 8800
        laser[4].cost = 4500;
        laser[4].dmg = 350f;
        laser[4].range = 3.5f;
        laser[4].fireRate = 1.5f;
        laser[4].laser = new LaserProperties { beamEnabled = true, beamMultiplier = 0.1f };
        laser[4].description = "Wearing eye protection is highly recommended while handling high power lasers.";
        // RIGHT Side 3
        // DPS: 3000
        // Total cost: 16300
        laser[6].cost = 7500;
        laser[6].dmg = 450f;
        laser[6].range = 4.5f;
        laser[6].fireRate = 2f;
        laser[6].laser = new LaserProperties { beamEnabled = true, beamMultiplier = 0.3f };
        laser[6].description = "Melts your worries away.";


        // ---- BLEED TOWER ----
        // Base Tower
        // DPS: 112.5
        // Total cost: 1000
        bleed[0].name = "Bleed Tower";
        bleed[0].type = TowerType.Bleed;
        bleed[0].cost = 1000;
        bleed[0].dmg = 0f;
        bleed[0].range = 3f;
        bleed[0].fireRate = 1f;
        bleed[0].dot = new DotProperties { enabled = true, damage = 30f, amount = 5, frequency = 0.5f };
        bleed[0].description = "Inflicts damage over time effect to enemies. Good for weakening strong enemies.";
        // Left Side 1 -- Melee
        // DPS: 75
        // Total cost: 4000
        bleed[1].name = "Shuriken Storm";
        bleed[1].cost = 3000;
        bleed[1].dmg = 100f;
        bleed[1].range = 1.6f;
        bleed[1].fireRate = 0.4f;
        bleed[1].meleeDmg = true;
        bleed[1].dot = new DotProperties { enabled = true, damage = 25f, amount = 5, frequency = 1f };
        bleed[1].description = "Shurikens? SHURIKENS. Spins shurikens around the tower that deal damage on hit and leaves a wound that bleeds over time.";
        // Left Side 2
        // DPS: 165
        // Total cost: 9500
        bleed[3].cost = 5500;
        bleed[3].dmg = 200f;
        bleed[3].dot = new DotProperties { enabled = true, damage = 30f, amount = 8, frequency = 0.75f };
        bleed[3].description = "Sharper shurikens that deal even more damage on hit and leaves deeper wounds. Range stays the same unfortunately.";
        // Left  Side 3
        // DPS: 250 + 150 tick dmg between fire | Total bleed 750 over 5s
        // Total cost: 20000
        bleed[5].cost = 10500;
        bleed[5].dmg = 500f;
        bleed[5].fireRate = 0.5f;
        bleed[5].dot = new DotProperties { enabled = true, damage = 75f, amount = 10, frequency = 0.5f };
        bleed[5].description = "Shuriken technology upgrades keep making this towers bleed stronger and stronger.";

        // Right side 1 -- Ranged projectile, single target
        // DPS: 225
        // Total cost: 1800
        bleed[2].name = "Rupture";
        bleed[2].cost = 800;
        bleed[2].range = 4f;
        bleed[2].dot = new DotProperties { enabled = true, damage = 30f, amount = 10, frequency = 0.75f };
        bleed[2].description = "Tri-edged bullet that causes the target to start bleeding.";
        // Right Side 2
        // DPS: 375
        // Total cost: 3300
        bleed[4].cost = 1500;
        bleed[4].dot = new DotProperties { enabled = true, damage = 50f, amount = 10, frequency = 0.6f };
        bleed[4].description = "We made the bullets even more edgy, causing more bleeds!";
        // Right Side 3
        // DPS: 3000 dmg over 8s
        // Total cost: 11800
        bleed[6].cost = 8500;
        bleed[6].fireRate = 0.75f;
        bleed[6].dot = new DotProperties { enabled = true, damage = 150f, amount = 20, frequency = 0.4f };
        bleed[6].description = "<b>.</b>"; // Funny joke
        // Old: It's almost cruel how much bleed damage this causes

        // ---- SLOW TOWER ----
        // Base tower
        // Total cost: 1200
        // DPS: 22
        cryo[0].name = "Cryo Tower";
        cryo[0].type = TowerType.Cryo;
        cryo[0].cost = 1200;
        cryo[0].dmg = 50f;
        cryo[0].range = 3f;
        cryo[0].fireRate = 1.1f;
        cryo[0].slow = new SlowProperties { enabled = true, time = 1f, multiplier = 0.6f };
        cryo[0].description = "Shoots a sub zero bullet that causes anything hit go slower for a short time.";

        // Left side 1 -  slow beam
        // Total cost: 2200
        // DPS: 100
        cryo[1].name = "Frost Beam";
        cryo[1].cost = 1000;
        cryo[1].dmg = 100f;
        cryo[1].range = 2.2f;
        cryo[1].fireRate = 1f;
        cryo[1].slow = new SlowProperties { beamEnabled = true, multiplier = 0.45f };
        cryo[1].description = "Beam of cold. Good for keeping strong enemies moving slow. <color=#617CFE>[Can slow otherwise unstobbale enemies]</color>";
        // Left side 2
        // Total cost: 4500
        // DPS: 150
        cryo[3].cost = 2500;
        cryo[3].dmg = 200f;
        cryo[3].fireRate = 1.5f;
        cryo[3].slow = new SlowProperties { beamEnabled = true, multiplier = 0.35f };
        cryo[3].description = "Isn't the beam we are shooting just a giant icicle?";
        // Left side 3
        // Total cost: 8000
        // DPS: 200
        cryo[5].cost = 3500;
        cryo[5].dmg = 300f;
        cryo[5].fireRate = 2f;
        cryo[5].slow = new SlowProperties { beamEnabled = true, multiplier = 0.2f };
        cryo[5].description = "This is what you want for the strong boi bosses, a beam that turns enemies into popsicles!";

        // Right side 1
        // Total cost: 3700
        // DPS: 
        cryo[2].name = "Winter Is Coming";
        cryo[2].cost = 2500;
        cryo[2].dmg = 25f;
        cryo[2].range = 3.5f;
        cryo[2].fireRate = 0.75f;
        cryo[2].splashRadius = 1f;
        cryo[2].splashProjectile = true;
        cryo[2].slow = new SlowProperties { enabled = true, time = 1.5f, multiplier = 0.7f };
        cryo[2].description = "Freezing spreads to enemies in the close proximity of the enemy hit making them all slowed.";
        // Right Side 2
        // Total cost: 9700
        // DPS: 
        cryo[4].cost = 6000;
        cryo[4].slow = new SlowProperties { enabled = true, time = 2.5f, multiplier = 0.5f };
        cryo[4].description = "Freezes enemies almost completely leaving them crippled for a long duration.";
        // Right Side 3
        // Total cost: 19700
        // DPS: 
        cryo[6].cost = 10000;
        cryo[6].fireRate = 0.5f;
        cryo[6].splashRadius = 1.25f;
        cryo[6].slow = new SlowProperties { enabled = true, time = 3.5f, multiplier = 0.3f };
        cryo[6].description = "Makes Amber Heard seem like a warm person next to this."; // <- lol


        // ---- Pulse bomb TOWER ----
        // Base Tower
        // Total cost: 2000
        // DPS: 250
        pulse[0].name = "Pulse Bomb Tower";
        pulse[0].type = TowerType.Pulse;
        pulse[0].cost = 2000;
        pulse[0].dmg = 90f;
        pulse[0].range = 2.5f;
        pulse[0].fireRate = 0.5f;
        pulse[0].splashProjectile = true;
        pulse[0].splashRadius = 1.3f;
        pulse[0].description = "Shoots a charge that deals damage in an area around the impact.";

        // Left side 1 - Stuns
        // Total cost: 3000
        // DPS: 250
        pulse[1].name = "Quantum Lock";
        pulse[1].cost = 1000;
        pulse[1].dmg = 110f;
        pulse[1].splashRadius = 1f;
        pulse[1].slow = new SlowProperties { enabled = true, multiplier = 0, time = 0.4f };
        pulse[1].description = "Charges that make enemies think twice about moving on, almost as if they are <color=#CA50FE>stunned</color>.";
        // Left side 2
        // Total cost: 6000
        // DPS: 375
        pulse[3].cost = 3000;
        pulse[3].dmg = 125f;
        pulse[3].splashRadius = 1.25f;
        pulse[3].slow = new SlowProperties { enabled = true, multiplier = 0, time = 0.5f };
        pulse[3].description = "ZA WARUDO"; // Is that a motherfucking JoJo's reference?
        // Left Side 3
        // Total cost: 13500
        // DPS: 500
        pulse[5].cost = 7500;
        pulse[5].dmg = 200f;
        pulse[5].splashRadius = 1.5f;
        pulse[5].slow = new SlowProperties { enabled = true, multiplier = 0, time = 0.75f };
        pulse[5].description = "When this thing goes off it seems like time itself stops.";

        // Right Side 1 - AoE splash damage
        // Total cost: 5000
        // DPS: 600
        pulse[2].name = "Inverse Nuke";
        pulse[2].cost = 3000;
        pulse[2].dmg = 200f;
        pulse[2].splashRadius = 2f;
        pulse[2].fireRate = 0.4f;
        pulse[2].description = "Makes charges even stronger increasing damage and explosion radius.";
        // Right Side 2
        // Total cost: 12500
        // DPS: 937
        pulse[4].cost = 7500;
        pulse[4].dmg = 750f;
        pulse[4].fireRate = 0.25f;
        pulse[4].splashRadius = 2f;
        pulse[4].description = "More bang for your buck.";
        // Right Side 3
        // Total cost: 37500
        // DPS: About fifty
        pulse[6].cost = 25000;
        pulse[6].dmg = 2750f; // Needs to be at least 3 times the last upgrade in damage with the current price
        pulse[6].splashRadius = 2.25f;
        pulse[6].description = "Enemies that get hit by this get sent to a free trip to the Bahamas.";


        // ---- JAVELIN TOWER ----
        // Base Tower
        // Total cost: 5000
        // DPS: 500
        javelin[0].name = "Javelin Tower";
        javelin[0].type = TowerType.Javelin;
        javelin[0].cost = 5000;
        javelin[0].dmg = 500f;
        javelin[0].range = 7f;
        javelin[0].fireRate = 1f;
        javelin[0].description = "Discharges a massive bolt towards unsuspecting targets from faraway distance.";

        // Left 1 - Piercing
        // Total cost: 8000
        // DPS: 
        javelin[1].name = "Fragmentation";
        javelin[1].cost = 3000;
        javelin[1].fireRate = 0.5f;
        javelin[1].piercing = new PiercingProperties { enabled = true, chance = 0.6f };
        javelin[1].description = "Javelins now have a chance to pierce through enemies.";
        // Left 2
        // Total cost: 13500
        // DPS: 
        javelin[3].cost = 5500;
        javelin[3].dmg = 850f;
        javelin[3].piercing = new PiercingProperties { enabled = true, chance = 0.8f };
        javelin[3].description = "Sharper javelins can pierce even better!";
        // Left 3
        // Total cost: 27500
        // DPS: 
        javelin[5].cost = 14000;
        javelin[5].dmg = 2000f;
        javelin[5].piercing = new PiercingProperties { enabled = true, chance = 0.9f };
        javelin[5].description = "The days of praying for the RNG god have ended.";

        // Right 1 - Long Range, high DMG
        // Total cost: 8500
        // DPS: 750
        javelin[2].name = "The All Seeing Eye";
        javelin[2].cost = 3500;
        javelin[2].dmg = 1500f;
        javelin[2].range = 10;
        javelin[2].fireRate = 0.5f;
        javelin[2].description = "Javelin tower can see even further and deals more damage with reduced firerate.";
        // Right Side 2
        // Total cost: 14000
        // DPS: 1000
        javelin[4].cost = 5500;
        javelin[4].dmg = 2500f;
        javelin[4].range = 15f;
        javelin[4].fireRate = 0.4f;
        javelin[4].description = "We gave the tower a new pair of glasses so it can see even better and hit the targets more reliably.";
        // Right Side 3
        // Total cost: 25500
        // DPS: 5000
        javelin[6].cost = 11500;
        javelin[6].dmg = 20000f;
        javelin[6].range = 40f;
        javelin[6].fireRate = 0.25f;
        javelin[6].description = "Huge range and damage increase. Excels at picking off high health enemies from anywhere in the map.";
    }


    public static string TowerStatDesc(TurretBlueprint turret, TowerType type, bool left) =>
        type switch {
            TowerType.Bleed =>
                "Damage per tick: <color=#FA4647>[" + turret.dot.damage + "]</color> and ticks: <color=#FA4647>[" + turret.dot.amount + "]</color> times.",
            TowerType.Laser => left
                ? "Penetrates: <color=#FE7D2E>[" + turret.laser.penetration + "]</color> enemies."
                : "",
            TowerType.Cryo => left
                ? "Slows: <color=#617CFE>[" + (1 - turret.slow.multiplier) * 100 + "%]</color>"
                : "Slows: <color=#617CFE>[" + (1 - turret.slow.multiplier) * 100 + "%]</color> for <color=#617CFE>[" + turret.slow.time + "s]</color>",
            TowerType.Pulse => left
                ? "Stuns for: <color=#CA50FE>[" + turret.slow.time + "s]</color>. Splash radius: <color=#CA50FE>[" + turret.splashRadius + "]</color>"
                : "Splash radius: <color=#CA50FE>[" + turret.splashRadius + "]</color>",
            TowerType.Javelin => left
                ? "Chance to pierce: <color=#EFC821>[" + turret.piercing.chance * 100 + "%]</color>"
                : "",
            _ => ""
        };


    /// <summary>
    /// Get base or upgrade stats for tower.
    /// </summary>
    /// <param name="turret">Turret index.</param>
    /// <param name="tier">Optional, upgrade tier.</param>
    /// <returns>Bluerprint of turret.</returns>
    public static TurretBlueprint Get(int turret, int tier = 0) {
        // 4 is max tier assuming every tower has 2 tiers plus choice
        if (tier > 6) {
            Debug.LogWarning("[Config]: Invalid tier, please contact customer service");
            return basic[0];
        }

        switch (turret) {
            case 0:
                return basic[tier];
            case 1:
                return laser[tier];
            case 2:
                return cryo[tier];
            case 3:
                return bleed[tier];
            case 4:
                return pulse[tier];
            case 5:
                return javelin[tier];
            default:
                Debug.LogWarning("[Config]: Invalid turret index, please contact customer service");
                return basic[0];
        }
    }
}