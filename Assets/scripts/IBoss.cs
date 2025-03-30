using UnityEngine;

public interface IBoss
{
    float damagePerTick { get; set; } // Damage caused per tick
    float damagePerTickMultiplier { get; set; } // Multiplier for damage per tick

}
