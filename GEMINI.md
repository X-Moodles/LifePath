# LifePath - AI Native Unity Project

## Project Overview
**LifePath** is a Unity-based simulation game that explores life choices through the lens of algorithms. Players (or AI agents) navigate through decades of life (from age 0 to 60+), making decisions that impact their Value, Stress, and Weight. The core mechanic involves selecting different pathfinding and optimization strategies (Manual, Greedy, Backtracking, DP) to maximize life outcomes within time and stress constraints.

## Getting Started

### Prerequisites
*   **Unity Editor:** Required to open and run the project. (Check `ProjectSettings/ProjectVersion.txt` for the exact version if needed, likely 2021.3+ based on package manifestations).

### Running the Game
1.  Open the project in Unity.
2.  Navigate to `Assets/Scenes/SampleScene.unity`.
3.  Press **Play**.
4.  **Controls:**
    *   **UI:** Select strategies using the in-game UI (handled by `UIManager`).
    *   **Keyboard Shortcuts:**
        *   `1`: Manual Strategy
        *   `2`: Greedy Strategy
        *   `3`: Backtrack Strategy

## Core Architecture

### 1. Game Loop (`GameManager.cs`)
The `GameManager` acts as the central singleton controlling the game flow:
*   **Time System:** Simulates time passing (Decades). Time speed is affected by player stress.
*   **Lifecycle:** Manages `StartNextDecade`, `EndDecade`, and `GameOver` states.
*   **Win/Loss:** Game ends if Stress > 100 or Age >= 60.

### 2. AI & Strategy System (`AISystem.cs`, `Strategies/`)
This is the "AI Native" aspect of the project. The player's movement is determined by interchangeable strategies implementing the `IStrategy` interface.
*   **Interface:** `IStrategy` defines `CalculateMove(PlayerEntity, List<ItemEntity>)`.
*   **Implementations:**
    *   `ManualStrategy`: User controlled.
    *   `GreedyStrategy`: Likely picks the nearest high-value item.
    *   `BacktrackStrategy`: Search-based approach.
    *   `DPStrategy`: Dynamic Programming approach (placeholder or implemented).

### 3. Data & Events (`LifeEvent_SO.cs`)
Game content is data-driven using ScriptableObjects.
*   **LifeEvent_SO:** Defines events (e.g., "Study", "Game", "Travel") with properties:
    *   `Value`: The benefit gained.
    *   `Weight`: The cost or burden.
    *   `Stress`: Impact on the player's stress level.

### 4. Systems
*   **EventManager:** Decouples game logic from UI using C# Actions/Events (`OnTimeChanged`, `OnStatsChanged`).
*   **MapSystem:** Handles world generation and item spawning.
*   **StatsSystem:** Manages player statistics.

## Development Conventions
*   **Language:** C#
*   **Comments:** The codebase extensively uses **Chinese** comments for documentation and logic explanation.
*   **Pattern:** Heavily relies on **Strategy Pattern** for AI and **Observer Pattern** (via `EventManager`) for UI updates.
*   **Dependencies:** Uses `TextMeshPro` for UI text rendering.

## Directory Structure
*   `Assets/Scripts/Strategies/`: Contains all AI algorithms.
*   `Assets/Scripts/Systems/`: Core systems (AI, Map, Stats, UI).
*   `Assets/Resources/Events/`: ScriptableObject assets defining game events.
