using System.Collections;
using UnityEngine;
using TMPro;

namespace MagicBedlam
{
    /// <summary>
    /// Calculating and output fps to the screen 
    /// </summary>
    public class FPS : MonoBehaviour
    {
        [SerializeField] TMP_Text _FPS;
        protected float _fps;

        protected void Start() 
        {
            StartCoroutine(WaitForUpdateFPS());
        }

        protected void Update()
        {
            _fps = 1 / Time.deltaTime;
        } 

        protected IEnumerator WaitForUpdateFPS()
        {
            while(true) 
            {
                yield return new WaitForSeconds(0.5f);
                _FPS.text = $"FPS: {Mathf.RoundToInt(_fps).ToString("0")}";
            }
        }
    }
}
