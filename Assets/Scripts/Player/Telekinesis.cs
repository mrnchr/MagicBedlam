using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Mirror;

namespace MagicBedlam
{
    /// <summary>
    /// Telekinesis abitity
    /// </summary>
    public class Telekinesis : NetworkBehaviour
    {
        [Tooltip("The object taking distance")]
        [SerializeField] protected float _limitTelekinesis;
        [Tooltip("Time which the object flies to the player for")]
        [SerializeField] 
        [Range(0, 5)]
        protected float _attractionTime;
        [Tooltip("Power which the player throw the object with")]
        [SerializeField] 
        [Range(0, 20)]
        protected float _throwingPower;
        [Tooltip("The object position relative to the camera")]
        [SerializeField] 
        protected Vector3 _tookObjectPosition;
        [Tooltip("The maximum distance where the player aims if there are no other objects closer")]
        [SerializeField] 
        protected float _viewDistance;
        [Tooltip("Time for which the player can not take objects after a throwing")]
        [SerializeField] 
        protected float _coolDownTime;
        [Tooltip("Objects which can be moved by telekinesis")]
        [SerializeField] 
        protected LayerMask _movableMask;
        [Tooltip("Objects which can be threw other objects in")]
        [SerializeField] 
        protected LayerMask _allMasks;
        [Tooltip("Own Mover component")]
        [SerializeField] 
        protected Mover _mover;
        [Tooltip("Own Animator component")]
        [SerializeField] 
        protected NetworkAnimator _anim;
        [SerializeField]
        protected PlayerAudioSource _ownAudio;

        [SyncVar(hook = nameof(WhenChangeAbility))] protected Ability ability = Ability.Ready;
        [SyncVar(hook = nameof(WhenChangeObject))] protected MovableObject _movableObject;

        protected Transform _defaultParent;
        protected Rigidbody _objectRigidbody;

        public void ApplyAbility()
        {
            if (ability == Ability.CoolDown || ability == Ability.Apply || !_movableObject)
                return;

            if (ability == Ability.Hold)
                CmdThrowObject();
            else
                CmdApplyAbility();
        }

        [Server]
        public void DropObject()
        {
            if (ability == Ability.CoolDown || ability == Ability.Ready) return;
            _movableObject.transform.SetParent(_defaultParent);
            _movableObject.ChangeLayer(6);
            RpcThrowObject();

            MagicTransition(true);
            _anim.SetTrigger("Throw");

            _movableObject.isThrowing = false;
            _movableObject.owner = null;

            // recharge enabling
            _objectRigidbody = null;
            _defaultParent = null;
            _movableObject = null;
            
            ability = Ability.Ready;
        }

        [Server]
        protected void CheckAim()
        {
            if (ability == Ability.Ready)
            {
                RaycastHit hit;

                if (Physics.Raycast(_mover.FakeCamera.position, _mover.FakeCamera.forward, out hit, _limitTelekinesis, _movableMask))
                {
                    MovableObject movable = hit.transform.GetComponent<MovableObject>();
                    if ((!_movableObject || _movableObject != hit.transform) && !movable.owner)
                    {
                        _movableObject = movable;
                        _objectRigidbody = hit.rigidbody;
                    }
                }
                else
                {
                    _movableObject = null;
                    _objectRigidbody = null;
                }
            }
        }

        [Command]
        protected void CmdApplyAbility()
        {
            if (!_movableObject)
                return;

            ability = Ability.Apply;

            _defaultParent = _movableObject.transform.parent;
            _movableObject.transform.SetParent(transform);
            _movableObject.owner = gameObject;

            // NOTE: changing layer is necessary to when throwing the object not to aim at it
            _movableObject.ChangeLayer(2);
            RpcApplyAbility();
            MagicTransition(false);
            _anim.SetTrigger("Take");
            _ownAudio.RpcPlay(PlayerSound.Pick);

            StartCoroutine(Flight());
        }

        [Command]
        protected void CmdThrowObject()
        {
            // if client called command before several times in a row object
            // may be took for the client but not for the server
            if (ability != Ability.Hold)
                return;

            // direction of throw
            RaycastHit hit;
            Vector3 aimPos;

            aimPos = Physics.Raycast(_mover.FakeCamera.position, _mover.FakeCamera.forward, out hit, _viewDistance, _allMasks, QueryTriggerInteraction.Ignore) ? hit.point
                : _mover.FakeCamera.position + _mover.FakeCamera.forward * _viewDistance;

            _movableObject.transform.SetParent(_defaultParent);
            _movableObject.ChangeLayer(6);

            RpcThrowObject();

            MagicTransition(true);
            _anim.SetTrigger("Throw");
            _ownAudio.RpcPlay(PlayerSound.Throw);

            _objectRigidbody.AddForce((aimPos - _movableObject.transform.position).normalized * _throwingPower, ForceMode.Impulse);
            _movableObject.isThrowing = true;

            // recharge enabling
            _objectRigidbody = null;
            _defaultParent = null;
            _movableObject = null;
            StartCoroutine(CoolDown());
        }

        [Server]
        protected IEnumerator CoolDown()
        {
            ability = Ability.CoolDown;
            yield return new WaitForSeconds(_coolDownTime);
            ability = Ability.Ready;
        }

        [Server]
        protected IEnumerator Flight()
        {
            float procent = 0;
            Vector3 startPos = _movableObject.transform.localPosition;

            while ((_movableObject.transform.localPosition - _tookObjectPosition).sqrMagnitude > Mathf.Pow(0.01f, 2))
            {
                _movableObject.transform.localPosition = Vector3.Lerp(startPos, _tookObjectPosition, procent);
                procent += 1 / _attractionTime * Time.deltaTime;
                yield return null;
                if (ability != Ability.Apply)
                    yield break;
            }

            ability = Ability.Hold;
        }

        [Server]
        protected void MagicTransition(bool hasPhysics)
        {
            _objectRigidbody.useGravity = hasPhysics;
            _objectRigidbody.isKinematic = !hasPhysics;
        }

        public override void OnStartLocalPlayer()
        {
            Debug.Log("Telekinesis:OnStartLocalPlayer");
            InputManager.singleton.SetTelekinesis(this);
        }

        public override void OnStopLocalPlayer()
        {
            InputManager.singleton.SetTelekinesis(null);
        }

        public override void OnStartServer()
        {
            StartCoroutine(WaitForCheckAim());
        }

        [ClientRpc]
        protected void RpcApplyAbility()
        {
            if (isClientOnly)
            {
                StartCoroutine(WaitApplyAbility());
            }
        }

        [ClientRpc]
        protected void RpcThrowObject()
        {
            if (isClientOnly)
            {
                StartCoroutine(WaitThrowObject());
            }
        }

        protected IEnumerator WaitApplyAbility()
        {
            while (!_movableObject)
            {
                yield return null;
            }

            _defaultParent = _movableObject.transform.parent;
            _movableObject.transform.SetParent(transform);
        }

        [Server]
        protected IEnumerator WaitForCheckAim()
        {
            while (NetworkServer.active)
            {
                CheckAim();
                yield return new WaitForSeconds(0.1f);
            }
        }
        
        protected IEnumerator WaitThrowObject()
        {
            while (!_movableObject)
            {
                yield return null;
            }

            _movableObject.transform.SetParent(_defaultParent);
            _defaultParent = null;
        }

        protected void WhenChangeAbility(Ability oldValue, Ability newValue)
        {
            if (newValue == Ability.Apply && isLocalPlayer)
            {
                _movableObject.Glow(false);
            }
        }

        protected void WhenChangeObject(MovableObject oldValue, MovableObject newValue)
        {
            if (isLocalPlayer)
            {
                oldValue?.Glow(false);
                newValue?.Glow(true);
            }
        }

        protected void Reset() {
            TryGetComponent<Mover>(out _mover);
            TryGetComponent<NetworkAnimator>(out _anim);
        }

        protected enum Ability
        {
            CoolDown,
            Ready,
            Apply,
            Hold
        }
    }
}
