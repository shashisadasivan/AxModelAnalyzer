//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AxModel.Data2
{
    using System;
    using System.Collections.Generic;
    
    public partial class ModelElement
    {
        public int ElementType { get; set; }
        public int RootHandle { get; set; }
        public int ParentHandle { get; set; }
        public int ElementHandle { get; set; }
        public string Name { get; set; }
        public int AxId { get; set; }
        public Nullable<int> ParentId { get; set; }
        public Nullable<System.Guid> Origin { get; set; }
        public int PartOfInheritance { get; set; }
    }
}
