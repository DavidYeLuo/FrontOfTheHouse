using UnityEditor;
using UnityEngine;
namespace CLI {
public class BuildCLI {
  public static void Build() {
    BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
    buildPlayerOptions.scenes =
        new[] { "Assets/Scenes/NewHire_ShallowEllum_Survival.unity",
                "Assets/Scenes/FirstPrototype/FirstLevel.unity",
                "Assets/Scenes/MainMenu.unity" };
    buildPlayerOptions.locationPathName = "./Build/StandaloneLinux64";
    buildPlayerOptions.target = BuildTarget.StandaloneLinux64;
    // buildPlayerOptions.options =
    //     BuildOptions.CleanBuildCache | BuildOptions.StrictMode;
    buildPlayerOptions.options =
        BuildOptions.StrictMode | BuildOptions.Development;
    BuildPipeline.BuildPlayer(buildPlayerOptions);
  }
}
}
