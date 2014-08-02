using UnityEngine;
using System.Collections;

public class GUIController : MonoBehaviour
{
	public Texture minimap;
	public float minimapScale = 1.0f;
	public int minimapPadding = 32;
	public Texture minimapIndicator;
	public float indicatorScale = 1.0f;
	public float gameworldSize = 100; //size in the ortho minimap camera used to get the minimap texture
	public float minimapAlpha = 0.75f;

	const int minimapBaseSize = 256;
	const int indicatorBaseSize = 32;

	int screenWidth, screenHeight;
	float screenRatio;
	Rect minimapLocation;
	int minimapSize, indicatorSize;
	GameObject player;
	Color guiColor = Color.white;

	void Start() {
		player = GameObject.Find("First Person Controller");

		screenWidth = Screen.width;
		screenHeight = Screen.height;

		screenRatio = screenWidth / 1920.0f;

		minimapSize = (int)(minimapBaseSize * screenRatio * minimapScale);
		indicatorSize = (int)(indicatorBaseSize * screenRatio * indicatorScale);
		minimapPadding = (int)(minimapPadding * screenRatio);

		guiColor.a = minimapAlpha;


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

	void OnGUI() {
		GUI.color = guiColor;

		//Draw minimap
		GUI.DrawTexture(minimapLocation, minimap, ScaleMode.ScaleToFit);

		//Draw player position indicator
		Rect indicatorLocation = new Rect(minimapLocation.left + minimapLocation.width / 2 + (player.transform.position.x / gameworldSize * minimapSize / 2) - indicatorSize / 2, minimapLocation.top + minimapLocation.height / 2 - (player.transform.position.z / gameworldSize * minimapSize / 2) - indicatorSize / 2, indicatorSize, indicatorSize);
		Matrix4x4 backup = GUI.matrix;
		GUIUtility.RotateAroundPivot(player.transform.eulerAngles.y, new Vector2(indicatorLocation.x + indicatorLocation.width / 2, indicatorLocation.y + indicatorLocation.height / 2));
		GUI.DrawTexture(indicatorLocation, minimapIndicator, ScaleMode.ScaleToFit);
		GUI.matrix = backup;

		GUI.color = Color.white;
	}
}