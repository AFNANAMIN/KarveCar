﻿using KarveCar.ViewModel.MaestrosViewModel;
using System.Windows.Controls;

namespace KarveCar.View
{
    /// <summary>
    /// Lógica de interacción para GrupoVehiculoDataGridUserControl.xaml
    /// </summary>
    public partial class GrupoVehiculoDataGridUserControl : UserControl
    {
        public GrupoVehiculoDataGridUserControl()
        {
            InitializeComponent();
            this.PaymentSystem.Theme = ExtendedGrid.ExtendedGridControl.ExtendedDataGrid.Themes.Office2007Silver;
        }
    }
}
