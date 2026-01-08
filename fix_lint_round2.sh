#!/bin/bash
# Round 2: Fix remaining array specs
sed -i '7s/new string\[\]/new[]/' src/HorseGame.Shared/Consts.cs
sed -i '13s/new string\[\]/new[]/' src/HorseGame.Shared/Consts.cs

# Fix lambda unused params - change specific lines
sed -i '61s/(t)/(\_)/' src/Horse Game.Shared/GameGenerator.cs
sed -i '34s/(s, e)/(_,__)/' src/HorseGame.Unified/Components/RaceControlPanel.cs  
sed -i '39s/(s, e)/(_ ,__)/' src/HorseGame.Unified/Components/RaceControlPanel.cs
sed -i '36s/(s, e)/(_,__)/' src/HorseGame.Unified/Windows/CluesWindow.cs
sed -i '19s/(o, e)/(_,__)/' src/HorseGame.Unified/Windows/MainMenuWindow.cs
sed -i '29s/(s, e)/(_,__)/' src/HorseGame.Unified/Windows/NewGameDialog.cs

# Fix BettingHistoryPanel field -> local  
sed -i '11s/private ListStore/private readonly ListStore/' src/HorseGame.Unified/Components/BettingHistoryPanel.cs

echo "✅ Round 2 complete"
