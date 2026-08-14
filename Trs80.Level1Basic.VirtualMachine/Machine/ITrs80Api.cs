namespace Trs80.Level1Basic.VirtualMachine.Machine;

public interface ITrs80Api
{
    int Int(dynamic value);
    dynamic Mem();
    dynamic Abs(dynamic value);
    dynamic Chr(dynamic value);
    int Asc(dynamic value);
    int Len(dynamic value);
    int InStr(string source, string match);
    float Val(string value);
    string Str(dynamic value);
    int CInt(dynamic value);
    int Fix(dynamic value);
    double CDbl(dynamic value);
    float CSng(dynamic value);
    string LCase(string value);
    string UCase(string value);
    string Trim(string value);
    string LTrim(string value);
    string RTrim(string value);
    int Peek(int address);
    void Poke(int address, int value);
    int Pos(int position);
    int CsrLin();
    string InKey();
    string InputString(int length);
    string Date();
    string Time();
    int Sgn(dynamic value);
    string Hex(dynamic value);
    string Oct(dynamic value);
    string String(int count, string value);
    string Space(int length);
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