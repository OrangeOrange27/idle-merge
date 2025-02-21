# Extension points for ControllersTree

## What is it?
Ability to extend behaviour of exists controllers/states without modification of them. And don't interrupt lifecycle of parent controller even if side logic was failed

## Glossary
*Extension Points* has been taken from [this lecture](https://youtu.be/Y0D62N_Xo8A?si=-O23QjWtHfjNgFlM) aka "Plugin points"
Basically in current implementation you can use any controller as Extension Point. Cause you can extend work of your Controller by infinitive amount of SubControllers

*SubController* is the same controller. Only difference that it would automatically start in parallel with Extension Point Controller

## Why?
First of all, I suggest to take a look at [the lecture from Uber](https://youtu.be/Y0D62N_Xo8A?si=-O23QjWtHfjNgFlM) who implemented similar architecture.

Main idea is isolating side features and makes the game growing without modifying code that doesn't require to change. And change direction of dependencies

Example case:
`LobbyIdleState.cs` has many subscription on many buttons for every feature. And every new feature or button we have to modify this class to add one more transition or extra logic.
With SubControllers you can such subscriptions/spawning object/extra logic out of `LobbyIdleState.cs` that doesn't really need to know about your feature and shouldn't be broken if avatar frame wouldn't work

#### Regular way
Map knows about every feature. That means that Map depends on all these features and we cannot use Map without all features.
![CleanShot 2023-04-30 at 20.50.30@2x.png](/CleanShot 2023-04-30 at 20.50.30@2x.png)

#### Extension Points approach
With extension approach Map doesn't depends on other feature. There are other features depends on the MapExtentionPoint and we have isolated features
![CleanShot 2023-04-30 at 20.52.35@2x.png](/CleanShot 2023-04-30 at 20.52.35@2x.png)

#### Example in code
```csharp
//Feature.Map.Implementation.asmdef
class MapState
{
	IMapExtentsionPoint _mapExtentsionPoint;
	
	void IntializeViews()
	{
		SpawnUI();
		_mapExtentsionPoint.SpawnView();
	}
}

//Feature.Map.ExtensionPoints.asmdef
interface IMapExtentsionPoint
{
	void SpawnView();
}

class MapExtentsionPoint: IMapExtentsionPoint
{
	IEnumerable<IMapExtentsionPoint> _allRegisteredExtensions;

	void SpawnView()
	{
		foreach(var extesionPoint in _allRegisteredExtensions)
		{
			extesionPoint.SpawnView();
		}
	}
}


//Feature.A.Implementation.asmdef
class FeatureASpawnViewAtMap : IMapExtentsionPoint
{
	void SpawnView()
	{
		_viewLoader.Spawn(key);
	}
}
void InstallDependencies(DiContainer container)
{
	container.Bind<IMapExtentsionPoint, FeatureASpawnViewAtMap>().AsSingle();
}


//Feature.B.Implementation.asmdef
class FeatureBSpawnViewAtMap : IMapExtentsionPoint
{
	void SpawnView()
	{
		_viewLoader.Spawn(key);
	}
}
void InstallDependencies(DiContainer container)
{
	container.Bind<IMapExtentsionPoint, FeatureBSpawnViewAtMap>().AsSingle();
}

```

#### Benefits
- Extension points makes your code more modular and independent from each one
- You have a determined contract (public API) for using your feature from out of implementation.
- You can easily enable/disable features inside a integrated feature and configurate other behaviour inside the feature. and not publish it outside
- If integrated feature have any errors during execution, then we can just automatically disable it and don't influence on other features
- Decrease bottle necks, of modifying the same files by isolating all your changes at modules
