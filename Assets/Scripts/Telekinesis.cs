using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Mirror;

public class Telekinesis : NetworkBehaviour
{
    [Tooltip("The object taking distance")]
    [SyncVar] [SerializeField] private float _limitTelekinesis;
    [SyncVar] [SerializeField] private float _throwingPower;
    [Tooltip("The object position relative to the camera")]
    [SyncVar] [SerializeField] private Vector3 _tookObjectPosition;
    [Tooltip("The error rate of the object location relative to Took Object Position")]
    [SyncVar] [SerializeField] private float _errorRate;
    [Tooltip("The maximum distance where the player aims if there are no other objects closer")]
    [SyncVar] [SerializeField] private float _viewDistance;
    [Tooltip("Time for which the player can not take objects after a throwing")]
    [SyncVar] [SerializeField] private float _coolDownTime;
    [SyncVar] [SerializeField] private LayerMask _movableMask;

    [SyncVar(hook = nameof(WhenChangeObject))] private MovableObject _movableObject;
    [SyncVar(hook = nameof(WhenChangeIsTook))] private bool _isTook;
    [SyncVar(hook = nameof(WhenChangeToPlayer))] private bool _toPlayer;
    [SyncVar] private bool _isCoolDown;

    private Rigidbody _objectRigidbody; // only on the server or the host
    private Transform _defaultParent; // only on the server or the host
    [SerializeField] private Player _player; // only on the server or the host

    public override void OnStartLocalPlayer()
    {
        Debug.Log("Telekinesis:OnStartLocalPlayer");
        InputManager.Instance.SetTelekinesis(this);
    }

    [Command] 
    private void CmdApplyAbility() {
        _isTook = true;

        _defaultParent = _movableObject.transform.parent;
        _movableObject.transform.SetParent(_player.FakeCamera);
        //_movableObject.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);
        Debug.Log(_movableObject.GetComponent<NetworkIdentity>().connectionToClient);
        MagicTransition();

        _toPlayer = true;
        StartCoroutine(Flight());
    }

    public void ApplyAbility() {
        if(_isCoolDown) 
            return;

        if(_movableObject && !_isTook) {
            CmdApplyAbility();
        }
        else if(_isTook && !_toPlayer) {
            ThrowObject();
        }
    }

    private void WhenChangeIsTook(bool oldValue, bool newValue) {
        if(isClientOnly && isLocalPlayer) {
            if(newValue) {
                Flame(_movableObject, false);
                _defaultParent = _movableObject.transform.parent;
                _movableObject.transform.SetParent(_player.FakeCamera);
            }
            else {
                _movableObject.transform.SetParent(_defaultParent);
            }
        }
    }

    private void WhenChangeToPlayer(bool oldValue, bool newValue) {

    }

    private void FixedUpdate() {
        if(isLocalPlayer) {
            CmdCheckAim(Spawner.Instance.Camera.position, Spawner.Instance.Camera.forward);
        }
    }

    // TODO: to check
    [Command]
    private void CmdCheckAim(Vector3 raycastPos, Vector3 raycastDir) {
        if(!_isTook && !_isCoolDown) {
            RaycastHit hit;

            if(Physics.Raycast(raycastPos, raycastDir, out hit, _limitTelekinesis, _movableMask)) {
                if(!_movableObject || _movableObject != hit.transform) {
                    _movableObject = hit.transform.GetComponent<MovableObject>();
                    _objectRigidbody = hit.rigidbody;
                }
            }
            else {
                _movableObject = null;
                _objectRigidbody = null;
            }
        }
    }

    private void WhenChangeObject(MovableObject oldValue, MovableObject newValue) {
        Flame(oldValue, false);
        Flame(newValue, true);
    }

    /*
    // !!!FIX: Raycast does on the client and changes _movableObject. It allowed for user to change 
    // object by himself
    private void CheckAim() {
        if(!_isTook && !_isCoolDown) {
            RaycastHit hit;

            if(Physics.Raycast(Spawner.Instance.Camera.position, Spawner.Instance.Camera.forward, out hit, _limitTelekinesis, _movableMask)) {
                if(!_movableObject || _movableObject.transform != hit.transform) {
                    _movableObject = hit.transform.GetComponent<MovableObject>();
                    _objectRigidbody = hit.rigidbody;
                }
            }
            else {
                Debug.Log("I don't see it");
                _movableObject = null;
                _objectRigidbody = null;
            }
        }
    }*/


    private void Flame(MovableObject movableObj, bool highlighted) => movableObj?.Glow(highlighted);

    [Command]
    private void CmdMagicTransition() {
        Debug.Log(_movableObject.GetComponent<NetworkIdentity>().connectionToClient);
        _objectRigidbody.isKinematic = _isTook;
        _objectRigidbody.useGravity = !_isTook;
    }

    private void MagicTransition() { // only on the server
        Debug.Log(_movableObject.GetComponent<NetworkIdentity>().connectionToClient);
        _objectRigidbody.isKinematic = _isTook;
        _objectRigidbody.useGravity = !_isTook;
    }

    [Command] 
    private void CmdEnableFlight() {
        StartCoroutine(Flight());
    }

    [Command] 
    private void CmdThrowObject() {
        _isTook = false;
        _movableObject.transform.SetParent(_defaultParent);

        MagicTransition();

        // direction of throw
        RaycastHit hit;
        Vector3 aimPos;

        aimPos = Physics.Raycast(_player.FakeCamera.position, _player.FakeCamera.forward, out hit, _viewDistance, LayerMask.GetMask(), QueryTriggerInteraction.Ignore) ? hit.point 
            : _player.FakeCamera.position + _player.FakeCamera.forward * _viewDistance;
        
        Debug.Log(hit.transform);
        Debug.Log(_player.FakeCamera.position);
        Debug.Log(_player.FakeCamera.forward);
        Debug.Log(_player.FakeCamera.forward * _viewDistance);
        Debug.Log(aimPos);

        _objectRigidbody.AddForce((aimPos - _movableObject.transform.position).normalized * _throwingPower, ForceMode.Impulse);

        //_movableObject.GetComponent<NetworkIdentity>().RemoveClientAuthority();

        // recharge enabling
        _isCoolDown = true;
        StartCoroutine(CoolDown());
        _objectRigidbody = null;
        _defaultParent = null;
    }

    private void ThrowObject() {
        CmdThrowObject();
        _defaultParent = null;
    }

    private IEnumerator Flight() { // only the server or the host
        Debug.Log("i fly");
        Debug.Log(isServer);
        Vector3 endPos = _tookObjectPosition;
        Vector3 startPos = _movableObject.transform.localPosition;
        Debug.Log(endPos);
        Debug.Log(startPos);
        while((_tookObjectPosition - _movableObject.transform.localPosition).sqrMagnitude > Mathf.Pow(_errorRate, 2))
        {
            _movableObject.transform.localPosition += (endPos - startPos) / 2 * Time.fixedDeltaTime;
            Debug.Log(_movableObject);
            yield return null;
        }

        _toPlayer = false;
    }

    private IEnumerator CoolDown() {
        yield return new WaitForSeconds(_coolDownTime);
        _isCoolDown = false;
    }
}
