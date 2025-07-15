using CommonLib.Model;
using NetahsilatWebServiceLib.ErpWebService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetahsilatWebServiceLib
{
    public class DynamicValuesHelper
    {
        public static DynamicFieldsModel GetAllDynamicFieldValues(PaymentServiceModel paymentServiceModel)
        {
            DynamicFieldsModel dynamicFieldsModel = new DynamicFieldsModel();

            dynamicFieldsModel.AuthCode = GetDynamicValue("authcode", paymentServiceModel);
            dynamicFieldsModel.ContractNumber = GetDynamicValue("kontratno", paymentServiceModel);
            dynamicFieldsModel.ProjectCode = GetDynamicValue("projectcode", paymentServiceModel);
            dynamicFieldsModel.Salesman = GetDynamicValue("salesman", paymentServiceModel);
            dynamicFieldsModel.DivisionCode = GetDynamicValue("divisioncode", paymentServiceModel);
            dynamicFieldsModel.DepartmentCode = GetDynamicValue("departmentcode", paymentServiceModel);
            dynamicFieldsModel.ReserveNumber = GetDynamicValue("rezervno", paymentServiceModel);
            dynamicFieldsModel.SpecialCode = GetDynamicValue("specode", paymentServiceModel);

            return dynamicFieldsModel;
        }

        public static DynamicFieldsModel GetAllDynamicFieldValues(ReversalServiceModel reversalServiceModel)
        {
            DynamicFieldsModel dynamicFieldsModel = new DynamicFieldsModel();

            dynamicFieldsModel.AuthCode = GetDynamicValue("authcode", reversalServiceModel);
            dynamicFieldsModel.ContractNumber = GetDynamicValue("kontratno", reversalServiceModel);
            dynamicFieldsModel.ProjectCode = GetDynamicValue("projectcode", reversalServiceModel);
            dynamicFieldsModel.Salesman = GetDynamicValue("salesman", reversalServiceModel);
            dynamicFieldsModel.DivisionCode = GetDynamicValue("divisioncode", reversalServiceModel);
            dynamicFieldsModel.DepartmentCode = GetDynamicValue("departmentcode", reversalServiceModel);
            dynamicFieldsModel.ReserveNumber = GetDynamicValue("rezervno", reversalServiceModel);
            dynamicFieldsModel.SpecialCode = GetDynamicValue("specode", reversalServiceModel);

            return dynamicFieldsModel;
        }

        public static string GetDynamicValue(string dynamicFieldName, PaymentServiceModel paymentServiceModel)
        {
            var dynamicFieldValue = String.Empty;

            if (paymentServiceModel != null)
            {
                if (!String.IsNullOrWhiteSpace(dynamicFieldName))
                {
                    if (paymentServiceModel.DynamicDataCollection != null && paymentServiceModel.DynamicDataCollection.Any())
                    {
                        dynamicFieldValue = GetDynamicValue(dynamicFieldName, paymentServiceModel.DynamicDataCollection);
                    }
                    if (paymentServiceModel.Agent != null && String.IsNullOrWhiteSpace(dynamicFieldValue))
                    {
                        if (paymentServiceModel.Agent.DynamicDataCollection != null && paymentServiceModel.Agent.DynamicDataCollection.Any())
                        {
                            dynamicFieldValue = GetDynamicValue(dynamicFieldName, paymentServiceModel.Agent.DynamicDataCollection);
                        }
                    }
                }
            }

            return dynamicFieldValue;
        }

        public static string GetDynamicValue(string dynamicFieldName, ReversalServiceModel reversalServiceModel)
        {
            var dynamicFieldValue = String.Empty;

            if (reversalServiceModel != null)
            {
                if (!String.IsNullOrWhiteSpace(dynamicFieldName))
                {
                    if (reversalServiceModel.Payment.DynamicDataCollection != null && reversalServiceModel.Payment.DynamicDataCollection.Any())
                    {
                        dynamicFieldValue = GetDynamicValue(dynamicFieldName, reversalServiceModel.Payment.DynamicDataCollection);
                    }

                    if (reversalServiceModel.Agent != null && String.IsNullOrWhiteSpace(dynamicFieldValue))
                    {
                        if (reversalServiceModel.Agent.DynamicDataCollection != null && reversalServiceModel.Agent.DynamicDataCollection.Any())
                        {
                            dynamicFieldValue = GetDynamicValue(dynamicFieldName, reversalServiceModel.Agent.DynamicDataCollection);
                        }
                    }
                }
            }

            return dynamicFieldValue;
        }

        private static string GetDynamicValue(string dynamicFieldName, DynamicFieldCollection[] dynamicFields)
        {
            try
            {
                if (dynamicFields != null && dynamicFields.Any())
                {
                    var dynamicData = dynamicFields.FirstOrDefault(x =>
                    x.IntegrationId == dynamicFieldName
                    && (!String.IsNullOrEmpty(x.Value) || !String.IsNullOrEmpty(x.IntegrationValues?.FirstOrDefault()?.Value)));

                    if (dynamicData != null)
                    {
                        if (dynamicData.IntegrationValues != null && dynamicData.IntegrationValues.Any())
                        {
                            return dynamicData.IntegrationValues.FirstOrDefault().Value;
                        }
                        else if (dynamicData.Values != null && dynamicData.Values.Any())
                        {
                            return dynamicData.Values.FirstOrDefault().Value?.Split('-')[0].Trim();
                        }
                        else if (!String.IsNullOrEmpty(dynamicData.Value))
                        {
                            return dynamicData.Value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
               Logging.AddLog($"Hata: '{dynamicFieldName}' isimli dinamik alanın değeri okunurken hata alındı. Hata Detayı: {ex.Message}");
            }

            return String.Empty;
        }
    }
}