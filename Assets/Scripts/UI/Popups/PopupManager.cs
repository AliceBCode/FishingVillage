using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PrimeTween;

[Serializable]
public class PopupSettings
{
    [SerializeField] private Color backgroundColor = Color.white;
    [SerializeField] private Sprite icon;
    
    public Color BackgroundColor => backgroundColor;
    public Sprite Icon => icon;
}

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
    
    private readonly Queue<PopupNotification> activePopups = new Queue<PopupNotification>();
    private readonly Queue<PopupNotification> popupPool = new Queue<PopupNotification>();

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
        ShowPopup($"New Mission: {mission.Name}", missionStartSettings);
    }

    private void OnMissionCompleted(SOMission mission)
    {
        ShowPopup($"Mission Complete: {mission.Name}", missionCompleteSettings);
    }

    private void OnObjectiveMet(MissionObjective objective)
    {
        if (objective.IsHidden) return;
        
        ShowPopup($"Objective Complete: {objective.Description}", objectiveCompleteSettings);
    }

    private void OnItemObtained(SOItem item)
    {
        ShowPopup($"Obtained: {item.Name}", itemObtainedSettings, item.Icon);
    }

    private void OnItemRemoved(SOItem item)
    {
        ShowPopup($"Removed: {item.Name}", itemRemovedSettings, item.Icon);
    }
    

    public void ShowPopup(string message, PopupSettings settings, Sprite overrideIcon = null)
    {
        var popup = GetOrCreatePopup();
        
        Sprite icon = overrideIcon ? overrideIcon : settings?.Icon;
        Color color = settings?.BackgroundColor ?? Color.white;
        
        popup.Setup(message, color, icon);
        
        activePopups.Enqueue(popup);
        
        if (activePopups.Count > maxVisiblePopups)
        {
            var oldest = activePopups.Dequeue();
            ReturnToPool(oldest);
        }
        
        RepositionPopups();
        StartCoroutine(HidePopupAfterDelay(popup, popupDuration));
    }

    private PopupNotification GetOrCreatePopup()
    {
        if (popupPool.Count > 0)
        {
            var popup = popupPool.Dequeue();
            popup.gameObject.SetActive(true);
            return popup;
        }
        
        return Instantiate(popupPrefab, popupContainer);
    }

    private void ReturnToPool(PopupNotification popup)
    {
        popup.gameObject.SetActive(false);
        popupPool.Enqueue(popup);
    }

    private void RepositionPopups()
    {
        int index = 0;
        foreach (var popup in activePopups)
        {
            float targetY = -index * (popup.RectTransform.rect.height + popupSpacing);
            Tween.UIAnchoredPositionY(popup.RectTransform, targetY, 0.3f, Ease.OutCubic);
            index++;
        }
    }

    private IEnumerator HidePopupAfterDelay(PopupNotification popup, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (activePopups.Contains(popup))
        {
            var tempList = new List<PopupNotification>(activePopups);
            tempList.Remove(popup);
            activePopups.Clear();
            foreach (var p in tempList)
            {
                activePopups.Enqueue(p);
            }
            
            popup.Hide(() => ReturnToPool(popup));
            RepositionPopups();
        }
    }
}