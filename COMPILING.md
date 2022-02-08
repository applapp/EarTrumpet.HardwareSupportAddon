# Compiling EarTrumpet and the HardwareSupportAddon

1. Follow the instructions on how to compile the EarTrumpet application (https://github.com/File-New-Project/EarTrumpet/blob/master/COMPILING.md). Make sure that you can compile and run EarTrumpet.

2. Clone this repository to the same parent directory as the EarTrumpet repository. The directory structure with the .sln files should look like this:

    > EarTrumpet/EarTrumpet.vs15.sln  
    > EarTrumpet.HardwareSupportAddon/EarTrumpet-WithAddons.sln

3. Open the solution `EarTrumpet-WithAddons.sln` in Visual Studio.

4. Build the project folder with both projects (EarTrumpet and EarTrumpet.HardwareControls).
   
   **Note:** Currently, addons are only loaded in **Debug** builds.

5. Run the application.
