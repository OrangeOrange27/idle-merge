using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Features.Core.GameAreaInitializationSystem.Models;
using Features.Core.Placeables.Editor;
using Features.Core.Placeables.Models;

namespace Common.Utils.Editor
{
    //  IT IS A TEMPORARY TOOL FOR SAVING ONLY PRODUCTION BUILDINGS
    //  PLS USE GameAreaSaverEditorWindow WHEN IT'S READY <3
    public class ProductionBuildingsSaverEditor : EditorWindow
    {
        [SerializeField] private GameAreaConfigSO gameAreaConfigSO;
        
        [MenuItem("Tools/Game Area/Save Production Buildings")]
        public static void ShowWindow()
        {
            GetWindow<ProductionBuildingsSaverEditor>("Production Buildings Saver");
        }

        private void OnGUI()
        {
            GUILayout.Label("Production Buildings Saver", EditorStyles.boldLabel);

            gameAreaConfigSO = (GameAreaConfigSO)EditorGUILayout.ObjectField(
                "Game Area Config SO",
                gameAreaConfigSO,
                typeof(GameAreaConfigSO),
                false
            );

            if (gameAreaConfigSO == null)
            {
                EditorGUILayout.HelpBox("Please assign a GameAreaConfigSO asset to save data to.", MessageType.Warning);
                return;
            }

            if (GUILayout.Button("Save Placeables from Selected Object"))
            {
                GameObject rootObject = Selection.activeGameObject;
                if (rootObject == null)
                {
                    EditorGUILayout.HelpBox("Please select a GameObject in the Hierarchy that contains the PlaceableViews you want to save.", MessageType.Error);
                    return;
                }

                SavePlaceablesInternal(rootObject);
            }
        }

        private void SavePlaceablesInternal(GameObject rootObject)
        {
            var placeableConfigs = new List<GameAreaPlaceableConfigEntry>();
            
            foreach (var productionBuilding in rootObject.GetComponentsInChildren<ProductionBuildingEditorModel>())
            {
                var placeableRawPosition = productionBuilding.transform.position;
                var placeablePosition = new Vector3Int(
                    Mathf.FloorToInt(placeableRawPosition.x),
                    Mathf.FloorToInt(placeableRawPosition.y),
                    0);

                var model = CreateModel(productionBuilding);
                if (model == null)
                {
                    Debug.LogError($"Could not create model for placeable '{productionBuilding.name}' on position {placeablePosition}. Skipping.");
                    continue;
                }

                placeableConfigs.Add(new GameAreaPlaceableConfigEntry()
                    { Position = placeablePosition, Placeable = model });
            }

            if (gameAreaConfigSO.GameAreaConfig == null)
            {
                gameAreaConfigSO.GameAreaConfig = new GameAreaConfig();
            }

            gameAreaConfigSO.GameAreaConfig.Placeables.Clear();
            foreach (var configEntry in placeableConfigs)
            {
                gameAreaConfigSO.GameAreaConfig.Placeables[configEntry.Position] = configEntry.Placeable;
            }

            EditorUtility.SetDirty(gameAreaConfigSO);
            AssetDatabase.SaveAssets();

            Debug.Log($"Successfully saved {placeableConfigs.Count} placeables to '{gameAreaConfigSO.name}'.");
        }

        private ProductionBuildingModel CreateModel(ProductionBuildingEditorModel productionBuilding)
        {
            return productionBuilding.GetModel();
        }
    }
}