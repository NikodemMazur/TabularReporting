using System;
using System.Collections.Generic;

namespace TabularReporting.Abstractions
{
    public interface IBranchingSourcedColumnQuery<T> : ISourcedColumnQuery<T>
    {
        IEnumerable<T> Branch { get; }
    }
}