using System;
using RobbieWagnerGames.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace RobbieWagnerGames.FirstPerson.Interaction
{
    public class InteractionUIManager : MonoBehaviourSingleton<InteractionUIManager>
    {
        [SerializeField] private Image cursorImage;
        private Sprite defaultCursorSprite = null;

        public void UpdateCursor(Sprite newSprite)
        {
            cursorImage.sprite = newSprite != null ? newSprite : defaultCursorSprite;
        }

        public void DisableCursor()
        {
            cursorImage.enabled = false;
        }

        public void EnableCursor()
        {
            cursorImage.enabled = true;        
        }
    }
}