using KarveCommon.Generic;
using KarveCommon.Services;
using KarveCommonInterfaces;
using KarveDataServices;
using KarveDataServices.DataTransferObject;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using KarveCar.Navigation;
using DataAccessLayer.DataObjects;
using Prism.Regions;

namespace {{data.modulename}}.ViewModels
{
    public class {{data.name}}ViewModel : KarveRoutingBaseViewModel, ICreateRegionManagerScope, IEventObserver, INavigationAware
    {
        private ICommand _deleteCommand;
        private ICommand _saveCommand;
        private {{data.name}}Dto _dataObject;
        private I{{data.name}}Service _data{{data.name}}Service;
        private IAssistDataService _assistDataService;
        private I{{data.name}} _domainObject;
        private IKarveNavigator _navigator;
        #region DtoDeclaration
        {% for dto in data.dto_values %}
        private IEnumerable<{{dto.type}}> _{{dto.name}};
        {%endfor%}
        #end DtoDeclaration
        private IUserSettings _userSettings;

        public {{data.name}}ViewModel(IDataServices services, IInteractionRequestController controller,
                               IDialogService dialogService,
                               IEventManager eventManager,
                               IKarveNavigator navigation,
                               IConfigurationService configurationService,
                               IRegionManager regionManager) : base(services, controller, dialogService, eventManager, regionManager)
        {
            AssistCommand = new DelegateCommand<object>(OnAssistCommand);
            ItemChangedCommand = new DelegateCommand<object>(OnChangedField);

            SubSystem = DataSubSystem.{{data.subsystem}};

            ViewModelUri = new Uri("karve://{{data.baseuri}}/show/viewmodel?id=" + Guid.ToString());
            _navigator = navigation;
            _userSettings = configurationService.GetUserSettings();
            _deleteCommand = new DelegateCommand<object>(deleteCommandFunction);
            _saveCommand = new DelegateCommand<object>(saveCommandFunction);
            _data{Name}Service = services.Get{{data.name}}DataService();
            _assistDataService = services.GetAssistDataServices();
            AssistMapper = _assistDataService.Mapper;
            EventManager.RegisterObserverSubsystem(Karve.Constants.Group{{data.name}}, this);
        }

        private void OnChangedField(object ev)
        {
            if (ev is IDictionary<string, object> eventData)
            {

                OnChangedCommand(DataObject,
                                       eventData,
                                       DataSubSystem.{{data.subsystem}},
                                       Karve.Constants.Group{{data.name}},
                                       ViewModelUri.ToString());
            }
        }

        public override void DisposeEvents()
        {
            base.DisposeEvents();
            EventManager.DeleteObserverSubSystem(Karve.Constants.Group{{data.name}}, this);
        }

        private void OnAssistCommand(object param)
        {
            IDictionary<string, string> values = (Dictionary<string, string>)param;
            string assistTableName = values.ContainsKey("AssistTable") ? values["AssistTable"] as string : null;
            string assistQuery = values.ContainsKey("AssistQuery") ? values["AssistQuery"] as string : null;
            this.AssistNotifierInitialized = NotifyTaskCompletion.Create<bool>(AssistQueryRequestHandler(assistTableName, assistQuery), (sender, ev) => {

            });
        }
        private async Task<bool> AssistQueryRequestHandler(string assistTableName, string assistQuery)
        {
            var collectionValue = await AssistMapper.ExecuteAssistGeneric(assistTableName, assistQuery);

            switch (assistTableName)
            {
                {% for assist in data.assist_list %}
                case "{{assist.tag}}":
                    {
                        {{assist.dto_name}} = (IEnumerable<{{assist.dto_type}}>)collectionValue;
                        break;
                    };
                {% endfor %}
            }
            return true;
        }
        /// <summary>
        ///  This delete asynchronously.
        /// </summary>
        /// <param name="primaryKey">Value of the primary key.</param>
        /// <param name="subSystem">Kind of subsystem</param>
        /// <param name="eventSubSystem">Event subsystem</param>
        /// <returns></returns>
        private async Task<bool> DeleteAsync(string primaryKey, DataSubSystem subSystem, string eventSubSystem)
        {
            if (string.IsNullOrEmpty(primaryKey))
                throw new ArgumentNullException();
            var deleted = false;
            try
            {
                deleted = await _data{{data.name}}Service.DeleteAsync(_domainObject).ConfigureAwait(false);

            }
            catch (Exception e)
            {
                DialogService?.ShowErrorMessage(e.Message);
            }
            return deleted;
        }

        public bool CreateRegionManagerScope => false;


        public void IncomingPayload(DataPayLoad dataPayLoad)
        {
            if (dataPayLoad == null) return;
            if ((dataPayLoad.Sender != null) && (dataPayLoad.Sender.Equals(ViewModelUri)))
            {
                return;
            }
            var interpeter = new PayloadInterpeter<I{{data.name}}>();
            var currentId = _data{{data.name}}Service.NewId();
            interpeter.Init = Init;
            interpeter.CleanUp = CleanUp;

            if (string.IsNullOrEmpty(PrimaryKeyValue)) PrimaryKeyValue = dataPayLoad.PrimaryKeyValue;

            if (string.IsNullOrEmpty(PrimaryKeyValue))
                return;

            OperationalState = interpeter.CheckOperationalType(dataPayLoad);
            interpeter.ActionOnPayload(dataPayLoad, PrimaryKeyValue, currentId, SubSystem,
                EventSubsystem.BookingSubsystemVm);
        }

        protected void CleanUp(DataPayLoad payLoad, DataSubSystem subsystem, string eventSubsystem)
        {
            //    DeleteItem(payLoad);
        }
        /// <summary>
        ///  Called at initializing the form.
        /// </summary>
        /// <param name="value">Value of the PrimaryKeyValue</param>
        /// <param name="payload">Value of the Payload</param>
        /// <param name="insertion">True if it is inserted or false otherwise</param>
        protected void Init(string value, DataPayLoad payload, bool insertion)
        {
            if (payload.DataObject==null)
            {
                return;
            }
           if (!(payload.DataObject is I{{data.name}}Service))
            {
                return;
            }
            _domainObject = payload.DataObject as I{{data.name}}Service;
            DataObject = _domainObject.Value;
            {% for dto in data.dto_values %}
             _{{dto.name}} = _domainObject._{{dto.name}}
            {%endfor%}
            RegisterToolBar();
            ActiveSubSystem();
        }

        public override bool IsDeleter { get { return true; } }

        public override bool DeleteView(object key)
        {
            base.DeleteView(key);
            DataPayLoad payload = new DataPayLoad();
            _reservationRequest.Value = DataObject;
            payload.DataObject = _domainObject;
            payload.HasDataObject = true;
            payload.PrimaryKeyValue = DataObject.CodeId;
            DeleteItem(payload);
            return true;
        }
        protected override void DeleteItem(DataPayLoad payLoad)
        {
            bool isDeleted = false;
            NotifyTaskCompletion.Create(DeleteAsync(payLoad),
                (sender, args) =>
                {

                    if (sender is INotifyTaskCompletion<bool> taskCompletion)
                    {
                        if (taskCompletion.IsFaulted)
                            DialogService?.ShowErrorMessage("Error during deleting");

                        if (!taskCompletion.IsSuccessfullyCompleted) return;
                        isDeleted = true;

                    }
                });

            if (isDeleted)
            {
                var dataPayLoad = new DataPayLoad
                {
                    Sender = ViewModelUri.ToString(),
                    PayloadType = DataPayLoad.Type.UpdateView
                };
                EventManager.NotifyObserverSubsystem("{data.name}Group", dataPayLoad);
                UnregisterToolBar(payLoad.PrimaryKeyValue);
                DeleteRegion();
            }
        }
        private async Task<bool> DeleteAsync(DataPayLoad payLoad)
        {
            bool retValue = false;
            if (payLoad.DataObject is IReservationRequest request)
            {
                try
                {
                    retValue = await _dataReservationService.DeleteAsync(request).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    var msg = "Error during saving request:" + ex.Message;
                    DialogService?.ShowErrorMessage(msg);
                }
            }
            return retValue;
        }
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        protected override string GetRouteName(string name)
        {
            return ViewModelUri.ToString();
        }

        protected override void SetRegistrationPayLoad(ref DataPayLoad payLoad)
        {
            payLoad.Subsystem = DataSubSystem.{{data.subystem}}

        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }
        public {data.name}Dto DataObject {
            set {
                _dataObject = value; RaisePropertyChanged();
            }
            get
            {
                return _dataObject;
            }
        }
        #region DtoProperties
        {% for dto in data.dto_values %}
         IEnumerable<{{dto.name}}> {{dto.name}} { get =>  _{{dto.name}}Value; set {  _{{dto.name}}Value; = value; RaisePropertyChanged(); } }
        {%endfor%}
        #end DtoProperties
    }
}