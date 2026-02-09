using DNExtensions.Utilities;
using DNExtensions.Utilities.AutoGet;
using FishingVillage.Interactable;
using FishingVillage.Player;
using FishingVillage.Rope;
using FishingVillage.UI;
using UnityEngine;

namespace FishingVillage.Gameplay
{
    public interface IConstrainablePath
    {
        float GetClosestT(Vector3 position);
        Vector3 GetPositionAt(float t);
        void Release();
    }
}