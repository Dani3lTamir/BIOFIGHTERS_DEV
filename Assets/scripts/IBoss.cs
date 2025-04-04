using UnityEngine;

public interface IBoss
{
    float damagePerTickMultiplier { get; set; } // Multiplier for damage per tick
    bool getMovmentStatus();
    void EnableMovement();
    void DisableMovement();

}
