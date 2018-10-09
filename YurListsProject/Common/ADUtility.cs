// Decompiled with JetBrains decompiler
// Type: Definitions.ADUtility
// Assembly: NormGroup, Version=1.0.0.0, Culture=neutral, PublicKeyToken=9cfa31019f937ed3
// MVID: 5F82EBC5-F326-4558-8B82-DE6FC143EE5E
// Assembly location: C:\WorkSpace\Soutions\Recover_Colution\normgroup.wsp\NormGroup.dll

using FinalizeDocument.Common;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;

namespace Definitions
{
  public class ADUtility
  {
    public static List<ADUtility.ADGroup> GetAllDepartments(string site_id)
    {
      try
      {
        List<ADUtility.ADGroup> adGroupList = new List<ADUtility.ADGroup>();
        using (DirectoryEntry searchRoot = new DirectoryEntry(BusinessLogic.WebKey_Get("AD_Path", site_id), BusinessLogic.WebKey_Get("AD_Login", site_id), BusinessLogic.WebKey_Get("AD_Password", site_id)))
        {
          using (DirectorySearcher directorySearcher = new DirectorySearcher(searchRoot, "(&(objectClass=organizationalUnit))", new string[2]
          {
            "objectguid",
            "name"
          }))
          {
            directorySearcher.PageSize = int.Parse(BusinessLogic.WebKey_Get("AD_Size", site_id));
            using (SearchResultCollection all = directorySearcher.FindAll())
            {
              foreach (SearchResult searchResult in all)
                adGroupList.Add(new ADUtility.ADGroup()
                {
                  ID = searchResult.Properties.Contains("objectguid") ? BitConverter.ToString((byte[]) searchResult.Properties["objectguid"][0]).Replace("-", string.Empty) : string.Empty,
                  Title = searchResult.Properties.Contains("name") ? (string) searchResult.Properties["name"][0] : string.Empty
                });
              return adGroupList;
            }
          }
        }
      }
      catch (Exception ex)
      {
        return new List<ADUtility.ADGroup>()
        {
          new ADUtility.ADGroup() { Title = ex.Message }
        };
      }
    }

    public static ADUtility.ADUsers GetUserByName(string login, string site_id)
    {
      ADUtility.ADUsers adUsers = new ADUtility.ADUsers();
      try
      {
        using (DirectoryEntry searchRoot = new DirectoryEntry(BusinessLogic.WebKey_Get("AD_Path", site_id), BusinessLogic.WebKey_Get("AD_Login", site_id), BusinessLogic.WebKey_Get("AD_Password", site_id)))
        {
          using (DirectorySearcher directorySearcher = new DirectorySearcher(searchRoot, string.Format("(&(objectCategory=person)(objectClass=user)(sAMAccountName={0}))", (object) login.Trim()), new string[8]
          {
            "objectguid",
            "samaccountname",
            "mail",
            "displayname",
            "name",
            "givenName",
            "sn",
            "manager"
          }))
          {
            directorySearcher.PageSize = int.Parse(BusinessLogic.WebKey_Get("AD_Size", site_id));
            SearchResult one = directorySearcher.FindOne();
            if (one != null)
            {
              adUsers.ID = one.Properties.Contains("objectguid") ? BitConverter.ToString((byte[]) one.Properties["objectguid"][0]).Replace("-", string.Empty) : string.Empty;
              adUsers.Email = one.Properties.Contains("mail") ? (string) one.Properties["mail"][0] : string.Empty;
              adUsers.Login = one.Properties.Contains("samaccountname") ? (string) one.Properties["samaccountname"][0] : string.Empty;
              adUsers.DisplayName = one.Properties.Contains("displayname") ? (string) one.Properties["displayname"][0] : string.Empty;
              adUsers.FistName = one.Properties.Contains("givenName") ? (string) one.Properties["givenName"][0] : string.Empty;
              adUsers.LastName = one.Properties.Contains("sn") ? (string) one.Properties["sn"][0] : string.Empty;
              adUsers.Manager = one.Properties.Contains("manager") ? (string) one.Properties["manager"][0] : string.Empty;
            }
          }
        }
      }
      catch (Exception ex)
      {
      }
      return adUsers;
    }

    public static List<ADUtility.ADUsers> Path_GetAllUsers(string path, string site_id)
    {
      List<ADUtility.ADUsers> source = new List<ADUtility.ADUsers>();
      try
      {
        using (DirectoryEntry searchRoot = new DirectoryEntry(BusinessLogic.WebKey_Get("AD_Path", site_id), BusinessLogic.WebKey_Get("AD_Login", site_id), BusinessLogic.WebKey_Get("AD_Password", site_id)))
        {
          using (DirectorySearcher directorySearcher = new DirectorySearcher(searchRoot, string.Format("(&(objectCategory=person)(objectClass=user))", (object) path), new string[9]
          {
            "objectguid",
            "samaccountname",
            "mail",
            "displayname",
            "name",
            "givenName",
            "sn",
            "distinguishedName",
            "manager"
          }))
          {
            directorySearcher.PageSize = int.Parse(BusinessLogic.WebKey_Get("AD_Size", site_id));
            using (SearchResultCollection all = directorySearcher.FindAll())
            {
              foreach (SearchResult searchResult in all)
              {
                if (searchResult != null)
                  source.Add(new ADUtility.ADUsers()
                  {
                    ID = searchResult.Properties.Contains("objectguid") ? BitConverter.ToString((byte[]) searchResult.Properties["objectguid"][0]).Replace("-", string.Empty) : string.Empty,
                    Email = searchResult.Properties.Contains("mail") ? (string) searchResult.Properties["mail"][0] : string.Empty,
                    Login = searchResult.Properties.Contains("samaccountname") ? (string) searchResult.Properties["samaccountname"][0] : string.Empty,
                    DisplayName = searchResult.Properties.Contains("displayname") ? (string) searchResult.Properties["displayname"][0] : string.Empty,
                    FistName = searchResult.Properties.Contains("givenName") ? (string) searchResult.Properties["givenName"][0] : string.Empty,
                    LastName = searchResult.Properties.Contains("sn") ? (string) searchResult.Properties["sn"][0] : string.Empty,
                    Path = searchResult.Properties.Contains("distinguishedName") ? (string) searchResult.Properties["distinguishedName"][0] : string.Empty,
                    Manager = searchResult.Properties.Contains("manager") ? (string) searchResult.Properties["manager"][0] : string.Empty
                  });
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
      }
      return source.Where<ADUtility.ADUsers>((Func<ADUtility.ADUsers, bool>) (t => t.Path.Contains(path))).ToList<ADUtility.ADUsers>();
    }

    public static List<ADUtility.ADUsers> AD_GetAllUsers(string site_id)
    {
      List<ADUtility.ADUsers> adUsersList = new List<ADUtility.ADUsers>();
      try
      {
        using (DirectoryEntry searchRoot = new DirectoryEntry(BusinessLogic.WebKey_Get("AD_Path", site_id), BusinessLogic.WebKey_Get("AD_Login", site_id), BusinessLogic.WebKey_Get("AD_Password", site_id)))
        {
          using (DirectorySearcher directorySearcher = new DirectorySearcher(searchRoot, "(&(objectCategory=person)(objectClass=user))", new string[9]
          {
            "objectguid",
            "samaccountname",
            "mail",
            "displayname",
            "name",
            "givenName",
            "sn",
            "distinguishedName",
            "manager"
          }))
          {
            directorySearcher.PageSize = int.Parse(BusinessLogic.WebKey_Get("AD_Size", site_id));
            using (SearchResultCollection all = directorySearcher.FindAll())
            {
              foreach (SearchResult searchResult in all)
              {
                if (searchResult != null)
                  adUsersList.Add(new ADUtility.ADUsers()
                  {
                    ID = searchResult.Properties.Contains("objectguid") ? BitConverter.ToString((byte[]) searchResult.Properties["objectguid"][0]).Replace("-", string.Empty) : string.Empty,
                    Email = searchResult.Properties.Contains("mail") ? (string) searchResult.Properties["mail"][0] : string.Empty,
                    Login = searchResult.Properties.Contains("samaccountname") ? (string) searchResult.Properties["samaccountname"][0] : string.Empty,
                    DisplayName = searchResult.Properties.Contains("displayname") ? (string) searchResult.Properties["displayname"][0] : string.Empty,
                    FistName = searchResult.Properties.Contains("givenName") ? (string) searchResult.Properties["givenName"][0] : string.Empty,
                    LastName = searchResult.Properties.Contains("sn") ? (string) searchResult.Properties["sn"][0] : string.Empty,
                    Path = searchResult.Properties.Contains("distinguishedName") ? (string) searchResult.Properties["distinguishedName"][0] : string.Empty,
                    Manager = searchResult.Properties.Contains("manager") ? (string) searchResult.Properties["manager"][0] : string.Empty
                  });
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
      }
      return adUsersList;
    }

    public static List<ADUtility.ADUsers> AD_GetAllUserByFullName(string site_id, string name)
    {
      List<ADUtility.ADUsers> source = new List<ADUtility.ADUsers>();
      try
      {
        using (DirectoryEntry searchRoot = new DirectoryEntry(BusinessLogic.WebKey_Get("AD_Path", site_id), BusinessLogic.WebKey_Get("AD_Login", site_id), BusinessLogic.WebKey_Get("AD_Password", site_id)))
        {
          using (DirectorySearcher directorySearcher = new DirectorySearcher(searchRoot, "(&(objectCategory=person)(objectClass=user))", new string[9]
          {
            "objectguid",
            "samaccountname",
            "mail",
            "displayname",
            "name",
            "givenName",
            "sn",
            "distinguishedName",
            "manager"
          }))
          {
            directorySearcher.PageSize = int.Parse(BusinessLogic.WebKey_Get("AD_Size", site_id));
            using (SearchResultCollection all = directorySearcher.FindAll())
            {
              foreach (SearchResult searchResult in all)
              {
                if (searchResult != null)
                  source.Add(new ADUtility.ADUsers()
                  {
                    ID = searchResult.Properties.Contains("objectguid") ? BitConverter.ToString((byte[]) searchResult.Properties["objectguid"][0]).Replace("-", string.Empty) : string.Empty,
                    Email = searchResult.Properties.Contains("mail") ? (string) searchResult.Properties["mail"][0] : string.Empty,
                    Login = searchResult.Properties.Contains("samaccountname") ? (string) searchResult.Properties["samaccountname"][0] : string.Empty,
                    DisplayName = searchResult.Properties.Contains("displayname") ? (string) searchResult.Properties["displayname"][0] : string.Empty,
                    FistName = searchResult.Properties.Contains("givenName") ? (string) searchResult.Properties["givenName"][0] : string.Empty,
                    LastName = searchResult.Properties.Contains("sn") ? (string) searchResult.Properties["sn"][0] : string.Empty,
                    Path = searchResult.Properties.Contains("distinguishedName") ? (string) searchResult.Properties["distinguishedName"][0] : string.Empty,
                    Manager = searchResult.Properties.Contains("manager") ? (string) searchResult.Properties["manager"][0] : string.Empty
                  });
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
      }
      return source.Where<ADUtility.ADUsers>((Func<ADUtility.ADUsers, bool>) (t => t.DisplayName.Contains(name))).ToList<ADUtility.ADUsers>();
    }

    public static bool IsUserBlocked(string login, string site_id)
    {
      int num = 0;
      try
      {
        using (DirectoryEntry searchRoot = new DirectoryEntry(BusinessLogic.WebKey_Get("AD_Path", site_id), BusinessLogic.WebKey_Get("AD_Login", site_id), BusinessLogic.WebKey_Get("AD_Password", site_id)))
        {
          if (searchRoot.NativeGuid == null)
            return false;
          using (DirectorySearcher directorySearcher = new DirectorySearcher(searchRoot, string.Format("(&(objectCategory=person)(objectClass=user)(sAMAccountName={0}))", (object) login.Trim()), new string[1]
          {
            "userAccountControl"
          }))
          {
            directorySearcher.PageSize = int.Parse(BusinessLogic.WebKey_Get("AD_Size", site_id));
            SearchResult one = directorySearcher.FindOne();
            if (one != null)
              num = one.Properties.Contains("userAccountControl") ? (int) one.Properties["userAccountControl"][0] : 0;
          }
        }
      }
      catch (Exception ex)
      {
      }
      return Convert.ToBoolean(num & 2);
    }

    public class ADUsers : IEquatable<ADUtility.ADUsers>
    {
      public string ID { get; set; }

      public string Login { get; set; }

      public string DisplayName { get; set; }

      public string Email { get; set; }

      public string FistName { get; set; }

      public string LastName { get; set; }

      public string Path { get; set; }

      public string Manager { get; set; }

      public bool Equals(ADUtility.ADUsers other)
      {
        return this.ID.Trim() == other.ID.Trim() && this.Login.Trim() == other.Login.Trim() && this.DisplayName.Trim() == other.DisplayName.Trim() && this.Email.Trim() == other.Email.Trim();
      }

      public override int GetHashCode()
      {
        return (this.ID == null ? 0 : this.ID.GetHashCode()) ^ (this.Login == null ? 0 : this.Login.GetHashCode()) ^ (this.DisplayName == null ? 0 : this.DisplayName.GetHashCode()) ^ (this.Email == null ? 0 : this.Email.GetHashCode());
      }
    }

    public class ADGroup
    {
      public string ID { get; set; }

      public string Title { get; set; }
    }
  }
}
