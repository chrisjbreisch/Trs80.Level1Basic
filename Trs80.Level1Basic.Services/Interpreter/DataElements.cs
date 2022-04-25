using System;
using System.Collections.Generic;

namespace Trs80.Level1Basic.Services.Interpreter
{
    public class DataElements
    {
        private readonly List<dynamic> _dataElements = new List<dynamic>();
        private int _listIndex;

        public void Add(dynamic value)
        {
            _dataElements.Add(value);
        }

        public void MoveFirst()
        {
            _listIndex = 0;
        }

        public dynamic GetNext()
        {
            return _listIndex < _dataElements.Count
                ? _dataElements[_listIndex++]
                : throw new IndexOutOfRangeException();
        }

        public void Clear()
        {
            _dataElements.Clear();
        }
    }
}
