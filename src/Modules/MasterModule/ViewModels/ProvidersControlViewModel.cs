﻿using System;
using System.Data;
using System.Windows.Input;
using KarveDataServices;
using KarveCommon.Services;
using Prism.Commands;
using Microsoft.Practices.Unity;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using KarveCommon.Generic;
using KarveDataServices.DataObjects;
using MasterModule.Common;
using MasterModule.Interfaces;
using MasterModule.Views;
using Prism.Regions;
using KarveDataServices.DataTransferObject;
using System.Threading.Tasks;
using DataAccessLayer.Model;
using NLog;
using Syncfusion.UI.Xaml.Grid;
using DataRow = System.Data.DataRow;
using KarveCommonInterfaces;

namespace MasterModule.ViewModels
{
    /// <summary>
    /// This is the control view model for the suppliers.
    /// It shows the summary of the suppliers that are present in the system and on a click from the view fires a info view model.
    /// In the current system there are two type of view model:
    /// 1. ControlViewModels. This kind of view model is responsible for: 
    /// a. fire up all the tabs needed foreach item.
    /// b. delete  
    ///     It exposes a summary data trasnsfer object 
    /// 
    /// 2. InfoViewModels. This kind of view model is responsible for show the value of each specific item.
    /// </summary>
    internal sealed class ProvidersControlViewModel : MasterControlViewModuleBase, IProvidersViewModel
    {
        private const string ProviderNameColumn = "Nombre";
        private const string ProviderColumnCode = "Codigo";
        private const string ProviderModuleRoutePrefix = "ProviderModule:";
       
        /// <summary>
        ///  Type of payment data services.
        /// </summary>
        private readonly IUnityContainer _container;
        private static string PROVIDER_SUMMARY_VM = "ProvidersControlViewModel";
        private DataTable _extendedSupplierDataTable;

        private IRegionManager _regionManager;
        private Stopwatch _timing = new Stopwatch();
        private IEnumerable<SupplierSummaryDto> _summaryCollection = new List<SupplierSummaryDto>();
        private static long _uniqueIdentifier = 0;

       
        private string _mailBoxName;
        private INotifyTaskCompletion<IEnumerable<SupplierSummaryDto>> _supplierTaskNotify;
        private PropertyChangedEventHandler _supplierTaskEvent;

        private ISupplierDataServices _supplierDataServices;
        // Yes it violateds SRP it does two things. Show the main and fireup a new ui.
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public ProvidersControlViewModel(IConfigurationService configurationService,
            IUnityContainer container,
            IDataServices services,
            IRegionManager regionManager,
            IDialogService service,
            IEventManager eventManager) : base(configurationService, eventManager, services, null, service,regionManager)
        {
            _container = container;
            _regionManager = regionManager;
            _extendedSupplierDataTable = new DataTable();
            _uniqueIdentifier = _uniqueIdentifier % Int64.MaxValue;
            _mailBoxName = PROVIDER_SUMMARY_VM;
            _supplierTaskEvent += OnNotifyIncrementalList<SupplierSummaryDto>;
            _supplierDataServices = DataServices.GetSupplierDataServices();
            PagingEvent += OnPagedEvent;
            OpenItemCommand = new DelegateCommand<object>(OpenCurrentItem);
            InitViewModel();
        }
        /// <summary>
        ///  Property that return the summary of Suppliers.
        /// </summary>
        public IEnumerable<SupplierSummaryDto> SummaryCollection
        {
            set
            {
                _summaryCollection = value;
                RaisePropertyChanged();
            }
            get => _summaryCollection;
        }

        private void InitViewModel()
        {
            base.GridIdentifier = GridIdentifiers.Supplier;
            SubSystem = DataSubSystem.SupplierSubsystem;
            StartAndNotify();
            ActiveSubSystem();
        }
        /// <summary>
        ///  Command to open a new view for each detailed supplier.
        /// </summary>
       
        public ICommand OpenItem
        {
            get { return OpenItemCommand; }
            set { OpenItemCommand = value; }
        }

        protected override void SetTable(DataTable table)
        {
            SummaryView = table;
           
        }
        protected override void SetRegistrationPayLoad(ref DataPayLoad payLoad)
        {
            payLoad.PayloadType = DataPayLoad.Type.RegistrationPayload;
            payLoad.Subsystem = DataSubSystem.SupplierSubsystem;
        }
        

        protected override void SetDataObject(object result)
        {
            // we dont use here any data object.
        }
        /// <summary>
        ///  This route a name.
        /// </summary>
        /// <param name="name">This write a name to be sent as routring</param>
        /// <returns></returns>
        protected override string GetRouteName(string name)
        {
            var routedName = ProviderModuleRoutePrefix + name;
            return routedName;

        }
        /// <summary>
        /// This method creates a new item using prsim scope navigation.
        ///    
        /// </summary>
        protected override void NewItem()
        {
            string name = KarveLocale.Properties.Resources.ProvidersControlViewModel_NewItem_NuevoProveedor;
            string supplierId = DataServices.GetSupplierDataServices().GetNewId();
            string viewNameValue = name + "." + supplierId;
            // here shall be added to the region
            var navigationParameters = new NavigationParameters();
            navigationParameters.Add("supplierId", supplierId);
            navigationParameters.Add(ScopedRegionNavigationContentLoader.DefaultViewName, viewNameValue);
            var uri = new Uri(typeof(ProviderInfoView).FullName + navigationParameters, UriKind.Relative);
            _regionManager.RequestNavigate("TabRegion", uri);
            DataPayLoad currentPayload = BuildShowPayLoadDo(viewNameValue);
            currentPayload.Subsystem = DataSubSystem.SupplierSubsystem;
            currentPayload.PayloadType = DataPayLoad.Type.Insert;
            currentPayload.PrimaryKeyValue = supplierId;
            currentPayload.DataObject = DataServices.GetSupplierDataServices().GetNewSupplierDo(supplierId);
            currentPayload.HasDataObject = true;
            currentPayload.Sender = EventSubsystem.SuppliersSummaryVm;
            EventManager.NotifyObserverSubsystem(MasterModuleConstants.ProviderSubsystemName, currentPayload);
        }
       
        private async void OpenCurrentItem(object currentItem)
        {
            var name = string.Empty;
            var supplierId = string.Empty;
            var tabName = string.Empty;

            if (currentItem is DataRowView rowView)
            {
                DataRow row = rowView.Row;
                 name = row[ProviderNameColumn] as string;
                supplierId = row[ProviderColumnCode] as string;
                tabName = supplierId + "." + name;
                
            }
            if (currentItem is SupplierSummaryDto item)
            {
                name = item.Nombre;
                supplierId = item.Codigo;
                tabName = supplierId + "." + name;
            }

            var navigationParameters = new NavigationParameters
            {
                {"supplierId", supplierId},
                {ScopedRegionNavigationContentLoader.DefaultViewName, tabName}
            };
            var uri = new Uri(typeof(ProviderInfoView).FullName + navigationParameters, UriKind.Relative);
            _regionManager.RequestNavigate("TabRegion", uri);
            var provider = await _supplierDataServices.GetAsyncSupplierDo(supplierId);
            var currentPayload = BuildShowPayLoadDo(tabName, provider);
            currentPayload.PrimaryKeyValue = supplierId;
            currentPayload.Sender = _mailBoxName;
            Logger.Log(LogLevel.Debug, "[UI] ProviderControlViewModel. Opening Supplier Tab: " + supplierId);
            EventManager.NotifyObserverSubsystem(MasterModuleConstants.ProviderSubsystemName, currentPayload);

        }

        public override void StartAndNotify()
        {
            MessageHandlerMailBox += MessageHandler;
            _mailBoxName = ProvidersControlViewModel.PROVIDER_SUMMARY_VM;
            EventManager.RegisterMailBox(_mailBoxName, MessageHandlerMailBox);
            var supplierDataServices = DataServices.GetSupplierDataServices();
            _supplierTaskNotify = NotifyTaskCompletion.Create<IEnumerable<SupplierSummaryDto>>(supplierDataServices.GetPagedSummaryDoAsync(1, DefaultPageSize), _supplierTaskEvent);
           
        }
     
        public override async Task<bool> DeleteAsync(string supplierId, DataPayLoad payLoad)
        {
            var provider = await _supplierDataServices.GetAsyncSupplierDo(supplierId).ConfigureAwait(false);
            var retValue = await _supplierDataServices.DeleteAsyncSupplierDo(provider).ConfigureAwait(false);
            return retValue;
        }
        public override void DisposeEvents()
        {
           base.DisposeEvents();
           DeleteMailBox(_mailBoxName);
        }
        protected override void OnPagedEvent(object sender, PropertyChangedEventArgs e)
        {
            var listCompletion =
                sender as INotifyTaskCompletion<IEnumerable<SupplierSummaryDto>>;
            if ((listCompletion != null) && (listCompletion.IsSuccessfullyCompleted))
            {

                if (SummaryView is IncrementalList<SupplierSummaryDto> summary)
                {
                    summary.LoadItems(listCompletion.Result);
                    SummaryView = summary;
                }
            }

            if ((listCompletion != null) && (listCompletion.IsFaulted))
            {
                DialogService?.ShowErrorMessage("Error Loading data " + listCompletion.ErrorMessage);
            }
        }
 
        protected override void LoadMoreItems(uint count, int baseIndex)
        {
            Logger.Debug("Base" + baseIndex.ToString());
            NotifyTaskCompletion.Create<IEnumerable<SupplierSummaryDto>>(
                _supplierDataServices.GetPagedSummaryDoAsync(baseIndex, DefaultPageSize), PagingEvent);

        }
        protected override void SetResult<T>(IEnumerable<T> result)
        {
            var reSummaryDtos = result as IEnumerable<SupplierSummaryDto>;
            var maxItems = _supplierDataServices.NumberItems;
            PageCount = _supplierDataServices.NumberPage;
            SummaryView = new IncrementalList<SupplierSummaryDto>(LoadMoreItems) { MaxItemCount = (int)maxItems };
        }
    }
}
