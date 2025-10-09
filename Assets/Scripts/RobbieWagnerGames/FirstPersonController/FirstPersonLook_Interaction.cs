using UnityEngine;
using RobbieWagnerGames.Utilities;
using RobbieWagnerGames.FirstPerson.Interaction;

namespace RobbieWagnerGames.FirstPerson
{
    /// <summary>
    /// Handles first-person Look at interactions
    /// </summary>
    public partial class FirstPersonLook : MonoBehaviourSingleton<FirstPersonLook>
    {
        [HideInInspector] public bool canInteract = true;
        public bool enableInteractionByDefault = true;
        private Interactable _currentInteractable = null;
        public delegate void InteractableChanged(Interactable newInteractable);
        public event InteractableChanged OnCurrentInteractableChanged;

        private Interactable currentInteractable
        {
            get => _currentInteractable;
            set
            {
                if (_currentInteractable != value)
                {
                    _currentInteractable = value;
                    OnCurrentInteractableChanged?.Invoke(_currentInteractable);
                }
            }
        }

        private void SetupInteraction()
        {
            canInteract = enableInteractionByDefault;
        }

        private void UpdateInteractionState()
        {
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            Interactable hitInteractable = null;
            if (Physics.Raycast(ray, out RaycastHit hit, 3f))
            {
                hitInteractable = hit.collider.GetComponent<Interactable>();
            }

            if (hitInteractable != null)
            {
                if (currentInteractable == null)
                {
                    hitInteractable.OnCursorEnter();
                }
                else if (currentInteractable == hitInteractable)
                {
                    hitInteractable.OnCursorStay();
                }
                else
                {
                    currentInteractable.OnCursorExit();
                    hitInteractable.OnCursorEnter();
                }
                currentInteractable = hitInteractable;
            }
            else
            {
                if (currentInteractable != null)
                {
                    currentInteractable.OnCursorExit();
                    currentInteractable = null;
                }
            }
        }

        private void OnInteractPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (currentInteractable != null)
            {
                currentInteractable.Interact();
            }
        }

        private void HandleCurrentInteractableChanged(Interactable newInteractable)
        {
            if (newInteractable == null)
            {
                InteractionUIManager.Instance.DisableCursor();
            }
            else
            {
                Sprite sprite = newInteractable.cursorOverSprite;
                InteractionUIManager.Instance.UpdateCursor(sprite);
                InteractionUIManager.Instance.EnableCursor();
            }
            
            
        }
    }
}