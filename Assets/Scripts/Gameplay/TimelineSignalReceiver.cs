
using UnityEngine;

namespace FishingVillage.Gameplay
{
    
    public class TimelineSignalReceiver : MonoBehaviour
    {
        [SerializeField] private string signalID;

        public void OnSignal()
        {
            GameEvents.TimelineSignalReceived(signalID);
        }
    }
}