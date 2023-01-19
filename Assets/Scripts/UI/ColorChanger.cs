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
        [Tooltip("Fields which changes color")]
        [SerializeField]
        protected Graphic[] _colorFields;

        protected void Awake()
        {
            singleton = Singleton.Create<ColorChanger>(this, singleton);
        }

        public void SetColor(Color menuColor)
        {
            foreach(var field in _colorFields)
            {
                field.color = menuColor;
            }
        }
    }
}
