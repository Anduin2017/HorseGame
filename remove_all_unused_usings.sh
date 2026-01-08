#!/bin/bash
# Remove first using statement from specified files

# For files with "using System;" at line 1
for file in \
  src/HorseGame. Shared/Consts.cs \
  src/HorseGame.Shared/Clues/Faster.cs \
  src/HorseGame.Shared/Clues/OverTake.cs \
  src/HorseGame.Shared/Clues/Positioning.cs \
  src/HorseGame.Shared/Clues/TotalCountMore.cs \
  src/HorseGame.Shared/Game.cs \
  src/HorseGame.Shared/HorseEvaluator.cs \
  src/HorseGame.Shared/IClue.cs
do
  [ -f "$file" ] && sed -i '1{/^using System;$/d;}' "$file"
done

# For TotalCount.cs - remove first 2 lines
sed -i '1,2d' src/HorseGame.Shared/Clues/TotalCount.cs

# For GameGenerator.cs, SuitableGameGenerator.cs - remove using HorseGame.Shared
sed -i '/^using HorseGame.Shared;$/d' src/HorseGame.Shared/GameGenerator.cs
sed -i '/^using HorseGame.Shared;$/d' src/HorseGame.Shared/SuitableGameGenerator.cs

# For test file  
sed -i '2d' tests/HorseGame.Tests/BettingServiceTests.cs

# For Unified files
sed -i '1,2{/^using /d;}' src/HorseGame.Unified/Services/BettingService.cs
sed -i '1{/^using Gtk;$/d;}' src/HorseGame.Unified/Windows/MainMenuWindow.cs

# For GameRepository - remove using System;
sed -i '/^using System;$/d' src/HorseGame.Unified/Services/GameRepository.cs

echo "✅ Removed all unused usings"
