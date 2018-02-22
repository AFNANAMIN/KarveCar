﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using KarveCommon.Services;
using KarveDataServices;
using KarveDataServices.DataTransferObject;
using Prism.Regions;
using KarveCommon.Generic;
using MasterModule.Views;
using MasterModule.Common;
using KarveDataServices.DataObjects;
using System.ComponentModel;
using System.Windows;
using System.Diagnostics.Contracts;
using Prism.Commands;
using System.Diagnostics;
using NLog;

namespace MasterModule.ViewModels
{
    /// <summary>
    ///  View of control for the office.
    /// </summary>
    public class OfficesControlViewModel : MasterControlViewModuleBase
    {
        /// <summary>
        ///  Control view for the office
        /// </summary>
        /// <param name="configurationService">Configuration service</param>
        /// <param name="eventManager">Event manager</param>
        /// <param name="services">Data Services</param>
        /// <param name="regionManager">Region manager</param>
        public OfficesControlViewModel(IConfigurationService configurationService, IEventManager eventManager, IDataServices services, IRegionManager regionManager) : base(configurationService, eventManager, services, regionManager)
        {
            InitViewModel();
        }

        /// <summary>
        ///  Grid of the offices in the database.
        /// </summary>
        public IEnumerable<OfficeSummaryDto> SourceView
        {
            get => _sourceView;
            set { _sourceView = value; RaisePropertyChanged(); }
        }
        /// <summary>
        ///  Delete asynchronously an office
        /// </summary>
        /// <param name="primaryKey">Primary Key</param>
        /// <param name="payLoad">Data payload</param>
        /// <returns>True or false</returns>
        public override async Task<bool> DeleteAsync(string primaryKey, DataPayLoad payLoad)
        {
            IOfficeDataServices service = DataServices.GetOfficeDataServices();
            IOfficeData data = await service.GetAsyncOfficeDo(primaryKey);
            bool retValue = await service.DeleteOfficeAsyncDo(data);
            return retValue;
        }
        
        /// <summary>
        ///  NewItem. This is a new item for the office.
        /// </summary>
        public override void NewItem()
        {
            string name =KarveLocale.Properties.Resources.OfficesControlViewModel_NewItem_NuevaOfficina;
            string officeId = DataServices.GetOfficeDataServices().GetNewId();
            string viewNameValue = name + "." + officeId;
            // here shall be added to the region
            var navigationParameters = new NavigationParameters();
            navigationParameters.Add("id", officeId);
            navigationParameters.Add(ScopedRegionNavigationContentLoader.DefaultViewName, viewNameValue);
            var uri = new Uri(typeof(OfficeInfoView).FullName + navigationParameters, UriKind.Relative);
            RegionManager.RequestNavigate("TabRegion", uri);
            DataPayLoad currentPayload = BuildShowPayLoadDo(viewNameValue);
            currentPayload.Subsystem = DataSubSystem.CompanySubsystem;
            currentPayload.PayloadType = DataPayLoad.Type.Insert;
            currentPayload.PrimaryKeyValue = officeId;
            currentPayload.DataObject = DataServices.GetCompanyDataServices().GetNewCompanyDo(officeId);
            currentPayload.HasDataObject = true;
            currentPayload.Sender = EventSubsystem.CompanySummaryVm;
            EventManager.NotifyObserverSubsystem(MasterModuleConstants.OfficeSubSytemName, currentPayload);
        }
        /// <summary>
        ///  This initalize the view model.
        /// </summary>
        public override void StartAndNotify()
        {
            IOfficeDataServices officeDataService = DataServices.GetOfficeDataServices();
            InitializationNotifierOffice = NotifyTaskCompletion.Create<IEnumerable<OfficeSummaryDto>>(officeDataService.GetAsyncAllOfficeSummary(), InitEventHandler);
        }

        private void InitViewModel()
        {
            OpenItemCommand = new DelegateCommand<object>(OnOpenItemCommand);
            GridIdentifier = KarveCommon.Generic.GridIdentifiers.OfficeSummaryGrid;
            InitEventHandler += LoadNotificationHandler;
            _mailBoxName = EventSubsystem.OfficeSummaryVm ;
            OpenItemCommand = new DelegateCommand<object>(OnOpenItemCommand);
            StartAndNotify();
            
        }
        /// <summary>
        ///  Open a new item.
        /// </summary>
        /// <param name="selectedItem">Selected command</param>
        private async void OnOpenItemCommand(object selectedItem)
        {
            OfficeSummaryDto summaryItem = selectedItem as OfficeSummaryDto;
            Stopwatch watch = new Stopwatch();
            
            if (selectedItem != null)
            {
                watch.Start();
                string name = summaryItem.Name;
                string id = summaryItem.Code;
                string tabName = id + "." + name;
                var navigationParameters = new NavigationParameters();
                navigationParameters.Add("Id", id);
                navigationParameters.Add(ScopedRegionNavigationContentLoader.DefaultViewName, tabName);
                var uri = new Uri(typeof(OfficeInfoView).FullName + navigationParameters, UriKind.Relative);
                Logger.Log(LogLevel.Debug, "[UI] OfficeInfoViewModel. Before navigation: " + id + "Elapsed time: " + watch.ElapsedMilliseconds);
                RegionManager.RequestNavigate("TabRegion", uri);
                Logger.Log(LogLevel.Debug, "[UI] OfficeInfoViewModel. Data before: " + id + "Elapsed time: " + watch.ElapsedMilliseconds);
                IOfficeData provider = await DataServices.GetOfficeDataServices().GetAsyncOfficeDo(id);
                  
                Logger.Log(LogLevel.Debug, "[UI] OfficeInfoViewModel. Data loaded: " + id + "Elapsed time: " + watch.ElapsedMilliseconds);
                DataPayLoad currentPayload = BuildShowPayLoadDo(tabName, provider);
                currentPayload.PrimaryKeyValue = id;
                currentPayload.Sender = _mailBoxName;
                watch.Stop();
                Logger.Log(LogLevel.Debug, "[UI] OfficeInfoViewModel. Opening Office Tab: " + id + "Elapsed time: " + watch.ElapsedMilliseconds);
                EventManager.NotifyObserverSubsystem(MasterModuleConstants.OfficeSubSytemName, currentPayload);
            }
        }

        /// <summary>
        ///  FIXME: i repeat myself.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="ev">Office name</param>
        private void LoadNotificationHandler(object sender, PropertyChangedEventArgs ev)
        {
            INotifyTaskCompletion<IEnumerable<OfficeSummaryDto>> value = sender as INotifyTaskCompletion<IEnumerable<OfficeSummaryDto>>;
            string propertyName = ev.PropertyName;
            if (propertyName.Equals("Status"))
            {
                if (InitializationNotifierOffice.IsSuccessfullyCompleted)
                {
                    if (value != null)
                    {
                        SourceView = value.Task.Result;
                    }

                }
            }
            else if (propertyName.Equals("IsSuccessfullyCompleted"))
            {
                var result = value?.Task.Result;
                SourceView = result;
            }
            else
            {
                if (InitializationNotifierOffice.IsFaulted)
                {
                    MessageBox.Show(InitializationNotifierOffice.ErrorMessage);
                }
            }
            Contract.Ensures(SourceView != null, "SourceView shall be present");
        }

        /// <summary>
        ///  This swet the routename for the event mailbox.
        /// </summary>
        /// <param name="name">Name of the route</param>
        /// <returns></returns>
        protected override string GetRouteName(string name)
        {
            var route = @"master://" + MasterModuleConstants.OfficeSubSytemName + "//" + name;
            string routedName = new Uri(route).AbsoluteUri;
            return routedName;
        }
        /// <summary>
        ///  DataPayload
        /// </summary>
        /// <param name="payLoad">Registration payload for the office.</param>
        protected override void SetRegistrationPayLoad(ref DataPayLoad payLoad)
        {
            payLoad.PayloadType = DataPayLoad.Type.RegistrationPayload;
            payLoad.Subsystem = DataSubSystem.OfficeSubsystem;
        }
        protected override void SetDataObject(object result)
        {
            
        }
        // TODO eliminate this.
        protected override void SetTable(DataTable table)
        {
            
        }
        #region Private Fields

        private INotifyTaskCompletion<IEnumerable<OfficeSummaryDto>> InitializationNotifierOffice;
        private IEnumerable<OfficeSummaryDto> _sourceView;
        private string _mailBoxName;
        /// <summary>
        ///  This is the client subsystem prefix module.
        /// </summary>
        private const string ClientModuleRoutePrefix = MasterModuleConstants.ClientSubSystemName;


        #endregion
    }
}