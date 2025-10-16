using System;
using RobbieWagnerGames.Utilities;
using UnityEngine;

namespace RobbieWagnerGames.MultiFactorGame
{
    public class AuthenticationManager : MonoBehaviourSingleton<AuthenticationManager>
    {
        [SerializeField] private AuthenticationStep[] authenticationSteps;
        private int currentStepIndex = 0;

        public void RunAuthentication()
        {
            if (authenticationSteps.Length == 0)
            {
                Debug.LogWarning("No authentication steps assigned.");
                return;
            }

            currentStepIndex = 0;
            RunCurrentStep();
        }

        private void RunCurrentStep()
        {
            if (currentStepIndex < authenticationSteps.Length)
            {
                AuthenticationStep currentStep = authenticationSteps[currentStepIndex];
                currentStep.onStepCompleted += OnCurrentStepCompleted;
                currentStep.StartStep();
            }
            else
            {
                Debug.Log("All authentication steps completed.");
            }
        }

        private void OnCurrentStepCompleted()
        {
            AuthenticationStep currentStep = authenticationSteps[currentStepIndex];
            currentStep.onStepCompleted -= OnCurrentStepCompleted;
            currentStepIndex++;
            RunCurrentStep();
        }
    }
}