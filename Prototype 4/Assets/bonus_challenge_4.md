# 1. Overview
This tutorial outlines four potential bonus features for the Sumo Battle Prototype at varying levels of difficulty:

Easy: Harder enemy
Medium: Homing rockets
Hard: Smashingly good
Expert: Boss battle
Here’s what the prototype could look like if you complete all four features:

The Easy and Medium features can probably be completed entirely with skills from this course, but the Hard and Expert features will require some additional research.

Since this is optional, you can attempt none of them, all of them, or any combination in between.
You can come up with your own original bonus features as well!

Then, at the end of this tutorial, there is an opportunity to share your work.

We highly recommend that you attempt these using relentless Googling and troubleshooting, but if you do get completely stuck, there are hints and step-by-step solutions available below.

# Good luck!

[x] 2. Easy: Harder enemy
Add a new more difficult type of enemy and randomly select which is spawned.

[] 3.Medium: Homing rockets
 Create a new powerup that gives the player the ability to launch projectiles at enemies to knock them off (or something that automatically fires projectiles in all directions when the powerup is enabled).

[] 4. Hard: Smash attack
Create a new powerup that allows the player to hop up into the air and smash down onto the ground, sending any enemies nearby flying away from the player. Ideally, the closer an enemy is, the more it should be impacted by the smash.

[] 5. Expert: Boss battle
After a certain number of waves, program a mini “boss battle,” where the boss has some completely new abilities. For example, maybe the boss can fire projectiles at you, maybe it is extremely agile, or maybe it occasionally generates little minions that come after you.

# 6. Hints and solution walkthrough
Hints:

Easy: Harder enemy
Try using an array for the enemy prefabs.

Medium: Homing rockets
Try using an enum to differentiate the power ups

Hard: Smashingly good
Extend the enum you created in the previous challenge

Expert: Boss battle
Create a new SpawnBossWave function that only runs if the wave number is a multiple of a particular value.
