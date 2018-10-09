using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using EmployeeList.Common;

namespace YurListsProject.Features.FeatureFillYurDepatmentList
{
    /// <summary>
    /// Этот класс обрабатывает события, возникающие в ходе активации, деактивации, установки, удаления и обновления компонентов.
    /// </summary>
    /// <remarks>
    /// GUID, присоединенный к этому классу, может использоваться при создании пакета и не должен изменяться.
    /// </remarks>

    [Guid("8c4df177-c4c4-4749-9a5e-23b87f8be028")]
    public class FeatureFillYurDepatmentListEventReceiver : SPFeatureReceiver
    {
        // Раскомментируйте ниже метод для обработки события, возникающего после активации компонента.

        private const string List_JOB_NAME = "UpdateYurDepartmentsListFrom1C";

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            bool administratorAccessDenied = SPWebService.ContentService.RemoteAdministratorAccessDenied;
            try
            {
                SPWebService.ContentService.RemoteAdministratorAccessDenied = false;
                SPSite parent = properties.Feature.Parent as SPSite;
                foreach (SPJobDefinition jobDefinition in (SPPersistedObjectCollection<SPJobDefinition>)parent.WebApplication.JobDefinitions)
                {
                    if (jobDefinition.Name == "UpdateYurDepartmentsListFrom1C")
                        jobDefinition.Delete();
                }
                UpdateYurDepartmentsFrom1C updateYurDepartmentsFrom1C = new UpdateYurDepartmentsFrom1C("UpdateYurDepartmentsListFrom1C", parent.WebApplication);
                updateYurDepartmentsFrom1C.Schedule = (SPSchedule)new SPDailySchedule()
                {
                    BeginHour = 2,
                    BeginMinute = 15,
                    BeginSecond = 15,
                    EndHour = 2,
                    EndMinute = 30,
                    EndSecond = 45
                };
                updateYurDepartmentsFrom1C.Update();
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
