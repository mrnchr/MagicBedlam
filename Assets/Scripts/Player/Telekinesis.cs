using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Mirror;

public class Telekinesis : NetworkBehaviour
{
    [Tooltip("The object taking distance")]
    [SyncVar] [SerializeField] private float _limitTelekinesis;
    [SyncVar] [SerializeField] private float _attractionTime;
    [SyncVar] [SerializeField] private float _throwingPower;
    [Tooltip("The object position relative to the camera")]
    [SerializeField] private Vector3 _tookObjectPosition;
    [Tooltip("The error rate of the object location relative to Took Object Position")]
    [SerializeField] private float _errorRate;
    [Tooltip("The maximum distance where the player aims if there are no other objects closer")]
    [SyncVar] [SerializeField] private float _viewDistance;
    [Tooltip("Time for which the player can not take objects after a throwing")]
    [SyncVar] [SerializeField] private float _coolDownTime;
    [SyncVar] [SerializeField] private LayerMask _movableMask;
    [SyncVar] [SerializeField] private LayerMask _allMasks;

    [SyncVar(hook = nameof(WhenChangeObject))] private MovableObject _movableObject;
    [SyncVar(hook = nameof(WhenChangeIsTook))] private bool _isTook;
    [SyncVar] private bool _toPlayer;
    [SyncVar] private bool _isCoolDown;

    private Rigidbody _objectRigidbody; // only on the server or the host
    private Transform _defaultParent;
    [SerializeField] private Mover _mover; // only on the server or the host
    [SerializeField] private NetworkAnimator _anim;

    public override void OnStartLocalPlayer()
    {
        Debug.Log("Telekinesis:OnStartLocalPlayer");
        InputManager.Instance.SetTelekinesis(this);
    }

    public override void OnStopLocalPlayer()
    {
        InputManager.Instance.SetTelekinesis(null);
    }

    [Command] 
    private void CmdApplyAbility() {
        if(!_movableObject)
            return;

        _isTook = true;

        _defaultParent = _movableObject.transform.parent;
        _movableObject.transform.SetParent(transform);
        _movableObject.owner = gameObject;
        _movableObject.gameObject.layer = 2;
        RpcApplyAbility();
        MagicTransition();
        _anim.SetTrigger("Take");

        _toPlayer = true;
        StartCoroutine(Flight());
    }

    [ClientRpc]
    private void RpcApplyAbility() {
        if(isClientOnly) {
            StartCoroutine(WaitApplyAbility());
        }
    }

    private IEnumerator WaitApplyAbility() {
        while (!_movableObject) {
            yield return null;
        }
        
        _defaultParent = _movableObject.transform.parent;
        _movableObject.transform.SetParent(transform);
    }

    public void ApplyAbility() {
        if(_isCoolDown || _toPlayer || !_movableObject) 
            return;

        if(_isTook)
            CmdThrowObject();
        else
            CmdApplyAbility();
    }

    private void WhenChangeIsTook(bool oldValue, bool newValue) {
        if(newValue && isLocalPlayer) {
            Flame(_movableObject, false);
        }
    }
    [ServerCallback]
    private void FixedUpdate() {
        CheckAim();
        if(_isTook) {
            StartCoroutine(Flight());
        }
    }

    // TODO: to check
    private void CheckAim() {
        if(!_isTook && !_isCoolDown) {
            RaycastHit hit;

            if(Physics.Raycast(_mover.FakeCamera.position, _mover.FakeCamera.forward, out hit, _limitTelekinesis, _movableMask)) {
                if((!_movableObject || _movableObject != hit.transform) && !hit.transform.GetComponent<MovableObject>().owner) {
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
        if(isLocalPlayer) {
            Flame(oldValue, false);
            Flame(newValue, true);
        }
    }
    
    private void Flame(MovableObject movableObj, bool highlighted) => movableObj?.Glow(highlighted);

    [Server]
    private void MagicTransition() {
        _objectRigidbody.useGravity = !_isTook;
    }

    [Command] 
    private void CmdEnableFlight() {
        StartCoroutine(Flight());
    }

    [Command] 
    private void CmdThrowObject() {
        if(!_movableObject)
            return;

        // direction of throw
        RaycastHit hit;
        Vector3 aimPos;

        aimPos = Physics.Raycast(_mover.FakeCamera.position, _mover.FakeCamera.forward, out hit, _viewDistance, _allMasks, QueryTriggerInteraction.Ignore) ? hit.point 
            : _mover.FakeCamera.position + _mover.FakeCamera.forward * _viewDistance;

        _isTook = false;
        _movableObject.transform.SetParent(_defaultParent);
        _movableObject.gameObject.layer = 6;

        RpcThrowObject();

        MagicTransition();
        _anim.SetTrigger("Throw");

        _objectRigidbody.AddForce((aimPos - _movableObject.transform.position).normalized * _throwingPower, ForceMode.Impulse);
        _movableObject.isThrowing = true;

        // recharge enabling
        _objectRigidbody = null;
        _defaultParent = null;
        _movableObject = null;
        StartCoroutine(CoolDown());
    }

    private IEnumerator WaitThrowObject() {
        while (!_movableObject) {
            yield return null;
        }
        
        _movableObject.transform.SetParent(_defaultParent);
        _defaultParent = null;
    }

    [Server]
    public void DropObject() {
        if(!_isTook) return;
        _isTook = false;
        _movableObject.transform.SetParent(_defaultParent);
        RpcThrowObject();

        MagicTransition();

        _movableObject.isThrowing = false;
        _movableObject.owner = null;

        // recharge enabling
        _objectRigidbody = null;
        _defaultParent = null;
        _movableObject = null;
    }

    [ClientRpc]
    private void RpcThrowObject() {
        if(isClientOnly) {
            StartCoroutine(WaitThrowObject());
        }

    }

    [Server]
    private IEnumerator Flight() {
        float procent = 0;

        while((_tookObjectPosition - _movableObject.transform.localPosition).sqrMagnitude > Mathf.Pow(_errorRate, 2))
        {
            _movableObject.transform.localPosition = Vector3.Lerp(_movableObject.transform.localPosition, _tookObjectPosition, procent);
            procent += 1 / _attractionTime * Time.fixedDeltaTime;
            yield return null;
            if(!_isTook)
                yield break;
        }

        _toPlayer = false;
    }

    private IEnumerator CoolDown() {
        _isCoolDown = true;
        yield return new WaitForSeconds(_coolDownTime);
        _isCoolDown = false;
    }
}
