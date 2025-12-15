using Internal.Scripts.Bootstrap;
using Internal.Scripts.Controllers.Buildings;
using Internal.Scripts.Controllers.Player;
using Internal.Scripts.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Internal.Scripts.UI
{
  public class BuildMenuController : MonoBehaviour
  {
    [SerializeField] private GameObject _buildMenu;
    [SerializeField] private Transform _playerTransform; // Reference to the player transform
    [SerializeField] private LayerMask _tileLayerMask;   // Layer mask for tile objects
    [SerializeField] private Button BuildMenuButton;    // Reference to the build menu button
    
    private Building _selectedBuildingPrefab;
    private bool _isBuildingSelected = false;
    
    [SerializeField]
    private ResourceInventory _resourceInventory; // Ссылка на инвентарь ресурсов

    private void Start()
    {
      // Debug.Log("[BuildMenuController] Starting initialization...");
      
      // If player transform isn't assigned via inspector, try to get it from this object
      if (_playerTransform == null)
      {
        _playerTransform = transform;
        // Debug.Log("[BuildMenuController] Player transform assigned from self");
      }
      else
      {
        // Debug.Log($"[BuildMenuController] Player transform assigned: {_playerTransform.name}");
      }
      
      // Disable build menu initially
      _buildMenu.SetActive(false);
      // Debug.Log("[BuildMenuController] Build menu disabled initially");
    }

    private void Update()
    {
      UpdateBuildMenuAvailability();
    }

    private void UpdateBuildMenuAvailability()
    {
      // Get the tile under the player
      Vector3 playerPosition = _playerTransform.position;
      
      // Adjust raycast origin to look down from above the player
      Vector3 rayOrigin = new Vector3(playerPosition.x, playerPosition.y + 10f, playerPosition.z);
      
      RaycastHit hit;
      if (Physics.Raycast(rayOrigin, Vector3.down, out hit, Mathf.Infinity, _tileLayerMask))
      {
        // Check if the tile is a valid build location
        if (hit.collider.TryGetComponent(out TileController tileController))
        {
          var tile = tileController.Tile;
          
          bool isValidBuildLocation = tile.Type == Tile.TileType.Ground || 
                                    tile.Type == Tile.TileType.ResourceTree ||
                                    tile.Type == Tile.TileType.ResourceStone ||
                                    tile.Type == Tile.TileType.ResourceDiamond;
          
          if (BuildMenuButton != null)
          {
            BuildMenuButton.interactable = isValidBuildLocation;
          }
        }
        else
        {
          if (BuildMenuButton != null)
          {
            BuildMenuButton.interactable = false;
          }
        }
      }
      else
      {
        if (BuildMenuButton != null)
        {
          BuildMenuButton.interactable = false;
        }
      }
    }

    public void OnBuildMenuButtonPressed()
    {
      // Debug.Log("[BuildMenuController] OnBuildMenuButtonPressed called");
      
      // Only open the menu if the player is on a valid build location
      Vector3 playerPosition = _playerTransform.position;
      Vector3 rayOrigin = new Vector3(playerPosition.x, playerPosition.y + 10f, playerPosition.z);
      RaycastHit hit;
      
      if (Physics.Raycast(rayOrigin, Vector3.down, out hit, Mathf.Infinity, _tileLayerMask))
      {
        // Debug.Log($"[BuildMenuController] Menu button pressed - raycast hit: {hit.collider.name}");
        
        if (hit.collider.TryGetComponent(out TileController tileController))
        {
          var tile = tileController.Tile;
          // Debug.Log($"[BuildMenuController] Menu button pressed - found tile with type: {tile.Type}");
          
          bool isValidBuildLocation = tile.Type == Tile.TileType.Ground || 
                                    tile.Type == Tile.TileType.ResourceTree ||
                                    tile.Type == Tile.TileType.ResourceStone ||
                                    tile.Type == Tile.TileType.ResourceDiamond;
          
          // Debug.Log($"[BuildMenuController] Menu button pressed - valid build location: {isValidBuildLocation}");
          
          if (isValidBuildLocation)
          {
            bool newState = !_buildMenu.activeSelf;
            _buildMenu.SetActive(newState);
            // Debug.Log($"[BuildMenuController] Build menu set to: {newState}");
            
            _isBuildingSelected = false; // Reset selection when opening/closing menu
          }
          else
          {
            // Debug.Log("[BuildMenuController] Cannot open menu - invalid build location");
          }
        }
        else
        {
          // Debug.Log("[BuildMenuController] Cannot open menu - no tileController component found");
        }
      }
      else
      {
        // Debug.Log("[BuildMenuController] Cannot open menu - raycast failed");
      }
    }
    
    private bool CheckResourceAmount(string resourceId, int requiredAmount)
    {
      if (_resourceInventory == null) return false;
      return _resourceInventory.GetResourceAmount(resourceId) >= requiredAmount;
    }
    
    public void SelectBuilding(Building buildingPrefab)
    {
      foreach (var cost in buildingPrefab.CostResources)
      {
        if (!CheckResourceAmount(cost.resourceId, cost.amount))
        {
          // Debug.Log($"[BuildMenuController] Not enough resources to build {buildingPrefab.name}");
          return; // Not enough resources
        }
      }
      // Debug.Log($"[BuildMenuController] SelectBuilding called with prefab: {buildingPrefab?.name}");
      
      _selectedBuildingPrefab = buildingPrefab;
      _isBuildingSelected = true;
      
      // Close the menu after selection
      _buildMenu.SetActive(false);
      PlaceSelectedBuilding();
      // Debug.Log("[BuildMenuController] Build menu closed after selection");
    }

    // Call this method when you want to place the selected building
    public void PlaceSelectedBuilding()
    {
      // Debug.Log("[BuildMenuController] PlaceSelectedBuilding called");
      
      if (!_isBuildingSelected || _selectedBuildingPrefab == null)
      {
        // Debug.Log("[BuildMenuController] No building selected or selection is null");
        return;
      }

      // Debug.Log("[BuildMenuController] Attempting to place selected building");
      
      // Get the tile under the player
      Vector3 playerPosition = _playerTransform.position;
      Vector3 rayOrigin = new Vector3(playerPosition.x, playerPosition.y + 10f, playerPosition.z);
      RaycastHit hit;
      
      if (Physics.Raycast(rayOrigin, Vector3.down, out hit, Mathf.Infinity, _tileLayerMask))
      {
        // Debug.Log($"[BuildMenuController] Placement raycast hit: {hit.collider.name}");
        
        if (hit.collider.TryGetComponent(out TileController tileController))
        {
          var tile = tileController.Tile;
          // Debug.Log($"[BuildMenuController] Placement - found tile with type: {tile.Type}");
          
          // Check if the tile is a valid build location
          bool isValidBuildLocation = tile.Type == Tile.TileType.Ground || 
                                    tile.Type == Tile.TileType.ResourceTree ||
                                    tile.Type == Tile.TileType.ResourceStone ||
                                    tile.Type == Tile.TileType.ResourceDiamond;
          
          // Debug.Log($"[BuildMenuController] Placement - valid build location: {isValidBuildLocation}");
          
          if (isValidBuildLocation)
          {
            // Проверяем, достаточно ли ресурсов перед постройкой (дублируем проверку из SelectBuilding)
            foreach (var cost in _selectedBuildingPrefab.CostResources)
            {
              if (!CheckResourceAmount(cost.resourceId, cost.amount))
              {
                // Debug.Log($"[BuildMenuController] Not enough resources to build {_selectedBuildingPrefab.name}");
                return; // Not enough resources
              }
            }
            
            // Calculate the center position of the tile
            var settings = GameSettingsManager.GetInstance().Settings;
            Vector3 tileCenter = new Vector3(
              tile.X * settings.GroundGeneratorSettings.DefaultTileSize, 
              settings.GroundGeneratorSettings.MiscHeightOffset, 
              tile.Z * settings.GroundGeneratorSettings.DefaultTileSize
            );
            // Debug.Log($"[BuildMenuController] Placing building at tile center: {tileCenter}");
            
            // Instantiate the building at the tile center
            Building placedBuilding = Instantiate(_selectedBuildingPrefab, tileCenter, Quaternion.identity);
            // Debug.Log($"[BuildMenuController] Building instantiated: {placedBuilding?.name}");
            
            // Вычитаем ресурсы после успешного создания здания
            foreach (var cost in _selectedBuildingPrefab.CostResources)
            {
              ConsumeResources(cost.resourceId, cost.amount);
            }
            
            // Mark the tile as occupied by a building (you may need to update your Tile system to support this)
            // Example: tile.OccupiedByBuilding = placedBuilding; (if you add this property to Tile)
            
            // Reset selection
            _selectedBuildingPrefab = null;
            _isBuildingSelected = false;
            // Debug.Log("[BuildMenuController] Selection reset after placement");
          }
          else
          {
            // Debug.Log("[BuildMenuController] Cannot place building - invalid tile type for construction");
          }
        }
        else
        {
          // Debug.Log("[BuildMenuController] Cannot place building - no tileController component found");
        }
      }
      else
      {
        // Debug.Log("[BuildMenuController] Cannot place building - raycast failed");
      }
    }

    // Новый приватный метод для вычитания ресурсов из инвентаря
    private void ConsumeResources(string resourceId, int amount)
    {
      if (_resourceInventory == null) return;
      
      // Вызываем OnResourceDropped нужное количество раз для вычитания ресурсов
      for (int i = 0; i < amount; i++)
      {
        _resourceInventory.OnResourceDropped(resourceId);
      }
    }
  }
}