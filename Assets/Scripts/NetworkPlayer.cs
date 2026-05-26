using Fusion;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    public override void Spawned()
    {
        if (!Object.HasInputAuthority)
        {
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null)
                cam.gameObject.SetActive(false);
        }
    }
}