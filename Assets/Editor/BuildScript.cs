// AtomCoLab-VR - Crystal Structure Visualization for Meta Quest
// Copyright (c) 2024 AtomCoLab-VR Contributors
// Licensed under PolyForm Noncommercial 1.0.0 (see LICENSE)
// For commercial licensing, see LICENSE-COMMERCIAL.md

using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BuildScript
{
    private const string AppName = "AtomCoLab-VR";

    [MenuItem("Build/Build Android (Quest)")]
    public static void BuildAndroid()
    {
        var options = new BuildPlayerOptions
        {
            scenes = GetEnabledScenes(),
            locationPathName = $"build/Android/{AppName}.apk",
            target = BuildTarget.Android,
            options = BuildOptions.None
        };

        ConfigureAndroidSettings();

        var report = BuildPipeline.BuildPlayer(options);
        HandleBuildReport(report);
    }

    [MenuItem("Build/Build Android Development")]
    public static void BuildAndroidDevelopment()
    {
        var options = new BuildPlayerOptions
        {
            scenes = GetEnabledScenes(),
            locationPathName = $"build/Android/{AppName}-dev.apk",
            target = BuildTarget.Android,
            options = BuildOptions.Development | BuildOptions.AllowDebugging
        };

        ConfigureAndroidSettings();

        var report = BuildPipeline.BuildPlayer(options);
        HandleBuildReport(report);
    }

    private static string[] GetEnabledScenes()
    {
        return EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => s.path)
            .ToArray();
    }

    private static void ConfigureAndroidSettings()
    {
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel29;
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel32;
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;

        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
        EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
    }

    private static void HandleBuildReport(BuildReport report)
    {
        var summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"Build succeeded: {summary.totalSize / (1024 * 1024):F2} MB");
            Debug.Log($"Build time: {summary.totalTime}");
            Debug.Log($"Output: {summary.outputPath}");
        }
        else if (summary.result == BuildResult.Failed)
        {
            Debug.LogError("Build failed!");
            foreach (var step in report.steps)
            {
                foreach (var message in step.messages)
                {
                    if (message.type == LogType.Error || message.type == LogType.Exception)
                    {
                        Debug.LogError(message.content);
                    }
                }
            }

            if (Application.isBatchMode)
            {
                EditorApplication.Exit(1);
            }
        }
    }
}
