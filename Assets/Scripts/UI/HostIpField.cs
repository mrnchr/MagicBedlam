using UnityEngine;
using TMPro;

public class HostIpField : MonoBehaviour 
{
    [SerializeField] private TMP_InputField _self;

    private void Start() {
        _self.text = NetworkInteraction.Instance.LastConnection;
    }
}