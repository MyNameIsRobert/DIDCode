using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {

    [SerializeField]
    DoorState doorState;

    [SerializeField]
    [Tooltip("The number that should correspond with a key that is accessible without the player needing to unlock the door. The player by default has a hidden key of 999, which allows for door testing while creating")]
    int lockNumber = 0;

    [SerializeField]
    bool isLocked = false;

    [HideInInspector]
    public bool isOpen = false;
    [SerializeField]
    bool playerIsInOpenCollider = false;
    [SerializeField]
    bool playerIsInCloseCollider = false;

    public DoorDirection doorDirection;
    public DoorType doorType;

    Transform playerPos;

    public enum DoorDirection
    {
        Up,
        [Tooltip("Moves the door in the positive Z direction")]
        Left,
        [Tooltip("Moves the door in the negative Z direction")]
        Right,
        Down,
        [Tooltip("Opens door towards the 'Open Collider")]
        In,
        [Tooltip("Opens door towards the 'Close Collider'")]
        Out,
        AwayFromPlayer
    }
    public enum DoorType
    {
        /// <summary>
        /// The door can be opened without a key initally, but will close once the player leaves the "Open" collider, and enters the "Close" collider, and will require a key to open again
        /// </summary>
        RequiresKeyAfter,
        /// <summary>
        /// Requires a key with the correct number to unlock the door initially, but the door stays open afterwards
        /// </summary>
        RequiresKeyInit,
        /// <summary>
        /// Opens when player enters collider with an "AutoDoor" script attached to it. Opens automatically both ways. Stays open while player is in the collider
        /// </summary>
        Automatic,
        /// <summary>
        /// Requires that the player press the 'Use' button to open the door
        /// </summary>
        Use
    }
    enum DoorState
    {
        Open,
        Closed
    }

    Animator doorAnim;
	// Use this for initialization
	void Start () {
        try
        {
            doorAnim = GetComponent<Animator>();
        }
        catch (MissingComponentException e){
            Debug.LogException(e);
        }

        if(doorType == DoorType.RequiresKeyInit)
        {
            isLocked = true;
        }
	}
	
	// Update is called once per frame
	void Update () {

        doorState = (isOpen) ? DoorState.Open : DoorState.Closed;
        
	}

    public bool UnlockDoor(int keyNumber)
    {
        bool unlockedDoor = false;
        if(keyNumber == lockNumber)
        {
            isLocked = false;
            unlockedDoor = true;
            lockNumber = -1;
        }
        else
        {
            unlockedDoor = false;
        }

        return unlockedDoor;
    }

    public bool OpenDoorAutomatically()
    {
        bool opened = false;
        if (isLocked)
        {
            return opened;
        }
        
        if(doorType == DoorType.Automatic || doorType == DoorType.RequiresKeyAfter)
        {
            OpenDoor();
            opened = true;
        }
        else
        {
            Debug.Log("Door type is not automatic, so the door can't be opened automatically!");
            opened = false;
        }   

        return opened;
    }

    public bool OpenDoorWithUse()
    {
        bool opened = false;
        if (isLocked)
        {
            return opened;
        }
        OpenDoor();

        opened = true;
        
        return opened;
    }
    public bool CloseDoorWithUse()
    {
        CloseDoor();
        return true;
    }
    public bool LockDoor()
    {
        bool locked = false;
        if (isLocked)
        {
            Debug.Log("Door is already locked!");
            locked = false;
            return locked;
        }

        if(lockNumber == -1)
        {
            Debug.Log("Door cannot be locked!");
            return locked;
        }

        if(doorType == DoorType.RequiresKeyAfter)
        {
            if (playerIsInCloseCollider)
            {
                if (!playerIsInOpenCollider)
                {                    
                    isLocked = true;
                    locked = true;
                }
                else
                {
                    Debug.Log("Player is still in the 'open' collider!"); 
                }
            }
        }
        else if(doorType == DoorType.RequiresKeyInit)
        {
            Debug.Log("Door cannot be locked after it is unlocked because it is set to RequiresKeyInit!");
            locked = false;
        }
        return locked;
    }
    public void CloseDoorAutomatically()
    {
        Debug.Log("Close door was called");
        if (!isOpen)
        {
            Debug.Log("Door is not open!");
            return;
        }
        if(doorType == DoorType.RequiresKeyAfter && lockNumber != -1)
        {
            if (playerIsInCloseCollider)
            {
                if (!playerIsInOpenCollider)
                {
                    CloseDoor();
                    bool locked = LockDoor();
                    if (locked)
                    {
                        Debug.Log("Door was successfully locked and closed");
                    }
                    else
                    {
                        Debug.Log("Door was closed but not locked. There was an error somewhere");
                    }
                }
            }
        }
        else
        {
            CloseDoor();
        }

    }
    void OpenDoor()
    {

        bool opened = true;
        switch (doorDirection)
        {
            case DoorDirection.Up:
                doorAnim.SetTrigger("Up");
                break;
            case DoorDirection.Right:
                doorAnim.SetTrigger("Right");
                break;
            case DoorDirection.Out:
                doorAnim.SetTrigger("Out");
                break;
            case DoorDirection.Left:
                doorAnim.SetTrigger("Left");
                break;
            case DoorDirection.In:
                doorAnim.SetTrigger("In");
                break;
            case DoorDirection.Down:
                doorAnim.SetTrigger("Down");
                break;
            case DoorDirection.AwayFromPlayer:
                if (playerIsInCloseCollider)
                {
                    doorAnim.SetTrigger("In");
                }
                else if (playerIsInOpenCollider)
                {
                    doorAnim.SetTrigger("Out");
                }
                else
                {
                    Debug.Log("Player is not in open or close collider, so the door does not know which way to open! Make sure the automatic open collider is smaller than either close or open collider, and make sure the 'close' and 'open' colliders don't overlap!");
                    opened = false;
                }
                break;

        }

        if (opened)
        {
            isOpen = true;
        }
    }
    void CloseDoor()
    {
        Debug.Log("Door was closed");
        isOpen = false;
        doorAnim.SetTrigger("Close");
    }

    public void SetOpenCollider(bool open)
    {
        playerIsInOpenCollider = open;
    }
    public void SetCloseCollider(bool close)
    {
        playerIsInCloseCollider = close;
    }
    public void SetPlayerPosition(Transform player)
    {
        playerPos = player;
    }
    public bool GetIsLocked()
    {
        return isLocked;
    }
    public bool IsCorrectKey(int keyNumber)
    {
        if (keyNumber == lockNumber)
            return true;
        else
            return false;
    }
}
