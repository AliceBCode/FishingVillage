using System;
using DNExtensions.Shapes;
using DNExtensions.Utilities;
using TMPro;
using UnityEngine;



public class GameMenuPrompt : MonoBehaviour
{
    
    [Header("Settings")]
    [SerializeField] private string menuText = "Menu";
    [SerializeField] private Color menuColor = Color.dodgerBlue;
    [SerializeField] private Color menuTextColor = Color.black;
    
    [Header("References")]
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private SDFCircle circle;

    
    private string _hudText = "";
    private Color _hudColor = Color.clear;
    private Color _hudTextColor = Color.white;

    private void Start()
    {
        _hudText = textMesh.text;
        _hudColor = circle.baseColor;
        _hudTextColor = textMesh.color;
    }

    public void ShowMenuVisuals()
    {
        textMesh.text = menuText;
        circle.baseColor = menuColor;
        textMesh.color = menuTextColor;
    }

    public void ShowDefaultVisuals()
    {
        textMesh.text = _hudText;
        circle.baseColor = _hudColor;
        textMesh.color = _hudTextColor;
    }
}