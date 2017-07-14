﻿using KRibbon.ViewModel.GenericViewModel;
using System;
using System.Windows.Input;

namespace KRibbon.Commands.ToolBarCommand
{
    public class ImprimirToolBarCommand : ICommand
    {
        private ToolBarViewModel toolbarvm;

        public ImprimirToolBarCommand(ToolBarViewModel vm)
        {
            this.toolbarvm = vm;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            toolbarvm.ImprimirToolBar(parameter);
        }
    }
}