# Nishijin Power Flash Thunderbird
## Balls out
[x] The Queen lamp should light when the balls are out of stock from the balls feed on top of the machine.
[x] you win flag appears if the prize total balls was all paid
[x] Add jackpot total reserve(reserve of balls that the machine can hold to do the payouts)
[x] subtract the prize total balls after payout

## Not Enough balls to Start Playing
[] the lamp should lights if there is no balls to throw from the white tray that go to the shooting point.
[] collect white tray balls to shooting credits
[] disable the queens lamp after adding credits
[] rename credits to reserved ball to shoot?

## Topmost on Tulip of each side(King Tulip)
[x] When balls enters one of the Topmost Tulip of each side, it opens all tulips on it's side that it is located.
[x] spit outs some amount of balls
[x] The Tulip Will close after the next ball enters the tulip
    [x] will spit some bonus balls(payout) again after closing
[x] Will close all tulips on it's side after the next ball enters the Queen tulip.(yellows)
    [x] will spit some bonus balls(payout) again after closing the tulips
[x] King(and Queen in some machines) lamps is on when the payout is in process

## Pink Tulip
[x] If a ball enters an open pink tulip it pays a prise and closes

## Orange Tulip
[x] If a ball enters a closed orange Tulip it will open and pays a prize
[x] If a ball enters an open orange tulip it pays a prise and closes
[x] If a ball enters the top most orange tulip, it will open all tulips on it's side(except the pink)

## Topmost Center pocket is the Jackpot
[x] If a ball enter the jackpot pocket it will open all the Tulips and [x ] give the player a payout!
[x] the ball should fall straight down to the pink tulip // analogy

## Thunderbird's wing
[x] If a ball enter one of the wings of the Thunderbird a payout is made.

## Dai's pockets
[x] if a ball falls on the one of the sides of the Dai's pockets, it have to open the tulips on the same side(some machines open the pink tulip too)
[x] pays a prize(don't pay in some machines)

## Yellow pocket on the bottom
[x] If a ball enter on the yellow pocket it will pays a prize.
[x] closes all tulips that are open on its side(except the pink)
[x] make the star spin if a ball enter the yellow pocket too


## TODOs
[x] Add shooting sound effects
[x] Add jackpot sound effects
[x] Add balls spitting out on Jackpot
[x] Add Tulips opening sound effects
[x] Add Tulips closing sound effects
[x] Add Wings Balls collector box collider as trigger and counter script
[x] Add free camera
[x] Add Game Menu
    [x] difficulty menu
    [x] restart option
    [x] pause mechanism
    [x] start game paused(not shooting balls)
    [x] start game after choose difficulty
    [x] hide main menu after choose difficulty
    [x] show game menu on game start
    [x] hide the game panel before game starts
[x] Add Entering the ball lose hole audio
[x] Add ball enter top most pocket Audio
[x] Ball and Nail Collision Sound Effects
[x] Add option to the player to use auto shoot or manual shoot
[x] Add an individual Count for every pocket too
[x] Add lights on Queen
[x] Add lights on King
[x] Add lights on the Victory Lamp
[x] Separate the machine state into a Single Machine Manager
[x] Animate the lever when shooting
[x] wire the King lamp to light when a jackpot is underway
[x] Add payout spot
[x] Add Collect btn
[x] Add missing tulips card identities on prefab
[x] Animate the collect btn(orange)
[x] replace the close and open of the tulips by Animation to reduce errors
[x] Add basket of balls
[x] make shoot if player clicks the lever
[x] Fix failing jackpot sound and lamp fx
[x] Add Start Auto shoot switch on the panel
[x] make auto shoot shoots if is on
[x] Add Manual Shoot input Action
[x] fix auto shoot not starting

## Functional and Features
[x] make long press call many times collect balls
[x] make long press call many times use balls
[x] catch missed shoots on the shooting chamber
[x] Add missed balls Counter
[x] Add payout collector
[x] Add payout exit to collect tray
[x] Add Recharge credits btn
[x] Display available credits
[x] change the count, lost text color
[] Redesign panel( the count, lost, etc...)
[x] make the orange btn on the machine transfer the balls to a basket

[x] instantiate baskets after collecting n balls on a visible place
[x] put balls that escaped/falled from the machine into the green tray
[x] Add move balls to basquet on action input
[] Reorganize Settings Menu
[] Add Sound Fx Volume
[] Add Music volume
[x] Add shooting settings into menu
    [x] set the checkbox on Awake
[] Make the machine use the balls of white tray if any
    [] Remove the credits amount
    [] Make the Initial balls spawn from the jackpot payout

[] make spawn spendable balls on white tray otherthan add on the game manager on the flair

[] Create Various Levels and the their Various payouts settings
[] Make add the balls on basquet to the credits(pop dialog to confirm use)
[] Sqlite
    [] wire stacked bar graph
    [] wire line graph
[] Unity Services
    [] Add login btn
    [] retrieve users informations
    [] save the users informations into the database
[] Offline and cache
    [] Add lifecycle that checks the synchronized data(to send what is not sent)
[x] Circular slider to show the shooting levels
[x] enable the circular slider on play
[x] disable the circular slider when game not running
[x] wire the circular slider to the trigger

[x] Add Localization Package
[x] Add language selection
[x] Fix the non visible Japanese Characters
[x] Translate Game
### to add localization to an text: -on text ... menu,
     choose Add Binding -> Type = Localized String
      -> Select Entry -> press Add Binding

## Visual
[x] replace the metal material
[x] Animate the collect white switch
[] clicking the collect switch should switch
[] Add tilt the Machine to move balls that are stuck
[x] stacked chart
    [] background
    [] scaling
[x] line graph
    [] background
    [] scaling
[x] change the hdri
[x] Add more scene objects to enrich environment
[] Add more cameras
[x] better illumination

## Sound Fx
[] Add Wing switching Audio

## Bugs
[] Semiautomatic shooting shoots even without credits
[x] Fix the conflicting existence of having manual and automatic triger from circular slider
[] Fix the null exception if no balls on the green tray
[] Make the use of balls add credits and poweroff the queens light
[] Use ball is not updating the Credits
[] Manual Shoot don't subtract from credits
[] Manual Shoot can shoot even if credits are zero
[] Game just shoot once and stops after restarting the game
[] Add an Queue for the jackpot payout to not cutoff the soundfx
[] don't play the payout sound fx if there is no jackpot reserves
[] BUG: review if the ball is counted twice or more
[] Fix the broken king lamp is always on
[] fix zoom out(scrolling is not taking effect)
[] fix animation of manual shoot
[] Fix the animation of long pressing orange btn when released
[] Fix missed balls that comeback to the shooting place
[] Fix the animation flick when releasing the collector switch

## Performance
[] put the audio source on the nail not on the balls(performance)

## Code Improvements & Refactories
[] eliminate game manager tight copling
    [] clean duplicated codes
[] Separate the btn logic from the machine?
[] Separate the lever logic from the machine?

## Adjustments
[] Adjust the semiautomatic shoot settings