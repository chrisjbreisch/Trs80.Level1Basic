using System.Collections.Generic;
using Trs80.Level1Basic.Domain;

namespace Trs80.Level1Basic.Services.Parser.Statement
{
    public interface IStatementVisitor
    {
        void VisitClsStatement(Cls root);
        void VisitDeleteStatement(Delete root);
        void VisitEndStatement(End root);
        void VisitForStatement(For root);
        void VisitGotoStatement(Goto root);
        void VisitIfStatement(If root);
        void VisitInputStatement(Input root);
        void VisitLetStatement(Let root);
        void VisitListStatement(List root);
        void VisitLoadStatement(Load root);
        void VisitNewStatement(New root);
        void VisitNextStatement(Next root);
        void VisitPrintStatement(Print root);
        void VisitRemStatement(Rem root);
        void VisitRunStatement(Run root);
        void VisitSaveStatement(Save root);
        void VisitStatementExpressionStatement(StatementExpression root);
    }

    public abstract class Statement
    {
        public int LineNumber { get; set; }
        public string SourceLine { get; set; }

        public abstract void Accept(IStatementVisitor visitor);
    }

















}
