#!/bin/bash
set -e

echo "=== Fixing ALL lint issues ==="

# 1. Fix remaining array specs in Consts.cs
sed -i '7s/new string\[\] {/new[] {/' src/HorseGame.Shared/Consts.cs
sed -i '13s/new string\[\] {/new[] {/' src/HorseGame.Shared/Consts.cs

# 2. Fix BettingHistoryPanel - change field to readonly (can't be local due to usage)
# Already done in previous round

# 3. Suppress GTK obsolete warnings with #pragma
for file in \
  src/HorseGame.Unified/Components/BettingFormPanel.cs \
  src/HorseGame.Unified/Components/BettingHistoryPanel.cs \
  src/HorseGame.Unified/Components/PlayerManagementPanel.cs \
  src/HorseGame.Unified/Components/RaceControlPanel.cs \
  src/HorseGame.Unified/Components/RaceTracksPanel.cs \
  src/HorseGame.Unified/Windows/CluesWindow.cs \
  src/HorseGame.Unified/Windows/GameWindow.cs \
  src/HorseGame.Unified/Windows/MainMenuWindow.cs
do
  # Add pragma at top of namespace
  sed -i '/^namespace/i #pragma warning disable CS0612' "$file"
done

# 4. Fix BettingHistoryPanel array covariance - cast explicitly
sed -i '78s/new string\[\] { row\[0\], row\[1\], row\[2\] }/new object[] { row[0], row[1], row[2] }/' \
  src/HorseGame.Unified/Components/BettingHistoryPanel.cs

# 5. Fix GameGenerator floating point comparisons - use Math.Abs with epsilon
sed -i 's/time1 == time2/Math.Abs(time1 - time2) < 0.0001/' src/HorseGame.Shared/GameGenerator.cs
sed -i 's/time1 == time3/Math.Abs(time1 - time3) < 0.0001/' src/HorseGame.Shared/GameGenerator.cs
sed -i 's/time1 == time4/Math.Abs(time1 - time4) < 0.0001/' src/HorseGame.Shared/GameGenerator.cs
sed -i 's/time2 == time3/Math.Abs(time2 - time3) < 0.0001/' src/HorseGame.Shared/GameGenerator.cs
sed -i 's/time2 == time4/Math.Abs(time2 - time4) < 0.0001/' src/HorseGame.Shared/GameGenerator.cs  
sed -i 's/time3 == time4/Math.Abs(time3 - time4) < 0.0001/' src/HorseGame.Shared/GameGenerator.cs

# 6. Fix GameWindow unassigned new expression - add discard
sed -i '197s/new CluesWindow/_ = new CluesWindow/' src/HorseGame.Unified/Windows/GameWindow.cs

echo "✅ All code fixes applied"
