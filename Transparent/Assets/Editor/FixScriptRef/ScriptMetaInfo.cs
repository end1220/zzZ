using System;
using UnityEngine;

namespace Float
{
    [Serializable]
    public struct ScriptMetaEntry
    {
        public int fileId;
        public string guid;
    }

    public class ScriptMetaInfo : ScriptableObject
    {
        public ScriptMetaEntry[] scriptMetas;
    }
}