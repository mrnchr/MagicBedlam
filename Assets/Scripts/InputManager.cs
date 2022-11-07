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
    private Telekinesis _tl;

    public void SetPlayer(Player pl) => _pl = pl;
    public void SetTelekinesis(Telekinesis tl) => _tl = tl;

    private bool EnableAbility() => Input.GetKeyDown(KeyCode.E);

    private void Update() {
        MoveInput();

        if(_pl) {
            _pl.CmdMove(_inputMovement);
            _pl.Rotate(_inputMouse);
            
            if(EnableAbility()) {
                _tl.ApplyAbility();
            }
        }
    }

    private void MoveInput() {
        _inputMovement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Jump"), Input.GetAxis("Vertical"));
        _inputMouse = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }
}
