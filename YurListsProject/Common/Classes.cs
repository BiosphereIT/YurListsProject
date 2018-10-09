// Decompiled with JetBrains decompiler
// Type: FinalizeDocument.Common.Classes
// Assembly: NormGroup, Version=1.0.0.0, Culture=neutral, PublicKeyToken=9cfa31019f937ed3
// MVID: 5F82EBC5-F326-4558-8B82-DE6FC143EE5E
// Assembly location: C:\WorkSpace\Soutions\Recover_Colution\normgroup.wsp\NormGroup.dll

using Microsoft.SharePoint;
using System;

namespace FinalizeDocument.Common
{
    public class ApproveTableItem
    {
        public int ID { get; set; }

        public string Workflow { get; set; }

        public string WorkflowID { get; set; }

        public int StatusID { get; set; }

        public string Status { get; set; }

        public SPPrincipal Approver { get; set; }

        public string List { get; set; }

        public string ListUrl { get; set; }
    }
    public class CalculationZP
    {
        public int ID { get; set; }

        public string Title { get; set; }

        public int DepartmentID { get; set; }

        public string Department { get; set; }

        public string TypeZP { get; set; }

        public int UnitID { get; set; }

        public string UnitName { get; set; }

        public int ResponsibleID { get; set; }

        public string Responsible { get; set; }

        public int StatusID { get; set; }

        public string Status { get; set; }

        public string Comment { get; set; }

        public string Month { get; set; }

        public string ID1C { get; set; }
    }
    public class ContactSharepoint
    {
        public int ID { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Company { get; set; }

        public string JobTitle { get; set; }

        public string HomePhone { get; set; }

        public string WorkPhone { get; set; }

        public string CellPhone { get; set; }

        public string WorkFax { get; set; }

        public string WorkAddress { get; set; }

        public string WorkCity { get; set; }

        public string WorkState { get; set; }

        public string WorkZip { get; set; }

        public string WorkCountry { get; set; }

        public string WebPage { get; set; }

        public string Comments { get; set; }

        public string ID1C { get; set; }

        public string FullNameAD { get; set; }

        public string ADLogin { get; set; }

        public string InnerPhone { get; set; }

        public string BirthDay { get; set; }

        public string Department { get; set; }

        public string Fired { get; set; }

        public string Photo { get; set; }

        public string WorkPhone2 { get; set; }

        public string Skype { get; set; }

        public string TabNumber { get; set; }

        public string FunctionalHead { get; set; }

        public string Manager { get; set; }
    }
    internal class DisabledItemEventsScope : SPItemEventReceiver, IDisposable
    {
        public DisabledItemEventsScope()
        {
            this.EventFiringEnabled = false;
        }

        public void Dispose()
        {
            this.EventFiringEnabled = true;
        }
    }
    public class EmployeeItem
    {
        public int empID { get; set; }

        public string EmployeeID { get; set; }

        public string EmployeeName { get; set; }

        public string CalculationTypeID { get; set; }

        public string CalculationType { get; set; }

        public string PayRollID { get; set; }

        public string Total { get; set; }
    }
    public class GroupClass
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public bool IsCurrentUserOwner { get; set; }

        public bool IsMember { get; set; }
    }
    public class ListInfo
    {
        public string ID { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }
    }
    public class SharepointItem
    {
        public int ID { get; set; }

        public string Title { get; set; }

        public string ID1C { get; set; }
    }
    public class User1C
    {
        public string ID1C { get; set; }

        public string FullName { get; set; }
    }
    public class WorkflowInfo
    {
        public string ID { get; set; }

        public string Title { get; set; }

        public string ListID { get; set; }
    }
}
