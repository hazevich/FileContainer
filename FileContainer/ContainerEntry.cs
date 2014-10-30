using System;

namespace FileContainer
{
    /// <summary>
    /// Model used to create a file container.
    /// </summary>
    [Serializable]
    public class ContainerEntry
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public int Size { get; set; }
        public int Offset { get; set; }
        public int EndOffset { get; set; }

        public override bool Equals(object obj)
        {
            if (this.Name == (obj as ContainerEntry).Name)
                return true;

            return false;
        }
    }
}
