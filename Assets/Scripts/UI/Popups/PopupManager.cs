
using System.Collections;
using System.Collections.Generic;
using DNExtensions.Systems.ObjectPooling;
using FishingVillage.Gameplay;
using FishingVillage.Missions;
using FishingVillage.Missions.Objectives;
using UnityEngine;
using PrimeTween;

namespace FishingVillage.UI.Popup
{
    public class PopupManager : MonoBehaviour
    {
        public static PopupManager Instance { get; private set;}

        [Header("Popup Settings")] 
        [SerializeField] private PopupNotification popupPrefab;
        [SerializeField] private Transform popupContainer;
        [SerializeField] private float popupDuration = 3f;
        [SerializeField] private float popupSpacing = 10f;
        [SerializeField] private int maxVisiblePopups = 5;

        [Header("Event Popup Settings")] 
        [SerializeField] private PopupSettings missionStartSettings;
        [SerializeField] private PopupSettings missionCompleteSettings;
        [SerializeField] private PopupSettings objectiveActivatedSettings;
        [SerializeField] private PopupSettings objectiveCompleteSettings;
        [SerializeField] private PopupSettings itemObtainedSettings;

        private readonly Queue<PopupNotification> _activePopups = new Queue<PopupNotification>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnEnable()
        {
            GameEvents.OnMissionStarted += OnMissionStarted;
            GameEvents.OnMissionCompleted += OnMissionCompleted;
            GameEvents.OnObjectiveActivated += OnObjectiveActivated;
            GameEvents.OnObjectiveMet += OnObjectiveMet;
            GameEvents.OnItemObtained += OnItemObtained;
        }
        

        private void OnDisable()
        {
            GameEvents.OnMissionStarted -= OnMissionStarted;
            GameEvents.OnMissionCompleted -= OnMissionCompleted;
            GameEvents.OnObjectiveActivated -= OnObjectiveActivated;
            GameEvents.OnObjectiveMet -= OnObjectiveMet;
            GameEvents.OnItemObtained -= OnItemObtained;
        }

        private void OnMissionStarted(SOMission mission)
        {
            if (!missionStartSettings.Enabled) return;
            
            ShowPopup($"New Mission:\n{mission.Name}", missionStartSettings);
        }

        private void OnMissionCompleted(SOMission mission)
        {
            if (!missionCompleteSettings.Enabled) return;
            
            ShowPopup($"Mission Complete:\n{mission.Name}", missionCompleteSettings);
        }
        
        private void OnObjectiveActivated(MissionObjective objective)
        {
            if (objective.IsHidden || !objectiveActivatedSettings.Enabled) return;

            ShowPopup($"Objective Activated:\n{objective.GetDescription()}", objectiveActivatedSettings);
        }

        private void OnObjectiveMet(MissionObjective objective)
        {
            if (objective.IsHidden || !objectiveCompleteSettings.Enabled) return;

            ShowPopup($"Objective Complete:\n{objective.GetDescription()}", objectiveCompleteSettings);
        }

        private void OnItemObtained(SOItem item)
        {
                if (!itemObtainedSettings.Enabled) return;
                
            ShowPopup($"{item.Name}", itemObtainedSettings, item.Icon);
        }
        

        private void ShowPopup(string message, PopupSettings settings, Sprite overrideIcon = null)
        {
            var popupGo = ObjectPooler.GetObjectFromPool(popupPrefab);
            popupGo.transform.SetParent(popupContainer, false);
            var popup = popupGo.GetComponent<PopupNotification>();

            popup.RectTransform.anchoredPosition = Vector2.zero;

            Sprite icon = overrideIcon ? overrideIcon : settings?.Icon;
            Color color = settings?.BackgroundColor ?? Color.white;

            popup.Setup(message, color, icon);

            _activePopups.Enqueue(popup);

            if (_activePopups.Count > maxVisiblePopups)
            {
                var oldest = _activePopups.Dequeue();
                ObjectPooler.ReturnObjectToPool(oldest);
            }

            RepositionPopups();
            StartCoroutine(HidePopupAfterDelay(popup, popupDuration));
        }
        private void RepositionPopups()
        {
            int index = 0;
            foreach (var popup in _activePopups)
            {
                float targetY = -index * (popup.RectTransform.rect.height + popupSpacing);
                Tween.UIAnchoredPositionY(popup.RectTransform, targetY, 0.3f, Ease.OutCubic);
                index++;
            }
        }

        private IEnumerator HidePopupAfterDelay(PopupNotification popup, float delay)
        {
            yield return new WaitForSeconds(delay);

            if (_activePopups.Contains(popup))
            {
                var tempList = new List<PopupNotification>(_activePopups);
                tempList.Remove(popup);
                _activePopups.Clear();
                foreach (var p in tempList)
                {
                    _activePopups.Enqueue(p);
                }

                popup.Hide(() => ObjectPooler.ReturnObjectToPool(popup));
                RepositionPopups();
            }
        }
    }
}