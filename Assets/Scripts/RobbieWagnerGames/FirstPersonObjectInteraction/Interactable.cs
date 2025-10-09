using UnityEngine;

namespace RobbieWagnerGames.FirstPerson.Interaction
{ 
    public class Interactable : MonoBehaviour
    {
        public Sprite cursorOverSprite = null;

        public virtual void OnCursorEnter()
        {
            // Debug.Log($"Cursor entered {gameObject.name}");
        }

        public virtual void OnCursorStay()
        {
            // Debug.Log($"Cursor staying on {gameObject.name}");
        }

        public virtual void OnCursorExit()
        {
            // Debug.Log($"Cursor exited {gameObject.name}");
        }

        public virtual void Interact()
        {
            //Debug.Log($"Interacted with {gameObject.name}");
        }
    }
}