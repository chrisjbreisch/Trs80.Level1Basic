//
// This file is automatically generated. Do not modify.
//
namespace Trs80.Level1Basic.Interpreter.Parser.Statements;

public interface IStatementVisitor
{
    void VisitClsStatement(Cls root);
    void VisitContStatement(Cont root);
    void VisitDataStatement(Data root);
    void VisitDeleteStatement(Delete root);
    void VisitEndStatement(End root);
    void VisitForStatement(For root);
    void VisitGotoStatement(Goto root);
    void VisitGosubStatement(Gosub gosubStatement);
    void VisitIfStatement(If root);
    void VisitInputStatement(Input root);
    void VisitLetStatement(Let root);
    void VisitListStatement(List root);
    void VisitLoadStatement(Load root);
    void VisitMergeStatement(Merge root);
    void VisitNewStatement(New root);
    void VisitNextStatement(Next root);
    void VisitOnStatement(On root);
    void VisitPrintStatement(Print root);
    void VisitReplaceStatement(Replace root);
    void VisitReadStatement(Read root);
    void VisitRemStatement(Rem root);
    void VisitRestoreStatement(Restore root);
    void VisitReturnStatement(Return root);
    void VisitRunStatement(Run root);
    void VisitSaveStatement(Save root);
    void VisitStatementExpressionStatement(StatementExpression root);
    void VisitStopStatement(Stop root);
}