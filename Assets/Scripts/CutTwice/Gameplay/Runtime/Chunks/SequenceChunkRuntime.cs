using System.Collections.Generic;
using CutTwice.Gameplay.Runtime.Chunks.Actions;

namespace CutTwice.Gameplay.Runtime.Chunks
{
    public class SequenceChunkRuntime
    {
        public string Name;
        public List<ISequenceActionRuntime> Actions = new();
    }
}