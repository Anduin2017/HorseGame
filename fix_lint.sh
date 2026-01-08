#!/bin/bash
echo "Fixing lint issues..."

# 1. Fix unused local variable
sed -i '/var horses = new\[\] { "Gryffindor", "Hufflepuff", "Ravenclaw", "Slytherin" };/d' src/HorseGame.Unified/Generators/BetterThanGenerator.cs

# 2. Fix redundant init  
sed -i 's/public int CurrentRound { get; set; } = 0;/public int CurrentRound { get; set; }/' src/HorseGame.Shared/GameSession.cs

# 3. Fix array specs
sed -i 's/new string\[\]/new[]/' src/HorseGame.Shared/Consts.cs
sed -i 's/new double\[\]/new[]/' src/HorseGame.Shared/GameGenerator.cs
sed -i 's/new int\[\]/new[]/' tests/HorseGame.Tests/EvaluatorTests.cs  
sed -i 's/new string\[\]/new[]/' src/HorseGame.Unified/Generators/TotalCountGenerator.cs

# 4. Fix unused params
sed -i 's/(t) => random/(_) => random/' src/HorseGame.Shared/GameGenerator.cs
sed -i 's/.OrderBy(t => random/.OrderBy(_ => random/' src/HorseGame.Unified/Services/ClueService.cs

echo "✅ Done"
