using InventoryMvc.Models;
using InventoryMvc.ViewModels;

namespace InventoryMvc.Mappers;

public  static class SupplierMapper
{
    public static Supplier ToSupplier(this AddSupplierViewModel supplier)
    {
        return new Supplier
        {
            Id = supplier.Id,
            Address = supplier.Address,
            City = supplier.City,
            Country = supplier.Country,
            ContactPerson = supplier.ContactPerson,
            CreateDate = supplier.CreateDate,
            Email = supplier.Email,
            IsActive = supplier.IsActive,
            PaymentTerms = supplier.PaymentTerms,
            Phone = supplier.Phone,
            PostalCode = supplier.PostalCode,
            State   = supplier.State,
            SupplierName = supplier.SupplierName,
            TaxNumber = supplier.TaxNumber
        };
    }


    public static AddSupplierViewModel ToSupplierViewModel(this Supplier supplier)
    {
        return new AddSupplierViewModel
        {
            Id = supplier.Id,
            Address = supplier.Address,
            City = supplier.City,
            Country = supplier.Country,
            ContactPerson = supplier.ContactPerson,
            CreateDate = supplier.CreateDate,
            Email = supplier.Email,
            IsActive = supplier.IsActive,
            PaymentTerms = supplier.PaymentTerms,
            Phone = supplier.Phone,
            PostalCode = supplier.PostalCode,
            State = supplier.State,
            SupplierName = supplier.SupplierName,
            TaxNumber = supplier.TaxNumber
        };
    }

    public static ReadSupplierViewModel ToReadSupplierViewModel(this Supplier supplier) {
        return new ReadSupplierViewModel
        {
            Id = supplier.Id,
            Address = supplier.Address,
            City = supplier.City,
            Country = supplier.Country,
            ContactPerson = supplier.ContactPerson,
            Email = supplier.Email,
            IsActive = supplier.IsActive,
            PaymentTerms = supplier.PaymentTerms,
            Phone = supplier.Phone,
            PostalCode = supplier.PostalCode,
            State = supplier.State,
            SupplierName = supplier.SupplierName,
            TaxNumber = supplier.TaxNumber
        };
    }
}
