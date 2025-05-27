# 🌌 Cosmopoly

- Cosmopoly is a strategic board game set in a cosmic realm, developed using WPF (.NET) technology. 
- Players take on the role of a galactic businessman, exploring planetary systems, investing in infrastructure, dodging pirates, and competing for galactic dominance.

# 🚀 Game Rules

### 🎮 General
- Players: 2–4
- Currency: Galactic Credits
- Map: A galaxy comprised of planetary systems, special action fields, and galactic railway routes.


# 🌍 Planets and Planetary Systems
- Players claim a planet by building a spaceport on it.
- Each planet can have the following structures (if the player owns it):

### 🔹 Outpost
- Can be upgraded to:
	- Residential Habitat
	- Colony
	- Galactic Hotel
	- Planetary Hotel Network (if the player owns all planets in the system)
- Player cost for staying at a hotel is based on hotel level
	
### 🔹Mines
- Upgrades to Level 3
- passive income

### 🔹 Farm
- Upgrades to Level 5
- passive income

# 🌀 Singularity Area
- Landing on a singularity field causes the player to draw an event card. Possible effects include:
	- Pirate attack
	- Pirate defense card
	- Galactic ticket
	- Emperor collects property tax (percentage of value)
	- Win the galactic lottery
	- Ship engine failure (lose a turn + towing cost)
	- (Additionally, if the player has a shipyard): Galactic shipyard malfunction

# ☠️ Pirate Field
- Lose 2 turns
- Options:
	- Pay a ransom
	- Use a defense card
	
# 🚅 Galactic Railway
- A player with a galactic ticket can move to any railway stop.

# 🏗️ Construction and Upgrades
- Only one construction or upgrade action per planet per turn.

# 🏭 Planetary System Ownership
- If a player owns all planets in a system, they can:
	- Build a Galactic Shipyard (additional profit, not upgradable)
	- Build Mines in asteroid belts (up to Level 5) 

# 💰 Passive Income
- Evertime players visit first planet they receive income based on:
	- The number and levels of structures (outposts, mines, farms, hotels, shipyards)

# ⏭️ Skipping a Turn
- Players can skip a turn, but a maximum of 2 consecutive times.


# 🧱 Project Structure
```
Cosmopoly/
├── Engine/           <- Game logic (rules, gameplay, models, resource calculation)
├── Game/             <- WPF (.NET) game frontend, UI, board, interactions
├── GameContainers/   <- Shared UI components, controls, graphical containers
```

# 🔧 Customization
- Easily adjust game values and settings in the configs folder.

# 📦 Installation & Setup
- From Source
	1. Open the project in Visual Studio.
	2. Set the Game project as the startup project.
	3. Run the project (F5).
- From a Release Build
	1. Download the latest `.zip` archive
	2. Extract the contents to a folder.
	3. Locate and run the `Game.exe` file.

# 🌐 Languages
- Game is available in Polish.

# 👨‍💻 System Requirements
- .NET 8.0+
- Windows 10/11
- Visual Studio 2022 (recommended)

# 📜 License
- This project is created for educational and entertainment purposes.