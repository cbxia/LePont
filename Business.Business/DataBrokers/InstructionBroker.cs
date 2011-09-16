using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JasminSoft.NHibernateUtils;
using NHibernate;

namespace LePont.Business
{
    public class InstructionBroker : BaseGenericDataBroker<Instruction, int>
    {
        public Instruction GetById(int id, Department dep)
        {
            string queryString = "from Instruction where ID = :id and (TargetCase.Department.ID = :dep_id or TargetCase.Department.Code like :code_pattern)";
            Instruction result = PerformUniqueQueryAction<Instruction>(queryString, query =>
            {
                query
                    .SetInt32("id", id)
                    .SetInt32("dep_id", dep.ID)
                    .SetString("code_pattern", string.Format("{0}%", dep.Code));
                return query;
            });
            return result;
        }

        // TODO: Currently 1-1 relationship is effectively assumed btw Case and Instruction. See if this is correct.
        // public Instruction[] GetByCaseId(int caseId, Department dep) {}
        public Instruction GetByCase(int caseId, Department dep)
        {
            string queryString = "from Instruction where TargetCase.ID = :case_id and (TargetCase.Department.ID = :dep_id or TargetCase.Department.Code like :code_pattern)";
            Instruction result = PerformUniqueQueryAction<Instruction>(queryString, query =>
            {
                query
                    .SetInt32("case_id", caseId)
                    .SetInt32("dep_id", dep.ID)
                    .SetString("code_pattern", string.Format("{0}%", dep.Code))
                    .SetMaxResults(1);
                return query;
            });
            return result;
        }
    }
}
