using System;
using System.Collections.Generic;

namespace SaveYourLuckBruh.Core;

internal class DropEvaluationState
{
    public Character Attacker;
    public Dictionary<string, Tuple<float, int>> InitialCounters;

    public DropEvaluationState(Character attacker, Dictionary<string, Tuple<float, int>> initialCounters)
    {
        Attacker = attacker;
        InitialCounters = initialCounters;
    }
}
