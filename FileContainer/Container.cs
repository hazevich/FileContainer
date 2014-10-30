using System;
using System.Collections.Generic;

namespace FileContainer
{
    [Serializable]
    public class Container
    {
        public List<ContainerEntry> ContainerEntries { get; set; }
    }
}
