using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace MagicBedlam
{
    /// <summary>
    /// Distribute colors to players
    /// </summary>
    public class ColorPicker
    {
        protected List<Color> _availableColors;

        public ColorPicker()
        {
            _availableColors = new List<Color>();

            Color[] colors = GameData.singleton.GetAllColors();
            colors.CopyTo<Color>(_availableColors);
        }

        /// <summary>
        ///     Pick random color from player color array
        /// </summary>
        /// <returns></returns>
        public Color PickColor()
        {
            Color endColor = _availableColors[Random.Range(0, _availableColors.Count)];
            _availableColors.Remove(endColor);

            return endColor;
        }
    }
}