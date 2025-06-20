# Procedural Dungeon Generator

A Windows Forms app that generates procedural dungeons with support for styles like Dungeon, Spaceship, and Cave.

## Features

- Generate maps in 3 styles: **Dungeon**, **Spaceship**, and **Cave**
- Zoom and pan the dungeon map
- Load configuration from CSV and JSON
- Save map as PNG or JPG
- Supports different visual styles

## Requirements

- Windows 10 or newer
- [.NET 6.0 SDK or later](https://dotnet.microsoft.com/download)
- Any C# IDE (e.g., Visual Studio 2022+)

## Setup

1. Install .NET SDK (if not already installed)
2. Open the `.sln` file in Visual Studio
3. Build and run the project

## Usage

- Select a style and click **Generate**
- Use mouse to zoom (scroll) and pan (drag)
- Click **Save** to export the image

## Config

- `config_general.json` – sets `tileSize`, `gridWidth`, and `gridHeight`
- `config_room.csv` – defines room sizes and types per style
- `config_enemy.csv` – defines enemies per style
- `config_artifact.csv` – defines artifacts per style
- `config_item.csv` – defines items per style

