using UnityEngine;
using Mirror;

public class Fire : MonoBehaviour {
    [ServerCallback]
    private void FixedUpdate() {
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}