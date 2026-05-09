# bank-heist-oop-game

## Overview

This project was mainly a way for me to consolidate my OOP when I first started using object oriented ideas, I wanted a way to train my skills while not making an overly complex outcome, hence the simple game.

**Core Loop** - `Bank` class controls everything, updating guard positions, rendering the display, processing the players actions, expiring used items and checking for any end conditions each turn. Repeates indefinitely until player is ends the game, is caught or inputs 'quit' as their action.

**Layout and Room Management** - `Layout` class genereates all rooms when the program complies and connects them with different `Door` objects, standard for all rooms other than `rSafeDep` and `rVault` which are connected through a `VaultDoor` object.

**Guards and Player Actions** - Both guards and the player are inherit from the `Entity` base class and move around the bank at the end of each cycle. Guards follow a fixed path assigned to each upon creation of a new `Guard` object, whereas players move freely between any room in their current location `ConnectedRooms` list. Players have the option to carry out other actions as well such as taking, dropping or using items when possible. At the start of each turn the guard's method `CheckRoom` is called to see if an undisguised player is in the same room as them, if so the `CatchPlayer` method is subsequently called, ending the game.

**Items** - All item classes inherit from the `Item` base class and are given hard-coded values for worth, display name, whether it will fit a bag item. Select items also have uses and that can benifit the player durring the game and a number of uses before it is discarded. If an item has no use (e.g. cacsh) the number of eses is set to -1.

**Rendering** - The `Display` class prints the updated UI every time the user enters an input and shows the uesr's take, items avalible to pick up in a room, the user's inventory and bag (if equipped), the rooms a user can move to and whether there will be a guard there on the next turn, and the list of possible actions for the user to pick from.

**Win/Loss Conditions** - The game ends when: the player moves to a restricted room occupied by a guard while not having a disguise equipped, the player entering 'E' after returning to the street with their total take, the player entering 'quit' when asked to input an action

## Gameplay

Play as a theif attempting to rob a bank, moving through room, collecting items and avoiding guards. The game ends when you succesfully exit the building and make it back to the street with the items you have stolen or when a guard catches you in a restricted area of the bank.

## Controls

|Input|Action|
|-|-|
|`W`| Walk - Move to another room
|`T`| Take item - Add an item from the room to your inventory
|`D`| Drop item - Remove an item from your inventory
|`U`|Use an item from your inventory or bag|
|`E`|End the heist (must be on the street)|
|`quit`|Quit the game immediately|

## Rules

* **Restricted Areas** - All room past the Main Entrance and the Lobby are restricted - if a guard find you here the game ends with you being caught
* **Hiding Rooms** - Both the Storage Room on the lower flor and Electrical on the upper floor can be used to avoid guards as they never enter these rooms
* **Guard Uniforms** - Can be equipped by the player to avoid being cauht by a guard whilst being in the same room, each uniform lasts two turns and a new one can be equipped on the turn that the current one expires on (when the interface shows **0 turns remaining**)
* **Lockpicks** - Are used to unlock locked doors, each has two uses before being discarded from the player's inventory
* **The Vault** - The final room, has no key and instead requires a code to enter. The code is hidden somewhere in the bank and can be picked up and carried as an item
* **Score** - Any items in your inventory and on the street count towards the total take at the end of the game, the take is displayed in the top right of the interface throught the game
* **Winning** - Once you have decided you have taken enough, make  your way back to the street and enter 'E' to end the game

## Map

```
Street
  └── Main Entrance
        └── Lobby
              ├── Teller Station
              ├── Hall
              │     ├── Office
              │     │     ├── Safe Deposit Room
              │     │     │     └── Vault  ← requires code
              │     │     └── Elevator
              │     │           └── Landing (Upper Floor)
              │     │                 ├── Manager's Office
              │     │                 ├── Server Room
              │     │                 ├── Electrical
              │     │                 └── Counting Room
              │     ├── Board Room
              │     ├── Break Room
              │     │     ├── Security Room  ← vault code is here
              │     │     └── Storage Room
              │     └── Stairwell (Down → Up → Landing)
              └── Office
```
Locked doors are marked in game, each room contains a set of items

## Items

|Item|Value|Fits in Bag|Notes|
|-|-|-|-|
|Cash|£100|Yes||
|Watch|£250|Yes||
|Necklace|£500|Yes||
|Gold Bar|£1,000|Yes||
|Bonds|£1,000|Yes||
|Hard Drive|£1,500|Yes||
|Painting|£7,500|**No**|Must be carried in inventory|
|Copper Wire|£10|Yes||
|Guard Uniform|—|Yes|Disguise; lasts 2 turns|
|Lockpick|—|Yes|Unlocks doors; 2 uses|
|Vault Code|—|Yes|Required to open the vault|
|Bag|—|No|Carries up to 6 items|

## Inventory

You have two slots, however this can be expanded to an additional six by picking up a bag. When taking or dropping items you will be asked to use your inventory or bag (if avalible). Note that the painting cannot be added to a bag and must be carried in your inventory

## Guards

Guards patrol the building, moving through fixed paths each turn. If they find you in a restriced area without a disguise equipped, the game will be ended and you will loose. While you are currently playing your turn, door option will have a **\[GUARD NEXT]** warning message if that room is the next on their path. 

## Tips

* Check the rooms for whether a guard is prest there next
* The vault code is randomly generated each game, so make sure you've found it before making your way to the vault
* Items left of the street still count towards your total take, so once you've been in once, there's still more to get by going in again
* Lockpicks and disguises have limited uses and do not regenerate, so use them carefully

