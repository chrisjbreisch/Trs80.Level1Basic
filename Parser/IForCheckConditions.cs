using System.Collections.Generic;

namespace Trs80.Level1Basic.Parser
{
    public interface IForCheckConditions : IReadOnlyCollection<ForCheckCondition>

    {
        ForCheckCondition Peek();
        ForCheckCondition Pop();
        void Push(ForCheckCondition item);
        void Remove(ForCheckCondition item);
    }
}