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

    private void Awake() {
        _instance = this;
    }
    #endregion

    private Vector3 _inputMovement;
    private Vector2 _inputMouse;
    private Player _pl;

    public void SetPlayer(Player pl) => _pl = pl;

    private void FixedUpdate() {
        MoveInput();

        if(_pl) {
            _pl.Move(_inputMovement);
            _pl.Rotate(_inputMouse);
        }
    }

    private void MoveInput() {
        _inputMovement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        _inputMouse = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }
}
