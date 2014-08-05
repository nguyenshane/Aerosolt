using UnityEngine;
using System.Collections;

/*
 * Handles all GUI drawing:
 * Minimap
 * Reticule
 * Health and ammo indicators (TBA)
 * Objectives (TBA)
 */

public class GUIController : MonoBehaviour
{
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

	GameObject GUIRenderObject;
	RenderTexture GUIRenderTexture;
	OVRGUI guiHelper;
	// Handle to OVRCameraController
	private OVRCameraController CameraController = null;
	// Handle to OVRPlayerController
	private OVRPlayerController PlayerController = null;

	Color minimapTint = Color.white;

	void Awake()
	{
		// Find camera controller
		OVRCameraController[] CameraControllers;
		CameraControllers = gameObject.GetComponentsInChildren<OVRCameraController>();
		
		if(CameraControllers.Length == 0)
			Debug.LogWarning("OVRMainMenu: No OVRCameraController attached.");
		else if (CameraControllers.Length > 1)
			Debug.LogWarning("OVRMainMenu: More then 1 OVRCameraController attached.");
		else
			CameraController = CameraControllers[0];
		
		// Find player controller
		OVRPlayerController[] PlayerControllers;
		PlayerControllers = gameObject.GetComponentsInChildren<OVRPlayerController>();
		
		if(PlayerControllers.Length == 0)
			Debug.LogWarning("OVRMainMenu: No OVRPlayerController attached.");
		else if (PlayerControllers.Length > 1)
			Debug.LogWarning("OVRMainMenu: More then 1 OVRPlayerController attached.");
		else
			PlayerController = PlayerControllers[0];
		
	}

	void Start() {
		player = GameObject.Find("First Person Controller");
		GUIRenderObject = GameObject.Instantiate(Resources.Load("OVRGUIObjectMain")) as GameObject;
		guiHelper = new OVRGUI();

		if(GUIRenderTexture == null)
		{
			int w = Screen.width;
			int h = Screen.height;
			
			// We don't need a depth buffer on this texture
			GUIRenderTexture = new RenderTexture(w, h, 0);	
			guiHelper.SetPixelResolution(w, h);
			// NOTE: All GUI elements are being written with pixel values based
			// from DK1 (1280x800). These should change to normalized locations so 
			// that we can scale more cleanly with varying resolutions
			//GuiHelper.SetDisplayResolution(OVRDevice.HResolution, 
			//								 OVRDevice.VResolution);
			guiHelper.SetDisplayResolution(1280.0f, 800.0f);
		}

		// Attach GUI texture to GUI object and GUI object to Camera
		if(GUIRenderTexture != null && GUIRenderObject != null)
		{
			GUIRenderObject.renderer.material.mainTexture = GUIRenderTexture;
			
			if(CameraController != null)
			{
				// Grab transform of GUI object
				Vector3 ls = GUIRenderObject.transform.localScale;
				Vector3 lp = GUIRenderObject.transform.localPosition;
				Quaternion lr = GUIRenderObject.transform.localRotation;
				
				// Attach the GUI object to the camera
				CameraController.AttachGameObjectToCamera(ref GUIRenderObject);
				// Reset the transform values (we will be maintaining state of the GUI object
				// in local state)
				
				GUIRenderObject.transform.localScale = ls;
				GUIRenderObject.transform.localRotation = lr;
				
				// Deactivate object until we have completed the fade-in
				// Also, we may want to deactive the render object if there is nothing being rendered
				// into the UI
				// we will move the position of everything over to the left, so get
				// IPD / 2 and position camera towards negative X
				float   ipd = 0.0f;
				CameraController.GetIPD(ref ipd);
				lp.x -= ipd * 0.5f;
				GUIRenderObject.transform.localPosition = lp;
				
				GUIRenderObject.SetActive(false);
			}
		}

		GUIRenderObject.SetActive(true);

		screenWidth = Screen.width;
		screenHeight = Screen.height;

		screenRatio = screenWidth / 1920.0f;

		minimapSize = (int)(minimapBaseSize * screenRatio * minimapScale);
		indicatorSize = (int)(indicatorBaseSize * screenRatio * indicatorScale);
		reticuleSize = (int)(reticuleBaseSize * screenRatio * reticuleScale);
		minimapPadding = (int)(minimapPadding * screenRatio);

		minimapTint.a = minimapAlpha;


		//Top right
		//minimapLocation = new Rect(screenWidth - minimapSize - minimapPadding, 0 + minimapPadding, minimapSize, minimapSize);

		//Bottom right
		minimapLocation = new Rect(screenWidth - minimapSize - minimapPadding, screenHeight - minimapPadding - minimapSize, minimapSize, minimapSize);

		//Bottom left
		//minimapLocation = new Rect(0 + minimapPadding, screenHeight - minimapPadding - minimapSize, minimapSize, minimapSize);

		//Top left
		//minimapLocation = new Rect(0 + minimapPadding, 0 + minimapPadding, minimapSize, minimapSize);
	}
	
	void Update() {

	}

	/*
	public override void OnVRGUI()
	{
		GUILayout.BeginArea(new Rect(0f, 0f, Screen.width, Screen.height));

		GUI.DrawTexture(minimapLocation, minimap, ScaleMode.ScaleToFit);

		GUILayout.EndArea();
	}
*/

	void OnGUI() {
		GUI.color = minimapTint;

		//Draw minimap
		GUI.DrawTexture(minimapLocation, minimap, ScaleMode.ScaleToFit);

		GUI.color = Color.white;

		//Draw player position indicator
		Rect indicatorLocation = new Rect(minimapLocation.left + minimapLocation.width / 2 + (player.transform.position.x / gameworldSize * minimapSize / 2) - indicatorSize / 2, minimapLocation.top + minimapLocation.height / 2 - (player.transform.position.z / gameworldSize * minimapSize / 2) - indicatorSize / 2, indicatorSize, indicatorSize);
		Matrix4x4 backup = GUI.matrix;
		GUIUtility.RotateAroundPivot(player.transform.eulerAngles.y, new Vector2(indicatorLocation.x + indicatorLocation.width / 2, indicatorLocation.y + indicatorLocation.height / 2));
		GUI.DrawTexture(indicatorLocation, minimapIndicator, ScaleMode.ScaleToFit);
		GUI.matrix = backup;

		//Draw reticule
		GUI.DrawTexture(new Rect(screenWidth / 2 - reticuleSize / 2, screenHeight / 2 - reticuleSize / 2, reticuleSize, reticuleSize), reticule, ScaleMode.ScaleToFit);

/*
		//Oculus stuff

		// Set the GUI matrix to deal with portrait mode
		Vector3 scale = Vector3.one;
		Matrix4x4 svMat = GUI.matrix; // save current matrix
		// substitute matrix - only scale is altered from standard
		GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, scale);
		
		// Cache current active render texture
		RenderTexture previousActive = RenderTexture.active;
		
		// if set, we will render to this texture
		if(GUIRenderTexture != null)
		{
			RenderTexture.active = GUIRenderTexture;
			GL.Clear (false, true, new Color (0.0f, 0.0f, 0.0f, 0.0f));
		}
		
		// Update OVRGUI functions (will be deprecated eventually when 2D renderingc
		// is removed from GUI)
		//guiHelper.SetFontReplace(FontReplace);
		
		// Restore active render texture
		RenderTexture.active = previousActive;
		
		// ***
		// Restore previous GUI matrix
		GUI.matrix = svMat;


		string str = "text";
		//guiHelper.StereoDrawTexture(Screen.width / 4, Screen.height / 2, minimapLocation.width, minimapLocation.height, ref minimap, Color.white);
		guiHelper.StereoBox(Screen.width / 4, Screen.height / 2, 200, 200, ref str, Color.white);
		*/
	}
}