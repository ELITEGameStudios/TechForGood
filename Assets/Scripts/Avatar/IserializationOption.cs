using UnityEngine;

public interface IserializationOption
{
    string content_type { get; }
    T Deserialize<T>(string text);
}
