using UnityEngine;
using System.Collections;

/*
 * Handles all GUI drawing:
 * Minimap
 * Reticule
 * Health and ammo indicators (TBA)
 * Objectives (TBA)
 * Framerate (if enabled)
 */

public class GUIController : MonoBehaviour
{
	public bool oculusEnabled = false;
	public bool showFramerate = false;
	public Texture minimap;
	public float minimapScale = 1.0f;
	public int minimapPadding = 32;
	public float minimapAlpha = 0.75f;
	public Texture minimapIndicator;
	public float indicatorScale = 1.0f;
	public float gameworldSize = 100; //size in the ortho minimap camera used to get the minimap texture
	public Texture reticule;
	public float reticuleScale = 1.0f;

	const int minimapBaseSize = 256;
	const int indicatorBaseSize = 16;
	const int reticuleBaseSize = 16;

	int screenWidth, screenHeight;
	float screenRatio;
	Rect minimapLocation;
	int minimapSize, indicatorSize, reticuleSize;
	GameObject player;
	Color minimapTint = Color.white;
	bool showMinimap = true;
	float minimapToggleDelay = 0.25f;


	void Start() {
		player = GameObject.Find("First Person Controller");

		minimapTint.a = minimapAlpha;

		if (!oculusEnabled) {
			screenWidth = Screen.width;
			screenHeight = Screen.height;

			screenRatio = screenWidth / 1920.0f;

			minimapSize = (int)(minimapBaseSize * screenRatio * minimapScale);
			indicatorSize = (int)(indicatorBaseSize * screenRatio * indicatorScale);
			reticuleSize = (int)(reticuleBaseSize * screenRatio * reticuleScale);
			minimapPadding = (int)(minimapPadding * screenRatio);

			//Standard camera minimap locations
			
			//Top right
			//minimapLocation = new Rect(screenWidth - minimapSize - minimapPadding, 0 + minimapPadding, minimapSize, minimapSize);
			
			//Bottom right
			minimapLocation = new Rect(screenWidth - minimapSize - minimapPadding, screenHeight - minimapPadding - minimapSize, minimapSize, minimapSize);
			
			//Bottom left
			//minimapLocation = new Rect(0 + minimapPadding, screenHeight - minimapPadding - minimapSize, minimapSize, minimapSize);
			
			//Top left
			//minimapLocation = new Rect(0 + minimapPadding, 0 + minimapPadding, minimapSize, minimapSize);
		} else {
			screenWidth = 1280;
			screenHeight = 800;

			minimapSize = (int)(minimapBaseSize * minimapScale * 0.5f);
			indicatorSize = (int)(indicatorBaseSize * indicatorScale * 0.5f);
			reticuleSize = (int)(reticuleBaseSize * reticuleScale * 0.5f);
			minimapPadding += 64;

			//Oculus minimap locations

			//Top right
			//minimapLocation = new Rect(screenWidth - minimapSize - minimapPadding, 0 + minimapPadding, minimapSize, minimapSize);
			
			//Bottom right
			//minimapLocation = new Rect(screenWidth - minimapSize - minimapPadding, screenHeight - minimapPadding - minimapSize, minimapSize, minimapSize);
			
			//Bottom left
			//minimapLocation = new Rect(0 + minimapPadding, screenHeight - minimapPadding - minimapSize, minimapSize, minimapSize);
			
			//Top left
			minimapLocation = new Rect(screenWidth * 0.25f + minimapPadding, screenHeight * 0.0f + minimapPadding, minimapSize, minimapSize);
		}
	}
	

	void Update() {
		if (minimapToggleDelay >= 0) minimapToggleDelay -= Time.deltaTime;
		else if (Input.GetAxisRaw("Toggle Minimap") > 0) {
			showMinimap = !showMinimap;
			minimapToggleDelay = 0.25f;
		}
	}


	void OnGUI() {
		if (oculusEnabled) return;

		if (showMinimap) {
			GUI.color = minimapTint;
			//Draw minimap
			GUI.DrawTexture(minimapLocation, minimap, ScaleMode.ScaleToFit);
			GUI.color = Color.white;

			//Draw player position indicator
			Rect indicatorLocation = new Rect(minimapLocation.xMin + minimapLocation.width / 2 + (player.transform.position.x / gameworldSize * minimapSize / 2) - indicatorSize / 2, minimapLocation.yMin + minimapLocation.height / 2 - (player.transform.position.z / gameworldSize * minimapSize / 2) - indicatorSize / 2, indicatorSize, indicatorSize);
			Matrix4x4 backup = GUI.matrix;
			GUIUtility.RotateAroundPivot(player.transform.eulerAngles.y, new Vector2(indicatorLocation.x + indicatorLocation.width / 2, indicatorLocation.y + indicatorLocation.height / 2));
			GUI.DrawTexture(indicatorLocation, minimapIndicator, ScaleMode.ScaleToFit);
			GUI.matrix = backup;
		}

		//Draw reticule
		GUI.DrawTexture(new Rect(screenWidth / 2 - reticuleSize / 2, screenHeight / 2 - reticuleSize / 2, reticuleSize, reticuleSize), reticule, ScaleMode.ScaleToFit);

		//Draw framerate
		if (showFramerate) GUI.Label(new Rect(32 * screenRatio, 32 * screenRatio, 400 * screenRatio, 400 * screenRatio), (1 / Time.deltaTime).ToString());
	}


	//Called in OVRMainMenu.cs
	public void OculusGUI(ref OVRGUI GuiHelper) {
		if (!oculusEnabled) return;

		if (showMinimap) {
			GUI.color = minimapTint;
			//Minimap
			GuiHelper.StereoDrawTexture((int)(minimapLocation.x), (int)(minimapLocation.y), (int)minimapLocation.width, (int)minimapLocation.height, ref minimap, Color.white);
			GUI.color = Color.white;
		
			//Indicator
			Rect indicatorLocation = new Rect(minimapLocation.xMin + minimapLocation.width / 2 + (player.transform.position.x / gameworldSize * minimapSize / 2) - indicatorSize / 2, minimapLocation.yMin + minimapLocation.height / 2 - (player.transform.position.z / gameworldSize * minimapSize / 2) - indicatorSize / 2, indicatorSize, indicatorSize);
			//Rect pivot = new Rect();
			//GuiHelper.CalcPositionAndSize(minimapLocation.xMin + minimapLocation.width / 2 + (player.transform.position.x / gameworldSize * minimapSize / 2), minimapLocation.yMin + minimapLocation.height / 2 - (player.transform.position.z / gameworldSize * minimapSize / 2), indicatorSize, indicatorSize, ref pivot);
			//Matrix4x4 backup = GUI.matrix;
			//GUIUtility.RotateAroundPivot(player.transform.eulerAngles.y, new Vector2(pivot.x, pivot.y));
			GuiHelper.StereoDrawTexture((int)(indicatorLocation.x), (int)(indicatorLocation.y), (int)indicatorLocation.width, (int)indicatorLocation.height, ref minimapIndicator, Color.white);
			//GUI.matrix = backup;
		}

		//Reticule
		GuiHelper.StereoDrawTexture((int)(screenWidth / 2 - reticuleSize / 2), (int)(screenHeight / 2 - reticuleSize / 2), (int)reticuleSize, (int)reticuleSize, ref reticule, Color.white);
	}
}