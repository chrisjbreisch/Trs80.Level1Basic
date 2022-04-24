using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Trs80.Level1Basic.Parser
{
    public class ForCheckConditions : IForCheckConditions
    {
        private readonly List<ForCheckCondition> _checkConditions = new List<ForCheckCondition>();
        public IEnumerator<ForCheckCondition> GetEnumerator()
        {
            for (int i = _checkConditions.Count - 1; i >= 0; i--)
                yield return _checkConditions[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _checkConditions).GetEnumerator();
        }

        public int Count => _checkConditions.Count;
        public ForCheckCondition Peek()
        {
            return _checkConditions.Last();
        }

        public ForCheckCondition Pop()
        {
            var item = Peek();
            _checkConditions.Remove(item);
            return item;
        }

        public void Push(ForCheckCondition item)
        {
            _checkConditions.Add(item);
        }

        public void Remove(ForCheckCondition item)
        {
            if (_checkConditions.Contains(item))
                _checkConditions.Remove(item);

        }
    }
}
