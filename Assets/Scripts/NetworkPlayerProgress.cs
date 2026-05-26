using Fusion;
using UnityEngine;

public class NetworkPlayerProgress : NetworkBehaviour
{
    [Networked] public int LastCheckpointIndex { get; set; } = -1;
    [Networked] public string CurrentRoute { get; set; } = "Principal";
    [Networked] public int CurrentLap { get; set; } = 0;

    public override void Spawned()
    {
        LastCheckpointIndex = -1;
        CurrentRoute = "Principal";
        CurrentLap = 1;
    }

    public void SetCheckpoint(int index, string route)
    {
        if (CurrentRoute != route)
        {
            Debug.Log($"[RED] Jugador {Object.InputAuthority.PlayerId} cambió a la ruta: {route}");
            CurrentRoute = route;
            LastCheckpointIndex = index;
            return;
        }

        if (index > LastCheckpointIndex || (index == 0 && LastCheckpointIndex > 0))
        {
            if (index == 0 && LastCheckpointIndex == -1) return;

            LastCheckpointIndex = index;

            if (index == 0)
            {
                CurrentLap++;
                Debug.Log($"¡Jugador {Object.InputAuthority.PlayerId} inició la Vuelta {CurrentLap}!");

                NetworkGameManager gameManager = UnityEngine.Object.FindFirstObjectByType<NetworkGameManager>();

                if (gameManager != null && !gameManager.IsRaceEnded())
                {
                    if (CurrentLap > gameManager.totalLaps)
                    {
                        Debug.Log($"¡JUGADOR {Object.InputAuthority.PlayerId} HA GANADO!");

                        if (Runner.IsServer)
                        {
                            gameManager.RPC_OnRaceFinished(Object.InputAuthority.PlayerId);
                            gameManager.RPC_LoadVictoryScene();
                        }
                    }
                }
            }
        }
    } // <--- Cierre de SetCheckpoint
} // <--- Cierre de la clase NetworkPlayerProgress