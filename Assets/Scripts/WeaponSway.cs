using UnityEngine;
using System.Collections;

public class WeaponSway : MonoBehaviour {

	public float moveAmount = 1;
	public float moveSpeed = 2;
    //set in inventory v
	public GameObject gun;
	public float moveOnX;
	public float moveOnY;

    //set in inventory v
    public Vector3 defaultPosition;
	public Vector3 newPosition;

    public FPSWalker FPSWalkerScript;
    public SpeedCalculator speedCalculatorScript;

    public float crouchOffset = -0.05f;
    public float jumpOffset = -0.05f;
    public float moveMultiplier = -2;

    float eks;

    //set in inventory v
    public Transform animationTransform;
    public Vector3 animationTransformPositionTarget;

    public Vector3 XYMoveDirection;
    public float moveThreshold = 0.001f;

	// Use this for initialization
	void Start () {


        if (gun) {
            defaultPosition = gun.GetComponent<Gun>().originalPosition;
        }
        
        

	}
	
	// Update is called once per frame
	void Update () {

        if (animationTransform)
        {

            XYMoveDirection = new Vector3(speedCalculatorScript.measured3DSpeed.x, 0, speedCalculatorScript.measured3DSpeed.z);



            animationTransformPositionTarget.z = moveMultiplier * XYMoveDirection.magnitude;

            if (FPSWalkerScript.crouching)
            {


                eks = crouchOffset;

            }
            else
            {


                eks = 0;

            }





            if (!FPSWalkerScript.grounded)
            {

                animationTransformPositionTarget.y = jumpOffset;

            }
            else
            {


                animationTransformPositionTarget.y = Mathf.Lerp(animationTransformPositionTarget.y, 0, FPSWalkerScript.crouchSpeed * Time.deltaTime);
            }


            animationTransformPositionTarget.x = Mathf.Lerp(animationTransformPositionTarget.x, eks, FPSWalkerScript.crouchSpeed * Time.deltaTime);



            animationTransform.localPosition = animationTransformPositionTarget;

        }


        if (gun != null)
        {

            moveOnX = Input.GetAxis("Mouse X") * Time.deltaTime * moveAmount;
            moveOnY = Input.GetAxis("Mouse Y") * Time.deltaTime * moveAmount;

            newPosition = new Vector3(defaultPosition.x + moveOnX, defaultPosition.y + moveOnY, defaultPosition.z);
            gun.transform.localPosition = Vector3.Lerp(gun.transform.localPosition, newPosition, Time.deltaTime * moveSpeed);


        }


	}
}
