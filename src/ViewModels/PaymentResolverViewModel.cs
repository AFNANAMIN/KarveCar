﻿//using PaymentTypeModule;

namespace KarveCar.ViewModels
{
    public class PaymentResolverViewModel
    {
        /*
        #region Variables
        private DelegateCommand<object> _showPaymentCommand;
        #endregion

        #region Constructor
        public PaymentResolverViewModel()
        {
            this._showPaymentCommand = new DelegateCommand<object>(showPaymentCommand);
        }
        #endregion

        #region Commands
        public ICommand ShowPaymentCommand
        {
            get
            {
                return _showPaymentCommand;
            }
        }
        #endregion
        
        #region Métodos
        /// <summary>
        /// Crea el TabItem para CRUD los Grupos de Vehículos
        /// </summary>
        /// <param name="parameter"></param>
        ///         
        public void showPaymentCommand(object parameter)
        {
            EOpcion opcion = EOpcion.rbtnFormasCobroClientes;
            View.MainWindow mainWindow = Application.Current.MainWindow as View.MainWindow;
            IUnityContainer container = mainWindow.UnityContainer;
            IPaymentView paymentView = container.Resolve<IPaymentView>();
            IPaymentViewModule paymentViewModule = container.Resolve<IPaymentViewModule>();
            UserControl view = paymentView as UserControl;
            view.DataContext = paymentViewModule;

            if (!ribbonbuttondictionary.ContainsKey(opcion))
            {
                TemplateInfoRibbonButton ribbonTemplate = new TemplateInfoRibbonButton();
                ribbonTemplate.propertiesresources = "lrbtnFormasCobroClientes"; 
                ribbonbuttondictionary.Add(opcion, ribbonTemplate);   
            }          
            //Si el param no se encuentra en la Enum EOpcion, no hace nada, sino mostraría 
            //la Tab correspondiente al primer valor de la Enum EOpcion
            if (opcion.ToString() == parameter.ToString())
            {
                TabItemLogic.CreateTabItemUserControl(opcion, paymentView);
            
            }
        }
        
        #endregion
        */
    }
}