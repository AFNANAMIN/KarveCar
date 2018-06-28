﻿using System;
namespace KarveDataServices.DataTransferObject
{
    /// <summary>
    ///  VehcileExtraDto.
    /// </summary>
    public class VehicleExtraDto : BaseDtoDefaultName
    {
        /// <summary>
        ///  Reference of the extra
        /// </summary>
        public string Reference { set; get; }
        /// <summary>
        ///  Vehicle Type of the extras.
        /// </summary>
        public string Notes { set; get; }
        public VehicleTypeDto VehicleType { set; get; }

        bool IsInvalid()
        {
            var errors = base.HasErrors;
            if (!errors)
            {
                if ((Notes != null) && (Notes.Length>200))
                {
                    return true;
                }
                if ((Reference != null) && (Reference.Length > 10))
                {
                    return true;
                }
            }
            return errors;
        }
        public override bool HasErrors { get => IsInvalid(); set => base.HasErrors = value; }
    }

}