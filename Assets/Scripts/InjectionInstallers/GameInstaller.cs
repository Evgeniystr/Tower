using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField]
    private FruitItemSettings _fruitItemSettings;
    [SerializeField]
    private CameraSettings _cameraSettings;
    [SerializeField]
    private GameSettings _gameSettings;
    [SerializeField]
    private SoundSettings _soundSettings;
    [SerializeField]
    private SplashColorPalette _splashColorPalette;

    [SerializeField]
    private Transform _splashParent;
    [SerializeField]
    private Transform _camTransform;
    [SerializeField]
    private Transform _towerBaseItem;
    [SerializeField]
    private GameObject _fruitPrefab;
    [SerializeField]
    private SplashView _splashPrefab;

    [SerializeField]
    private AudioService _audioService;
    [SerializeField]
    private PlayerInputService _playerInputService;
    [SerializeField]
    private GameService _gameService;


    public override void InstallBindings()
    {
        Container.Bind<FruitItemSettings>().FromInstance(_fruitItemSettings).AsSingle();
        Container.Bind<CameraSettings>().FromInstance(_cameraSettings).AsSingle();
        Container.Bind<GameSettings>().FromInstance(_gameSettings).AsSingle();
        Container.Bind<SoundSettings>().FromInstance(_soundSettings).AsSingle();
        Container.Bind<SplashColorPalette>().FromInstance(_splashColorPalette).AsSingle();

        Container.Bind<Transform>().WithId(Constants.SplashParent).FromInstance(_splashParent).AsCached();
        Container.Bind<Transform>().WithId(Constants.Camera).FromInstance(_camTransform).AsCached();
        Container.Bind<GameObject>().WithId(Constants.FruitPrefab).FromInstance(_fruitPrefab).AsSingle();
        Container.Bind<Transform>().WithId(Constants.TowerBaseItem).FromInstance(_towerBaseItem).AsSingle();
        Container.Bind<SplashView>().FromInstance(_splashPrefab).AsSingle();

        Container.Bind<AudioService>().FromInstance(_audioService).AsSingle();
        Container.Bind<PlayerInputService>().FromInstance(_playerInputService).AsSingle();
        Container.Bind<GameService>().FromInstance(_gameService).AsSingle();
        Container.Bind<SplashService>().AsSingle();
        Container.Bind<TowerBuilderService>().AsSingle();
        Container.Bind<CameraService>().AsSingle();
        Container.Bind<ScoreService>().AsSingle();
    }
}