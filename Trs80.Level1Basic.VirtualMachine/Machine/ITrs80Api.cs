namespace Trs80.Level1Basic.VirtualMachine.Machine;

public interface ITrs80Api
{
    int Int(dynamic value);
    dynamic Mem();
    dynamic Abs(dynamic value);
    dynamic Chr(dynamic value);
    string Left(string value, int length);
    string Right(string value, int length);
    string Mid(string value, int start, int length);
    dynamic Rnd(int control);
    string Tab(dynamic value);
    string PadQuadrant();
    object Set(float x, float y);

    object Reset(float x, float y);

    int Point(int x, int y);


}