# TODO: provide the Unity editor directory
UNITY_DIRECTORY=/home/exampleuser/Unity/Hub/Editor/2022.3.37f1/Editor/Unity 
BUILD_DIRECTORY=$(shell pwd)/Build
build: | $(BUILD_DIRECTORY)
	$(UNITY_DIRECTORY) -quit -batchmode -nographics -projectPath $(shell pwd) -executeMethod CLI.BuildCLI.Build
clean:
	rm -rf $(BUILD_DIRECTORY)
run: $(BUILD_DIRECTORY)/StandaloneLinux64
	$(shell pwd)/Build/StandaloneLinux64

$(BUILD_DIRECTORY):
	mkdir -p $@
$(BUILD_DIRECTORY)/StandaloneLinux64: | build
