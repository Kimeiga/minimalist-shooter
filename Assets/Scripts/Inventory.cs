using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Inventory : MonoBehaviour {

    public bool newWeapon = false;

    public GameObject[] scrollWheelWeapons;

    public GameObject equippedItem;
    public GameObject oldEquippedItem;

    public float range = 5;
    public RaycastHit hit;

    public GameObject playerCamera;
    public GameObject headEmpty;
    public SpeedCalculator speedCalculatorScript;
    public PlayerMouseLook bodyPlayerMouseLookScript;
    public WeaponSway weaponSwayScript;
    public Crosshair crosshairScript;
    public PlayerMouseLook headPlayerMouseLookScript;

    public float grabbingTime = 0.1f;
    public float holsteringTime = 0.15f;
    

    public Transform rightHolster;
    public Transform leftHolster;
    

    // Use this for initialization
    void Start () {

        equippedItem = null;
        oldEquippedItem = null;


        if (scrollWheelWeapons[0]) {
            equippedItem = scrollWheelWeapons[0];
        }
        

        

	}
	
	// Update is called once per frame
	void Update () {

        
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, range)) {


            if (hit.transform.gameObject.tag == "Gun") {

                if (Input.GetButtonDown("Interact")) {

                    oldEquippedItem = equippedItem;
                    
                    GameObject grabbedGun = hit.transform.gameObject;

                    if (scrollWheelWeapons[0] == null)
                    {
                        scrollWheelWeapons[0] = grabbedGun;
                    }

                    else if (scrollWheelWeapons[1] == null)
                    {
                        scrollWheelWeapons[1] = grabbedGun;
                    }

                    Gun gunScript = grabbedGun.GetComponent<Gun>();

                    grabbedGun.transform.parent = headEmpty.transform;
                    
                    Rigidbody rig = grabbedGun.GetComponent<Rigidbody>();
                    rig.isKinematic = true;
                    rig.useGravity = false;

                    gunScript.speedCalculatorScript = speedCalculatorScript;
                    gunScript.shootTransform = headEmpty.transform;
                    gunScript.playerCamera = playerCamera.GetComponent<Camera>();

                    EquipItem(grabbedGun);

                    if (oldEquippedItem) {
                        UnequipItem(oldEquippedItem);
                    }
                    
                    equippedItem = grabbedGun;

                }
            }

        }
        

        

        if (scrollWheelWeapons[0] && scrollWheelWeapons[0] != equippedItem) {
            if (Input.GetAxis("Mouse ScrollWheel") > 0.01f)
            {
                oldEquippedItem = equippedItem;
                equippedItem = scrollWheelWeapons[0];
                EquipItem(equippedItem);
                UnequipItem(oldEquippedItem);

            }

        }


        if (scrollWheelWeapons[1] && scrollWheelWeapons[1] != equippedItem) {

            if (Input.GetAxis("Mouse ScrollWheel") < -0.01f)
            {

                 oldEquippedItem = equippedItem;
                 equippedItem = scrollWheelWeapons[1];
                 EquipItem(equippedItem);
                 UnequipItem(oldEquippedItem);

            }

        }
            
        
        



    }

    IEnumerator MoveObjectLocal(Transform thisTransform, Vector3 startPosition, Vector3 endPosition, float time, bool worldToLocal)
    {
        float i = 0.0f;
        float rate = 1.0f / time;

        Vector3 realStartPosition;

        if (worldToLocal)
        {
            realStartPosition = headEmpty.transform.InverseTransformPoint(startPosition);
        }
        else {
            realStartPosition = startPosition;
        }


        while (i < 1.0f)
        {
            i += Time.deltaTime * rate;
            thisTransform.localPosition = Vector3.Lerp(realStartPosition, endPosition, i);
            yield return null;
        }
    }

    IEnumerator RotateObjectLocal(Transform thisTransform, Quaternion startRotation, Quaternion endRotation, float time, bool worldToLocal)
    {
        float i = 0.0f;
        float rate = 1.0f / time;

        Quaternion realStartRotation;

        if (worldToLocal)
        {
            realStartRotation = Quaternion.Euler(startRotation.eulerAngles);
        }
        else {
            realStartRotation = startRotation;
        }
        
        while (i < 1.0f)
        {
            i += Time.deltaTime * rate;
            thisTransform.localRotation = Quaternion.Slerp(realStartRotation, endRotation, i);
            yield return null;
        }


    }


    void EquipItem( GameObject itemToEquip) {

        //all used to equip a gun
        if (itemToEquip.tag == "Gun")
        {

            Gun gunScript = itemToEquip.GetComponent<Gun>();

            gunScript.enabled = true;

            //hold w/ right hand if it is in the 0 slot
            if (itemToEquip == scrollWheelWeapons[0])
            {
                StartCoroutine(MoveObjectLocal(itemToEquip.transform, itemToEquip.transform.position, gunScript.originalPosition, grabbingTime, true));

                weaponSwayScript.defaultPosition = gunScript.originalPosition;
            }

            //hold with left hand if it is in the 1 slot
            if (itemToEquip == scrollWheelWeapons[1])
            {

                Vector3 leftHandPosition = new Vector3(-gunScript.originalPosition.x, gunScript.originalPosition.y, gunScript.originalPosition.z);

                StartCoroutine(MoveObjectLocal(itemToEquip.transform, itemToEquip.transform.position, leftHandPosition, grabbingTime, true));

                weaponSwayScript.defaultPosition = leftHandPosition;

            }


            bodyPlayerMouseLookScript.gunScript = gunScript;
            crosshairScript.gunScript = gunScript;
            headPlayerMouseLookScript.gunScript = gunScript;
            weaponSwayScript.gun = itemToEquip;
            
            weaponSwayScript.animationTransform = itemToEquip.transform.Find("Animation Transform");

            
            
            StartCoroutine(RotateObjectLocal(itemToEquip.transform, itemToEquip.transform.rotation, Quaternion.identity, grabbingTime, true));


        }

    }

    void UnequipItem (GameObject itemToUnequip) {

        //then unequip it if its a gun
        if (itemToUnequip.tag == "Gun")
        {

            Gun gunScript = itemToUnequip.GetComponent<Gun>();

            gunScript.enabled = false;


            if (itemToUnequip == scrollWheelWeapons[0])
            {

                StartCoroutine(MoveObjectLocal(itemToUnequip.transform, itemToUnequip.transform.localPosition, rightHolster.localPosition, holsteringTime, false));
                StartCoroutine(RotateObjectLocal(itemToUnequip.transform, itemToUnequip.transform.localRotation, rightHolster.localRotation, holsteringTime, false));

            }

            if (itemToUnequip == scrollWheelWeapons[1])
            {

                StartCoroutine(MoveObjectLocal(itemToUnequip.transform, itemToUnequip.transform.localPosition, leftHolster.localPosition, holsteringTime, false));
                StartCoroutine(RotateObjectLocal(itemToUnequip.transform, itemToUnequip.transform.localRotation, leftHolster.localRotation, holsteringTime, false));

            }

        }

    }


}
