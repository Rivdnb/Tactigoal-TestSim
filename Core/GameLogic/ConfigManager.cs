using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using Core.GameObjects;

namespace Core.GameLogic
{
    public class ConfigManager
    {
        public ConfigManager() { }

        public UnitType LoadUnitType(string path)
        {
            string json = File.ReadAllText(path);

            JsonSerializerOptions options = new(){ Converters = { new JsonStringEnumConverter() } };
            UnitType? unitType = JsonSerializer.Deserialize<UnitType>(json, options);

            if (unitType == null)
                throw new Exception("Failed to load unit type configuration.");
            
            return unitType;
        }

        public void StoreUnitType(UnitType unitType, string configDirectoryPath)
        {
            string unitTypePath = Path.Combine(configDirectoryPath, $"{unitType.Name}.json");
            string json = JsonSerializer.Serialize<UnitType>(unitType);

            try { File.WriteAllText(unitTypePath, json); }
            catch (Exception e) { throw new Exception(e.Message); }

        }

        public GameConfig LoadGameConfig(string configDirectoryPath, string dataDirectoryPath)
        {
            string unitConfigPath = "";
            string gameConfigPath = Path.Combine(configDirectoryPath, "GameConfig.json");

            GameConfig gameConfig = JsonSerializer.Deserialize<GameConfig>(File.ReadAllText(gameConfigPath))
                ?? throw new NullReferenceException($"Could not generate game config from file at path: {gameConfigPath}");

            List<UnitType> unitTypes = [];
            if (gameConfig.UnitTypePaths == null) throw new Exception("No paths found for unit types in game config file");

            foreach (string path in gameConfig.UnitTypePaths)
            {
                unitConfigPath = Path.Combine(configDirectoryPath, path);
                unitTypes.Add(LoadUnitType(unitConfigPath));
            }

            return new GameConfig(gameConfig.P1, gameConfig.P2, gameConfig.BoardRows, gameConfig.BoardColumns,
                unitTypes, gameConfig.UnitTypePaths, Path.Combine(dataDirectoryPath, gameConfig.LineUpsPath));


        }

        public void StoreGameConfig(GameConfig gameConfig, string configDirectoryPath)
        {
            foreach (UnitType unitType in gameConfig.UnitTypes)
            {
                StoreUnitType(unitType, configDirectoryPath);
            }
            
            gameConfig.UpdateUnitTypes(new List<UnitType>());

            string gameConfigPath = Path.Combine(configDirectoryPath, "GameConfig.json");
            string configJson = JsonSerializer.Serialize<GameConfig>(gameConfig);

            try { File.WriteAllText(gameConfigPath, configJson); }
            catch (Exception e) { throw new Exception(e.Message); }
            
        }
    }
}
