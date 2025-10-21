using System;
using UnityEngine;

namespace RobbieWagnerGames.MultiFactorGame
{
    public class AuthenticationStep : MonoBehaviour
    {
        [SerializeField] private GameObject stepUI;
        [SerializeField] protected AuthenticationScreen authenticationScreen;
        [SerializeField] private GameObject defaultSelectedObject;

        private bool hasStarted = false;

        private bool _isStepComplete;
        public bool isStepComplete
        {
            get => _isStepComplete;
            protected set
            {
                if (_isStepComplete == value) return;
                _isStepComplete = value;
                if (_isStepComplete)
                {
                    onStepCompleted?.Invoke();
                }
            }
        }

        public delegate void StepCompleted();
        public event StepCompleted onStepCompleted;

        public void LoadStep()
        {
            if (hasStarted)
            {
                ContinueStep();
            }
            else
            {
                StartStep();
            }

            // Ensure the screen displays whenever a step is loaded
            authenticationScreen?.DisplayScreen();
        }

        public virtual void StartStep()
        {
            hasStarted = true;
            if (stepUI != null) stepUI.SetActive(true);

            if (authenticationScreen != null)
                authenticationScreen.onInputSubmitted += ValidateInput;

            Debug.Log("starting step");
        }

        public virtual void ContinueStep()
        {
            if (stepUI != null) stepUI.SetActive(true);

            if (authenticationScreen != null)
            {
                // Ensure single subscription
                authenticationScreen.onInputSubmitted -= ValidateInput;
                authenticationScreen.onInputSubmitted += ValidateInput;
            }

            Debug.Log("continuing step");
        }

        public virtual void EndStep()
        {
            if (authenticationScreen != null)
                authenticationScreen.onInputSubmitted -= ValidateInput;

            if (stepUI != null) stepUI.SetActive(false);

            Debug.Log("Ending authentication step: " + gameObject.name);
            isStepComplete = true;
        }

        /// <summary>
        /// Override this to validate input submitted by the authentication screen.
        /// </summary>
        protected virtual void ValidateInput(string input)
        {
            // Default does nothing.
        }
    }
}