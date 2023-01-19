using UnityEngine;
using Mirror;

namespace MagicBedlam
{
    /// <summary>
    /// Process input data
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        public static InputManager singleton { get; protected set; }

        protected Vector2 _inputMouse;
        protected Vector3 _inputMovement;
        protected Mover _mover;
        protected Telekinesis _tl;
        protected InputState input;

        protected void Awake()
        {
            singleton = Singleton.Create<InputManager>(this, singleton);

            Debug.Log("InputManager:Awake");
            input = InputState.Unlock;
        }

        /// <summary>
        /// Lock any input
        /// </summary>
        public void LockInput() 
        {
            input = InputState.Lock;

            ResetInput();
        }

        /// <summary>
        ///     Set player movement script
        /// </summary>
        /// <param name="mover"></param>
        public void SetMover(Mover mover) => _mover = mover;

        // NOTE: used by button
        /// <summary>
        /// Set input on pause and set pause menu
        /// </summary>
        public void SetPause()
        {
            input = input == InputState.Pause ? InputState.Unlock : InputState.Pause;

            ResetInput();

            GameMenu.singleton?.SetPauseMenu(input == InputState.Pause);
        }

        /// <summary>
        ///     Set player telekinesis script
        /// </summary>
        /// <param name="tl"></param>
        public void SetTelekinesis(Telekinesis tl) => _tl = tl;

        protected void MoveInput()
        {
            _inputMovement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Jump"), Input.GetAxis("Vertical"));
            _inputMouse = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        }

        protected void ResetInput() {
            _mover?.Move(Vector3.zero);
            _mover?.Rotate(Vector3.zero);
        }

        protected void Update()
        {
            // NOTE: in the build in the first frames wintracker doesn't exist
            if (input != InputState.Lock)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    SetPause();
                }

                if (input == InputState.Unlock)
                {
                    MoveInput();

                    _mover?.Move(_inputMovement);
                    _mover?.Rotate(_inputMouse);

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        _tl?.ApplyAbility();
                    }
                }
            }

#if DEVELOPMENT_BUILD
            if (Input.GetKeyDown(KeyCode.BackQuote))
                Cursor.lockState = CursorLockMode.None;
#endif
        }

        protected enum InputState 
        {
            Unlock,
            Pause,
            Lock
        }
    }
}
