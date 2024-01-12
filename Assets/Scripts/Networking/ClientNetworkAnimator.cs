using Unity.Netcode.Components;
using UnityEngine;

public class ClientNetworkAnimator : NetworkAnimator
{
    [SerializeField] private AuthorityMode authorityMode = AuthorityMode.Client;
    protected override bool OnIsServerAuthoritative()
    {
        return authorityMode == AuthorityMode.Server;
    }
}
