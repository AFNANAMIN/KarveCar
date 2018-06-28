﻿using System;

namespace KarveDataServices.DataTransferObject
{
    /// <summary>
    /// Vehicle group dto. 
    /// </summary>
    public class VehicleGroupDto: BaseDto
    {
        /// <summary>
        ///  Set or get the CODIGO property.
        /// </summary>

        public string Codigo { get; set; }

        /// <summary>
        ///  Set or get the NOMBRE property.
        /// </summary>

        public string Nombre { get; set; }

        bool IsInvalid()
        {
            var errors = base.HasErrors;
            if (!errors)
            {
                if ((Nombre != null) && (Nombre.Length > 35))
                {
                    return true;
                }
            }
            return errors;
        }
        public override bool HasErrors { get => IsInvalid(); set => base.HasErrors = value; }

    }
}