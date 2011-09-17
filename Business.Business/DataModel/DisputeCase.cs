using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace LePont.Business
{
    /* Note: 
     * If this class is named "Case", that will cause an HQL parsing error "NHibernate.Antlr.Runtime.NoViableAltException",
     * when a query contains the "where" clause.
     */ 
    public class DisputeCase : DeactivatableEntity
    {
        public virtual string Title { get; set; }
        public virtual string Locality { get; set; }
        public virtual Department Department { get; set; }
        public virtual CaseType InternalCaseType { get; set; }
        public virtual CaseType ExternalCaseType { get; set; }
        public virtual string Content { get; set; }
        public virtual decimal? MoneyInvolved { get; set; }
        public virtual short? PeopleInvolved { get; set; }
        public virtual bool? Flag1 { get; set; }
        public virtual bool? Flag2 { get; set; }
        public virtual bool? Flag3 { get; set; }
        public virtual bool? Flag4 { get; set; }
        public virtual bool? Flag5 { get; set; }
        public virtual bool? Flag6 { get; set; }
        public virtual bool? Flag7 { get; set; }
        public virtual bool? Flag8 { get; set; }
        public virtual bool? Flag9 { get; set; }
        public virtual bool? Flag10 { get; set; }
        public virtual byte? Status { get; set; }
        public virtual RelationType PartiesRelationType { get; set; }
        public virtual string MediatorAdvice { get; set; }
        public virtual string Instructions { get; set; }
        public virtual string Progress { get; set; }
        public virtual string Disposal { get; set; }
        public virtual string Responsable { get; set; }
        public virtual string ResponsablePhone { get; set; }
        public virtual bool? IsConcluded { get; set; }
        public virtual DateTime? ConcludeDate { get; set; }
        public virtual User Registrar { get; set; }
        public virtual DateTime? DateTime { get; set; }
        public virtual DateTime? LastModifyTime { get; set; }
    }

    public class CaseType : DeactivatableEntity
    {
        public virtual string Name { get; set; }
        public virtual string Domain { get; set; }
        public virtual string Description { get; set; }
        public virtual short? ListOrder { get; set; }

    }

    public class RelationType : DeactivatableEntity
    {
        public virtual string Name { get; set; }
        public virtual short? ListOrder { get; set; }
    }
}