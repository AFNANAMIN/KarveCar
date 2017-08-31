﻿using KarveCommon.Generic;
namespace KarveCommon.Services
{
    /// <summary>
    ///  This interface is for the global enviroment of the program.
    /// </summary>
    public interface IEnviromentVariables
    {
        /// <summary>
        ///  This gives us if the variable is present.
        /// </summary>
        /// <param name="config">Enviroment config</param>
        /// <param name="key">Key name</param>
        /// <returns></returns>
        bool HasKey(EnvironmentConfig config, string key);
        /// <summary>
        /// This check if a value is set for a given key.
        /// </summary>
        /// <param name="config">Enviroment config</param>
        /// <param name="key">Key name</param>
        /// <returns></returns>
        bool IsSet(EnvironmentConfig config, string key);
        /// <summary>
        /// This method set the key to the value.
        /// </summary>
        /// <param name="config">Enviroment config</param>
       /// <param name="key">Key Name</param>
        /// <param name="value">Key Value</param>
        void SetKey(EnvironmentConfig config, string key, object value);
        /// <summary>
        /// This method remove the key form the enviroment.
        /// </summary>
        /// <param name="config">Config</param>
        /// <param name="key">Key Value</param>
        void EmptyKey(EnvironmentConfig config, string key);
        /// <summary>
        ///  This returns the key that it is available.
        /// </summary>
        /// <param name="config">Enviroment config</param>
        /// <param name="key">Key Name</param>
        /// <returns></returns>
        object GetKey(EnvironmentConfig config, string key);
        /// <summary>
        /// This unset a value in the configuration enviroment.
        /// </summary>
        /// <param name="config">Enviroment config</param>
        /// <param name="key">Key Name</param>
        void UnSet(EnvironmentConfig config, string key);
        bool IsSetNotEmpty(EnvironmentConfig karveConfiguration, string v);
    }
}