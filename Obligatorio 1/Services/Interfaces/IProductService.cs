using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IProductService
    {
        void Add(Product p);
        void Delete(Guid pId);
        void Modify(Product p);
        Product Get(Guid id);
        void ChangeCategory(Guid id, Guid cId);
        void AddProductFeature(ProductFeature productFeature);
        void ModifyProductFeatureValue(Guid id, string val);
        List<Product> GetAllFromOrder(Order order);
        void RemoveFeatureFromProduct(Product p, Feature f);
    }
}
