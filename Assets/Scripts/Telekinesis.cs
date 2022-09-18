using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Telekinesis : MonoBehaviour
{
    [SerializeField] private float _limitTelekinesis;
    [SerializeField] private float _trowingPower;
    [SerializeField] private Vector3 _tookObjectPosition;
    [SerializeField] private float _errorRate;
    [SerializeField] private GameObject _camera;

    private MovedObject _movedObject;
    private Transform _defaultParent;
    private Rigidbody _objectRigidbody;
    private bool _isTook;
    private bool _toPlayer;

    private void Start()
    {
        _movedObject = null;
        _isTook = false;
        _toPlayer = false;
    }

    private void Update()
    {
        RaycastHit hit;

        if (!_isTook)
        {
            if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out hit))
            {
                if (hit.transform.tag == "MovedObject" && Vector3.Distance(hit.transform.position, transform.position) <= _limitTelekinesis)
                {
                    if (!_movedObject || _movedObject.transform != hit.transform)
                    {
                        Flame(false);

                        _movedObject = hit.transform.GetComponent<MovedObject>();
                        Flame(true);
                    }

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        Take(hit);
                    }
                }
                else // any another object
                {
                    Flame(false);
                }
            }
            else // no object
            {
                Flame(false);          
            }
        }

        if (_toPlayer)
        {
            StartCoroutine(Flight());
        }

        if(_isTook && !_toPlayer && Input.GetKeyDown(KeyCode.E))
        {
            Throw();
        }
    }

    private void Flame(bool highlighted)
    {
        if(_movedObject)
            _movedObject.Glow(highlighted);
    }

    private void Take(RaycastHit hit)
    {
        _isTook = true;
        _toPlayer = true;

        Flame(false);
        _defaultParent = _movedObject.transform.parent;
        _movedObject.transform.SetParent(_camera.transform);
        _objectRigidbody = hit.rigidbody;

        MagicTransition();

    }

    private void MagicTransition()
    {
        _objectRigidbody.isKinematic = _isTook;
        _objectRigidbody.useGravity = !_isTook;
    }

    private void Throw()
    {
        _isTook = false;

        _movedObject.transform.SetParent(_defaultParent);

        MagicTransition();

        _objectRigidbody.AddForce(transform.forward * _trowingPower, ForceMode.Impulse);

        _defaultParent = null;
        _objectRigidbody = null;
    }

    private IEnumerator Flight()
    {
        Vector3 endPos = _tookObjectPosition;
        Vector3 startPos = _movedObject.transform.localPosition;

        while(Vector3.Distance(_tookObjectPosition, _movedObject.transform.localPosition) > _errorRate)
        {
            _movedObject.transform.localPosition += (endPos - startPos) / 2 * Time.fixedDeltaTime;
            yield return null;
        }

        _toPlayer = false;
    }
}
