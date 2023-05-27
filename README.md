# PlanetGravity
Learning Neural Network to control rocket flight in Unity 2D with Genetic Algorithms.

## How to launch a project
Open project in Unity. In `Scenes` folder there are two scenes named `HumanPlayerScene` and `AIPlayerScene`. Open one of these and start execution.

### AI Player
AI player will learn to control rocket flight by itself. You can see how fitness values are changing in the left side of interface with new generations.
Still you can change some values of `Academy` object to experiment with learning process. You can also take a look at `GeneticController` and `AIRocketController` scripts.

### Human Player
The main purpose is learning Neural Network, so this scene is just for simulation testing (or fun).
If you start project with "HumanPlayerScene" you can control flight of rocket by yourself: 
- control direction with arrowkeys;
- change value of velocity that you add to rocket on clicking arrow keys with "Q" and "E" keys;
- you can also experemint with checkboxes of rocket object, though most of them are related to training Neural Network;
- you can make run simulation faster changing `TimeScale` value in `SimulationHandler` object.

## Simulation
I used Newton's law of universal gravitation to simulate movement of objects in the system, though I turned off gravity force on Earth to keep it immovable, but you can play with checkboxes to see what happens. Also you can try to build your own Solar system.

To make Moon move around the Earth I assigned initial velocity (impulse) to it. It should represent the real world velocity in our scale, I managed to calculate it so simulation looks natural.

I tried to use real world scales for this project: distance between Earth and Moon, their sizes and masses are similar to real. But rocket is much heavier and bigger than real, I couldn't make such small details work in Unity.

## Notes
- Simulation needs to be improved, collisions may work wrong.
- Neural Network learns good if you reset Moon position on each rocket launch (experiment = 1 in `Academy`). But I couldn't make Neural Network learn good for different Moon positions (experiment > 1). 
- I will update readme later to make it more informable, now this project is in progress.
