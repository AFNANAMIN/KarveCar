﻿using KarveCar.Model.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using KarveCommon.Generic;
using static KarveCar.Model.Generic.RecopilatorioCollections;
using static KarveCar.Model.Generic.RecopilatorioEnumerations;

namespace KarveCar.Utility
{
    public class ValidationRuleDataGrid : ValidationRule
    {
        /// <summary>
        /// It validates that the values are not empty.
        /// </summary>
        /// <param name="value">DataGrid entry to be validated</param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {


            if (value == null)
            {
                return new ValidationResult(false, MessageBox.Show(string.Concat("Error en la edición de datos. Los motivos pueden ser los siguientes:",
                    "\n-No se admite un valor vacío"), "Error de edición", MessageBoxButton.OK, MessageBoxImage.Error));
            }
            object obj = (value as BindingGroup).Items[0] as object;

            if (ValidateNotNullOrEmpty(obj))
            {
                // TODO: this string shall be inserted in the resx manager
                return new ValidationResult(false, MessageBox.Show(string.Concat("Error en la edición de datos. Los motivos pueden ser los siguientes:",
                                                                                "\n-No se admite un valor vacío"), "Error de edición", MessageBoxButton.OK, MessageBoxImage.Error));
            }
            if (ValidateDuplicateValue(obj))
            {
                // TODO: this string shall be inserted in the resx manager

                return new ValidationResult(false, MessageBox.Show(string.Concat("Error en la edición de datos. Los motivos pueden ser los siguientes:",
                                                                                "\n-No se admite un valor repetido"), "Error de edición", MessageBoxButton.OK, MessageBoxImage.Error));
            }

            return ValidationResult.ValidResult;

        }

        /// <summary>
        /// This function validates that the values are not empty and insert the value inside the grid
        /// </summary>
        /// <param name="obj">Object to be inserted</param>
        /// <returns></returns>
        private bool ValidateNotNullOrEmpty(object obj)
        {
            bool result = false;
            //Se recuperan las properties del object pasado por params
            var objproperties = ManageGenericObject.GetProperties(obj);


            foreach (var objprop in objproperties)
            {   //Para cada property del object, se recupera su valor
                object objvalue = ManageGenericObject.PropertyGetValue(obj, objprop.Name.ToString());

                //Se comprueba que no sea nulo o esté vacío
                if (objvalue == null)
                {
                    result = true;
                    break;
                }
                if (objvalue.ToString().Length == 0)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Valida que no hayan valores repetidos cuando se modifica o inserta un nuevo Row en el DataGrid
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool ValidateDuplicateValue(object obj)
        {
            bool result = false;

            try
            {   //Se recupera la EOpcion del TabItem activo
                EOpcion opcion = TabControlAndTabItemUtil.TabControlSelectedItemEOpcion();

                //Se recupera la GenericObservableCollection del TabItem activo
                GenericObservableCollection auxobscollection = tabitemdictionary.Where(g => g.Key == opcion).FirstOrDefault().Value.GenericObsCollection;

                //Se crea un SortedSet donde se guardarán los values para cada property de la GenericObservableCollection, 
                //y así poderlos comparar con el value del object pasado por params para esa property
                ISet<string> collectionTemp = new SortedSet<string>();


                foreach (var itemStringItem in auxobscollection.GenericObsCollection)
                {   //Se recorre la GenericObservableCollection, y cada object se convierte en un string
                    //como el ejemplo: [key1, value1];[ke2, value2]...
                    string stringItem = ManageGenericObject.PropertyConvertToDictionary(itemStringItem);
                    //Se hace un split del string con los object de la GenericObservableCollection, y se
                    //guardan los valores de una misma propiedad en una IList
                    IList<string> tempString = stringItem.Split(';').ToList();
                    
                    //Se recorren los valores para cada IList que corresponde a cada propiedad del object,
                    //y se comprueba que no haya valores repetidos
                    foreach (string itemTempString in tempString)
                    { 
                        // We have to check that the code is not duplicated.
                        if (itemTempString.Contains("CODE"))
                        {
                            if (!collectionTemp.Contains(itemTempString))
                            {
                                collectionTemp.Add(itemTempString);
                            }
                            else
                            {
                                result = true;
                                break;
                            }
                        }
                    }
                    if (result)
                    {
                        break;
                    }
                }

            }
            catch (Exception) { }
            return result;
        }
    }
}
