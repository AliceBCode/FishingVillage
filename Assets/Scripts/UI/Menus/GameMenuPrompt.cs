using System;
using DNExtensions.Utilities;
using DNExtensions.Utilities.Shapes;
using TMPro;
using UnityEngine;



public class GameMenuPrompt : MonoBehaviour
{
    
    [Header("Settings")]
    [SerializeField] private string hudText = "Prompt";
    [SerializeField] private Color hudColor = Color.clear;
    [SerializeField] private Color hudTextColor = Color.white;
    [Separator]
    [SerializeField] private string menuText = "Menu";
    [SerializeField] private Color menuColor = Color.dodgerBlue;
    [SerializeField] private Color menuTextColor = Color.black;
    
    [Header("References")]
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private SDFCircle circle;


    private void Awake()
    {
        ShowHud();
    }

    public void ShowMenu()
    {
        textMesh.text = menuText;
        circle.baseColor = menuColor;
        textMesh.color = menuTextColor;
    }

    public void ShowHud()
    {
        textMesh.text = hudText;
        circle.baseColor = hudColor;
        textMesh.color = hudTextColor;
    }
}