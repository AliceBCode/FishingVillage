using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PrimeTween;

namespace FishingVillage.UI.Popup
{
    public class PopupManager : MonoBehaviour
    {
        public static PopupManager Instance;

        [Header("Popup Settings")] 
        [SerializeField] private PopupNotification popupPrefab;
        [SerializeField] private Transform popupContainer;
        [SerializeField] private float popupDuration = 3f;
        [SerializeField] private float popupSpacing = 10f;
        [SerializeField] private int maxVisiblePopups = 5;

        [Header("Event Popup Settings")] 
        [SerializeField] private PopupSettings missionStartSettings;
        [SerializeField] private PopupSettings missionCompleteSettings;
        [SerializeField] private PopupSettings objectiveCompleteSettings;
        [SerializeField] private PopupSettings itemObtainedSettings;
        [SerializeField] private PopupSettings itemRemovedSettings;

        private readonly Queue<PopupNotification> _activePopups = new Queue<PopupNotification>();
        private readonly Queue<PopupNotification> _popupPool = new Queue<PopupNotification>();

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
            MissionObjective.OnObjectiveMet += OnObjectiveMet;

            GameEvents.OnItemObtained += OnItemObtained;
            GameEvents.OnItemRemoved += OnItemRemoved;
        }

        private void OnDisable()
        {
            GameEvents.OnMissionStarted -= OnMissionStarted;
            GameEvents.OnMissionCompleted -= OnMissionCompleted;
            MissionObjective.OnObjectiveMet -= OnObjectiveMet;

            GameEvents.OnItemObtained -= OnItemObtained;
            GameEvents.OnItemRemoved -= OnItemRemoved;
        }

        private void OnMissionStarted(SOMission mission)
        {
            ShowPopup($"New Mission:\n{mission.Name}", missionStartSettings);
        }

        private void OnMissionCompleted(SOMission mission)
        {
            ShowPopup($"Mission Complete:\n{mission.Name}", missionCompleteSettings);
        }

        private void OnObjectiveMet(MissionObjective objective)
        {
            if (objective.IsHidden) return;

            ShowPopup($"Objective Complete:\n{objective.GetDescription()}", objectiveCompleteSettings);
        }

        private void OnItemObtained(SOItem item)
        {
            ShowPopup($"Obtained:\n{item.Name}", itemObtainedSettings, item.Icon);
        }

        private void OnItemRemoved(SOItem item)
        {
            ShowPopup($"Removed:\n{item.Name}", itemRemovedSettings, item.Icon);
        }


        public void ShowPopup(string message, PopupSettings settings, Sprite overrideIcon = null)
        {
            var popup = GetOrCreatePopup();

            Sprite icon = overrideIcon ? overrideIcon : settings?.Icon;
            Color color = settings?.BackgroundColor ?? Color.white;

            popup.Setup(message, color, icon);

            _activePopups.Enqueue(popup);

            if (_activePopups.Count > maxVisiblePopups)
            {
                var oldest = _activePopups.Dequeue();
                ReturnToPool(oldest);
            }

            RepositionPopups();
            StartCoroutine(HidePopupAfterDelay(popup, popupDuration));
        }

        private PopupNotification GetOrCreatePopup()
        {
            if (_popupPool.Count > 0)
            {
                var popup = _popupPool.Dequeue();
                popup.gameObject.SetActive(true);
                return popup;
            }

            return Instantiate(popupPrefab, popupContainer);
        }

        private void ReturnToPool(PopupNotification popup)
        {
            popup.gameObject.SetActive(false);
            _popupPool.Enqueue(popup);
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

                popup.Hide(() => ReturnToPool(popup));
                RepositionPopups();
            }
        }
    }
}