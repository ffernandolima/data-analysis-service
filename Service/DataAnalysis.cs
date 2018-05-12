using DataAnalysis.Common.Extensions;
using DataAnalysis.Common.General;
using DataAnalysis.Framework.Extensions;
using DataAnalysis.Framework.Stream;
using DataAnalysis.Model;
using DataAnalysis.Model.Enums;
using DataAnalysis.Service.Configuration;
using DataAnalysis.Service.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text;

namespace DataAnalysis.Service
{
    public class DataAnalysis
    {
        private readonly Settings _settings;
        private readonly MemoryCache _cache;
        private readonly DirectoryInfo _inputDirectory;
        private FlatFile _flatFile;

        public DataAnalysis(Settings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings", "Settings cannot be null.");
            }

            this._settings = settings;
            this._cache = MemoryCache.Default;
            this._inputDirectory = new DirectoryInfo(this._settings.DefaultInputDirectory);

            if (!this._inputDirectory.Exists)
            {
                throw new ArgumentNullException("Default input directory doesn't exist.");
            }

            this._flatFile = new FlatFile();
        }

        public bool Process()
        {
            var datFiles = this._inputDirectory.GetFiles(string.Format("*{0}", Constants.ALLOWED_EXTENSION), SearchOption.TopDirectoryOnly)
                                               .Where(x => x != null && x.Exists)
                                               .ToList();

            var result = this.InternalProcess(datFiles);
            return result;
        }

        private string GenerateCacheKey(FileInfo fileInfo)
        {
            var keyStr = string.Format("{0}|{1}|{2}", fileInfo.FullName, fileInfo.LastWriteTimeUtc.ToBinary(), fileInfo.Length);
            return keyStr.GetHashCode().ToString();
        }

        private bool InternalProcess(List<FileInfo> datFiles)
        {
            if (!datFiles.Any())
            {
                return false;
            }

            foreach (var datFile in datFiles)
            {
                var cacheKey = this.GenerateCacheKey(datFile);

                if (this._cache.Contains(cacheKey))
                {
                    continue;
                }

                this._flatFile = (this._flatFile.HasData() ? new FlatFile() : this._flatFile);

                using (var streamReader = new DataAnalysisStreamReader(datFile))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            continue;
                        }

                        var identifier = line.Substring(0, Constants.LENGTH);
                        var dataRowIdentifier = EnumExtensions.GetValueFromDescription<DataRowIdentifier>(identifier);
                        var dataRowKind = dataRowIdentifier.GetDataRowKind();

                        if (dataRowKind == DataRowKind.None)
                        {
                            throw new Exception("Unknown row kind.");
                        }

                        this.ProcessLine(line, dataRowKind);
                    }
                }

                this.GenerateReport(datFile);
                this.AddToCache(cacheKey);
            }

            return true;
        }

        private void ProcessLine(string line, DataRowKind dataRowKind)
        {
            var row = line.Split(new[] { Constants.ROW_SEPARATOR }, StringSplitOptions.None);

            switch (dataRowKind)
            {
                case DataRowKind.Salesman:

                    var salesman = new Salesman
                    {
                        Cpf = row.Get<string>(SalesmanColumns.Cpf),
                        Name = row.Get<string>(SalesmanColumns.Name),
                        Salary = row.Get<decimal>(SalesmanColumns.Salary)
                    };

                    if (!this._flatFile.Salesmans.Any(x => x.Cpf.Equals(salesman.Cpf, StringComparison.OrdinalIgnoreCase)))
                    {
                        this._flatFile.Salesmans.Add(salesman);
                    }

                    break;

                case DataRowKind.Customer:

                    var customer = new Customer
                    {
                        Cnpj = row.Get<string>(CustomerColumns.Cnpj),
                        Name = row.Get<string>(CustomerColumns.Name),
                        BusinessArea = row.Get<string>(CustomerColumns.BusinessArea)
                    };

                    if (!this._flatFile.Customers.Any(x => x.Cnpj.Equals(customer.Cnpj, StringComparison.OrdinalIgnoreCase)))
                    {
                        this._flatFile.Customers.Add(customer);
                    }

                    break;

                case DataRowKind.Sales:

                    var sale = new Sale
                    {
                        SaleId = row.Get<int>(SaleColumns.SaleId),
                        SalesmanName = row.Get<string>(SaleColumns.SalesmanName)
                    };

                    var saleItems = row.Get<string>(SaleColumns.SaleItens);

                    var sb = new StringBuilder(saleItems);

                    sb = sb.Replace(Constants.LEFT_BRACKET, string.Empty)
                           .Replace(Constants.RIGHT_BRACKET, string.Empty);

                    sb = sb.Trim();

                    saleItems = sb.ToString();

                    var items = saleItems.Split(new[] { Constants.ITEM_SEPARATOR }, StringSplitOptions.None);

                    foreach (var item in items)
                    {
                        var values = item.Split(new[] { Constants.ITEM_VALUES_SEPARATOR }, StringSplitOptions.None);

                        var saleItem = new SaleItem
                        {
                            SaleItemId = values.Get<int>(SaleItemColumns.SaleItemId),
                            Quantity = values.Get<int>(SaleItemColumns.Quantity),
                            Price = values.Get<decimal>(SaleItemColumns.Price),
                            SaleId = sale.SaleId
                        };

                        sale.SaleItems.Add(saleItem);
                    }

                    var foundSalesman = this._flatFile.Salesmans.Distinct().SingleOrDefault(x => x.Name.Equals(sale.SalesmanName, StringComparison.OrdinalIgnoreCase));

                    if (foundSalesman != null)
                    {
                        foundSalesman.Sales.Add(sale);
                    }

                    this._flatFile.Sales.Add(sale);
                    break;
            }
        }

        private void GenerateReport(FileSystemInfo datFile)
        {
            var path = this._settings.DefaultOutputDirectory + string.Format(@"\{0}.done.dat", datFile.Name.Replace(datFile.Extension, string.Empty));

            using (var writer = new DataAnalysisStreamWriter(path))
            {
                writer.WriteLine(string.Format("Amount of clients in the input file: {0}", this._flatFile.GetAmountOfclients()));
                writer.WriteLine(string.Format("Amount of salesman in the input file: {0}", this._flatFile.GetAmountOfSalesman()));
                writer.WriteLine(string.Format("ID of the most expensive sale: {0}", this._flatFile.GetMostExpensiveSaleId()));
                writer.WriteLine(string.Format("Worst salesman ever: {0}", this._flatFile.GetWorstSalesmanName()));
            }
        }

        private void AddToCache(string cacheKey)
        {
            var itemPolicy = new CacheItemPolicy
            {
                SlidingExpiration = TimeSpan.Zero,
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddDays(1)
            };

            this._cache.Add(cacheKey, DateTime.UtcNow, itemPolicy);
        }
    }
}
