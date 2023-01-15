using System.Collections.Generic;
using UnityEngine;

namespace MagicBedlam
{
    /// <summary>
    /// Store global game data
    /// </summary>
    public class GameData : MonoBehaviour
    {
        static public GameData singleton { get; protected set; }

        [Tooltip("Time of one game session in seconds")]
        [SerializeField]
        [Range(0, 1000)]
        protected int _sessionTime;
        [Tooltip("A number of scores for win")]
        [SerializeField]
        [Range(0, 100)]
        protected int _winScores;
        [Tooltip("The colors for five players")]
        [SerializeField]
        protected Color[] _playerColors;
        [Tooltip("The spelling of the player colors")]
        [SerializeField]
        protected string[] _colorNames;

        protected Dictionary<string, Color> _playerColor;

        public int SessionTime
        {
            get
            {
                return _sessionTime;
            }
        }

        public int WinScores
        {
            get
            {
                return _winScores;
            }
        }

        protected void Awake()
        {
            singleton = Singleton.Create<GameData>(this, singleton);

            _playerColor = new Dictionary<string, Color>(_colorNames.Length);
            for (int i = 0; i < _colorNames.Length; i++)
            {
                _playerColor[_colorNames[i]] = _playerColors[i];
            }
        }

        /// <summary>
        ///     Get the accepted player colors array
        /// </summary>
        /// <returns></returns>
        public Color[] GetAllColors()
        {
            Debug.Log(_playerColors[4]);
            return _playerColors;
        }

        /// <summary>
        ///     Get the player color by its name
        /// </summary>
        /// <param name="name">A name corresponding to color</param>
        /// <returns></returns>
        public Color GetColor(string name)
        {
            return _playerColor[name];
        }

        /// <summary>
        ///     Get the name of player color
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public string GetColorName(Color color)
        {
            foreach (var playercolor in _playerColor)
            {
                if (playercolor.Value == color)
                {
                    return playercolor.Key;
                }
            }

            return null;
        }

        protected void OnValidate()
        {
            if (_playerColors.Length != _colorNames.Length || _playerColors.Length != 5)
            {
                Debug.LogWarning($"The number of player colors and their names must be equal to 5 in {this.GetType().ToString()} component");
            }
        }
    }
}