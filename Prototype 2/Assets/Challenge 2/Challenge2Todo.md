Challenge Outcome:

A random ball (of 3) is generated at a random x position above the screen
When the user presses spacebar, a dog is spawned and runs to catch the ball
If the dog collides with the ball, the ball is destroyed
If the ball hits the ground, a “Game Over” debug message is displayed
The dogs and balls are removed from the scene when they leave the screen

# Bugs to Fix:
## 3. Dogs are spawning at the top of the screen
[x] Make the balls spawn from the top of the screen.
-replace the wrong prefabs

## 4. The player is spawning green balls instead of dogs
[x] Make the player spawn dogs
-replace the wrong prefab

## 5. The balls are destroyed if anywhere near the dog
[x] The balls should only be destroyed when coming into direct contact with a dog.
-adjust the box collider boundaries

## 6. Nothing is being destroyed off screen
[x] Balls should be destroyed when they leave the bottom of the screen and dogs should be destroyed when they leave the left side of the screen.
- adjust x destroy coordinate from positive to negative, and fix the wrong condition from > to <
- fix the wrong destroy direction for balls that should be position.y

## 7. Only one type of ball is being spawned
[x] Ball 1, 2, and 3 should be spawned randomly
-use Random.Range method

## 8. Bonus: The spawn interval is always the same
[x] Make the spawn interval a random value between 3 seconds and 5 seconds

## 9. Bonus: The player can “spam” the spacebar key
[x] Only allow the player to spawn a new dog after a certain amount of time has passed


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


# Bonus Features 2
## Easy: Vertical player movement
[] Allow the player to move forward and backwards within a certain range.

## Medium: Aggressive animals
[] Have animals that also spawn from the left and right side of the screen. If one of them hits you, “Game Over” should be logged to the console.

## Hard: Game user interface
At the start of the game, display in the console that the player’s Lives = 3 and Score = 0. If the player feeds an animal, increase and display the Score. If the player misses an animal or is hit by one, decrease and display the Lives. When the number of Lives reaches 0, log “Game Over” in the console.

## Expert: Animal hunger bar
Display a “hunger bar” on top of each of the animals. Then, each time you feed one of them, the hunger bar fills up a little. Each animal should require different amounts of food to successfully “feed” them. They should only disappear after their hunger bars are full.

# 6. Hints and solution walkthrough
Hints:

## Easy: Vertical player movement
Look at how we are doing the left and right movement range.
## Medium: Aggressive animals
Look at how we are currently spawning animals and doing collisions.
## Hard: Game user interface
You will need to update the score and lives in the DetectCollisions script, and update the lives in the DestroyOutOfBounds script.
## Expert: Animal hunger bar
You will need to add a UI Slider object in World space Render Mode as a prefab for each animal, then set the slider’s value through a script every time the animal is fed.
