using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class NetworkGameManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public static string SelectedMode = "";
    public static string pendingErrorMessage = "";

    public NetworkPrefabRef playerPrefab;

    [Header("Spawn Settings")]
    public Transform[] spawnPoints;

    [Header("Race Settings")]
    public int totalLaps = 3;

    private TextMeshProUGUI statusText;
    private bool raceEnded = false;
    private NetworkRunner runner;
    private bool isConnecting = false;

    // =========================
    // START (CORRECTO)
    // =========================
    private IEnumerator Start()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            SelectedMode = "";
            isConnecting = false;
        }

        // Esperar a que exista el UI
        yield return new WaitUntil(() =>
        {
            GameObject obj = GameObject.Find("StatusText_Network");
            if (obj != null)
            {
                statusText = obj.GetComponent<TextMeshProUGUI>();
                return true;
            }
            return false;
        });

        // Mostrar mensaje pendiente (si existe)
        if (!string.IsNullOrEmpty(pendingErrorMessage))
        {
            statusText.text = pendingErrorMessage;
            pendingErrorMessage = "";

            yield return new WaitForSeconds(3f);

            if (statusText != null)
                statusText.text = "";
        }
        else
        {
            statusText.text = "";
        }

        // Lógica de entrada a la carrera
        if (SceneManager.GetActiveScene().name == "yoshicar" && !isConnecting)
        {
            if (SelectedMode == "HOST")
            {
                UpdateStatus("Iniciando Servidor...");
                _ = StartGameAsync(GameMode.Host);
            }
            else if (SelectedMode == "CLIENT")
            {
                UpdateStatus("Buscando al Host... Por favor espera.");
                _ = StartGameAsync(GameMode.Client);
            }
        }
    }

    // =========================
    // NETWORK START
    // =========================
    async System.Threading.Tasks.Task StartGameAsync(GameMode mode)
    {
        isConnecting = true;

        runner = gameObject.AddComponent<NetworkRunner>();
        runner.ProvideInput = true;

        var args = new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "Room1",
            Scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex),
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        };

        var fusionTask = runner.StartGame(args);

        if (mode == GameMode.Client)
        {
            var delayTask = System.Threading.Tasks.Task.Delay(6000);

            var completed = await System.Threading.Tasks.Task.WhenAny(fusionTask, delayTask);

            if (completed == delayTask)
            {
                ForceAbortConnection("No se encontró servidor. Inténtelo de nuevo.");
                return;
            }
        }

        await fusionTask;
    }

    // =========================
    // ABORT CONNECTION
    // =========================
    private void ForceAbortConnection(string errorMsg)
    {
        isConnecting = false;
        SelectedMode = "";

        if (runner != null)
        {
            runner.Shutdown(false, ShutdownReason.Ok);
            Destroy(runner);
        }

        // 🔥 IMPORTANTE: usa el mensaje recibido
        pendingErrorMessage = errorMsg;

        SceneManager.LoadScene("MainMenu");
    }

    // =========================
    // UI UPDATE
    // =========================
    private void UpdateStatus(string message)
    {
        if (statusText != null)
            statusText.text = message;
    }

    // =========================
    // BUTTONS
    // =========================
    public void StartGameAsHost()
    {
        pendingErrorMessage = "";
        SelectedMode = "HOST";
        SceneManager.LoadScene("yoshicar");
    }

    public void StartGameAsClient()
    {
        pendingErrorMessage = "";
        SelectedMode = "CLIENT";
        SceneManager.LoadScene("yoshicar");
    }

    // =========================
    // PLAYER JOIN
    // =========================
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        isConnecting = false;

        if (statusText != null)
            statusText.text = "";

        if (!runner.IsServer) return;

        if (!playerPrefab.IsValid)
        {
            Debug.LogError("[NETWORK] PlayerPrefab no asignado.");
            return;
        }

        int index = 0;
        int i = 0;

        foreach (var p in runner.ActivePlayers)
        {
            if (p == player) break;
            i++;
        }

        index = Mathf.Clamp(i, 0, spawnPoints.Length - 1);

        Vector3 pos = spawnPoints[index].position;
        Quaternion rot = spawnPoints[index].rotation;

        runner.Spawn(playerPrefab, pos, rot, player);
    }

    // =========================
    // SHUTDOWN
    // =========================


    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        if (shutdownReason != ShutdownReason.Ok && isConnecting)
        {
            ForceAbortConnection("No se encontro Host,Intentelo de nuevo.");
        }
    }

    // =========================
    // INPUT
    // =========================
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        NetworkInputData data = new NetworkInputData();
        data.steering = Input.GetAxisRaw("Horizontal");
        data.throttle = Input.GetAxisRaw("Vertical");
        data.brake = Input.GetKey(KeyCode.Space);
        input.Set(data);
    }

    // =========================
    // RPC
    // =========================
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_OnRaceFinished(int winnerPlayerId)
    {
        raceEnded = true;

        PrometeoCarController[] cars =
            Object.FindObjectsByType<PrometeoCarController>(FindObjectsSortMode.None);

        foreach (var car in cars)
        {
            Rigidbody rb = car.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }

            car.enabled = false;
        }
    }

[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
public void RPC_LoadVictoryScene()
{
    // 1. PRIMERO: Cambiamos la escena inmediatamente. 
    // Esto garantiza que el jugador salga de la partida actual.
    SceneManager.LoadScene("Ganar");

    // 2. DESPUÉS: Limpiamos la red. 
    // Como ya estamos en otra escena, el Runner se destruirá solo, 
    // pero llamamos a Shutdown para asegurar que no queden procesos fantasma.
    if (runner != null)
    {
        runner.Shutdown();
    }
}

    public bool IsRaceEnded() => raceEnded;

    // =========================
    // REQUIRED CALLBACKS
    // =========================
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, System.ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
}