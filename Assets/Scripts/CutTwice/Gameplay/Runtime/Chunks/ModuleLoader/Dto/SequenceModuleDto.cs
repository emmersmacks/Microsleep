using System;
using Newtonsoft.Json.Linq;

namespace CutTwice.Gameplay.Runtime.Chunks.ModuleLoader.Dto
{
    [Serializable]
    public struct SequenceModuleDto
    {
        public string Name;
        public int Difficulty;
        public SequenceChunkRefDto[] Sequences;
    }
    
    [Serializable]
    public struct SequenceChunkRefDto
    {
        public string Id;
    }
    
    [Serializable]
    public struct SequenceChunkDto
    {
        public string Id;
        public ChunkType ChunkType;
        public SequenceActionDto[] Actions;
    }
    
    [Serializable]
    public struct ChunkListDto
    {
        public ChunkType ChunkType;
        public SequenceChunkDto[] Chunks;
    }

    public enum ChunkType
    {
        Easy,
        Normal,
        Hard
    }

    public enum ActionType
    {
        SpawnTraffic,
        SpawnDeer,
        BackviewObject,
        SideviewObject,
        Delay
    }

    [Serializable]
    public struct SequenceActionDto
    {
        public ActionType Type;
        public JObject Parameters;
    }
}