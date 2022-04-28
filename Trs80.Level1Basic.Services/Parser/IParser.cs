using System.Collections.Generic;
using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Services.Parser;

public interface IParser
{
    ParsedLine Parse(List<Token> tokens);
}