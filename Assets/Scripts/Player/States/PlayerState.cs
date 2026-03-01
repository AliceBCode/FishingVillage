using UnityEngine;

namespace FishingVillage.Player
{
    public enum PlayerState
    {
        [Tooltip("A player state that allows the player to move,jump,interact,open menus")]
        Normal = 0,
        [Tooltip("A player state that prevents the player from moving or jumping, but still allows interaction and menu access")]
        Constrained = 1,
        [Tooltip("A player state that prevents the player from doing anything")]
        Locked  = 2,
    }
}