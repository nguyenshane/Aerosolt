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
	public bool dynamicMinimap = false;

	public GUIStyle background, health, activeMenuItem, inactiveMenuItem;

	public Texture minimap;
	public float minimapScale = 1.0f;
	public int minimapPadding = 32;
	public float minimapAlpha = 0.5f;

	public Texture minimapIndicator;
	public float indicatorScale = 1.0f;
	
	public Texture reticule;
	public float reticuleScale = 1.0f;

	const int minimapBaseSize = 256;
	const int indicatorBaseSize = 16;
	const int reticuleBaseSize = 16;

	readonly string[] menuOptions = {"Continue", "Main Menu", "Exit"};
	const int menuItemSpacing = 80;
	const float inputRepeatDelay = 0.25f;

	int screenWidth, screenHeight;
	float screenRatio;

	Rect minimapLocation;
	int minimapSize, indicatorSize, reticuleSize;
	Color minimapTint = Color.white;
	bool showMinimap = true;
	float gameworldSize; //size in the ortho minimap camera used to get the minimap texture

	int menuSelection = 0;
	int prevMenuInputState = 0;
	float inputRepeatTimer;
	int prevSelectionInputState = 0;
	float selectionInputRepeatTimer;
	int prevActivationInputState = 0;
	float activationInputRepeatTimer;
	int spacing;
	bool showingMenu = false;
	
	GameObject player;


	void Start() {
		Screen.showCursor = false;

		inputRepeatTimer = selectionInputRepeatTimer = activationInputRepeatTimer = inputRepeatDelay;

		player = GameObject.Find("First Person Controller");

		Transform minimapCamera = transform.Find("Minimap Camera");
		Camera mmCameraComp = minimapCamera.gameObject.GetComponent<Camera>();
		gameworldSize = mmCameraComp.orthographicSize;

		if (dynamicMinimap) {
			mmCameraComp.targetTexture = new RenderTexture(1024, 1024, 16, RenderTextureFormat.ARGB32);
			RenderTexture currentRT = RenderTexture.active;
			RenderTexture.active = mmCameraComp.targetTexture;
			mmCameraComp.Render();
			Texture2D mm = new Texture2D(mmCameraComp.targetTexture.width, mmCameraComp.targetTexture.height);
			mm.ReadPixels(new Rect(0, 0, mmCameraComp.targetTexture.width, mmCameraComp.targetTexture.height), 0, 0);
			mm.Apply();
			minimap = mm as Texture;
			RenderTexture.active = currentRT;
		}

		minimapCamera.gameObject.SetActive(false);
		
		
		minimapTint.a = minimapAlpha;

		if (!oculusEnabled) {
			screenWidth = Screen.width;
			screenHeight = Screen.height;

			screenRatio = screenWidth / 1920.0f;

			minimapSize = (int)(minimapBaseSize * screenRatio * minimapScale);
			indicatorSize = (int)(indicatorBaseSize * screenRatio * indicatorScale);
			reticuleSize = (int)(reticuleBaseSize * screenRatio * reticuleScale);
			minimapPadding = (int)(minimapPadding * screenRatio);

			health.fontSize = (int)(health.fontSize * screenRatio);
			activeMenuItem.fontSize = (int)(activeMenuItem.fontSize * screenRatio);
			inactiveMenuItem.fontSize = (int)(inactiveMenuItem.fontSize * screenRatio);
			spacing = (int)(menuItemSpacing * screenRatio);

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
			//minimapLocation = new Rect(screenWidth * 0.25f + minimapPadding, screenHeight * 0.0f + minimapPadding, minimapSize, minimapSize);

			//Centered
			minimapLocation = new Rect(screenWidth * 0.5f - minimapSize / 2, screenHeight * 0.5f - minimapSize / 2, minimapSize, minimapSize);
		}
	}
	

	void Update() {
		if (Input.GetAxisRaw("Show Minimap") > 0) showMinimap = true;
		else showMinimap = false;

		if (inputRepeatTimer >= 0) inputRepeatTimer -= Time.unscaledDeltaTime;
		if (selectionInputRepeatTimer >= 0) selectionInputRepeatTimer -= Time.unscaledDeltaTime;
		if (activationInputRepeatTimer >= 0) activationInputRepeatTimer -= Time.unscaledDeltaTime;

		if (showingMenu) {
			//Selection axes
			if (Input.GetAxisRaw("Menu Navigation") > 0) {
				//Up selection
				if (prevMenuInputState != 1 || inputRepeatTimer <= 0) {
					inputRepeatTimer = inputRepeatDelay;
					menuSelection--;
					if (menuSelection < 0) menuSelection = menuOptions.Length-1;
				}

				prevMenuInputState = 1;
			} else if (Input.GetAxisRaw("Menu Navigation") < 0) {
				//Down selection
				if (prevMenuInputState != -1 || inputRepeatTimer <= 0) {
					inputRepeatTimer = inputRepeatDelay;
					menuSelection++;
					if (menuSelection >= menuOptions.Length) menuSelection = 0;
				}

				prevMenuInputState = -1;
			} else prevMenuInputState = 0; //No axis movement

			//Selection button
			if (Input.GetAxisRaw("Menu Selection") > 0) {
				if (prevSelectionInputState != 1 || selectionInputRepeatTimer <= 0) {
					selectionInputRepeatTimer = inputRepeatDelay;
					handleMenuSelection();
				}

				prevSelectionInputState = 1;
			} else prevSelectionInputState = 0;

			//Back button
			if (Input.GetAxisRaw("Menu Return") > 0) {
				if (prevActivationInputState != 1 || activationInputRepeatTimer <= 0) {
					activationInputRepeatTimer = inputRepeatDelay;
					deactivateMenu();
				}

				prevActivationInputState = 1;
			} else prevActivationInputState = 0;
		} else { //In-game controls
			//Menu button
			if (Input.GetAxisRaw("Menu Activation") > 0) {
				if (prevActivationInputState != 1 || activationInputRepeatTimer <= 0) {
					activationInputRepeatTimer = inputRepeatDelay;
					activateMenu();
				}

				prevActivationInputState = 1;
			} else prevActivationInputState = 0;
		}
	}


	void OnGUI() {
		if (oculusEnabled) return;

		//In-game UI

		//Draw reticule
		GUI.DrawTexture(new Rect(screenWidth / 2 - reticuleSize / 2, screenHeight / 2 - reticuleSize / 2, reticuleSize, reticuleSize), reticule, ScaleMode.ScaleToFit);

		//Draw health
		GUI.Label(new Rect((int)screenWidth / 3, (int)screenHeight / 3, 60 * screenRatio, 20 * screenRatio), player.GetComponentInChildren<Player>().getHP().ToString(), health);
		
		//Draw framerate
		if (showFramerate) GUI.Label(new Rect(32 * screenRatio, 32 * screenRatio, 400 * screenRatio, 400 * screenRatio), (1 / Time.deltaTime).ToString());

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

		//Overlay menu
		if (showingMenu) {
			GUI.Box(new Rect(0, 0, screenWidth, screenHeight), "", background);

			switch (menuSelection) {
			case 0:
				GUI.Label(new Rect(screenWidth / 2, screenHeight / 2 - spacing, 200 * screenRatio, 40 * screenRatio), menuOptions[0], activeMenuItem);
				GUI.Label(new Rect(screenWidth / 2, screenHeight / 2, 200 * screenRatio, 40 * screenRatio), menuOptions[1], inactiveMenuItem);
				GUI.Label(new Rect(screenWidth / 2, screenHeight / 2 + spacing, 200 * screenRatio, 40 * screenRatio), menuOptions[2], inactiveMenuItem);
				break;

			case 1:
				GUI.Label(new Rect(screenWidth / 2, screenHeight / 2 - spacing, 200 * screenRatio, 40 * screenRatio), menuOptions[0], inactiveMenuItem);
				GUI.Label(new Rect(screenWidth / 2, screenHeight / 2, 200 * screenRatio, 40 * screenRatio), menuOptions[1], activeMenuItem);
				GUI.Label(new Rect(screenWidth / 2, screenHeight / 2 + spacing, 200 * screenRatio, 40 * screenRatio), menuOptions[2], inactiveMenuItem);
				break;

			case 2:
				GUI.Label(new Rect(screenWidth / 2, screenHeight / 2 - spacing, 200 * screenRatio, 40 * screenRatio), menuOptions[0], inactiveMenuItem);
				GUI.Label(new Rect(screenWidth / 2, screenHeight / 2, 200 * screenRatio, 40 * screenRatio), menuOptions[1], inactiveMenuItem);
				GUI.Label(new Rect(screenWidth / 2, screenHeight / 2 + spacing, 200 * screenRatio, 40 * screenRatio), menuOptions[2], activeMenuItem);
				break;
			}
		}
	}


	//Called in OVRMainMenu.cs
	public void OculusGUI(ref OVRGUI GuiHelper) {
		if (!oculusEnabled) return;

		//In-game UI

		//Reticule
		GuiHelper.StereoDrawTexture((int)(screenWidth / 2 - reticuleSize / 2), (int)(screenHeight / 2 - reticuleSize / 2), (int)reticuleSize, (int)reticuleSize, ref reticule, Color.white);
		
		//Health
		string hp = player.GetComponentInChildren<Player>().getHP().ToString();
		GuiHelper.StereoBox((int)screenWidth / 3, (int)screenHeight / 3, 60, 20, ref hp, Color.red);
		
		//Framerate
		if (showFramerate) GUI.Label(new Rect(600, 240, 400, 400), (1 / Time.deltaTime).ToString());

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

		//Overlay menu
		if (showingMenu) {
			GUI.Box(new Rect(0, 0, screenWidth, screenHeight), "", background);
			
			switch (menuSelection) {
			case 0:
				GuiHelper.StereoBox((int)(screenWidth / 2 - 50), (int)(screenHeight / 2 - 40), 100, 20, ref menuOptions[0], Color.red);
				GuiHelper.StereoBox((int)(screenWidth / 2 - 50), (int)(screenHeight / 2), 100, 20, ref menuOptions[1], Color.white);
				GuiHelper.StereoBox((int)(screenWidth / 2 - 50), (int)(screenHeight / 2 + 40), 100, 20, ref menuOptions[2], Color.white);
				break;
				
			case 1:
				GuiHelper.StereoBox((int)(screenWidth / 2 - 50), (int)(screenHeight / 2 - 40), 100, 20, ref menuOptions[0], Color.white);
				GuiHelper.StereoBox((int)(screenWidth / 2 - 50), (int)(screenHeight / 2), 100, 20, ref menuOptions[1], Color.red);
				GuiHelper.StereoBox((int)(screenWidth / 2 - 50), (int)(screenHeight / 2 + 40), 100, 20, ref menuOptions[2], Color.white);
				break;
				
			case 2:
				GuiHelper.StereoBox((int)(screenWidth / 2 - 50), (int)(screenHeight / 2 - 40), 100, 20, ref menuOptions[0], Color.white);
				GuiHelper.StereoBox((int)(screenWidth / 2 - 50), (int)(screenHeight / 2), 100, 20, ref menuOptions[1], Color.white);
				GuiHelper.StereoBox((int)(screenWidth / 2 - 50), (int)(screenHeight / 2 + 40), 100, 20, ref menuOptions[2], Color.red);
				break;
			}
		}
	}


	public void activateMenu() {
		Time.timeScale = 0;
		showingMenu = true;
	}
	
	public void deactivateMenu() {
		Time.timeScale = 1;
		showingMenu = false;
	}

	public void returnToMenu() {
		deactivateMenu();
		Application.LoadLevel(0);
	}


	private void handleMenuSelection() {
		switch (menuSelection) {
		case 0: //Continue
			deactivateMenu();
			break;
			
		case 1: //Main menu
			returnToMenu();
			break;
			
		case 2: //Exit
			Application.Quit();
			break;
		}
	}
}