🎮 Ocean Blast – Match & Launch Puzzle Game

Ocean Blast is a fast-paced, ocean-themed 3D tile-launch puzzle game.
Players launch colored target tiles from launcher slots and match them with the grid.
Matches trigger satisfying animations, power-ups, combos, and polished visual effects to deliver a dynamic and engaging gameplay experience.

📦 Project Features
🎯 Core Mechanics

4 Launcher Boxes: Target tiles are placed here and launched toward the grid.

Grid-Based Tile System: Colorful tiles arranged as 3D objects inside a structured grid.

Matching Logic: Launched tiles interact with the first row of the grid to check for matches.

Merge System: Three identical target tiles merge into a more powerful upgraded tile.

Goal Completion: A level is cleared once all target goals are successfully completed.

⚙️ Architecture & Performance

Event-Driven Communication using Zenject SignalBus for decoupled gameplay flow.

Dependency Injection with Zenject for modular and maintainable system structure.

Object Pooling Systems for:

Tiles

GoalItems

Splash effects

Particle systems

Fully SOLID-Compliant Modular Architecture designed for scalability and clean separation of concerns.

GridData ScriptableObject enabling easy level configuration and fast iteration.

🔥 Game Feel & Animations

Implemented using DOTween:

Smooth launch animations (DOJump + Scale)

Merge animations

Destruction & particle effects

Splash effects appearing during tile launch

Win & Game Over UI transitions

Combo highlights & visual feedback

Camera shake and zoom effects for added impact

🛠️ Technologies Used

Unity 2022.x

C#

Zenject (Dependency Injection)
