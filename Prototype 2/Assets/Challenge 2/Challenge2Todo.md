Challenge Outcome:

A random ball (of 3) is generated at a random x position above the screen
When the user presses spacebar, a dog is spawned and runs to catch the ball
If the dog collides with the ball, the ball is destroyed
If the ball hits the ground, a “Game Over” debug message is displayed
The dogs and balls are removed from the scene when they leave the screen

# Bugs to Fix:
## 3. Dogs are spawning at the top of the screen
[] Make the balls spawn from the top of the screen.

## 4. The player is spawning green balls instead of dogs
[] Make the player spawn dogs

## 5. The balls are destroyed if anywhere near the dog
[] The balls should only be destroyed when coming into direct contact with a dog.

## 6. Nothing is being destroyed off screen
[] Balls should be destroyed when they leave the bottom of the screen and dogs should be destroyed when they leave the left side of the screen.

## 7. Only one type of ball is being spawned
[] Ball 1, 2, and 3 should be spawned randomly

## 8. Bonus: The spawn interval is always the same
[] Make the spawn interval a random value between 3 seconds and 5 seconds

## 9. Bonus: The player can “spam” the spacebar key
[] Only allow the player to spawn a new dog after a certain amount of time has passed


# 10. Hints
## Make the balls spawn from the top of the screen
Hint - Click on the Spawn Manager object and look at the “Ball Prefabs” array
## Make the player spawn dogs
Hint - Click on the Player object and look at the “Dog Prefab” variable
## The balls should only be destroyed when coming into direct contact with a dog
Hint - Check out the box collider on the dog prefab
## Balls should be destroyed when they leave the bottom of the screen and dogs should be destroyed when they leave the left side of the screen
Hint - In the DestroyOutOfBounds script, double-check the lowerLimit and leftLimit variables, the greater than vs less than signs, and which position (x,y,z) is being tested
## Ball 1, 2, and 3 should be spawned randomly
Hint - In the SpawnRandomBall() method, you should declare a new random int index variable, then incorporate that variable into the Instantiate call
## Bonus - Make the spawn interval a random value between 3 seconds and 5 seconds
Hint - Set the spawnInterval value to a new random number between 3 and 5 seconds in the SpawnRandomBall method