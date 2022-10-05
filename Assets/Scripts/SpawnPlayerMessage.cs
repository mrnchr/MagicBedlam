using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public struct SpawnPlayerMessage : NetworkMessage
{
    public Color playerColor;
    public Vector3 spawnPosition;
}
