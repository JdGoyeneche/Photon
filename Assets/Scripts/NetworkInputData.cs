using Fusion;

public struct NetworkInputData : INetworkInput
{
    public float steering;
    public float throttle;
    public NetworkBool brake;
}