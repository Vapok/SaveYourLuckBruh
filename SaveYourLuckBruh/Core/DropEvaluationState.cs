using System.Collections.Generic;

namespace SaveYourLuckBruh.Core;

internal class DropEvaluationState
{
    public Character Attacker;
    public Dictionary<int, int> InitialCounters;

    public DropEvaluationState(Character attacker, Dictionary<int, int> initialCounters)
    {
        Attacker = attacker;
        InitialCounters = initialCounters;
    }
}
