# 2048 3D Prototype - Unity

The project is a 3D physical variation of the popular "2048" puzzle game developed for Android.

## 🎮 Gameplay Features
* **Physical Interactions:** Cubes are launched using physics and collide realistically.
* **Merge Mechanics:** Same-value cubes merge into a higher power of 2 (2+2=4, etc.).
* **Controls:** Touch input with drag-to-aim and release-to-shoot mechanics.
* **Dynamic Spawning:** 75% chance for "2", 25% chance for "4".
* **Scoring System:** Points are awarded based on merge value.
* **Win/Loss Condition:** Game Over triggers when the board is full (cubes cross the dead zone).

## 🛠 Technical Implementation & Architecture
The project follows **SOLID principles** and uses key design patterns to ensure scalability and clean code:

* **Observer Pattern:** Used in `InputHandler` to decouple input logic from game logic. `GameManager` subscribes to input events.
* **Singleton Pattern:** Used for `GameManager` and `ScoreManager` to provide easy global access to core systems without messy dependencies.
* **Clean Code:** Logic is separated into distinct responsibilities:
    * `InputHandler`: Handles touch input.
    * `GameManager`: Controls game flow (Spawn, Game Over, Restart).
    * `Cube`: Handles individual physics, collisions, and visuals.
    * `ScoreManager`: Handles UI and score tracking.

## 📺 Video Demo
https://drive.google.com/drive/folders/1yadLzfP56GMIx4zwDiOmh21KItDhI1El?usp=sharing

## 📱 How to Play (APK)
The built APK file can be downloaded here:
https://drive.google.com/drive/folders/1yadLzfP56GMIx4zwDiOmh21KItDhI1El?usp=sharing
