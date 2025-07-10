# Goal Post Setup Guide

## Overview
This guide will help you set up the Prop_Star as a goal post to transition between levels in your game.

## Scripts Created
1. **GoalPost.cs** - Main script that handles level transitions when player touches the star
2. **LevelManager.cs** - Optional script that provides additional level management features
3. **README_GoalPost_Setup.md** - This setup guide

## Setup Instructions

### Step 1: Prepare Your Player
1. Make sure your player character has the tag "Player"
   - Select your player object in the hierarchy
   - In the Inspector, set the Tag dropdown to "Player"
   - If "Player" tag doesn't exist, create it in the Tags & Layers settings

### Step 2: Set Up the Prop_Star as Goal Post
1. Select the Prop_Star object in your hierarchy
2. Add the GoalPost script to it:
   - In the Inspector, click "Add Component"
   - Search for "GoalPost" and add it
3. Configure the GoalPost settings:
   - **Next Scene Name**: Leave empty to auto-load next scene, or specify scene name
   - **Transition Delay**: How long to wait before transitioning (default: 1 second)
   - **Rotate Idle**: Makes the star spin (recommended: true)
   - **Rotation Speed**: How fast the star rotates (default: 50)

### Step 3: Add Level Manager (Optional but Recommended)
1. Create an empty GameObject in your scene called "LevelManager"
2. Add the LevelManager script to it
3. Configure settings:
   - **Level Name**: Display name for this level
   - **Time Limit**: Set to 0 for no time limit
   - **Required Coins**: Set to 0 if not requiring coins

### Step 4: Build Settings
1. Go to File > Build Settings
2. Make sure your scenes are added in the correct order:
   - Scene 0: MainMenu
   - Scene 1: MainGame (your current level)
   - Scene 2+: Additional levels you create
3. Click "Add Open Scenes" if your current scene isn't listed

### Step 5: Create Additional Levels
1. Duplicate your MainGame scene
2. Rename it (e.g., "Level2", "Level3", etc.)
3. Modify the level layout
4. Make sure each level has:
   - A Prop_Star with GoalPost script
   - A LevelManager (optional)
   - Player spawn point

## Features

### GoalPost Script Features
- **Automatic Trigger Setup**: Converts any collider to a trigger automatically
- **Visual Rotation**: Makes the star spin for visual appeal
- **Sound & Effects**: Support for particle effects and audio
- **Smart Scene Loading**: Handles both scene names and build indices
- **Requirement Checking**: Can require coins or other conditions

### LevelManager Features
- **Progress Tracking**: Saves level completion and best times
- **Time Limits**: Optional time-based challenges
- **Level Unlocking**: Progressive level unlocking system
- **UI Integration**: Text display for level info

## Testing
1. Play your scene
2. Move your player to touch the Prop_Star
3. You should see "Goal reached!" in the Console
4. The scene should transition after the delay

## Troubleshooting

### Star Not Responding
- Check that your player has the "Player" tag
- Verify the GoalPost script is attached to the Prop_Star
- Check that the star's collider is set as a trigger

### Scene Not Loading
- Verify scenes are added to Build Settings in correct order
- Check Console for error messages
- Make sure next scene exists

### Player Falling Through Star
- The GoalPost script automatically sets the collider as a trigger
- If you need a solid star, add a separate GameObject with a non-trigger collider

## Advanced Customization

### Adding Particle Effects
1. Create or import a particle effect prefab
2. Assign it to the "Goal Effect" field in GoalPost
3. It will play when the goal is reached

### Adding Sound Effects
1. Add an AudioSource component to the Prop_Star
2. Assign an audio clip to it
3. Reference the AudioSource in the "Goal Sound" field

### Requiring Coins
1. Set "Require All Coins" to true in GoalPost
2. This currently shows a debug message
3. You would need to modify CoinCollection.cs to expose coin counts for full functionality

## Next Steps
- Create additional levels by duplicating and modifying scenes
- Add particle effects and sounds for better game feel
- Implement UI elements for level progression
- Consider adding checkpoints or save systems

Happy game developing! 