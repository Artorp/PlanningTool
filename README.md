# Planning Tool

A planning tool mod that lets you drag shapes, copy paste plans, and build your base with ease.

## Development setup

The project has been tested to successfully be imported into Visual Studio 2019 and Rider without problems.

Clone the repository into a folder of your choice. Open the solution file (*.sln) with either Visual Studio or Rider. If your installation of Oxygen Not Included is installed in another place than the default Steam folder on C:, open `Directory.Build.props` and modify the OxygenNotIncludedDllsPath attribute to point to the correct `Managed` folder in Oxygen Not Included directory (it should contain Assembly-CSharp.dll among others).

Nuget packages are defined in packages.config and should be automatically installed first time you open the solution.

Two configurations are defined, `Debug` and `Release`. Release is configured to merge PLib.dll into the final file using ILRepack.

Build the solution, Build > Build Solution. The build output directory is set to Dist at the git repository root. Copy or create a junction link to Klei's local mods location: `%userprofile%\Documents\Klei\OxygenNotIncluded\mods\Dev`

### Configuration files

This section will describe the configuration files included in this repository.

`Directory.Build.props` defines Oxygen Not Included managed directory as well as ReferencePath that points to it. ReferencePath takes precedence over referenced assemblies HintPath, to properly resolve dll locations.

Since PLib.dll is required and the recommended way to include the dll is to bundle it, ILRepack is used to merge the output assembly with PLib to end up with a single dll.

`ILRepack.targets` defines the ILRepack build step that merges dlls, as well as deleting left over files. ILRepack will only run as part of the Release configuration.

`ILRepack.Config.props` defines a few properties to match the recommended build settings per PLib docs.
