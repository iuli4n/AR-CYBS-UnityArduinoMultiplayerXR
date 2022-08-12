using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointsModelBothHandsNetworking: MonoBehaviour, IPunObservable
{
    public delegate void UpdatePose();
    public event UpdatePose onPoseUpdate;

    private Vector3 _wristPos;
    private Quaternion _wristRot,
    _indexDistalJoint, _indexKnuckle, _indexMiddleJoint, 
    _middleDistalJoint, _middleKnuckle, _middleMiddleJoint, 
    _pinkyDistalJoint, _pinkyKnuckle, _pinkyMiddleJoint,
    _ringDistalJoint, _ringKnuckle, _ringMiddleJoint,
    _thumbDistalJoint, _thumbMetacarpalJoint, _thumbProximalJoint;



    private bool poseUpdatesEnabled = true;
    private bool needPoseUpdate = false;
    // Use this whenever pose updates were made and may need to be broadcast
    private void PoseWasUpdated()
    {
        needPoseUpdate = true;

        if (poseUpdatesEnabled)
        {
            onPoseUpdate?.Invoke();
            needsNetworkSync = true;

            needPoseUpdate = false;
        }
    }
    // After calling this, all onPoseUpdate calls will be suppressed
    public void DisablePoseUpdates()
    {
        poseUpdatesEnabled = false;
    }
    // After calling this, updates get notified via onPoseUpdate (and sends an event if the update was changed since disabling)
    public void EnablePoseUpdates()
    {
        poseUpdatesEnabled = true;
        if (needPoseUpdate) PoseWasUpdated();
    }


    private bool needsNetworkSync = false;

    #region IPunObservable implementation



    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // called both when reading or writing

        if (stream.IsWriting)
        {
            // WRITING: we own this player so we send out this data : send the others our data

            if (needsNetworkSync)
            {
                stream.SendNext(_wristPos);
                stream.SendNext(_wristRot);

                stream.SendNext(_indexDistalJoint);
                stream.SendNext(_indexKnuckle);
                stream.SendNext(_indexMiddleJoint);
                stream.SendNext(_middleDistalJoint);
                stream.SendNext(_middleKnuckle);
                stream.SendNext(_middleMiddleJoint);
                stream.SendNext(_pinkyDistalJoint);
                stream.SendNext(_pinkyKnuckle);
                stream.SendNext(_pinkyMiddleJoint);
                stream.SendNext(_ringDistalJoint);
                stream.SendNext(_ringKnuckle);
                stream.SendNext(_ringMiddleJoint);
                stream.SendNext(_thumbDistalJoint);
                stream.SendNext(_thumbMetacarpalJoint);
                stream.SendNext(_thumbProximalJoint);

                needsNetworkSync = false;
            }
        }
        else
        {
            // READING from network, receive data from another client
            _wristPos = (Vector3)stream.ReceiveNext();
            _wristRot = (Quaternion)stream.ReceiveNext();

            _indexDistalJoint = (Quaternion)stream.ReceiveNext();
            _indexKnuckle = (Quaternion)stream.ReceiveNext();
            _indexMiddleJoint = (Quaternion)stream.ReceiveNext();
            _middleDistalJoint = (Quaternion)stream.ReceiveNext();
            _middleKnuckle = (Quaternion)stream.ReceiveNext();
            _middleMiddleJoint = (Quaternion)stream.ReceiveNext();
            _pinkyDistalJoint = (Quaternion)stream.ReceiveNext();
            _pinkyKnuckle = (Quaternion)stream.ReceiveNext();
            _pinkyMiddleJoint = (Quaternion)stream.ReceiveNext();
            _ringDistalJoint = (Quaternion)stream.ReceiveNext();
            _ringKnuckle = (Quaternion)stream.ReceiveNext();
            _ringMiddleJoint = (Quaternion)stream.ReceiveNext();
            _thumbDistalJoint = (Quaternion)stream.ReceiveNext();
            _thumbMetacarpalJoint = (Quaternion)stream.ReceiveNext();
            _thumbProximalJoint = (Quaternion)stream.ReceiveNext();

            onPoseUpdate?.Invoke();
        }

    }

    #endregion //NetworkedModel



    public Quaternion _IndexDistalJoint
    {
        get
        {
            return _indexDistalJoint;
        }

        set
        {
            if (_indexDistalJoint != value)
            {
                _indexDistalJoint = value;
                PoseWasUpdated();
            }

        }
    }

    public Quaternion _IndexKnuckle
    {
        get
        {
            return _indexKnuckle;
        }

        set
        {
            if (_indexKnuckle != value)
            {
                _indexKnuckle = value;
                PoseWasUpdated();
            }

        }
    }

    public Quaternion _IndexMiddleJoint
    {
        get
        {
            return _indexMiddleJoint;
        }

        set
        {
            if (_indexMiddleJoint != value)
            {
                _indexMiddleJoint = value;
                PoseWasUpdated();
            }

        }
    }

    public Quaternion _MiddleDistalJoint
    {
        get
        {
            return _middleDistalJoint;
        }

        set
        {
            if (_middleDistalJoint != value)
            {
                _middleDistalJoint = value;
                PoseWasUpdated();
            }

        }
    }

    public Quaternion _MiddleKnuckle
    {
        get
        {
            return _middleKnuckle;
        }

        set
        {
            if (_middleKnuckle != value)
            {
                _middleKnuckle = value;
                PoseWasUpdated();
            }

        }
    }

    public Quaternion _MiddleMiddleJoint
    {
        get
        {
            return _middleMiddleJoint;
        }

        set
        {
            if (_middleMiddleJoint != value)
            {
                _middleMiddleJoint = value;
                PoseWasUpdated();
            }

        }
    }

    public Quaternion _PinkyDistalJoint
    {
        get
        {
            return _pinkyDistalJoint;
        }

        set
        {
            if (_pinkyDistalJoint != value)
            {
                _pinkyDistalJoint = value;
                PoseWasUpdated();
            }

        }
    }

    public Quaternion _PinkyKnuckle
    {
        get
        {
            return _pinkyKnuckle;
        }

        set
        {
            if (_pinkyKnuckle != value)
            {
                _pinkyKnuckle = value;
                PoseWasUpdated();
            }

        }
    }

    public Quaternion _PinkyMiddleJoint
    {
        get
        {
            return _pinkyMiddleJoint;
        }

        set
        {
            if (_pinkyMiddleJoint != value)
            {
                _pinkyMiddleJoint = value;
                PoseWasUpdated();
            }

        }
    }

    public Quaternion _RingDistalJoint
    {
        get
        {
            return _ringDistalJoint;
        }

        set
        {
            if (_ringDistalJoint != value)
            {
                _ringDistalJoint = value;
                PoseWasUpdated();
            }

        }
    }

    public Quaternion _RingKnuckle
    {
        get
        {
            return _ringKnuckle;
        }

        set
        {
            if (_ringKnuckle != value)
            {
                _ringKnuckle = value;
                PoseWasUpdated();
            }

        }
    }

    public Quaternion _RingMiddleJoint
    {
        get
        {
            return _ringMiddleJoint;
        }

        set
        {
            if (_ringMiddleJoint != value)
            {
                _ringMiddleJoint = value;
                PoseWasUpdated();
            }

        }
    }

    public Quaternion _ThumbDistalJoint
    {
        get
        {
            return _thumbDistalJoint;
        }

        set
        {
            if (_thumbDistalJoint != value)
            {
                _thumbDistalJoint = value;
                PoseWasUpdated();
            }

        }
    }

    public Quaternion _ThumbMetacarpalJoint
    {
        get
        {
            return _thumbMetacarpalJoint;
        }

        set
        {
            if (_thumbMetacarpalJoint != value)
            {
                _thumbMetacarpalJoint = value;
                PoseWasUpdated();
            }

        }
    }

    public Quaternion _ThumbProximalJoint
    {
        get
        {
            return _thumbProximalJoint;
        }

        set
        {
            if (_thumbProximalJoint != value)
            {
                _thumbProximalJoint = value;
                PoseWasUpdated();
            }

        }
    }

    public Quaternion _WristRot
    {
        get
        {
            return _wristRot;
        }

        set
        {
            if (_wristRot != value)
            {
                _wristRot = value;
                PoseWasUpdated();
            }

        }
    }

    public Vector3 _WristPos
    {
        get
        {
            return _wristPos;
        }

        set
        {
            if (_wristPos != value)
            {
                _wristPos = value;
                PoseWasUpdated();
            }

        }
    }


    void Awake()
    {
        _IndexDistalJoint = new Quaternion(0, 0, 0, 0);
        _IndexKnuckle = new Quaternion(0, 0, 0, 0);
        _IndexMiddleJoint = new Quaternion(0, 0, 0, 0);

        _MiddleDistalJoint = new Quaternion(0, 0, 0, 0);
        _MiddleKnuckle = new Quaternion(0, 0, 0, 0);
        _MiddleMiddleJoint = new Quaternion(0, 0, 0, 0);


        _PinkyDistalJoint = new Quaternion(0, 0, 0, 0);
        _PinkyKnuckle = new Quaternion(0, 0, 0, 0);
        _PinkyMiddleJoint = new Quaternion(0, 0, 0, 0);

        _RingDistalJoint = new Quaternion(0, 0, 0, 0);
        _RingKnuckle = new Quaternion(0, 0, 0, 0);
        _RingMiddleJoint = new Quaternion(0, 0, 0, 0);

        _ThumbDistalJoint = new Quaternion(0, 0, 0, 0);
        _ThumbMetacarpalJoint = new Quaternion(0, 0, 0, 0);
        _ThumbProximalJoint = new Quaternion(0, 0, 0, 0);

        _WristRot = new Quaternion(0, 0, 0, 0);
        _WristPos = new Vector3(0, 0, 0);

    }



    /**** OLDER
    public Quaternion _IndexDistalJoint
    {
        get
        {
            return _indexDistalJoint;
        }

        set
        {
            if (_indexDistalJoint != value)
            {
                _indexDistalJoint = value;

                if (onPoseUpdate != null)
                {
                    onPoseUpdate();
                }
            }

        }
    }

    public Quaternion _IndexKnuckle
    {
        get
        {
            return _indexKnuckle;
        }

        set
        {
            if (_indexKnuckle != value)
            {
                _indexKnuckle = value;

                if (onPoseUpdate != null)
                {
                    onPoseUpdate();
                }
            }

        }
    }

    public Quaternion _IndexMiddleJoint
    {
        get
        {
            return _indexMiddleJoint;
        }

        set
        {
            if (_indexMiddleJoint != value)
            {
                _indexMiddleJoint = value;

                if (onPoseUpdate != null)
                {
                    onPoseUpdate();
                }
            }

        }
    }

    public Quaternion _MiddleDistalJoint
    {
        get
        {
            return _middleDistalJoint;
        }

        set
        {
            if (_middleDistalJoint != value)
            {
                _middleDistalJoint = value;

                if (onPoseUpdate != null)
                {
                    onPoseUpdate();
                }
            }

        }
    }

    public Quaternion _MiddleKnuckle
    {
        get
        {
            return _middleKnuckle;
        }

        set
        {
            if (_middleKnuckle != value)
            {
                _middleKnuckle = value;

                if (onPoseUpdate != null)
                {
                    onPoseUpdate();
                }
            }

        }
    }

    public Quaternion _MiddleMiddleJoint
    {
        get
        {
            return _middleMiddleJoint;
        }

        set
        {
            if (_middleMiddleJoint != value)
            {
                _middleMiddleJoint = value;

                if (onPoseUpdate != null)
                {
                    onPoseUpdate();
                }
            }

        }
    }

    public Quaternion _PinkyDistalJoint
    {
        get
        {
            return _pinkyDistalJoint;
        }

        set
        {
            if (_pinkyDistalJoint != value)
            {
                _pinkyDistalJoint = value;

                if (onPoseUpdate != null)
                {
                    onPoseUpdate();
                }
            }

        }
    }

    public Quaternion _PinkyKnuckle
    {
        get
        {
            return _pinkyKnuckle;
        }

        set
        {
            if (_pinkyKnuckle != value)
            {
                _pinkyKnuckle = value;

                if (onPoseUpdate != null)
                {
                    onPoseUpdate();
                }
            }

        }
    }

    public Quaternion _PinkyMiddleJoint
    {
        get
        {
            return _pinkyMiddleJoint;
        }

        set
        {
            if (_pinkyMiddleJoint != value)
            {
                _pinkyMiddleJoint = value;

                if (onPoseUpdate != null)
                {
                    onPoseUpdate();
                }
            }

        }
    }

    public Quaternion _RingDistalJoint
    {
        get
        {
            return _ringDistalJoint;
        }

        set
        {
            if (_ringDistalJoint != value)
            {
                _ringDistalJoint = value;

                if (onPoseUpdate != null)
                {
                    onPoseUpdate();
                }
            }

        }
    }

    public Quaternion _RingKnuckle
    {
        get
        {
            return _ringKnuckle;
        }

        set
        {
            if (_ringKnuckle != value)
            {
                _ringKnuckle = value;

                if (onPoseUpdate != null)
                {
                    onPoseUpdate();
                }
            }

        }
    }

    public Quaternion _RingMiddleJoint
    {
        get
        {
            return _ringMiddleJoint;
        }

        set
        {
            if (_ringMiddleJoint != value)
            {
                _ringMiddleJoint = value;

                if (onPoseUpdate != null)
                {
                    onPoseUpdate();
                }
            }

        }
    }

    public Quaternion _ThumbDistalJoint
    {
        get
        {
            return _thumbDistalJoint;
        }

        set
        {
            if (_thumbDistalJoint != value)
            {
                _thumbDistalJoint = value;

                if (onPoseUpdate != null)
                {
                    onPoseUpdate();
                }
            }

        }
    }

    public Quaternion _ThumbMetacarpalJoint
    {
        get
        {
            return _thumbMetacarpalJoint;
        }

        set
        {
            if (_thumbMetacarpalJoint != value)
            {
                _thumbMetacarpalJoint = value;

                PoseWasUpdated();
            }

        }
    }

    public Quaternion _ThumbProximalJoint
    {
        get
        {
            return _thumbProximalJoint;
        }

        set
        {
            if (_thumbProximalJoint != value)
            {
                _thumbProximalJoint = value;

                if (onPoseUpdate != null)
                {
                    onPoseUpdate();
                }
            }

        }
    }

    public Quaternion _WristRot
    {
        get
        {
            return _wristRot;
        }

        set
        {
            if (_wristRot != value)
            {
                _wristRot = value;

                if (onPoseUpdate != null)
                {
                    onPoseUpdate();
                }
            }

        }
    }

    public Vector3 _WristPos
    {
        get
        {
            return _wristPos;
        }

        set
        {
            if (_wristPos != value)
            {
                _wristPos = value;

                if (onPoseUpdate != null)
                {
                    onPoseUpdate();
                }
            }

        }
    }


    void Awake()
    {
        //onPoseUpdate += () => { if (photonView.IsMine) needsSync = true; };
        //Error CS0103  The name 'photonView' does not exist in the current context

                 _IndexDistalJoint = new Quaternion(0, 0, 0, 0);
        _IndexKnuckle = new Quaternion(0, 0, 0, 0);
        _IndexMiddleJoint = new Quaternion(0, 0, 0, 0);

        _MiddleDistalJoint = new Quaternion(0, 0, 0, 0);
        _MiddleKnuckle = new Quaternion(0, 0, 0, 0);
        _MiddleMiddleJoint = new Quaternion(0, 0, 0, 0);


        _PinkyDistalJoint = new Quaternion(0, 0, 0, 0);
        _PinkyKnuckle = new Quaternion(0, 0, 0, 0);
        _PinkyMiddleJoint = new Quaternion(0, 0, 0, 0);

        _RingDistalJoint = new Quaternion(0, 0, 0, 0);
        _RingKnuckle = new Quaternion(0, 0, 0, 0);
        _RingMiddleJoint = new Quaternion(0, 0, 0, 0);

        _ThumbDistalJoint = new Quaternion(0, 0, 0, 0);
        _ThumbMetacarpalJoint = new Quaternion(0, 0, 0, 0);
        _ThumbProximalJoint = new Quaternion(0, 0, 0, 0);

        _WristRot = new Quaternion(0, 0, 0, 0);
        _WristPos = new Vector3(0, 0, 0);
     
    }


    ***/

}
