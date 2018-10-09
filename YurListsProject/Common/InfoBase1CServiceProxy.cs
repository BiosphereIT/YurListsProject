// Decompiled with JetBrains decompiler
// Type: ExpImp1C.Codes.InfoBase1CServiceProxy
// Assembly: NormGroup, Version=1.0.0.0, Culture=neutral, PublicKeyToken=9cfa31019f937ed3
// MVID: 5F82EBC5-F326-4558-8B82-DE6FC143EE5E
// Assembly location: C:\WorkSpace\Soutions\Recover_Colution\normgroup.wsp\NormGroup.dll

using FinalizeDocument.Common;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using YurListsProject.Service1C;

namespace ExpImp1C.Codes
{
  internal class InfoBase1CServiceProxy : IDisposable
  {
    private WebSharepointPortTypeClient proxy;

    public InfoBase1CServiceProxy(string siteID)
    {
      BasicHttpBinding basicHttpBinding = new BasicHttpBinding();
      basicHttpBinding.MaxBufferSize = 20000000;
      basicHttpBinding.MaxReceivedMessageSize = 20000000L;
      basicHttpBinding.MaxBufferPoolSize = 20000000L;
      basicHttpBinding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
      basicHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
      EndpointAddress remoteAddress = new EndpointAddress(BusinessLogic.WebKey_Get("1C_Uri", siteID));
      this.proxy = new WebSharepointPortTypeClient((Binding) basicHttpBinding, remoteAddress);
      this.proxy.ClientCredentials.UserName.UserName = BusinessLogic.WebKey_Get("1C_UserName", siteID);
      this.proxy.ClientCredentials.UserName.Password = BusinessLogic.WebKey_Get("1C_Password", siteID);
      this.proxy.Open();
    }

    public List<DeptString> GetDebt(DeptString term)
    {
      List<DeptString> deptStringList1 = new List<DeptString>();
      List<DeptString> deptStringList2;
      try
      {
        deptStringList2 = term == null || string.IsNullOrEmpty(term.SprName) ? (List<DeptString>) null : (List<DeptString>) this.proxy.GetDept(term);
      }
      catch (FaultException ex)
      {
        deptStringList2 = new List<DeptString>()
        {
          new DeptString()
          {
            Code = ex.Message,
            Name = ex.Message,
            SprName = ex.Message
          }
        };
      }
      catch (CommunicationException ex)
      {
        deptStringList2 = new List<DeptString>()
        {
          new DeptString()
          {
            Code = ex.Message,
            Name = ex.Message,
            SprName = ex.Message
          }
        };
      }
      catch (Exception ex)
      {
        deptStringList2 = new List<DeptString>()
        {
          new DeptString()
          {
            Code = ex.Message,
            Name = ex.Message,
            SprName = ex.Message
          }
        };
      }
      return deptStringList2;
    }

    public List<DeptString> SelectTypeCalculationKinds()
    {
      List<DeptString> deptStringList1 = new List<DeptString>();
      List<DeptString> deptStringList2;
      try
      {
        deptStringList2 = (List<DeptString>) this.proxy.SelectTypeCalculationKinds();
      }
      catch (Exception ex)
      {
        deptStringList2 = new List<DeptString>()
        {
          new DeptString()
          {
            Code = ex.Message,
            Name = ex.Message,
            SprName = ex.Message
          }
        };
      }
      return deptStringList2;
    }

    public List<OrgUser> SelectOrgUsersByDepartment(string term)
    {
      List<OrgUser> orgUserList1 = new List<OrgUser>();
      List<OrgUser> orgUserList2;
      try
      {
        orgUserList2 = (List<OrgUser>) this.proxy.SelectOrgUsersByDepartment(term, DateTime.Now.ToString("dd.MM.yyyy"));
      }
      catch (Exception ex)
      {
        orgUserList2 = new List<OrgUser>()
        {
          new OrgUser()
          {
            BirthDay = ex.Message,
            BirthPlace = ex.Message,
            Comment = ex.Message,
            Department = ex.Message,
            ID = ex.Message,
            INN = ex.Message,
            Name = ex.Message,
            Region = ex.Message,
            Responsible = ex.Message,
            Sex = ex.Message
          }
        };
      }
      return orgUserList2;
    }

    public List<DeptString> SelectTypeRetentionKinds()
    {
      List<DeptString> deptStringList1 = new List<DeptString>();
      List<DeptString> deptStringList2;
      try
      {
        deptStringList2 = (List<DeptString>) this.proxy.SelectTypeRetentionKinds();
      }
      catch (Exception ex)
      {
        deptStringList2 = new List<DeptString>()
        {
          new DeptString()
          {
            Code = ex.Message,
            Name = ex.Message,
            SprName = ex.Message
          }
        };
      }
      return deptStringList2;
    }

    public string ZP_ID1C(ZPTitleParam title, ZPTableTable table)
    {
      string str;
      try
      {
        str = this.proxy.ImportZPTable(title, table);
      }
      catch (Exception ex)
      {
        str = ex.Message;
      }
      return str;
    }

    public List<DeptString> Check_ZP_ID1C(ZPTitleParam title, ZPTableTable table)
    {
      List<DeptString> deptStringList = new List<DeptString>();
      try
      {
        deptStringList = (List<DeptString>) this.proxy.GetBonusOverlimitedEmployeesTable(title, table);
      }
      catch (Exception ex)
      {
        deptStringList.Add(new DeptString()
        {
          Code = ex.Message,
          Name = ex.Message,
          SprName = ex.Message
        });
      }
      return deptStringList;
    }

    public void Dispose()
    {
      if (this.proxy == null)
        return;
      this.proxy.Close();
     // this.proxy.Dispose();
    }
  }
}
