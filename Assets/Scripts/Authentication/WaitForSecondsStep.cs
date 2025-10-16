using System.Collections;
using TMPro;
using UnityEngine;

namespace RobbieWagnerGames.MultiFactorGame
{
    public class WaitForSecondsStep : AuthenticationStep
    {
        public int waitTimeInSeconds = 3;
        public TextMeshProUGUI countdownText;

        public override void StartStep()
        {
            base.StartStep();
            StartCoroutine(WaitAndComplete());
        }

        private IEnumerator WaitAndComplete()
        {
            int timeWaited = 0;
            while (timeWaited < waitTimeInSeconds)
            {
                if (countdownText != null)
                {
                    countdownText.text = (waitTimeInSeconds - timeWaited).ToString();
                }
                yield return new WaitForSeconds(1);
                timeWaited++;
            }

            EndStep();
        }
    }
}