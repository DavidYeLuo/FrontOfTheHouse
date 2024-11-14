# TODO: provide the Unity editor directory
UNITY_DIRECTORY=/home/exampleuser/Unity/Hub/Editor/2022.3.37f1/Editor/Unity 
build:
	$(UNITY_DIRECTORY) -quit -batchmode -nographics -projectPath $(shell pwd) -executeMethod CLI.BuildCLI.Build
run:
	$(shell pwd)/Build/StandaloneLinux64
