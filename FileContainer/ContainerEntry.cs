using System;

namespace FileContainer
{
    /// <summary>
    /// Entry of the file container.
    /// </summary>
    [Serializable]
    public class ContainerEntry
    {
        /// <summary>
        /// Name of File
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Path to File
        /// </summary>
        public string Path { get; set; }
        
        /// <summary>
        /// Calculated property - size of File
        /// </summary>
        public int Size {get { return EndOffset - Offset; }}
        
        /// <summary>
        /// Start index in Container
        /// </summary>
        public int Offset { get; set; }
        
        /// <summary>
        /// End index in Container
        /// </summary>
        public int EndOffset { get; set; }

        /// <summary>
        /// Overriding Equals method according to Name property
        /// </summary>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            
            return obj.GetType() == this.GetType() 
                && Equals((ContainerEntry) obj);
        }

        protected bool Equals(ContainerEntry other)
        {
            return string.Equals(Name, other.Name);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }
}
