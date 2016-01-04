// GOALS: Improvements from Ast3

// Problems with network manager
//    GameManager needs to be a networkbehaviour and have a network identity.
//    GameManager can't be on offline screen because the logic for moving between screens creates copies of the singleton which you can see in the hierarchy.

// Scene should have an array of spawn points.
// Overload cast operator. Components are a lot like COM interfaces.
// x Eggs - When player dies, create an egg too.
// x 2D scrolling.
// Slow down enemy vertical. Too fast.
// Eggs - What do they give me.
// Hatch enemy eggs
// Enemy should understand that the world wraps.
// Can get stuck if enemies park on spawns.
// Multi-game. Need to have same screensize everywhere. <-- Could be resolved by not wrapping. Or scaling screen to make sure it fits.
// Multi-game. Guy sticks to top.
// Multi-game. Extra player invisible.
// x Fallback to single player.
// Score
// Lives
// Dragon
// Flames
// Hand
// Asset bundles
// Save game info.
// Side scroll for a while before wrapping. Or just have a wall at the edge after some side scrolling.
// Better AI enemies.
// New level when all spawned enemies are killed.
// Remote player doesn't "walk" on the server. 
// Todo: Feet collider should only stop you from hitting ground. It should NOT collide with another player while flying.
// In orginal game, your feet would not collide with floor if you hit it fast horizontally (on a platform, so you'd schootch alonig).

// x http://wiki.unity3d.com/index.php/Toolbox
// x Instantiate multiple enemies at spawn locations.
// x Teleport across screen in network mode is problematic because of collions and "INTERPOLATE" mode on the NetworkTransform.
// x Enemies come up on top of each other.

// x Handle player respawn.
// x Enemies don't have feet.
// x Enemies should track.
// x Spawned enemy doesn't have SOUND.
// x Death and respawn.
// x On connection, need to spawn new player.
// x Finish sound: Spawn and bounce
// x AI enempies
// x Animated player.
// x Quit menu.
// x Network buttons
// x Shared ToolBox.DLL - Buggy with 
// x Networking support for 2-player
// x Auto-deploy to GITHUB.
// x Why sound delayed in EDITOR? Problem with MIXER.


// TODO:
// Refactor scene controller a bit more
