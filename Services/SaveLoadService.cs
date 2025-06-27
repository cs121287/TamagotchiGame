using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using TamagotchiGame.Models;

namespace TamagotchiGame.Services
{
    public class SaveLoadService
    {
        private const string SaveFileName = "tamagotchi_save.json";
        
        public async Task SavePetAsync(Pet pet)
        {
            try
            {
                var saveData = new
                {
                    Name = pet.Name,
                    Hunger = pet.Hunger,
                    Happiness = pet.Happiness,
                    Health = pet.Health,
                    Energy = pet.Energy,
                    Age = pet.Age,
                    LastUpdateTime = DateTime.Now
                };
                
                string jsonString = JsonSerializer.Serialize(saveData, new JsonSerializerOptions { WriteIndented = true });
                
                string savePath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "TamagotchiGame",
                    SaveFileName);
                
                // Ensure directory exists
                Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                
                await File.WriteAllTextAsync(savePath, jsonString);
            }
            catch (Exception ex)
            {
                // Handle errors (log or display to user)
                Console.WriteLine($"Error saving pet: {ex.Message}");
            }
        }
        
        public async Task<Pet> LoadPetAsync()
        {
            try
            {
                string savePath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "TamagotchiGame",
                    SaveFileName);
                
                if (!File.Exists(savePath))
                    return new Pet(); // Return a new pet if no save exists
                
                string jsonString = await File.ReadAllTextAsync(savePath);
                var saveData = JsonSerializer.Deserialize<dynamic>(jsonString);
                
                var pet = new Pet(saveData.GetProperty("Name").GetString())
                {
                    Hunger = saveData.GetProperty("Hunger").GetInt32(),
                    Happiness = saveData.GetProperty("Happiness").GetInt32(),
                    Health = saveData.GetProperty("Health").GetInt32(),
                    Energy = saveData.GetProperty("Energy").GetInt32(),
                    Age = saveData.GetProperty("Age").GetInt32(),
                    LastUpdateTime = saveData.GetProperty("LastUpdateTime").GetDateTime()
                };
                
                return pet;
            }
            catch (Exception ex)
            {
                // Handle errors (log or display to user)
                Console.WriteLine($"Error loading pet: {ex.Message}");
                return new Pet(); // Return a new pet on error
            }
        }
    }
}