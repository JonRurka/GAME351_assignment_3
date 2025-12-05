"# GAME351_assignment_3" 

We used Cinamachine to create the introduction cutscene with custom sound track. Virtual cameras
use the Timeline to switch between active virtual cameras, and use tracks to guide movement. Triggers
are used from the timeline to notify of important events, like switching to the camera that follows
the player, and signal when the game should start.

The game mode controller contains a state machine to determine overal gameplay logic with the following
states:

* Intro - intro cutscene
* MomentsBeforeFight - Transition from cutscene to dual.
* TexasRedDual - face off with the Ranger and Texas Red.
* FightingBandits - After shooting and killing Texas Red, fight with bandits begin, and player can move.
* Relaxing - If player hasn't shot his gun for 10 seconds, the slower pased music plays.
* Dead - Player died.
* Win - Player killed all bandits.

The game opens with the Intro cutscene, after the cutscene finishes it transitions to a standoff street dual between
The Ranger and Texas Red. After a bandit yells "Fire", the player can try to shoot Texas Red, or get shot. Player is 
locked in place during the dual, and can only look and shoot. If the player kills Texas Red, the Ranger can move 
around and shoot the bandits. The bandits randomly shoot at the Ranger at random intervals, and shot taunts at random
intervals, and cannot move. If the Ranger gets hit by any bullet, he dies.

A constant wind track is looped over the scene. The game loads up with a song specific to the cutscene, and fades into
a fighting song for the dual. If the player has not shot the gun or kicked in 10 seconds, a more relaxed song plays.
If the player is in the relaxed state and nears the supply store, a different song plays. The piano has a track that 
can be heard if the Ranger steps near it. Step sounds are played when walking.

The barrels can be blown up if hit by a bullet. Various objects can be kicked and moved a distance. There is a "Shot"
method on any object that the bullet can interact with (player, bandits, barrels) that uses SendMessage to trigger
from the bullet. The trajectory of the bullet from the player is determined from a ray shot from the center of the
camera to determine a final "end point" for the bullet, and the actuall bullet is sent from the middle of the Rangers
torso to that hit point. For the bandits, the bullet is just sent forward from their torso. The bullets shot from
bandits other than Texas Red have a random variation added to the trajectory of the bullet that can be clamped within
a min and max angle from their forward vector. Texas Red shoots straight every time and will kill the Ranger if the
player isn't fast enough. Some small amount of random variation is added to the speed at which Texas Red shoots, with
a base speed set in the inspector.