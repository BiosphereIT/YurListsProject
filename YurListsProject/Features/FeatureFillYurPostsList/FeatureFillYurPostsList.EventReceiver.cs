using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using EmployeeList.Common;

namespace YurListsProject.Features.FeatureFillYurPostsList
{
    /// <summary>
    /// Этот класс обрабатывает события, возникающие в ходе активации, деактивации, установки, удаления и обновления компонентов.
    /// </summary>
    /// <remarks>
    /// GUID, присоединенный к этому классу, может использоваться при создании пакета и не должен изменяться.
    /// </remarks>

    [Guid("6f8ac133-8244-45dd-a691-3d5dc48a2a97")]
    public class FeatureFillYurPostsListEventReceiver : SPFeatureReceiver
    {
        // Раскомментируйте ниже метод для обработки события, возникающего после активации компонента.


        private const string List_JOB_NAME = "UpdateYurPostsListFrom1C";

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            bool administratorAccessDenied = SPWebService.ContentService.RemoteAdministratorAccessDenied;
            try
            {
                SPWebService.ContentService.RemoteAdministratorAccessDenied = false;
                SPSite parent = properties.Feature.Parent as SPSite;
                foreach (SPJobDefinition jobDefinition in (SPPersistedObjectCollection<SPJobDefinition>)parent.WebApplication.JobDefinitions)
                {
                    if (jobDefinition.Name == "UpdateYurPostsListFrom1C")
                        jobDefinition.Delete();
                }
                UpdateYurPostsFrom1C updateYurPostsFrom1C = new UpdateYurPostsFrom1C("UpdateYurPostsListFrom1C", parent.WebApplication);
                updateYurPostsFrom1C.Schedule = (SPSchedule)new SPDailySchedule()
                {
                    BeginHour = 3,
                    BeginMinute = 15,
                    BeginSecond = 15,
                    EndHour = 7,
                    EndMinute = 30,
                    EndSecond = 45
                };
                updateYurPostsFrom1C.Update();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                SPWebService.ContentService.RemoteAdministratorAccessDenied = administratorAccessDenied;
            }
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            bool administratorAccessDenied = SPWebService.ContentService.RemoteAdministratorAccessDenied;
            try
            {
                SPWebService.ContentService.RemoteAdministratorAccessDenied = false;
                foreach (SPJobDefinition jobDefinition in (SPPersistedObjectCollection<SPJobDefinition>)(properties.Feature.Parent as SPSite).WebApplication.JobDefinitions)
                {
                    if (jobDefinition.Name == "UpdateNormGroupListFrom1C")
                        jobDefinition.Delete();
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                SPWebService.ContentService.RemoteAdministratorAccessDenied = administratorAccessDenied;
            }
        }


        // Раскомментируйте ниже метод для обработки события, возникающего после установки компонента.

        //public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        //{
        //}


        // Раскомментируйте ниже метод для обработки события, возникающего перед удалением компонента.

        //public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        //{
        //}

        // Раскомментируйте ниже метод для обработки события, возникающего при обновлении компонента.

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        //{
        //}
    }
}
