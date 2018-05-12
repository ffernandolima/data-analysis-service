using DataAnalysis.Model.Enums;
using System.Collections.Generic;

namespace DataAnalysis.Common.Extensions
{
    public static class DataRowIdentifierExtensions
    {
        private static readonly IDictionary<DataRowIdentifier, DataRowKind> MAPPING;

        static DataRowIdentifierExtensions()
        {
            MAPPING = GenerateMapping();
        }

        public static DataRowKind GetDataRowKind(this DataRowIdentifier dataRowIdentifier)
        {
            return MAPPING[dataRowIdentifier];
        }

        private static Dictionary<DataRowIdentifier, DataRowKind> GenerateMapping()
        {
            var mapping = new Dictionary<DataRowIdentifier, DataRowKind>
            {
                { DataRowIdentifier.None,               DataRowKind.None     },
                                                        
                { DataRowIdentifier.Identifier001,      DataRowKind.Salesman },
                { DataRowIdentifier.Identifier002,      DataRowKind.Customer },
                { DataRowIdentifier.Identifier003,      DataRowKind.Sales    }
            };

            return mapping;
        }
    }
}
