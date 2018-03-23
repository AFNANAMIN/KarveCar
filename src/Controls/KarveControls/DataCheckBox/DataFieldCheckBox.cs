﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using KarveControls.Generic;
using System.Windows.Input;

namespace KarveControls
{
    /// <summary>
    ///  Represent a control that a user can select and clear.
    /// </summary>
    public class DataFieldCheckBox: CheckBox
    {
        private bool? _isChecked;
        private bool? _previous;
        private bool _isChanged;
        static DataFieldCheckBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DataFieldCheckBox), 
                                                           new FrameworkPropertyMetadata(typeof(DataFieldCheckBox)));
        }
        /// <summary>
        /// Data object properties.
        /// </summary>
        public static DependencyProperty DataObjectDependencyProperty =
            DependencyProperty.Register(
                "DataObject",
                typeof(object),
                typeof(DataFieldCheckBox));

        
       

        /// <summary>
        /// Data object dependency property. It get/set an object in the component.
        /// </summary>
        public object DataObject
        {
            set
            {
                SetValue(DataObjectDependencyProperty, value);
            }
            get { return (object) GetValue(DataObjectDependencyProperty); }
        }
        /// <summary>
        ///  default checkbox.
        /// </summary>
        public DataFieldCheckBox() : base()
        {
             this.LostFocus+=OnLostFocus;
            this.Checked+=OnChecked;
            this.Unchecked+=OnUnchecked;
         
            _isChecked = IsChecked;
            _previous = IsChecked;
            _isChanged = false;
        }

        private void OnUnchecked(object sender, RoutedEventArgs routedEventArgs)
        {
            DataFieldCheckBox checkBox = sender as DataFieldCheckBox;
            var currentChecked = false;
            if (checkBox != null)
            {
                _previous = _isChecked;
                if (checkBox.IsChecked != null)
                {
                    if ( currentChecked!= _isChecked)
                    {
                        _isChanged = true;
                    }
                    else
                    {
                        _isChanged = false;
                    }
                    _isChecked = checkBox.IsChecked.Value;
                    
                }
            }
            SendEventOnChanged();
        }

        private void OnChecked(object sender, RoutedEventArgs routedEventArgs)
        {
            DataFieldCheckBox checkBox = sender as DataFieldCheckBox;
            var currentChecked = true;
            if (checkBox != null)
            {
                _previous = _isChecked;

                if (checkBox.IsChecked != null)
                {
                    if (currentChecked != _isChecked)
                    {
                        _isChanged = true;
                    }
                    else
                    {
                        _isChanged = false;
                    }
                    _isChecked = checkBox.IsChecked.Value;

                }
                

            }
           SendEventOnChanged();
        }

        private void SendEventOnChanged()
        {
            
                // ok we can raise the event.
                var path = ControlExt.GetDataSourcePath(this);
                if (path != null)
                {
                    var tmp = ControlExt.GetDataSource(this);
                    if (tmp != null)
                    {
                     //   ComponentUtils.SetPropValue(tmp, path, _isChecked);
                      //  DataObject = tmp;
                    }
                }
                IDictionary<string, object> values = new Dictionary<string, object>();
                values.Add("DataObject", DataObject);
                values.Add("ChangedValue", _isChecked);
                values.Add("PreviousValue", _previous);
                values.Add("FieldChanged", path);
            if (Command != null)
            {
                Command.Execute(values);
            }
            else
            {
                DataFieldCheckBoxEventArgs args = new DataFieldCheckBoxEventArgs(DataFieldCheckBoxChangedEvent);
                args.ChangedValuesObjects = values;
                RaiseEvent(args);
            }
        }
        // in case it is changed.
        private void OnLostFocus(object sender, RoutedEventArgs routedEventArgs)
        {
         
            
        }

        
        /// <summary>
        ///  Apply template.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        /// <summary>
        ///  Event launched when the checkbox change.
        /// </summary>
        public static RoutedEvent DataFieldCheckBoxChangedEvent = EventManager.RegisterRoutedEvent("DataFieldCheckBoxChanged", 
            RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DataFieldCheckBox));
        /// <summary>
        /// This the event when it changes the checkbox.
        /// </summary>
        public event RoutedEventHandler
            DataFieldCheckBoxChanged
            {
                add
                {
                    AddHandler(DataFieldCheckBoxChangedEvent, value);
                }
                remove
                {
                    RemoveHandler(DataFieldCheckBoxChangedEvent, value);
                }
            }

    }
}
