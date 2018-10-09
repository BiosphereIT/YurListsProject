// Decompiled with JetBrains decompiler
// Type: EmployeeList.Common.UpdateNormFrom1C
// Assembly: NormGroup, Version=1.0.0.0, Culture=neutral, PublicKeyToken=9cfa31019f937ed3
// MVID: 5F82EBC5-F326-4558-8B82-DE6FC143EE5E
// Assembly location: C:\WorkSpace\Soutions\Recover_Colution\normgroup.wsp\NormGroup.dll

using FinalizeDocument.Common;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System;

namespace EmployeeList.Common
{
  internal class UpdateYurDepartmentsFrom1C : SPJobDefinition
  {
    public UpdateYurDepartmentsFrom1C()
    {
    }

    public UpdateYurDepartmentsFrom1C(string jobName, SPService service, SPServer server, SPJobLockType targetType)
      : base(jobName, service, server, targetType)
    {
    }

    public UpdateYurDepartmentsFrom1C(string jobName, SPWebApplication webApplication)
      : base(jobName, webApplication, (SPServer) null, SPJobLockType.ContentDatabase)
    {
      this.Title = "Update Yur Departments From1C";
    }

    public override void Execute(Guid contentDbId)
    {
      try
      {
        SPSecurity.RunWithElevatedPrivileges((SPSecurity.CodeToRunElevated) (() =>
        {
          for (int index = 0; index < this.WebApplication.Sites.Count; ++index)
          {
            using (SPSite site = this.WebApplication.Sites[index])
            {
              foreach (SPWeb allWeb in (SPBaseCollection) site.AllWebs)
              {
                SPList spList = (SPList) null;
                try
                {
                  spList = allWeb.GetList("Lists/YurDepatmentList");
                }
                catch (Exception ex)
                {
                }
                if (spList != null)
                  BusinessLogic.Fill1CList(site.Url.ToString(), allWeb.ServerRelativeUrl.ToString(), "Lists/YurDepatmentList", "ПодразделенияОрганизаций");
              }
            }
          }
        }));
      }
      catch (Exception ex)
      {
      }
    }

    protected override bool HasAdditionalUpdateAccess()
    {
      return true;
    }
  }
}
