//
//
// This file was automatically generated by generateAst
// at 2022-06-26 3:23:04 PM UTC. Do not modify.
//
//
namespace Trs80.Level1Basic.VirtualMachine.Parser.Statements;

public interface IVisitor<out T>
{
    T VisitClsStatement(Cls statement);
    T VisitCompoundStatement(Compound statement);
    T VisitContStatement(Cont statement);
    T VisitDataStatement(Data statement);
    T VisitDeleteStatement(Delete statement);
    T VisitEndStatement(End statement);
    T VisitForStatement(For statement);
    T VisitGosubStatement(Gosub statement);
    T VisitGotoStatement(Goto statement);
    T VisitIfStatement(If statement);
    T VisitInputStatement(Input statement);
    T VisitLetStatement(Let statement);
    T VisitListStatement(List statement);
    T VisitLoadStatement(Load statement);
    T VisitMergeStatement(Merge statement);
    T VisitNewStatement(New statement);
    T VisitNextStatement(Next statement);
    T VisitOnStatement(On statement);
    T VisitPrintStatement(Print statement);
    T VisitReplaceStatement(Replace statement);
    T VisitReadStatement(Read statement);
    T VisitRemStatement(Rem statement);
    T VisitRestoreStatement(Restore statement);
    T VisitReturnStatement(Return statement);
    T VisitRunStatement(Run statement);
    T VisitSaveStatement(Save statement);
    T VisitStatementExpressionStatement(StatementExpression statement);
    T VisitStopStatement(Stop statement);
}
