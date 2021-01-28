# Tanks-Game

You play as a Tank and traverse a large island map. Your mission. Destory all the turrets on the island. Before they destory you that is.

* Single player gameplay
* Tank moves as a car would
* Large map to drive around and take our enemies
* Stationary AI enemy turrets that track and fire at the player if within range

# Motivation and Learning

This is the first 3D game that I have developed. It's relatively simple but there are a lot of moving parts and pieces to learn. I wanted to make a game that worked start to finish and wanted to upscale from 2d develpment. This is my first shot.

* Learn to build a vehicle that simulates the motion and look of a car
* Learn to use multiple cameras that follow the character as they move throughout the world
* Learn to simulate projectile style shells that follow a natural arc to damage targets
* Learn to automate documentation and create html to display the documentation using Doxygen
* Laern to work under a deadline and develop on feedback

# Screenshots
![Documentation](/img/Documentation.png "HTML Documentation file sample")
![Tank Model](/img/TankModel.PNG "Model of the Tank")
![Follow Camera](/img/FollowCamera.PNG "Follow Camera")
![Terrain](/img/Terrain.PNG "Terrain")
![Turret Model](/img/TurretModel.PNG "Turret Model")
![Overhead Camera](/img/OverheadCamera.PNG "Overhead Camera")

**To see more** information about the game, including videos and images of gameplay, check out [my website](https://www.theshumaker.com/).

# Tech/framework used

**Built with**
* C#
* Unity Engine

# Features

* Tank's front wheels can turn while stationary and once movement starts the tank moves in the direction of the wheels as a car would
* The Tank will self correct while turning if the player lets go of the controls
* Tank's turret moves independently of the body
* Camera switches between follow camera and overhead
* Shell from turret travels in a mathematically correct arc (related to acceleration and gravity)

# Credits
* Original turret by [brihernandez](https://github.com/brihernandez/GunTurrets).
* Island availible [here](https://assetstore.unity.com/packages/3d/environments/landscapes/free-island-collection-104753).

