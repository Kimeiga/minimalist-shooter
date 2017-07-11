using UnityEngine;
using System.Collections;

public class Crosshair : MonoBehaviour {


	//** CROSSHAIR SIZE **

	public float scaleFactor;
	public float spread;
	


	//** CROSSHAIR STYLE **

	public Color crosshairColor = Color.white;
	public float width = 2;
	public float height = 4;



	//** MISCELLANEOUS **
    
	public bool drawCrosshair = true;
	private Texture2D tex;
	private GUIStyle lineStyle;
    

    //12/31
    public Gun gunScript;

    public SpeedCalculator speedCalculatorScript;
    public float measuredSpeedModifier = 1;

    void Awake (){
		tex = new Texture2D(1,1);
		
		SetColor(tex, crosshairColor); //Set color
		
		lineStyle = new GUIStyle();
		lineStyle.normal.background = tex;
	}

	void Update() {


        if (gunScript)
        {

            if (!gunScript.perfectAccuracy)
            {


                //Set the scale factor between the initial accuracy and the initial crosshair radius
                scaleFactor = gunScript.crosshairRadius / gunScript.accuracyMinimum;

                //Update the spread based on the current accuracy
                spread = height + gunScript.accuracy * scaleFactor;


            }
            else
            {

                spread = 0;

            }


        }
        else {

            spread = height + speedCalculatorScript.measuredSpeed * measuredSpeedModifier;

        }


    }
	
	void OnGUI (){
		var centerPoint = new Vector2(Screen.width / 2, Screen.height / 2);
		
		if(drawCrosshair){
			GUI.Box(new Rect(centerPoint.x - width / 2, centerPoint.y - (height + spread), width, height), "", lineStyle);
			GUI.Box(new Rect(centerPoint.x - width / 2, centerPoint.y + spread, width, height), "", lineStyle);
			GUI.Box(new Rect(centerPoint.x + spread, (centerPoint.y - width / 2), height , width), "", lineStyle);
			GUI.Box(new Rect(centerPoint.x - (height + spread), (centerPoint.y - width / 2), height , width), "", lineStyle);
		}   
	}
	
	//Applies color to the crosshair
	void SetColor(Texture2D myTexture, Color myColor){
		for (int y = 0; y < myTexture.height; ++y){
			for (int x = 0; x < myTexture.width; ++x){
				myTexture.SetPixel(x, y, myColor);
			}
		}
		
		myTexture.Apply();
	}
}