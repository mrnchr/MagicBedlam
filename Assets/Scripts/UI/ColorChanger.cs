using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MagicBedlam
{
    /// <summary>
    /// Changing local player interface color
    /// </summary>
    public class ColorChanger : MonoBehaviour
    {
        public static ColorChanger singleton { get; protected set; }
        [Tooltip("Text field with time")]
        [SerializeField] 
        protected TMP_Text _timeText;
        [Tooltip("The player cursor")]
        [SerializeField] 
        protected Image _cursor;

        protected void Awake()
        {
            singleton = Singleton.Create<ColorChanger>(this, singleton);
        }

        public void SetColor(Color menuColor)
        {
            _timeText.color = _cursor.color = menuColor;
        }
    }
}
