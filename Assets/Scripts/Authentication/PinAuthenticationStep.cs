using System.Collections;
using UnityEngine;

namespace RobbieWagnerGames.MultiFactorGame
{
    /// <summary>
    /// Pin pad authentication step. Exposes 10 number buttons (0-9) and a TextMeshProUGUI display.
    /// When 4 digits are entered the pin is checked; if correct, base.EndStep() is called, otherwise the input resets.
    /// </summary>
    public class PinAuthenticationStep : AuthenticationStep
    {
        [SerializeField] private string correctPin = "1234";

        public override void StartStep()
        {
            base.StartStep();
            authenticationScreen.gameObject.SetActive(true);
        }

        public override void ContinueStep()
        {
            base.ContinueStep();
            authenticationScreen.gameObject.SetActive(true);
        }

        private IEnumerator RejectAndReset()
        {
            Debug.LogWarning("Incorrect PIN entered");
            yield return new WaitForSeconds(0.4f);
            authenticationScreen.Clear();

            // Clear the pin pad UI
            if (authenticationScreen != null) authenticationScreen.Clear();
        }

        public override void EndStep()
        {
            // Only call base.EndStep if the entered pin is correct
            if (authenticationScreen.userInput == correctPin)
            {
                // cleanup
                // base.EndStep will unsubscribe authenticationScreen (which is our authenticationScreen)

                if (authenticationScreen.gameObject != null)
                    authenticationScreen.gameObject.SetActive(false);

                base.EndStep();
            }
            else
            {
                Debug.LogWarning("Cannot complete step - PIN is incorrect or missing");
            }
        }


        protected override void ValidateInput(string input)
        {
            if (authenticationScreen.userInput == correctPin)
            {
                EndStep();
            }
            else
            {
                StartCoroutine(RejectAndReset());
            }
        }

        private void OnDisable()
        {
            // Ensure listeners are removed when disabled/destroyed
            if (authenticationScreen != null) authenticationScreen.onInputSubmitted -= ValidateInput;
            if (authenticationScreen != null) authenticationScreen.Clear();
        }
    }
}
