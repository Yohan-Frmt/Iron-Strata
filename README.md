# Iron Strata

Iron Strata is a high-stakes survival roguelike set within a boundless, brutalist world of infinite concrete and forgotten machinery. Players command The Strata—a moving fortress that serves as the last functioning ecosystem for the remnants of humanity. Every run is a journey through procedurally generated tunnels, hostile silicon-based lifeforms, and the crushing indifference of an architecture that has outlived its purpose.

The game combines the spatial strategy of a deckbuilder, the tension of tower defense, and the emotional depth of a colony management simulator.

## Design Philosophy: Industrial Existentialism

Iron Strata is built upon the lens of "Industrial Existentialism":

*   **Industrial:** Survival is a process of inputs and outputs. The player is not a hero, but an engineer managing a complex machine to prevent it from seizing up.
*   **Existentialism:** The Megastructure is indifferent to human life. Survival itself is the meaning, with every choice and every loss carrying weight in a cold, eternal universe.

## Core Gameplay Pillars

### I. The Living Machine (Spatial Strategy)
Cards are replaced by physical Wagons that must be attached to The Strata in three-dimensional space.
*   **Spatial Puzzle:** Placement is permanent and determines synergy. Neighbors provide bonuses, while height affects vulnerability and structural stability.
*   **Structural Sway:** Tall configurations risk dynamic instability at high speeds, requiring balancing and stabilization.

### II. The Human Component (Micro-Society)
Every passenger is a unique individual with a name, history, specialty, and stress level.
*   **Management:** Passengers require food (Rations), rest (Bunks), and medical care. Their performance degrades as stress increases.
*   **Stakes:** Loss is permanent. The systems are designed to foster attachment, making the survival of every crew member a critical priority.

### III. The Brutalist Odyssey (Atmosphere)
Inspired by the works of Tsutomu Nihei (BLAME!), the game focuses on scale incongruity—the contrast between fragile humanity and the gargantuan, raw concrete architecture of the Megastructure.

## Key Systems

### The Strata & Wagons
The train consists of a Locomotive and various specialized Wagons:
*   **Combat:** Weapon platforms, shield emitters, and electronic warfare systems.
*   **Support:** Kitchens, Bunks, Med-Bays, and Power Conduits.
*   **Production:** Hydroponics, Scrap refineries, and Research units.
*   **Experimental:** Rare, powerful technology salvaged from the deep Megastructure.

### Navigation and Economy
Players navigate a Node Map, choosing between Combat, Resource, Settlement, and Mystery nodes.
*   **Rations:** The constant drain on resources representing food and life support.
*   **Scrap:** The primary material for construction, repairs, and upgrades.
*   **Knowledge:** A meta-resource used for permanent progression between runs.

### Silicon Life
The antagonists are not machines, but silicon-based organisms performing maintenance protocols on the Megastructure. They range from swarming Crawlers to massive, intelligent Sentinels and the mysterious Observers.

## Technical Specifications

*   **Engine:** Godot 4 (Vulkan Forward+ Renderer)
*   **Logic:** C# (.NET 8)
*   **Architecture:** Component-based data architecture with multi-threaded procedural generation.
*   **Sound:** Procedural audio engine generating dynamic industrial soundscapes and drones.

## Project Structure

*   `Scenes/`: Godot scene files for HUD, Wagons, and Enemies.
*   `Scripts/`: Core logic, ECS (Entity Component System), and game systems.
*   `Resources/`: Game assets including models, images, and card definitions.
*   `pr.md`: Full design documentation and technical specification.

## Development Roadmap

1.  **Phase 1: Vertical Prototype** – Core spatial mechanics, basic wagon types, and the combat loop.
2.  **Phase 2: Social Engine** – Full passenger system, stress mechanics, and Pilot meta-progression.
3.  **Phase 3: The Megastructure** – Content completion, all zones, bosses, and narrative systems.
