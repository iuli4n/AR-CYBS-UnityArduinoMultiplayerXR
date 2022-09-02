using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // if we collide with something it might be a player object that's within a hierarchy where the collider is in the bottom somewhere
        if (!TryDestroyThis(other.transform.gameObject) &&
            !TryDestroyThis(other.transform.parent.gameObject) &&
            !TryDestroyThis(other.transform.parent.parent.gameObject) &&
            !TryDestroyThis(other.transform.parent.parent.gameObject)
            )
        {
            Debug.LogWarning("Trash: Did not erase object that collided with trash: " + other.name);
        }


    }

    bool TryDestroyThis(GameObject other)
    {
        if (other.GetComponent<PlayerCreatedObject>())
        {
            PhotonView pv = other.gameObject.GetComponent<PhotonView>();
            if (pv && pv.OwnershipTransfer == OwnershipOption.Takeover)
            {
                // networked object which can be destroyed
                SceneStepsManager.Instance.FromClient_DeleteObject(pv);
            }
            return true;
        }
        else
        {
            return false;
        }
    }
}
