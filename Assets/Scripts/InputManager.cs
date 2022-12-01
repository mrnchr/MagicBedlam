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
    private Player _pl;
    private Telekinesis _tl;
    private bool _isMoving;

    public void SetPlayer(Player pl) => _pl = pl;
    public void SetTelekinesis(Telekinesis tl) => _tl = tl;

    private void Awake() {
        if(_instance != null) {
            Debug.LogError("Two singleton. The second one will be destroyed");
            Destroy(gameObject);
            return;
        }
        _instance = this;

        _isMoving = true;
    }

    public void SetPause() {
        _isMoving = !_isMoving;
        GameMenu.Instance.SetPause(!_isMoving);
    }

    private void Update() {
        if(!GameManager.Instance.EndOfGame) {
            if(Input.GetKeyDown(KeyCode.Escape)) {
                SetPause();
            }

            if(_isMoving) {
                MoveInput();

                _pl?.CmdMove(_inputMovement);
                _pl?.Rotate(_inputMouse);
                
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
