using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
[RequireComponent (typeof (CharacterController))]
public class PlayerControllerMultiplayer: NetworkBehaviour {

	//Handling Variables
	public float climbSpeed = 10f;
	public bool climb = false;

	public float moveSpeed = 0;
	public float rotationSpeed = 150f;
	public float walkSpeed = 10f;
	public float runSpeed = 6f;
	public float jumpSpeed = 8f;
	public float gravity = 20f;

	public float defaultShootSpeed = 1000f;
	public float shootSpeed = 0;
	public float fireRate = .5f;
	public float lastShot = .0f;
	public float bulletLifeTime = 2f;


	public float powerUpTimer = 0f;



	private bool curserLock = true;

	Spawner spawn;

	public   int spawnLocation;

	//Returns a random Int
	public   int RandomNumber(int n){
		return ((int)(n * Random.value) + 1);

	}

	//System Variables
	private Vector3 moveDirection= Vector3.zero;

	//Components
	private CharacterController controller;
	public GameObject player;
	public GameObject bulletSpawn;
    PlayerInputControls control;

	//Bullets
	public GameObject defaultBullet;
	public GameObject sniperBullet;
	public GameObject mgBullet;
	public GameObject bulletPrefab;




	// Use this for initialization
	void Start () {
		controller = GetComponent<CharacterController> ();
        control = GetComponent<PlayerInputControls>();

	}
	
	// Update is called once per frame
	void Update () {
		if (!isLocalPlayer) {
			return;
		}
		//Gets the input for which direction to move
		moveSpeed = (Input.GetKey(control.run)? walkSpeed:runSpeed);
		//Sets the Rotation based off of the mouse
		transform.Rotate (0, Input.GetAxis ("Mouse X") * rotationSpeed, 0);

		//Moves
		if (controller.isGrounded) {
			moveDirection = new Vector3 (Input.GetAxis ("Strafe"), 0, Input.GetAxis ("Vertical"));
			moveDirection = transform.TransformDirection (moveDirection);
			moveDirection *= moveSpeed;

			if (Input.GetKey (control.jump))
				moveDirection.y = jumpSpeed;
		}
		//Do not have gravity if on ladder
		if (!climb) {
			moveDirection.y -= gravity * Time.deltaTime;
		}
		controller.Move(moveDirection * Time.deltaTime);


	
		//Shooting Mechanic
		if(Input.GetButton("Fire")){
			CmdShoot ();
		}



		//Cursor Locking
		if (Input.GetKeyUp (KeyCode.Escape)) {
			curserLock = false;		
		}
		if (Input.GetKeyUp (KeyCode.W) || Input.GetKeyUp (KeyCode.S) || Input.GetKeyUp (KeyCode.A) || Input.GetKeyUp (KeyCode.D)) {
			curserLock = true;		
		}



		//If the player falls off of the map, respawn
		if (transform.position.y < -1) {
			//Respawn ();
		}

		//Runs CurserLock method
		CurserLock ();
		//Ladder Climbing
		if (climb) {
			if(Input.GetKey(KeyCode.W)){
				transform.Translate (Vector3.up * Time.deltaTime * climbSpeed);
			}
			if (Input.GetKey (KeyCode.S)) {
				transform.Translate (Vector3.down * Time.deltaTime * climbSpeed);
			}
		}
		//Bullet Powerup Timer
		powerUpTimer -= Time.deltaTime;
		PowerUp ();



	}
	[Command]
	public void CmdShoot (){
		Quaternion angle = bulletSpawn.transform.localRotation;

		
		if (Time.time > fireRate + lastShot) {
			GameObject bullet = (GameObject)Object.Instantiate (bulletPrefab, bulletSpawn.transform.position, angle);

			bullet.GetComponent<Rigidbody> ().AddForce (bulletSpawn.transform.right * shootSpeed);
			lastShot = Time.time;
			NetworkServer.Spawn (bullet);
			Destroy (bullet, bulletLifeTime);
		}
	}
		

	void CurserLock(){
		if (curserLock) {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			
		} else if (!curserLock) {
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	
	}

	void OnTriggerEnter(Collider other){
		if (other.CompareTag ("Ladder")) {
			climb = true;
			Debug.Log ("Enter Ladder Collider");
		}
		else if (other.CompareTag ("MG")) {
			bulletPrefab = mgBullet;
			fireRate = .1f;
			powerUpTimer = 20;
		} else if (other.CompareTag ("Sniper")) {
			bulletPrefab = sniperBullet;
			powerUpTimer = 20;
			fireRate = 1f;
			shootSpeed = 10000;
		}
			
	
	}
	void OnTriggerExit(Collider other){
		if(other.CompareTag("Ladder")){
			climb = false;
			Debug.Log("Exit Ladder Collider");
		}
	}

	private void PowerUp (){
		if (powerUpTimer < 0) {
			bulletPrefab = defaultBullet;
			fireRate = .5f;
			shootSpeed = defaultShootSpeed;	
		}

	}

	public override void OnStartLocalPlayer(){
		GetComponent<MeshRenderer> ().material.color = Color.green;
	}

}
