﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using KarveControls.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.Windows.Interactivity;
using ICommand = System.Windows.Input.ICommand;
using Syncfusion.UI.Xaml.Grid;

namespace KarveControls
{
    /// <summary>
    ///  This is a list of attached properties to be associated to each karve control.
    /// </summary>
    public class ControlExt: DependencyObject
    {

        private static string lastTextBoxValue = "";
        /// <summary>
        ///  Email data type
        /// </summary>
        public static DataType EmailDataType = DataType.Email;
        /// <summary>
        ///  DataType to be allowed.
        /// </summary>
        public enum DataType
        {
            /// <summary>
            ///  Double kind of data.
            /// </summary>
            DoubleField, 
            /// <summary>
            /// Integer field of the component.
            /// </summary>
            IntegerField,
            /// <summary>
            ///  Nif field of the component.
            /// </summary>
            NifField,
            /// <summary>
            ///  Iban field of the component.
            /// </summary>
            IbanField,
            /// <summary>
            ///  Any other kind of field control.
            /// </summary>
            Any,
            /// <summary>
            ///  Email kind field of the control.
            /// </summary>
            Email,
            /// <summary>
            ///  Phone field of the control.
            /// </summary>
            Phone,
            /// <summary>
            /// Bank Account of the control.
            /// </summary>
            BankAccount,
            /// <summary>
            ///  Swift field of the control.
            /// </summary>
            Swift,
            /// <summary>
            ///  Datatime field of the contorl.
            /// </summary>
            DateTime
        }
        #region Description
        /// <summary>
        ///  This is a metadata that describe a component.
        /// </summary>
        public static readonly DependencyProperty DescriptionDependencyProperty =
            DependencyProperty.RegisterAttached(
                "Description",
                typeof(string),
                typeof(ControlExt),
                new PropertyMetadata(String.Empty));
        #endregion


        private static bool changedValue = false;

        #region ItemChangedCommand
        /// <summary>
        ///  This is the kind of data allowd.
        /// </summary>
        public static readonly DependencyProperty ItemChangedCommandDependencyProperty =
            DependencyProperty.RegisterAttached(
                "ItemChangedCommand",
                typeof(ICommand),
                typeof(ControlExt),
                new PropertyMetadata(null, PropertyChangedCb));
       
        /// <summary>
        ///  This is a property changed util.
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <param name="eventArgs"></param>
        public static void PropertyChangedCb(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {

            if (dependencyObject is Behavior<SfDataGrid>)
            {
                // it is a chnaged that is coming fom the grid.

                Behavior<SfDataGrid> changedBehavior = dependencyObject as Behavior<SfDataGrid>;
                /*
            
            Dictionary<string, object> objectName = new Dictionary<string, object>();

                objectName["DataObject"] = GetDataSource(dependencyObject);
                objectName["ChangedValue"] = textBox.Text;
                objectName["PreviousValue"] = lastTextBoxValue;
                */
            }
            if (dependencyObject is DataDatePicker)
            {
                DataDatePicker dataDatePicker = dependencyObject as DataDatePicker;
                dataDatePicker.DataDatePickerChanged += DataDatePicker_DataDatePickerChanged;

            }
            if (dependencyObject is TextBox)
            {
                TextBox box = dependencyObject as TextBox;
                box.TextChanged += TextBox_ChangedBehaviour;
                box.LostFocus += Box_LostFocus;
            }
            if (dependencyObject is DataFieldCheckBox)
            {
                DataFieldCheckBox checkBox = dependencyObject as DataFieldCheckBox;
                checkBox.DataFieldCheckBoxChanged += CheckBox_DataFieldCheckBoxChanged;
            }
            if (dependencyObject is ComboBox)
            {
                ComboBox comboBox = dependencyObject as ComboBox;
                if (comboBox != null)
                {
                    // here we do the combox box.
                    comboBox.SelectionChanged += ComboBox_SelectionChanged;

                }
            }
        }

        private static void Box_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                changedValue = false;
                var command = textBox.GetValue(ItemChangedCommandDependencyProperty) as ICommand;
                if (command != null)
                {
                    IDictionary<string, object> objectName = new Dictionary<string, object>();
                    objectName["DataObject"] = GetDataSource(textBox);
                    objectName["DataSourcePath"] = GetDataSourcePath(textBox);
                    objectName["ChangedValue"] = textBox.Text;
                    objectName["PreviousValue"] = lastTextBoxValue;
                    lastTextBoxValue = textBox.Text;
                    command.Execute(objectName);
                }
            }
        }

        /// <summary>
        ///  SelectionChanged. This event get triggered when the selection change in a property
        /// </summary>
        /// <param name="sender">The sender of the call</param>
        /// <param name="e"> Event args</param>
        private static void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                var command = comboBox.GetValue(ItemChangedCommandDependencyProperty) as ICommand;
                if (command != null)
                {
                    IDictionary<string, object> objectName = new Dictionary<string, object>();
                    object dataObject = GetDataSource(comboBox);
                    if (dataObject != null)
                    {
                        string dataPath = GetDataSourcePath(comboBox);
                        if (string.IsNullOrEmpty(dataPath))
                        {
                            int selectedIndex = comboBox.SelectedIndex;
                            ComponentUtils.SetPropValue(dataObject, dataPath, selectedIndex);
                            objectName["ChangedIndex"] = selectedIndex;
                            objectName["ChangedValue"] =  comboBox.SelectedValue;
                        }
                        objectName["DataObject"] = GetDataSource(comboBox);
                        objectName["DataSourcePath"] = GetDataSourcePath(comboBox);
                    }
                    command.Execute(objectName);
                }
            }
        }

        /// <summary>
        ///  DataFieldCheckBoxChanged.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void CheckBox_DataFieldCheckBoxChanged(object sender, RoutedEventArgs e)
        {
            var dataFieldCheckBox = sender as DataFieldCheckBox;
            if (dataFieldCheckBox != null)
            {
                var command = dataFieldCheckBox.GetValue(ItemChangedCommandDependencyProperty) as ICommand;
                if (command != null)
                {
                    IDictionary<string, object> objectName = new Dictionary<string, object>();
                    objectName["DataObject"] = GetDataSource(dataFieldCheckBox);
                    objectName["DataSourcePath"] = GetDataSourcePath(dataFieldCheckBox);
                    objectName["ChangedValue"] = dataFieldCheckBox.IsChecked;
                    command.Execute(objectName);
                }
            }
        }

        /// <summary>
        ///  Set the item changed command, Attached behaviour for the component.
        /// </summary>
        /// <param name="d">Depedency property</param>
        /// <param name="e">Value</param>
        public static void SetItemChangedCommand(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.SetValue(ItemChangedCommandDependencyProperty, e);
        }
        private static void TextBox_ChangedBehaviour(object sender, RoutedEventArgs args)
        {
            changedValue = true;
            
        }
        private static void DataDatePicker_DataDatePickerChanged(object sender, RoutedEventArgs e)
        {
            DataDatePicker.DataDatePickerEventArgs args = (DataDatePicker.DataDatePickerEventArgs) e;
            DataDatePicker picker = sender as DataDatePicker;
            if (picker != null)
            {
                ICommand changedCommand =  picker.GetValue(ItemChangedCommandDependencyProperty) as ICommand;
                IDictionary<string, object> evArgs = args.ChangedValuesObjects;
                if (changedCommand!=null)
                {
                    changedCommand.Execute(evArgs);
                }
            }
        }

        /// <summary>
        ///  Get item changed command.
        /// </summary>
        /// <param name="d">Dependency Properties</param>
        /// <param name="e">Value</param>
        /// <returns></returns>
        public static ICommand GetItemChangedCommand(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            return (ICommand) d.GetValue(ItemChangedCommandDependencyProperty);
        }

        #endregion

        #region DataAllowed
        /// <summary>
        ///  This is the kind of data allowd.
        /// </summary>
        public static readonly DependencyProperty DataAllowedDependencyProperty =
            DependencyProperty.RegisterAttached(
                "DataAllowed",
                typeof(DataType),
                typeof(ControlExt),
                new PropertyMetadata(DataType.Any));
        /// <summary>
        ///  Kind of data allowed for this component.
        /// </summary>
        public DataType DataAllowed
        {
            get { return (DataType) GetValue(DataAllowedDependencyProperty); }
            set { SetValue(DataAllowedDependencyProperty, value); }
        }

        #endregion
        #region DataSource        
        /// <summary>
        ///  DataSource: data table or data object associaed with this control.
        /// </summary>
        public static DependencyProperty DataSourceDependencyProperty
            = DependencyProperty.RegisterAttached(
                "DataSource",
                typeof(object),
                typeof(ControlExt),
                new UIPropertyMetadata(new object(), DataSourceChanged));
        /// <summary>
        /// CheckAndAssign
        /// </summary>
        /// <param name="dataFieldCheckBox"></param>
        /// <param name="sourceNew"></param>
        /// <param name="path"></param>
        private static void CheckAndAssign(DataFieldCheckBox dataFieldCheckBox, object sourceNew, string path)
        {
            Contract.Requires((dataFieldCheckBox != null) &&
                              (sourceNew != null) &&
                              !string.IsNullOrEmpty(path));

           
            var propValue = ComponentUtils.GetPropValue(sourceNew, path);
            int value = 0;
            if (propValue != null)
            {
                if (propValue is string)
                {
                    value = int.Parse(propValue as string);
                    dataFieldCheckBox.IsChecked = value != 0;
                }
                if (propValue.GetType().IsAssignableFrom(typeof(int)))
                {
                    // here we have a tinyint.
                    value = Convert.ToInt32(propValue);
                    dataFieldCheckBox.IsChecked = value == 0;
                   
                }
            }
        }

        /// <summary>
        /// CheckAndAssignText.
        /// </summary>
        /// <param name="dataAreaFiled"></param>
        /// <param name="sourceNew"></param>
        /// <param name="path"></param>
        private static void CheckAndAssignText(DataArea dataAreaFiled, object sourceNew, string path)
        {
            string propValue = ComponentUtils.GetPropValue(sourceNew, path) as string;
            if (!string.IsNullOrEmpty(propValue))
            {
                dataAreaFiled.EditorText.Text = propValue;
            }
        }
        /// <summary>
        /// CheckAndAssignDate
        /// </summary>
        /// <param name="dataDatePicker">DataDatePicker component</param>
        /// <param name="sourceNew">Object to be assigned</param>
        /// <param name="path">Path of the date</param>
        private static void CheckAndAssignDate(DataDatePicker dataDatePicker, object sourceNew, string path)
        {
            Contract.Requires((dataDatePicker != null) &&
                              (sourceNew != null) &&
                              !string.IsNullOrEmpty(path));

            if ((dataDatePicker == null) || 
                (sourceNew == null) ||   
                (string.IsNullOrEmpty(path)))
            {
                return;
            }
            var propValue = ComponentUtils.GetPropValue(sourceNew, path);
            DateTime timeValue = DateTime.Now;
            if (propValue != null)
            {

                if (propValue is string)
                {
                    try
                    {
                        timeValue = DateTime.Parse(propValue as string);
                    }
                    catch (Exception e)
                    {
                        timeValue = DateTime.Now;
                    }
                }
                else
                {
                    timeValue = (DateTime)propValue;

                }
                if (timeValue != null)
                {
                    dataDatePicker.DateContent = timeValue;
                }
                else
                {
                    dataDatePicker.DateContent = DateTime.Now;
                }
            }
        }
        private static void DataSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            object sourceNew = e.NewValue;
            string path = GetDataSourcePath(d);
            if (sourceNew == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            if (d is DataFieldCheckBox dataFieldCheckBox)
            {
                dataFieldCheckBox.DataObject = sourceNew;
                if (!string.IsNullOrEmpty(path))
                {
                    CheckAndAssign(dataFieldCheckBox, sourceNew, path);

                }
            }

            if(d is DataArea)
            {
                DataArea dataArea = (DataArea)d;
                CheckAndAssignText(dataArea, sourceNew, path);
                dataArea.DataSource = e.NewValue;
               // dataArea.DataSourcePath = path;
            }
            if (d is TextBox)
            {
                TextBox box = (TextBox) d;
                string propValue = ComponentUtils.GetPropValue(sourceNew, path) as string;
                if (propValue != null)
                {
                    box.Text = propValue;
                }
            }
            if (d is DataDatePicker dataPicker)
            {
                if (dataPicker != null)
                {
                    CheckAndAssignDate(dataPicker, sourceNew, path);
                }
            }

        }
        /// <summary>
        ///  Get DataSource 
        /// </summary>
        /// <param name="ds">Data Source</param>
        /// <returns></returns>
        
        public static object GetDataSource(DependencyObject ds)
        {
           return ds.GetValue(DataSourceDependencyProperty);
        }
        /// <summary>
        /// Set DataSource
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="item"></param>
        public static void SetDataSource(DependencyObject ds, object item)
        {
            ds.SetValue(DataSourceDependencyProperty, item);
        }

        /// <summary>
        /// DataSource Path
        /// </summary>
        /// <param name="dspm">Data Source Path</param>
        /// <param name="item">Item to set</param>
        public static void SetDataSourcePath(DependencyObject dspm, string item)
        {
            dspm.SetValue(DataSourcePathDependencyProperty, item);
        }
        /// <summary>
        /// Get the data source path.
        /// </summary>
        /// <param name="dps"></param>
        /// <returns></returns>
        public static string  GetDataSourcePath(DependencyObject dps)
        {
            return (string)dps.GetValue(DataSourcePathDependencyProperty);
        }
        
        /// <summary>
        ///  Data Source Path to be used.
        /// </summary>
        public static DependencyProperty DataSourcePathDependencyProperty
            = DependencyProperty.RegisterAttached(
                "DataSourcePath",
                typeof(string),
                typeof(ControlExt),
                new UIPropertyMetadata(string.Empty, DataSourcePathChange));

        private static void DataSourcePathChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            string path = e.NewValue as string ;
            object value = GetDataSource(d);
            ComponentFiller filler = new ComponentFiller();
            if (value != null)
            {
                if (value is DataTable)
                {
                    DataTable currentTable = value as DataTable;
                    var propValue = filler.FetchDataFieldObject(currentTable, path);
                    

                }
                else
                {
                    var propValue = ComponentUtils.GetPropValue(value, path);
                    if (propValue != null)
                    {
                        ComponentUtils.SetPropValue(value, path, propValue);
                    }
                }
            }
        }

        #endregion
        #region TableName
        /// <summary>
        ///  Dependency property associated with the name of the table.
        /// </summary>
        public static readonly DependencyProperty DbTableNameDependencyProperty =
            DependencyProperty.RegisterAttached(
                "TableName",
                typeof(string),
                typeof(ControlExt),
                new PropertyMetadata(string.Empty));

        /// <summary>
        ///  Set or Get the name of the table associated to this control.
        /// </summary>
        public string TableName
        {
            get { return (string) GetValue(DbTableNameDependencyProperty); }
            set { SetValue(DbTableNameDependencyProperty, value); }
        }
        #endregion
    }
}