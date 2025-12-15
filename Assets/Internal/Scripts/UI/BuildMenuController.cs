using System.Linq;
using Internal.Scripts.Bootstrap;
using Internal.Scripts.Controllers.Buildings;
using Internal.Scripts.Controllers.Player;
using Internal.Scripts.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Internal.Scripts.UI
{
  /// <summary>
  /// Контроллер меню строительства, управляющий открытием/закрытием меню и размещением зданий
  /// </summary>
  public class BuildMenuController : MonoBehaviour
  {
    #region Поля

    [SerializeField] 
    private GameObject _buildMenu;
    
    [SerializeField] 
    private Transform _playerTransform; // Ссылка на трансформ игрока
    
    [SerializeField] 
    private LayerMask _tileLayerMask;   // Маска слоя для объектов тайлов
    
    [SerializeField] 
    private Button BuildMenuButton;    // Ссылка на кнопку меню строительства
    
    private Building _selectedBuildingPrefab;
    private bool _isBuildingSelected = false;
    
    [SerializeField]
    private ResourceInventory _resourceInventory; // Ссылка на инвентарь ресурсов

    #endregion

    #region Методы Unity

    /// <summary>
    /// Вызывается при старте компонента, инициализирует контроллер
    /// </summary>
    private void Start()
    {
      // Если трансформ игрока не назначен через инспектор, пытаемся получить его из этого объекта
      if (!_playerTransform)
      {
        _playerTransform = transform;
      }
      
      // Отключаем меню строительства изначально
      _buildMenu.SetActive(false);
    }

    /// <summary>
    /// Вызывается каждый кадр, обновляет доступность меню строительства
    /// </summary>
    private void Update()
    {
      // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
      UpdateBuildMenuAvailability();
    }

    #endregion

    #region Методы обновления состояния

    /// <summary>
    /// Обновляет доступность меню строительства в зависимости от положения игрока
    /// </summary>
    private void UpdateBuildMenuAvailability()
    {
      // Получаем тайл под игроком
      var playerPosition = _playerTransform.position;
    
      // Регулируем начало луча для проверки с верхней части игрока
      var rayOrigin = new Vector3(playerPosition.x, playerPosition.y + 10f, playerPosition.z);

      if (Physics.Raycast(rayOrigin, Vector3.down, out var hit, Mathf.Infinity, _tileLayerMask))
      {
        // Проверяем, если тайл является допустимым местом для строительства
        if (hit.collider.TryGetComponent(out TileController tileController))
        {
          var tile = tileController.Tile;
            
          // Проверяем, что тайл допускает строительство И не занят другим зданием
          var isValidBuildLocation = tile.Type is Tile.TileType.Ground or Tile.TileType.ResourceTree or Tile.TileType.ResourceStone or Tile.TileType.ResourceDiamond &&
                                     !tile.IsOccupied; // Добавляем проверку на занятость
            
          if (BuildMenuButton)
          {
            BuildMenuButton.interactable = isValidBuildLocation;
          }
        }
        else
        {
          if (BuildMenuButton)
          {
            BuildMenuButton.interactable = false;
          }
        }
      }
      else
      {
        if (BuildMenuButton)
        {
          BuildMenuButton.interactable = false;
        }
      }
    }

    #endregion

    #region Методы взаимодействия с меню

    /// <summary>
    /// Вызывается при нажатии кнопки меню строительства
    /// </summary>
    public void OnBuildMenuButtonPressed()
    {
      // Только открываем меню, если игрок находится на допустимом месте для строительства
      var playerPosition = _playerTransform.position;
      var rayOrigin = new Vector3(playerPosition.x, playerPosition.y + 10f, playerPosition.z);

      if (!Physics.Raycast(rayOrigin, Vector3.down, out var hit, Mathf.Infinity, _tileLayerMask))
        return;
      if (!hit.collider.TryGetComponent(out TileController tileController))
        return;
      var tile = tileController.Tile;
          
      var isValidBuildLocation = tile.Type == Tile.TileType.Ground || 
                                 tile.Type == Tile.TileType.ResourceTree ||
                                 tile.Type == Tile.TileType.ResourceStone ||
                                 tile.Type == Tile.TileType.ResourceDiamond;

      if (!isValidBuildLocation)
        return;
      var newState = !_buildMenu.activeSelf;
      _buildMenu.SetActive(newState);
            
      _isBuildingSelected = false; // Сбрасываем выбор при открытии/закрытии меню
    }

    /// <summary>
    /// Выбирает здание для строительства
    /// </summary>
    /// <param name="buildingPrefab">Префаб здания для строительства</param>
    public void SelectBuilding(Building buildingPrefab)
    {
      if (buildingPrefab.CostResources.Any(cost => !CheckResourceAmount(cost.resourceId, cost.amount)))
      {
        return; // Недостаточно ресурсов
      }
      
      _selectedBuildingPrefab = buildingPrefab;
      _isBuildingSelected = true;
      
      // Закрываем меню после выбора
      _buildMenu.SetActive(false);
      PlaceSelectedBuilding();
    }

    /// <summary>
    /// Размещает выбранное здание на карте
    /// </summary>
    private void PlaceSelectedBuilding()
    {
      if (!_isBuildingSelected || !_selectedBuildingPrefab)
      {
        return;
      }

      // Получаем тайл под игроком
      var playerPosition = _playerTransform.position;
      var rayOrigin = new Vector3(playerPosition.x, playerPosition.y + 10f, playerPosition.z);

      if (!Physics.Raycast(rayOrigin, Vector3.down, out var hit, Mathf.Infinity, _tileLayerMask))
        return;
      if (!hit.collider.TryGetComponent(out TileController tileController))
        return;
      var tile = tileController.Tile;
          
      // Проверяем, является ли тайл допустимым местом для строительства
      var isValidBuildLocation = tile.Type is Tile.TileType.Ground or Tile.TileType.ResourceTree or Tile.TileType.ResourceStone or Tile.TileType.ResourceDiamond;

      if (!isValidBuildLocation)
        return;
      // Проверяем, достаточно ли ресурсов перед постройкой (дублируем проверку из SelectBuilding)
      if (_selectedBuildingPrefab.CostResources.Any(cost => !CheckResourceAmount(cost.resourceId, cost.amount)))
      {
        return; // Недостаточно ресурсов
      }
            
      // Рассчитываем центральную позицию тайла
      var settings = GameSettingsManager.GetInstance().Settings;
      var tileCenter = new Vector3(
        tile.X * settings.GroundGeneratorSettings.DefaultTileSize, 
        settings.GroundGeneratorSettings.MiscHeightOffset, 
        tile.Z * settings.GroundGeneratorSettings.DefaultTileSize
      );
    
      // Создаем здание в центре тайла
      var placedBuilding = Instantiate(_selectedBuildingPrefab, tileCenter, Quaternion.identity);
    
      // Вычитаем ресурсы после успешного создания здания
      foreach (var cost in _selectedBuildingPrefab.CostResources)
      {
        ConsumeResources(cost.resourceId, cost.amount);
      }
    
      // Устанавливаем флаг занятости тайла
      tile.IsOccupied = true;
    
      // Сбрасываем выбор
      _selectedBuildingPrefab = null;
      _isBuildingSelected = false;
    }

    #endregion

    #region Вспомогательные методы

    /// <summary>
    /// Проверяет, достаточно ли ресурсов в инвентаре
    /// </summary>
    /// <param name="resourceId">Идентификатор ресурса</param>
    /// <param name="requiredAmount">Требуемое количество ресурса</param>
    /// <returns>true если ресурсов достаточно, иначе false</returns>
    private bool CheckResourceAmount(string resourceId, int requiredAmount)
    {
      if (!_resourceInventory) return false;
      return _resourceInventory.GetResourceAmount(resourceId) >= requiredAmount;
    }

    /// <summary>
    /// Вычитает ресурсы из инвентаря
    /// </summary>
    /// <param name="resourceId">Идентификатор ресурса</param>
    /// <param name="amount">Количество ресурса для вычитания</param>
    private void ConsumeResources(string resourceId, int amount)
    {
      if (!_resourceInventory) return;
      
      // Вызываем OnResourceDropped нужное количество раз для вычитания ресурсов
      for (var i = 0; i < amount; i++)
      {
        _resourceInventory.OnResourceDropped(resourceId);
      }
    }

    #endregion
  }
}