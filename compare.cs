  
  ///Main Worker
  public static string EnumeratePropertyDifferences<T>(T source, T target)
        {
            try
            {
                var entity = typeof(T);
                var propertyInfos = entity.GetProperties().Where(y => !y.PropertyType.FullName.Contains("Collection") && !y.PropertyType.FullName.Contains(".Model")).ToList();
                var result = !propertyInfos.Any() ? null : (from pi in propertyInfos let value1 = entity.GetProperty(pi.Name).GetValue(source, null) let value2 = entity.GetProperty(pi.Name).GetValue(target, null) where !pi.Name.Equals("UpdatedOnUtc") && value1 != value2 && (value1 == null || !value1.Equals(value2)) select new { FieldName = pi.Name, From = value1, To = value2 });
                return JsonConvert.SerializeObject(result);
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }  


///Usage
internal void SaveChangedDetail(Product product)
        {
            var difference = EnumeratePropertyDifferences(_productRepository.TableNoTracking.FirstOrDefault(x => x.Id.Equals(product.Id)), product);
            var log = !(string.IsNullOrEmpty(difference) || difference.Equals("[]")) ? _logService.InsertLog(Core.Domain.Logging.LogLevel.Information, $"{CommonHelper.ChangedProductDetailHistoryPattern}{product.Id}", difference, _workContext.CurrentCustomer) : null;

        }
