using DNExtensions.Utilities;
using DNExtensions.Utilities.SerializedInterface;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FishingVillage.Player
{
    [RequireComponent(typeof(PlayerControllerInput))]
    public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private bool canInteractWhileAirborne = true;
    [SerializeField] private float interactCheckRange = 3f;
    [SerializeField] private Vector3 interactCheckOffset = Vector3.zero;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField, ReadOnly] private InterfaceReference<Interactable.IInteractable> closestInteractable;
    
    private PlayerControllerInput _input;
    private PlayerController _playerController;
    
    private bool CanInteract => canInteractWhileAirborne || _playerController.isGrounded;
    


    private void Awake()
    {
        _input = GetComponent<PlayerControllerInput>();
        _playerController = GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        _input.OnInteractAction += OnInteractAction;
    }

    private void OnDisable()
    {
        _input.OnInteractAction -= OnInteractAction;
    }
    
    private void OnInteractAction(InputAction.CallbackContext context)
    {
        if (!_playerController.CanInteract()) return;
        
        if (CanInteract && context.performed)
        {
            closestInteractable?.Value?.Interact();
        }
    }

    private void FixedUpdate()
    {
        CheckForInteractable();
    }



    private void CheckForInteractable()
    {
        if (!_playerController.CanInteract()) return;
        
        
        var colliders = Physics.OverlapSphere(transform.position + interactCheckOffset, interactCheckRange, interactableLayer);
        var closestDistance = float.MaxValue;
        Interactable.IInteractable closest = null;

        foreach (var col in colliders)
        {
            if (col.TryGetComponent(out Interactable.IInteractable interactable) && interactable.CanInteract())
            {
                float distance = Vector3.Distance(transform.position, col.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = interactable;
                }
            }
        }

        closestInteractable.TryGetValue(out Interactable.IInteractable current);

        if (closest != current)
        {
            current?.HideInteract();
            closest?.ShowInteract();
        }
    
        closestInteractable.Value = closest;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + interactCheckOffset, interactCheckRange);
    }
    }
}