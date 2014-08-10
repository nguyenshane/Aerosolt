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

	public GUIStyle background, health, ammo, activeMenuItem, inactiveMenuItem;

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

	readonly string[] menuOptions = {"Continue", "Restart", "Main Menu", "Exit"};
	readonly string[] deathOptions = {"Restart", "Main Menu", "Exit"};
	string deathString = "Death!";
	const int menuItemSpacing = 80;
	const float inputRepeatDelay = 0.5f;

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
	Rect menuItem;
	bool showingMenu = false;
	bool showingDeathScreen = false;
	
	GameObject player;
	Player playerScript;


	void Start() {
		Screen.showCursor = false;

		inputRepeatTimer = selectionInputRepeatTimer = activationInputRepeatTimer = inputRepeatDelay;

		player = GameObject.Find("First Person Controller");
		playerScript = player.GetComponentInChildren<Player>();

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
			ammo.fontSize = (int)(ammo.fontSize * screenRatio);
			activeMenuItem.fontSize = (int)(activeMenuItem.fontSize * screenRatio);
			inactiveMenuItem.fontSize = (int)(inactiveMenuItem.fontSize * screenRatio);
			spacing = (int)(menuItemSpacing * screenRatio);

			menuItem = new Rect(screenWidth / 2 - 100 * screenRatio, screenHeight / 2 - 20 * screenRatio, 200 * screenRatio, 40 * screenRatio);

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
			spacing = 60;

			menuItem = new Rect((int)(screenWidth / 2 - 50), (int)(screenHeight / 2 - 10), 100, 20);

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
			handleMenuInput(menuOptions.Length);
		} else if (showingDeathScreen) {
			handleMenuInput(deathOptions.Length);
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
		GUI.Label(new Rect((int)screenWidth / 4, (int)screenHeight / 4, 60 * screenRatio, 20 * screenRatio), playerScript.getHP().ToString("F0"), health);

		//Draw ammo
		GUI.Label(new Rect((int)screenWidth / 4, (int)screenHeight / 4 + spacing, 60 * screenRatio, 20 * screenRatio), playerScript.getAmmo().ToString("F0"), ammo);
		
		//Draw framerate
		if (showFramerate) GUI.Label(new Rect(32 * screenRatio, 32 * screenRatio, 400 * screenRatio, 400 * screenRatio), (1 / Time.deltaTime).ToString("F4"));

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
				GUI.Label(new Rect(menuItem.x, menuItem.y - spacing * 1.5f, menuItem.width, menuItem.height), menuOptions[0], activeMenuItem);
				GUI.Label(new Rect(menuItem.x, menuItem.y - spacing * 0.5f, menuItem.width, menuItem.height), menuOptions[1], inactiveMenuItem);
				GUI.Label(new Rect(menuItem.x, menuItem.y + spacing * 0.5f, menuItem.width, menuItem.height), menuOptions[2], inactiveMenuItem);
				GUI.Label(new Rect(menuItem.x, menuItem.y + spacing * 1.5f, menuItem.width, menuItem.height), menuOptions[3], inactiveMenuItem);
				break;

			case 1:
				GUI.Label(new Rect(menuItem.x, menuItem.y - spacing * 1.5f, menuItem.width, menuItem.height), menuOptions[0], inactiveMenuItem);
				GUI.Label(new Rect(menuItem.x, menuItem.y - spacing * 0.5f, menuItem.width, menuItem.height), menuOptions[1], activeMenuItem);
				GUI.Label(new Rect(menuItem.x, menuItem.y + spacing * 0.5f, menuItem.width, menuItem.height), menuOptions[2], inactiveMenuItem);
				GUI.Label(new Rect(menuItem.x, menuItem.y + spacing * 1.5f, menuItem.width, menuItem.height), menuOptions[3], inactiveMenuItem);
				break;

			case 2:
				GUI.Label(new Rect(menuItem.x, menuItem.y - spacing * 1.5f, menuItem.width, menuItem.height), menuOptions[0], inactiveMenuItem);
				GUI.Label(new Rect(menuItem.x, menuItem.y - spacing * 0.5f, menuItem.width, menuItem.height), menuOptions[1], inactiveMenuItem);
				GUI.Label(new Rect(menuItem.x, menuItem.y + spacing * 0.5f, menuItem.width, menuItem.height), menuOptions[2], activeMenuItem);
				GUI.Label(new Rect(menuItem.x, menuItem.y + spacing * 1.5f, menuItem.width, menuItem.height), menuOptions[3], inactiveMenuItem);
				break;

			case 3:
				GUI.Label(new Rect(menuItem.x, menuItem.y - spacing * 1.5f, menuItem.width, menuItem.height), menuOptions[0], inactiveMenuItem);
				GUI.Label(new Rect(menuItem.x, menuItem.y - spacing * 0.5f, menuItem.width, menuItem.height), menuOptions[1], inactiveMenuItem);
				GUI.Label(new Rect(menuItem.x, menuItem.y + spacing * 0.5f, menuItem.width, menuItem.height), menuOptions[2], inactiveMenuItem);
				GUI.Label(new Rect(menuItem.x, menuItem.y + spacing * 1.5f, menuItem.width, menuItem.height), menuOptions[3], activeMenuItem);
				break;
			}
		} else if (showingDeathScreen) {
			GUI.Box(new Rect(0, 0, screenWidth, screenHeight), "", background);
			
			GUI.Label(new Rect(menuItem.x, menuItem.y - spacing * 2.0f, menuItem.width, menuItem.height), deathString, health);

			switch (menuSelection) {
			case 0:
				GUI.Label(new Rect(menuItem.x, menuItem.y - spacing * 0.0f, menuItem.width, menuItem.height), deathOptions[0], activeMenuItem);
				GUI.Label(new Rect(menuItem.x, menuItem.y + spacing * 1.0f, menuItem.width, menuItem.height), deathOptions[1], inactiveMenuItem);
				GUI.Label(new Rect(menuItem.x, menuItem.y + spacing * 2.0f, menuItem.width, menuItem.height), deathOptions[2], inactiveMenuItem);
				break;
				
			case 1:
				GUI.Label(new Rect(menuItem.x, menuItem.y - spacing * 0.0f, menuItem.width, menuItem.height), deathOptions[0], inactiveMenuItem);
				GUI.Label(new Rect(menuItem.x, menuItem.y + spacing * 1.0f, menuItem.width, menuItem.height), deathOptions[1], activeMenuItem);
				GUI.Label(new Rect(menuItem.x, menuItem.y + spacing * 2.0f, menuItem.width, menuItem.height), deathOptions[2], inactiveMenuItem);
				break;
				
			case 2:
				GUI.Label(new Rect(menuItem.x, menuItem.y - spacing * 0.0f, menuItem.width, menuItem.height), deathOptions[0], inactiveMenuItem);
				GUI.Label(new Rect(menuItem.x, menuItem.y + spacing * 1.0f, menuItem.width, menuItem.height), deathOptions[1], inactiveMenuItem);
				GUI.Label(new Rect(menuItem.x, menuItem.y + spacing * 2.0f, menuItem.width, menuItem.height), deathOptions[2], activeMenuItem);
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
		string hp = playerScript.getHP().ToString("F0");
		GuiHelper.StereoBox((int)screenWidth / 3, (int)screenHeight / 4, 60, 20, ref hp, Color.red);

		//Ammo
		string ammo = playerScript.getAmmo().ToString("F0");
		GuiHelper.StereoBox((int)screenWidth / 3, (int)screenHeight / 4 + 40, 60, 20, ref ammo, Color.blue);
		
		//Framerate
		if (showFramerate) GUI.Label(new Rect(600, 240, 400, 400), (1 / Time.deltaTime).ToString("F4"));

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
			GUI.Box(new Rect(0, 0, (int)screenWidth, (int)screenHeight), "", background);

			switch (menuSelection) {
			case 0:
				GuiHelper.StereoBox((int)menuItem.x, (int)(menuItem.y - 60), (int)menuItem.width, (int)menuItem.height, ref menuOptions[0], Color.red);
				GuiHelper.StereoBox((int)menuItem.x, (int)(menuItem.y - 20), (int)menuItem.width, (int)menuItem.height, ref menuOptions[1], Color.white);
				GuiHelper.StereoBox((int)menuItem.x, (int)(menuItem.y + 20), (int)menuItem.width, (int)menuItem.height, ref menuOptions[2], Color.white);
				GuiHelper.StereoBox((int)menuItem.x, (int)(menuItem.y + 60), (int)menuItem.width, (int)menuItem.height, ref menuOptions[3], Color.white);
				break;
				
			case 1:
				GuiHelper.StereoBox((int)menuItem.x, (int)(menuItem.y - 60), (int)menuItem.width, (int)menuItem.height, ref menuOptions[0], Color.white);
				GuiHelper.StereoBox((int)menuItem.x, (int)(menuItem.y - 20), (int)menuItem.width, (int)menuItem.height, ref menuOptions[1], Color.red);
				GuiHelper.StereoBox((int)menuItem.x, (int)(menuItem.y + 20), (int)menuItem.width, (int)menuItem.height, ref menuOptions[2], Color.white);
				GuiHelper.StereoBox((int)menuItem.x, (int)(menuItem.y + 60), (int)menuItem.width, (int)menuItem.height, ref menuOptions[3], Color.white);
				break;
				
			case 2:
				GuiHelper.StereoBox((int)menuItem.x, (int)(menuItem.y - 60), (int)menuItem.width, (int)menuItem.height, ref menuOptions[0], Color.white);
				GuiHelper.StereoBox((int)menuItem.x, (int)(menuItem.y - 20), (int)menuItem.width, (int)menuItem.height, ref menuOptions[1], Color.white);
				GuiHelper.StereoBox((int)menuItem.x, (int)(menuItem.y + 20), (int)menuItem.width, (int)menuItem.height, ref menuOptions[2], Color.red);
				GuiHelper.StereoBox((int)menuItem.x, (int)(menuItem.y + 60), (int)menuItem.width, (int)menuItem.height, ref menuOptions[3], Color.white);
				break;

			case 3:
				GuiHelper.StereoBox((int)menuItem.x, (int)(menuItem.y - 60), (int)menuItem.width, (int)menuItem.height, ref menuOptions[0], Color.white);
				GuiHelper.StereoBox((int)menuItem.x, (int)(menuItem.y - 20), (int)menuItem.width, (int)menuItem.height, ref menuOptions[1], Color.white);
				GuiHelper.StereoBox((int)menuItem.x, (int)(menuItem.y + 20), (int)menuItem.width, (int)menuItem.height, ref menuOptions[2], Color.white);
				GuiHelper.StereoBox((int)menuItem.x, (int)(menuItem.y + 60), (int)menuItem.width, (int)menuItem.height, ref menuOptions[3], Color.red);
				break;
			}
		} else if (showingDeathScreen) {
			GUI.Box(new Rect((int)0, (int)0, (int)screenWidth, (int)screenHeight), "", background);

			GuiHelper.StereoBox((int)menuItem.x, (int)(menuItem.y - 80), (int)menuItem.width, (int)menuItem.height, ref deathString, Color.red);

			switch (menuSelection) {
			case 0:
				GuiHelper.StereoBox((int)menuItem.x, (int)(menuItem.y + 0), (int)menuItem.width, (int)menuItem.height, ref deathOptions[0], Color.red);
				GuiHelper.StereoBox((int)menuItem.x, (int)(menuItem.y + 40), (int)menuItem.width, (int)menuItem.height, ref deathOptions[1], Color.white);
				GuiHelper.StereoBox((int)menuItem.x, (int)(menuItem.y + 80), (int)menuItem.width, (int)menuItem.height, ref deathOptions[2], Color.white);
				break;
				
			case 1:
				GuiHelper.StereoBox((int)menuItem.x, (int)(menuItem.y + 0), (int)menuItem.width, (int)menuItem.height, ref deathOptions[0], Color.white);
				GuiHelper.StereoBox((int)menuItem.x, (int)(menuItem.y + 40), (int)menuItem.width, (int)menuItem.height, ref deathOptions[1], Color.red);
				GuiHelper.StereoBox((int)menuItem.x, (int)(menuItem.y + 80), (int)menuItem.width, (int)menuItem.height, ref deathOptions[2], Color.white);
				break;
				
			case 2:
				GuiHelper.StereoBox((int)menuItem.x, (int)(menuItem.y + 0), (int)menuItem.width, (int)menuItem.height, ref deathOptions[0], Color.white);
				GuiHelper.StereoBox((int)menuItem.x, (int)(menuItem.y + 40), (int)menuItem.width, (int)menuItem.height, ref deathOptions[1], Color.white);
				GuiHelper.StereoBox((int)menuItem.x, (int)(menuItem.y + 80), (int)menuItem.width, (int)menuItem.height, ref deathOptions[2], Color.red);
				break;
			}
		}
	}


	//Activates the main overlay menu
	public void activateMenu() {
		player.GetComponent<MouseLook>().enabled = false;
		Time.timeScale = 0;
		showingMenu = true;
	}

	//Deactivates all menus
	public void deactivateMenu() {
		menuSelection = 0;
		player.GetComponent<MouseLook>().enabled = true;
		Time.timeScale = 1;
		showingMenu = false;
		showingDeathScreen = false;
	}

	//Loads the main menu
	public void returnToMenu() {
		deactivateMenu();
		Application.LoadLevel(0);
	}

	public void activateDeathScreen() {
		player.GetComponent<MouseLook>().enabled = false;
		Time.timeScale = 0;
		showingMenu = false; //should really use an enum for menu state instead but w/e
		showingDeathScreen = true;
	}


	private void handleMenuInput(int menuOptionsLength) {
		//Selection axes
		if (Input.GetAxisRaw("Menu Navigation") > 0) {
			//Up selection
			if (prevMenuInputState != 1 || inputRepeatTimer <= 0) {
				inputRepeatTimer = inputRepeatDelay;
				menuSelection--;
				if (menuSelection < 0) menuSelection = menuOptionsLength-1;
			}
			
			prevMenuInputState = 1;
		} else if (Input.GetAxisRaw("Menu Navigation") < 0) {
			//Down selection
			if (prevMenuInputState != -1 || inputRepeatTimer <= 0) {
				inputRepeatTimer = inputRepeatDelay;
				menuSelection++;
				if (menuSelection >= menuOptionsLength) menuSelection = 0;
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
	}

	private void handleMenuSelection() {
		if (showingMenu) {
			switch (menuSelection) {
			case 0: //Continue
				deactivateMenu();
				break;

			case 1: //Restart
				deactivateMenu();
				Application.LoadLevel(Application.loadedLevel);
				break;
				
			case 2: //Main menu
				returnToMenu();
				break;
				
			case 3: //Exit
				Application.Quit();
				break;
			}
		} else {
			switch (menuSelection) {
			case 0: //Restart
			deactivateMenu();
			Application.LoadLevel(Application.loadedLevel);
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
}