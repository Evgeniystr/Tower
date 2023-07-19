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
    private SplashSettings _splashSettings;

    [SerializeField]
    private Transform _splashParent;
    [SerializeField]
    private Transform _towerParent;
    [SerializeField]
    private Transform _camTransform;
    [SerializeField]
    private TowerItem _towerBaseItem;
    [SerializeField]
    private TowerItem _fruitPrefab;

    [SerializeField]
    private AudioService _audioService;
    [SerializeField]
    private PlayerInputService _playerInputService;
    [SerializeField]
    private ADSService _adService;
    [SerializeField]
    private GameService _gameService;


    public override void InstallBindings()
    {
        Container.Bind<FruitItemSettings>().FromInstance(_fruitItemSettings).AsSingle();
        Container.Bind<CameraSettings>().FromInstance(_cameraSettings).AsSingle();
        Container.Bind<GameSettings>().FromInstance(_gameSettings).AsSingle();
        Container.Bind<SoundSettings>().FromInstance(_soundSettings).AsSingle();
        Container.Bind<SplashSettings>().FromInstance(_splashSettings).AsSingle();

        Container.Bind<Transform>().WithId(Constants.Camera).FromInstance(_camTransform).AsCached();
        Container.Bind<Transform>().WithId(Constants.SplashParent).FromInstance(_splashParent).AsCached();
        Container.Bind<Transform>().WithId(Constants.TowerParent).FromInstance(_towerParent).AsCached();
        Container.Bind<TowerItem>().WithId(Constants.TowerBaseItem).FromInstance(_towerBaseItem).AsCached();
        Container.Bind<TowerItem>().FromInstance(_fruitPrefab).AsCached();

        Container.Bind<AudioService>().FromInstance(_audioService).AsSingle();
        Container.Bind<PlayerInputService>().FromInstance(_playerInputService).AsSingle();
        Container.Bind<ADSService>().FromInstance(_adService).AsSingle();
        Container.Bind<GameService>().FromInstance(_gameService).AsSingle();
        Container.Bind<SplashService>().AsSingle();
        Container.Bind<TowerBuilderService>().AsSingle();
        Container.Bind<CameraService>().AsSingle().NonLazy();
        Container.Bind<ScoreService>().AsSingle();
    }
}
