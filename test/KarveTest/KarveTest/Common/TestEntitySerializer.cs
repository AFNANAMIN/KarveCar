﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using DataAccessLayer;
using DataAccessLayer.Crud;
using DataAccessLayer.DataObjects;
using DataAccessLayer.Logic;
using DataAccessLayer.Model;
using DataAccessLayer.SQL;
using KarveDapper.Extensions;
using KarveDataServices;
using KarveDataServices.DataTransferObject;
using KarveTest.DAL;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace KarveTest.Common
{
    [TestFixture]
    class TestEntitySerializer
    {
        private readonly TestBase _testBase = new TestBase();
        private readonly  QueryStoreFactory _queryStoreFactory = new QueryStoreFactory();
        private readonly IList<object> _entities = new List<object>() {
            new POBLACIONES(),
            new TIPOCLI(),
            new MERCADO(),
            new ZONAS(),
            new IDIOMAS(),
            new TARCREDI(),
            new CANAL(),
            new SUBLICEN(),
            new OFICINAS(),
            new USO_ALQUILER(),
            new ACTIVI(),
            new ClientSummaryDto(),
            new CliContactsPoco(),
            new FORMAS()
        };
        private readonly IList<object> _dto = new List<object>()
        {
            new CityDto(),
            new ClientTypeDto(),
            new MercadoDto(),
            new ClientZoneDto(),
            new LanguageDto(),
            new CreditCardDto(),
            new ChannelDto(),
            new CompanyDto(),
            new OfficeDtos(),
            new RentingUseDto(),
            new ActividadDto(),
            new ClientSummaryDto(),
            new ContactsDto(),
            new PaymentFormDto()
        };
        
        private string ValueToString(byte? b)
        {
            if (b.HasValue)
            {
                var value = (b == 0) ? "0" : "1";
                return value;
            }
            return string.Empty;
        }
        private IQueryStore CreateQueryStore(ClientDto clientPoco)
        {
            QueryStoreFactory factory = new QueryStoreFactory();
            var store = factory.GetQueryStore();
            store.AddParam(QueryType.QueryCity, clientPoco.CP);
            store.AddParam(QueryType.QueryClientType, clientPoco.TIPOCLI);
            store.AddParam(QueryType.QueryMarket, clientPoco.MERCADO);
            store.AddParam(QueryType.QueryZone, clientPoco.ZONA);
            store.AddParam(QueryType.QueryLanguage, ValueToString(clientPoco.IDIOMA));
            store.AddParam(QueryType.QueryCreditCard, clientPoco.TARTI);
            store.AddParam(QueryType.QueryChannel, clientPoco.CANAL);
            store.AddParam(QueryType.QueryCompany, clientPoco.SUBLICEN);
            store.AddParam(QueryType.QueryOffice, clientPoco.OFICINA);
            store.AddParam(QueryType.QueryRentingUse, ValueToString(clientPoco.USO_ALQUILER));
            store.AddParam(QueryType.QueryActivity, clientPoco.SECTOR);
            store.AddParam(QueryType.QueryClientSummary, clientPoco.CLIENTEFAC);
            store.AddParam(QueryType.QueryClientContacts, clientPoco.NUMERO_CLI);
            store.AddParam(QueryType.QueryPaymentForm, ValueToString(clientPoco.FPAGO));
            return store;
        }

        [Test]
        public async Task ShouldEntityDeserialize_Deserialize_AVehicleHelpersCorrectly()
        {

            var sqlExecutor = _testBase.SetupSqlQueryExecutor();
            var ds = new DataServiceImplementation(sqlExecutor);
            var vehicleDs = ds.GetVehicleDataServices();
            var vehicleSummary = await vehicleDs.GetAsyncVehicleSummary();
            var vehicleExt = vehicleSummary.FirstOrDefault();
            var entityMapper = new EntityMapper();
            if (vehicleExt != null)
            {
                var vehicle = await vehicleDs.GetVehicleDo(vehicleExt.Code);
                var entities = new List<object>
                {
                    new MARCAS(),
                    new PICTURES(),
                    new ACTIVEHI(),
                    new OwnerDto(),
                    new AgentDto(),
                    new MaintainanceDto(),
                    new COLORFL(),
                    new MODELO(),
                    new GRUPOS()
                };
                var dto = new List<object>()
                {
                    new BrandVehicleDto(),
                    new PictureDto(),
                    new ActividadDto(),
                    new OwnerDto(),
                    new AgentDto(),
                    new MaintainanceDto(),
                    new ColorDto(),
                    new ModelVehicleDto(),
                    new VehicleGroupDto()
                };
                var vehicleValue = vehicle.Value;
                var queryBuilder = _queryStoreFactory.GetQueryStore();
                queryBuilder.AddParam(QueryType.QueryBrandByVehicle, vehicleValue.CODIINT);
                queryBuilder.AddParam(QueryType.QueryVehiclePhoto, vehicleValue.CODIINT);
                queryBuilder.AddParam(QueryType.QueryVehicleActivity, vehicleValue.ACTIVIDAD);
                queryBuilder.AddParam(QueryType.QueryVehicleOwner, vehicleValue.PROPIE);
                queryBuilder.AddParam(QueryType.QueryAgentByVehicle, vehicleValue.AGENTE);
                queryBuilder.AddParam(QueryType.QueryVehicleMaintenance);
                queryBuilder.AddParam(QueryType.QueryVehicleColor, vehicleValue.COLOR);
                queryBuilder.AddParam(QueryType.QueryVehicleGroup, vehicleValue.GRUPO);
                var queryModel =
                    $"SELECT * FROM MODELO WHERE MARCA='{vehicleValue.MAR}' AND CODIGO='{vehicleValue.MO1}' AND VARIANTE='{vehicleValue.MO2}'";


                var auxQuery = queryBuilder.BuildQuery();
                auxQuery = auxQuery + queryModel + ";";
                using (var connection = sqlExecutor.OpenNewDbConnection())
                {
                    if (!string.IsNullOrEmpty(auxQuery))
                    {
                        var reader = await connection.QueryMultipleAsync(auxQuery);
                        if (reader != null)
                        {
                            var deserializer = new EntityDeserializer(entities, dto);
                            var mappedEntity = entityMapper.Map(reader, deserializer);
                            
                        }
                    }
                }
            }
        }

        [Test]
        public async Task ShouldEntityDeserialize_Deserialize_AVehicleGroupCorrectly()
        {

            var sqlExecutor = _testBase.SetupSqlQueryExecutor();
            var ds = new DataServiceImplementation(sqlExecutor);
            var vehicleDs = ds.GetVehicleDataServices();
            var vehicleSummary = await vehicleDs.GetAsyncVehicleSummary();
            var vehicleExt = vehicleSummary.FirstOrDefault();
            var entityMapper = new EntityMapper();
            if (vehicleExt != null)
            {
                var vehicle = await vehicleDs.GetVehicleDo(vehicleExt.Code);
                var entities = new List<object>
                {
                    new MODELO(),
                    new MARCAS(),
                    new GRUPOS()
                };
                var dto = new List<object>()
                {
                    new ModelVehicleDto(),
                    new BrandVehicleDto(),
                    new VehicleGroupDto()
                };
                var vehicleValue = vehicle.Value;
                var queryBuilder = _queryStoreFactory.GetQueryStore();
                queryBuilder.AddParam(QueryType.QueryBrandByVehicle, vehicleValue.CODIINT);
                queryBuilder.AddParam(QueryType.QueryVehicleGroup, vehicleValue.GRUPO);
                var queryModel =
                    $"SELECT * FROM MODELO WHERE MARCA='{vehicleValue.MAR}' AND CODIGO='{vehicleValue.MO1}' AND VARIANTE='{vehicleValue.MO2}';";


                var auxQuery = queryBuilder.BuildQuery();
                auxQuery = queryModel + auxQuery;
                using (var connection = sqlExecutor.OpenNewDbConnection())
                {
                    if (!string.IsNullOrEmpty(auxQuery))
                    {
                        var reader = await connection.QueryMultipleAsync(auxQuery);
                        if (reader != null)
                        {
                            var deserializer = new EntityDeserializer(entities, dto);
                            var mappedEntity = entityMapper.Map(reader, deserializer);
                            Assert.AreEqual(3, mappedEntity.Count);
                        }
                    }
                }

            }
        }


        [Test]
        public async Task ShouldEntityDeserialize_Deserialize_AClientCorrectly()
        {
            // arrange
            var executor = _testBase.SetupSqlQueryExecutor();
            var factory = CrudFactory.GetFactory(executor);
            var mapper = MapperField.GetMapper();
            SqlMapper.GridReader reader = null;
            using (var dbConnection = executor.OpenNewDbConnection())
            {
                var alldata = await factory.ClientLoader.LoadAsyncAll();
                var currentValue = alldata.FirstOrDefault();
                if (currentValue != null)
                {
                    var store = CreateQueryStore(currentValue);
                    var q = store.BuildQuery();
                    reader = await dbConnection.QueryMultipleAsync(q);

                }
            }
            if (reader != null)
            {
                var deserializer = new EntityDeserializer(_entities, _dto);
                var entityMapper = new EntityMapper();
                var mappedEntity = entityMapper.Map(reader, deserializer);
                if (mappedEntity != null)
                {
                    var poblaciones = deserializer.SelectDto<POBLACIONES,CityDto>(mapper, mappedEntity);
                    var clientType = deserializer.SelectDto<TIPOCLI, ClientTypeDto>(mapper, mappedEntity);
                    var market = deserializer.SelectDto<MERCADO, MercadoDto>(mapper, mappedEntity);
                    var zone = deserializer.SelectDto<ZONAS, ClientZoneDto>(mapper, mappedEntity);
                    var language = deserializer.SelectDto<IDIOMAS, LanguageDto>(mapper, mappedEntity);
                    var creditCard = deserializer.SelectDto<TARCREDI, CreditCardDto>(mapper, mappedEntity);
                    var channel = deserializer.SelectDto<CANAL, ChannelDto>(mapper, mappedEntity);
                    var office = deserializer.SelectDto<OFICINAS, OfficeDtos>(mapper, mappedEntity);
                    var rentUsage = deserializer.SelectDto<USO_ALQUILER, RentingUseDto>(mapper, mappedEntity);
                    var actividadDtos = deserializer.SelectDto<ACTIVI, ActividadDto>(mapper, mappedEntity);
                    var clientSummaryDtos = deserializer.SelectDto<ClientSummaryDto, ClientSummaryDto>(mapper, mappedEntity);
                    var contacts = deserializer.SelectDto<CliContactsPoco, ContactsDto>(mapper, mappedEntity);
                    var paymentForm = deserializer.SelectDto<FORMAS, PaymentFormDto>(mapper, mappedEntity);


                }
            } 
        }
    }
}
