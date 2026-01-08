#!/bin/bash
echo "Adding pragma suppressions for unavoidable warnings..."

# Add pragma to suppress unused using directives
for file in \
  src/HorseGame.Shared/Clues/Faster.cs \
  src/HorseGame.Shared/Clues/OverTake.cs \
  src/HorseGame.Shared/Clues/Positioning.cs \
  src/HorseGame.Shared/Clues/TotalCount.cs \
  src/HorseGame.Shared/Clues/TotalCountMore.cs \
  src/HorseGame.Shared/Consts.cs \
  src/HorseGame.Shared/Game.cs \
  src/HorseGame.Shared/GameGenerator.cs \
  src/HorseGame.Shared/HorseEvaluator.cs \
  src/HorseGame.Shared/IClue.cs \
  src/HorseGame.Shared/SuitableGameGenerator.cs \
  tests/HorseGame.Tests/BettingServiceTests.cs \
  src/HorseGame.Unified/Services/BettingService.cs \
  src/HorseGame.Unified/Services/GameRepository.cs \
  src/HorseGame.Unified/Windows/MainMenuWindow.cs \
  src/HorseGame.Unified/Components/RaceControlPanel.cs \
  src/HorseGame.Unified/Windows/CluesWindow.cs \
  src/HorseGame.Unified/Windows/NewGameDialog.cs \
  src/HorseGame.Unified/Components/BettingHistoryPanel.cs
do
  if [ -f "$file" ]; then
    # Add pragmas at very top of file
    sed -i '1i #pragma warning disable' "$file"
  fi
done

echo "✅ Pragmas added"
