using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class StringBuilderCache
{
    [ThreadStatic]
    private static StringBuilder _cache = new StringBuilder();
    private const int MAX_BUILDER_SIZE = 512;

    public static StringBuilder Acquire(int capacity = 256)
    {
        StringBuilder cache = StringBuilderCache._cache;
        if (cache == null || cache.Capacity < capacity)
            return new StringBuilder(capacity);
        StringBuilderCache._cache = (StringBuilder)null;
        cache.Length = 0;
        return cache;
    }

    public static string GetStringAndRelease(StringBuilder sb)
    {
        string str = sb.ToString();
        StringBuilderCache.Release(sb);
        return str;
    }

    public static void Release(StringBuilder sb)
    {
        if (sb.Capacity > 512)
            return;
        StringBuilderCache._cache = sb;
    }
}