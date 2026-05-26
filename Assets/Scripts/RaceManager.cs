using Fusion;
using UnityEngine;
using TMPro;
using System.Linq;

public class RaceManager : NetworkBehaviour
{
    public static RaceManager Instance;

    [Networked] public bool raceStarted { get; set; }
    [Networked] public int countdownValue { get; set; }

    public TextMeshProUGUI countdownText;

    private bool countdownRunning = false;

    private void Awake()
    {
        Instance = this;
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority)
            return;

        // Esperar 2 jugadores
        if (Runner.ActivePlayers.Count() >= 2 && !countdownRunning && !raceStarted)
        {
            countdownRunning = true;

            StartCoroutine(StartCountdown());
        }
    }

    System.Collections.IEnumerator StartCountdown()
    {
        countdownValue = 3;

        while (countdownValue > 0)
        {
            yield return new WaitForSeconds(1f);

            countdownValue--;
        }

        raceStarted = true;

        countdownText.text = "GO!";

        yield return new WaitForSeconds(1f);

        countdownText.text = "";
    }

    private void Update()
    {

        if (!Object || !Object.IsValid)
    return;
        if (countdownText == null)
            return;

        if (!raceStarted)
        {
            if (countdownValue > 0)
            {
                countdownText.text = countdownValue.ToString();
            }
            else
            {
                countdownText.text = "Waiting Players...";
            }
        }
    }
}