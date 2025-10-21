using System;
using UnityEngine;
using UnityEngine.Events;

namespace RobbieWagnerGames.MultiFactorGame
{
    /// <summary>
    /// displays ui and accepts input for authentication steps
    /// </summary>
    public class AuthenticationScreen : MonoBehaviour
    {
        // Event fired when the screen has a completed input value (string) to validate
        public event Action<string> onInputSubmitted;

        // Optional Unity event for inspector wiring
        private UnityEvent<string> onInputSubmittedUnity = new UnityEvent<string>();

        [HideInInspector] public string userInput = string.Empty;

        public virtual void DisplayScreen()
        {
            // Override in derived classes to display the screen
        }

        public virtual void Clear()
        {
            // Override in derived classes to clear input/display
        }

        /// <summary>
        /// Call this to submit the input value to any subscribers.
        /// </summary>
        public virtual void SubmitInput(string input)
        {
            onInputSubmitted?.Invoke(input);
            onInputSubmittedUnity?.Invoke(input);
        }
    }
}