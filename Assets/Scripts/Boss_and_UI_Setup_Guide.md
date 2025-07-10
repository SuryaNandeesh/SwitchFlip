# Level Text & Boss Battle Setup Guide

## 🎯 Part 1: Level Text Display Setup

### Step 1: Create the UI Canvas
1. **Create UI Canvas**:
   - Right-click in hierarchy → UI → Canvas
   - Name it "Game UI"
   - Set Canvas Scaler to "Scale With Screen Size"
   - Reference Resolution: 1920x1080

### Step 2: Create Level Text
1. **Create Level Text**:
   - Right-click on Canvas → UI → Text - TextMeshPro
   - Name it "Level Text"
   - Position in top-left corner
   - Set text to "Level 1" as placeholder
   - Font size: 36, Color: White

2. **Create Completion Text**:
   - Right-click on Canvas → UI → Text - TextMeshPro
   - Name it "Completion Text"
   - Position in center of screen
   - Set text to "Level Complete!" as placeholder
   - Font size: 48, Color: Yellow
   - **Disable this GameObject initially**

### Step 3: Set Up UIManager
1. **Create UIManager Object**:
   - Right-click in hierarchy → Create Empty
   - Name it "UIManager"
   - Add Component → UIManager script

2. **Assign UI References**:
   ```
   Level Display:
   - Level Text: Drag "Level Text" object here
   - Completion Text: Drag "Completion Text" object here
   
   Boss Battle UI: (Leave empty for now)
   - Boss Health Text: (We'll set this up later)
   - Boss Health Slider: (We'll set this up later)
   - Boss Battle Panel: (We'll set this up later)
   
   General UI:
   - Main Canvas Group: Add CanvasGroup component to Canvas, then drag Canvas here
   ```

### Step 4: Update LevelManager
1. **Select your LevelManager object**
2. **Configure settings**:
   ```
   Level Settings:
   - Level Name: "Level 1" (or whatever you want)
   - Time Limit: 0 (no time limit)
   
   Boss Battle:
   - Is Boss Level: ✓ (check this for boss levels)
   - Hide Goal Until Boss Defeated: ✓ (recommended)
   ```

---

## 🐉 Part 2: Boss Battle Setup

### Step 1: Create the Boss Object
1. **Create Boss**:
   - Create a 3D object (Cube, Capsule, or import your boss model)
   - Name it "Boss"
   - Scale it to be larger than the player (e.g., 2x2x2)
   - Add a Rigidbody component
   - Add a Collider (set as Trigger)

2. **Add Boss Script**:
   - Add Component → PlatformerBoss script

### Step 2: Configure Boss Settings
```
Boss Stats:
- Max Health: 3
- Current Health: (auto-set)

Movement Settings:
- Move Speed: 2
- Jump Force: 8
- Movement Range: 5

Attack Settings:
- Attack Cooldown: 3
- Knockback Force: 10
- Attack Damage: 1

Damage Settings:
- Invulnerability Time: 2
- Hit Knockback: 5

Visual Effects: (Optional)
- Hit Effect: (drag particle effect prefab)
- Death Effect: (drag particle effect prefab)
- Invulnerable Material: (create a flashing/transparent material)

Audio: (Optional)
- Hit Sound: (add AudioSource with hit sound)
- Death Sound: (add AudioSource with death sound)
- Attack Sound: (add AudioSource with attack sound)
```

### Step 3: Create Boss UI (Optional but Recommended)
1. **Create Boss Health Bar**:
   - Right-click on Canvas → UI → Slider
   - Name it "Boss Health Bar"
   - Position at top-center of screen
   - Set Max Value to 3, Value to 3
   - Style as desired (red fill, etc.)

2. **Create Boss Health Text**:
   - Right-click on Canvas → UI → Text - TextMeshPro
   - Name it "Boss Health Text"
   - Position below health bar
   - Set text to "Boss Health: 3/3"

3. **Create Boss Battle Panel**:
   - Right-click on Canvas → UI → Panel
   - Name it "Boss Battle Panel"
   - Make it a container for boss UI elements
   - Move health bar and text inside this panel
   - **Disable this GameObject initially**

4. **Update UIManager References**:
   ```
   Boss Battle UI:
   - Boss Health Text: Drag "Boss Health Text" here
   - Boss Health Slider: Drag "Boss Health Bar" here
   - Boss Battle Panel: Drag "Boss Battle Panel" here
   ```

### Step 4: Set Up Level as Boss Level
1. **Configure LevelManager**:
   - Is Boss Level: ✓ (checked)
   - Hide Goal Until Boss Defeated: ✓ (checked)

2. **Position Goal Post**:
   - Move your Prop_Star to where you want it after boss defeat
   - The system will automatically hide it until boss is defeated

### Step 5: Test the Boss Battle
1. **Play the scene**
2. **Expected behavior**:
   - Boss UI appears when scene starts
   - Boss moves back and forth in its range
   - Boss attacks periodically when player is near
   - Jump on boss to damage it (should see "Boss took damage!" in Console)
   - After 3 hits, boss dies and goal post appears
   - Health bar updates with each hit

---

## 🎮 Boss Battle Mechanics

### How to Damage the Boss
- **Jump on the boss** - Player must be above the boss when collision occurs
- **Boss becomes invulnerable** for 2 seconds after taking damage
- **Visual feedback** - Boss changes material/color during invulnerability
- **Knockback** - Boss gets knocked back when hit

### Boss Attack Patterns
- **Movement** - Boss moves back and forth in its range
- **Jump Attack** - Boss jumps when player is nearby
- **Knockback** - If player touches boss from the side, player gets knocked back

### Win Condition
- **3 hits** defeats the boss
- **Goal post appears** automatically after boss defeat
- **"Boss Defeated!"** message shows
- **Level completion** works normally after touching goal post

---

## 🛠️ Troubleshooting

### Level Text Not Showing
- ✅ Check UIManager has Level Text assigned
- ✅ Check Canvas is set to Screen Space - Overlay
- ✅ Check Text object is active and visible

### Boss Not Taking Damage
- ✅ Player has "Player" tag
- ✅ Boss collider is set as Trigger
- ✅ Player is jumping on boss from above
- ✅ Check Console for "Boss took damage!" messages

### Boss UI Not Appearing
- ✅ UIManager has boss UI references assigned
- ✅ Boss Battle Panel is initially disabled
- ✅ PlatformerBoss script is attached to boss

### Goal Post Not Appearing After Boss Defeat
- ✅ LevelManager is set as Boss Level
- ✅ Goal Post exists in scene
- ✅ Hide Goal Until Boss Defeated is checked

### Player Can't Damage Boss
- ✅ Player must jump ON TOP of boss
- ✅ Boss can't be invulnerable (check timing)
- ✅ Boss collider must be a Trigger

---

## 🎯 Advanced Features

### Adding Boss Animations
If you have an Animator for your boss:
1. Create animation parameters:
   - `isWalking` (Bool)
   - `isAttacking` (Trigger)
   - `isHit` (Trigger)
   - `isDead` (Trigger)

2. The PlatformerBoss script will automatically use these if they exist

### Adding Visual Effects
1. **Hit Effect**: Create particle system for when boss takes damage
2. **Death Effect**: Create explosion or similar for boss death
3. **Invulnerable Material**: Create flashing/transparent material

### Adding Sound Effects
1. Add AudioSource components to boss
2. Assign audio clips for hit, death, and attack sounds
3. Reference AudioSources in PlatformerBoss script

### Multiple Boss Phases
You can extend the boss script to add phases:
- Different attack patterns at different health levels
- Speed increases as health decreases
- New abilities when boss reaches low health

---

## 📝 Testing Checklist

### Level Text System
- [ ] Level name displays correctly
- [ ] Completion message shows when level ends
- [ ] Text updates automatically

### Boss Battle System
- [ ] Boss moves back and forth
- [ ] Boss attacks when player is near
- [ ] Player can damage boss by jumping on it
- [ ] Boss becomes invulnerable after taking damage
- [ ] Boss health UI updates correctly
- [ ] Goal post appears after boss defeat
- [ ] Level completes normally after boss defeat

## 🎉 You're All Set!

Your game now has:
- **Level text display** that shows current level
- **Complete boss battle system** with 3-hit mechanic
- **Boss UI** with health bar and feedback
- **Integrated progression** where boss defeat reveals goal post

The boss battle provides a classic platformer experience where players must jump on the boss to damage it, just like in Mario games!

Happy boss fighting! 🐉⚔️ 