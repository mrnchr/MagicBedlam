using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    #region Singleton
    static private InputManager _instance;

    static public InputManager Instance {
        get {
            return _instance;
        }
    }
    #endregion

    private Vector3 _inputMovement;
    private Vector2 _inputMouse;
    private Mover _mover;
    private Telekinesis _tl;
    private bool _isPause;

    public void SetMover(Mover mover) => _mover = mover;
    public void SetTelekinesis(Telekinesis tl) => _tl = tl;

    private void Awake() {
        if(_instance != null) {
            Debug.LogError("Two singleton. The second one will be destroyed");
            Destroy(gameObject);
            return;
        }
        _instance = this;

        _isPause = false;
    }

    public void SetPause() {
        _isPause = !_isPause;
        GameMenu.Instance.SetPause(_isPause);
    }

    private void Update() {
        if(!WinTracker.Instance.EndOfGame) {
            if(Input.GetKeyDown(KeyCode.Escape)) {
                SetPause();
            }

            if(!_isPause) {
                MoveInput();

                _mover?.CmdMove(_inputMovement);
                _mover?.Rotate(_inputMouse);
                
                if(Input.GetKeyDown(KeyCode.E)) {
                    _tl?.ApplyAbility();
                }
            }
        }
    }

    private void MoveInput() {
        _inputMovement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Jump"), Input.GetAxis("Vertical"));
        _inputMouse = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }
}
