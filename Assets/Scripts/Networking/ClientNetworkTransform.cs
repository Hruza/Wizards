using Unity.Netcode.Components;
using UnityEngine;

public enum AuthorityMode{
    Server,
    Client
}

public class ClientNetworkTransform : NetworkTransform
{
    public AuthorityMode authorityMode = AuthorityMode.Client;
    protected override bool OnIsServerAuthoritative()
    {
        return authorityMode == AuthorityMode.Server;
    }
}
