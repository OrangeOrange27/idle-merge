// using System;
// using System.Collections.Generic;
// using Features.Core.Common.Models;
// using UnityEditor;
// using UnityEngine;
// using Features.Core.GameAreaInitializationSystem.Models;
// using Features.Core.Placeables.Models;
// using Features.Core.Placeables.Views;
//
// namespace Common.Utils.Editor
// {
//     public class GameAreaSaverEditorWindow : EditorWindow
//     {
//         [SerializeField] private GameAreaConfigSO gameAreaConfigSO;
//         
//         [MenuItem("Tools/Game Area/Save Current Area Config")]
//         public static void ShowWindow()
//         {
//             GetWindow<GameAreaSaverEditorWindow>("Game Area Saver");
//         }
//
//         private void OnGUI()
//         {
//             GUILayout.Label("Game Area Config Saver", EditorStyles.boldLabel);
//
//             gameAreaConfigSO = (GameAreaConfigSO)EditorGUILayout.ObjectField(
//                 "Game Area Config SO",
//                 gameAreaConfigSO,
//                 typeof(GameAreaConfigSO),
//                 false
//             );
//
//             if (gameAreaConfigSO == null)
//             {
//                 EditorGUILayout.HelpBox("Please assign a GameAreaConfigSO asset to save data to.", MessageType.Warning);
//                 return;
//             }
//
//             if (GUILayout.Button("Save Placeables from Selected Object"))
//             {
//                 GameObject rootObject = Selection.activeGameObject;
//                 if (rootObject == null)
//                 {
//                     EditorGUILayout.HelpBox("Please select a GameObject in the Hierarchy that contains the PlaceableViews you want to save.", MessageType.Error);
//                     return;
//                 }
//
//                 SavePlaceablesInternal(rootObject);
//             }
//         }
//
//         private void SavePlaceablesInternal(GameObject rootObject)
//         {
//             var placeableConfigs = new List<GameAreaPlaceableConfigEntry>();
//             
//             foreach (var placeableView in rootObject.GetComponentsInChildren<PlaceableView>())
//             {
//                 var placeableRawPosition = placeableView.transform.position;
//                 var placeablePosition = new Vector3Int(
//                     Mathf.FloorToInt(placeableRawPosition.x),
//                     Mathf.FloorToInt(placeableRawPosition.y),
//                     -1);
//
//                 var model = CreateModel(placeableView);
//                 if (model == null)
//                 {
//                     Debug.LogError($"Could not create model for placeable '{placeableView.name}' on position {placeablePosition}. Skipping.");
//                     continue;
//                 }
//
//                 placeableConfigs.Add(new GameAreaPlaceableConfigEntry()
//                     { Position = placeablePosition, Placeable = model });
//             }
//
//             if (gameAreaConfigSO.GameAreaConfig == null)
//             {
//                 gameAreaConfigSO.GameAreaConfig = new GameAreaConfig();
//             }
//
//             gameAreaConfigSO.GameAreaConfig.Placeables.Clear();
//             foreach (var configEntry in placeableConfigs)
//             {
//                 gameAreaConfigSO.GameAreaConfig.Placeables[configEntry.Position] = configEntry.Placeable;
//             }
//
//             EditorUtility.SetDirty(gameAreaConfigSO);
//             AssetDatabase.SaveAssets();
//
//             Debug.Log($"Successfully saved {placeableConfigs.Count} placeables to '{gameAreaConfigSO.name}'.");
//         }
//
//         private PlaceableModel CreateModel(PlaceableView view)
//         {
//             var instructionGetter = view.GetComponent<PlaceableCreationInstructionEditor>();
//             if (instructionGetter == null)
//             {
//                 Debug.LogError($"{view.name} does not have a {nameof(PlaceableCreationInstructionEditor)} component. Cannot create model.");
//                 return null;
//             }
//             
//             var instruction = instructionGetter.GetInstruction();
//             
//             Enum objectType = instruction.PlaceableType switch
//             {
//                 PlaceableType.ProductionBuilding => instruction.ProductionType,
//                 PlaceableType.MergeableObject => instruction.MergeableType,
//                 PlaceableType.CollectibleObject => instruction.CollectibleType,
//                 PlaceableType.ProductionEntity => instruction.ProductionType,
//                 _ => throw new ArgumentOutOfRangeException($"Unhandled PlaceableType: {instruction.PlaceableType} for view {view.name}")
//             };
//             
//             var model = _placeablesFactory.Create(instruction.PlaceableType, objectType);
//             
//             return model;
//         }
//     }
// }