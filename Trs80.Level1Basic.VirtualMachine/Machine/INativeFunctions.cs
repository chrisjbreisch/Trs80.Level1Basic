using System.Collections.Generic;

namespace Trs80.Level1Basic.VirtualMachine.Machine;

public interface INativeFunctions
{
    List<Callable> Get(string name);
}