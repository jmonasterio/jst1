using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Assets.scripts;
using Assets.scripts.behaviors;
using Toolbox;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
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
    public Egg EggPrefab;
    public GameOver GameOverPrefab;
    public Instructions InstructionsPrefab;
    public AudioClip LevelStartSound;

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
    public int _remainingEnemiesToSpawnOnThisLevel;

    private int _nextFreeLifeScore;

    private GameOver _gameOver;
    private Instructions _instructions;
    private List<Player> _players;
    private List<Enemy> _enemies;
    private List<Egg> _eggs; 

    public IList<Player> Players
    {
        get { return _players; }
    } 

 //   private List<Asteroid> _asteroids = new List<Asteroid>();
//    private Alien _alien;

    //private float _nextJawsSoundTime;
    //private float _jawsIntervalSeconds;
    //private bool _jawsAlternate;
    private double _disableStartButtonUntilTime;
    public Player LocalPlayer;
    private const int MAX_ENEMIES = 3;
    //private GameObject _asteroidContainer;
    //private float _lastAsteroidKilled;

    private Transform GetRandomSpawmPoint()
    {
        int pt = Random.Range(1, 4);
        string name = "SpawnPoint (" + pt + ")";
        return SafeGameManager.SceneRoot.Find(name);
    }

    public Vector3 GetRandomSpawnPoint()
    {
        var spawnPoint = GetRandomSpawmPoint();
        var pos = spawnPoint.position;
        return pos;

    }

    private void EnemyPrespawnLater()
    {
        _enemyPrespawnCount++;
        _remainingEnemiesToSpawnOnThisLevel--;

        StartCoroutine(CoroutineUtils.DelaySeconds( 2.0f, () =>
        {
            AddSomeEnemies(1);
        }));
    }

    // TBD: Should be event.
    public void KillEnemy(Enemy enemy)
    {
        if (!GameManager.Instance.isServer)
        {
            return;
        }

        var rigidbody2D = enemy.GetComponent<Rigidbody2D>();
        EggSpawn(enemy.transform.position, rigidbody2D.velocity, MakeMother(enemy)); 
        DestroyEnemy(enemy);
    }

    private Egg.Mothers MakeMother(Enemy enemy)
    {
        // TBD: Figure out mother based on type of enemy.
        return Egg.Mothers.Enemy1;
    }

    private void EggSpawn( Vector3 pos, Vector3 v, Egg.Mothers mother)
    {
        var egg = EggPrefab.InstantiateInTransform(null);
        _eggs.Add(egg);
        egg.transform.position = pos;
        egg.Hatched += egg_hatched; //not firing.
        egg.GetComponent<Pickup>().PickedUp += egg_pickedUp;

        NetworkServer.Spawn(egg.gameObject);
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

            if (!IsSafeSpawnPoint(pos))
            {
                continue;
            }
            SpawnEnemyAt(pos);

            // Do this in OnStartServer, because sound needs to be played in the client.
        }
    }

    public void SpawnEnemyAt(Vector3 pos)
    {
        var enemy = EnemyPrefab.InstantiateInTransform(null);
        _enemies.Add(enemy);
        enemy.transform.position = pos;

        NetworkServer.Spawn(enemy.gameObject);

        _remainingEnemiesToSpawnOnThisLevel--;
    }

    private bool IsSafeSpawnPoint(Vector3 pos)
    {
        const float MAX_DISTANCE_FROM_SPAWN = 0.7f;
        if (_enemies.Any(_ => Vector3.Distance(_.transform.position, pos) < MAX_DISTANCE_FROM_SPAWN))
        {
            return false;
        }
        if ((_players != null) && (_players.Any(_ => Vector3.Distance(_.transform.position, pos) < MAX_DISTANCE_FROM_SPAWN)))
        {
            return false;
        }
        return true;
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
        _eggs = new List<Egg>();
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

    public int GetEnemyCountForLevel()
    {
        return MAX_ENEMIES + Level;
    }

    // Update is called once per frame
    void Update()
	{
        try
        {

        // TBD: These could be components. Especially JAWS sound and free lives.

        UpdateFreeLives();

        if ( ShouldSpawnMoreEnemies() )
        {
            Debug.Assert(_remainingEnemiesToSpawnOnThisLevel > 0);
            Debug.Assert((_players.Count > 0));
            EnemyPrespawnLater();
        }
        else if (IsLevelOver())
        {
            StartLevel();
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

    private bool ShouldSpawnMoreEnemies()
    {
        return (_remainingEnemiesToSpawnOnThisLevel > 0) && (_players.Count > 0);
    }

    private bool IsLevelOver()
    {
        return  (_enemies.Count == 0) && (_eggs.Count == 0)  && (_enemyPrespawnCount <= 0 ) && (_remainingEnemiesToSpawnOnThisLevel <= 0) && (_players.Count > 0);
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
                SetPlayerDead( p, false);
                this.AttachLocalPlayer(p);
            }
        }
    }

    public void RespawnPlayer(Player p)
    {
        //print("Start player respawn timer.");
        // Assume they still have more lives for now.
        StartCoroutine(CoroutineUtils.DelaySeconds(1.0f, () =>
            StartCoroutine(CoroutineUtils.UntilTrue(() =>
            {
                var spawnPoint = SafeGameManager.SceneController.GetRandomSpawnPoint();
                if (IsSafeSpawnPoint(spawnPoint))
                {
                    p.RespawnAt(spawnPoint);
                    if (!_players.Contains(p))
                    {
                        this.AttachLocalPlayer(p);
                    }
                    SetPlayerDead( p, false);
                    return true;
                }
                else
                {
                    return false;
                }
            }))));

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
        LocalPlayer = player;
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
        _remainingEnemiesToSpawnOnThisLevel = GetEnemyCountForLevel();
        SafeGameManager.PlayClip(LevelStartSound);
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

    public void GameOver(Player p)
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

    private void egg_pickedUp(Pickup pickup, EventArgs e)
    {
        if (pickup.PickedUpBy.GetComponent<Player>() != null)
        {
            // TBD: Increase score
        }
        else if (pickup.PickedUpBy.GetComponent<Enemy>() != null)
        {
            // TBD: Powerup enemy.
        }
        else
        {
            Debug.Assert(false, "Who picked up egg?");
        }

        Cleanup(pickup.PickedUpItem.GetComponent<Egg>());
    }

    private void egg_hatched(Egg egg, EventArgs e)
    {
        var pos = this.transform.position;
        pos = new Vector3(pos.x, pos.y + 0.1f, pos.z); // Spawn player a little higher so feet not in ground.
        SafeGameManager.SceneController.SpawnEnemyAt(pos);
        Cleanup(egg);
    }

    private void Cleanup(Egg egg)
    {
        _eggs.Remove(egg);
        egg.Hatched -= egg_hatched;
        Destroy(egg.gameObject);
    }

    // TBD: Should be event.
    public void KillPlayer(Player pl)
    {

        SetPlayerDead(pl, true);
        _players.Remove(pl);

        if (SafeGameManager.PlayController.Lives < 1)
        {
            if (_players.Count == 0)
            {
                SafeGameManager.SetGameState(PlayController.States.Over);
                GameOver(pl);
            }
        }
        else
        {
            RespawnPlayer(pl);
        }
    }

    private void SetPlayerDead(Player pl, bool dead )
    {
        pl.IsDead = dead; // So enemies don't follow anymore
        pl.gameObject.SetActive( !dead);
        if (!dead)
        {
            SafeGameManager.PlayClip(pl.GetComponent<Bird>().SpawnSound);
            pl.GetComponent<Blinker>().Blink();
        }
        else
        {
            SafeGameManager.PlayClip(pl.GetComponent<Bird>().DieSound);
        }
        //Show(pl, !dead);
        //pl.transform.position = new Vector3(-10000f,-10000f,-10000f);
        // Move player off screen until respawn (so we don't see him anymore??)
        //var wrapped = pl.GetComponent<Wrapped2D>();
        //wrapped.StartTeleportTo(new Vector3(-10000f, -10000f, -10000.0f));
    }

    private void Show(Player pl, bool show)
    {
        foreach (var sr in GetComponents<SpriteRenderer>())
        {
            sr.enabled = show;
        }

    }
}
