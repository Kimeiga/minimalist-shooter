using UnityEngine;
using System.Collections;

public class OldGun : MonoBehaviour {


	//* Gun Statistics *

	//Ammunition
	private int startingAmmo = 30;
	public int ammo;

	//Fire Rate
	public float fireRate = 0.1f;
	public float nextFire;

	//Damage
	public float damage;

	//Sweetspot
	public float sweetSpot;

	//Mobility
	public float mobility;



	//** SHOOTING MECHANICS **

	//Actual Raycast
	public float range = 1000;
	public Ray shot;
	public RaycastHit hit;

	//Accuracy Randomization
	private Vector3 ranSphere;
	private Vector2 ranCircle;
	public Vector3 direction;

	//Accuracy
	public float accuracy = 0.1f;
	public float accMin = 0.01f;
	private float accMax = 0.05f;
	
	//Shooting Accuracy
	private float accModShoot = 0f;
	private float accModShootMax = 0.03f;
	private float accIncrement = 0.015f;
	//private float accReset = 0.001f;

	//Moving Accuracy
	public float accModMove;
	public float accModMoveMax;

	//Recoil
	public AnimationCurve horRecoil;
	public AnimationCurve verRecoil;
	private float horRecoilMod = 0.2f;
	private float verRecoilMod = 0.3f;
	private float curRecoil;
	private float curRecoilIncrement;

	//** MISCELLANEOUS **
	
	public Camera playerCamera;
	public Transform shooterTransform;
	public bool canShoot = true;
	public GameObject ball;
	public float crosshairRadius;
	public bool firing;
	private float timeOfShot;
	public GameObject guidingBall;
	public LayerMask guidingLayerMask;


	//** TEMPORARY VARIABLES **

	public LayerMask myLayerMask;
	Vector3 v1;
	Vector3 v2;



	// Use this for initialization
	void Start () {
	

		
		//Reset (accModShoot,accModShoot,0.0f,1.0f);
		//print(accModShoot.ToString());

		//Set Current Recoil Increment
		curRecoilIncrement = 0.033f;

		//Setting the camera variable
		playerCamera = transform.parent.GetComponent<Camera>();

		//Setting values to their initial values
		accuracy = accMin;
		ammo = startingAmmo;

		
		//Calculate crosshair radius
		Vector3 dir0 = shooterTransform.forward + shooterTransform.right * accMin;
		
		RaycastHit hit0;
		
		if(Physics.Raycast (shooterTransform.position, dir0, out hit0, 1000, myLayerMask)){
			v1 = hit0.point;
		}
		
		RaycastHit hit1;
		if(Physics.Raycast(shooterTransform.position,shooterTransform.forward, out hit1, 1000, myLayerMask)){
			v2 = hit1.point;
		}
		
		v1 = playerCamera.WorldToScreenPoint(v1);
		v2 = playerCamera.WorldToScreenPoint(v2);
		crosshairRadius = Vector3.Distance(v1,v2);



	}



	void Update(){


        //this really shouldn't be in update() because it is very game sensitive.

		if(firing){
			Vector3 direction2;

			direction2 = shooterTransform.forward;
			direction2 += new Vector3(horRecoil.Evaluate(curRecoil) * horRecoilMod * 0.5f,0,0);
			direction2 += new Vector3(0,verRecoil.Evaluate(curRecoil) * verRecoilMod * 0.5f,0);
			
			//RaycastHit hit2;
			Ray shot2;
			//Cast ray
			shot2 = new Ray(shooterTransform.position, direction2);
			//if(Physics.Raycast(shot2, out hit2, range, guidingLayerMask)){
				
				//Put a ball there!
				//Instantiate(guidingBall, hit2.point, Quaternion.identity);
				//guidingBall.transform.position =  hit2.point;
			//}

			guidingBall.transform.position = shot2.GetPoint(range);

		}
	}



	// Update is called once per frame
	void FixedUpdate () {





		//Accuracy core
		accuracy = accMin + accModShoot + accModMove;
		
		//Clamp accuracy
		accuracy = Mathf.Clamp(accuracy,accMin,accMax);

		//Gradually lerp shooting accuracy back to 0
		accModShoot = Mathf.Lerp(accModShoot, 0, Time.time - timeOfShot);


		//Clamp shooting accuracy
		accModShoot = Mathf.Clamp(accModShoot, 0, accModShootMax);

		//Clamp current recoil
		curRecoil = Mathf.Clamp(curRecoil, 0, 1);

		//Set variable firing
		if(Input.GetButton("Fire") && ammo > 0 && canShoot == true){
			firing = true;
		} else {
			firing = false;
		}

		//Lerp current recoil back to 0
		if(firing == false){
			curRecoil = Mathf.Lerp(curRecoil, 0, Time.time - timeOfShot);
		}


		if(Input.GetButton("Fire") && ammo > 0 && canShoot == true && Time.time > nextFire){

			timeOfShot = Time.time;

			//Calculate Inaccuracy
			ranSphere = Random.insideUnitSphere * accuracy;
			ranCircle = new Vector2(ranSphere.x,ranSphere.y);

			//Calculate final path of bullet with inaccuracy and recoil
			direction = shooterTransform.forward + shooterTransform.right * ranCircle.x  + shooterTransform.up * ranCircle.y;
			direction += new Vector3(horRecoil.Evaluate(curRecoil) * horRecoilMod,0,0);
			direction += new Vector3(0,verRecoil.Evaluate(curRecoil) * verRecoilMod,0);



			//Cast ray
			shot = new Ray(shooterTransform.position, direction);
			if(Physics.Raycast(shot, out hit, range)){

				//Put a ball there!
				Instantiate(ball, hit.point, Quaternion.identity);

			}

			//Use 1 bullet
			ammo -= 1;

			//Set nextFire
			nextFire = Time.time + fireRate;

			//Increment accuracy
			accModShoot += accIncrement;

			//Increment recoil
			curRecoil += curRecoilIncrement;
		}

		//For the time being, I will allow reloading, until I make the inventory script
		if(Input.GetButtonDown("Cycle")){
			ammo = startingAmmo;
		}


	}

	void OnGUI() {
		GUI.Label( new Rect(0,0,100,100),accuracy.ToString());
		GUI.Label( new Rect(0,25,100,100),curRecoil.ToString());
	}



}
