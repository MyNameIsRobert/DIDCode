using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
[RequireComponent (typeof (CharacterController))]
[RequireComponent (typeof (Stamina))]
[RequireComponent(typeof (PlayerProperties))]
public class PlayerControllerSinglePlayer: MonoBehaviour {

    #region Variables
    #region Climbing
    public float climbSpeed = 10f;
	public bool climb = false;
    #endregion

    #region Movement and Aiming
    [HideInInspector]
	public float moveSpeed = 0;
    [SerializeField]
    public float appliedMoveSpeed = 0;
    [SerializeField]
    float moveSpeedModifier = 1;
    [SerializeField]
    float jumpSpeedModifier = 1;
    float appliedJumpSpeed;
	public float sensitivity;
    public float mouseSensitivity;
    float defaultMouseSensitivity;
    float aimingSensitivity;
    public float walkSpeed = 10f;
	public float runSpeed = 12f;
    public float airSpeed = 3f;
	public float jumpSpeed = 8f;
	public float gravity = 20f;
    [SerializeField]
    float maxAirSpeed = 10f;
    private float xVelocity, ZVelocity;
    private Vector3 moveDirection = Vector3.zero;

    public float xDirection = 0;

    bool isMoving = false;

    

    #endregion

    #region Components
    private CharacterController controller;
	public GameObject player;
    Stamina stamina;
    PlayerProperties properties;
    WeaponSwitch weaponSwitch;
    PlayerInputControls control;
    #endregion
    #endregion

    // Use this for initialization
    #region Start
    void Start () {
        properties = GetComponent<PlayerProperties>();
		controller = GetComponent<CharacterController> ();
        stamina = GetComponent<Stamina>();
        weaponSwitch = GetComponent<WeaponSwitch>();
        control = GetComponent<PlayerInputControls>();

	}
#endregion
    // Update is called once per frame
    #region FixedUpdate
    void Update() {
        if(Time.timeScale == 0)
        {
            return;
        }
        #region Setting Aiming Sensitivity
        defaultMouseSensitivity = sensitivity;
        float tempZoomAmount = 1;
        if (weaponSwitch.equippedWeapon != null)
        {
            if(weaponSwitch.equippedWeapon.GetComponent<Weapon>().weaponType == Weapon.WeaponType.Gun) { 
                tempZoomAmount = weaponSwitch.equippedWeapon.GetComponent<Shoot>().zoomAmount;
            }
            else
            {
                tempZoomAmount = 1;
            }
        }
        aimingSensitivity = defaultMouseSensitivity / (2 * tempZoomAmount);
#endregion

        #region Sets the moveSpeed based on whether or not the shift button is being held
        if (!stamina.isStaminaEmpty)
        {
            if(!Input.GetKey(control.run))
            {
                moveSpeed = walkSpeed;
                GetComponent<PlayerProperties>().SetSprinting(false);
            }
            else
            {
                moveSpeed = runSpeed;
                
                //Only Decrease stamina if the player intends on moving
                if(Input.GetAxis("Strafe") != 0 || Input.GetAxis("Vertical") != 0)
                {
                    stamina.DecreaseStamina(5 * Time.deltaTime);
                    GetComponent<PlayerProperties>().SetSprinting(true);
                }
                else
                {
                    GetComponent<PlayerProperties>().SetSprinting(false);
                }
            }

        }
        else
        {
            moveSpeed = walkSpeed;
            GetComponent<PlayerProperties>().SetSprinting(false);
        }

        moveSpeed = (properties.GetEncumbered() ? moveSpeed / 2 : moveSpeed);

        appliedJumpSpeed = jumpSpeed * jumpSpeedModifier;
        appliedMoveSpeed = moveSpeed * moveSpeedModifier;
        #endregion

        #region Changes Properties depending on whether or not player is aiming
        moveSpeed = (properties.aiming ? moveSpeed / 2 : moveSpeed);
        mouseSensitivity = (properties.aiming ? aimingSensitivity : defaultMouseSensitivity);
        #endregion

        #region Sets the Rotation based off of the mouse
        transform.Rotate(0, Input.GetAxis("Mouse X") * mouseSensitivity, 0);
        xDirection = transform.rotation.y;
        #endregion

        #region Moves

            #region If player is on ground
        if (controller.isGrounded) {
            moveDirection = new Vector3(Input.GetAxis("Strafe"), 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= appliedMoveSpeed;

            if (Input.GetKey(control.jump))
            {
                moveDirection.y = appliedJumpSpeed;
                xVelocity = controller.velocity.x;
                ZVelocity = controller.velocity.z;

                if (xVelocity < 0)
                    xVelocity *= -1;
                if (ZVelocity < 0)
                    ZVelocity *= -1;
            }
        }
        #endregion

            #region If player is in air
        else
        {
            moveDirection = new Vector3(Input.GetAxis("Strafe"), moveDirection.y, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            if (airSpeed + xVelocity > (maxAirSpeed * moveSpeedModifier) || airSpeed + ZVelocity > (maxAirSpeed * moveSpeedModifier))
            {
                moveDirection.x *= (maxAirSpeed * moveSpeedModifier);
                moveDirection.z *= (maxAirSpeed * moveSpeedModifier);
            }
            else
            {
                moveDirection.x *= airSpeed + xVelocity;
                moveDirection.z *= airSpeed + ZVelocity;
            }
        }
        #endregion

            #region Do not have gravity if on ladder
        if (!climb) {
			moveDirection.y -= gravity * Time.deltaTime;
		}
        #endregion

            #region Calling CharacterController Move Funciton
        controller.Move(moveDirection * Time.deltaTime);
        #endregion

        #endregion

        #region  Ladder Climbing
        if (climb) {
		    //Implement better ladder code here
		}
        #endregion

        #region Setting isMoving
        if (controller.velocity.magnitude >= .5f)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
#endregion

    }
#endregion

    #region Setting Climb
    void OnTriggerEnter(Collider other){

		if (other.CompareTag ("Ladder"))
        {
			climb = true;
			Debug.Log ("Enter Ladder Collider");
		}

			
	
	}

	void OnTriggerExit(Collider other){
		if(other.CompareTag("Ladder")){
			climb = false;
			Debug.Log("Exit Ladder Collider");
		}
	}
#endregion

    /// <summary>
    /// Increases the move speed modifier by a percentage, and then stores the flat amount changed in amountChangedBy
    /// </summary>
    /// <param name="percentage"></param>
    /// <param name="amountChangedBy"></param>
    public void ChangeMoveSpeedModifierPercentage(float percentage, out float amountChangedBy)
    {
        percentage /= 100;
        amountChangedBy = percentage * moveSpeedModifier;
        moveSpeedModifier += amountChangedBy;
    }
    public void ChangeMoveSpeedModifierFlatNumber(float number)
    {
        moveSpeedModifier -= number;
    }

    public void ChangeJumpSpeedModifierPercentage(float percentage, out float amountChangedBy)
    {
        percentage /= 100;
        amountChangedBy = percentage * jumpSpeedModifier;
        jumpSpeedModifier += amountChangedBy;

    }
    public void ChangeJumpSpeedModifierFlat(float number)
    {
        jumpSpeedModifier += number;
    }

    public bool GetIsMoving()
    {
        return isMoving;
    }

}

