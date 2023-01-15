using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

namespace MagicBedlam
{
    public class TableKeeper : MonoBehaviour
    {
        public static TableKeeper singleton { get; protected set; }

        [Tooltip("Text fields of color names")]
        [SerializeField] 
        protected TMP_Text[] _colors;
        [Tooltip("Text fields of scores")]
        [SerializeField] 
        protected TMP_Text[] _scores;

        protected int _numberOfPlayers;

        protected void Awake()
        {
            singleton = Singleton.Create<TableKeeper>(this, singleton);

            for (int i = 0; i < _scores.Length; i++)
            {
                _scores[i].gameObject.SetActive(false);
                _colors[i].gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Open new table string about the player with playerColor
        /// </summary>
        /// <param name="playerColor"></param>
        public void AddPlayerInfo(Color playerColor)
        {
            for (int i = 0; i < _numberOfPlayers; i++)
            {
                if (_scores[i].color == playerColor)
                    return;
            }

            _scores[_numberOfPlayers].gameObject.SetActive(true);
            _scores[_numberOfPlayers].color = playerColor;
            _scores[_numberOfPlayers].text = "0";
            _colors[_numberOfPlayers].gameObject.SetActive(true);
            _colors[_numberOfPlayers].color = playerColor;
            _colors[_numberOfPlayers].text = GameData.singleton.GetColorName(playerColor);

            ++_numberOfPlayers;
        }

        /// <summary>
        /// Change table string with the color and the table order
        /// </summary>
        /// <param name="color"></param>
        /// <param name="scores"></param>
        public void ChangePlayerTable(Color color, int scores)
        {
            int oldIndex;
            int newIndex;

            for (oldIndex = 0; oldIndex < _numberOfPlayers; ++oldIndex)
            {
                if (_scores[oldIndex].color == color)
                    break;
            }

            _scores[oldIndex].text = scores.ToString();

            // NOTE: you can not to swap positions objects because you have the different indexes
            // NOTE: also you need to change fields each other in loop not to break the order
            for (newIndex = oldIndex - 1; newIndex >= 0 && scores > int.Parse(_scores[newIndex].text); --newIndex)
            {
                SwapTableString(oldIndex, newIndex);
            }
        }

        public Color GetWinner() {
            return _colors[0].color;
        }

        protected void SwapTableString(int oldIndex, int newIndex)
        {
            SwapTableText(_scores[oldIndex], _scores[newIndex]);
            SwapTableText(_colors[oldIndex], _colors[newIndex]);
        }

        protected void SwapTableText(TMP_Text a, TMP_Text b)
        {
            (a.color, b.color) = (b.color, a.color);
            (a.text, b.text) = (b.text, a.text);
        }
    }
}
