// GOALS: Improvements from Ast3

// Problems with network manager
//    GameManager needs to be a networkbehaviour and have a network identity.
//    GameManager can't be on offline screen because the logic for moving between screens creates copies of the singleton which you can see in the hierarchy.

// x http://wiki.unity3d.com/index.php/Toolbox
// x Instantiate multiple enemies at spawn locations.
// x Teleport across screen in network mode is problematic because of collions and "INTERPOLATE" mode on the NetworkTransform.
// Enemies come up on top of each other.

// x Spawned enemy doesn't have SOUND.
// x Death and respawn.
// Remote player doesn't "walk" on the server. 
// Todo: Feet collider should only stop you from hitting ground. It should collide with another player while flying.
// In orginal game, your feet would not collide with floor if you hit it fast horizontally (on a platform, so you'd schootch alonig).
// x On connection, need to spawn new player.
// x Finish sound: Spawn and bounce
// Network spawning points.
// AI enempies
// x Animated player.
// Eggs
// Multi-game. Need to have same screensize everywhere.
// Multi-game. Guy sticks to top.
// Multi-game. Extra player invisible.
// Fallback to single player.
// Score
// Lives
// Dragon
// Quit menu.
// Network buttons
// Hatch enemy eggs
// Flames
// Hand
// x Shared ToolBox.DLL - Buggy with 
// Asset bundles
// Save game info.
// x Networking support for 2-player
// x Auto-deploy to GITHUB.


// TODO:
// Why sound delayed in EDITOR
// Refactor scene controller a bit more
