using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

[InitializeOnLoad]
public class AutoSetOrientation : IPreprocessBuildWithReport
{
    static AutoSetOrientation()
    {
        SetOrientationSettings();
    }

    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        SetOrientationSettings();
    }

    private static void SetOrientationSettings()
    {
        // Áp dụng cho cả Android và iOS
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
        PlayerSettings.allowedAutorotateToPortrait = false;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.allowedAutorotateToLandscapeLeft = true;
        PlayerSettings.allowedAutorotateToLandscapeRight = true;
        PlayerSettings.statusBarHidden = true;

        // Android: Ẩn thanh status bar (pin, giờ) và tràn viền qua vùng tai thỏ / camera cutout
        PlayerSettings.Android.renderOutsideSafeArea = true;
        PlayerSettings.Android.maxAspectRatio = 2.5f;

#if UNITY_IOS
        // Cấu hình cho iOS
        PlayerSettings.iOS.showActivityIndicatorOnLoading = iOSShowActivityIndicatorOnLoading.DontShow;
        PlayerSettings.iOS.hideHomeButton = true;
        PlayerSettings.iOS.deferSystemGesturesMode = IOSDeferSystemGesturesMode.AllEdges;
#endif
        Debug.Log("🔄 Setting default orientation to Landscape & Immersive Fullscreen (Android & iOS)");
    }
}
