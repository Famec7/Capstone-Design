using UnityEngine;

[CreateAssetMenu(fileName = "WalletAddress", menuName = "ScriptableObjects/WalletAddress")]
public class WalletAddress : ScriptableObject
{
    [SerializeField] private string walletAddress;

    public string Address
    {
        get => walletAddress;
        set => walletAddress = value;
    }

    public void ClearWalletAddress()
    {
        Address = string.Empty;
    }
}