using System;
using UnityEngine;

namespace Lite
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