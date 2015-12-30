using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Assets.scripts;
using Toolbox;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

public class SceneController : BaseNetworkBehaviour
{
    public struct Scenes
    {
        public const string Main = "main";
        public const string Offline = "offline";
        public const string Join = "join";
        public const string Host = "host";
    }
    public int Level { get; set; }
    //public Asteroid[] AsteroidPrefabs;
    public Enemy EnemyPrefab; 
    public Bird BirdPrefab;
    public GameOver GameOverPrefab;
    public Instructions InstructionsPrefab;
#if OLD_WAY
    public ParticleSystem AsteroidExplosionParticlePrefab;
#endif
    //public AudioClip Jaws1;
    //public AudioClip Jaws2;
    public AudioClip FreeLifeSound;
    public AudioClip BumpSound;
    public AudioClip LanceSound;
    //public AudioClip AlienSoundBig;
    //public AudioClip AlienSoundSmall;
    public int FREE_USER_AT = 10000;

    private int _enemyPrespawnCount = 0;

    private int _nextFreeLifeScore;

    private GameOver _gameOver;
    private Instructions _instructions;
    private List<Player> _players;
    private List<Enemy> _enemies;
 //   private List<Asteroid> _asteroids = new List<Asteroid>();
//    private Alien _alien;

    //private float _nextJawsSoundTime;
    //private float _jawsIntervalSeconds;
    //private bool _jawsAlternate;
    private double _disableStartButtonUntilTime;
    private const int MAX_ENEMIES = 5;
    //private GameObject _asteroidContainer;
    //private float _lastAsteroidKilled;

    private string GetRandomSpawmPoint()
    {
        int pt = Random.Range(1, 4);
        return "SpawnPoint (" + pt + ")";
    }

    public Vector3 GetRandomSpawnPoint()
    {
        string spawnPoint = GetRandomSpawmPoint();
        var pos = SafeGameManager.SceneRoot.Find(spawnPoint).transform.position;
        return pos;

    }

    private void EnemyPrespawnLater()
    {
        _enemyPrespawnCount++;

        StartCoroutine(CoroutineUtils.DelaySeconds(() =>
        {
            AddSomeEnemies(1);
        }, 2.0f));
    }

    public void RespawnEnemy(Enemy enemy)
    {
        if (!GameManager.Instance.isServer)
        {
            return;
        }

        DestroyEnemy(enemy);

        EnemyPrespawnLater();
    }

    public void AddSomeEnemies( int numEnemies)
    {
        if (!GameManager.Instance.isServer)
        {
            return;
        }

        for (int i = 0; i < numEnemies; i++)
        {
            _enemyPrespawnCount--;

            var pos = GetRandomSpawnPoint();

            const float MAX_DISTANCE_FROM_SPAWN = 0.5f;
            if (_enemies.Any(_ => Vector3.Distance(_.transform.position, pos) < MAX_DISTANCE_FROM_SPAWN))
            {
                continue;
            }
            if ( (_players != null) && (_players.Any(_ => Vector3.Distance(_.transform.position, pos) < MAX_DISTANCE_FROM_SPAWN)))
            {
                continue;
            }
            var enemy = EnemyPrefab.InstantiateInTransform(null);
            _enemies.Add(enemy);
            enemy.transform.position = pos;

            NetworkServer.Spawn(enemy.gameObject);

            // Do this in OnStartServer, because sound needs to be played in the client.
        }
    }

    public void PlayerKilled(Bird bird)
    {
        if (SafeGameManager.PlayController.Lives < 1)
        {
            SafeGameManager.SetGameState( PlayController.States.Over);
            GameOver(bird);
        }
        else
        {
            //Respawn(bird, 2.0f);

        }

    }

    public void DestroyEnemy(Enemy e, bool explode = false)
    {
        _enemies.Remove(e);
        Destroy(e.gameObject);
    }


#if OLD_WAY
    public Vector3? GetAlienTargetOrNull()
    {
        if (Random.Range(0, 10) > 4)
        {
            if ( _birdPlayer == null)
            {
                return null;
            }
            return _birdPlayer.transform.position;
        }
        else
        {
            if (_asteroids.Count > 0)
            {
                return _asteroids[Random.Range(0, _asteroids.Count)].transform.position;
            }
        }
        return null;
    }
#endif

    void Awake()
    {
        _players = new List<Player>();
        _enemies = new List<Enemy>();
    }

    // Use this for initialization
    void Start()
    {
        try
        {
            //_asteroidContainer = GameManager.Instance.SceneRoot.FindOrCreateTempContainer("AsteroidContainer");
            _gameOver = GameOverPrefab.InstantiateInTransform( SafeGameManager.SceneRoot);
            _instructions = InstructionsPrefab.InstantiateInTransform(SafeGameManager.SceneRoot);

            ShowGameOver(false);
            ShowInstructions(false);
            _disableStartButtonUntilTime = Time.time;

            // Try to prevent game starting right after previous if you keep firing.
            SafeGameManager.StartGame();
        }
        catch (Exception ex)
        {
            Debug.Log("SceneController.Start: " + ex.Message);
        }

    }

    // Update is called once per frame
    void Update()
	{
        try
        {

        // TBD: These could be components. Especially JAWS sound and free lives.

        UpdateFreeLives();

        if ((_enemyPrespawnCount < 3) && (_players.Count > 0) && (_enemies.Count < MAX_ENEMIES) )
        {
            EnemyPrespawnLater();
        }

#if OLD_WAY
        UpdateJawsSound();

        UpdateAlienSpawn();
#endif

        if (SafeGameManager.GetGameState() == PlayController.States.Over)
        {
            if (Input.GetButton(PlayController.Buttons.FLAP))
            {

            }
        }

        }
        catch (Exception ex)
        {
            Debug.Log("SceneController.Udpate: " + ex.Message);
        }

    }

    
#if OLD_WAY
    private void UpdateAlienSpawn()
    {
        if (IsGamePlaying())
        {
            if (_alien == null)
            {
                float diff = Time.time - _lastAsteroidKilled;

                if ((diff > 8.0f) || GetRemainingAsteroidCount() < 5 && Random.Range(0, 1000) > (996 - Level*2))
                {
                    _alien = Instantiate(AlienPrefab);
                    _alien.Size = (Random.Range(0, 3) == 0) ? Alien.Sizes.Small : Alien.Sizes.Big;
                    _alien.SetPath(MakeRandomPath());
                    _alien.transform.localScale = _alien.transform.localScale*
                                                  (_alien.Size == Alien.Sizes.Small ? 0.3f : 0.6f);

                    if (IsGamePlaying())
                    {
                        _alien.PlaySound(true);
                    }
                }
            }
        }
    }
#endif

#if OLD_WAY
    private void UpdateJawsSound()
    {
        if (Time.time > _nextJawsSoundTime)
        {
            if (_birdPlayer != null) // Means we're in level
            {
                if (_jawsIntervalSeconds > .1800f)
                {
                    _jawsIntervalSeconds -= .005f;
                }
                _nextJawsSoundTime = Time.time + _jawsIntervalSeconds;
                if (_jawsAlternate)
                {
                    GameManager.Instance.PlayClip(Jaws1);
                }
                else
                {
                    GameManager.Instance.PlayClip(Jaws2);
                }
                _jawsAlternate = !_jawsAlternate;
            }
        }
    }
#endif

    private void UpdateFreeLives()
    {
        if (SafeGameManager.PlayController.Score > _nextFreeLifeScore)
        {
            SafeGameManager.PlayController.Lives++;
            _nextFreeLifeScore += FREE_USER_AT;
            SafeGameManager.PlayClip(FreeLifeSound);
        }

    }

    private bool IsGamePlaying()
    {
        return Level > 0;
    }

#if OLD_WAY
    private List<Vector3> MakeRandomPath()
    {
        var camRect = GetCameraWorldRect();
        var path = new List<Vector3>();

        path.Add( new Vector3(_camRect.xMin, Random.Range(_camRect.yMin*0.8f, _camRect.yMax*0.8f)));

//        for (var segment = 0; segment <= 4; segment++)
  //      {
    //        path.Add( )
     //   }
        path.Add( new Vector3(_camRect.xMax, Random.Range(_camRect.yMin * 0.8f, _camRect.yMax * 0.8f)));

        if (Random.Range(0, 2) == 0)
        {
            path.Reverse();
        }

        return path;
    }
#endif

    public void OnDestroy()
    {
        if (_players != null)
        {
            foreach (var birdPlayer in _players)
            {
                if (birdPlayer != null) // May have been destroyed when scene was unloading and somehow unity NULLs the object.
                {
                    Destroy(birdPlayer.gameObject);
                }
            }
            _players = new List<Player>();
        }
        if (_instructions != null)
        {
            Destroy(_instructions);
        }
        if (_gameOver != null)
        {
            Destroy(_gameOver);
        }
        //ClearAsteroids();
    }
#if OLD_WAY
    private void ClearAsteroids()
    {
        if (_asteroids != null)
        {
            foreach (var ast in _asteroids)
            {
                Destroy(ast.gameObject);
            }
            _asteroids = new List<Asteroid>();
        }
    }
#endif

    private bool CanStartGame()
    {
        if (Time.time < _disableStartButtonUntilTime)
        {
            return false;
        }
        return true;
    }

    public void StartGame()
    {
        
        Level = 0;
        _nextFreeLifeScore = FREE_USER_AT;
        _players = new List<Player>(); // Only really used on  server?

        ShowGameOver(false);
#if OLD_WAY
        ClearBullets(this);
#endif
        
        ShowInstructions(false);
        StartLevel();

        SpawnNewPlayer();

        if (GameManager.Instance.isServer)
        {
            for (int ii = 0; ii < _enemyPrespawnCount; ii++)
            {
                EnemyPrespawnLater();
            }
        }
    }

    private void SpawnNewPlayer()
    {
        ClientScene.AddPlayer(this.connectionToServer, 0);
        foreach (var p in FindObjectsOfType<Player>())
        {
            var playerNetwork = p.GetComponent<NetworkBehaviour>();
            if (playerNetwork.localPlayerAuthority)
            {
                this.AttachLocalPlayer(p);
            }
        }
    }

#if OLD_WAY
    private void ClearBullets(SceneController sceneController)
    {

        Alien.ClearBullets();
        Bird.ClearBullets();
    }
#endif

    private void AttachLocalPlayer( Player player)
    {
        _players.Add( player);
    }

#if OLD_WAY
    public void HyperSpace()
    {
        _birdPlayer.transform.position = MakeRandomPos(); // Not safe on purpose
    }

    private void AddAsteroids(int astCount)
    {
        for (int ii = 0; ii < astCount; ii++)
        {
            var pos = MakeSafeAsteroidPos();
            AddAsteroidWithSizeAt( Asteroid.Sizes.Large, pos);
        }
    }

    private Asteroid GetPrefabBySize(Asteroid.Sizes astSize)
    {
        return AsteroidPrefabs[(int) astSize];
    }
#endif


    public void StartLevel()
    {
        Level++;

#if OLD_WAY
        _jawsIntervalSeconds = 0.9f;
        _jawsAlternate = true;
        _nextJawsSoundTime += _jawsIntervalSeconds;
        AddAsteroids((int) (2 + Level)); // 3.0 + Mathf.Log( (float) Level)));
        _lastAsteroidKilled = Time.time + 15.0f;
#endif
    }

    private void ShowGameOver(bool b)
    {
        _disableStartButtonUntilTime = Time.time + 1.5;
        _gameOver.GetComponent<MeshRenderer>().enabled = b;
        if (b)
        {
            _gameOver.GetComponent<Blinker>().BlinkText(2.0f, 0.1f, Color.gray);
        }
    }

    private void ShowInstructions(bool b)
    {
        _instructions.GetComponent<MeshRenderer>().enabled = b;
    }

    public void GameOver(Bird bird)
    {
#if OLD_WAY
        if (_alien != null)
        {
            _alien.PlaySound( false);
        }
#endif
        _players = new List<Player>();
        Level = 0;
        // Leave score.

        ShowGameOver(true);
        ShowInstructions(true);
    }


    public void Respawn(Bird bird, float delay )
    {

        StartCoroutine(CoroutineUtils.DelaySeconds(() =>
        {
            // Change the count AFTER the respawn occurs. It looks better.
            SafeGameManager.PlayController.Lives--;
#if OLD_WAY
            _lastAsteroidKilled = Time.time;
#endif
        }, delay));

    }

#if OLD_WAY
    public void DestroyAlien(Alien alien, bool explode)
    {
        Debug.Assert( alien.GetInstanceID() == _alien.GetInstanceID());
        _alien = null;
        if (explode)
        {
            CreateAsteroidOrAlienExplosion(alien.transform.position);
        }
        Destroy(alien.gameObject); 
    }

    public void ReplaceAsteroidWith(Asteroid ast, int p1, Asteroid.Sizes astSize, Bullet bullet)
    {
        bool removed = _asteroids.Remove(ast);
        System.Diagnostics.Debug.Assert(removed);

        CreateAsteroidOrAlienExplosion(ast.transform.position); 

        for (int ii = 0; ii < p1; ii++)
        {
            var newAst = AddAsteroidWithSizeAt(astSize, ast.transform.position);

            // Give some momentum from the bullet.
            var rigid = newAst.GetComponent<Rigidbody2D>();
            var bulletVelocity = bullet.GetComponent<Rigidbody2D>().velocity;
            var f = bulletVelocity.normalized*Random.Range(0.05f, 0.2f);
            rigid.AddRelativeForce( f, ForceMode2D.Impulse );

            // Speed up smaller ones
            //rigid.AddRelativeForce( rigid.velocity * 0.05f, ForceMode2D.Impulse);
        }

        Destroy(ast.gameObject); 

        if (_asteroids.Count == 0)
        {
            StartLevel();
        }
        else
        {
            _lastAsteroidKilled = Time.time;
        }
    }
#endif

#if OLD_WAY
    private Asteroid AddAsteroidWithSizeAt(Asteroid.Sizes astSize, Vector3 pos)
    {
        //var astSize = (Asteroid.Sizes)Random.Range(0, AsteroidPrefabs.Length);
        var spin = Random.Range(10.0f, 50.0f);
        var force = MakeRandomForce();
        var prefab = GetPrefabBySize(astSize);
        var newAst = Instantiate(prefab);
        newAst.transform.localScale = newAst.transform.localScale * 1; // Original graphics were too tiny.
        newAst.transform.position = pos;
        newAst.transform.rotation = Quaternion.identity;
        newAst.transform.parent = _asteroidContainer.transform;
        newAst.GetComponent<Rigidbody2D>().AddForce(force);
        newAst.GetComponent<Rigidbody2D>().AddTorque(spin);
        newAst.gameObject.SetActive(true);

        //newAst.GetComponent<SpriteRenderer>().sprite.bounds.size = newAst.GetComponent<Rigidbody2D>().transform.localScale = sz;
        _asteroids.Add(newAst);
        return newAst;
    }

    private void CreateAsteroidOrAlienExplosion(Vector3 position)
    {
        var explosion = Instantiate(AsteroidExplosionParticlePrefab);
        explosion.transform.parent = _asteroidContainer.transform;
        explosion.transform.position = position;
        explosion.transform.rotation = this.transform.rotation;
        //_exhaust.enableEmission = true;
        explosion.Play();
        DestroyObject( explosion, explosion.duration + 0.25f);

    }

    public int GetRemainingAsteroidCount()
    {
        int cnt = 0;
        foreach (var ast in _asteroids)
        {
            if (ast.Size == Asteroid.Sizes.Small)
            {
                cnt++;
            }
            else if (ast.Size == Asteroid.Sizes.Medium)
            {
                cnt += 3;
            }
            else if (ast.Size == Asteroid.Sizes.Large)
            {
                cnt += 6;
            }
            else
            {
                throw new NotImplementedException( "unexpected asteroid size");
            }
        }
        return cnt;
    }
#endif



}
