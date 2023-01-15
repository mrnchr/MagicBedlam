using UnityEngine;
using TMPro;

namespace MagicBedlam
{
    public class HostIpField : MonoBehaviour
    {
        [Tooltip("Own InputField component")]
        [SerializeField]
        protected TMP_InputField _ownInputField;

        public string Text 
        {
            get
            {
                return _ownInputField.text;
            }
            set 
            {
                _ownInputField.text = value;
            }
        }

        protected void Start()
        {
            _ownInputField.text = NetworkInteraction.singleton.LastConnection;
        }

        protected void Reset() {
            TryGetComponent<TMP_InputField>(out _ownInputField);
        }
    }
}