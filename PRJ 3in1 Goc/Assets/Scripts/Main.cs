using System;
using System.Net.NetworkInformation;
using System.Threading;
using UnityEngine;

public class Main : MonoBehaviour
{
	public static Main main;

	public static mGraphics g;

	public static GameMidlet midlet;

	public static string res = "res";

	public static string mainThreadName;

	public static bool started;

	public static bool isIpod;

	public static bool isIphone4;

	public static bool isPC;

	public static bool isWindowsPhone;

	public static bool isIPhone;

	public static bool IphoneVersionApp;

	public static string IMEI;

	public static int versionIp;

	public static int numberQuit = 1;

	public static int typeClient = 4;

	public const sbyte PC_VERSION = 4;

	public const sbyte IP_APPSTORE = 5;

	public const sbyte WINDOWSPHONE = 6;

	private int level;

	public const sbyte IP_JB = 3;

	private int updateCount;

	private int paintCount;

	private int count;

	private int fps;

	private int max;

	private int up;

	private int upmax;

	private long timefps;

	private long timeup;

	private bool isRun;

	public static int waitTick;

	public static int f;

	public static bool isResume;

	public static bool isMiniApp = true;

	public static bool isQuitApp;

	private Vector2 lastMousePos;

	public static int a = 1;

	public static bool isCompactDevice = true;

	private void Start()
	{
		if (started)
			return;

		if (Thread.CurrentThread.Name != "Main")
			Thread.CurrentThread.Name = "Main";

		mainThreadName = Thread.CurrentThread.Name;
		isPC = Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer;
		isIPhone = (IphoneVersionApp = Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android);
		started = true;

		if (isPC && !isIPhone)
		{
			level = Rms.loadRMSInt("levelScreenKN");
			if (level == 1)
				Screen.SetResolution(720, 320, fullscreen: false);
			else
				Screen.SetResolution(1024, 600, fullscreen: false);
		}
		else if (isIPhone)
		{
			Screen.fullScreen = true;
			Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
			Application.targetFrameRate = 60;
			GameCanvas.isTouch = true;
			HideSystemUI();
		}

		try
		{
			if (mFont.tahoma_7 == null)
			{
				Debug.Log("Loading fonts before game init...");
				mFont.init(); // hoặc mFont.initFont() tùy bạn đang dùng
			}

			// if (Info= null)
			// {
			//     Debug.Log("Creating Info singleton...");
			//     new Info();
			// }

			if (InfoMe.gI() == null)
			{
				Debug.Log("Creating InfoMe singleton...");
				new InfoMe();
			}
		}
		catch (System.Exception ex)
		{
			Debug.LogError("Pre-initialization error: " + ex);
		}
		ModFunc.GI().LoadGame();
	}


	private void SetInit()
	{
		base.enabled = true;
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		if (hasFocus && (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer))
		{
			HideSystemUI();
		}
	}

	private void OnHideUnity(bool isGameShown)
	{
		if (!isGameShown)
		{
			Time.timeScale = 0f;
		}
		else
		{
			Time.timeScale = 1f;
		}
	}

	private void OnGUI()
	{
		if (count < 10)
		{
			return;
		}
		if (fps == 0)
		{
			timefps = mSystem.currentTimeMillis();
		}
		else if (mSystem.currentTimeMillis() - timefps > 1000)
		{
			max = fps;
			fps = 0;
			timefps = mSystem.currentTimeMillis();
		}
		fps++;
		checkInput();
		Session_ME.update();
		Session_ME2.update();
		if (Event.current.type.Equals(EventType.Repaint) && paintCount <= updateCount)
		{
			if (GameMidlet.gameCanvas != null)
			{
				GameMidlet.gameCanvas.paint(g);
			}
			paintCount++;
			if (g != null)
			{
				g.reset();
			}
		}
	}

	public void setsizeChange()
	{
		if (!isRun)
		{
			Screen.orientation = ScreenOrientation.AutoRotation;
			Application.runInBackground = true;
			base.useGUILayout = false;
			isCompactDevice = detectCompactDevice();
			if (main == null)
			{
				main = this;
			}
			isRun = true;
			ScaleGUI.initScaleGUI();
			if (isPC)
			{
				IMEI = SystemInfo.deviceUniqueIdentifier;
				Screen.fullScreen = false;
				typeClient = 4;
			}
			else
			{
				IMEI = GetMacAddress();
				Screen.fullScreen = true;
				HideSystemUI();
			}
			if (isWindowsPhone)
			{
				typeClient = 6;
			}
			if (isIPhone || IphoneVersionApp)
			{
				typeClient = 4;
			}
			if (iPhoneSettings.generation == iPhoneGeneration.iPodTouch4Gen)
			{
				isIpod = true;
			}
			if (iPhoneSettings.generation == iPhoneGeneration.iPhone4)
			{
				isIphone4 = true;
			}
			g = new mGraphics();
			midlet = new GameMidlet();
			TileMap.loadBg();
			Paint.loadbg();
			PopUp.loadBg();
			GameScr.loadBg();
			InfoMe.gI().loadCharId();
			Panel.loadBg();
			Menu.loadBg();
			Key.mapKeyPC();
			SoundMn.gI().loadSound(TileMap.mapID);
			g.CreateLineMaterial();
		}
	}

	public static void setBackupIcloud(string path)
	{
	}

	public string GetMacAddress()
	{
		_ = string.Empty;
		NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
		for (int i = 0; i < allNetworkInterfaces.Length; i++)
		{
			PhysicalAddress physicalAddress = allNetworkInterfaces[i].GetPhysicalAddress();
			if (physicalAddress.ToString() != string.Empty)
			{
				return physicalAddress.ToString();
			}
		}
		return string.Empty;
	}

	public void doClearRMS()
	{
		if (isPC && Rms.loadRMSInt("lastZoomlevel") != mGraphics.zoomLevel)
		{
			Rms.clearAll();
			Rms.saveRMSInt("lastZoomlevel", mGraphics.zoomLevel);
			Rms.saveRMSInt("levelScreenKN", level);
		}
	}

	public static void closeKeyBoard()
	{
		if (TouchScreenKeyboard.visible)
		{
			TField.kb.active = false;
			TField.kb = null;
		}
	}

	[Obsolete]
	private void FixedUpdate()
	{
		Rms.update();
		count++;
		if (count >= 10)
		{
			if (up == 0)
			{
				timeup = mSystem.currentTimeMillis();
			}
			else if (mSystem.currentTimeMillis() - timeup > 1000)
			{
				upmax = up;
				up = 0;
				timeup = mSystem.currentTimeMillis();
			}
			up++;
			setsizeChange();
			checkScreenSize();
			updateCount++;
			ipKeyboard.update();
			if (GameMidlet.gameCanvas != null)
			{
				GameMidlet.gameCanvas.update();
			}
			Image.update();
			DataInputStream.update();
			f++;
			if (f > 8)
			{
				f = 0;
			}
			if (!isPC)
			{
				_ = 1 / a;
			}
		}
	}

	private void checkScreenSize()
	{
		if (ScaleGUI.WIDTH != (float)Screen.width || ScaleGUI.HEIGHT != (float)Screen.height)
		{
			ScaleGUI.initScaleGUI();
			if (MotherCanvas.instance != null)
			{
				MotherCanvas.instance.checkZoomLevel((int)ScaleGUI.WIDTH, (int)ScaleGUI.HEIGHT);
				if (GameCanvas.instance != null)
				{
					GameCanvas.w = MotherCanvas.instance.getWidthz();
					GameCanvas.h = MotherCanvas.instance.getHeightz();
					GameCanvas.hw = GameCanvas.w / 2;
					GameCanvas.hh = GameCanvas.h / 2;
					GameCanvas.wd3 = GameCanvas.w / 3;
					GameCanvas.hd3 = GameCanvas.h / 3;
					GameCanvas.w2d3 = 2 * GameCanvas.w / 3;
					GameCanvas.h2d3 = 2 * GameCanvas.h / 3;
					GameCanvas.w3d4 = 3 * GameCanvas.w / 4;
					GameCanvas.h3d4 = 3 * GameCanvas.h / 4;
					GameCanvas.wd6 = GameCanvas.w / 6;
					GameCanvas.hd6 = GameCanvas.h / 6;
					GameScr.d = ((GameCanvas.w <= GameCanvas.h) ? GameCanvas.h : GameCanvas.w) + 20;
					if (GameCanvas.currentScreen != null)
					{
						GameCanvas.currentScreen.switchToMe();
					}
				}
			}
		}
	}

	private void checkInput()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Vector3 mousePosition = Input.mousePosition;
			GameMidlet.gameCanvas.pointerPressed((int)(mousePosition.x / (float)mGraphics.zoomLevel), (int)(((float)Screen.height - mousePosition.y) / (float)mGraphics.zoomLevel) + mGraphics.addYWhenOpenKeyBoard);
			lastMousePos.x = mousePosition.x / (float)mGraphics.zoomLevel;
			lastMousePos.y = mousePosition.y / (float)mGraphics.zoomLevel + (float)mGraphics.addYWhenOpenKeyBoard;
		}
		if (Input.GetMouseButton(0))
		{
			Vector3 mousePosition2 = Input.mousePosition;
			GameMidlet.gameCanvas.pointerDragged((int)(mousePosition2.x / (float)mGraphics.zoomLevel), (int)(((float)Screen.height - mousePosition2.y) / (float)mGraphics.zoomLevel) + mGraphics.addYWhenOpenKeyBoard);
			lastMousePos.x = mousePosition2.x / (float)mGraphics.zoomLevel;
			lastMousePos.y = mousePosition2.y / (float)mGraphics.zoomLevel + (float)mGraphics.addYWhenOpenKeyBoard;
		}
		if (Input.GetMouseButtonUp(0))
		{
			Vector3 mousePosition3 = Input.mousePosition;
			lastMousePos.x = mousePosition3.x / (float)mGraphics.zoomLevel;
			lastMousePos.y = mousePosition3.y / (float)mGraphics.zoomLevel + (float)mGraphics.addYWhenOpenKeyBoard;
			GameMidlet.gameCanvas.pointerReleased((int)(mousePosition3.x / (float)mGraphics.zoomLevel), (int)(((float)Screen.height - mousePosition3.y) / (float)mGraphics.zoomLevel) + mGraphics.addYWhenOpenKeyBoard);
		}
		if (Input.anyKeyDown && Event.current.type == EventType.KeyDown)
		{
			int num = MyKeyMap.map(Event.current.keyCode);
			if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
			{
				switch (Event.current.keyCode)
				{
					case KeyCode.Alpha2:
						num = 64;
						break;
					case KeyCode.Minus:
						num = 95;
						break;
				}
			}
			if (num != 0)
			{
				GameMidlet.gameCanvas.keyPressedz(num);
			}
		}
		if (Event.current.type == EventType.KeyUp)
		{
			int num2 = MyKeyMap.map(Event.current.keyCode);
			if (num2 != 0)
			{
				GameMidlet.gameCanvas.keyReleasedz(num2);
			}
		}
		if (isPC)
		{
			if (GameMidlet.gameCanvas == null || mGraphics.zoomLevel <= 0)
				return;

			float scroll = Input.GetAxis("Mouse ScrollWheel") * 10f;
			if (Mathf.Abs(scroll) > 0.01f)
			{
				GameMidlet.gameCanvas.scrollMouse((int)scroll);
			}

			float x = Input.mousePosition.x;
			float y = Input.mousePosition.y;

			int x2 = (int)x / mGraphics.zoomLevel;
			int y2 = (Screen.height - (int)y) / mGraphics.zoomLevel;

			GameMidlet.gameCanvas.pointerMouse(x2, y2);
		}

	}

	private void OnApplicationQuit()
	{
		Debug.LogWarning("APP QUIT");
		GameCanvas.bRun = false;
		Session_ME.gI().close();
		Session_ME2.gI().close();
		if (isPC)
		{
			Application.Quit();
		}
	}

	private void OnApplicationPause(bool paused)
	{
		isResume = false;
		if (paused)
		{
			if (GameCanvas.isWaiting())
			{
				isQuitApp = true;
			}
		}
		else
		{
			isResume = true;
			if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
			{
				HideSystemUI();
			}
		}
		if (TouchScreenKeyboard.visible)
		{
			TField.kb.active = false;
			TField.kb = null;
		}
		if (isQuitApp)
		{
			Application.Quit();
		}
	}

	public static void exit()
	{
		if (isPC)
		{
			main.OnApplicationQuit();
		}
		else
		{
			a = 0;
		}
	}

	public static bool detectCompactDevice()
	{
		if (iPhoneSettings.generation == iPhoneGeneration.iPhone || iPhoneSettings.generation == iPhoneGeneration.iPhone3G || iPhoneSettings.generation == iPhoneGeneration.iPodTouch1Gen || iPhoneSettings.generation == iPhoneGeneration.iPodTouch2Gen)
		{
			return false;
		}
		return true;
	}

	public static bool checkCanSendSMS()
	{
		if (iPhoneSettings.generation == iPhoneGeneration.iPhone3GS || iPhoneSettings.generation == iPhoneGeneration.iPhone4 || iPhoneSettings.generation > iPhoneGeneration.iPodTouch4Gen)
		{
			return true;
		}
		return false;
	}

	public static void HideSystemUI()
	{
		Screen.fullScreen = true;
		Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
#if UNITY_ANDROID && !UNITY_EDITOR
		try
		{
			using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			using (AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
			{
				activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
				{
					using (AndroidJavaObject window = activity.Call<AndroidJavaObject>("getWindow"))
					{
						try
						{
							using (AndroidJavaClass buildVersion = new AndroidJavaClass("android.os.Build$VERSION"))
							{
								int sdkInt = buildVersion.GetStatic<int>("SDK_INT");
								if (sdkInt >= 28)
								{
									using (AndroidJavaObject layoutParams = window.Call<AndroidJavaObject>("getAttributes"))
									{
										// LAYOUT_IN_DISPLAY_CUTOUT_MODE_SHORT_EDGES = 1
										layoutParams.Set("layoutInDisplayCutoutMode", 1);
										window.Call("setAttributes", layoutParams);
									}
								}

								if (sdkInt >= 30)
								{
									using (AndroidJavaObject insetsController = window.Call<AndroidJavaObject>("getInsetsController"))
									{
										if (insetsController != null)
										{
											// WindowInsets.Type.statusBars() (1) | navigationBars() (2) | captionBar() (4) = 7
											insetsController.Call("hide", 7);
											// WindowInsetsController.BEHAVIOR_SHOW_TRANSIENT_BARS_BY_SWIPE = 2
											insetsController.Call("setSystemBarsBehavior", 2);
										}
									}
								}
							}
						}
						catch (System.Exception exCutout)
						{
							Debug.LogWarning("Cutout/Insets mode error: " + exCutout.Message);
						}

						try
						{
							window.Call("clearFlags", 2048); // FLAG_FORCE_NOT_FULLSCREEN
							// FLAG_FULLSCREEN (1024) | FLAG_LAYOUT_NO_LIMITS (512) | FLAG_LAYOUT_IN_SCREEN (256)
							window.Call("addFlags", 1024 | 512 | 256);
						}
						catch (System.Exception) {}

						try
						{
							using (AndroidJavaObject decorView = window.Call<AndroidJavaObject>("getDecorView"))
							{
								// SYSTEM_UI_FLAG_LAYOUT_STABLE (256) | SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION (512) |
								// SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN (1024) | SYSTEM_UI_FLAG_HIDE_NAVIGATION (2) |
								// SYSTEM_UI_FLAG_FULLSCREEN (4) | SYSTEM_UI_FLAG_IMMERSIVE_STICKY (4096)
								decorView.Call("setSystemUiVisibility", 5894);
							}
						}
						catch (System.Exception) {}
					}
				}));
			}
		}
		catch (System.Exception ex)
		{
			Debug.LogWarning("HideSystemUI error: " + ex.Message);
		}
#endif
	}
}
