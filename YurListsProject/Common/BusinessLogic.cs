// Decompiled with JetBrains decompiler
// Type: FinalizeDocument.Common.BusinessLogic
// Assembly: NormGroup, Version=1.0.0.0, Culture=neutral, PublicKeyToken=9cfa31019f937ed3
// MVID: 5F82EBC5-F326-4558-8B82-DE6FC143EE5E
// Assembly location: C:\WorkSpace\Soutions\Recover_Colution\normgroup.wsp\NormGroup.dll

using ExpImp1C.Codes;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Administration.Claims;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Configuration;
using YurListsProject.Service1C;

namespace FinalizeDocument.Common
{
  public class BusinessLogic
  {
    public static double GetDouble(string value)
    {
      double result;
      if (!double.TryParse(value, NumberStyles.Any, (IFormatProvider) CultureInfo.CurrentCulture, out result) && !double.TryParse(value, NumberStyles.Any, (IFormatProvider) CultureInfo.GetCultureInfo("en-US"), out result) && !double.TryParse(value, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result))
        result = 0.0;
      return result;
    }

    public static string GetClearLogin(string old_login)
    {
      SPClaimProviderManager local = SPClaimProviderManager.Local;
      string str = "";
      if ((SPPersistedObject) local != (SPPersistedObject) null)
      {
        if (old_login.Split('\\').Length > 1)
          str = old_login.Split('\\')[1];
      }
      return str;
    }

    public static DateTime Date_Get(string _value)
    {
      DateTime result;
      if (!DateTime.TryParseExact(_value, "MM/dd/yyyy hh:mm:ss tt", (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.None, out result) && !DateTime.TryParseExact(_value, "MM/dd/yyyy h:mm:ss tt", (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.None, out result) && !DateTime.TryParseExact(_value, "yyyy-MM-dd h:mm:ss", (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.None, out result) && !DateTime.TryParse(_value, out result))
        result = new DateTime();
      return result;
    }

    public static string WebKey_Get(string key, string siteid)
    {
      string result = "";
      try
      {
        SPSecurity.RunWithElevatedPrivileges((SPSecurity.CodeToRunElevated) (() =>
        {
          using (SPSite spSite = new SPSite(new Guid(siteid)))
          {
            AppSettingsSection appSettings = WebConfigurationManager.OpenWebConfiguration("/", spSite.WebApplication.Name).AppSettings;
            if (appSettings.Settings.Count <= 0)
              return;
            KeyValueConfigurationElement setting = appSettings.Settings[key];
            if (setting != null)
              result = setting.Value;
          }
        }));
      }
      catch (Exception ex)
      {
        result = ex.Message;
      }
      return result;
    }

    public static void UpdatePermissionsItem(Guid siteID, Guid webID, Guid listID, int itemID)
    {
      try
      {
        SPSecurity.RunWithElevatedPrivileges((SPSecurity.CodeToRunElevated) (() =>
        {
          using (SPSite spSite = new SPSite(siteID))
          {
            using (SPWeb spWeb = spSite.OpenWeb(webID))
            {
              spWeb.AllowUnsafeUpdates = true;
              SPListItem itemById = spWeb.Lists[listID].Items.GetItemById(itemID);
              itemById.BreakRoleInheritance(true);
              SPRoleAssignmentCollection roleAssignments = itemById.RoleAssignments;
              SPRoleDefinition byType1 = itemById.Web.RoleDefinitions.GetByType(SPRoleType.Contributor);
              itemById.Web.RoleDefinitions.GetByType(SPRoleType.WebDesigner);
              SPRoleDefinition byType2 = itemById.Web.RoleDefinitions.GetByType(SPRoleType.Reader);
              SPRoleDefinition byType3 = itemById.Web.RoleDefinitions.GetByType(SPRoleType.Administrator);
              SPGroup spGroup = (SPGroup) null;
              try
              {
                spGroup = spWeb.SiteGroups["Archivarius"];
              }
              catch
              {
              }
              if (spGroup == null)
              {
                spWeb.SiteGroups.Add("Archivarius", (SPMember) spSite.Owner, spWeb.Author, "Archivarius");
                spGroup = spWeb.SiteGroups["Archivarius"];
                SPRoleDefinition byType4 = spWeb.RoleDefinitions.GetByType(SPRoleType.Contributor);
                spWeb.RoleAssignments.Add(new SPRoleAssignment((SPPrincipal) spGroup)
                {
                  RoleDefinitionBindings = {
                    byType4
                  }
                });
                spWeb.Update();
              }
              for (int index = itemById.RoleAssignments.Count - 1; index >= 0; --index)
              {
                if (itemById.RoleAssignments[index] != null)
                {
                  SPRoleDefinitionBindingCollection definitionBindings = itemById.RoleAssignments[index].RoleDefinitionBindings;
                  if (!itemById.RoleAssignments[index].RoleDefinitionBindings.Contains(byType3))
                  {
                    itemById.RoleAssignments[index].RoleDefinitionBindings.RemoveAll();
                    itemById.RoleAssignments[index].RoleDefinitionBindings.Add(byType2);
                    itemById.RoleAssignments[index].Update();
                  }
                }
              }
              if (spGroup != null)
                itemById.RoleAssignments.Add(new SPRoleAssignment((SPPrincipal) spGroup)
                {
                  RoleDefinitionBindings = {
                    byType1
                  }
                });
              itemById.SystemUpdate(false);
              spWeb.AllowUnsafeUpdates = false;
            }
          }
        }));
      }
      catch
      {
      }
      finally
      {
      }
    }

    public static void UpdatePermissionsItemByApprover(Guid siteID, Guid webID, Guid listID, int itemID, string field)
    {
      try
      {
        SPSecurity.RunWithElevatedPrivileges((SPSecurity.CodeToRunElevated) (() =>
        {
          using (SPSite spSite = new SPSite(siteID))
          {
            using (SPWeb spWeb = spSite.OpenWeb(webID))
            {
              spWeb.AllowUnsafeUpdates = true;
              SPListItem itemById = spWeb.Lists[listID].Items.GetItemById(itemID);
              itemById.BreakRoleInheritance(true);
              SPRoleAssignmentCollection roleAssignments = itemById.RoleAssignments;
              SPRoleDefinition byType1 = itemById.Web.RoleDefinitions.GetByType(SPRoleType.Editor);
              itemById.Web.RoleDefinitions.GetByType(SPRoleType.Contributor);
              itemById.Web.RoleDefinitions.GetByType(SPRoleType.WebDesigner);
              SPRoleDefinition byType2 = itemById.Web.RoleDefinitions.GetByType(SPRoleType.Reader);
              SPRoleDefinition byType3 = itemById.Web.RoleDefinitions.GetByType(SPRoleType.Administrator);
              SPGroup spGroup = (SPGroup) null;
              SPUser spUser1 = (SPUser) null;
              SPUser spUser2 = (SPUser) null;
              try
              {
                spGroup = BusinessLogic.GetSPGroup(itemById, field, spSite.RootWeb);
                spUser1 = BusinessLogic.GetSPUser(itemById, field);
                spUser2 = BusinessLogic.GetSPUser(itemById, "Author");
              }
              catch (Exception ex)
              {
              }
              for (int index = itemById.RoleAssignments.Count - 1; index >= 0; --index)
              {
                if (itemById.RoleAssignments[index] != null)
                {
                  SPRoleDefinitionBindingCollection definitionBindings = itemById.RoleAssignments[index].RoleDefinitionBindings;
                  if (!itemById.RoleAssignments[index].RoleDefinitionBindings.Contains(byType3) && !itemById.RoleAssignments[index].RoleDefinitionBindings.Contains(byType1) && !itemById.RoleAssignments[index].RoleDefinitionBindings.Contains(byType2))
                  {
                    itemById.RoleAssignments[index].RoleDefinitionBindings.RemoveAll();
                    itemById.RoleAssignments[index].Update();
                  }
                  else if (!itemById.RoleAssignments[index].RoleDefinitionBindings.Contains(byType3) && (itemById.RoleAssignments[index].RoleDefinitionBindings.Contains(byType1) || itemById.RoleAssignments[index].RoleDefinitionBindings.Contains(byType2)))
                  {
                    itemById.RoleAssignments[index].RoleDefinitionBindings.RemoveAll();
                    itemById.RoleAssignments[index].RoleDefinitionBindings.Add(byType2);
                    itemById.RoleAssignments[index].Update();
                  }
                }
              }
              if (spGroup != null)
                itemById.RoleAssignments.Add(new SPRoleAssignment((SPPrincipal) spGroup)
                {
                  RoleDefinitionBindings = {
                    byType1
                  }
                });
              if (spUser1 != null)
                itemById.RoleAssignments.Add(new SPRoleAssignment((SPPrincipal) spUser1)
                {
                  RoleDefinitionBindings = {
                    byType1
                  }
                });
              if (spUser2 != null)
                itemById.RoleAssignments.Add(new SPRoleAssignment((SPPrincipal) spUser2)
                {
                  RoleDefinitionBindings = {
                    byType1
                  }
                });
              itemById.SystemUpdate(false);
              spWeb.AllowUnsafeUpdates = false;
            }
          }
        }));
      }
      catch
      {
      }
      finally
      {
      }
    }

    public static void UpdatePermissionsItemOnlyRead(Guid siteID, Guid webID, Guid listID, int itemID)
    {
      try
      {
        SPSecurity.RunWithElevatedPrivileges((SPSecurity.CodeToRunElevated) (() =>
        {
          using (SPSite spSite = new SPSite(siteID))
          {
            using (SPWeb spWeb = spSite.OpenWeb(webID))
            {
              spWeb.AllowUnsafeUpdates = true;
              SPListItem itemById = spWeb.Lists[listID].Items.GetItemById(itemID);
              itemById.BreakRoleInheritance(true);
              SPRoleAssignmentCollection roleAssignments = itemById.RoleAssignments;
              itemById.Web.RoleDefinitions.GetByType(SPRoleType.Editor);
              itemById.Web.RoleDefinitions.GetByType(SPRoleType.Contributor);
              itemById.Web.RoleDefinitions.GetByType(SPRoleType.WebDesigner);
              SPRoleDefinition byType = itemById.Web.RoleDefinitions.GetByType(SPRoleType.Reader);
              itemById.Web.RoleDefinitions.GetByType(SPRoleType.Administrator);
              for (int index = itemById.RoleAssignments.Count - 1; index >= 0; --index)
              {
                if (itemById.RoleAssignments[index] != null)
                {
                  SPRoleDefinitionBindingCollection definitionBindings = itemById.RoleAssignments[index].RoleDefinitionBindings;
                  itemById.RoleAssignments[index].RoleDefinitionBindings.RemoveAll();
                  itemById.RoleAssignments[index].RoleDefinitionBindings.Add(byType);
                  itemById.RoleAssignments[index].Update();
                }
              }
              itemById.SystemUpdate(false);
              spWeb.AllowUnsafeUpdates = false;
            }
          }
        }));
      }
      catch
      {
      }
      finally
      {
      }
    }

    public static void UpdatePermissionsItemByApprover(Guid siteID, Guid webID, Guid listID, int itemID, SPPrincipal principial)
    {
      try
      {
        SPSecurity.RunWithElevatedPrivileges((SPSecurity.CodeToRunElevated) (() =>
        {
          using (SPSite spSite = new SPSite(siteID))
          {
            using (SPWeb spWeb = spSite.OpenWeb(webID))
            {
              spWeb.AllowUnsafeUpdates = true;
              SPListItem itemById = spWeb.Lists[listID].Items.GetItemById(itemID);
              itemById.BreakRoleInheritance(true);
              SPRoleAssignmentCollection roleAssignments = itemById.RoleAssignments;
              SPRoleDefinition byType1 = itemById.Web.RoleDefinitions.GetByType(SPRoleType.Editor);
              itemById.Web.RoleDefinitions.GetByType(SPRoleType.Contributor);
              itemById.Web.RoleDefinitions.GetByType(SPRoleType.WebDesigner);
              SPRoleDefinition byType2 = itemById.Web.RoleDefinitions.GetByType(SPRoleType.Reader);
              SPRoleDefinition byType3 = itemById.Web.RoleDefinitions.GetByType(SPRoleType.Administrator);
              for (int index = itemById.RoleAssignments.Count - 1; index >= 0; --index)
              {
                if (itemById.RoleAssignments[index] != null)
                {
                  SPRoleDefinitionBindingCollection definitionBindings = itemById.RoleAssignments[index].RoleDefinitionBindings;
                  if (!itemById.RoleAssignments[index].RoleDefinitionBindings.Contains(byType3) && !itemById.RoleAssignments[index].RoleDefinitionBindings.Contains(byType1) && !itemById.RoleAssignments[index].RoleDefinitionBindings.Contains(byType2))
                  {
                    itemById.RoleAssignments[index].RoleDefinitionBindings.RemoveAll();
                    itemById.RoleAssignments[index].Update();
                  }
                  else if (!itemById.RoleAssignments[index].RoleDefinitionBindings.Contains(byType3) && (itemById.RoleAssignments[index].RoleDefinitionBindings.Contains(byType1) || itemById.RoleAssignments[index].RoleDefinitionBindings.Contains(byType2)))
                  {
                    itemById.RoleAssignments[index].RoleDefinitionBindings.RemoveAll();
                    itemById.RoleAssignments[index].RoleDefinitionBindings.Add(byType2);
                    itemById.RoleAssignments[index].Update();
                  }
                }
              }
              if (principial != null)
                itemById.RoleAssignments.Add(new SPRoleAssignment(principial)
                {
                  RoleDefinitionBindings = {
                    byType1
                  }
                });
              itemById.SystemUpdate(false);
              spWeb.AllowUnsafeUpdates = false;
            }
          }
        }));
      }
      catch
      {
      }
      finally
      {
      }
    }

    public static void UpdateDepartmentPermissionsItem(Guid siteID, Guid webID, Guid listID, int itemID, string fieldName)
    {
      try
      {
        SPSecurity.RunWithElevatedPrivileges((SPSecurity.CodeToRunElevated) (() =>
        {
          using (new DisabledItemEventsScope())
          {
            using (SPSite spSite = new SPSite(siteID))
            {
              using (SPWeb rootWeb = spSite.RootWeb)
              {
                using (SPWeb spWeb = spSite.OpenWeb(webID))
                {
                  spWeb.AllowUnsafeUpdates = true;
                  rootWeb.AllowUnsafeUpdates = true;
                  SPListItem itemById = spWeb.Lists[listID].Items.GetItemById(itemID);
                  itemById.BreakRoleInheritance(true);
                  SPRoleAssignmentCollection roleAssignments = itemById.RoleAssignments;
                  SPRoleDefinition byType1 = spWeb.RoleDefinitions.GetByType(SPRoleType.Contributor);
                  spWeb.RoleDefinitions.GetByType(SPRoleType.WebDesigner);
                  SPRoleDefinition byType2 = spWeb.RoleDefinitions.GetByType(SPRoleType.Reader);
                  SPRoleDefinition byType3 = spWeb.RoleDefinitions.GetByType(SPRoleType.Administrator);
                  List<SPPrincipal> spPrinsipials = BusinessLogic.MultiUsers_GetSPPrinsipials(rootWeb, itemById, fieldName);
                  SPUser spUser = BusinessLogic.GetSPUser(itemById, "Author");
                  for (int index = itemById.RoleAssignments.Count - 1; index >= 0; --index)
                  {
                    if (itemById.RoleAssignments[index] != null)
                    {
                      SPRoleDefinitionBindingCollection definitionBindings = itemById.RoleAssignments[index].RoleDefinitionBindings;
                      if (!itemById.RoleAssignments[index].RoleDefinitionBindings.Contains(byType3))
                      {
                        itemById.RoleAssignments[index].RoleDefinitionBindings.RemoveAll();
                        itemById.RoleAssignments[index].RoleDefinitionBindings.Add(byType2);
                        itemById.RoleAssignments[index].Update();
                      }
                    }
                  }
                  for (int index = 0; index < spPrinsipials.Count; ++index)
                    itemById.RoleAssignments.Add(new SPRoleAssignment(spPrinsipials[index])
                    {
                      RoleDefinitionBindings = {
                        byType1
                      }
                    });
                  if (spUser != null)
                    itemById.RoleAssignments.Add(new SPRoleAssignment((SPPrincipal) spUser)
                    {
                      RoleDefinitionBindings = {
                        byType1
                      }
                    });
                  itemById.SystemUpdate(false);
                  spWeb.AllowUnsafeUpdates = false;
                  rootWeb.AllowUnsafeUpdates = false;
                }
              }
            }
          }
        }));
      }
      catch
      {
      }
      finally
      {
      }
    }

    public static void UpdatePayRollPermissionsItem(Guid siteID, Guid webID, Guid listID, int itemID)
    {
      try
      {
        SPSecurity.RunWithElevatedPrivileges((SPSecurity.CodeToRunElevated) (() =>
        {
          using (new DisabledItemEventsScope())
          {
            using (SPSite spSite = new SPSite(siteID))
            {
              using (SPWeb rootWeb = spSite.RootWeb)
              {
                using (SPWeb spWeb = spSite.OpenWeb(webID))
                {
                  spWeb.AllowUnsafeUpdates = true;
                  rootWeb.AllowUnsafeUpdates = true;
                  SPList list1 = spWeb.Lists[listID];
                  SPList list2 = spWeb.GetList("Lists/DepartmentsList");
                  SPList list3 = spWeb.GetList("Lists/PayRollPossitionList");
                  SPListItem itemById1 = list1.Items.GetItemById(itemID);
                  itemById1.BreakRoleInheritance(true);
                  SPRoleAssignmentCollection roleAssignments = itemById1.RoleAssignments;
                  SPRoleDefinition byType1 = spWeb.RoleDefinitions.GetByType(SPRoleType.Contributor);
                  spWeb.RoleDefinitions.GetByType(SPRoleType.WebDesigner);
                  SPRoleDefinition byType2 = spWeb.RoleDefinitions.GetByType(SPRoleType.Reader);
                  SPRoleDefinition byType3 = spWeb.RoleDefinitions.GetByType(SPRoleType.Administrator);
                  int num = 0;
                  try
                  {
                    num = BusinessLogic.LookUpID_Get(itemById1, "Status");
                  }
                  catch
                  {
                  }
                  SPGroup group = (SPGroup) null;
                  SPUser user = (SPUser) null;
                  try
                  {
                    int id = BusinessLogic.LookUpID_Get(itemById1, "Department1");
                    SPListItem itemById2 = list2.Items.GetItemById(id);
                    user = BusinessLogic.GetSPUser(itemById2, "Manger");
                    group = BusinessLogic.GetSPGroup(itemById2, "Manager", rootWeb);
                  }
                  catch
                  {
                  }
                  for (int index = itemById1.RoleAssignments.Count - 1; index >= 0; --index)
                  {
                    if (itemById1.RoleAssignments[index] != null)
                    {
                      SPRoleDefinitionBindingCollection definitionBindings = itemById1.RoleAssignments[index].RoleDefinitionBindings;
                      if (!itemById1.RoleAssignments[index].RoleDefinitionBindings.Contains(byType3))
                      {
                        itemById1.RoleAssignments[index].RoleDefinitionBindings.RemoveAll();
                        itemById1.RoleAssignments[index].RoleDefinitionBindings.Add(byType2);
                        itemById1.RoleAssignments[index].Update();
                      }
                    }
                  }
                  List<SPWorkflow> list4 = itemById1.Workflows.OfType<SPWorkflow>().Where<SPWorkflow>((Func<SPWorkflow, bool>) (t => !t.IsCompleted)).ToList<SPWorkflow>();
                  List<ApproveTableItem> approve_list = new List<ApproveTableItem>();
                  foreach (SPWorkflow spWorkflow in list4)
                  {
                    foreach (ApproveTableItem approveTableItem in BusinessLogic.ApproveTableItems_Get(spSite.Url, spWeb.ServerRelativeUrl, num, "Lists/ApproversList", spWorkflow.AssociationId.ToString()))
                      approve_list.Add(approveTableItem);
                  }
                  switch (num)
                  {
                    case 1:
                      SPListItemCollection items = list3.GetItems(new SPQuery()
                      {
                        Query = "<OrderBy>" + "<FieldRef Name='ID' Ascending='FALSE' />" + "</OrderBy>" + "<Where>" + "<Eq>" + "<FieldRef Name='PayRollID' />" + "<Value Type='Number'>" + itemById1.ID.ToString() + "</Value>" + "</Eq>" + "</Where>"
                      });
                      if (items.Count > 0)
                      {
                        foreach (SPListItem spListItem in (SPBaseCollection) items)
                          BusinessLogic.UpdatePayRollPermissionsSubItem(siteID, webID, list3.ID, spListItem.ID, group, user, approve_list, num);
                      }
                      itemById1.SystemUpdate(false);
                      spWeb.AllowUnsafeUpdates = false;
                      rootWeb.AllowUnsafeUpdates = false;
                      break;
                    case 2:
                      if (user != null)
                      {
                        itemById1.RoleAssignments.Add(new SPRoleAssignment((SPPrincipal) user)
                        {
                          RoleDefinitionBindings = {
                            byType1
                          }
                        });
                        itemById1["Approver"] = (object) user;
                        goto case 1;
                      }
                      else if (group != null)
                      {
                        itemById1.RoleAssignments.Add(new SPRoleAssignment((SPPrincipal) group)
                        {
                          RoleDefinitionBindings = {
                            byType1
                          }
                        });
                        itemById1["Approver"] = (object) group;
                        goto case 1;
                      }
                      else
                        goto case 1;
                    default:
                      using (List<ApproveTableItem>.Enumerator enumerator = approve_list.GetEnumerator())
                      {
                        while (enumerator.MoveNext())
                        {
                          ApproveTableItem current = enumerator.Current;
                          itemById1.RoleAssignments.Add(new SPRoleAssignment(current.Approver)
                          {
                            RoleDefinitionBindings = {
                              byType1
                            }
                          });
                          itemById1["Approver"] = (object) current;
                        }
                        goto case 1;
                      }
                  }
                }
              }
            }
          }
        }));
      }
      catch
      {
      }
      finally
      {
      }
    }

    public static void UpdatePayRollPermissionsSubItem(Guid siteID, Guid webID, Guid listID, int itemID, SPGroup group, SPUser user, List<ApproveTableItem> approve_list, int status_id)
    {
      try
      {
        SPSecurity.RunWithElevatedPrivileges((SPSecurity.CodeToRunElevated) (() =>
        {
          using (new DisabledItemEventsScope())
          {
            using (SPSite spSite = new SPSite(siteID))
            {
              using (SPWeb rootWeb = spSite.RootWeb)
              {
                using (SPWeb spWeb = spSite.OpenWeb(webID))
                {
                  spWeb.AllowUnsafeUpdates = true;
                  rootWeb.AllowUnsafeUpdates = true;
                  SPListItem itemById = spWeb.Lists[listID].Items.GetItemById(itemID);
                  itemById.BreakRoleInheritance(true);
                  SPRoleAssignmentCollection roleAssignments = itemById.RoleAssignments;
                  SPRoleDefinition byType1 = spWeb.RoleDefinitions.GetByType(SPRoleType.Contributor);
                  spWeb.RoleDefinitions.GetByType(SPRoleType.WebDesigner);
                  SPRoleDefinition byType2 = spWeb.RoleDefinitions.GetByType(SPRoleType.Reader);
                  SPRoleDefinition byType3 = spWeb.RoleDefinitions.GetByType(SPRoleType.Administrator);
                  for (int index = itemById.RoleAssignments.Count - 1; index >= 0; --index)
                  {
                    if (itemById.RoleAssignments[index] != null)
                    {
                      SPRoleDefinitionBindingCollection definitionBindings = itemById.RoleAssignments[index].RoleDefinitionBindings;
                      if (!itemById.RoleAssignments[index].RoleDefinitionBindings.Contains(byType3))
                      {
                        itemById.RoleAssignments[index].RoleDefinitionBindings.RemoveAll();
                        itemById.RoleAssignments[index].RoleDefinitionBindings.Add(byType2);
                        itemById.RoleAssignments[index].Update();
                      }
                    }
                  }
                  switch (status_id)
                  {
                    case 1:
                      itemById.SystemUpdate(false);
                      spWeb.AllowUnsafeUpdates = false;
                      rootWeb.AllowUnsafeUpdates = false;
                      break;
                    case 2:
                      if (user != null)
                      {
                        itemById.RoleAssignments.Add(new SPRoleAssignment((SPPrincipal) user)
                        {
                          RoleDefinitionBindings = {
                            byType1
                          }
                        });
                        itemById["Approver"] = (object) user;
                        goto case 1;
                      }
                      else if (group != null)
                      {
                        itemById.RoleAssignments.Add(new SPRoleAssignment((SPPrincipal) group)
                        {
                          RoleDefinitionBindings = {
                            byType1
                          }
                        });
                        itemById["Approver"] = (object) group;
                        goto case 1;
                      }
                      else
                        goto case 1;
                    default:
                      using (List<ApproveTableItem>.Enumerator enumerator = approve_list.GetEnumerator())
                      {
                        while (enumerator.MoveNext())
                        {
                          ApproveTableItem current = enumerator.Current;
                          itemById.RoleAssignments.Add(new SPRoleAssignment(current.Approver)
                          {
                            RoleDefinitionBindings = {
                              byType1
                            }
                          });
                          itemById["Approver"] = (object) current;
                        }
                        goto case 1;
                      }
                  }
                }
              }
            }
          }
        }));
      }
      catch
      {
      }
      finally
      {
      }
    }

    public static SPUser GetSPUser(SPListItem item, string key)
    {
      SPFieldUser fieldByInternalName = item.Fields.GetFieldByInternalName(key) as SPFieldUser;
      try
      {
        if (fieldByInternalName != null)
        {
          if (item[key] != null)
          {
            try
            {
              SPFieldUserValue fieldValue = fieldByInternalName.GetFieldValue(item[key].ToString()) as SPFieldUserValue;
              if (fieldValue != null)
                return fieldValue.User;
            }
            catch (Exception ex)
            {
            }
          }
        }
      }
      catch (Exception ex)
      {
      }
      return (SPUser) null;
    }

    public static SPGroup GetSPGroup(SPListItem item, string key, SPWeb web)
    {
      SPFieldUser fieldByInternalName = item.Fields.GetFieldByInternalName(key) as SPFieldUser;
      try
      {
        if (fieldByInternalName != null)
        {
          if (item[key] != null)
          {
            SPFieldUserValue fieldValue = fieldByInternalName.GetFieldValue(item[key].ToString()) as SPFieldUserValue;
            if (fieldValue != null)
            {
              try
              {
                return web.Groups[fieldValue.LookupValue];
              }
              catch (Exception ex)
              {
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
      }
      return (SPGroup) null;
    }

    public static void DownloadFileUsingFileStream(SPFile file, string path)
    {
      using (Stream stream = file.OpenBinaryStream())
      {
        using (FileStream fileStream = new FileStream(path + file.Name, FileMode.Create, FileAccess.Write))
        {
          byte[] buffer = new byte[file.Length];
          int count;
          while ((count = stream.Read(buffer, 0, buffer.Length)) > 0)
            fileStream.Write(buffer, 0, count);
        }
      }
    }

    public static float GetFloat(string value)
    {
      float result;
      if (!float.TryParse(value, NumberStyles.Any, (IFormatProvider) CultureInfo.CurrentCulture, out result) && !float.TryParse(value, NumberStyles.Any, (IFormatProvider) CultureInfo.GetCultureInfo("en-US"), out result) && !float.TryParse(value, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result))
        result = 0.0f;
      return result;
    }

    public static string GetNumber(string value)
    {
      string str = "";
      try
      {
        if (value.Contains("(ЄДРПОУ:"))
        {
          int startIndex = value.IndexOf("(ЄДРПОУ:") + 8;
          int num = value.IndexOf(")", value.IndexOf("(ЄДРПОУ:") + 8);
          str = value.Substring(startIndex, num - startIndex).Replace(" ", "");
        }
      }
      catch (Exception ex)
      {
        str = ex.Message;
      }
      return str;
    }

    public static void SendGroupEmail(string subject, string body, SPGroup group, string SiteID)
    {
      try
      {
        SPSecurity.RunWithElevatedPrivileges((SPSecurity.CodeToRunElevated) (() =>
        {
          using (SPSite spSite = new SPSite(new Guid(SiteID)))
          {
            using (SPWeb web = spSite.OpenWeb("/"))
            {
              foreach (SPUser user in (SPBaseCollection) group.Users)
                SPUtility.SendEmail(web, false, false, user.Email, subject, body);
            }
          }
        }));
      }
      catch (Exception ex)
      {
      }
    }

    public static string FillDirectoryList(string site_url, string web_url, string list_url, string directory)
    {
      string result = "";
      try
      {
        SPSecurity.RunWithElevatedPrivileges((SPSecurity.CodeToRunElevated) (() =>
        {
          using (SPSite spSite = new SPSite(site_url))
          {
            using (SPWeb spWeb = spSite.OpenWeb(web_url))
            {
              spWeb.AllowUnsafeUpdates = true;
              SPList list = spWeb.GetList(list_url);
              List<DeptString> deptStringList = new List<DeptString>();
              using (InfoBase1CServiceProxy base1CserviceProxy = new InfoBase1CServiceProxy(spSite.ID.ToString()))
              {
                DeptString term = new DeptString()
                {
                  Code = "",
                  Name = "",
                  SprName = directory
                };
                deptStringList = base1CserviceProxy.GetDebt(term).OrderBy<DeptString, string>((Func<DeptString, string>) (t => t.Code)).ToList<DeptString>();
              }
              for (int index = 0; index < deptStringList.Count; ++index)
              {
                if (!string.IsNullOrEmpty(deptStringList[index].Code.Replace(" ", "")))
                {
                  SPListItemCollection items = list.GetItems(new SPQuery()
                  {
                    Query = "<OrderBy>" + "<FieldRef Name='ID' Ascending='FALSE' />" + "</OrderBy>" + "<Where>" + "<Eq>" + "<FieldRef Name='ID1C' />" + "<Value Type='Text'>" + deptStringList[index].Code + "</Value>" + "</Eq>" + "</Where>"
                  });
                  if (items.Count > 0)
                  {
                    foreach (SPListItem spListItem in (SPBaseCollection) items)
                    {
                      spListItem["Title"] = (object) deptStringList[index].Name;
                      spListItem.Update();
                    }
                  }
                  else
                  {
                    SPListItem spListItem = list.AddItem();
                    spListItem["ID1C"] = (object) deptStringList[index].Code;
                    spListItem["Title"] = (object) deptStringList[index].Name;
                    spListItem.Update();
                  }
                }
              }
              result = "Win";
              spWeb.AllowUnsafeUpdates = false;
            }
          }
        }));
      }
      catch (Exception ex)
      {
        result = ex.Message;
      }
      return result;
    }

    public static string FillDepartmentList(string site_url, string web_url, string list_url)
    {
      string result = "";
      try
      {
        SPSecurity.RunWithElevatedPrivileges((SPSecurity.CodeToRunElevated) (() =>
        {
          using (SPSite spSite = new SPSite(site_url))
          {
            using (SPWeb spWeb = spSite.OpenWeb(web_url))
            {
              spWeb.AllowUnsafeUpdates = true;
              SPList list = spWeb.GetList(list_url);
              List<DeptString> deptStringList = new List<DeptString>();
              using (InfoBase1CServiceProxy base1CserviceProxy = new InfoBase1CServiceProxy(spSite.ID.ToString()))
              {
                DeptString term = new DeptString()
                {
                  Code = "",
                  Name = "",
                  SprName = "Подразделения"
                };
                deptStringList = base1CserviceProxy.GetDebt(term).OrderBy<DeptString, string>((Func<DeptString, string>) (t => t.Code)).ToList<DeptString>();
              }
              for (int index = 0; index < deptStringList.Count; ++index)
              {
                if (!string.IsNullOrEmpty(deptStringList[index].Code.Replace(" ", "")))
                {
                  SPListItemCollection items = list.GetItems(new SPQuery()
                  {
                    Query = "<OrderBy>" + "<FieldRef Name='ID' Ascending='FALSE' />" + "</OrderBy>" + "<Where>" + "<Eq>" + "<FieldRef Name='ID1C' />" + "<Value Type='Text'>" + deptStringList[index].Code + "</Value>" + "</Eq>" + "</Where>"
                  });
                  if (items.Count > 0)
                  {
                    foreach (SPListItem spListItem in (SPBaseCollection) items)
                    {
                      spListItem["Title"] = (object) deptStringList[index].Name;
                      spListItem.Update();
                    }
                  }
                  else
                  {
                    SPListItem spListItem = list.AddItem();
                    spListItem["ID1C"] = (object) deptStringList[index].Code;
                    spListItem["Title"] = (object) deptStringList[index].Name;
                    spListItem.Update();
                  }
                }
              }
              result = "Win";
              spWeb.AllowUnsafeUpdates = false;
            }
          }
        }));
      }
      catch (Exception ex)
      {
        result = ex.Message;
      }
      return result;
    }

    public static string Fill1CList(string site_url, string web_url, string list_url, string name)
    {
      string result = "";
      try
      {
        SPSecurity.RunWithElevatedPrivileges((SPSecurity.CodeToRunElevated) (() =>
        {
          using (SPSite spSite = new SPSite(site_url))
          {
            using (SPWeb spWeb = spSite.OpenWeb(web_url))
            {
              spWeb.AllowUnsafeUpdates = true;
              SPList list = spWeb.GetList(list_url);
              List<DeptString> deptStringList = new List<DeptString>();
              using (InfoBase1CServiceProxy base1CserviceProxy = new InfoBase1CServiceProxy(spSite.ID.ToString()))
              {
                DeptString term = new DeptString()
                {
                  Code = "",
                  Name = "",
                  SprName = name
                };
                deptStringList = base1CserviceProxy.GetDebt(term).OrderBy<DeptString, string>((Func<DeptString, string>) (t => t.Code)).ToList<DeptString>();
              }
              for (int index = 0; index < deptStringList.Count; ++index)
              {
                if (!string.IsNullOrEmpty(deptStringList[index].Code.Replace(" ", "")))
                {
                  SPListItemCollection items = list.GetItems(new SPQuery()
                  {
                    Query = "<OrderBy>" + "<FieldRef Name='ID' Ascending='FALSE' />" + "</OrderBy>" + "<Where>" + "<Eq>" + "<FieldRef Name='ID1C' />" + "<Value Type='Text'>" + deptStringList[index].Code + "</Value>" + "</Eq>" + "</Where>"
                  });
                  if (items.Count > 0)
                  {
                    foreach (SPListItem spListItem in (SPBaseCollection) items)
                    {
                      spListItem["Title"] = (object) deptStringList[index].Name;
                      spListItem.Update();
                    }
                  }
                  else
                  {
                    SPListItem spListItem = list.AddItem();
                    spListItem["ID1C"] = (object) deptStringList[index].Code;
                    spListItem["Title"] = (object) deptStringList[index].Name;
                    spListItem.Update();
                  }
                }
              }
              result = "Win";
              spWeb.AllowUnsafeUpdates = false;
            }
          }
        }));
      }
      catch (Exception ex)
      {
        result = ex.Message;
      }
      return result;
    }

    public static string FillCashKindList(string site_url, string web_url, string list_url)
    {
      string result = "";
      try
      {
        SPSecurity.RunWithElevatedPrivileges((SPSecurity.CodeToRunElevated) (() =>
        {
          using (SPSite spSite = new SPSite(site_url))
          {
            using (SPWeb spWeb = spSite.OpenWeb(web_url))
            {
              spWeb.AllowUnsafeUpdates = true;
              SPList list = spWeb.GetList(list_url);
              List<DeptString> deptStringList = new List<DeptString>();
              using (InfoBase1CServiceProxy base1CserviceProxy = new InfoBase1CServiceProxy(spSite.ID.ToString()))
                deptStringList = base1CserviceProxy.SelectTypeCalculationKinds().OrderBy<DeptString, string>((Func<DeptString, string>) (t => t.Code)).ToList<DeptString>();
              foreach (SPListItem spListItem in (SPBaseCollection) list.Items)
              {
                spListItem["IsActive1"] = (object) 0;
                spListItem.Update();
              }
              for (int index = 0; index < deptStringList.Count; ++index)
              {
                if (!string.IsNullOrEmpty(deptStringList[index].Code.Replace(" ", "")))
                {
                  SPListItemCollection items = list.GetItems(new SPQuery()
                  {
                    Query = "<OrderBy>" + "<FieldRef Name='ID' Ascending='FALSE' />" + "</OrderBy>" + "<Where>" + "<Eq>" + "<FieldRef Name='ID1C' />" + "<Value Type='Text'>" + deptStringList[index].Code + "</Value>" + "</Eq>" + "</Where>"
                  });
                  if (items.Count > 0)
                  {
                    foreach (SPListItem spListItem in (SPBaseCollection) items)
                    {
                      spListItem["Title"] = (object) deptStringList[index].Name;
                      spListItem["IsActive1"] = (object) 1;
                      spListItem.Update();
                    }
                  }
                  else
                  {
                    SPListItem spListItem = list.AddItem();
                    spListItem["ID1C"] = (object) deptStringList[index].Code;
                    spListItem["Title"] = (object) deptStringList[index].Name;
                    spListItem["IsActive1"] = (object) 1;
                    spListItem.Update();
                  }
                }
              }
              result = "Win";
              spWeb.AllowUnsafeUpdates = false;
            }
          }
        }));
      }
      catch (Exception ex)
      {
        result = ex.Message;
      }
      return result;
    }

    public static string FillRetentionKindList(string site_url, string web_url, string list_url)
    {
      string result = "";
      try
      {
        SPSecurity.RunWithElevatedPrivileges((SPSecurity.CodeToRunElevated) (() =>
        {
          using (SPSite spSite = new SPSite(site_url))
          {
            using (SPWeb spWeb = spSite.OpenWeb(web_url))
            {
              spWeb.AllowUnsafeUpdates = true;
              SPList list = spWeb.GetList(list_url);
              List<DeptString> deptStringList = new List<DeptString>();
              using (InfoBase1CServiceProxy base1CserviceProxy = new InfoBase1CServiceProxy(spSite.ID.ToString()))
                deptStringList = base1CserviceProxy.SelectTypeRetentionKinds().OrderBy<DeptString, string>((Func<DeptString, string>) (t => t.Code)).ToList<DeptString>();
              foreach (SPListItem spListItem in (SPBaseCollection) list.Items)
              {
                spListItem["IsActive1"] = (object) 0;
                spListItem.Update();
              }
              for (int index = 0; index < deptStringList.Count; ++index)
              {
                if (!string.IsNullOrEmpty(deptStringList[index].Code.Replace(" ", "")))
                {
                  SPListItemCollection items = list.GetItems(new SPQuery()
                  {
                    Query = "<OrderBy>" + "<FieldRef Name='ID' Ascending='FALSE' />" + "</OrderBy>" + "<Where>" + "<Eq>" + "<FieldRef Name='ID1C' />" + "<Value Type='Text'>" + deptStringList[index].Code + "</Value>" + "</Eq>" + "</Where>"
                  });
                  if (items.Count > 0)
                  {
                    foreach (SPListItem spListItem in (SPBaseCollection) items)
                    {
                      spListItem["Title"] = (object) deptStringList[index].Name;
                      spListItem["IsActive1"] = (object) 1;
                      spListItem.Update();
                    }
                  }
                  else
                  {
                    SPListItem spListItem = list.AddItem();
                    spListItem["ID1C"] = (object) deptStringList[index].Code;
                    spListItem["Title"] = (object) deptStringList[index].Name;
                    spListItem["IsActive1"] = (object) 1;
                    spListItem.Update();
                  }
                }
              }
              result = "Win";
              spWeb.AllowUnsafeUpdates = false;
            }
          }
        }));
      }
      catch (Exception ex)
      {
        result = ex.Message;
      }
      return result;
    }

    public static SharepointItem SharepointItem_Get(string site_url, string web_url, string list_url, int ID)
    {
      SharepointItem result = new SharepointItem();
      try
      {
        SPSecurity.RunWithElevatedPrivileges((SPSecurity.CodeToRunElevated) (() =>
        {
          using (SPSite spSite = new SPSite(site_url))
          {
            using (SPWeb spWeb = spSite.OpenWeb(web_url))
            {
              spWeb.AllowUnsafeUpdates = true;
              SPListItemCollection items = spWeb.GetList(list_url).GetItems(new SPQuery()
              {
                Query = "<OrderBy>" + "<FieldRef Name='ID' Ascending='FALSE' />" + "</OrderBy>" + "<Where>" + "<Eq>" + "<FieldRef Name='ID' />" + "<Value Type='Number'>" + ID.ToString() + "</Value>" + "</Eq>" + "</Where>"
              });
              if (items.Count > 0)
              {
                foreach (SPListItem spListItem in (SPBaseCollection) items)
                  result = new SharepointItem()
                  {
                    ID = ID,
                    Title = spListItem["Title"] == null ? "" : spListItem["Title"].ToString(),
                    ID1C = spListItem["ID1C"] == null ? "" : spListItem["ID1C"].ToString()
                  };
              }
              spWeb.AllowUnsafeUpdates = false;
            }
          }
        }));
      }
      catch (Exception ex)
      {
        result = new SharepointItem()
        {
          ID = -1,
          Title = ex.Message,
          ID1C = ex.Message
        };
      }
      return result;
    }

    public static List<SharepointItem> SharepointItems_Get(string site_url, string web_url, string list_url)
    {
      List<SharepointItem> result = new List<SharepointItem>();
      try
      {
        SPSecurity.RunWithElevatedPrivileges((SPSecurity.CodeToRunElevated) (() =>
        {
          using (SPSite spSite = new SPSite(site_url))
          {
            using (SPWeb spWeb = spSite.OpenWeb(web_url))
            {
              spWeb.AllowUnsafeUpdates = true;
              SPListItemCollection items = spWeb.GetList(list_url).GetItems(new SPQuery()
              {
                Query = "<OrderBy>" + "<FieldRef Name='ID' />" + "</OrderBy>" + "<Where>" + "<Eq>" + "<FieldRef Name='IsActive1' />" + "<Value Type='Boolean'>1</Value>" + "</Eq>" + "</Where>"
              });
              if (items.Count > 0)
              {
                foreach (SPListItem spListItem in (SPBaseCollection) items)
                  result.Add(new SharepointItem()
                  {
                    ID = spListItem.ID,
                    Title = spListItem["Title"] == null ? "" : spListItem["Title"].ToString(),
                    ID1C = spListItem["ID1C"] == null ? "" : spListItem["ID1C"].ToString()
                  });
              }
              spWeb.AllowUnsafeUpdates = false;
            }
          }
        }));
      }
      catch (Exception ex)
      {
        result.Add(new SharepointItem()
        {
          ID = -1,
          Title = ex.Message,
          ID1C = ex.Message
        });
      }
      return result;
    }

    public static SharepointItem SharepointItem_GetBycode1C(string site_url, string web_url, string list_url, string ID)
    {
      SharepointItem result = new SharepointItem();
      try
      {
        SPSecurity.RunWithElevatedPrivileges((SPSecurity.CodeToRunElevated) (() =>
        {
          using (SPSite spSite = new SPSite(site_url))
          {
            using (SPWeb spWeb = spSite.OpenWeb(web_url))
            {
              spWeb.AllowUnsafeUpdates = true;
              SPListItemCollection items = spWeb.GetList(list_url).GetItems(new SPQuery()
              {
                Query = "<OrderBy>" + "<FieldRef Name='ID' Ascending='FALSE' />" + "</OrderBy>" + "<Where>" + "<Eq>" + "<FieldRef Name='ID1C' />" + "<Value Type='Text'>" + ID + "</Value>" + "</Eq>" + "</Where>"
              });
              if (items.Count > 0)
              {
                foreach (SPListItem spListItem in (SPBaseCollection) items)
                  result = new SharepointItem()
                  {
                    ID = spListItem.ID,
                    Title = spListItem["Title"] == null ? "" : spListItem["Title"].ToString(),
                    ID1C = spListItem["ID1C"] == null ? "" : spListItem["ID1C"].ToString()
                  };
              }
              spWeb.AllowUnsafeUpdates = false;
            }
          }
        }));
      }
      catch (Exception ex)
      {
        result = new SharepointItem()
        {
          ID = -1,
          Title = ex.Message,
          ID1C = ex.Message
        };
      }
      return result;
    }

    public static int LookUpID_Get(SPListItem item, string fieldName)
    {
      int num = 0;
      try
      {
        if (item[fieldName] != null && !string.IsNullOrEmpty(item[fieldName].ToString()))
          num = new SPFieldLookupValue(item[fieldName].ToString()).LookupId;
      }
      catch (Exception ex)
      {
        num = -1;
      }
      return num;
    }

    public static string LookUpValue_Get(SPListItem item, string fieldName)
    {
      string str = "";
      try
      {
        if (item[fieldName] != null && !string.IsNullOrEmpty(item[fieldName].ToString()))
          str = new SPFieldLookupValue(item[fieldName].ToString()).LookupValue;
      }
      catch (Exception ex)
      {
        str = ex.Message;
      }
      return str;
    }

    public static string DateTimeField_Get(SPWeb web, SPListItem item, string fieldName)
    {
      string str = "";
      try
      {
        if (item[fieldName] != null && !string.IsNullOrEmpty(item[fieldName].ToString()))
          str = SPUtility.FormatDate(web, DateTime.Parse(item[fieldName].ToString()), SPDateFormat.DateTime);
      }
      catch (Exception ex)
      {
        str = ex.Message;
      }
      return str;
    }

    public static bool BoolField_Get(SPWeb web, SPListItem item, string fieldName)
    {
      bool flag = false;
      try
      {
        if (item[fieldName] != null && !string.IsNullOrEmpty(item[fieldName].ToString()))
          flag = (bool) (item.Fields.GetFieldByInternalName(fieldName) as SPFieldBoolean).GetFieldValue(item[fieldName].ToString());
      }
      catch (Exception ex)
      {
        flag = false;
      }
      return flag;
    }

    public static string UrlField_Get(SPListItem item, string fieldName)
    {
      string str = "";
      try
      {
        if (item[fieldName] != null && !string.IsNullOrEmpty(item[fieldName].ToString()))
          str = new SPFieldUrlValue(item[fieldName].ToString()).Url;
      }
      catch (Exception ex)
      {
        str = ex.Message;
      }
      return str;
    }

    public static List<User1C> Users1C_Get(string site_url, string web_url, string list_url, string list_department_url, string code, string department_code, string login)
    {
      List<User1C> result = new List<User1C>();
      login = login.Replace("@@@", "\\");
      try
      {
        SPSecurity.RunWithElevatedPrivileges((SPSecurity.CodeToRunElevated) (() =>
        {
          using (SPSite spSite = new SPSite(site_url))
          {
            using (SPWeb spWeb = spSite.OpenWeb(web_url))
            {
              spWeb.AllowUnsafeUpdates = true;
              SPList list1 = spWeb.GetList(list_url);
              SPList list2 = spWeb.GetList(list_department_url);
              login = login.Contains("\\") ? "i:0#.w|" + login : login;
              spWeb.EnsureUser(login);
              SPListItem itemById = list2.GetItemById(int.Parse(department_code));
              SPListItemCollection items = list1.GetItems(new SPQuery()
              {
                Query = "<OrderBy>" + "<FieldRef Name='ID' Ascending='FALSE' />" + "</OrderBy>" + "<Where>" + "<And>" + "<Contains>" + "<FieldRef Name='Title' />" + "<Value Type='Text'>" + code + "</Value>" + "</Contains>" + "<Eq>" + "<FieldRef Name='IDDepartment1C' />" + "<Value Type='Text'>" + (itemById["ID1C"] == null ? "" : itemById["ID1C"].ToString()) + "</Value>" + "</Eq>" + "</And>" + "</Where>",
                RowLimit = 10U
              });
              if (items.Count > 0)
              {
                foreach (SPListItem spListItem in (SPBaseCollection) items)
                {
                  if (!string.IsNullOrEmpty(spListItem["ID1C"] == null ? "" : spListItem["ID1C"].ToString()))
                    result.Add(new User1C()
                    {
                      ID1C = spListItem["ID1C"].ToString(),
                      FullName = spListItem["Title"] == null ? "" : spListItem["Title"].ToString()
                    });
                }
              }
              spWeb.AllowUnsafeUpdates = false;
            }
          }
        }));
      }
      catch (Exception ex)
      {
        result.Add(new User1C()
        {
          ID1C = ex.Message,
          FullName = ex.Message
        });
      }
      return result;
    }

    public static CalculationZP CalculationZP_Get(string site_url, string web_url, string list_url, int ID)
    {
      CalculationZP result = new CalculationZP();
      try
      {
        SPSecurity.RunWithElevatedPrivileges((SPSecurity.CodeToRunElevated) (() =>
        {
          using (SPSite spSite = new SPSite(site_url))
          {
            using (SPWeb spWeb = spSite.OpenWeb(web_url))
            {
              spWeb.AllowUnsafeUpdates = true;
              SPListItemCollection items = spWeb.GetList(list_url).GetItems(new SPQuery()
              {
                Query = "<OrderBy>" + "<FieldRef Name='ID' />" + "</OrderBy>" + "<Where>" + "<Eq>" + "<FieldRef Name='ID' />" + "<Value Type='Number'>" + ID.ToString() + "</Value>" + "</Eq>" + "</Where>"
              });
              if (items.Count > 0)
              {
                foreach (SPListItem spListItem in (SPBaseCollection) items)
                  result = new CalculationZP()
                  {
                    ID = ID,
                    Title = spListItem["Title"] == null ? "" : spListItem["Title"].ToString(),
                    DepartmentID = BusinessLogic.LookUpID_Get(spListItem, "Department1"),
                    Comment = spListItem["Comment1"] == null ? "" : spListItem["Comment1"].ToString(),
                    TypeZP = spListItem["TypeZP"] == null ? "" : spListItem["TypeZP"].ToString(),
                    Department = BusinessLogic.LookUpValue_Get(spListItem, "Department1"),
                    Status = BusinessLogic.LookUpValue_Get(spListItem, "Status"),
                    ResponsibleID = BusinessLogic.LookUpID_Get(spListItem, "Responsible"),
                    StatusID = BusinessLogic.LookUpID_Get(spListItem, "Status"),
                    Responsible = BusinessLogic.LookUpValue_Get(spListItem, "Responsible"),
                    UnitID = BusinessLogic.LookUpID_Get(spListItem, "CurrencyUnit"),
                    UnitName = BusinessLogic.LookUpValue_Get(spListItem, "CurrencyUnit"),
                    Month = spListItem["Month"] == null ? "" : spListItem["Month"].ToString(),
                    ID1C = spListItem["ID1C"] == null ? "" : spListItem["ID1C"].ToString()
                  };
              }
              spWeb.AllowUnsafeUpdates = false;
            }
          }
        }));
      }
      catch (Exception ex)
      {
        result = new CalculationZP()
        {
          ID = -1,
          Title = ex.Message,
          DepartmentID = -1,
          Comment = ex.Message,
          Department = ex.Message,
          Status = ex.Message,
          ResponsibleID = -1,
          StatusID = -1,
          Responsible = ex.Message
        };
      }
      return result;
    }

    public static ContactSharepoint ContactSharepoint_GetByID(string site_url, string web_url, string list_url, string ID)
    {
      ContactSharepoint result = new ContactSharepoint();
      try
      {
        SPSecurity.RunWithElevatedPrivileges((SPSecurity.CodeToRunElevated) (() =>
        {
          using (SPSite spSite = new SPSite(site_url))
          {
            using (SPWeb web = spSite.OpenWeb(web_url))
            {
              web.AllowUnsafeUpdates = true;
              SPList list = web.GetList(list_url);
              SPQuery query = new SPQuery();
              string loginName = web.AllUsers.GetByID(int.Parse(ID)).LoginName;
              if (loginName.Split('\\').Length > 1)
                loginName = loginName.Split('\\')[1];
              query.Query = "<OrderBy>" + "<FieldRef Name='ID' Ascending='FALSE' />" + "</OrderBy>" + "<Where>" + "<Eq>" + "<FieldRef Name='ADLogin' />" + "<Value Type='Text'>" + loginName + "</Value>" + "</Eq>" + "</Where>";
              query.RowLimit = 1U;
              SPListItemCollection items = list.GetItems(query);
              if (items.Count > 0)
              {
                foreach (SPListItem spListItem in (SPBaseCollection) items)
                  result = new ContactSharepoint()
                  {
                    ID = int.Parse(spListItem["ID"].ToString()),
                    LastName = spListItem["Title"] == null ? "" : spListItem["Title"].ToString(),
                    FirstName = spListItem["FirstName"] == null ? "" : spListItem["FirstName"].ToString(),
                    FullName = spListItem["FullName"] == null ? "" : spListItem["FullName"].ToString(),
                    Email = spListItem["Email"] == null ? "" : spListItem["Email"].ToString(),
                    JobTitle = spListItem["JobTitle"] == null ? "" : spListItem["JobTitle"].ToString(),
                    HomePhone = spListItem["HomePhone"] == null ? "" : spListItem["HomePhone"].ToString(),
                    WorkPhone = spListItem["WorkPhone"] == null ? "" : spListItem["WorkPhone"].ToString(),
                    CellPhone = spListItem["CellPhone"] == null ? "" : spListItem["CellPhone"].ToString(),
                    WorkFax = spListItem["WorkFax"] == null ? "" : spListItem["WorkFax"].ToString(),
                    WorkAddress = spListItem["WorkAddress"] == null ? "" : spListItem["WorkAddress"].ToString(),
                    WorkCity = spListItem["WorkCity"] == null ? "" : spListItem["WorkCity"].ToString(),
                    WorkState = spListItem["WorkState"] == null ? "" : spListItem["WorkState"].ToString(),
                    WorkZip = spListItem["WorkZip"] == null ? "" : spListItem["WorkZip"].ToString(),
                    WorkCountry = spListItem["WorkCountry"] == null ? "" : spListItem["WorkCountry"].ToString(),
                    WebPage = spListItem["WebPage"] == null ? "" : spListItem["WebPage"].ToString(),
                    Comments = spListItem["Comments"] == null ? "" : spListItem["Comments"].ToString(),
                    ID1C = spListItem["ID1C"] == null ? "" : spListItem["ID1C"].ToString(),
                    FullNameAD = spListItem["FullNameAD"] == null ? "" : spListItem["FullNameAD"].ToString(),
                    ADLogin = spListItem["ADLogin"] == null ? "" : spListItem["ADLogin"].ToString(),
                    InnerPhone = spListItem["InnerPhone"] == null ? "" : spListItem["InnerPhone"].ToString(),
                    BirthDay = BusinessLogic.DateTimeField_Get(web, spListItem, "BirthDay"),
                    Department = BusinessLogic.LookUpValue_Get(spListItem, "Department1"),
                    Fired = BusinessLogic.BoolField_Get(web, spListItem, "Fired") ? "Да" : "Нет",
                    Photo = BusinessLogic.UrlField_Get(spListItem, "Photo"),
                    WorkPhone2 = spListItem["WorkPhone2"] == null ? "" : spListItem["WorkPhone2"].ToString(),
                    Skype = spListItem["Skype"] == null ? "" : spListItem["Skype"].ToString(),
                    TabNumber = spListItem["TabNumber"] == null ? "" : spListItem["TabNumber"].ToString(),
                    FunctionalHead = BusinessLogic.LookUpValue_Get(spListItem, "FunctionalHead"),
                    Manager = BusinessLogic.LookUpValue_Get(spListItem, "Manager")
                  };
              }
              else
                result = new ContactSharepoint()
                {
                  ID = -1,
                  LastName = "",
                  FirstName = "",
                  FullName = "",
                  Email = "",
                  JobTitle = "",
                  HomePhone = "",
                  WorkPhone = "",
                  CellPhone = "",
                  WorkFax = "",
                  WorkAddress = "",
                  WorkCity = "",
                  WorkState = "",
                  WorkZip = "",
                  WorkCountry = "",
                  WebPage = "",
                  Comments = "",
                  ID1C = "",
                  FullNameAD = "",
                  ADLogin = "",
                  InnerPhone = "",
                  BirthDay = "",
                  Department = "",
                  Fired = "",
                  Photo = "",
                  WorkPhone2 = "",
                  Skype = "",
                  TabNumber = "",
                  FunctionalHead = "",
                  Manager = ""
                };
              web.AllowUnsafeUpdates = false;
            }
          }
        }));
      }
      catch (Exception ex)
      {
        result = new ContactSharepoint()
        {
          ID = -1,
          LastName = ex.Message,
          FirstName = ex.Message,
          FullName = ex.Message,
          Email = ex.Message,
          JobTitle = ex.Message,
          HomePhone = ex.Message,
          WorkPhone = ex.Message,
          CellPhone = ex.Message,
          WorkFax = ex.Message,
          WorkAddress = ex.Message,
          WorkCity = ex.Message,
          WorkState = ex.Message,
          WorkZip = ex.Message,
          WorkCountry = ex.Message,
          WebPage = ex.Message,
          Comments = ex.Message,
          ID1C = ex.Message,
          FullNameAD = ex.Message,
          ADLogin = ex.Message,
          InnerPhone = ex.Message,
          BirthDay = ex.Message,
          Department = ex.Message,
          Fired = ex.Message,
          Photo = ex.Message,
          WorkPhone2 = ex.Message,
          Skype = ex.Message,
          TabNumber = ex.Message,
          FunctionalHead = ex.Message,
          Manager = ex.Message
        };
      }
      return result;
    }

    public static List<SharepointItem> SharepointItems_GetByLogin(string site_url, string web_url, string list_url, string login)
    {
      List<SharepointItem> result = new List<SharepointItem>();
      try
      {
        SPSecurity.RunWithElevatedPrivileges((SPSecurity.CodeToRunElevated) (() =>
        {
          using (SPSite spSite = new SPSite(site_url))
          {
            using (SPWeb spWeb = spSite.OpenWeb(web_url))
            {
              spWeb.AllowUnsafeUpdates = true;
              login = login.Contains("\\") ? "i:0#.w|" + login : login;
              SPUser user = spWeb.EnsureUser(login);
              SPListItemCollection items = spWeb.GetList(list_url).GetItems(new SPQuery()
              {
                Query = "<OrderBy>" + "<FieldRef Name='ID' Ascending='FALSE' />" + "</OrderBy>"
              });
              if (items.Count > 0)
              {
                foreach (SPListItem spListItem in (SPBaseCollection) items)
                {
                  if (spListItem.DoesUserHavePermissions(user, SPBasePermissions.EditListItems))
                    result.Add(new SharepointItem()
                    {
                      ID = spListItem.ID,
                      Title = spListItem["Title"] == null ? "" : spListItem["Title"].ToString(),
                      ID1C = spListItem["ID1C"] == null ? "" : spListItem["ID1C"].ToString()
                    });
                }
              }
              spWeb.AllowUnsafeUpdates = false;
            }
          }
        }));
      }
      catch (Exception ex)
      {
        result.Add(new SharepointItem()
        {
          ID = -1,
          Title = ex.Message,
          ID1C = ex.Message
        });
      }
      return result;
    }

    public static void UpdatePermissionsList(Guid siteID, Guid webID, Guid listID, string name)
    {
      try
      {
        SPSecurity.RunWithElevatedPrivileges((SPSecurity.CodeToRunElevated) (() =>
        {
          using (SPSite spSite = new SPSite(siteID))
          {
            using (SPWeb rootWeb = spSite.RootWeb)
            {
              using (SPWeb spWeb = spSite.OpenWeb(webID))
              {
                spWeb.AllowUnsafeUpdates = true;
                rootWeb.AllowUnsafeUpdates = true;
                SPList list = spWeb.Lists[listID];
                list.BreakRoleInheritance(true);
                SPRoleAssignmentCollection roleAssignments = list.RoleAssignments;
                SPRoleDefinition byType1 = spWeb.RoleDefinitions.GetByType(SPRoleType.Contributor);
                spWeb.RoleDefinitions.GetByType(SPRoleType.WebDesigner);
                SPRoleDefinition byType2 = spWeb.RoleDefinitions.GetByType(SPRoleType.Reader);
                SPRoleDefinition byType3 = spWeb.RoleDefinitions.GetByType(SPRoleType.Administrator);
                for (int index = list.RoleAssignments.Count - 1; index >= 0; --index)
                {
                  if (list.RoleAssignments[index] != null)
                  {
                    SPRoleDefinitionBindingCollection definitionBindings = list.RoleAssignments[index].RoleDefinitionBindings;
                    if (!list.RoleAssignments[index].RoleDefinitionBindings.Contains(byType3))
                    {
                      list.RoleAssignments[index].RoleDefinitionBindings.RemoveAll();
                      list.RoleAssignments[index].RoleDefinitionBindings.Add(byType2);
                      list.RoleAssignments[index].Update();
                    }
                  }
                }
                foreach (SPGroup group in (SPBaseCollection) rootWeb.Groups)
                {
                  if (group.Name.Contains(name))
                    list.RoleAssignments.Add(new SPRoleAssignment((SPPrincipal) group)
                    {
                      RoleDefinitionBindings = {
                        byType1
                      }
                    });
                }
                list.Update();
                spWeb.AllowUnsafeUpdates = false;
              }
            }
          }
        }));
      }
      catch
      {
      }
      finally
      {
      }
    }

    public static List<ApproveTableItem> ApproveTableItems_Get(string site_url, string web_url, int statusID, string list_url, string WorkflowID)
    {
      List<ApproveTableItem> result = new List<ApproveTableItem>();
      try
      {
        SPSecurity.RunWithElevatedPrivileges((SPSecurity.CodeToRunElevated) (() =>
        {
          using (SPSite spSite = new SPSite(site_url))
          {
            using (SPWeb web = spSite.OpenWeb(web_url))
            {
              web.AllowUnsafeUpdates = true;
              SPListItemCollection items = web.GetList(list_url).GetItems(new SPQuery()
              {
                Query = "<OrderBy>" + "<FieldRef Name='ID' Ascending='FALSE' />" + "</OrderBy>" + "<Where>" + "<And>" + "<And>" + "<Eq>" + "<FieldRef Name='ListUrl' />" + "<Value Type='Text'>" + list_url + "</Value>" + "</Eq>" + "<Eq>" + "<FieldRef Name='ApproveID' />" + "<Value Type='Text'>" + WorkflowID + "</Value>" + "</Eq>" + "</And>" + "<Eq>" + "<FieldRef Name='Status'  LookupId='True' />" + "<Value Type='Lookup'>" + statusID.ToString() + "</Value>" + "</Eq>" + "</And>" + "</Where>"
              });
              if (items.Count > 0)
              {
                foreach (SPListItem spListItem in (SPBaseCollection) items)
                {
                  SPPrincipal spPrincipal = (SPPrincipal) BusinessLogic.GetSPGroup(spListItem, "Approver", web) ?? (SPPrincipal) BusinessLogic.GetSPUser(spListItem, "Approver");
                  result.Add(new ApproveTableItem()
                  {
                    ID = spListItem.ID,
                    Workflow = spListItem["Workflow"] == null ? "" : spListItem["Workflow"].ToString(),
                    WorkflowID = spListItem["WorkflowID"] == null ? "" : spListItem["WorkflowID"].ToString(),
                    StatusID = BusinessLogic.LookUpID_Get(spListItem, "Status"),
                    Status = BusinessLogic.LookUpValue_Get(spListItem, "Status"),
                    List = spListItem["ListWorkflow"] == null ? "" : spListItem["ListWorkflow"].ToString(),
                    ListUrl = spListItem["ListUrl"] == null ? "" : spListItem["ListUrl"].ToString(),
                    Approver = spPrincipal
                  });
                }
              }
              web.AllowUnsafeUpdates = false;
            }
          }
        }));
      }
      catch (Exception ex)
      {
        result.Add(new ApproveTableItem()
        {
          Workflow = ex.Message,
          WorkflowID = ex.Message,
          List = ex.Message,
          ListUrl = ex.Message,
          Status = ex.Message,
          StatusID = -1,
          ID = -1
        });
      }
      return result;
    }

    public static SPMember Approver_Get(string site_url, string web_url, int statusID, string list_url, string WorkflowID, SPUser author)
    {
      SPMember result = (SPMember) author;
      try
      {
        SPSecurity.RunWithElevatedPrivileges((SPSecurity.CodeToRunElevated) (() =>
        {
          using (SPSite spSite = new SPSite(site_url))
          {
            using (SPWeb web = spSite.OpenWeb(web_url))
            {
              web.AllowUnsafeUpdates = true;
              SPListItemCollection items = web.GetList(list_url).GetItems(new SPQuery()
              {
                Query = "<OrderBy>" + "<FieldRef Name='ID' Ascending='FALSE' />" + "</OrderBy>" + "<Where>" + "<And>" + "<And>" + "<Eq>" + "<FieldRef Name='ListUrl' />" + "<Value Type='Text'>" + list_url + "</Value>" + "</Eq>" + "<Eq>" + "<FieldRef Name='ApproveID' />" + "<Value Type='Text'>" + WorkflowID + "</Value>" + "</Eq>" + "</And>" + "<Eq>" + "<FieldRef Name='Status'  LookupId='True' />" + "<Value Type='Lookup'>" + statusID.ToString() + "</Value>" + "</Eq>" + "</And>" + "</Where>"
              });
              if (items.Count > 0)
              {
                foreach (SPListItem spListItem in (SPBaseCollection) items)
                  result = (SPMember) ((SPPrincipal) BusinessLogic.GetSPGroup(spListItem, "Approver", web) ?? (SPPrincipal) BusinessLogic.GetSPUser(spListItem, "Approver"));
              }
              web.AllowUnsafeUpdates = false;
            }
          }
        }));
      }
      catch (Exception ex)
      {
      }
      return result;
    }

    public static List<ListInfo> ListsInfo_Get(string site_url, string web_url)
    {
      List<ListInfo> result = new List<ListInfo>();
      try
      {
        SPSecurity.RunWithElevatedPrivileges((SPSecurity.CodeToRunElevated) (() =>
        {
          using (SPSite spSite = new SPSite(site_url))
          {
            using (SPWeb spWeb = spSite.OpenWeb(web_url))
            {
              spWeb.AllowUnsafeUpdates = true;
              result = spWeb.Lists.OfType<SPList>().Select<SPList, ListInfo>((Func<SPList, ListInfo>) (t => new ListInfo()
              {
                Title = t.Title,
                Url = t.RootFolder.ServerRelativeUrl,
                ID = t.ID.ToString()
              })).ToList<ListInfo>();
              spWeb.AllowUnsafeUpdates = false;
            }
          }
        }));
      }
      catch (Exception ex)
      {
        result.Add(new ListInfo()
        {
          ID = ex.Message,
          Title = ex.Message,
          Url = ex.Message
        });
      }
      return result;
    }

    //public static List<WorkflowInfo> Workflows_Get(string site_url, string web_url, string list_id)
    //{
    //  List<WorkflowInfo> result = new List<WorkflowInfo>();
    //  try
    //  {
    //    SPSecurity.RunWithElevatedPrivileges((SPSecurity.CodeToRunElevated) (() =>
    //    {
    //      using (SPSite spSite = new SPSite(site_url))
    //      {
    //        using (SPWeb web = spSite.OpenWeb(web_url))
    //        {
    //          web.AllowUnsafeUpdates = true;
    //          SPList list = web.Lists[new Guid(list_id)];
    //          SPWorkflowManager workflowManager = spSite.WorkflowManager;
    //          result = new WorkflowServicesManager(web).GetWorkflowSubscriptionService().EnumerateSubscriptionsByList(list.ID).OfType<WorkflowSubscription>().Select<WorkflowSubscription, WorkflowInfo>((Func<WorkflowSubscription, WorkflowInfo>) (t => new WorkflowInfo()
    //          {
    //            Title = t.Name,
    //            ListID = list_id,
    //            ID = t.Id.ToString()
    //          })).ToList<WorkflowInfo>();
    //          web.AllowUnsafeUpdates = false;
    //        }
    //      }
    //    }));
    //  }
    //  catch (Exception ex)
    //  {
    //    result.Add(new WorkflowInfo()
    //    {
    //      ID = ex.Message,
    //      Title = ex.Message,
    //      ListID = ex.Message
    //    });
    //  }
    //  return result;
    //}

    public static List<OrgUser> OrgUsers_Get(string site_url, string web_url, string list_url, string list_department_url, int departmentID)
    {
      List<OrgUser> result = new List<OrgUser>();
      try
      {
        SPSecurity.RunWithElevatedPrivileges((SPSecurity.CodeToRunElevated) (() =>
        {
          using (SPSite spSite = new SPSite(site_url))
          {
            using (SPWeb spWeb = spSite.OpenWeb(web_url))
            {
              spWeb.AllowUnsafeUpdates = true;
              SPList list = spWeb.GetList(list_url);
              SPListItem itemById = spWeb.GetList(list_department_url).Items.GetItemById(departmentID);
              SPListItemCollection items = list.GetItems(new SPQuery()
              {
                Query = "<OrderBy>" + "<FieldRef Name='ID' />" + "</OrderBy>" + "<Where>" + "<And>" + "<Eq>" + "<FieldRef Name='IsFired' />" + "<Value Type='Boolean'>0</Value>" + "</Eq>" + "<Eq>" + "<FieldRef Name='IDDepartment1C' />" + "<Value Type='Text'>" + (itemById["ID1C"] == null ? "" : itemById["ID1C"].ToString()) + "</Value>" + "</Eq>" + "</And>" + "</Where>"
              });
              if (items.Count > 0)
              {
                foreach (SPListItem spListItem in (SPBaseCollection) items)
                  result.Add(new OrgUser()
                  {
                    ID = spListItem["ID1C"] == null ? "" : spListItem["ID1C"].ToString(),
                    Name = spListItem["Title"] == null ? "" : spListItem["Title"].ToString()
                  });
              }
              spWeb.AllowUnsafeUpdates = false;
            }
          }
        }));
      }
      catch (Exception ex)
      {
        result = new List<OrgUser>()
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
      return result;
    }

    public static SPListItem GetRelatedItem(SPWeb web, string relatedItems, out Guid listID)
    {
      SPListItem spListItem = (SPListItem) null;
      try
      {
        relatedItems = relatedItems.Trim('[', '{', '}', ']');
        string[] strArray1 = relatedItems.Split(',');
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        foreach (string str in strArray1)
        {
          char[] chArray = new char[1]{ ':' };
          string[] strArray2 = str.Split(chArray);
          dictionary[strArray2[0].Trim('"')] = strArray2[1].Trim('"');
        }
        string s = dictionary["ItemId"];
        string g = dictionary["ListId"];
        listID = new Guid(g);
        spListItem = web.Lists[listID].GetItemById(int.Parse(s));
      }
      catch (Exception ex)
      {
        listID = new Guid();
      }
      return spListItem;
    }

    public static List<SharepointItem> SharepointDepartments_GetByLogin(string site_url, string web_url, string list_url, SPUser user)
    {
      List<SharepointItem> sharepointItemList = new List<SharepointItem>();
      try
      {
        using (SPSite spSite = new SPSite(site_url))
        {
          using (SPWeb spWeb = spSite.OpenWeb(web_url))
          {
            SPList list = spWeb.GetList(list_url);
            string str = BusinessLogic.CalmQueryGroups(user.Groups.OfType<SPGroup>().Select<SPGroup, GroupClass>((Func<SPGroup, GroupClass>) (t => new GroupClass()
            {
              ID = t.ID,
              Name = t.Name
            })).ToList<GroupClass>(), "GroupAccess");
            SPListItemCollection items = list.GetItems(new SPQuery()
            {
              Query = "<OrderBy>" + "<FieldRef Name='Title' />" + "</OrderBy>" + "<Where>" + (string.IsNullOrEmpty(str) ? "" : "<Or>" + str) + "<Includes>" + "<FieldRef Name=\"GroupAccess\" LookupId='TRUE' />" + "<Value Type='Integer' >" + user.ID.ToString() + "</Value>" + "</Includes>" + (string.IsNullOrEmpty(str) ? "" : "</Or>") + "</Where>"
            });
            if (items.Count > 0)
            {
              for (int index = 0; index < items.Count; ++index)
              {
                SPListItem spListItem = items[index];
                sharepointItemList.Add(new SharepointItem()
                {
                  ID = spListItem.ID,
                  Title = spListItem["Title"] == null ? "" : spListItem["Title"].ToString(),
                  ID1C = spListItem["ID1C"] == null ? "" : spListItem["ID1C"].ToString()
                });
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        sharepointItemList.Add(new SharepointItem()
        {
          ID = -1,
          Title = ex.Message,
          ID1C = ex.Message
        });
      }
      return sharepointItemList;
    }

    public static List<EmployeeItem> EmployeeItems_Get(string siteUrl, string webUrl, string ID, string typeZP)
    {
      List<EmployeeItem> result = new List<EmployeeItem>();
      try
      {
        SPSecurity.RunWithElevatedPrivileges((SPSecurity.CodeToRunElevated) (() =>
        {
          using (SPSite spSite = new SPSite(siteUrl))
          {
            using (SPWeb spWeb = spSite.OpenWeb(webUrl))
            {
              spWeb.AllowUnsafeUpdates = true;
              SPList list1 = spWeb.GetList("Lists/PayRollPossitionList");
              SPList list2 = spWeb.GetList("Lists/RetentionRollPossitionList");
              SPList list3 = spWeb.GetList("Lists/UsersList");
              int num1 = typeZP == "Начисление" ? 1 : 2;
              SPQuery query = new SPQuery();
              query.Query = "<OrderBy>" + "<FieldRef Name='ID' />" + "</OrderBy>" + "<Where>" + "<And>" + "<Eq>" + "<FieldRef Name='PayRollID' LookupId='True' />" + "<Value Type='Lookup'>" + ID.ToString() + "</Value>" + "</Eq>" + "<Eq>" + "<FieldRef Name='IsActive1' />" + "<Value Type='Boolean'>1</Value>" + "</Eq>" + "</And>" + "</Where>";
              SPListItemCollection listItemCollection = num1 == 1 ? list1.GetItems(query) : list2.GetItems(query);
              string str = "";
              if (listItemCollection.Count > 0)
              {
                for (int index = 0; index < listItemCollection.Count; ++index)
                {
                  SPListItem spListItem = listItemCollection[index];
                  int num2 = BusinessLogic.LookUpID_Get(spListItem, "Employee");
                  result.Add(new EmployeeItem()
                  {
                    CalculationTypeID = BusinessLogic.LookUpID_Get(spListItem, "CalculationKind").ToString(),
                    CalculationType = BusinessLogic.LookUpValue_Get(spListItem, "CalculationKind").ToString(),
                    empID = BusinessLogic.LookUpID_Get(spListItem, "Employee"),
                    PayRollID = ID,
                    EmployeeName = BusinessLogic.LookUpValue_Get(spListItem, "Employee"),
                    Total = BusinessLogic.GetDouble(spListItem["TotalTo"] == null ? "0" : spListItem["TotalTo"].ToString()).ToString()
                  });
                  str = (!string.IsNullOrEmpty(str) ? "<Or>" + str : "") + "<Eq>" + "<FieldRef Name='ID' />" + "<Value Type='Number'>" + (object) num2 + "</Value>" + "</Eq>" + (!string.IsNullOrEmpty(str) ? "</Or>" : "");
                }
              }
              SPListItemCollection items = list3.GetItems(new SPQuery()
              {
                Query = "<OrderBy>" + "<FieldRef Name='ID' />" + "</OrderBy>" + "<Where>" + str + "</Where>"
              });
              if (items.Count > 0)
              {
                for (int index = 0; index < items.Count; ++index)
                {
                  SPListItem item = items[index];
                  try
                  {
                    foreach (EmployeeItem employeeItem in result.Where<EmployeeItem>((Func<EmployeeItem, bool>) (t => t.empID == item.ID)).ToList<EmployeeItem>())
                    {
                      if (employeeItem != null)
                        employeeItem.EmployeeID = item["ID1C"] == null ? "" : item["ID1C"].ToString();
                    }
                  }
                  catch (Exception ex)
                  {
                  }
                }
              }
              spWeb.AllowUnsafeUpdates = false;
            }
          }
        }));
      }
      catch (Exception ex)
      {
        result.Add(new EmployeeItem()
        {
          CalculationTypeID = "0",
          EmployeeID = "0",
          PayRollID = "0",
          EmployeeName = ex.Message,
          Total = "0"
        });
      }
      return result;
    }

    public static string GetMonth(string title)
    {
      string str;
      try
      {
        string[] strArray = title.Split('.');
        str = new Dictionary<string, string>()
        {
          {
            "01",
            "Январь"
          },
          {
            "02",
            "Февраль"
          },
          {
            "03",
            "Март"
          },
          {
            "04",
            "Апрель"
          },
          {
            "05",
            "Май"
          },
          {
            "06",
            "Июнь"
          },
          {
            "07",
            "Июль"
          },
          {
            "08",
            "Август"
          },
          {
            "09",
            "Сентябрь"
          },
          {
            "10",
            "Октябрь"
          },
          {
            "11",
            "Ноябрь"
          },
          {
            "12",
            "Декабрь"
          }
        }[strArray[1]] + " " + strArray[2];
      }
      catch (Exception ex)
      {
        str = ex.Message;
      }
      return str;
    }

    public static List<SPPrincipal> MultiUsers_GetSPPrinsipials(SPWeb web, SPListItem item, string fieldName)
    {
      List<SPPrincipal> spPrincipalList = new List<SPPrincipal>();
      try
      {
        foreach (SPFieldUserValue spFieldUserValue in (List<SPFieldUserValue>) item[fieldName])
        {
          if (spFieldUserValue.User != null)
            spPrincipalList.Add((SPPrincipal) spFieldUserValue.User);
          else
            spPrincipalList.Add((SPPrincipal) web.SiteGroups.GetByID(spFieldUserValue.LookupId));
        }
      }
      catch (Exception ex)
      {
      }
      return spPrincipalList;
    }

    public static string CalmQueryContains(List<string> valuesList, string fieldName)
    {
      string str1 = "";
      try
      {
        if (valuesList.Count > 0)
        {
          foreach (string values in valuesList)
          {
            string str2;
            if (!string.IsNullOrEmpty(str1))
              str2 = "<Or>" + str1 + "<Contains>" + "<FieldRef Name=\"" + fieldName + "\" />" + "<Value Type=\"Text\" >" + values + "</Value>" + "</Contains" + "</Or>";
            else
              str2 = "<Contains>" + "<FieldRef Name=\"" + fieldName + "\" />" + "<Value Type=\"Text\" >" + values + "</Value>" + "</Contains";
            str1 = str2;
          }
        }
      }
      catch (Exception ex)
      {
        str1 = ex.Message;
      }
      return str1;
    }

    public static string CalmQueryGroups(List<GroupClass> groupList, string fieldName)
    {
      string str1 = "";
      try
      {
        if (groupList.Count > 0)
        {
          foreach (GroupClass group in groupList)
          {
            string str2;
            if (!string.IsNullOrEmpty(str1))
              str2 = "<Or>" + str1 + "<Includes>" + "<FieldRef Name='" + fieldName + "' LookupId='TRUE' />" + "<Value Type='Integer' >" + group.ID.ToString() + "</Value>" + "</Includes>" + "</Or>";
            else
              str2 = "<Includes>" + "<FieldRef Name='" + fieldName + "' LookupId='TRUE' />" + "<Value Type='Integer' >" + group.ID.ToString() + "</Value>" + "</Includes>";
            str1 = str2;
          }
        }
      }
      catch (Exception ex)
      {
        str1 = ex.Message;
      }
      return str1;
    }

    public static string CustomWorkflowEventReceiver(SPListItem item)
    {
      string str1 = "";
      try
      {
        SPSecurity.RunWithElevatedPrivileges((SPSecurity.CodeToRunElevated) (() =>
        {
          using (SPSite spSite = new SPSite(item.Web.Site.ID))
          {
            using (SPWeb web = spSite.OpenWeb(item.Web.ID))
            {
              web.AllowUnsafeUpdates = true;
              SPList list1 = web.GetList(item.ParentList.RootFolder.Url.ToString());
              if (list1.RootFolder.Url.ToLower().Contains("lists/taskslist"))
              {
                string relatedItems = string.Concat(item["RelatedItems"]);
                string str2 = item["CustomApprove"] == null ? "" : item["CustomApprove"].ToString();
                SPList list2 = web.GetList("Lists/ResourceApplicationList");
                Guid listID;
                SPListItem relatedItem = BusinessLogic.GetRelatedItem(web, relatedItems, out listID);
                if (relatedItem != null && listID.ToString() == list2.ID.ToString())
                {
                  if (str2 == "Утвердить")
                  {
                    SPQuery query = new SPQuery();
                    SPQuery spQuery = query;
                    string[] strArray1 = new string[21];
                    strArray1[0] = "<Where>";
                    strArray1[1] = "<And>";
                    strArray1[2] = "<Neq>";
                    strArray1[3] = "<FieldRef Name='Status'/>";
                    strArray1[4] = "<Value Type='Text'>Completed</Value>";
                    strArray1[5] = "</Neq>";
                    strArray1[6] = "<Eq>";
                    strArray1[7] = "<FieldRef Name='RelatedItems'/>";
                    strArray1[8] = "<Value Type='Text'>";
                    int index1 = 9;
                    string str3 = BusinessLogic.RelatedItemsStr(new List<SPListItem>()
                    {
                      relatedItem
                    });
                    strArray1[index1] = str3;
                    int index2 = 10;
                    string str4 = "</Value>";
                    strArray1[index2] = str4;
                    int index3 = 11;
                    string str5 = "</Eq>";
                    strArray1[index3] = str5;
                    int index4 = 12;
                    string str6 = "</And>";
                    strArray1[index4] = str6;
                    int index5 = 13;
                    string str7 = "<Eq>";
                    strArray1[index5] = str7;
                    int index6 = 14;
                    string str8 = "<FieldRef Name='WorkflowKey'/>";
                    strArray1[index6] = str8;
                    int index7 = 15;
                    string str9 = "<Value Type='Text'>";
                    strArray1[index7] = str9;
                    int index8 = 16;
                    string str10 = (relatedItem["WorkflowKey"] == null ? (object) "" : (object) relatedItem["WorkflowKey"].ToString()).ToString() + "@@@" + (object) BusinessLogic.LookUpID_Get(relatedItem, "Status");
                    strArray1[index8] = str10;
                    int index9 = 17;
                    string str11 = "</Value>";
                    strArray1[index9] = str11;
                    int index10 = 18;
                    string str12 = "</Eq>";
                    strArray1[index10] = str12;
                    int index11 = 19;
                    string str13 = "</And>";
                    strArray1[index11] = str13;
                    int index12 = 20;
                    string str14 = "</Where>";
                    strArray1[index12] = str14;
                    string str15 = string.Concat(strArray1);
                    spQuery.Query = str15;
                    if (list1.GetItems(query).Count <= 0)
                    {
                      int num = BusinessLogic.LookUpID_Get(relatedItem, "Status");
                      if (num > 2 && num != 6 && num != 7)
                      {
                        string str16 = relatedItem["Statuses"] == null ? "" : relatedItem["Statuses"].ToString();
                        string str17 = relatedItem["Chain"] == null ? "" : relatedItem["Chain"].ToString();
                        int[] array = ((IEnumerable<string>) str16.Split(';')).Select<string, int>((Func<string, int>) (t => int.Parse(t))).ToArray<int>();
                        string[] strArray2 = str17.Split(';');
                        SPFieldUserValueCollection userValueCollection = new SPFieldUserValueCollection();
                        for (int index13 = 0; index13 < array.Length; ++index13)
                        {
                          if (array[index13] == num)
                          {
                            if (index13 + 1 < array.Length)
                            {
                              item["Status"] = (object) array[index13 + 1];
                              string[] strArray3 = strArray2[index13 + 1].Split(',');
                              string str18 = "";
                              for (int index14 = 0; index14 < strArray3.Length; ++index14)
                              {
                                SPUser byId = spSite.RootWeb.AllUsers.GetByID(int.Parse(strArray3[index14]));
                                str18 = str18 + (string.IsNullOrEmpty(str18) ? "" : ";") + byId.LoginName;
                                SPFieldUserValue spFieldUserValue = new SPFieldUserValue(web, byId.ID, byId.LoginName);
                                userValueCollection.Add(spFieldUserValue);
                                SPListItem spListItem = list1.AddItem();
                                spListItem["Title"] = (object) ("Согласование " + (relatedItem["Title"] == null ? "" : relatedItem["Title"].ToString()));
                                spListItem["ContentTypeId"] = (object) "0x010013b8d166dc2f4a0eb0678cc6c2445c73";
                                spListItem["AssignedTo"] = (object) spSite.RootWeb.AllUsers.GetByID(int.Parse(strArray3[index14]));
                                spListItem["DueDate"] = (object) DateTime.Now.AddDays(10.0);
                                spListItem["RelatedItems"] = (object) BusinessLogic.RelatedItemsStr(new List<SPListItem>()
                                {
                                  relatedItem
                                });
                                spListItem.Update();
                              }
                              SPListItem spListItem1 = BusinessLogic.ReloadListItem(relatedItem);
                              spListItem1["Approver"] = (object) userValueCollection;
                              spListItem1.Update();
                              break;
                            }
                            SPListItem spListItem2 = BusinessLogic.ReloadListItem(relatedItem);
                            spListItem2["Status"] = (object) 6;
                            spListItem2.Update();
                            break;
                          }
                        }
                      }
                    }
                  }
                  else
                  {
                    SPQuery query1 = new SPQuery();
                    SPQuery spQuery1 = query1;
                    string[] strArray1 = new string[22];
                    strArray1[0] = "<Where>";
                    strArray1[1] = "<And>";
                    strArray1[2] = "<And>";
                    strArray1[3] = "<Neq>";
                    strArray1[4] = "<FieldRef Name='Status'/>";
                    strArray1[5] = "<Value Type='Text'>Completed</Value>";
                    strArray1[6] = "</Neq>";
                    strArray1[7] = "<Eq>";
                    strArray1[8] = "<FieldRef Name='RelatedItems'/>";
                    strArray1[9] = "<Value Type='Text'>";
                    int index1 = 10;
                    string str3 = BusinessLogic.RelatedItemsStr(new List<SPListItem>()
                    {
                      relatedItem
                    });
                    strArray1[index1] = str3;
                    int index2 = 11;
                    string str4 = "</Value>";
                    strArray1[index2] = str4;
                    int index3 = 12;
                    string str5 = "</Eq>";
                    strArray1[index3] = str5;
                    int index4 = 13;
                    string str6 = "</And>";
                    strArray1[index4] = str6;
                    int index5 = 14;
                    string str7 = "<Eq>";
                    strArray1[index5] = str7;
                    int index6 = 15;
                    string str8 = "<FieldRef Name='WorkflowKey'/>";
                    strArray1[index6] = str8;
                    int index7 = 16;
                    string str9 = "<Value Type='Text'>";
                    strArray1[index7] = str9;
                    int index8 = 17;
                    string str10 = (relatedItem["WorkflowKey"] == null ? (object) "" : (object) relatedItem["WorkflowKey"].ToString()).ToString() + "@@@" + (object) BusinessLogic.LookUpID_Get(relatedItem, "Status");
                    strArray1[index8] = str10;
                    int index9 = 18;
                    string str11 = "</Value>";
                    strArray1[index9] = str11;
                    int index10 = 19;
                    string str12 = "</Eq>";
                    strArray1[index10] = str12;
                    int index11 = 20;
                    string str13 = "</And>";
                    strArray1[index11] = str13;
                    int index12 = 21;
                    string str14 = "</Where>";
                    strArray1[index12] = str14;
                    string str15 = string.Concat(strArray1);
                    spQuery1.Query = str15;
                    if (list1.GetItems(query1).Count <= 0)
                    {
                      SPQuery query2 = new SPQuery();
                      SPQuery spQuery2 = query2;
                      string[] strArray2 = new string[28];
                      strArray2[0] = "<Where>";
                      strArray2[1] = "<And>";
                      strArray2[2] = "<And>";
                      strArray2[3] = "<And>";
                      strArray2[4] = "<Eq>";
                      strArray2[5] = "<FieldRef Name='Status'/>";
                      strArray2[6] = "<Value Type='Text'>Completed</Value>";
                      strArray2[7] = "</Eq>";
                      strArray2[8] = "<Eq>";
                      strArray2[9] = "<FieldRef Name='RelatedItems'/>";
                      strArray2[10] = "<Value Type='Text'>";
                      int index13 = 11;
                      string str16 = BusinessLogic.RelatedItemsStr(new List<SPListItem>()
                      {
                        relatedItem
                      });
                      strArray2[index13] = str16;
                      int index14 = 12;
                      string str17 = "</Value>";
                      strArray2[index14] = str17;
                      int index15 = 13;
                      string str18 = "</Eq>";
                      strArray2[index15] = str18;
                      int index16 = 14;
                      string str19 = "</And>";
                      strArray2[index16] = str19;
                      int index17 = 15;
                      string str20 = "<Eq>";
                      strArray2[index17] = str20;
                      int index18 = 16;
                      string str21 = "<FieldRef Name='WorkflowKey'/>";
                      strArray2[index18] = str21;
                      int index19 = 17;
                      string str22 = "<Value Type='Text'>";
                      strArray2[index19] = str22;
                      int index20 = 18;
                      string str23 = (relatedItem["WorkflowKey"] == null ? (object) "" : (object) relatedItem["WorkflowKey"].ToString()).ToString() + "@@@" + (object) BusinessLogic.LookUpID_Get(relatedItem, "Status");
                      strArray2[index20] = str23;
                      int index21 = 19;
                      string str24 = "</Value>";
                      strArray2[index21] = str24;
                      int index22 = 20;
                      string str25 = "</Eq>";
                      strArray2[index22] = str25;
                      int index23 = 21;
                      string str26 = "</And>";
                      strArray2[index23] = str26;
                      int index24 = 22;
                      string str27 = "<Eq>";
                      strArray2[index24] = str27;
                      int index25 = 23;
                      string str28 = "<FieldRef Name='CustomApprove'/>";
                      strArray2[index25] = str28;
                      int index26 = 24;
                      string str29 = "<Value Type='Text'>Утвердить</Value>";
                      strArray2[index26] = str29;
                      int index27 = 25;
                      string str30 = "</Eq>";
                      strArray2[index27] = str30;
                      int index28 = 26;
                      string str31 = "</And>";
                      strArray2[index28] = str31;
                      int index29 = 27;
                      string str32 = "</Where>";
                      strArray2[index29] = str32;
                      string str33 = string.Concat(strArray2);
                      spQuery2.Query = str33;
                      if (list1.GetItems(query2).Count <= 0)
                      {
                        SPListItem spListItem = BusinessLogic.ReloadListItem(relatedItem);
                        spListItem["Status"] = (object) 2;
                        spListItem["Apporover"] = (object) null;
                        spListItem.Update();
                      }
                    }
                  }
                }
              }
              web.AllowUnsafeUpdates = false;
            }
          }
        }));
      }
      catch (Exception ex)
      {
        str1 = ex.Message;
      }
      return str1;
    }

    public static string RelatedItemsStr(List<SPListItem> items)
    {
      string str1;
      try
      {
        List<string> stringList1 = new List<string>();
        foreach (SPListItem spListItem in items)
        {
          List<string> stringList2 = stringList1;
          string[] strArray = new string[7]
          {
            "{\"ItemId\":",
            spListItem.ID.ToString(),
            ",\"WebId\":\"",
            null,
            null,
            null,
            null
          };
          int index1 = 3;
          Guid id = spListItem.Web.ID;
          string str2 = id.ToString();
          strArray[index1] = str2;
          int index2 = 4;
          string str3 = "\",\"ListId\":\"";
          strArray[index2] = str3;
          int index3 = 5;
          id = spListItem.ParentList.ID;
          string str4 = id.ToString();
          strArray[index3] = str4;
          int index4 = 6;
          string str5 = "\"}";
          strArray[index4] = str5;
          string str6 = string.Concat(strArray);
          stringList2.Add(str6);
        }
        str1 = "[" + string.Join(",", (IEnumerable<string>) stringList1) + "]";
      }
      catch (Exception ex)
      {
        str1 = ex.Message;
      }
      return str1;
    }

    public static SPListItem ReloadListItem(SPListItem item)
    {
      if (item == null)
        return (SPListItem) null;
      return item.ParentList.GetItemByUniqueId(item.UniqueId);
    }
  }
}
