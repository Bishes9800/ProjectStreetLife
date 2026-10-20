#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class StreetLifeCloudBuild
{
    public static void BuildAndroid()
    {
        // Build the prototype scene from the included editor setup.
        StreetLifeSceneSetup.BuildScene();
        var scenePath = "Assets/StreetLifePrototype.unity";
        if (!File.Exists(scenePath))
            throw new Exception("Prototype scene was not created. Check StreetLifeSceneSetup errors in the Unity build log.");

        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(scenePath, true) };
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        PlayerSettings.productName = "Project StreetLife";
        PlayerSettings.applicationIdentifier = "com.projectstreetlife.game";
        PlayerSettings.bundleVersion = "0.1.0";
        PlayerSettings.Android.bundleVersionCode = 1;
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.activeInputHandler = 0; // Legacy Input Manager used by starter scripts.

        var output = Environment.GetEnvironmentVariable("STREETLIFE_APK_PATH");
        if (String.IsNullOrEmpty(output)) output = "Build/ProjectStreetLife.apk";
        Directory.CreateDirectory(Path.GetDirectoryName(output));
        var options = new BuildPlayerOptions
        {
            scenes = new[] { scenePath },
            locationPathName = output,
            target = BuildTarget.Android,
            options = BuildOptions.None
        };
        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result != BuildResult.Succeeded)
            throw new Exception("Android build failed: " + report.summary.result + "; errors=" + report.summary.totalErrors);
        Debug.Log("PROJECT_STREETLIFE_APK_OUTPUT=" + output);
    }
}
#endif
