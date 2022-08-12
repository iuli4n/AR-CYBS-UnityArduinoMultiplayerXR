using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazePointerModel : MonoBehaviourPunCallbacks, IPunObservable
{
    // this code stores the values for the starting position and the direction of gaze pointer

    public delegate void UpdatePose();
    public event UpdatePose onPositionUpdate;
    public event UpdatePose onDirectionUpdate;

    public delegate void UpdateView();
    public event UpdateView onViewUpdate;

    public Vector3 _startPosition;
    public Quaternion _direction;
    public int _viewType;



    #region IPunObservable implementation

    public void Awake()
    {
        onPositionUpdate += () => { if (PhotonSimulatorIuli.Instance.USING_REAL_PHOTON ? photonView.IsMine : true) needsSync = true; };
        onDirectionUpdate += () => { if (PhotonSimulatorIuli.Instance.USING_REAL_PHOTON ? photonView.IsMine : true) needsSync = true; };
        onViewUpdate += () => { if (PhotonSimulatorIuli.Instance.USING_REAL_PHOTON ? photonView.IsMine : true) needsSync = true; };
    }

    private bool needsSync = false;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // called both when reading or writing

        if (stream.IsWriting)
        {
            // WRITING: we own this player so we send out this data : send the others our data

            //if (needsSync)
            {
                stream.SendNext(_startPosition);
                stream.SendNext(_direction);
                stream.SendNext(_viewType);

                needsSync = false;
            }
        }
        else
        {
            // READING from network, receive data from another client

            Vector3 oldStartPos = _startPosition;
            Quaternion oldDirection = _direction;
            int oldViewType = _viewType;

            _startPosition = (Vector3)stream.ReceiveNext();
            _direction = (Quaternion)stream.ReceiveNext();
            _viewType = (int)stream.ReceiveNext();

            if (oldStartPos != _startPosition)
                onPositionUpdate?.Invoke();

            if (oldDirection != _direction)
                onDirectionUpdate?.Invoke();

            if (oldViewType != _viewType)
                onViewUpdate?.Invoke();

        }

    }


    #endregion //NetworkedModel



    public int ViewType
    {
        get
        {
            return _viewType;
        }

        set
        {
            if (_viewType != value)
            {
                _viewType = value;

                if (onViewUpdate != null)
                {
                    onViewUpdate();
                }
            }

        }
    }

    public Vector3 StartPos
    {
        get
        {
            return _startPosition;
        }

        set
        {
            if (_startPosition != value)
            {
                _startPosition = value;

                if (onPositionUpdate != null)
                {
                    onPositionUpdate();
                }
            }

        }
    }

    public Quaternion Direction
    {
        get
        {
            return _direction;
        }

        set
        {
            if (_direction != value)
            {
                _direction = value;

                if (onPositionUpdate != null)
                {
                    onDirectionUpdate();
                }
            }

        }
    }


}
