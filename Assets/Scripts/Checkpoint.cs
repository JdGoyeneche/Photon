using UnityEngine;
using Fusion;

public class Checkpoint : MonoBehaviour
{
    [Header("Configuración del Checkpoint")]
    [Tooltip("El número de este checkpoint dentro de su respectiva ruta.")]
    public int checkpointIndex;

    [Tooltip("A qué camino pertenece este checkpoint (Ej: 'RutaA', 'RutaB', 'Principal', 'Meta')")]
    public string routeName = "Principal";

    private void OnTriggerEnter(Collider other)
    {
        // Buscamos el componente de progreso en el carro que cruzó
        NetworkPlayerProgress playerProgress = other.GetComponentInParent<NetworkPlayerProgress>();
        
        if (playerProgress != null)
        {
            // Solo el Servidor/Host calcula y procesa las reglas del juego
            if (playerProgress.Runner.IsServer)
            {
                // Le enviamos los datos del checkpoint al carro para que los valide
                playerProgress.SetCheckpoint(checkpointIndex, routeName);
                
                Debug.Log($"[SERVIDOR] Jugador {playerProgress.Object.InputAuthority.PlayerId} validado en Checkpoint {checkpointIndex} de la ruta '{routeName}'");
            }
        }
    }
}